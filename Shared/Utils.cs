using System.Reflection;
using System.Text;
using System.Xml.Linq;
using static FreeCRM_Utilities.SplashScreen;
using static System.Net.Mime.MediaTypeNames;

namespace FreeCRM_Utilities;

public static class Utils
{
    private static bool _suppressLogs = false;

    public static void ConsoleLog(string? log = null)
    {
        if (!_suppressLogs) {
            if (!String.IsNullOrWhiteSpace(log)) {
                Console.WriteLine(log);
            } else {
                Console.WriteLine();
            }
        }
    }

    public static void DrawMessage(
        MessageItem message,
        System.ConsoleColor? overrideBackground = null,
        System.ConsoleColor? overrideForeground = null
    ) {
        var previousBG = Console.BackgroundColor;
        var previousFG = Console.ForegroundColor;
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
        htmlStyle += "background-color:" + ConsoleColorToHex(bgColor) + ";";

        Console.ForegroundColor = textColor;
        htmlStyle += "color:" + ConsoleColorToHex(textColor) + ";";

        Console.WriteLine(Util.WithStyle(message.Message, htmlStyle));

        Console.BackgroundColor = previousBG;
        Console.ForegroundColor = previousFG;
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
                    "© 2026 Bradley R. Wickett",
                    "",
                    "A set of utilities for working with a new instance of the",
                    "FreeCRM open-source Blazor application framework.",
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
        ConsoleLog("");
    }

    public static void HomeScreen(List<MessageItem>? messageItems = null)
    {
        DrawSplashScreen();

        if (messageItems != null && messageItems.Any()) {
            foreach(var msg in messageItems) {
                DrawMessage(msg);
            }

            Utils.ConsoleLog();
        }

        Utils.ConsoleLog("Select an Action or Press Enter to Exit:");
        Utils.ConsoleLog();

        if (RenameTools.AllowRename) {
            Utils.ConsoleLog("Rename - Rename the Application Namespace");
        }

        if (RemoveModulesTools.Modules.Count > 0) {
            Utils.ConsoleLog("Remove - Remove One or More Optional Modules");
        }

        Utils.ConsoleLog();
        Utils.ConsoleLog("Optionally you can enter any valid command-line parameters here to process them.");
        Utils.ConsoleLog();
        DrawPromptMessage("Enter your action:");
        

        var readLine = Console.ReadLine();

        if (String.IsNullOrWhiteSpace(readLine)) {
            return;
        } else if (readLine.Contains(" ")) {
            // This has multiple commands
            var items = readLine.Split(" ").ToList();
            Utils.ProcessCommandLineArguments(items);
        } else {
            Utils.ProcessCommandLineArguments(new List<string> { readLine });
        }
    }

