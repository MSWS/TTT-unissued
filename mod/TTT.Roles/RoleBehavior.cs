using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.Player;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;
using TTT.Public.Mod.Round;
using TTT.Public.Player;
using TTT.Round;

namespace TTT.Roles;

public class RoleBehavior(IPlayerService service, RoleConfig config) : IRoleService, IPluginBehavior
{
    private const int MaxDetectives = 3;

    private int _innocentsLeft;
    private IRoundService _roundService;
    private int _traitorsLeft;

    public void Start(BasePlugin parent)
    {
        ModelHandler.RegisterListener(parent);
       
        /*
        parent.RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
        parent.RegisterEventHandler<EventRoundFreezeEnd>(OnRoundStart);
        parent.RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
        parent.RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        parent.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath, HookMode.Pre);
        parent.RegisterEventHandler<EventGameStart>(OnMapStart);
        */
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundFreezeEnd @event, GameEventInfo info)
    {
        _roundService.SetRoundStatus(RoundStatus.Waiting);
        foreach (var player in Utilities.GetPlayers().Where(player =>
                     player.IsReal() && player.Team != CsTeam.None || player.Team != CsTeam.Spectator))
        {
            player.RemoveWeapons();
            if (!string.IsNullOrEmpty(config.SecondaryWeapon))
                player.GiveNamedItem(config.SecondaryWeapon);
            
            if (!string.IsNullOrEmpty(config.PrimaryWeapon))
                player.GiveNamedItem(config.PrimaryWeapon);
            
            player.GiveNamedItem("weapon_glock");
            service.GetPlayer(player).ModifyKarma();
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (Utilities.GetPlayers().Count(player =>
                player.IsReal() && player.Team != CsTeam.None || player.Team == CsTeam.Spectator) < 3)
        {
            _roundService.ForceEnd();
        }
        
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        info.DontBroadcast = true;

        var playerWhoWasDamaged = @event.Userid;
        var attacker = @event.Attacker;

        if (playerWhoWasDamaged == null) return HookResult.Continue;

        playerWhoWasDamaged.ModifyScoreBoard();

        service.GetPlayer(playerWhoWasDamaged).SetKiller(attacker);
        
        if (IsTraitor(playerWhoWasDamaged)) _traitorsLeft--;

        if (IsDetective(playerWhoWasDamaged) || IsInnocent(playerWhoWasDamaged)) _innocentsLeft--;

        if (_traitorsLeft == 0 || _innocentsLeft == 0) Server.NextFrame(() => _roundService.ForceEnd());

        Server.NextFrame(() => playerWhoWasDamaged.CommitSuicide(false, true));

        Server.NextFrame(() =>
        {
            Server.PrintToChatAll(
                StringUtils.FormatTTT($"{GetRole(playerWhoWasDamaged).FormatStringFullAfter(" has been found.")}"));

            if (attacker == playerWhoWasDamaged || attacker == null) return;

            attacker.ModifyScoreBoard();

            playerWhoWasDamaged.PrintToChat(StringUtils.FormatTTT(
                $"You were killed by {GetRole(attacker).FormatStringFullAfter(" " + attacker.PlayerName)}."));
            attacker.PrintToChat(StringUtils.FormatTTT(
                $"You killed {GetRole(playerWhoWasDamaged).FormatStringFullAfter(" " + playerWhoWasDamaged.PlayerName)}."));
        });

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        var players = Utilities.GetPlayers()
            .Where(player => player.IsValid).Where(player => player.IsReal()).ToList();

        foreach (var player in players) player.PrintToCenter(GetWinner().FormatStringFullAfter("s has won!"));

        Server.NextFrame(Clear);

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var player = @event.Userid;
        Server.NextFrame(() =>
        {
            service.RemovePlayer(player);
            if (service.Players().Count == 0) _roundService.SetRoundStatus(RoundStatus.Paused);
        });

        return HookResult.Continue;
    }

    public void AddRoles()
    {
        var eligible = Utilities.GetPlayers()
            .Where(player => player.IsReal())
            .Where(player => player.Team is not (CsTeam.Spectator or CsTeam.None))
            .ToList();

        var traitorCount = (int)Math.Floor(Convert.ToDouble(eligible.Count / config.TraitorRatio));
        var detectiveCount = (int)Math.Floor(Convert.ToDouble(eligible.Count / config.DetectiveRatio));

        _traitorsLeft = traitorCount;
        _innocentsLeft = eligible.Count - traitorCount;

        if (detectiveCount > MaxDetectives) detectiveCount = MaxDetectives;

        for (var i = 0; i < traitorCount; i++)
        {
            var chosen = eligible[Random.Shared.Next(eligible.Count)];
            eligible.Remove(chosen);
            AddTraitor(chosen);
        }

        for (var i = 0; i < detectiveCount; i++)
        {
            var chosen = eligible[Random.Shared.Next(eligible.Count)];
            eligible.Remove(chosen);
            AddDetective(chosen);
        }

        AddInnocents(eligible);
    }

    public ISet<CCSPlayerController> GetTraitors()
    {
        return service.Players().Where(player => player.PlayerRole() == Role.Traitor).Select(player => player.Player())
            .ToHashSet();
    }

    public ISet<CCSPlayerController> GetDetectives()
    {
        return service.Players().Where(player => player.PlayerRole() == Role.Detective).Select(player => player.Player())
            .ToHashSet();
    }

    public ISet<CCSPlayerController> GetInnocents()
    {
        return service.Players().Where(player => player.PlayerRole() == Role.Innocent).Select(player => player.Player())
            .ToHashSet();
    }


    public Role GetRole(CCSPlayerController player)
    {
        return service.GetPlayer(player).PlayerRole();
    }

    public void AddTraitor(CCSPlayerController player)
    {
        service.GetPlayer(player).SetPlayerRole(Role.Traitor);
        player.SwitchTeam(CsTeam.Terrorist);
        player.PrintToCenter(Role.Traitor.FormatStringFullBefore("You are now a(n)"));
        player.PrintToChat(Role.Traitor.FormatStringFullBefore("You are now a(n)"));
        ModelHandler.SetModelNextServerFrame(player, ModelHandler.ModelPathTmPhoenix);
    }

    public void AddDetective(CCSPlayerController player)
    {
        service.GetPlayer(player).SetPlayerRole(Role.Detective);
        player.SwitchTeam(CsTeam.CounterTerrorist);
        player.PrintToCenter(Role.Detective.FormatStringFullBefore("You are now a(n)"));
        player.GiveNamedItem(CsItem.Taser);
        ModelHandler.SetModelNextServerFrame(player, ModelHandler.ModelPathCtmSas);
    }

    public void AddInnocents(IEnumerable<CCSPlayerController> players)
    {
        foreach (var player in players)
        {
            service.GetPlayer(player).SetPlayerRole(Role.Innocent);
            player.PrintToCenter(Role.Innocent.FormatStringFullBefore("You are now an"));
            player.SwitchTeam(CsTeam.Terrorist);
            ModelHandler.SetModelNextServerFrame(player, ModelHandler.ModelPathTmPhoenix);
        }
    }

    public bool IsDetective(CCSPlayerController player)
    {
        return service.GetPlayer(player).PlayerRole() == Role.Detective;
    }

    public bool IsTraitor(CCSPlayerController player)
    {
        return service.GetPlayer(player).PlayerRole() == Role.Traitor;
    }

    public void Clear()
    {
        service.Clr();
        foreach (var key in service.Players()) key.SetPlayerRole(Role.Unassigned);
    }

    public bool IsInnocent(CCSPlayerController player)
    {
        return service.GetPlayer(player).PlayerRole() == Role.Innocent;
    }

    private Role GetWinner()
    {
        return _traitorsLeft == 0 ? Role.Traitor : Role.Innocent;
    }
}

