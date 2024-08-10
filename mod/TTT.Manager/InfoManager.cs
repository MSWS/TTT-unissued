using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.Player;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;
using TTT.Public.Mod.Round;
using TTT.Public.Player;
using TTT.Round;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.Manager;

public class InfoManager : IPluginBehavior
{
    private readonly Dictionary<CCSPlayerController, Tuple<CCSPlayerController, Role>> _playerLookAtRole = new();
    private readonly IPlayerService _playerService;
    private readonly IRoundService _manager;
    private readonly Dictionary<CCSPlayerController, Tuple<string, Role>> _spectatorLookAtRole = new();

    public InfoManager(IPlayerService playerService, IRoleService manager)
    {
        _playerService = playerService;
        _manager = manager.GetRoundService();
    }
    
    public void Start(BasePlugin plugin)
    {
        plugin.RegisterListener<Listeners.OnTick>(OnTick);
        plugin.AddTimer(0.1f, OnTickAll, TimerFlags.REPEAT);
    }

    public void Reset()
    {
        _playerLookAtRole.Clear();
        _spectatorLookAtRole.Clear();
    }
    
    public void RegisterLookAtRole(CCSPlayerController player, Tuple<CCSPlayerController, Role> role)
    {
        _playerLookAtRole.TryAdd(player, role);
    }

    public void RemoveLookAtRole(CCSPlayerController player)
    {
        _playerLookAtRole.Remove(player);
    }

    public void OnTick()
    {
        foreach (var gamePlayer in _playerService.Players())
        {
            if (_manager.GetRoundStatus() != RoundStatus.Started) return;

            var player = gamePlayer.Player();

            if (player == null) continue;
            if (!player.IsReal()) continue;

            var playerRole = gamePlayer.PlayerRole();

            if (playerRole == Role.Unassigned) continue;

            if (!player.PawnIsAlive) continue;

            if (!_playerLookAtRole.TryGetValue(player, out var value))
            {
                Server.NextFrame(() =>
                    player.PrintToCenterHtml(
                        $"<font class='fontsize=m' color='yellow'>Your Role: {playerRole.GetCenterRole()}"));
                continue;
            }

            if (!value.Item1.IsReal()) continue;

            if (!value.Item1.PawnIsAlive) HandleDeadTarget(player, playerRole, value.Item1, value.Item2);
            else HandleAliveTarget(player, playerRole, value.Item1, value.Item2);
        }
    }

    public void HandleDeadTarget(CCSPlayerController player, Role playerRole, CCSPlayerController target, Role targetRole)
    {
        Server.NextFrame(() => player.PrintToCenterHtml(
            $"<font class='fontsize=m' color='yellow'>Your Role: {playerRole.GetCenterRole()} <br>"
            + $"<font class='fontsize=l' color='red'>{target.PlayerName}'s Corpse <br>" 
            + $"<font class='fontsize=m' color='yellow'>{target.PlayerName}'s Role: {targetRole.GetCenterRole()}"));
    }

    public void HandleAliveTarget(CCSPlayerController player, Role playerRole, CCSPlayerController target, Role targetRole)
    {
        switch (targetRole)
        {
            case Role.Detective:
                Server.NextFrame(() => player.PrintToCenterHtml(
                    $"<font class='fontsize=m' color='yellow'>Your Role: {playerRole.GetCenterRole()} <br>"
                    + $"<font class='fontsize=m' color='yellow'>{target.PlayerName}'s Role: {targetRole.GetCenterRole()}"));
                return;
            case Role.Traitor when playerRole == Role.Traitor:
                Server.NextFrame(() => player.PrintToCenterHtml(
                    $"<font class='fontsize=m' color='yellow'>Your Role: {playerRole.GetCenterRole()} <br>"
                    + $"<font class='fontsize=m' color='maroon'>{target.PlayerName}'s Role: {targetRole.GetCenterRole()}"));
                return;
            default:
                Server.NextFrame(() => player.PrintToCenterHtml(
                    $"<font class='fontsize=m' color='yellow'>Your Role: {playerRole.GetCenterRole()} <br>"
                    + $"<font class='fontsize=m' color='yellow'>{target.PlayerName}'s Role: {Role.Unassigned.GetCenterRole()}"));
                break;
        }
    }
    
    public void OnTickAll()
    {
        var players = _playerService.Players().Select(plr => plr.Player());
        
        _playerLookAtRole.Clear();
        
        foreach (var player in players)
        { 
            if (player == null) continue;
            if (!player.IsReal()) continue;
            
            var target = player.GetClientPlayerAimTarget();
            
            if (target == null) continue;
            if (!target.IsReal()) continue;
            
            RegisterLookAtRole(player, new Tuple<CCSPlayerController, Role>(target, _playerService.GetPlayer(target).PlayerRole()));
        }
    }

    [GameEventHandler]
    private HookResult OnPlayerSpectateChange(EventSpecTargetUpdated @event, GameEventInfo info)
    {
        var player = @event.Userid;
        var target = new CCSPlayerController(@event.Target);

        if (player == null || !player.IsReal() || !target.IsReal()) return HookResult.Continue;
        
        _spectatorLookAtRole.TryAdd(player, new Tuple<string, Role>(target.PlayerName, _playerService.GetPlayer(target).PlayerRole()));
        
        return HookResult.Continue;
    }
}