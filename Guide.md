# Bot Guide

This guide offers a quick reference to the commands this bot has to offer, as well as the requirements

## Command List

### Command Breakdown
The command is listed as a forward slash, followed by a name, e.g., `/command`. Mandatory arguments are listed as full words with no surrounding braces. Optional arguments have square braces. Mutually exclusive arguments are found in curly braces with options being delimited by a vertical bar (`|`).

### Moderation Commands
_All of these commands send the user a direct message_

- `/ban User [Reason] [Number of Deletion Days]`
    - Requirements: `Permissions.BanMembers`
    - Logged command
- `/kick User [Reason]`
    - Requirements: `Permissions.KickMembers`
    - Logged Command
- `/unban User [Reason]`
    - Requirements: `Permissions.BanMembers`
    - Logged command
- `/warn User [Reason]`
    - Requirements: `Permissions.ManageMembers`
    - Logged Command
- `/timeout User {60 sec.|5 min.|10 min.|1 h.|1 d.|1 w.} [Reason]`
    - Requirements: `Permissions.ManageMembers`
    - Logged Command

### User Commands
_These commands **do not** send users a direct message_

- `/fetch User`
    - Requirements: `Permissions.ManageMembers`
    - Returns all logs associated with a specific user (from moderation commands), sorted in order of occurance
        - Each log has a unique GUID associated with it. This GUID is needed to alter or delete logs.
    - Not logged
- `/droplog ItemId`
    - Requirements: `Permissions.Administrator`, GUID of the log to drop
    - Removes the item with the listed Id from the database
    - Not logged by nature of the command
- `/alterlog ItemId NewReason`
    - Requirements: `Permissions.Administrator`, GUID of the log to alter
    - Changes the reasoning of a log. Does not edit the logged type
    - Not logged directly, but does affect another log.
- `/note User Note`
    - Requirements: `Permissions.ManageMembers`
    - Adds a note to a member's profile. This is different from a warn, as it does not send a DM to the undergoer.
- `/rename User Nick`
    - Requirements: `Permissions.ManageNicknames`
    - Changes the selected users nickname to the selected name

### Role Commands
_These commands **do not** send users a direct message_

- `/addrole User Role [Log={true|false}]`
    - Requirements: `Permissions.ManageRoles`
    - Adds the selected role to the selected user
    - _**Not**_ logged by default, however logging can be toggled manually
- `/removerole User Role [Log={true|false}]`
    - Requirements: `Permissions.ManageRoles`
    - Removes the selected role to the selected user
    - _**Not**_ logged by default, however logging can be toggled manually

### Modlogs
*** This section is under development. ***