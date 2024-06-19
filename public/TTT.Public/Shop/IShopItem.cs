using CounterStrikeSharp.API.Core;
using TTT.Player;

namespace TTT.Public.Shop;

public interface IShopItem
{
    /// <summary>
    /// Display name in the shop
    /// </summary>
    /// <returns>The item name</returns>
    string Name();
    
    /// <summary>
    /// The simple name of the item
    /// </summary>
    /// <returns>The simple name of the item</returns>
    string SimpleName();
    
    /// <summary>
    /// The price of the item
    /// </summary>
    /// <returns>The price of the item</returns>
    int Price();
    
    /// <summary>
    /// Called when the player buys the item
    /// </summary>
    /// <param name="player">The player that bought the item</param>
    /// <returns>if the item was bought or not.</returns>
    BuyResult OnBuy(GamePlayer player);
}