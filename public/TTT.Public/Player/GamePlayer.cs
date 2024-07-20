using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.Public.Mod.Role;

namespace TTT.Player;

public class GamePlayer {
  private long _credits;
  private bool _isDead;
  private bool _isFound;
  private int _karma;
  private CCSPlayerController? _killer;
  private readonly int _playerId;

  private Role _playerRole;
  private bool _shopOpen;

  public GamePlayer(Role playerRole, long credits, int karma, int playerId) {
    _playerRole = playerRole;
    _credits    = credits;
    _karma      = karma;
    _killer     = null;
    _playerId   = playerId;
  }

  public bool IsDead() { return _isDead; }

  public void SetDead(bool isDead = false) { _isDead = isDead; }

  public CCSPlayerController? Player() {
    return Utilities.GetPlayerFromUserid(_playerId);
  }

  public Role PlayerRole() { return _playerRole; }

  public int Karma() { return _karma; }

  public void SetKarma(int karma) { _karma = karma; }

  public void AddKarma(int karma) { _karma += karma; }

  public void RemoveKarma(int karma) {
    _karma -= karma;
    if (_karma >= 40) return;
    _karma = 80;
    //Server.ExecuteCommand($"css_ban #{_playerId} 1440 Karma too low");
  }

  public void SetPlayerRole(Role role) { _playerRole = role; }

  public long Credits() { return _credits; }

  public void AddCredits(long increment) { _credits += increment; }

  public void RemoveCredits(long decrement) { _credits -= decrement; }

  public void ResetCredits() { _credits = 800; }

  public CCSPlayerController? Killer() { return _killer; }

  public void SetKiller(CCSPlayerController? killer) { _killer = killer; }

  public void SetShopOpen(bool open) { _shopOpen = open; }

  public bool ShopOpen() { return _shopOpen; }

  public bool SetFound(bool found) {
    _isFound = found;
    return _isFound;
  }

  public bool IsFound() { return _isFound; }
}