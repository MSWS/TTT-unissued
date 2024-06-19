using System.Collections.Generic;
using System.Reflection;
using TTT.Player;
using TTT.Public.Behaviors;
using TTT.Public.Formatting;
using TTT.Public.Shop;
using TTT.Shop.Items;

namespace TTT.Shop;

public class BaseShopHandler : IShopItemHandler, IPluginBehavior
{
    private readonly ISet<IShopItem> _items = new HashSet<IShopItem>();

    private static readonly BaseShopHandler Instance = new BaseShopHandler();

    protected BaseShopHandler()
    {
        AddItems("All");
        AddItems("Innocent");
    }

    protected void AddItems(string name)
    {
        var fullName = "TTT.Shop.Items." + name;
        var q = from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass && t.Namespace == fullName && t.GetInterface("IShopItem") != null
            select t;

        foreach (var type in q)
        {
            if (type == null) return;
            var item = (IShopItem?) Activator.CreateInstance(type);
            if (item == null) return;
            AddShopItem(item);
        }
    }
    
    public void BuyItem(GamePlayer player, string itemName)
    {
        foreach (var item in _items)
        {
            if (!item.SimpleName().Equals(itemName)) continue;
            BuyItem(player, item);
            return;
        }
    }
    
    private void BuyItem(GamePlayer player, IShopItem item)
    {
        var successful = item.OnBuy(player);
        switch (successful)
        {
            case BuyResult.NotEnoughCredits:
                player.Player()
                    .PrintToChat(StringUtils.FormatTTT($"You don't have enough credits to buy {item.Name()}"));
                break;
            case BuyResult.Successful:
                player.Player().PrintToChat(StringUtils.FormatTTT($"You have bought {item.Name()}"));
                player.AddItem(item);
                break;
            case BuyResult.AlreadyOwned:
                player.Player().PrintToChat(StringUtils.FormatTTT($"You already own {item.Name()}"));
                break;
            case BuyResult.IncorrectRole:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static BaseShopHandler Get()
    {
        return Instance;
    }

    public ISet<IShopItem> GetShopItems()
    {
        return _items;
    }

    public void AddShopItem(IShopItem item)
    {
        _items.Add(item);
    }

    
}