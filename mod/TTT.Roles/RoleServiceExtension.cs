using Microsoft.Extensions.DependencyInjection;
using TTT.Public.Extensions;
using TTT.Public.Mod.Role;
using TTT.Roles.Commands;

namespace TTT.Roles;

public static class RoleServiceExtension {
  public static void AddTTTRoles(this IServiceCollection collection) {
    collection.AddPluginBehavior<IRoleService, RoleBehavior>();
    collection.AddPluginBehavior<RolesCommand>();
  }
}