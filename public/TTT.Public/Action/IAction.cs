using CounterStrikeSharp.API.Core;
using TTT.Public.Mod.Role;

namespace TTT.Public.Action;

public interface IAction
{
    CCSPlayerController Actor();
    string ActionMessage();

    protected static string GoodAction(Role actor, Role actor2)
    {
        return actor == actor2 ? "[Bad action!]" : "";
    }
}