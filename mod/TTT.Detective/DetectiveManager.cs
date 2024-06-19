using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory;
using TTT.Player;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Formatting;
using TTT.Public.Mod.Detective;
using TTT.Public.Mod.Role;

namespace TTT.Detective;

public class DetectiveManager : IDetectiveService, IPluginBehavior
{
    private const int TaserAmmoType = 18;
    private readonly IRoleService _roleService;

    public DetectiveManager(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public void Start(BasePlugin parent)
    {
        parent.RegisterListener<Listeners.OnTick>(() =>
        {
            foreach (var player in Utilities.GetPlayers().Where(player => player.IsValid && player.IsReal())
                         .Where(player => (player.Buttons & PlayerButtons.Use) != 0)) OnPlayerUse(player);
        });

        
        /**
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(hook =>
        {
            var info = hook.GetParam<CTakeDamageInfo>(1);
            if (info.Attacker.Value == null || !info.Attacker.Value.IsValid) return HookResult.Continue;
            var attacker = info.Attacker.Value.As<CCSPlayerController>();
            if (attacker == hook.GetParam<CBaseEntity>(0)) return HookResult.Continue;
            if (info.AmmoType is not TaserAmmoType) return HookResult.Continue;

            info.Damage = 1f;

            if (!attacker.IsReal()) return HookResult.Continue;

            var ammoType = info.AmmoType;

            return HookResult.Changed;
        }, HookMode.Pre);
        */
    }

    
    private void OnPlayerUse(CCSPlayerController player)
    {
        IdentifyBody(player);
    }

    private void IdentifyBody(CCSPlayerController caller)
    {
        //add states

       if (_roleService.GetRole(caller) != Role.Detective) return;

        var entity = caller.GetClientRagdollAimTarget();

        if (entity == null) return;
        
        if (entity.PawnIsAlive) return;
        
        var player = _roleService.GetPlayer(entity);

        if (player.IsFound()) return;
        
        var killerEntity= player.Killer();
        
        string message;

        var plr = player.Player();
        if (plr == null) return;

        if (killerEntity == null || !killerEntity.IsReal())
            message = StringUtils.FormatTTT(player.PlayerRole()
                .FormatStringFullAfter($"{plr.PlayerName} was killed by world"));
        else
            message = StringUtils.FormatTTT(
                player.PlayerRole().FormatStringFullAfter($"{plr.PlayerName} was killed by ") +
                _roleService.GetRole(killerEntity).FormatStringFullAfter(killerEntity.PlayerName));


        player.SetFound(true);
        
        Server.NextFrame(() => { Server.PrintToChatAll(message); });
    }
}