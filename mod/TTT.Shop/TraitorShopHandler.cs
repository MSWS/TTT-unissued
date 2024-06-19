

using TTT.Shop.Items;

namespace TTT.Shop;

public class TraitorShopHandler : BaseShopHandler
{
 
    private static readonly TraitorShopHandler _instance = new TraitorShopHandler();
    
    public TraitorShopHandler()
    {
        AddItems("Traitor");
    }
    
    public new static TraitorShopHandler Get()
    {
        return _instance;
    }
    
}