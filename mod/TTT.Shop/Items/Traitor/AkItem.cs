using CounterStrikeSharp.API.Modules.Entities.Constants;
using TTT.Player;
using TTT.Public.Mod.Role;
using TTT.Public.Shop;

namespace TTT.Shop.Items.Traitor;

public class AkItem : IShopItem
{
    public string Name()
    {
        return "AK-47";
    }

    public string SimpleName()
    {
        //css_buy ak47
        return "ak47";
    }

    public int Price()
    {
        return 500;
    }

    public BuyResult OnBuy(GamePlayer player)
    {
        if (player.Credits() < Price()) return BuyResult.NotEnoughCredits;
        if (player.PlayerRole() != Role.Traitor) return BuyResult.IncorrectRole;
        player.RemoveCredits(Price());
        var playerObject = player.Player();
        playerObject.GiveNamedItem(CsItem.AK47);
        return BuyResult.Successful;
    }
}