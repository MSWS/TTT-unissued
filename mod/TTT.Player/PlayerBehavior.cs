using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using TTT.Public.Behaviors;
using TTT.Public.Mod.Role;
using TTT.Public.Player;
using TTT.Round;

namespace TTT.Player;

public class PlayerBehavior(PlayerConfig config) : IPlayerService, IPluginBehavior
{
    
    public void Start(BasePlugin plugin)
    {
        
    }
    
    [GameEventHandler]
    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event.Userid == null) throw new NullReferenceException("Could not find player object when connecting.");
        CreatePlayer(@event.Userid);
        return HookResult.Continue;
    }
    
    private readonly Dictionary<CCSPlayerController, GamePlayer> _players = [];
    
    public Dictionary<CCSPlayerController, GamePlayer> GetPlayers()
    {
        return _players;
    }
    
    public void CreatePlayer(CCSPlayerController player)
    {
        if (_players.ContainsKey(player)) return;
        _players.Add(player, new GamePlayer(Role.Unassigned, config.StartCredits, config.StartKarma, player.UserId.Value));
    }
    
    public void AddKarma(CCSPlayerController player, int karma)
    {
        if (!_players.TryGetValue(player, out var value)) return;
        if (karma < 0) return;
        
        if (karma + value.Karma() > config.MaxKarma)
            value.SetKarma(config.MaxKarma);
        else
            value.AddKarma(karma);
    }
    
    public void RemoveKarma(CCSPlayerController player, int karma)
    {
        if (!_players.TryGetValue(player, out var value)) return;
        if (karma < 0) return;
        
        if (value.Karma() - karma < config.MinKarma)
            value.SetKarma(config.MinKarma);
        else
            value.RemoveKarma(karma);
    }

    public List<GamePlayer> Players()
    {
        return _players.Values.ToList();
    }

    public GamePlayer GetPlayer(CCSPlayerController player)
    {
        return _players[player];
    }

    public void RemovePlayer(CCSPlayerController player)
    {
        _players.Remove(player);
    }

    public void Clr()
    {
        foreach (var player in Players())
        {
            player.SetKiller(null);
            player.SetPlayerRole(Role.Unassigned);
            player.ResetCredits();
            player.ModifyKarma();
            player.SetFound(false);
            player.SetDead();
        }
    }
}