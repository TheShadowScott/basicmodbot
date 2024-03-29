﻿using System.Text;

namespace ModBot.Deps;
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
            !IsPrivate(ctx) && (ctx.Member.Permissions.HasPermission(perms) || IsAdmin(ctx) || IsMod(ctx.Member));
    public static bool IsMod(this DiscordMember member)
    {
        foreach (var role in member.Roles)
            if (Program.ModList.Contains(role.Id)) return true;
        return false;
    }
    public static bool IsMod (this InteractionContext ctx)
    {
        foreach (var role in ctx.Member.Roles)
            if (Program.ModList.Contains(role.Id)) return true;
        return false;
    }
    private static bool IsPrivate(InteractionContext ctx) => ctx.Channel.IsPrivate;

    public static bool IsAdmin(this InteractionContext ctx) => !IsPrivate(ctx) && ctx.Member.Permissions.HasPermission(Permissions.Administrator);
    public static bool EditCheck(this InteractionContext ctx) =>
    Program.LocalSettings.BotSettings.LogEditLevel == "Administrator" ?
        ctx.IsAdmin() : ctx.IsMod();

    public static string? ToDiscordRoleDisplayString(this IEnumerable<DiscordRole>? roleList)
    {
        if (roleList is null || !roleList.Any()) return null;
        var sb = new StringBuilder();
        foreach (var role in roleList)
            sb.Append(@$"<@&{role.Id}> ");
        sb.Length--;
        return sb.ToString();
    }

    public static string GetOrdinal(int input)
    {
        int num = input > 100 ? input % 100 : input;
        return (num % 10) switch
        {
            _ when num is > 10 and < 20 => "th",
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };
    }

    public static string PrependBackslashBeforeChars(this string str)
    {
        var sb = new StringBuilder();
        foreach (var c in str)
            sb.Append(@$"\{c}");
        return sb.ToString();
    }
    public static string AddOrdinalToDateObject(DateTimeOffset off, string format = "dddd, d MMMM yyyy HH:mm:ss")
    {
        if (format.Contains("dddd"))
            format = format.Replace("dddd", "Q");
        int day = off.Day;
        string ord = GetOrdinal(day).PrependBackslashBeforeChars();

        format = format.Replace("d", $"d{ord}").Replace("Q", "dddd");

        return off.ToString(format);
    }

}

public class CList<T>
{
    public List<T> Values { get; set; } = new();

    public static implicit operator List<T>(CList<T> list) => list.Values;
    public static implicit operator CList<T>(List<T> list) => new() { Values = list };
    public static CList<T> operator +(CList<T> list, T value) => new() { Values = list.Values.Append(value).ToList() };
    public static CList<T> operator +(CList<T> list, CList<T> value) => new() { Values = list.Values.Concat((List<T>)value).ToList() };
}