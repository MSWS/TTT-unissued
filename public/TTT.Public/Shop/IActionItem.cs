using TTT.Player;

namespace TTT.Public.Shop;

public interface IActionItem : IShopItem
{
    void OnAction(GamePlayer player);
}