using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBot;
public static class GMethods
{
    public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }
    public static bool CheckPermissions(this DSharpPlus.SlashCommands.InteractionContext ctx,
       Permissions perms) =>
            !ctx.Channel.IsPrivate && (ctx.Member.Permissions.HasPermission(perms));
}
