using System.Text;

namespace Util;

public static class Utils
{
    private static System.ConsoleColor _defaultBackground = System.ConsoleColor.Black;
    private static System.ConsoleColor _defaultForeground = System.ConsoleColor.Black;
    private static string _filePath = "";
    private static DateOnly _released;
    private static bool _suppressLogs = false;
    private static string _version = String.Empty;

    public static void Init(string version, string filePath, DateOnly released)
    {
        _defaultBackground = Console.BackgroundColor;
        _defaultForeground = Console.ForegroundColor;
        _filePath = filePath;
        _released = released;
        _version = version;
    }

    public static void ConsoleLog(string? log = null, bool forceShow = false)
    {
        if (!_suppressLogs || forceShow) {
            if (!String.IsNullOrWhiteSpace(log)) {
                Console.WriteLine(log);
            } else {
                Console.WriteLine();
            }
        }
    }

    public static System.ConsoleColor DefaultConsoleBackground {
        get {
            return _defaultBackground;
        }
    }

    public static System.ConsoleColor DefaultConsoleForeground {
        get {
            return _defaultForeground;
        }
    }

    public static void DrawMessage(
        MessageItem message,
        System.ConsoleColor? overrideBackground = null,
        System.ConsoleColor? overrideForeground = null
    ){
        Console.OutputEncoding = Encoding.UTF8;

        System.ConsoleColor bgColor = message.Error
            ? System.ConsoleColor.DarkRed
            : System.ConsoleColor.Green;

        System.ConsoleColor textColor = message.Error
            ? System.ConsoleColor.White
            : System.ConsoleColor.Black;

        if (overrideBackground != null && overrideBackground.HasValue) {
            bgColor = overrideBackground.Value;
        }

        if (overrideForeground != null && overrideForeground.HasValue) {
            textColor = overrideForeground.Value;
        }

        string htmlStyle = "font-family: monospace;";
        Console.BackgroundColor = bgColor;
        htmlStyle += "background-color:" + SplashScreen.ConsoleColorToHex(bgColor) + ";";

        Console.ForegroundColor = textColor;
        htmlStyle += "color:" + SplashScreen.ConsoleColorToHex(textColor) + ";";

        Console.WriteLine(message.Message, htmlStyle);

        Console.BackgroundColor = _defaultBackground;
        Console.ForegroundColor = _defaultForeground;
    }

    public static void DrawPromptMessage(string message)
    {
        DrawMessage(new MessageItem { Message = message }, System.ConsoleColor.Cyan, System.ConsoleColor.Black);
    }

    public static void DrawSplashScreen()
    {
        Console.Clear();
        SplashScreen.DrawSplashScreen(new SplashScreen.Splash {
            Title = "FreeCRM Utilities",
            Text = new List<string> {
                    "© " + _released.Year.ToString() + " Bradley R. Wickett",
                    "v " + _version + " - " + _released.ToLongDateString(),
                    "",
                    "A set of utilities for working with a new instance of the",
                    "FreeCRM open-source Blazor application framework.",
                    "",
                    "PATH: " + _filePath,
                    "LOGGING: " + (_suppressLogs ? "OFF" : "ON"),
                },
            Border = SplashScreen.SplashBorder.Single,
            BackgroundColor = System.ConsoleColor.Blue,
            Font = AsciiArt.Font.FutureSmooth,
            ForegroundColor = System.ConsoleColor.White,
            Padding = new SplashScreen.SplashPadding {
                Bottom = 0,
                Left = 1,
                Right = 1,
                Top = 0,
            }
        });
        Console.WriteLine("");
    }

    public static void HomeScreen(List<MessageItem>? messageItems = null)
    {
        DrawSplashScreen();

        if (messageItems != null && messageItems.Any()) {
            foreach(var msg in messageItems) {
                DrawMessage(msg);
            }

            Console.WriteLine("");
        }

        Console.WriteLine("Select an Action or Press Enter to Exit:");
        Console.WriteLine("");

        Console.WriteLine("H - Show Help");

        if (RenameTools.AllowRename) {
            Console.WriteLine("N - Rename the Application Namespace");
        }

        if (RemoveModulesTools.Modules.Count > 0) {
            Console.WriteLine("R - Remove One or More Optional Modules");
        }

        Console.WriteLine("U - Upgrade an Older Version");
        Console.WriteLine("X - Exit");

        Console.WriteLine("");
        Console.WriteLine("Optionally you can enter any valid command-line parameters here to process them.");
        Console.WriteLine("");
        DrawPromptMessage("Enter your action:");

        var readLine = Console.ReadLine();

        if (String.IsNullOrWhiteSpace(readLine)) {
            HomeScreen();
        } else {
            ProcessCommandLineArguments(readLine);
        }
    }

