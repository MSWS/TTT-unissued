namespace TTT.Public.Shop;

public interface IInventory
{
    List<IShopItem> GetItems();
    bool HasItem(string name);
    void AddItem(IShopItem item);
    void RemoveItem(string name);
}