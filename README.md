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
The modmail aspect of this bot relies on three aspects of your `.env` file:
1. The `MODMAIL_CHANNEL_ID`
	- This is the channel that the bot will send modmail messages to.
2. The `MOD_ROLE_ID`
	- This is the role that will be pinged when the default ping urgency is met
3. The `URGENCY_PING_LEVEL`
	- This is the level of which the `MOD_ROLE_ID` will be pinged. There are 5 levels:
		1. Level 1 &mdash; Pings will be sent with _every_ modmail message
		2. Level 2 &mdash; Pings will be sent with all modmail messages that aren't of _low_ urgency.
		3. Level 3 &mdash; Pings will be sent for _high_ and _urgent_ level messages.
		4. Level 4 &mdash; Pings will be sent for _urgent_ messages only.
		5. Level 5 &mdash; Pings will never be sent.
	- Keep in mind that users set the urgency themselves, and people have a tendency to overestimate their issues. You may want to set the ping level higher than you expect, or if spam issues present themselves, set the ping to 5.
	- Values of less than one or greater than five, _will not error_, however they will have no effects beyond those of level one and level five, respectfully.
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

## .env
The GitHub form of this application comes with `.example.env` with default key values. Replace those values with your own (the default values will produce no effect).

Once the values are replaced, rename the file to `.env` (removing the `.example`)

### Values
The .env file contains the following values
1. `DISCORD_TOKEN` : This is your personal Discord bot token
2. `CONN_STRING` : This is your SQL database connection string
3. `MODMAIL_CHANNEL_ID` : This is the channel that the bot will send modmail messages to
4. `MOD_ROLE_ID` : This is the role that will be pinged for moderator mail messages.
5. `URGENCY_PING_LEVEL` : This is the level at which moderator mail will ping the default moderator role.