    public static List<string> ParseCommandLineArguments(string? input)
    {
        List<string> output = new List<string>();

        if (String.IsNullOrWhiteSpace(input)) {
            return output;
        }

        int i = 0;
        while (i < input.Length) {
            // Skip whitespace
            while (i < input.Length && char.IsWhiteSpace(input[i])) {
                i++;
            }

            if (i >= input.Length) {
                break;
            }

            // Start of a new argument
            StringBuilder argument = new StringBuilder();

            // Arguments starting with / are treated as single parameters
            if (input[i] == '/') {
                argument.Append(input[i]);
                i++;

                // Read until we hit a space (considering quotes)
                bool inQuotes = false;
                while (i < input.Length) {
                    char c = input[i];

                    if (c == '"') {
                        inQuotes = !inQuotes;
                        argument.Append(c);
                        i++;
                    } else if (char.IsWhiteSpace(c) && !inQuotes) {
                        // End of this argument
                        break;
                    } else {
                        argument.Append(c);
                        i++;
                    }
                }

                output.Add(argument.ToString());
            } else {
                // Regular argument (not starting with /)
                bool inQuotes = false;
                while (i < input.Length) {
                    char c = input[i];

                    if (c == '"') {
                        inQuotes = !inQuotes;
                        i++;
                    } else if (char.IsWhiteSpace(c) && !inQuotes) {
                        break;
                    } else {
                        argument.Append(c);
                        i++;
                    }
                }

                if (argument.Length > 0) {
                    output.Add(argument.ToString());
                }
            }
        }

        return output;
    }

