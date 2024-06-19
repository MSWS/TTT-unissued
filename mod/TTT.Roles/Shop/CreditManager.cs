using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using TTT.Public.Extensions;
using TTT.Public.Mod.Role;
using TTT.Public.Player;

namespace TTT.Roles.Shop;

public class CreditManager
{
    private readonly IPlayerService _playerService;

    private CreditManager(BasePlugin plugin, IPlayerService playerService)
    {
        _playerService = playerService;
        plugin.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }

    public static void Register(BasePlugin parent, IPlayerService service)
    {
        new CreditManager(parent, service);
    }

    [GameEventHandler]
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var victim = @event.Userid;
        
        if (attacker == null || victim == null) return HookResult.Continue;
        if (attacker == victim) return HookResult.Continue;
        
        var attackerPlayer = _playerService.GetPlayer(attacker);
        var victimPlayer = _playerService.GetPlayer(victim);
        
        if (attackerPlayer.PlayerRole() == Role.Traitor && victimPlayer.PlayerRole() != Role.Traitor)
        {
            attackerPlayer.AddCredits(250);
            return HookResult.Continue;
        }
        
        if (attackerPlayer.PlayerRole() != Role.Traitor && victimPlayer.PlayerRole() == Role.Traitor)
        {
            attackerPlayer.AddCredits(250);
            return HookResult.Continue;
        }
        
        attackerPlayer.RemoveCredits(100);
        
        return HookResult.Continue;
    }

    
}