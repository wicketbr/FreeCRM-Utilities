using System.Reflection;
using System.Text;

namespace FreeCRM_Utilities;

internal class Program
{
    static void Main(string[] args)
    {
        // First, make sure that the app in the current directory has not already been renamed.
        var filePath = Directory.GetCurrentDirectory();

#if DEBUG
        // For local debugging set the path to my local work path.
        // This is where I copy the latest version of the FreeCRM files for testing.
        if (filePath.ToLower().Contains("\\freecrm-utilities\\freecrm utilities\\bin\\debug\\")) {
            filePath = @"C:\Working\CRM";
        }
#endif

        RemoveModulesTools.SetFilePath(filePath);
        RenameTools.SetFilePath(filePath);
        UpgradeTools.SetFilePath(filePath);

        if (args != null && args.Length > 0) {
            // App was started with command-line arguments, so just process all items and exit.
            Utils.ProcessCommandLineArguments(args.ToList(), true);
        } else {
            // App was started without command-line arguments, so show the main menu.
            Utils.HomeScreen();
        }
    }
}