    public static void ProcessCommandLineArguments(List<string> args, bool fromProgramStart = false)
    {
        DrawSplashScreen();

        List<MessageItem> returnMessages = new List<MessageItem>();
        var readLine = String.Empty;

        // Some items can only be processed as a single argument.
        // These are typically the single-digit commands.
        if (args.Count == 1 && !fromProgramStart && !args[0].StartsWith("--")) {
            // This was called from within the app for a single function that doesn't start with --.

            var arg = String.Empty + args[0];
            switch (arg.ToLower()) {
                case "h":
                case "help":
                case "?":
                    Console.Clear();
                    // 80 Characters  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    Console.WriteLine("The following single commands are available inside this application:");
                    Console.WriteLine("");
                    Console.WriteLine("H / HELP / ?           Shows this help page.");
                    Console.WriteLine("HL / HIDELOGS          Disables logging results to the conole.");
                    Console.WriteLine("R / REMOVE             Remove one or more optional module from the application.");
                    Console.WriteLine("SL / SHOWLOGS          Enables logging results to the conole.");
                    Console.WriteLine("N / RENAME             Renames the FreeCRM application.");
                    Console.WriteLine("U / UPGRADE            Upgrades a previous application.");
                    Console.WriteLine("X / EXIT               Exit this application.");
                    Console.WriteLine("");
                    Console.WriteLine("The following options can be chained together both in this application");
                    Console.WriteLine("or passed as command-line arguments from the console.");
                    Console.WriteLine("");
                    Console.WriteLine("--HL / --HIDELOGS      Disables logging results to the conole.");
                    Console.WriteLine("--KEEP:Item1,Item2     Removes all optional modules except those specified.");
                    Console.WriteLine("--PATH:\"new path\"      Updates the working path.");
                    Console.WriteLine("--REMOVE:ALL           Removes all optional modules.");
                    Console.WriteLine("--REMOVE:Item1,Item2   Removes the specified modules.");
                    Console.WriteLine("--RENAME:MyApp         Renames the application from FreeCRM to MyApp.");
                    Console.WriteLine("--SL / --SHOWLOGS      Enables logging results to the conole.");
                    Console.WriteLine("--UPGRADE:\"Path\"       Upgrades the application from the app at the path.");
                    Console.WriteLine("--X / --EXIT           Exit this application.");
                    Console.WriteLine("");
                    Console.WriteLine("Example:");
                    Console.WriteLine("  --Rename:MyApp --Remove:all --Upgrade:\"C:\\path to\\old app\"");
                    Console.WriteLine("");

                    DrawPromptMessage("Enter your action or press enter to return to the Main Menu:");

                    readLine = Console.ReadLine();

                    if (!String.IsNullOrWhiteSpace(readLine)) {
                        ProcessCommandLineArguments(readLine);
                        return;
                    }

                    break;

                case "hl":
                case "hidelogs":
                    SuppressLogs();
                    break;

                case "n":
                case "rename":
                    Console.Clear();
                    if (RenameTools.AllowRename) {
                        // Get the rename name.
                        // 80 Characters  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                        Console.WriteLine("This will update the solution file names, generate new GUIDs for your projects");
                        Console.WriteLine("and update namespaces. Enter a new name below for your project with no spaces,");
                        Console.WriteLine("periods, or numbers.");
                        Console.WriteLine();
                        Console.WriteLine("You should use PascalCase for best result (eg: MyNewAppName).");
                        Console.WriteLine();

                        DrawPromptMessage("Enter your new application name below:");

                        readLine = Console.ReadLine();

                        if (String.IsNullOrWhiteSpace(readLine)) {
                            // No need to show anything, just return to the Main Menu.
                            returnMessages.Add(new MessageItem { Message = "Unable to rename application. No name was specified.", Error = true });
                        } else if (readLine.Contains(" ")) {
                            returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + readLine + "'. App namespace cannot contain spaces.", Error = true });
                        } else if (readLine.Contains(".")) {
                            returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + readLine + "'. App namespace cannot contain periods.", Error = true });
                        } else if (readLine.Any(c => !Char.IsLetter(c))) {
                            returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + readLine + "'. App namespace cannot contain any non-letter characters.", Error = true });
                        } else {
                            Console.WriteLine("Renaming your project to: " + readLine);

                            RenameTools.SetAppName(readLine);

                            RenameTools.RenameFoldersAndSolutionFiles();
                            RenameTools.UpdateSolutionFile();
                            RenameTools.UpdateSecretsGuid();
                            RenameTools.RenameAllFileContents();

                            returnMessages.Add(new MessageItem { Message = "Renamed application namespace from 'CRM' to '" + readLine + "'." });
                        }
                    } else {
                        returnMessages.Add(new MessageItem { Message = "The application has already been renamed.", Error = true });
                    }
                    break;

                case "r":
                case "remove":
                    Console.Clear();
                    var modules = RemoveModulesTools.Modules;

                    if (modules.Count == 0) {
                        Console.WriteLine("There are no remaining modules to remove.");

                        if (!fromProgramStart) {
                            returnMessages.Add(new MessageItem { Message = "There are no remaining modules to remove." });
                        }
                    } else {
                        // 80 Characters  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                        Console.WriteLine("This will remove an individual module from FreeCRM. After removing a module");
                        Console.WriteLine("you will need to open the app in Visual Studio and perform a build and");
                        Console.WriteLine("resolve any remaining errors.");
                        Console.WriteLine();

                        string padding = String.Empty;

                        if (modules.Count > 9) {
                            padding = " ";
                        }

                        if (modules.Count > 1) {
                            Console.WriteLine(padding + "A - REMOVE ALL MODULES");
                        }

                        foreach (var module in modules.Index()) {
                            Console.WriteLine((module.Index < 9 ? padding : "") + (module.Index + 1).ToString() + " - " + module.Item.Name);
                        }

                        Console.WriteLine();
                        DrawPromptMessage("Select a Module to Remove or Press Enter to Return to the Main Menu");

                        readLine = Console.ReadLine();

                        if (modules.Count > 1 && (readLine == "a" || readLine == "A")) {
                            ConsoleLog("Removing All Modules...");

                            foreach (var module in modules) {
                                var results = RemoveModulesTools.RemoveModule(module.Name);
                                if (results.Any()) {
                                    foreach (var line in results) {
                                        ConsoleLog(line);
                                    }
                                }
                            }

                            Console.WriteLine();
                            DrawPromptMessage("Press Any Key to Return to the Main Menu");
                            Console.ReadKey();

                            if (!fromProgramStart) {
                                returnMessages.Add(new MessageItem { Message = "Removed All Modules" });
                            }
                        } else {
                            int input = 0;
                            try {
                                input = Convert.ToInt32(readLine);
                            } catch { }

                            var selectedItem = String.Empty;

                            if (input > 0 && input < modules.Count + 1) {
                                selectedItem = modules[input - 1].Name;
                            }

                            if (!String.IsNullOrWhiteSpace(selectedItem)) {
                                Console.Clear();

                                var results = RemoveModulesTools.RemoveModule(selectedItem);
                                if (results.Any()) {
                                    foreach (var item in results) {
                                        ConsoleLog(item);
                                    }
                                }

                                Console.WriteLine();
                                DrawPromptMessage("Press any key to continue:");

                                Console.ReadKey();

                                if (RemoveModulesTools.Modules.Count > 0) {
                                    Console.Clear();

                                    ProcessCommandLineArguments(new List<string> { "remove" });
                                    return;
                                }
                            }
                        }
                    }
                    break;

                case "sl":
                case "showlogs":
                    ShowLogs();
                    break;

                case "u":
                case "update":
                case "upgrade":
                    Console.Clear();
                    // 80 Characters  12345678901234567890123456789012345678901234567890123456789012345678901234567890
                    Console.WriteLine("This will upgrade your application to the latest version of the FreeCRM application.");
                    Console.WriteLine("");
                    Console.WriteLine("You should have already run the 'rename' command to rename the namespace to match");
                    Console.WriteLine("the namespace of the application you are upgrading.");
                    Console.WriteLine("");
                    Console.WriteLine("Also, you should have already run the 'remove' command to remove any modules that");
                    Console.WriteLine("you are not using in your application if you have removed modules from the");
                    Console.WriteLine("application you are upgrading.");
                    Console.WriteLine();

                    DrawPromptMessage("Enter the path to the old version of your application below:");
                    readLine = Console.ReadLine();

                    UpgradeTools.DoUpgrade(readLine, fromProgramStart);

                    Console.WriteLine();
                    DrawPromptMessage("Press Any Key to Return to the Main Menu");
                    Console.ReadKey();
                    break;

                case "x":
                case "exit":
                    Environment.Exit(0);
                    break;
            }
        } else {
            // This was either started from the command-line with arguments, or this is using
            // multiple items for arguments.

            foreach (var arg in args) {
                var argument = arg;

                if (argument.StartsWith("--")) {
                    argument = argument.Substring(2);
                }

                if (argument.ToLower() == "exit" || argument.ToLower() == "x") {
                    Environment.Exit(0);

                } else if (argument.ToLower() == "hl" || argument.ToLower() == "hidelogs") {
                    SuppressLogs();

                } else if (argument.ToLower() == "sl" || argument.ToLower() == "showlogs") {
                    ShowLogs();

                } else if (argument.ToLower().StartsWith("n:") || argument.ToLower().StartsWith("rename:")) {
                    if (RenameTools.AllowRename) {
                        string appName = String.Empty;

                        if (argument.ToLower().StartsWith("n:")) {
                            appName = argument.Substring(2).Trim();
                        } else if (argument.ToLower().StartsWith("rename:")) {
                            appName = argument.Substring(7).Trim();
                        }

                        if (String.IsNullOrWhiteSpace(appName)) {
                            // No need to show anything, just return to the Main Menu.
                            if (fromProgramStart) {
                                ConsoleLog("Unable to rename application. No name was specified after the rename: parameter.");
                            } else {
                                returnMessages.Add(new MessageItem { Message = "Unable to rename application. No name was specified after the rename: parameter.", Error = true });
                            }
                        } else if (appName.Contains(" ")) {
                            if (fromProgramStart) {
                                ConsoleLog("Unable to rename application '" + appName + "'. App namespace cannot contain spaces.");
                            } else {
                                returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + appName + "'. App namespace cannot contain spaces.", Error = true });
                            }
                        } else if (appName.Contains(".")) {
                            if (fromProgramStart) {
                                ConsoleLog("Unable to rename application '" + appName + "'. App namespace cannot contain periods.");
                            } else {
                                returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + appName + "'. App namespace cannot contain periods.", Error = true });
                            }
                        } else if (appName.Any(c => !Char.IsLetter(c))) {
                            if (fromProgramStart) {
                                ConsoleLog("Unable to rename application '" + appName + "'. App namespace cannot contain any non-letter characters.");
                            } else {
                                returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + appName + "'. App namespace cannot contain any non-letter characters.", Error = true });
                            }
                        } else {
                            ConsoleLog("Renaming your project to: " + appName);

                            RenameTools.SetAppName(appName);
                            RenameTools.RenameFoldersAndSolutionFiles();
                            RenameTools.UpdateSolutionFile();
                            RenameTools.UpdateSecretsGuid();
                            RenameTools.RenameAllFileContents();

                            ConsoleLog("Renamed Solution from 'CRM' to '" + appName + "'.");

                            if (!fromProgramStart) {
                                returnMessages.Add(new MessageItem { Message = "Renamed Solution from 'CRM' to '" + appName + "'." });
                            }
                        }
                    } else {
                        if (!fromProgramStart) {
                            returnMessages.Add(new MessageItem { Message = "The application has already been renamed.", Error = true });
                        }
                    }

                } else if (argument.ToLower().StartsWith("p:") || argument.ToLower().StartsWith("path:")) {
                    string path = String.Empty;

                    if (argument.ToLower().StartsWith("p:")) {
                        path = argument.Substring(2);
                    } else if (argument.ToLower().StartsWith("path:")) {
                        path = argument.Substring(5);
                    }

                    path = path.Replace("\"", "");

                    if (System.IO.Directory.Exists(path)) {
                        // Make sure the path has changed.
                        if (path != _filePath) {
                            // Make sure the path contains a solution file.
                            // *.sln*
                            var pathSolutionFile = System.IO.Directory.GetFiles(path, "*.sln*");

                            if (pathSolutionFile.Count() == 0) {
                                ConsoleLog("Path does not contain a solution file (.sln/.slnx).");

                                if (!fromProgramStart) {
                                    returnMessages.Add(new MessageItem { Message = "Unable to change path, Path does not contain a solution file (.sln/.slnx).", Error = true });
                                }
                            } else {
                                _filePath = path;
                                RemoveModulesTools.SetFilePath(path);
                                RenameTools.SetFilePath(path);
                                UpgradeTools.SetFilePath(path);

                                ConsoleLog("Path updated to '" + _filePath + "'");

                                if (!fromProgramStart) {
                                    returnMessages.Add(new MessageItem { Message = "Path updated to:" });
                                    returnMessages.Add(new MessageItem { Message = _filePath });
                                }
                            }
                        } else {
                            ConsoleLog("Path unchanged.");

                            if (!fromProgramStart) {
                                returnMessages.Add(new MessageItem { Message = "Path unchanged." });
                            }
                        }

                    } else {
                        ConsoleLog("Unable to update path, '" + path + "' does not exist.");

                        if (!fromProgramStart) {
                            returnMessages.Add(new MessageItem { Message = "Unable to update the path. The specified directory does not exist.", Error = true });
                        }
                    }

                } else if (argument.ToLower().StartsWith("r:") || argument.ToLower().StartsWith("remove:")) {
                    string remove = String.Empty;

                    if (argument.ToLower().StartsWith("r:")) {
                        remove = argument.Substring(2);
                    } else if (argument.ToLower().StartsWith("remove:")) {
                        remove = argument.Substring(7);
                    }

                    if (!String.IsNullOrWhiteSpace(remove)) {
                        if (RemoveModulesTools.Modules.Count == 0) {
                            ConsoleLog("There are no remaining modules to remove.");

                            if (!fromProgramStart) {
                                returnMessages.Add(new MessageItem { Message = "There are no remaining modules to remove.", Error = true });
                            }
                        } else {
                            if (remove.ToLower() == "a" || remove.ToLower() == "all") {
                                ConsoleLog("Removing All Modules...");

                                foreach (var module in RemoveModulesTools.Modules) {
                                    var results = RemoveModulesTools.RemoveModule(module.Name);
                                    if (results.Any()) {
                                        foreach (var line in results) {
                                            ConsoleLog("Removed Module '" + line + "'");
                                        }
                                    }
                                }

                                if (!fromProgramStart) {
                                    returnMessages.Add(new MessageItem { Message = "Removed All Modules" });
                                }
                            } else {
                                var toRemove = remove.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                                foreach (var item in toRemove) {
                                    var results = RemoveModulesTools.RemoveModule(item);
                                    if (results.Any()) {
                                        foreach (var line in results) {
                                            ConsoleLog(line);

                                            if (!fromProgramStart) {
                                                returnMessages.Add(new MessageItem { Message = line });
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    } else {
                        ConsoleLog("No module specified to remove.");

                        if (!fromProgramStart) {
                            returnMessages.Add(new MessageItem { Message = "No module specified to remove with the remove: command.", Error = true });
                        }
                    }

                } else if (argument.ToLower().StartsWith("keep:")) {
                    if (RemoveModulesTools.Modules.Count == 0) {
                        ConsoleLog("There are no remaining modules to remove.");

                        if (!fromProgramStart) {
                            returnMessages.Add(new MessageItem { Message = "There are no remaining modules to remove.", Error = true });
                        }
                    } else {
                        var keep = argument.Substring(5);

                        if (!String.IsNullOrWhiteSpace(keep)) {
                            var toKeep = keep.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                            foreach (var module in RemoveModulesTools.Modules) {
                                if (!toKeep.Any(x => x.ToLower() == module.Name.ToLower())) {
                                    var results = RemoveModulesTools.RemoveModule(module.Name);
                                    if (results.Any()) {
                                        foreach (var line in results) {
                                            ConsoleLog(line);

                                            if (!fromProgramStart) {
                                                returnMessages.Add(new MessageItem { Message = line });
                                            }
                                        }
                                    }
                                }
                            }
                        } else {
                            ConsoleLog("No module specified to keep.");

                            if (!fromProgramStart) {
                                returnMessages.Add(new MessageItem { Message = "No module specified to keep with the keep: command.", Error = true });
                            }
                        }
                    }

                } else if (argument.ToLower().StartsWith("u:") || argument.ToLower().StartsWith("update:") || argument.ToLower().StartsWith("upgrade:")) {
                    string path = String.Empty;

                    if (argument.ToLower().StartsWith("u:")) {
                        path = argument.Substring(2);
                    } else if (argument.ToLower().StartsWith("update:")) {
                        path = argument.Substring(7);
                    } else if (argument.ToLower().StartsWith("upgrade:")) {
                        path = argument.Substring(8);
                    }

                    path = path.Replace("\"", "");

                    UpgradeTools.DoUpgrade(path, fromProgramStart);
                } else {
                    ConsoleLog("Unknown command '" + arg + "'.");

                    if (!fromProgramStart) {
                        returnMessages.Add(new MessageItem { Message = "Unknown command '" + arg + "'.", Error = true });
                    }
                }
            }
        }

        if (!fromProgramStart) {
            HomeScreen(returnMessages);
        }
    }

    public static void ProcessCommandLineArguments(string arguments)
    {
        ProcessCommandLineArguments(ParseCommandLineArguments(arguments));
    }

    public static void ShowError(string errorMessage)
    {
        ShowErrors(new List<string> { errorMessage });
    }

    public static void ShowErrors(List<string> errorMessages)
    {
        Console.Clear();

        if (errorMessages.Count > 0) {
            if (errorMessages.Count == 1) {
                Console.WriteLine("The following error occurred:");
                Console.WriteLine();
                Console.WriteLine(errorMessages[0]);
                Console.WriteLine();
                DrawPromptMessage("Press any key to return to the main menu.");

                Console.ReadLine();

                HomeScreen();
            }
        }
    }

    public static void ShowLogs()
    {
        _suppressLogs = false;
    }

    public static void SuppressLogs()
    {
        _suppressLogs = true;
    }
}