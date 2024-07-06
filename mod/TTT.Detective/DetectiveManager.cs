using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using TTT.Player;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Formatting;
using TTT.Public.Mod.Detective;
using TTT.Public.Mod.Role;
using TTT.Public.Player;

namespace TTT.Detective;

public class DetectiveManager : IDetectiveService, IPluginBehavior
{
    private const int TaserAmmoType = 18;
    private readonly IPlayerService _roleService;

    public DetectiveManager(IPlayerService roleService)
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

        
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(OnZeus, HookMode.Pre);

    }

    public HookResult OnZeus(DynamicHook hook)
    {
        var ent = hook.GetParam<CBaseEntity>(0);

        var playerWhoWasDamaged = player(ent);

        if (playerWhoWasDamaged == null) return HookResult.Continue;

        var info = hook.GetParam<CTakeDamageInfo>(1);

        CCSPlayerController? attacker = null;

        if (info.Attacker.Value != null)
        {
            var playerWhoAttacked = info.Attacker.Value.As<CCSPlayerPawn>();

            attacker = playerWhoAttacked.Controller.Value.As<CCSPlayerController>();

        }

        if (info.BitsDamageType is not 256) return HookResult.Continue;
        if (attacker == null) return HookResult.Continue;

        info.Damage = 0;

        var targetRole = _roleService.GetPlayer(playerWhoWasDamaged);

        Server.NextFrame(() =>
        {
            attacker.PrintToChat(
                StringUtils.FormatTTT(
                    $"You tased player {playerWhoWasDamaged.PlayerName} they are a {targetRole.PlayerRole().FormatRoleFull()}"));
        });
        
        return HookResult.Stop;
    }

    
    private void OnPlayerUse(CCSPlayerController player)
    {
        IdentifyBody(player);
    }

    private void IdentifyBody(CCSPlayerController caller)
    {
        //add states

       if (_roleService.GetPlayer(caller).PlayerRole() != Role.Detective) return;

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
                _roleService.GetPlayer(killerEntity).PlayerRole().FormatStringFullAfter(killerEntity.PlayerName));


        player.SetFound(true);
        
        Server.NextFrame(() => { Server.PrintToChatAll(message); });
    }
    
    //to be moved to a utility class
    public static CCSPlayerController? player(CEntityInstance? instance)
    {
        if (instance == null)
        {
            return null;
        }

        if (instance.DesignerName != "player")
        {
            return null;
        }

        // grab the pawn index
        int player_index = (int)instance.Index;

        // grab player controller from pawn
        CCSPlayerPawn player_pawn = Utilities.GetEntityFromIndex<CCSPlayerPawn>(player_index);

        // pawn valid
        if (player_pawn == null || !player_pawn.IsValid)
        {
            return null;
        }

        // controller valid
        if (player_pawn.OriginalController == null || !player_pawn.OriginalController.IsValid)
        {
            return null;
        }

        // any further validity is up to the caller
        return player_pawn.OriginalController.Value;
    }
}