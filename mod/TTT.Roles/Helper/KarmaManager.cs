using CounterStrikeSharp.API.Core;
using TTT.Public.Mod.Role;
using TTT.Public.Player;

namespace TTT.Roles;

public class KarmaManager
{
    public KarmaManager(BasePlugin plugin, IRoleService service)
    {
        plugin.RegisterEventHandler<EventPlayerDeath>((@event, info) =>
        {
            var attacker = @event.Attacker;
            var killedPlayer = @event.Userid;
            
            if (killedPlayer == null || attacker == null) return HookResult.Continue;
            if (killedPlayer == attacker) return HookResult.Continue;
            
            var attackerRole = service.GetRole(attacker);
            var killedRole = service.GetRole(killedPlayer);
            
            if (attackerRole == Role.Traitor && killedRole != Role.Traitor) return HookResult.Continue;
            
            service.GetPlayer(attacker).AddKarma();
            
            return HookResult.Continue;
        });
    } 
}