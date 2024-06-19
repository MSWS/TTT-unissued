namespace TTT.Public.Shop;

public interface IShopItemHandler
{
    ISet<IShopItem> GetShopItems();
    void AddShopItem(IShopItem item);
}