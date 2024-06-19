using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using TTT.Public.Behaviors;
using TTT.Public.Mod.Role;
using TTT.Public.Mod.Round;
using TTT.Round;

namespace TTT.Roles;

public class RDMListener(IRoleService roleService) : IPluginBehavior
{
    public void Start(BasePlugin plugin)
    {
        plugin.RegisterEventHandler<EventPlayerDeath>(OnPlayerKill);
    }

    [GameEventHandler]
    private HookResult OnPlayerKill(EventPlayerDeath @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var killedPlayer = @event.Userid;

        if (killedPlayer == null || attacker == null) return HookResult.Continue;

        var attackerRole = roleService.GetRole(attacker);
        var killedRole = roleService.GetRole(killedPlayer);
        
        if (attackerRole == Role.Traitor && killedRole != Role.Traitor) return HookResult.Continue;
        if (killedRole == Role.Traitor) return HookResult.Continue;
        
        roleService.GetPlayer(attacker).RemoveKarma();
        
        return HookResult.Continue;
    }
}
