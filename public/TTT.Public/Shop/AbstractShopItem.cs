using TTT.Player;

namespace TTT.Public.Shop;

public abstract class AbstractShopItem {
  public abstract string Name { get; }
  public abstract string ShortName { get; }
  public abstract string Description { get; }
  public abstract void OnBuy(GamePlayer player);

  public bool CanBuy(GamePlayer player) {
    return player.Credits() >= ItemCost();
  }

  public int ItemCost() {
    //grab from config
    // config.getCost(Name)
    return 0;
  }
}