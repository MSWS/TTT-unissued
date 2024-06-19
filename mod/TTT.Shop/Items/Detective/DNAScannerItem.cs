using TTT.Player;
using TTT.Public.Shop;

namespace TTT.Shop.Items.Detective;

public class DNAScannerItem : IShopItem
{
    public string Name()
    {
        return "DNA Scanner";
    }

    public string SimpleName()
    {
        return "dnascanner";
    }

    public int Price()
    {
        return 1000;
    }

    public BuyResult OnBuy(GamePlayer player)
    {
        return BuyResult.Successful;
    }
}