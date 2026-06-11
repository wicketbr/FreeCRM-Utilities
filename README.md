# FreeCRM-Utilities
A set of utilities for working with FreeCRM using a new
single command-line utility that can be used to rename
your copy of the solution, remove modules from the solution,
and upgrade from an older version to a new version.

The commands available vary on whether or not the program
was started from the console with command-line arguments.

The help page will show the following info:

```
The following single commands are available inside this application:

H / HELP / ?           Shows this help page.
HL / HIDELOGS          Disables logging results to the conole.
R / REMOVE             Remove one or more optional module from the application.
SL / SHOWLOGS          Enables logging results to the conole.
N / RENAME             Renames the FreeCRM application.
U / UPGRADE            Upgrades a previous application.
X / EXIT               Exit this application.

The following options can be chained together both in this application
or passed as command-line arguments from the console.

--HL / --HIDELOGS      Disables logging results to the conole.
--KEEP:Item1,Item2     Removes all optional modules except those specified.
--PATH:"new path"      Updates the working path.
--REMOVE:ALL           Removes all optional modules.
--REMOVE:Item1,Item2   Removes the specified modules.
--RENAME:MyApp         Renames the application from FreeCRM to MyApp.
--SL / --SHOWLOGS      Enables logging results to the conole.
--UPGRADE:"Path"       Upgrades the application from the app at the path.
--X / --EXIT           Exit this application.

Example:
  --Rename:MyApp --Remove:all --Upgrade:"C:\path to\old app"
```

This utility is available as an open source project from my github repo
at https://github.com/wicketbr/FreeCRM-Utilities.
If you wish to contribute feel free to add a pull request or report
an issue on github.