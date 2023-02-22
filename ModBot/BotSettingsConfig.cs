using System.Xml.Serialization;

namespace ModBot;

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
    public List<ulong> ModRoleIds { get; set; }
}

[XmlRoot(ElementName = "AdminRoles")]
public class AdminRoles
{

    [XmlElement(ElementName = "AdminRoleId")]
    public List<double> AdminRoleId { get; set; }
}

[XmlRoot(ElementName = "BotSettings")]
public class BotSettings
{

    [XmlElement(ElementName = "ModRoles")]
    public ModRoles ModRoles { get; set; }

    [XmlElement(ElementName = "AdminRoles")]
    public AdminRoles AdminRoles { get; set; }
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
}