using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using TTT.Player;
using TTT.Public.Formatting;
using TTT.Public.Player;

namespace TTT.Public.Shop;

public class ShopMenu
{
    private readonly CenterHtmlMenu _menu;
    private readonly GamePlayer _playerService;
    private readonly IShopItemHandler _shopItemHandler;

    public ShopMenu(BasePlugin plugin, IShopItemHandler shopItemHandler, GamePlayer playerService)
    {
        _menu = new CenterHtmlMenu($"Shop - {playerService.Credits()} credits", plugin);
        _shopItemHandler = shopItemHandler;
        _playerService = playerService;
        Create();
    }

    public void BuyItem(GamePlayer player, IShopItem item)
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

    public void BuyItem(GamePlayer player, int index)
    {
        var item = _shopItemHandler.GetShopItems().ElementAt(index);
        BuyItem(player, item);
    }

    public void BuyItem(GamePlayer player, string name)
    {
        foreach (var item in _shopItemHandler.GetShopItems())
        {
            if (!item.SimpleName().Equals(name)) continue;
            BuyItem(player, item);
            return;
        }
        player.Player().PrintToChat(StringUtils.FormatTTT("Item not found!"));
    }

    public void Create()
    {
        foreach (var option in _menu.MenuOptions.Where(option => option.Text.Equals("close")))
        {
            option.OnSelect += (player, _) =>
            {
                _playerService.SetShopOpen(false);
            };
        }

        for (var index = 0; index < _shopItemHandler.GetShopItems().Count; index++)
        {
            var item = _shopItemHandler.GetShopItems().ElementAt(index);
            _menu.AddMenuOption(item.Name() + $" - {item.Price()} credits",
                (player, _) =>
                {
                    BuyItem(_playerService, item);
                    _playerService.SetShopOpen(false);
                });
        }
    }
    
    public void Open(CCSPlayerController player)
    {
        _menu.Open(player);
        
    }
}