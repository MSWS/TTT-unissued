using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Mod.Role;
using TTT.Public.Player;

namespace TTT.Player;

public class KarmaManager(IPlayerService playerService) : IPluginBehavior
{
    public void Start(BasePlugin plugin)
    {
        
    }
    
    [GameEventHandler]
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var victim = @event.Userid;
        
        if (attacker == null || victim == null) return HookResult.Continue;
        if (attacker == victim) return HookResult.Continue;
        
        GamePlayer gpAttacker = playerService.GetPlayer(attacker);
        GamePlayer gpVictim = playerService.GetPlayer(victim);
        
        var AttackerRole = gpAttacker.PlayerRole();
        var VictimRole = gpVictim.PlayerRole();

        if (AttackerRole == VictimRole)
        {
            gpAttacker.RemoveKarma(5);
            return HookResult.Continue;
        }
        
        if (VictimRole == Role.Traitor)
        {
            gpAttacker.AddKarma(3);
            return HookResult.Continue;
        }

        if (VictimRole == Role.Detective && AttackerRole == Role.Innocent) {
            gpAttacker.RemoveKarma(5);
            return HookResult.Continue;
        }

        if (VictimRole == Role.Detective && AttackerRole == Role.Traitor) {
            gpAttacker.AddKarma(3);
            return HookResult.Continue;
        }

        return HookResult.Continue;
    }
}
