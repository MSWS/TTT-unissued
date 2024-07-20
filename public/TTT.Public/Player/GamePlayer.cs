using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.Public.Mod.Role;

namespace TTT.Player;

public class GamePlayer(Role playerRole, long credits, int karma,
  int playerId) {
  private bool dead;
  private bool found;

  // TODO: Use Steam ID
  private CCSPlayerController? killer;

  private bool shopOpen;

  public bool IsDead() { return dead; }

  public void SetDead(bool isDead = false) { dead = isDead; }

  public CCSPlayerController? Player() {
    return Utilities.GetPlayerFromUserid(playerId);
  }

  public Role PlayerRole() { return playerRole; }

  public int Karma() { return karma; }

  public void SetKarma(int karma1) { karma = karma1; }

  public void AddKarma(int karma1) { karma += karma1; }

  public void RemoveKarma(int karma1) {
    karma -= karma1;
    if (karma >= 40) return;
    karma = 80;
    //Server.ExecuteCommand($"css_ban #{_playerId} 1440 Karma too low");
  }

  public void SetPlayerRole(Role role) { playerRole = role; }

  public long Credits() { return credits; }

  public void AddCredits(long increment) { credits += increment; }

  public void RemoveCredits(long decrement) { credits -= decrement; }

  public void ResetCredits() { credits = 800; }

  public CCSPlayerController? Killer() { return killer; }

  public void SetKiller(CCSPlayerController? killer) { this.killer = killer; }

  public void SetShopOpen(bool open) { shopOpen = open; }

  public bool ShopOpen() { return shopOpen; }

  public bool SetFound(bool found) {
    this.found = found;
    return this.found;
  }

  public bool IsFound() { return found; }
}