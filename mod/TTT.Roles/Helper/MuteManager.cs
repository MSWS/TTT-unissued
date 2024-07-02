using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.Public.Extensions;

namespace TTT.Roles;

public class MuteManager
{

    public MuteManager(BasePlugin plugin)
    {
        plugin.RegisterListener<Listeners.OnClientVoice>(OnPlayerSpeak);
    }
    
    public void Mute(CCSPlayerController player)
    {
        if (BypassMute(player))
            return;
        
        player.VoiceFlags |= VoiceFlags.Muted;
    }

    public void UnMute(CCSPlayerController player)
    {
        player.VoiceFlags &= ~VoiceFlags.Muted;
    }
    
    public void UnMuteAll()
    {
        foreach (var player in Utilities.GetPlayers().Where(player => player.IsReal() && player.Team == CsTeam.Terrorist))
        {
            UnMute(player);
        }
        foreach (var player in Utilities.GetPlayers().Where(player => player.IsReal() && player.Team == CsTeam.CounterTerrorist))
        {
            UnMute(player);
        }
    }
    
    private void OnPlayerSpeak(int playerSlot)
    {
        var player = Utilities.GetPlayerFromSlot(playerSlot);
        if (!player.IsReal())
            return;

        if (!player.PawnIsAlive && !BypassMute(player))
        {
            // Normal players can't speak when dead
            Mute(player);
            Server.NextFrame(() => player.PrintToCenter("You are dead and muted!"));
            return;
        }

        if (IsMuted(player))
        {
            Server.NextFrame(() => player.PrintToCenter("You are muted!"));
            return;
        }
    }
    
    private bool IsMuted(CCSPlayerController player)
    {
        if (!player.IsReal())
            return false;
        return (player.VoiceFlags & VoiceFlags.Muted) != 0;
    }

    private bool BypassMute(CCSPlayerController player)
    {
        return player.IsReal() && AdminManager.PlayerHasPermissions(player, "@css/chat");
    }
}