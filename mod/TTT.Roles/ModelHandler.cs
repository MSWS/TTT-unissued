using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace TTT.Roles;

public class ModelHandler {
  public static readonly string ModelPathCtmHeavy =
    "characters\\models\\ctm_heavy\\ctm_heavy.vmdl";

  public static readonly string ModelPathCtmSas =
    "characters\\models\\ctm_sas\\ctm_sas.vmdl";

  public static readonly string ModelPathTmHeavy =
    "characters\\models\\tm_phoenix_heavy\\tm_phoenix_heavy.vmdl";

  public static readonly string ModelPathTmPhoenix =
    "characters\\models\\tm_phoenix\\tm_phoenix.vmdl";


  public static void RegisterListener(BasePlugin plugin) {
    plugin.RegisterListener<Listeners.OnMapStart>(map => {
      Server.PrecacheModel(ModelPathCtmHeavy);
      Server.PrecacheModel(ModelPathCtmSas);
      Server.PrecacheModel(ModelPathTmPhoenix);
      Server.PrecacheModel(ModelPathTmHeavy);
    });
  }

  public static void SetModel(CCSPlayerController player, string modelPath) {
    player.PlayerPawn.Value.SetModel(modelPath);
  }

  public static void SetModelNextServerFrame(CCSPlayerController playerPawn,
    string model) {
    Server.NextFrame(() => { SetModel(playerPawn, model); });
  }
}