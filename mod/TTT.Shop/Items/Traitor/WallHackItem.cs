using System;
using TTT.Player;
using TTT.Public.Shop;

namespace TTT.Shop.Items.Traitor;

public class WallHackItem : IShopItem
{
    
    public string Name()
    {
        return "Wall Hack";
    }

    public string SimpleName()
    {
        return "wallhack";
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