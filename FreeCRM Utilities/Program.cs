namespace Util;

internal class Program
{
    static void Main(string[] args)
    {
        string version = "1.0.6";
        var released = DateOnly.FromDateTime(Convert.ToDateTime("6/25/2026"));

        // Sample of command-line arguments
        // --Rename:FormsToImaging --HideLogs --Remove:all --ShowLogs --Upgrade:"PATH"
        // --Rename:FormsToImaging --Keep:about,tags --Upgrade:"PATH"

        // Get the current directory where this application is running.
        var filePath = Directory.GetCurrentDirectory();

#if DEBUG
        // For local debugging set the path to my local work path.
        // This is where I copy the latest version of the FreeCRM files for testing.
        if (filePath.ToLower().Contains("\\freecrm-utilities\\freecrm utilities\\bin\\debug\\")) {
            filePath = @"C:\Working\CRM";
        }
#endif

        // Initialize all the static tool classes.
        RemoveModulesTools.SetFilePath(filePath);
        RenameTools.SetFilePath(filePath);
        UpgradeTools.SetFilePath(filePath);
        Utils.Init(version, filePath, released);

        if (args != null && args.Length > 0) {
            // App was started with command-line arguments, so just process all items and exit.
            bool fromProgramStart = true;

            // If the only command-line parameter was to update the path, then run in app mode.
            if (args.Count() == 1 && args[0].ToLower().StartsWith("/path:")) {
                fromProgramStart = false;
            }

            Utils.ProcessCommandLineArguments(args.ToList(), fromProgramStart);
            Environment.Exit(0);
        } else {
            // App was started without command-line arguments, so show the main menu.
            Utils.HomeScreen();
        }
    }
}
