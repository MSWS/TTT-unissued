using CounterStrikeSharp.API.Core;
using MySqlConnector;
using TTT.Player;
using TTT.Public.Mod.Role;

namespace TTT.Public.Database;

public class DatabaseService {
  private readonly MySqlConnection _connector;

  public DatabaseService() {
    _connector = new MySqlConnection();
    _connector.OpenAsync();
    CreateTable();
  }

  public void CreateTable() {
    Task.Run(async () => {
      var command = new MySqlCommand(
        "CREATE TABLE IF NOT EXISTS PlayerData(steamid VARCHAR(17) NOT NULL,"
        + "kills INT," + "deaths INT," + "karma INT," + "traitor_kills INT,"
        + "traitors_killed INT," + "PRIMARY KEY(steamid));");
      command.Connection = _connector;
      await command.ExecuteNonQueryAsync();
    });
  }

  public GamePlayer CreateProfile(CCSPlayerController player) {
    var id = player.SteamID;

    Task.Run(async () => {
      var command = new MySqlCommand(
        "INSERT IGNORE INTO PlayerData(steamid, kills, deaths, karma, traitor_kills, traitors_killed)"
        + $" VALUES ({id}, 0, 0, 80, 0, 0);");
      command.Connection = _connector;
      await command.ExecuteNonQueryAsync();
    });
    return new GamePlayer(Role.Unassigned, 800, 0, player.UserId.Value);
  }

  public void UpdatePlayer(GamePlayer player) {
    Task.Run(async () => {
      var command = new MySqlCommand(
        "UPDATE PlayerData SET kills = @kills, deaths = @deaths, karma = @karma, traitor_kills = @traitor_kills, traitors_killed = @traitors_killed"
        + " WHERE steamid = @steamid;");
      command.Parameters.AddWithValue("@kills", 0);
      command.Parameters.AddWithValue("@deaths", 0);
      command.Parameters.AddWithValue("@karma", player.Karma());
      command.Parameters.AddWithValue("@traitor_kills", 0);
      command.Parameters.AddWithValue("@traitors_killed", 0);
      command.Parameters.AddWithValue("@steamid", player.Player().SteamID);
      command.Connection = _connector;
      await command.ExecuteNonQueryAsync();
    });
  }
}