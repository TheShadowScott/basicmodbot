namespace DiscordBot;

public static class QuickCreate
{
    public static DiscordEmbed QuickEmbed(string data, int color, string author = "", string title = "", string footer = "")
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = title,
            Description = data,
            Color = new DiscordColor(color),
            Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = author
            },
            Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = footer
            }
        };
        return embed.Build();
    }
}