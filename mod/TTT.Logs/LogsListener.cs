using CounterStrikeSharp.API.Core;
using TTT.Public.Action;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Mod.Logs;
using TTT.Public.Mod.Role;
using TTT.Public.Player;

namespace TTT.Logs;

public class LogsListener(ILogService logService, IPlayerService playerService) : IPluginBehavior
{
    public void Start(BasePlugin plugin)
    {
        
    }
    
    public HookResult OnPlayerDamage(EventPlayerHurt @event, GameEventInfo info)
    {
        
        return HookResult.Continue;
    }
    
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var killedPlayer = @event.Userid;
        var killer = @event.Attacker;

        if (killedPlayer == null || !killedPlayer.IsReal()) return HookResult.Continue;
        
        var killedRole = playerService.GetPlayer(killedPlayer).PlayerRole();

        if (killer == null || !killer.IsReal())
        {
            logService.AddLog(new DeathAction(new Tuple<CCSPlayerController, Role>(killedPlayer, killedRole)));
            return HookResult.Continue;
        }

        var killerRole = playerService.GetPlayer(killer).PlayerRole();
        
        logService.AddLog(new KillAction(new Tuple<CCSPlayerController, Role>(killedPlayer, killedRole),
            new Tuple<CCSPlayerController, Role>(killer, killerRole)));
        return HookResult.Continue;
    }
}