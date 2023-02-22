using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;

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
    public static bool CheckPermissions(this InteractionContext ctx,
       Permissions perms) =>
            (!IsPrivate(ctx) && (ctx.Member.Permissions.HasPermission(perms))) || IsMod(ctx.Member);
    public static bool IsMod(this DiscordMember member)
    {
        foreach (var role in member.Roles)
            if (Program.ModList.Contains(role.Id)) return true;
        return false;
    }
    private static bool IsPrivate(InteractionContext ctx) => ctx.Channel.IsPrivate;

    public static bool IsAdmin(this InteractionContext ctx) => !IsPrivate(ctx) && ctx.Member.Permissions.HasPermission(Permissions.Administrator);
}
