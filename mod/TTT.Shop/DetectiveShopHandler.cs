
using System.Reflection;
using TTT.Public.Shop;

namespace TTT.Shop;

public class DetectiveShopHandler : BaseShopHandler
{
    private static readonly DetectiveShopHandler _instance = new DetectiveShopHandler();

    private DetectiveShopHandler()
    {
        AddItems("Detective");
    }
    
    
    public new static DetectiveShopHandler Get()
    {
        return _instance;
    }
}