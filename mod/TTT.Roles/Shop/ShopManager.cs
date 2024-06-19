using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using TTT.Player;
using TTT.Public.Mod.Role;
using TTT.Public.Player;
using TTT.Public.Shop;
using TTT.Shop;

namespace TTT.Roles.Shop;

public class ShopManager
{
    private readonly IPlayerService _playerService;
    private readonly BasePlugin _plugin;
    private static ShopManager Manager;
    
    private ShopManager(BasePlugin plugin, IPlayerService manager)
    {
        _playerService = manager;
        _plugin = plugin;
        plugin.AddCommand("css_shop", "Open the shop menu", OnShopCommand);
        plugin.AddCommand("css_buy", "Buys specified item", OnBuyCommand);
    }
    
    public static void Register(BasePlugin plugin, IPlayerService manager)
    {
        Manager = new ShopManager(plugin, manager);
    }
    
    public void OpenShop(GamePlayer player)
    {
        var role = player.PlayerRole();
        switch (role)
        {
            case Role.Innocent:
                new ShopMenu(_plugin, BaseShopHandler.Get(), player).Open(player.Player());
                player.SetShopOpen(true);
                break;
            case Role.Detective:
                new ShopMenu(_plugin, DetectiveShopHandler.Get(), player).Open(player.Player());

                player.SetShopOpen(true);
                break;
            case Role.Traitor:
                new ShopMenu(_plugin, TraitorShopHandler.Get(), player).Open(player.Player());
                player.SetShopOpen(true);
                break;
            case Role.Unassigned:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnShopCommand(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null)
        {
            info.ReplyToCommand("Can only be executed by a player!");
            return;
        }
        OpenShop(_playerService.GetPlayer(player));
    }

    private void OnBuyCommand(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null)
        {
            info.ReplyToCommand("Can only be executed by a player!");
            return;
        }

        var item = info.GetArg(1);
        
        if (string.IsNullOrEmpty(item))
        {
            info.ReplyToCommand("Please specify an item to buy!");
            return;
        }
        
        var gamePlayer = _playerService.GetPlayer(player);

        switch (gamePlayer.PlayerRole())
        {
            case Role.Traitor:
                TraitorShopHandler.Get().BuyItem(gamePlayer, item);
                break;
            case Role.Detective:
                DetectiveShopHandler.Get().BuyItem(gamePlayer, item);
                break;
            case Role.Innocent:
                BaseShopHandler.Get().BuyItem(gamePlayer, item);
                break;
            case Role.Unassigned:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}