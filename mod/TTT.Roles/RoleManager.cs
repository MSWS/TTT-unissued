using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using TTT.Player;
using TTT.Public.Action;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;
using TTT.Public.Mod.Round;
using TTT.Roles.Shop;
using TTT.Round;

namespace TTT.Roles;

public class RoleManager : PlayerHandler, IRoleService, IPluginBehavior
{
    private const int MaxDetectives = 3;

    private int _innocentsLeft;
    private IRoundService _roundService;
    private int _traitorsLeft;
    private InfoManager _infoManager;
    private MuteManager _muteManager;
    
    public void Start(BasePlugin parent)
    {
        _roundService = new RoundManager(this, parent);
        _infoManager = new InfoManager(this, _roundService, parent);
        _muteManager = new MuteManager(parent);
        ModelHandler.RegisterListener(parent);
        //ShopManager.Register(parent, this); //disabled until items are implemented.
        //CreditManager.Register(parent, this);
        
        parent.RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect);
        parent.RegisterEventHandler<EventRoundFreezeEnd>(OnRoundStart);
        parent.RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
        parent.RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        parent.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath, HookMode.Pre);
        parent.RegisterEventHandler<EventGameStart>(OnMapStart);
        parent.RegisterEventHandler<EventPlayerSpawn>(Event_PlayerSpawn, HookMode.Post);
        
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(hook =>
        {
            var ent = hook.GetParam<CBaseEntity>(0);

            var playerWhoWasDamaged = player(ent);

            if (playerWhoWasDamaged == null) return HookResult.Continue;
                 
            var info = hook.GetParam<CTakeDamageInfo>(1);
            
            if (info.BitsDamageType is not 256) return HookResult.Continue;
            
            var playerWhoAttacked = info.Attacker.Value.As<CCSPlayerPawn>();

            var playerWhoAttackedController = playerWhoAttacked.Controller.Value.As<CCSPlayerController>();
            
            info.Damage = 0f;
            
            var targetRole = GetPlayer(playerWhoWasDamaged);
            
            Server.NextFrame(() =>
            {
                playerWhoAttackedController.PrintToChat(
                    StringUtils.FormatTTT(
                        $"You tased player {playerWhoWasDamaged.PlayerName} they are a {targetRole.PlayerRole().FormatRoleFull()}"));
            });
            
            _roundService.GetLogsService().AddLog(new MiscAction("tased player " + targetRole.PlayerRole().FormatStringFullAfter(playerWhoWasDamaged.PlayerName), playerWhoAttackedController));
            
            return HookResult.Changed;
        }, HookMode.Pre);
        
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(hook =>
        {
            var ent = hook.GetParam<CBaseEntity>(0);

            var playerWhoWasDamaged = player(ent);

            if (playerWhoWasDamaged == null) return HookResult.Continue;
                 
            if (GetPlayer(playerWhoWasDamaged).IsDead()) return HookResult.Continue;
            
            GetPlayer(playerWhoWasDamaged).SetDead(true);
            
            var info = hook.GetParam<CTakeDamageInfo>(1);
            
            CCSPlayerController? attacker = null;
            
            if (info.Attacker.Value != null)
            {
                var playerWhoAttacked = info.Attacker.Value.As<CCSPlayerPawn>();

                attacker = playerWhoAttacked.Controller.Value.As<CCSPlayerController>();   
            }

            if (info.Damage < playerWhoWasDamaged.Health) return HookResult.Continue;
            
            info.Damage = 0;
            
            GetPlayer(playerWhoWasDamaged).SetKiller(attacker);
        
            _muteManager.Mute(playerWhoWasDamaged);
        
            if (IsTraitor(playerWhoWasDamaged)) _traitorsLeft--;
        
            if (IsDetective(playerWhoWasDamaged) || IsInnocent(playerWhoWasDamaged)) _innocentsLeft--;
        
            if (_traitorsLeft == 0 || _innocentsLeft == 0) Server.NextFrame(() => _roundService.ForceEnd());

            Server.NextFrame(() => playerWhoWasDamaged.CommitSuicide(false, true));
            
            Server.NextFrame(() =>
            {
                Server.PrintToChatAll(StringUtils.FormatTTT($"{GetRole(playerWhoWasDamaged).FormatStringFullAfter(" has been found.")}"));
            
                if (attacker == playerWhoWasDamaged || attacker == null) return;
        
                playerWhoWasDamaged.PrintToChat(StringUtils.FormatTTT(
                    $"You were killed by {GetRole(attacker).FormatStringFullAfter(" " + attacker.PlayerName)}."));
                attacker.PrintToChat(StringUtils.FormatTTT($"You killed {GetRole(playerWhoWasDamaged).FormatStringFullAfter(" " + playerWhoWasDamaged.PlayerName)}."));
            });
            
            return HookResult.Continue;
        }, HookMode.Pre);

    }

    [GameEventHandler]
    private HookResult OnRoundStart(EventRoundFreezeEnd @event, GameEventInfo info)
    {
        _roundService.SetRoundStatus(RoundStatus.Waiting);
        foreach (var player in Utilities.GetPlayers().Where(player => player.IsReal() && player.Team != CsTeam.None || player.Team != CsTeam.Spectator))
        {
            player.RemoveWeapons();
            player.GiveNamedItem("weapon_knife");
            player.GiveNamedItem("weapon_glock");
        }
        return HookResult.Continue;
    }

    [GameEventHandler]
    private HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (Utilities.GetPlayers().Count(player => player.IsReal() && player.Team != CsTeam.None || player.Team == CsTeam.Spectator) < 3)
        {
            _roundService.ForceEnd();
        }
        
        CreatePlayer(@event.Userid);
        
        return HookResult.Continue;
    }

    [GameEventHandler]
    private HookResult OnMapStart(EventGameStart @event, GameEventInfo info)
    {
        return HookResult.Continue;
    }
    
    [GameEventHandler]
    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        info.DontBroadcast = true;
        
        return HookResult.Continue;
    }

    [GameEventHandler]
    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        var players = Utilities.GetPlayers()
            .Where(player => player.IsValid).Where(player => player.IsReal()).ToList();

        foreach (var player in players) player.PrintToCenter(GetWinner().FormatStringFullAfter("s has won!"));

        Server.NextFrame(Clear);
        _muteManager.UnMuteAll();
        return HookResult.Continue;
    }

    [GameEventHandler]
    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var player = @event.Userid;
        Server.NextFrame(() =>
        {
            RemovePlayer(player);
            if (GetPlayers().Count == 0) _roundService.SetRoundStatus(RoundStatus.Paused);
        });
        
        return HookResult.Continue;
    }
    
    public void AddRoles()
    {
        var eligible = Utilities.GetPlayers()
            .Where(player => player.IsReal())
            .Where(player => player.Team is not (CsTeam.Spectator or CsTeam.None))
            .ToList();

        var traitorCount = (int)Math.Floor(Convert.ToDouble(eligible.Count / 3));
        var detectiveCount = (int)Math.Floor(Convert.ToDouble(eligible.Count / 8));

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
        return Players().Where(player => player.PlayerRole() == Role.Traitor).Select(player => player.Player()).ToHashSet();
    }

    public ISet<CCSPlayerController> GetDetectives()
    {
        return Players().Where(player => player.PlayerRole() == Role.Detective).Select(player => player.Player()).ToHashSet();
    }

    public ISet<CCSPlayerController> GetInnocents()
    {
        return Players().Where(player => player.PlayerRole() == Role.Innocent).Select(player => player.Player()).ToHashSet();
    }
    

    public Role GetRole(CCSPlayerController player)
    {
        return GetPlayer(player).PlayerRole();
    }

    public void AddTraitor(CCSPlayerController player)
    {
        GetPlayer(player).SetPlayerRole(Role.Traitor);
        player.SwitchTeam(CsTeam.Terrorist);
        player.PrintToCenter(Role.Traitor.FormatStringFullBefore("You are now a(n)"));
        player.PrintToChat(Role.Traitor.FormatStringFullBefore("You are now a(n)"));
        ModelHandler.SetModelNextServerFrame(player, ModelHandler.ModelPathTmPhoenix);
    }

    public void AddDetective(CCSPlayerController player)
    {
        GetPlayer(player).SetPlayerRole(Role.Detective);
        player.SwitchTeam(CsTeam.CounterTerrorist);
        player.PrintToCenter(Role.Detective.FormatStringFullBefore("You are now a(n)"));
        player.GiveNamedItem(CsItem.Taser);
        ModelHandler.SetModelNextServerFrame(player, ModelHandler.ModelPathCtmSas);
    }

    public void AddInnocents(IEnumerable<CCSPlayerController> players)
    {
        foreach (var player in players)
        {
            GetPlayer(player).SetPlayerRole(Role.Innocent);
            player.PrintToCenter(Role.Innocent.FormatStringFullBefore("You are now an"));
            player.SwitchTeam(CsTeam.Terrorist);     
            ModelHandler.SetModelNextServerFrame(player, ModelHandler.ModelPathTmPhoenix);
        }
    }

    public bool IsDetective(CCSPlayerController player)
    {
        return GetPlayer(player).PlayerRole() == Role.Detective;
    }

    public bool IsTraitor(CCSPlayerController player)
    {
        return GetPlayer(player).PlayerRole() == Role.Traitor;
    }

    public void Clear()
    {
        Clr();
        _infoManager.Reset();
        foreach (var key in GetPlayers()) key.Value.SetPlayerRole(Role.Unassigned);
    }

    public void ApplyColorFromRole(CCSPlayerController player, Role role)
    {
        switch (role)
        {
            case Role.Traitor:
                ApplyTraitorColor(player);
                break;
            case Role.Detective:
                ApplyDetectiveColor(player);
                break;
            case Role.Innocent:
                ApplyInnocentColor(player);
                break;
            case Role.Unassigned:
            default:
                break;
        }
    }
    
    public bool IsInnocent(CCSPlayerController player)
    {
        return GetPlayer(player).PlayerRole() == Role.Innocent;
    }

    private void SetColors()
    {
        foreach (var key in Players())
        {
            if (key.PlayerRole() != Role.Innocent) return;
            ApplyInnocentColor(key.Player());
        }
    }

    private void RemoveColors()
    {
        var players = Utilities.GetPlayers()
            .Where(player => player.IsValid).ToList();

        foreach (var player in players)
        {
            if (player.Pawn.Value == null) return;
            player.Pawn.Value.RenderMode = RenderMode_t.kRenderTransColor;
            player.Pawn.Value.Render =  Color.FromArgb(254, 255, 255, 255);
            Utilities.SetStateChanged(player.Pawn.Value, "CBaseModelEntity", "m_clrRender");
        }
    }
    
    private void SetColorTeam(CBaseEntity entity, string className, string fieldName, Role role = Role.Innocent)
    {
        Guard.IsValidEntity(entity);

        if (!Schema.IsSchemaFieldNetworked(className, fieldName))
        {
            Application.Instance.Logger.LogWarning("Field {ClassName}:{FieldName} is not networked, but SetStateChanged was called on it.", className, fieldName);
            return;
        }
        
        foreach (var player in GetPlayers().Values.Where(player => player.PlayerRole() == role))
        {
            Guard.IsValidEntity(player.Player());

            var playerObj = player.Player();
            
            int offset = Schema.GetSchemaOffset(className, fieldName);

            VirtualFunctions.StateChanged(entity.NetworkTransmitComponent.Handle, entity.Handle, offset, -1, (short)playerObj.Index);
        }
    }
    
    private void ApplyDetectiveColor(CCSPlayerController player)
    {
        if (!player.IsReal() || player.Pawn.Value == null)
            return;

        player.Pawn.Value.RenderMode = RenderMode_t.kRenderTransColor;
        player.Pawn.Value.Render = Color.Blue;
        SetColorTeam(player, "CBaseModelEntity", "m_clrRender", Role.Detective);
    }

    private void ApplyTraitorColor(CCSPlayerController player)
    {
        if (!player.IsReal() || player.Pawn.Value == null)
            return;

        player.Pawn.Value.RenderMode = RenderMode_t.kRenderGlow;
        player.Pawn.Value.Render = Color.Red;
        SetColorTeam(player, "CBaseModelEntity", "m_clrRender", Role.Traitor);
        //apply for traitors only somehow?
    }

    private void ApplyInnocentColor(CCSPlayerController player)
    {
        if (!player.IsReal() || player.Pawn.Value == null)
            return;
        
        player.Pawn.Value.RenderMode = RenderMode_t.kRenderGlow;
        player.Pawn.Value.Render = Color.Green;
        
        SetColorTeam(player, "CBaseModelEntity", "m_clrRender");
    }

    private Role GetWinner()
    {
        return _traitorsLeft == 0 ? Role.Traitor : Role.Innocent;
    }
    
    private HookResult Event_PlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if (!@event.Userid.IsValid)
        {
            return HookResult.Continue;
        }
    
        CCSPlayerController player = @event.Userid;

        if(!player.IsReal())
        {
            return HookResult.Continue;
        }

        if (!player.PlayerPawn.IsValid)
        {
            return HookResult.Continue;
        }

        CHandle<CCSPlayerPawn> pawn = player.PlayerPawn;

        Server.NextFrame(() => PlayerSpawnNextFrame(player, pawn));

        return HookResult.Continue;
    }

    
    private readonly WIN_LINUX<int> OnCollisionRulesChangedOffset = new(173, 172);

    private void PlayerSpawnNextFrame(CCSPlayerController player, CHandle<CCSPlayerPawn> pawn)
    {
        pawn.Value.Collision.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_DISSOLVING;

        pawn.Value.Collision.CollisionAttribute.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_DISSOLVING;

        VirtualFunctionVoid<nint> collisionRulesChanged =
            new VirtualFunctionVoid<nint>(pawn.Value.Handle, OnCollisionRulesChangedOffset.Get());

        collisionRulesChanged.Invoke(pawn.Value.Handle);
    }
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



internal static class IsValid
{
    public static bool PlayerIndex(uint playerIndex)
    {
        if(playerIndex == 0)
        {
            return false;
        }

        if(!(1 <= playerIndex && playerIndex <= Server.MaxPlayers))
        {
            return false;
        }

        return true;
    }
}


public class WIN_LINUX<T>
{
    [JsonPropertyName("Windows")]
    public T Windows { get; private set; }

    [JsonPropertyName("Linux")]
    public T Linux { get; private set; }

    public WIN_LINUX(T windows, T linux)
    {
        this.Windows = windows;
        this.Linux = linux;
    }

    public T Get()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return this.Windows;
        }
        else
        {
            return this.Linux;
        }
    }
}