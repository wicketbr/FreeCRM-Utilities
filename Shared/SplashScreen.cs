using System.Text;

namespace FreeCRM_Utilities;

public static class SplashScreen
{
    public enum SplashBorder
    {
        At,
        Block,
        Dots,
        Double,
        DoubleCorners,
        None,
        PlusBox,
        Shells,
        Simple,
        Single,
        Slash,
        Stars,
        Waves,
    }

    public class Splash
    {
        public string? Title { get; set; }
        public List<string>? Text { get; set; }
        public SplashBorder Border { get; set; } = SplashBorder.None;
        public System.ConsoleColor? BackgroundColor { get; set; }
        public System.ConsoleColor? ForegroundColor { get; set; }
        public SplashPadding? Padding { get; set; }
        public AsciiArt.Font? Font { get; set; }
    }

    public class SplashPadding
    {
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
    }

    public static string BorderCharacters(SplashBorder? border) {
        return border switch {
            SplashBorder.At => "@@@@@@@@",
            SplashBorder.Block => "▐▀▌▐▌▐▄▌",
            SplashBorder.Dots => "···::···",
            SplashBorder.Double => "╔═╗║║╚═╝",
            SplashBorder.DoubleCorners => "╔─╗││╚─╝",
            SplashBorder.None => String.Empty,
            SplashBorder.PlusBox => "+-+||+-+",
            SplashBorder.Shells => "########",
            SplashBorder.Simple => ".-.||'-'",
            SplashBorder.Single => "┌─┐││└─┘",
            SplashBorder.Slash => "////////",
            SplashBorder.Stars => "********",
            SplashBorder.Waves => "~~~~~~~~",
            _ => String.Empty
        };
    }

    public static string ConsoleColorToHex(ConsoleColor color) {
        System.Drawing.Color drawingColor = color switch {
            ConsoleColor.Black => System.Drawing.Color.FromArgb(0, 0, 0),
            ConsoleColor.DarkBlue => System.Drawing.Color.FromArgb(0, 0, 128),
            ConsoleColor.DarkGreen => System.Drawing.Color.FromArgb(0, 128, 0),
            ConsoleColor.DarkCyan => System.Drawing.Color.FromArgb(0, 128, 128),
            ConsoleColor.DarkRed => System.Drawing.Color.FromArgb(128, 0, 0),
            ConsoleColor.DarkMagenta => System.Drawing.Color.FromArgb(128, 0, 128),
            ConsoleColor.DarkYellow => System.Drawing.Color.FromArgb(128, 128, 0),
            ConsoleColor.Gray => System.Drawing.Color.FromArgb(192, 192, 192),
            ConsoleColor.DarkGray => System.Drawing.Color.FromArgb(128, 128, 128),
            ConsoleColor.Blue => System.Drawing.Color.FromArgb(0, 0, 255),
            ConsoleColor.Green => System.Drawing.Color.FromArgb(0, 255, 0),
            ConsoleColor.Cyan => System.Drawing.Color.FromArgb(0, 255, 255),
            ConsoleColor.Red => System.Drawing.Color.FromArgb(255, 0, 0),
            ConsoleColor.Magenta => System.Drawing.Color.FromArgb(255, 0, 255),
            ConsoleColor.Yellow => System.Drawing.Color.FromArgb(255, 255, 0),
            ConsoleColor.White => System.Drawing.Color.FromArgb(255, 255, 255),
            _ => System.Drawing.Color.Black
        };

        return System.Drawing.ColorTranslator.ToHtml(drawingColor);
    }

    private static List<string> AddPadding(List<string> lines, SplashPadding? padding, SplashBorder? border) {
        List<string> output = new List<string>();

        if (padding == null) {
            padding = new SplashPadding {
                Bottom = 0,
                Left = 0,
                Right = 0,
                Top = 0,
            };
        }

        var paddingLine = new string(' ', lines[0].Length + padding.Left + padding.Right);
        var paddingLeft = new string(' ', padding.Left);
        var paddingRight = new string(' ', padding.Right);
        var width = paddingLine.Length;

        for (int i = 0; i < padding.Top; i++) {
            output.Add(paddingLine);
        }

        foreach (var line in lines) {
            output.Add(paddingLeft + line + paddingRight);
        }

        for (int i = 0; i < padding.Bottom; i++) {
            output.Add(paddingLine);
        }

        if (border != null && border != SplashBorder.None) {
            var characters = BorderCharacters(border);
            if (!String.IsNullOrEmpty(characters)) {
                var l = output.ToList();
                output = new List<string>();

                output.Add(characters[0] + new string(characters[1], width) + characters[2]);

                foreach (var line in l) {
                    output.Add(characters[3] + line + characters[4]);
                }

                output.Add(characters[5] + new string(characters[6], width) + characters[7]);
            }
        }

        return output;
    }

    public static void DrawSplashScreen(Splash splash) {
        var previousBG = Console.BackgroundColor;
        var previousFG = Console.ForegroundColor;
        Console.OutputEncoding = Encoding.UTF8;

        string htmlStyle = "font-family: monospace;";
        if (splash.BackgroundColor.HasValue) {
            Console.BackgroundColor = splash.BackgroundColor.Value;
            htmlStyle += "background-color:" + ConsoleColorToHex(splash.BackgroundColor.Value) + ";";
        }

        if (splash.ForegroundColor.HasValue) {
            Console.ForegroundColor = splash.ForegroundColor.Value;
            htmlStyle += "color:" + ConsoleColorToHex(splash.ForegroundColor.Value) + ";";
        }

        List<string> lines = new List<string>();

        if (!String.IsNullOrWhiteSpace(splash.Title)) {
            if (splash.Font != null && splash.Font != AsciiArt.Font.FutureSmooth) {
                AsciiArt.SetFont(splash.Font.Value);
            }

            lines.AddRange(AsciiArt.TextToAsciiArtLines(splash.Title));
        }

        if (splash.Text != null && splash.Text.Count > 0) {
            lines.AddRange(splash.Text);
        }

        // Find the longest line.
        var longest = lines.Max(x => x.Length);
        lines = PadStringsToLength(lines, longest);

        if (splash.Padding != null || splash.Border != SplashBorder.None) {
            lines = AddPadding(lines, splash.Padding, splash.Border);
        }

        foreach (var line in lines) {
            // For debugging in LINQPad using the Util.WithStyle method
            // as the regular Console.WriteLine in LINQPad does not support
            // color styles.

            //Console.WriteLine(line);
            Console.WriteLine(Util.WithStyle(line, htmlStyle));
        }

        // Restore the colors
        Console.BackgroundColor = previousBG;
        Console.ForegroundColor = previousFG;
    }

    private static List<string> PadStringsToLength(List<string> strings, int length) {
        List<string> output = new List<string>();

        if (strings.Any()) {
            foreach (var str in strings) {
                output.Add(str.PadRight(length));
            }
        }

        return output;
    }
}