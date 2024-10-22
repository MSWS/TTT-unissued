﻿using Microsoft.Extensions.DependencyInjection;
using TTT.Public.Extensions;
using TTT.Public.Mod.Logs;
using TTT.Public.Mod.Role;

namespace TTT.Logs;

public static class LogServiceExtension
{
    public static void AddLogsService(this IServiceCollection collection)
    {
        collection.AddPluginBehavior<ILogService, LogBehavior>();
        collection.AddPluginBehavior<LogsCommand>();
        collection.AddPluginBehavior<LogsListener>();
    }
}