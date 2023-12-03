using System.Xml.Serialization;
#pragma warning disable 8618, CA1050
[XmlRoot(ElementName = "InitSettings")]
public class InitSettings
{

    [XmlElement(ElementName = "BotId")]
    public string BotId { get; set; }

    [XmlElement(ElementName = "SQLString")]
    public string SQLString { get; set; }

    [XmlElement(ElementName = "OwnerId")]
    public ulong OwnerId { get; set; }
}

[XmlRoot(ElementName = "ModMailSettings")]
public class ModMailSettings
{

    [XmlElement(ElementName = "ChannelId")]
    public ulong ChannelId { get; set; }

    [XmlElement(ElementName = "DefaultModeratorRole")]
    public ulong DefaultModeratorRole { get; set; }

    [XmlElement(ElementName = "UrgencyPingLvl")]
    public int UrgencyPingLvl { get; set; }
}

[XmlRoot(ElementName = "ModRoles")]
public class ModRoles
{

    [XmlElement(ElementName = "ModRoleId")]
    public List<ulong> ModRoleId { get; set; }
}

[XmlRoot(ElementName = "AdminRoles")]
public class AdminRoles
{

    [XmlElement(ElementName = "AdminRoleId")]
    public List<ulong> AdminRoleId { get; set; }
}

[XmlRoot(ElementName = "BotSettings")]
public class BotSettings
{

    [XmlElement(ElementName = "ModRoles")]
    public ModRoles ModRoles { get; set; }

    [XmlElement(ElementName = "AdminRoles")]
    public AdminRoles AdminRoles { get; set; }

    [XmlElement(ElementName = "LogEditLevel")]
    public string LogEditLevel { get; set; }

    [XmlElement(ElementName = "LogChannelId")]
    public ulong LogChannelId { get; set; }

    [XmlElement(ElementName = "MainServerId")]
    public ulong MainServerId { get; set; }
    [XmlElement(ElementName = "DeveloperAccessLevel")]
    public string? DeveloperAccessLevel { get; set; }
    [XmlElement(ElementName = "DeveloperRoleId")]
    public ulong? DeveloperRoleId { get; set; }
    [XmlElement(ElementName = "LogLevel")]
    public string? LogLevel { get; set; }

    public int GetLogLevel()
    {
        return LogLevel switch
        {
            "Trace" => 0,
            "Debug" => 1,
            "Information" => 2,
            "Warning" => 3,
            "Error" => 4,
            "Critical" => 5,
            "None" => 6,
            _ => throw new ArgumentException("Invalid log level", nameof(LogLevel))
        };
    }
}

[XmlRoot(ElementName = "Settings")]
public class Settings
{

    [XmlElement(ElementName = "InitSettings")]
    public InitSettings InitSettings { get; set; }

    [XmlElement(ElementName = "ModMailSettings")]
    public ModMailSettings ModMailSettings { get; set; }

    [XmlElement(ElementName = "BotSettings")]
    public BotSettings BotSettings { get; set; }
    public void InitDeveloperSettings()
    {
        if (BotSettings?.DeveloperAccessLevel is null || BotSettings?.DeveloperRoleId is null)
            return;
        switch (BotSettings?.DeveloperAccessLevel)
        {
            case "Owner":
            case "Admin":
                BotSettings.AdminRoles.AdminRoleId.Add((ulong)BotSettings.DeveloperRoleId);
                break;
            case "Mod":
                BotSettings.ModRoles.ModRoleId.Add((ulong)BotSettings.DeveloperRoleId);
                break;
            default:
                break;
        }
    }
}