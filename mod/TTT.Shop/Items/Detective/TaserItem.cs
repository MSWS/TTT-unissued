using System;
using TTT.Player;
using TTT.Public.Shop;

namespace TTT.Shop.Items.Detective;

public class TaserItem : IShopItem
{
    public string Name()
    {
       return "Taser";
    }

    public string SimpleName()
    {
        return "taser";
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