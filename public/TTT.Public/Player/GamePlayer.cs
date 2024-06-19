using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.Public.Mod.Role;
using TTT.Public.Player;
using TTT.Public.Shop;

namespace TTT.Player;

public class GamePlayer : IInventory
{

    private Role _playerRole;
    private int _playerId;
    private int _karma;
    private long _credits;
    private CCSPlayerController? _killer;
    private CRagdollProp? _ragdollProp;
    private readonly List<IShopItem> _items = [];
    private bool _shopOpen = false;
    private bool _isFound = false;

    public GamePlayer(Role playerRole, long credits, int karma, int playerId)
    {
        _playerRole = playerRole;
        _credits = credits;
        _karma = karma;
        _killer = null;
        _ragdollProp = null;
        _playerId = playerId;
    }

    public void AddItem(IShopItem item)
    {
        _items.Add(item);
    }

    public void RemoveItem(string name)
    {
        _items.RemoveAll(shopItem => shopItem.Name().Equals(name));
    }

    public List<IShopItem> GetItems()
    {
        return _items;
    }

    public bool HasItem(string item)
    {
        return _items.Any(shopItem => shopItem.Name().Equals(item));
    }

    public CCSPlayerController? Player()
    {
        return Utilities.GetPlayerFromUserid(_playerId);
    }

    public Role PlayerRole()
    {
        return _playerRole;
    }

    public int Karma()
    {
        return _karma;
    }

    public void AddKarma()
    {
        _karma += 2;
    }

    public void RemoveKarma()
    {
        _karma -= 5;
        if (_karma >= 40) return;
        _karma = 80;
        //Server.ExecuteCommand($"css_ban #{_playerId} 1440 Karma too low");
    }

    public void SetPlayerRole(Role role)
    {
        _playerRole = role;
    }

    public long Credits()
    {
        return _credits;
    }

    public void AddCredits(long increment)
    {
        _credits += increment;
    }

    public void RemoveCredits(long decrement)
    {
        _credits -= decrement;
    }

    public void ResetCredits()
    {
        _credits = 800; 
    }

    public CCSPlayerController? Killer()
    {
        return _killer;
    }

    public void SetKiller(CCSPlayerController? killer)
    {
        _killer = killer;
    }

    public CRagdollProp? RagdollProp()
    {
        return _ragdollProp;
    }

    public void SetRagdollProp(CRagdollProp? prop)
    {
        _ragdollProp = prop;
    }
    
    public void SetShopOpen(bool open)
    {
        _shopOpen = open;
    }
    
    public bool ShopOpen()
    {
        return _shopOpen;
    }
    
    public bool SetFound(bool found)
    {
        _isFound = found;
        return _isFound;
    }
    
    public bool IsFound()
    {
        return _isFound;
    }
}