using TTT.Formatting.Base;
using TTT.Public.Mod.Role;

namespace TTT.Formatting.Views;

public interface IRoleNotifications {
  IView GetRoleNotification(Role role);
  IView KilledByRoleNotification(Role role);
  IView KilledRoleNotification(Role role);
}