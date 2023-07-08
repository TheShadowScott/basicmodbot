# ModBot
_The Reimagined BasicModBot_

## Quick About
2 years ago I used a JavaScript discord library to familiarise myself with JS.
2 years later, its now a C# project with actual use planned for it.

## Features
* Basic Moderation Commands &mdash; Banning, kicking and muting (timing out) mebers is supported.
* Warnings and Logs &mdash; Loggable warnings are possible through a local SQL database (see [below](#sql-Setup))
* Modmail &mdash; By setting a default moderator role and discord channel, members can use a command by the bot to reach all mods at once. Features an additional customisable [urgency feature](#modmail).

## Modmail
The modmail aspect of this bot relies on three aspects of your `Settings.xml` file (all of which occur in the `<ModMailSettings>` tag:
1. `<ChannelId>`
	- This is the channel that the bot will send modmail messages to.
2. `<DefaultModeratorRole>`
	- This is the role that will be pinged when the default ping urgency is met
3. `<UrgencyPingLvl>`
	- This is the level of which the `DefaultModeratorRole` will be pinged. There are 5 levels:
		1. Level 1 &mdash; Pings will be sent with _every_ modmail message
		2. Level 2 &mdash; Pings will be sent with all modmail messages that aren't of _low_ urgency.
		3. Level 3 &mdash; Pings will be sent for _high_ and _urgent_ level messages.
		4. Level 4 &mdash; Pings will be sent for _urgent_ messages only.
		5. Level 5 &mdash; Pings will never be sent.
	- Keep in mind that users set the urgency themselves, and people have a tendency to overestimate their issues. You may want to set the ping level higher than you expect, or if spam issues present themselves, set the ping to 5.
## SQL Setup
For my SQL setup, I personally used SSMS for database and table creation, but as long as you are able to generate a connection string, you're good to go. A few things to note:
1. The name of your database _does not matter_.
2. The database must have a table by the name of *user_warnings*
	- That table must include the following columns:
		a. Id, of type `nvarchar(36)`,
		b. WarnType, of type `nvarchar(5)`,
		c. UserId, of type `bigint`,
		d. PerpetratorId, of type `bigint`,
		e. WarningReason, of type `text` and
		f. TimeMark, of type `nvarchar(28)`
	- See [here](#quick-table-setup) for a guide for the table.
3. Once the database is setup, set the connection string value in your [.env](#.env) file.


### Quick Table Setup
If you have your database ready, you can run the following to get your table set up:
```sql
CREATE TABLE user_warnings(
	Id NVARCHAR(36) NOT NULL,
	WarnType NVARCHAR(5),
	UserId BIGINT NOT NULL,
	PerpetratorId BIGINT NOT NULL,
	WarningReason TEXT,
	TimeMark NVARCHAR(28),

	PRIMARY KEY (Id)
);
```

## Settings.xml
Given the type checking nature of XML schema files, I decided using using an XML file for settings would be more fitting for this bot than a `.env` file. The full schema can be found in [Settings.xsd](./ModBot/Settings.xsd). _(The Settings.xml is not present in this repo given its customisable and private nature. An example is provided below.)_

### Example Settings.xml

```xml
<?xml version="1.0"?>
<?xml-model href="./Settings.xsd"?>

<!--
If Compiling in Visual Studio, make sure to add the above as 
the appropriate schema. VS Code should detect this automatically
-->
<!--
These Settings ARE NOT the actual bot values.
-->
<Settings>
	<InitSettings>
		<BotId>Cc1Wbj5r7Fe7HYe1A5an0Zn6.Adw24d.QMo4K0hQTAY8jhNUpPzoGKCZf8uUC4o9hhPygQ</BotId>
		<SQLString>Data Source=SRC;Initial Catalog=Database;Integrated Security=True</SQLString>
		<OwnerId>312781867949556946</OwnerId>
	</InitSettings>
	<ModMailSettings>
		<ChannelId>586895436707434127</ChannelId>
		<DefaultModeratorRole>974331301403587546</DefaultModeratorRole>
		<UrgencyPingLvl>0</UrgencyPingLvl>
	</ModMailSettings>
	<BotSettings>
		<ModRoles>
			<ModRoleId>128583866872312832</ModRoleId>
			<ModRoleId>667386822050733495</ModRoleId>
		</ModRoles>
		<AdminRoles>
			<AdminRoleId>369871204976799947</AdminRoleId>
			<AdminRoleId>024781138805157731</AdminRoleId>
		</AdminRoles>
		<LogEditLevel>Administrator</LogEditLevel>
	</BotSettings>
</Settings>

```