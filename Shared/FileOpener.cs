using System.Diagnostics;

namespace FreeCRM_Utilities;

public static class FileOpener
{
    public static void OpenInBrowser(string path) {
        try {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) {
                // Windows needs UseShellExecute = true to find the default app handler
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            } else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux)) {
                // Linux typically uses 'xdg-open' for default actions
                Process.Start("xdg-open", path);
            } else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX)) {
                // macOS uses the 'open' command
                Process.Start("open", path);
            }
        } catch (Exception ex) {
            Utils.ConsoleLog($"Could not open file: {ex.Message}");
        }
    }
}