    public static void ProcessCommandLineArguments(List<string> args, bool fromProgramStart = false)
    {
        Utils.DrawSplashScreen();

        //var modules = RemoveModulesTools.Modules;

        List<MessageItem> returnMessages = new List<MessageItem>();

        // Some items can only be processed as a single argument.
        // These are typically the single-digit commands.
        bool processingCompleted = false;
        if (args.Count == 1 && !fromProgramStart) {
            var arg = String.Empty + args[0];

            switch (arg.ToLower()) {
                case "rename":
                    if (RenameTools.AllowRename) {
                        // Get the rename name.
                        Utils.ConsoleLog("This will update the solution file names, generate new");
                        Utils.ConsoleLog("GUIDs for your projects, and update namespaces.");
                        Utils.ConsoleLog();
                        Utils.ConsoleLog("Enter a new name below for your project with");
                        Utils.ConsoleLog("no spaces, periods, or numbers.");
                        Utils.ConsoleLog();
                        Utils.ConsoleLog("You should use PascalCase for best result (eg: MyNewAppName).");
                        Utils.ConsoleLog();

                        DrawPromptMessage("Enter your new application name below:");

                        var appName = Console.ReadLine();

                        if (String.IsNullOrWhiteSpace(appName)) {
                            // No need to show anything, just return to the Main Menu.
                            returnMessages.Add(new MessageItem { Message = "Unable to rename application. No name was specified.", Error = true });
                        } else if (appName.Contains(" ")) {
                            returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + appName + "'. App namespace cannot contain spaces.", Error = true });
                        } else if (appName.Contains(".")) {
                            returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + appName + "'. App namespace cannot contain periods.", Error = true });
                        } else if (appName.Any(c => !Char.IsLetter(c))) {
                            returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + appName + "'. App namespace cannot contain any non-letter characters.", Error = true });
                        } else {
                            Utils.ConsoleLog("Renaming your project to: " + appName);

                            RenameTools.SetAppName(appName);

                            RenameTools.RenameFoldersAndSolutionFiles();
                            RenameTools.UpdateSolutionFile();
                            RenameTools.UpdateSecretsGuid();
                            RenameTools.RenameAllFileContents();

                            returnMessages.Add(new MessageItem { Message = "Renamed application namespace from 'CRM' to '" + appName + "'." });
                        }
                    } else {
                        returnMessages.Add(new MessageItem { Message = "The application has already been renamed.", Error = true });
                    }

                    processingCompleted = true;
                    break;

                case "remove":
                    var modules = RemoveModulesTools.Modules;

                    if (modules.Count == 0) {
                        Utils.ConsoleLog("There are no remaining modules to remove.");

                        if (!fromProgramStart) {
                            returnMessages.Add(new MessageItem { Message = "There are no remaining modules to remove." });
                        }

                        processingCompleted = true;
                    } else {
                        Utils.ConsoleLog("This will remove an individual module from FreeCRM.");
                        Utils.ConsoleLog("After removing a module you will need to open the");
                        Utils.ConsoleLog("app in Visual Studio and perform a build and resolve");
                        Utils.ConsoleLog("any remaining errors.");
                        Utils.ConsoleLog();

                        string padding = String.Empty;

                        if (modules.Count > 9) {
                            padding = " ";
                        }

                        if (modules.Count > 1) {
                            ConsoleLog(padding + "A - REMOVE ALL MODULES");
                        }

                        foreach (var module in modules.Index()) {
                            ConsoleLog((module.Index < 9 ? padding : "") + (module.Index + 1).ToString() + " - " + module.Item.Name);
                        }

                        ConsoleLog();
                        DrawPromptMessage("Select a Module to Remove or Press Enter to Return to the Main Menu");

                        var readLine = Console.ReadLine();

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

                            ConsoleLog();
                            ConsoleLog("Press Any Key to Return to the Main Menu");

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

                                ConsoleLog();
                                ConsoleLog("Press any key to continue:");

                                var readKey = Console.ReadKey();

                                if (RemoveModulesTools.Modules.Count > 0) {
                                    Console.Clear();

                                    ProcessCommandLineArguments(new List<string> { "remove" });
                                    return;
                                }
                            }
                        }

                        processingCompleted = true;
                    }

                    break;
            }
        }

        if (!processingCompleted) {
            foreach (var arg in args) {
                if (arg.ToLower() == "hidelogs") {
                    Utils.SuppressLogs();

                } else if (arg.ToLower() == "showlogs") {
                    Utils.ShowLogs();

                } else if (arg.ToLower().StartsWith("rename:")) {
                    if (RenameTools.AllowRename) {
                        var appName = arg.Substring(7).Trim();

                        if (String.IsNullOrWhiteSpace(appName)) {
                            // No need to show anything, just return to the Main Menu.
                            if (fromProgramStart) {
                                Utils.ConsoleLog("Unable to rename application. No name was specified after the rename: parameter.");
                            } else {
                                returnMessages.Add(new MessageItem { Message = "Unable to rename application. No name was specified after the rename: parameter.", Error = true });
                            }
                        } else if (appName.Contains(" ")) {
                            if (fromProgramStart) {
                                Utils.ConsoleLog("Unable to rename application '" + appName + "'. App namespace cannot contain spaces.");
                            } else {
                                returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + appName + "'. App namespace cannot contain spaces.", Error = true });
                            }
                        } else if (appName.Contains(".")) {
                            if (fromProgramStart) {
                                Utils.ConsoleLog("Unable to rename application '" + appName + "'. App namespace cannot contain periods.");
                            } else {
                                returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + appName + "'. App namespace cannot contain periods.", Error = true });
                            }
                        } else if (appName.Any(c => !Char.IsLetter(c))) {
                            if (fromProgramStart) {
                                Utils.ConsoleLog("Unable to rename application '" + appName + "'. App namespace cannot contain any non-letter characters.");
                            } else {
                                returnMessages.Add(new MessageItem { Message = "Unable to rename application '" + appName + "'. App namespace cannot contain any non-letter characters.", Error = true });
                            }
                        } else {
                            Utils.ConsoleLog("Renaming your project to: " + appName);

                            RenameTools.SetAppName(appName);
                            RenameTools.RenameFoldersAndSolutionFiles();
                            RenameTools.UpdateSolutionFile();
                            RenameTools.UpdateSecretsGuid();
                            RenameTools.RenameAllFileContents();

                            Utils.ConsoleLog("Renamed Solution from 'CRM' to '" + appName + "'.");

                            if (!fromProgramStart) {
                                returnMessages.Add(new MessageItem { Message = "Renamed Solution from 'CRM' to '" + appName + "'." });
                            }
                        }
                    } else {
                        if (!fromProgramStart) {
                            returnMessages.Add(new MessageItem { Message = "The application has already been renamed.", Error = true });
                        }
                    }

                } else if (arg.ToLower().StartsWith("remove:")) {
                    var remove = arg.Substring(7);

                    if (!String.IsNullOrWhiteSpace(remove)) {
                        if (RemoveModulesTools.Modules.Count == 0) {
                            Utils.ConsoleLog("There are no remaining modules to remove.");

                            if (!fromProgramStart) {
                                returnMessages.Add(new MessageItem { Message = "There are no remaining modules to remove.", Error = true });
                            }
                        } else {
                            if (remove.ToLower() == "all") {
                                Utils.ConsoleLog("Removing All Modules...");

                                foreach (var module in RemoveModulesTools.Modules) {
                                    var results = RemoveModulesTools.RemoveModule(module.Name);
                                    if (results.Any()) {
                                        foreach (var line in results) {
                                            Utils.ConsoleLog("Removed Module '" + line + "'");
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
                                            Utils.ConsoleLog(line);

                                            if (!fromProgramStart) {
                                                returnMessages.Add(new MessageItem { Message = line });
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    } else {
                        Utils.ConsoleLog("No module specified to remove.");

                        if (!fromProgramStart) {
                            returnMessages.Add(new MessageItem { Message = "No module specified to remove with the remove: command.", Error = true });
                        }
                    }

                } else if (arg.ToLower().StartsWith("keep:")) {
                    if (RemoveModulesTools.Modules.Count == 0) {
                        Utils.ConsoleLog("There are no remaining modules to remove.");

                        if (!fromProgramStart) {
                            returnMessages.Add(new MessageItem { Message = "There are no remaining modules to remove.", Error = true });
                        }
                    } else {
                        var keep = arg.Substring(5);

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

                } else {
                    Utils.ConsoleLog("Unknown command '" + arg + "'.");

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

    public static void ShowError(string errorMessage)
    {
        ShowErrors(new List<string> { errorMessage });
    }

    public static void ShowErrors(List<string> errorMessages)
    {
        Console.Clear();

        if (errorMessages.Count > 0) {
            if (errorMessages.Count == 1) {
                ConsoleLog("The following error occurred:");
                ConsoleLog();
                ConsoleLog(errorMessages[0]);
                ConsoleLog();
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