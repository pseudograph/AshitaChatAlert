# FFXIInviteAssist
Reads FFXI chatlogs and automatically inserts invite commands into the clipboard based on a whitelist.

Reads the latest file stored in ./chatlogs.

Content whitelist: Keywords in message content to look for (e.g. holla, dem, mea, yhoat, vahzl). Case-insensitive. Recognises superstrings such as Teleport-Holla.
Region whitelist: Regions to accept messages from when reading /yell chat (e.g. jeuno). Case-insensitive. Recognises superstrings such as LowJeuno.

To use: Compile, place in the folder containing the chatlogs directory and run.
Copies the string "/pcmd add {playername}" to the clipboard when a valid message appears in the chatlog.

