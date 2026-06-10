namespace FreeCRM_Utilities;

// Just in place to mimic the LINQPad Util functions used in the LINQPad development scripts
// allowing me to copy and paste the code here to maintain functionality.
public static class Util
{
    public static void ClearResults() {

    }

    public static System.Version LINQPadVersion {
        get {
            return new System.Version();
        }
    }

    // This mimics the Util.WithStyle function in LINQPad.
    // Here we are just using the Console.WriteLine wrapper for this
    // so we can ignore the htmlStyle as the Console styles handle
    // writing here.
    public static string WithStyle(string data, string htmlStyle) {
        return data;
    }
}