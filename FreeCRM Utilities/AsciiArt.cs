namespace Util;

public static class AsciiArt
{
    // Note: not all character sets contain all characters, so choose an appropriate font depending on your needs.
    // For example, the CyberMedium is a very limited character set, while the FutureSmooth set has most characters.

    private static Font _font = Font.FutureSmooth;

    public enum Font
    {
        AnsiRegular,
        AnsiShadow,
        Ascii9,
        Classy,
        CoderMini,
        CyberMedium,
        FutureSmooth,
    }

    // https://patorjk.com/software/taag/#p=display&f=ANSI+Regular&t=ABCDEFGHIJKLMNOPQRSTUVWXYZ%0A1234567890%0A%60%7e!%40%24%25%26*()-_%3d%2b%0A%5c%7c%5b%7b%5d%7d%3b%3a%27%22%2c.%2f%3f
    public static List<Letter> Letters_AnsiRegular {
        get {
            return new List<Letter>() {
                new Letter { Character = " ",  ASCII = new List<string> { "     ", "     ", "     ", "     ", "     ", "     ", "     " }},
                new Letter { Character = "A",  ASCII = new List<string> { " █████  ","██   ██ ","███████ ","██   ██ ","██   ██ ","        " }},
                new Letter { Character = "B",  ASCII = new List<string> { "██████  ","██   ██ ","██████  ","██   ██ ","██████  ","        " }},
                new Letter { Character = "C",  ASCII = new List<string> { " ██████ ","██      ","██      ","██      "," ██████ ","        " }},
                new Letter { Character = "D",  ASCII = new List<string> { "██████  ","██   ██ ","██   ██ ","██   ██ ","██████  ","        " }},
                new Letter { Character = "E",  ASCII = new List<string> { "███████ ","██      ","█████   ","██      ","███████ ","        " }},
                new Letter { Character = "F",  ASCII = new List<string> { "███████ ","██      ","█████   ","██      ","██      ","        " }},
                new Letter { Character = "G",  ASCII = new List<string> { " ██████  ","██       ","██   ███ ","██    ██ "," ██████  ","         " }},
                new Letter { Character = "H",  ASCII = new List<string> { "██   ██ ","██   ██ ","███████ ","██   ██ ","██   ██ ","        " }},
                new Letter { Character = "I",  ASCII = new List<string> { "██ ","██ ","██ ","██ ","██ ","   " }},
                new Letter { Character = "J",  ASCII = new List<string> { "     ██ ","     ██ ","     ██ ","██   ██ "," █████  ","        " }},
                new Letter { Character = "K",  ASCII = new List<string> { "██   ██ ","██  ██  ","█████   ","██  ██  ","██   ██ ","        " }},
                new Letter { Character = "L",  ASCII = new List<string> { "██      ","██      ","██      ","██      ","███████ ","        " }},
                new Letter { Character = "M",  ASCII = new List<string> { "███    ███ ","████  ████ ","██ ████ ██ ","██  ██  ██ ","██      ██ ","           " }},
                new Letter { Character = "N",  ASCII = new List<string> { "███    ██ ","████   ██ ","██ ██  ██ ","██  ██ ██ ","██   ████ ","          " }},
                new Letter { Character = "O",  ASCII = new List<string> { " ██████  ","██    ██ ","██    ██ ","██    ██ "," ██████  ","         " }},
                new Letter { Character = "P",  ASCII = new List<string> { "██████  ","██   ██ ","██████  ","██      ","██      ","        " }},
                new Letter { Character = "Q",  ASCII = new List<string> { " ██████  ","██    ██ ","██    ██ ","██ ▄▄ ██ "," ██████  ","    ▀▀   " }},
                new Letter { Character = "R",  ASCII = new List<string> { "██████  ","██   ██ ","██████  ","██   ██ ","██   ██ ","        " }},
                new Letter { Character = "S",  ASCII = new List<string> { "███████ ","██      ","███████ ","     ██ ","███████ ","        " }},
                new Letter { Character = "T",  ASCII = new List<string> { "████████ ","   ██    ","   ██    ","   ██    ","   ██    ","         " }},
                new Letter { Character = "U",  ASCII = new List<string> { "██    ██ ","██    ██ ","██    ██ ","██    ██ "," ██████  ","         " }},
                new Letter { Character = "V",  ASCII = new List<string> { "██    ██ ","██    ██ ","██    ██ "," ██  ██  ","  ████   ","         " }},
                new Letter { Character = "W",  ASCII = new List<string> { "██     ██ ","██     ██ ","██  █  ██ ","██ ███ ██ "," ███ ███  ","          " }},
                new Letter { Character = "X",  ASCII = new List<string> { "██   ██ "," ██ ██  ","  ███   "," ██ ██  ","██   ██ ","        " }},
                new Letter { Character = "Y",  ASCII = new List<string> { "██    █ "," ██  ██ ","  ████  ","   ██   ","   ██   ","        " }},
                new Letter { Character = "Z",  ASCII = new List<string> { " ███████ ","    ███  ","   ███   ","  ███    "," ███████ ","         " }},
                new Letter { Character = "1",  ASCII = new List<string> { " ██ ","███ "," ██ "," ██ "," ██ ","    " }},
                new Letter { Character = "2",  ASCII = new List<string> { "██████  ","     ██ "," █████  ","██      ","███████ ","        " }},
                new Letter { Character = "3",  ASCII = new List<string> { "██████  ","     ██ "," █████  ","     ██ ","██████  ","        " }},
                new Letter { Character = "4",  ASCII = new List<string> { "██   ██ ","██   ██ ","███████ ","     ██ ","     ██ ","        " }},
                new Letter { Character = "5",  ASCII = new List<string> { "███████ ","██      ","███████ ","     ██ ","███████ ","        " }},
                new Letter { Character = "6",  ASCII = new List<string> { " ██████ ","██      ","███████ ","██    █ "," ██████ ","        " }},
                new Letter { Character = "7",  ASCII = new List<string> { " ██████ ","      █ ","     ██ ","    ██  ","    ██  ","        " }},
                new Letter { Character = "8",  ASCII = new List<string> { "  █████ "," ██   █ ","  █████ "," ██   █ ","  █████ ","        " }},
                new Letter { Character = "9",  ASCII = new List<string> { "  █████ "," ██   █ ","  █████ ","      █ ","  █████ ","        " }},
                new Letter { Character = "0",  ASCII = new List<string> { "  ██████ "," ██  ███ "," ██ ██ █ "," ████  █ ","  ██████ ","         " }},
                new Letter { Character = "!",  ASCII = new List<string> { "██ ","██ ","██ ","   ","██ ","   " }},
                new Letter { Character = "@",  ASCII = new List<string> { " ██████  ","██    ██ ","██ ██ ██ ","██ ██ ██ "," █ ████  ","         " }},
                new Letter { Character = "$",  ASCII = new List<string> { "▄▄███▄▄ ","██      ","███████ ","     ██ ","███████ ","  ▀▀▀   " }},
                new Letter { Character = "%",  ASCII = new List<string> { "██  ██ ","   ██  ","  ██   "," ██    ","██  ██ ","       " }},
                new Letter { Character = "&",  ASCII = new List<string> { "   ██    ","   ██    ","████████ ","██  ██   ","██████   ","         " }},
                new Letter { Character = "*",  ASCII = new List<string> { "      ","▄ ██  "," ████ ","▀ ██  ","      ","      " }},
                new Letter { Character = "(",  ASCII = new List<string> { "  █ "," ██ "," ██ "," ██ ","  █ ","    " }},
                new Letter { Character = ")",  ASCII = new List<string> { " ██ ","  █ ","  █ ","  █ "," ██ ","    " }},
                new Letter { Character = "-",  ASCII = new List<string> { "      ","      "," ████ ","      ","      ","      " }},
                new Letter { Character = "_",  ASCII = new List<string> { "        ","        ","        ","        "," ██████ ","        " }},
                new Letter { Character = "[",  ASCII = new List<string> { "███ ","██  ","██  ","██  ","███ ","    " }},
                new Letter { Character = "]",  ASCII = new List<string> { "███ "," ██ "," ██ "," ██ ","███ ","    " }},
                new Letter { Character = ";",  ASCII = new List<string> { "   ","██ ","   ","▄█ ","▀  ","   " }},
                new Letter { Character = ":",  ASCII = new List<string> { "   ","██ ","   ","██ ","   ","   " }},
                new Letter { Character = ",",  ASCII = new List<string> { "   ","   ","   ","   ","▄█ ","   " }},
                new Letter { Character = ".",  ASCII = new List<string> { "   ","   ","   ","   ","██ ","   " }},
                new Letter { Character = "/",  ASCII = new List<string> { "    ██ ","   ██  ","  ██   "," ██    ","██     ","       " }},
                new Letter { Character = "?",  ASCII = new List<string> { "██████  ","     ██ ","  ▄███  ","  ▀▀    ","  ██    ","        " }},
            };
        }
    }

    public static List<Letter> Letters_AnsiShadow {
        get {
            return new List<Letter>() {
                new Letter { Character = " ",  ASCII = new List<string> { "     ", "     ", "     ", "     ", "     ", "     ", "     " }},
                new Letter { Character = "A",  ASCII = new List<string> { " █████╗  ","██╔══██╗ ","███████║ ","██╔══██║ ","██║  ██║ ","╚═╝  ╚═╝ " }},
                new Letter { Character = "B",  ASCII = new List<string> { "██████╗  ","██╔══██╗ ","██████╔╝ ","██╔══██╗ ","██████╔╝ ","╚═════╝  " }},
                new Letter { Character = "C",  ASCII = new List<string> { " ██████╗ ","██╔════╝ ","██║      ","██║      ","╚██████╗ "," ╚═════╝ " }},
                new Letter { Character = "D",  ASCII = new List<string> { "██████╗  ","██╔══██╗ ","██║  ██║ ","██║  ██║ ","██████╔╝ ","╚═════╝  " }},
                new Letter { Character = "E",  ASCII = new List<string> { "███████╗ ","██╔════╝ ","█████╗   ","██╔══╝   ","███████╗ ","╚══════╝ " }},
                new Letter { Character = "F",  ASCII = new List<string> { "███████╗ ","██╔════╝ ","█████╗   ","██╔══╝   ","██║      ","╚═╝      " }},
                new Letter { Character = "G",  ASCII = new List<string> { " ██████╗  ","██╔════╝  ","██║  ███╗ ","██║   ██║ ","╚██████╔╝ "," ╚═════╝  " }},
                new Letter { Character = "H",  ASCII = new List<string> { "██╗  ██╗ ","██║  ██║ ","███████║ ","██╔══██║ ","██║  ██║ ","╚═╝  ╚═╝ " }},
                new Letter { Character = "I",  ASCII = new List<string> { "██╗ ","██║ ","██║ ","██║ ","██║ ","╚═╝ " }},
                new Letter { Character = "J",  ASCII = new List<string> { "     ██╗ ","     ██║ ","     ██║ ","██   ██║ ","╚█████╔╝ "," ╚════╝  " }},
                new Letter { Character = "K",  ASCII = new List<string> { "██╗  ██╗ ","██║ ██╔╝ ","█████╔╝  ","██╔═██╗  ","██║  ██╗ ","╚═╝  ╚═╝ " }},
                new Letter { Character = "L",  ASCII = new List<string> { "██╗      ","██║      ","██║      ","██║      ","███████╗ ","╚══════╝ " }},
                new Letter { Character = "M",  ASCII = new List<string> { "███╗   ███╗ ","████╗ ████║ ","██╔████╔██║ ","██║╚██╔╝██║ ","██║ ╚═╝ ██║ ","╚═╝     ╚═╝ " }},
                new Letter { Character = "N",  ASCII = new List<string> { "███╗   ██╗ ","████╗  ██║ ","██╔██╗ ██║ ","██║╚██╗██║ ","██║ ╚████║ ","╚═╝  ╚═══╝ " }},
                new Letter { Character = "O",  ASCII = new List<string> { " ██████╗  ","██╔═══██╗ ","██║   ██║ ","██║   ██║ ","╚██████╔╝ "," ╚═════╝  " }},
                new Letter { Character = "P",  ASCII = new List<string> { "██████╗  ","██╔══██╗ ","██████╔╝ ","██╔═══╝  ","██║      ","╚═╝      " }},
                new Letter { Character = "Q",  ASCII = new List<string> { " ██████╗  ","██╔═══██╗ ","██║   ██║ ","██║▄▄ ██║ ","╚██████╔╝ "," ╚══▀▀═╝  " }},
                new Letter { Character = "R",  ASCII = new List<string> { "██████╗  ","██╔══██╗ ","██████╔╝ ","██╔══██╗ ","██║  ██║ ","╚═╝  ╚═╝ " }},
                new Letter { Character = "S",  ASCII = new List<string> { "███████╗ ","██╔════╝ ","███████╗ ","╚════██║ ","███████║ ","╚══════╝ " }},
                new Letter { Character = "T",  ASCII = new List<string> { "████████╗ ","╚══██╔══╝ ","   ██║    ","   ██║    ","   ██║    ","   ╚═╝    " }},
                new Letter { Character = "U",  ASCII = new List<string> { "██╗   ██╗ ","██║   ██║ ","██║   ██║ ","██║   ██║ ","╚██████╔╝ "," ╚═════╝  " }},
                new Letter { Character = "V",  ASCII = new List<string> { "██╗   ██╗ ","██║   ██║ ","██║   ██║ ","╚██╗ ██╔╝ "," ╚████╔╝  ","  ╚═══╝   " }},
                new Letter { Character = "W",  ASCII = new List<string> { "██╗    ██╗ ","██║    ██║ ","██║ █╗ ██║ ","██║███╗██║ ","╚███╔███╔╝ "," ╚══╝╚══╝  " }},
                new Letter { Character = "X",  ASCII = new List<string> { "██╗  ██╗ ","╚██╗██╔╝ "," ╚███╔╝  "," ██╔██╗  ","██╔╝ ██╗ ","╚═╝  ╚═╝ " }},
                new Letter { Character = "Y",  ASCII = new List<string> { "██╗   ██╗ ","╚██╗ ██╔╝ "," ╚████╔╝  ","  ╚██╔╝   ","   ██║    ","   ╚═╝    " }},
                new Letter { Character = "Z",  ASCII = new List<string> { "███████╗ ","╚══███╔╝ ","  ███╔╝  "," ███╔╝   ","███████╗ ","╚══════╝ " }},
                new Letter { Character = "1",  ASCII = new List<string> { " ██╗ ","███║ ","╚██║ "," ██║ "," ██║ "," ╚═╝ " }},
                new Letter { Character = "2",  ASCII = new List<string> { "██████╗  ","╚════██╗ "," █████╔╝ ","██╔═══╝  ","███████╗ ","╚══════╝ " }},
                new Letter { Character = "3",  ASCII = new List<string> { "██████╗  ","╚════██╗ "," █████╔╝ "," ╚═══██╗ ","██████╔╝ ","╚═════╝  " }},
                new Letter { Character = "4",  ASCII = new List<string> { "██╗  ██╗ ","██║  ██║ ","███████║ ","╚════██║ ","     ██║ ","     ╚═╝ " }},
                new Letter { Character = "5",  ASCII = new List<string> { "███████╗ ","██╔════╝ ","███████╗ ","╚════██║ ","███████║ ","╚══════╝ " }},
                new Letter { Character = "6",  ASCII = new List<string> { " ██████╗  ","██╔════╝  ","███████╗  ","██╔═══██╗ ","╚██████╔╝ "," ╚═════╝  " }},
                new Letter { Character = "7",  ASCII = new List<string> { "███████╗ ","╚════██║ ","    ██╔╝ ","   ██╔╝  ","   ██║   ","   ╚═╝   " }},
                new Letter { Character = "8",  ASCII = new List<string> { " █████╗  ","██╔══██╗ ","╚█████╔╝ ","██╔══██╗ ","╚█████╔╝ "," ╚════╝  " }},
                new Letter { Character = "9",  ASCII = new List<string> { " █████╗  ","██╔══██╗ ","╚██████║ "," ╚═══██║ "," █████╔╝ "," ╚════╝  " }},
                new Letter { Character = "0",  ASCII = new List<string> { " ██████╗  ","██╔═████╗ ","██║██╔██║ ","████╔╝██║ ","╚██████╔╝ "," ╚═════╝  " }},
                new Letter { Character = "!",  ASCII = new List<string> { "██╗ ","██║ ","██║ ","╚═╝ ","██╗ ","╚═╝ " }},
                new Letter { Character = "@",  ASCII = new List<string> { " ██████╗  ","██╔═══██╗ ","██║██╗██║ ","██║██║██║ ","╚█║████╔╝ "," ╚╝╚═══╝  " }},
                new Letter { Character = "$",  ASCII = new List<string> { "▄▄███▄▄· ","██╔════╝ ","███████╗ ","╚════██║ ","███████║ ","╚═▀▀▀══╝ " }},
                new Letter { Character = "%",  ASCII = new List<string> { "██╗ ██╗ ","╚═╝██╔╝ ","  ██╔╝  "," ██╔╝   ","██╔╝██╗ ","╚═╝ ╚═╝ " }},
                new Letter { Character = "&",  ASCII = new List<string> { "   ██╗    ","   ██║    ","████████╗ ","██╔═██╔═╝ ","██████║   ","╚═════╝   " }},
                new Letter { Character = "*",  ASCII = new List<string> { "       ","▄ ██╗▄ "," ████╗ ","▀╚██╔▀ ","  ╚═╝  ","       " }},
                new Letter { Character = "(",  ASCII = new List<string> { " ██╗ ","██╔╝ ","██║  ","██║  ","╚██╗ "," ╚═╝ " }},
                new Letter { Character = ")",  ASCII = new List<string> { "██╗  ","╚██╗ "," ██║ "," ██║ ","██╔╝ ","╚═╝  " }},
                new Letter { Character = "-",  ASCII = new List<string> { "       ","       ","█████╗ ","╚════╝ ","       ","       " }},
                new Letter { Character = "_",  ASCII = new List<string> { "         ","         ","         ","         ","███████╗ ","╚══════╝ " }},
                new Letter { Character = "[",  ASCII = new List<string> { "███╗ ","██╔╝ ","██║  ","██║  ","███╗ ","╚══╝ " }},
                new Letter { Character = "]",  ASCII = new List<string> { "███╗ ","╚██║ "," ██║ "," ██║ ","███║ ","╚══╝ " }},
                new Letter { Character = ";",  ASCII = new List<string> { "    ","██╗ ","╚═╝ ","▄█╗ ","▀═╝ ","    " }},
                new Letter { Character = ":",  ASCII = new List<string> { "    ","██╗ ","╚═╝ ","██╗ ","╚═╝ ","    " }},
                new Letter { Character = ",",  ASCII = new List<string> { "    ","    ","    ","    ","▄█╗ ","╚═╝ " }},
                new Letter { Character = ".",  ASCII = new List<string> { "    ","    ","    ","    ","██╗ ","╚═╝ " }},
                new Letter { Character = "/",  ASCII = new List<string> { "    ██╗ ","   ██╔╝ ","  ██╔╝  "," ██╔╝   ","██╔╝    ","╚═╝     " }},
                new Letter { Character = "?",  ASCII = new List<string> { "██████╗  ","╚════██╗ ","  ▄███╔╝ ","  ▀▀══╝  ","  ██╗    ","  ╚═╝    " }},
            };
        }
    }

    public static List<Letter> Letters_ASCII9 {
        get {
            return new List<Letter>() {
                new Letter { Character = " ",  ASCII = new List<string> { "    ", "    ", "    ", "    ", "    ", "    ", "    " }},
                new Letter { Character = "A",  ASCII = new List<string> { "  mm   ","  ##   "," #  #  "," #mm#  ","#    # ","       ","       ", }},
                new Letter { Character = "B",  ASCII = new List<string> { "mmmmm  ","#    # ","#mmmm  ","#    # ","#mmmm  ","       ","       ", }},
                new Letter { Character = "C",  ASCII = new List<string> { "  mmm  ","m      ","#      ","#      ","  mmm  ","       ","       ", }},
                new Letter { Character = "D",  ASCII = new List<string> { "mmmm   ","#    m ","#    # ","#    # ","#mmm   ","       ","       ", }},
                new Letter { Character = "E",  ASCII = new List<string> { "mmmmmm ","#      ","#mmmmm ","#      ","#mmmmm ","       ","       ", }},
                new Letter { Character = "F",  ASCII = new List<string> { "mmmmmm ","#      ","#mmmmm ","#      ","#      ","       ","       ", }},
                new Letter { Character = "G",  ASCII = new List<string> { "  mmm  ","m      ","#   mm ","#    # ","  mmm  ","       ","       ", }},
                new Letter { Character = "H",  ASCII = new List<string> { "m    m ","#    # ","#mmmm# ","#    # ","#    # ","       ","       ", }},
                new Letter { Character = "I",  ASCII = new List<string> { "mmmmm ", "  #   ", "  #   ", "  #   ", "mm#mm ", "      ", "      ",  }},
                new Letter { Character = "J",  ASCII = new List<string> { "  mmm ", "    # ", "    # ", "    # ", " mmm  ", "      ", "      ",  }},
                new Letter { Character = "K",  ASCII = new List<string> { "m    m ","#  m   ","#m#    ","#  #m  ","#    m ","       ","       ", }},
                new Letter { Character = "L",  ASCII = new List<string> { "m      ","#      ","#      ","#      ","#mmmmm ","       ","       ", }},
                new Letter { Character = "M",  ASCII = new List<string> { "m    m ","##  ## ","# ## # ","#    # ","#    # ","       ","       ", }},
                new Letter { Character = "N",  ASCII = new List<string> { "mm   m ","# m  # ","# #m # ","#  # # ","#   ## ","       ","       ", }},
                new Letter { Character = "O",  ASCII = new List<string> { " mmmm  ","m    m ","#    # ","#    # "," #mm#  ","       ","       ", }},
                new Letter { Character = "P",  ASCII = new List<string> { "mmmmm  ","#    # ","#mmm#  ","#      ","#      ","       ","       ", }},
                new Letter { Character = "Q",  ASCII = new List<string> { " mmmm  ","m    m ","#    # ","#    # "," #mm#  ","    #  ","       ", }},
                new Letter { Character = "R",  ASCII = new List<string> { "mmmmm  ","#    # ","#mmmm  ","#    m ","#      ","       ","       ", }},
                new Letter { Character = "S",  ASCII = new List<string> { " mmmm  ","#      "," #mmm  ","     # "," mmm#  ","       ","       ", }},
                new Letter { Character = "T",  ASCII = new List<string> { "mmmmmmm ","   #    ","   #    ","   #    ","   #    ","        ","        ", }},
                new Letter { Character = "U",  ASCII = new List<string> { "m    m ","#    # ","#    # ","#    # "," mmmm  ","       ","       ", }},
                new Letter { Character = "V",  ASCII = new List<string> { "m    m "," m  m  "," #  #  ","  mm   ","  ##   ","       ","       ", }},
                new Letter { Character = "W",  ASCII = new List<string> { "m     m ","#  #  # ","  # # # "," ## ##  "," #   #  ","        ","        ", }},
                new Letter { Character = "X",  ASCII = new List<string> { "m    m "," #  #  ","  ##   "," m  m  ","m    m ","       ","       ", }},
                new Letter { Character = "Y",  ASCII = new List<string> { "m     m ","  m m   ","   #    ","   #    ","   #    ","        ","        ", }},
                new Letter { Character = "Z",  ASCII = new List<string> { "mmmmmm ","    #  ","  m#   "," m     ","##mmmm ","       ","       ", }},
                new Letter { Character = "1",  ASCII = new List<string> { " mmm   ","   #   ","   #   ","   #   "," mm#mm ","       ","       ", }},
                new Letter { Character = "2",  ASCII = new List<string> { " mmmm  ","     # ","    m  ","  m    ","m#mmmm ","       ","       ", }},
                new Letter { Character = "3",  ASCII = new List<string> { " mmmm  ","     # ","  mmm  ","     # "," mmm#  ","       ","       ", }},
                new Letter { Character = "4",  ASCII = new List<string> { "   mm  ","  m #  "," #  #  ","#mmm#m ","    #  ","       ","       ", }},
                new Letter { Character = "5",  ASCII = new List<string> { "mmmmm  ","#      ","    mm ","     # "," mmm#  ","       ","       ", }},
                new Letter { Character = "6",  ASCII = new List<string> { "  mmm  ","m      ","#m  #m ","#    # "," #mm#  ","       ","       ", }},
                new Letter { Character = "7",  ASCII = new List<string> { "mmmmmm ","    #  ","   m   ","  m    "," m     ","       ","       ", }},
                new Letter { Character = "8",  ASCII = new List<string> { " mmmm  ","#    # "," mmmm  ","#    # "," #mmm  ","       ","       ", }},
                new Letter { Character = "9",  ASCII = new List<string> { " mmmm  ","#    m ","#m  m# ","     # "," mmm   ","       ","       ", }},
                new Letter { Character = "0",  ASCII = new List<string> { " mmmm  ","m    m ","#  m # ","#    # "," #mm#  ","       ","       ", }},
                new Letter { Character = "`",  ASCII = new List<string> { " m ",    "   ",    "   ",    "   ",    "   ",    "   ",    "   ",     }},
                new Letter { Character = "~",  ASCII = new List<string> { "       ","       ","mmm  m ","       ","       ","       ","       ", }},
                new Letter { Character = "!",  ASCII = new List<string> { "m ",     "# ",     "# ",     "  ",     "# ",     "  ",     "  ",      }},
                new Letter { Character = "@",  ASCII = new List<string> { "       "," m  #m ","#  mm# ","# #  # ","#m     ","  mmm  ","       ", }},
                new Letter { Character = "$",  ASCII = new List<string> { "  m   ", "m # m ", "#m#   ", "  # # ", " m#m  ", "  #   ", "      ",  }},
                new Letter { Character = "%",  ASCII = new List<string> { " mm     ","#  #    ","   m    ","   m  m ","    mm  ","        ","        ", }},
                new Letter { Character = "&",  ASCII = new List<string> { "  mmm  "," #     "," ##    ","#  #m# "," #mm#m ","       ","       ", }},
                new Letter { Character = "*",  ASCII = new List<string> { "  m   ", " m#m  ", "m # m ", "      ", "      ", "      ", "      ",  }},
                new Letter { Character = "(",  ASCII = new List<string> { " #  ",   "m   ",   "#   ",   "#   ",   " #  ",   "    ",   "    ",    }},
                new Letter { Character = ")",  ASCII = new List<string> { " m  ",   "  m ",   "  # ",   "  # ",   " #  ",   "    ",   "    ",    }},
                new Letter { Character = "-",  ASCII = new List<string> { "    ",   "    ",   "    ",   "    ",   "    ",   "    ",   "    ",    }},
                new Letter { Character = "_",  ASCII = new List<string> { "       ","       ","       ","       ","       ","       ","       ", }},
                new Letter { Character = "=",  ASCII = new List<string> { "       ","       ","mmmmmm ","mmmmmm ","       ","       ","       ", }},
                new Letter { Character = "+",  ASCII = new List<string> { "        ","   m    ","   #    ","   #    ","   #    ","        ","        ", }},
                new Letter { Character = "\\",  ASCII = new List<string> { "m        "," #       ","  #      ","   #     ","    #    ","         ","         ", }},
                new Letter { Character = "|",  ASCII = new List<string> { "# ",      "# ",      "# ",      "# ",      "# ",      "# ",      "  ",       }},
                new Letter { Character = "[",  ASCII = new List<string> { "#  ",     "#  ",     "#  ",     "#  ",     "#  ",     "   ",     "   ",      }},
                new Letter { Character = "{",  ASCII = new List<string> { "  m   ",  "  #   ",  "mm    ",  "  #   ",  "  #   ",  "      ",  "      ",   }},
                new Letter { Character = "]",  ASCII = new List<string> { " #  ",    " #  ",    " #  ",    " #  ",    " #  ",    "    ",    "    ",     }},
                new Letter { Character = "}",  ASCII = new List<string> { "  m   ",  "  #   ",  "   mm ",  "  #   ",  "  #   ",  "      ",  "      ",   }},
                new Letter { Character = ";",  ASCII = new List<string> { "   ",     "   ",     " # ",     "   ",     " # ",     "   ",     "   ",      }},
                new Letter { Character = ":",  ASCII = new List<string> { "  ",      "  ",      "# ",      "  ",      "# ",      "  ",      "  ",       }},
                new Letter { Character = "'",  ASCII = new List<string> { "m ",      "# ",      "  ",      "  ",      "  ",      "  ",      "  ",       }},
                new Letter { Character = "\"",  ASCII = new List<string> { "m m ",    "# # ",    "    ",    "    ",    "    ",    "    ",    "    ",     }},
                new Letter { Character = ",",  ASCII = new List<string> { "   ",     "   ",     "   ",     "   ",     " # ",     "   ",     "   ",      }},
                new Letter { Character = ".",  ASCII = new List<string> { "  ",      "  ",      "  ",      "  ",      "# ",      "  ",      "  ",       }},
                new Letter { Character = "/",  ASCII = new List<string> { "     m ", "    #  ", "   #   ", "  #    ", " #     ", "       ", "       ",  }},
                new Letter { Character = "?",  ASCII = new List<string> { "  mmm  ", "     # ", "  m#   ", "       ", "  #    ", "       ", "       ",  }},
            };
        }
    }

    public static List<Letter> Letters_Classy {
        get {
            return new List<Letter>() {
                new Letter { Character = " ",  ASCII = new List<string> { "     ", "     ", "     ", "     ", "     ", "     ", "     " }},
                new Letter { Character = "A",  ASCII = new List<string> { "     ▄▄     ","   ▄█▀▀█▄   ","   ██  ██   ","   ██▀▀██   "," ▄ ██  ██   "," ▀██▀  ▀█▄█ ","            ","            " }},
                new Letter { Character = "B",  ASCII = new List<string> { "   ▄▄▄    ","  ██▀▀█▄  ","  ██ ▄█▀  ","  ██▀▀█▄  ","▄ ██  ▄█  ","▀██████▀  ","          ","          " }},
                new Letter { Character = "C",  ASCII = new List<string> { "▄   ▄▄▄▄  ","▀██████▀  ","  ██      ","  ██      ","  ██      ","  ▀█████  ","          ","          " }},
                new Letter { Character = "D",  ASCII = new List<string> { " ▄▄▄▄▄▄    ","█▀██▀▀██   ","  ██   ██  ","  ██   ██  ","▄ ██   ██  ","▀██▀███▀   ","           ","           " }},
                new Letter { Character = "E",  ASCII = new List<string> { " ▄▄▄▄▄▄▄  ","█▀██▀▀▀   ","  ██      ","  ████    ","  ██      ","  ▀█████  ","          ","          " }},
                new Letter { Character = "F",  ASCII = new List<string> { " ▄▄▄▄▄▄▄ ","█▀██▀▀▀  ","  ██     ","  ███▀   ","▄ ██     ","▀██▀     ","         ","         " }},
                new Letter { Character = "G",  ASCII = new List<string> { "▄   ▄▄▄▄ ","▀██████▀ ","  ██   ▄ ","  ██  ██ ","  ██  ██ ","  ▀█████ ","  ▄   ██ ","  ▀████▀ " }},
                new Letter { Character = "H",  ASCII = new List<string> { " ▄▄▄  ▄▄▄  ","█▀██  ██   ","  ██  ██   ","  ██████   ","  ██  ██   ","▀██▀  ▀██▄ ","           ","           " }},
                new Letter { Character = "I",  ASCII = new List<string> { " ▄▄▄▄▄▄ ","█▀ ██   ","   ██   ","   ██   ","   ██   "," ▄▄██▄▄ ","        ","        " }},
                new Letter { Character = "J",  ASCII = new List<string> { "  ▄▄▄▄▄▄ "," █▀ ██   ","    ██   ","    ██   ","    ██   ","    ██   ","▄   ██   ","▀████▀   " }},
                new Letter { Character = "K",  ASCII = new List<string> { " ▄▄▄▄   ▄▄▄ ","█▀ ██  ██   ","   ██ ██    ","   █████    ","   ██ ██▄   "," ▀██▀  ▀██▄ ","            ","            " }},
                new Letter { Character = "L",  ASCII = new List<string> { " ▄▄▄     ","▀██▀     "," ██      "," ██      "," ██      ","████████ ","         ","         " }},
                new Letter { Character = "M",  ASCII = new List<string> { " ▄▄▄     ▄▄▄  ","  ███▄ ▄███   ","  ██ ▀█▀ ██   ","  ██     ██   ","  ██     ██   ","▀██▀     ▀██▄ ","              ","              " }},
                new Letter { Character = "N",  ASCII = new List<string> { "   ▄▄     ▄▄ ","   ██▄   ██▀ ","   ███▄  ██  ","   ██ ▀█▄██  ","   ██   ▀██  "," ▀██▀    ██  ","             ","             " }},
                new Letter { Character = "O",  ASCII = new List<string> { "   ▄▄▄▄   "," ▄█▀▀████ "," ██    ██ "," ██    ██ "," ██    ██ ","  ▀████▀  ","          ","          " }},
                new Letter { Character = "P",  ASCII = new List<string> { "  ▄▄▄▄▄▄  "," █▀██▀▀▀█ ","   ██▄▄▄█ ","   ██▀▀▀  "," ▄ ██     "," ▀██▀     ","          ","          " }},
                new Letter { Character = "Q",  ASCII = new List<string> { "   ▄▄▄▄   "," ▄█▀▀███▄ "," ██    ██ "," ██    ██ "," ██  ▄ ██ ","  ▀█████▄ ","       ▀█ ","          " }},
                new Letter { Character = "R",  ASCII = new List<string> { "  ▄▄▄▄▄▄   "," █▀██▀▀▀█▄ ","   ██▄▄▄█▀ ","   ██▀▀█▄  "," ▄ ██  ██  "," ▀██▀  ▀██ ","           ","           " }},
                new Letter { Character = "S",  ASCII = new List<string> { "  ▄▄▄▄▄  "," ██▀▀▀▀█ "," ▀██▄  ▄ ","   ▀██▄▄ "," ▄   ▀██ "," ▀██████ ","         ","         " }},
                new Letter { Character = "T",  ASCII = new List<string> { "  ▄▄▄▄▄▄▄ "," █▀▀██▀▀▀ ","    ██    ","    ██    ","    ██    ","    ▀██▄  ","          ","          " }},
                new Letter { Character = "U",  ASCII = new List<string> { "  ▄▄▄  ▄▄ "," █▀██  ██ ","   ██  ██ ","   ██  ██ ","   ██  ██ ","   ▀█████ ","          ","          " }},
                new Letter { Character = "V",  ASCII = new List<string> { "  ▄▄▄      "," █▀██  ██▀ ","   ██  ██  ","   ██  ██  ","   ██▄ ██  ","    ▀███▀  ","           ","           " }},
                new Letter { Character = "W",  ASCII = new List<string> { "  ▄▄▄          "," █▀██  ██  ██▀ ","   ██  ██  ██  ","   ██  ██  ██  ","   ██▄ ██▄ ██  ","   ▀████▀███▀  ","               ","               " }},
                new Letter { Character = "X",  ASCII = new List<string> { "  ▄▄▄   ▄▄ "," █▀▀██ ██▀ ","    ▀█▄█▀  ","     ███   ","   ▄█▀██▄  "," ▀██▀  ▀██ ","           ","           " }},
                new Letter { Character = "Y",  ASCII = new List<string> { "  ▄▄▄     "," █▀██  ██ ","   ██  ██ ","   ██  ██ ","   ██  ██ ","   ▀█████ ","   ▄   ██ ","   ▀████▀ " }},
                new Letter { Character = "Z",  ASCII = new List<string> { "  ▄▄▄▄▄▄▄ "," █▀▀▀▀▀██ ","      ▄█▀ ","    ▄█▀   ","  ▄█▀     "," ████████ ","          ","          " }},
                new Letter { Character = "1",  ASCII = new List<string> { "    ▄ ","  ▄██ ","▄█▀██ ","▀  ██ ","   ██ ","   ██ ","      ","      " }},
                new Letter { Character = "2",  ASCII = new List<string> { "  ▄▄▄▄  ","▄██████ ","▀█▄  ██ ","    ▄█▀ ","  ▄█▀   ","██████▄ ","        ","        " }},
                new Letter { Character = "3",  ASCII = new List<string> { " ▄▄▄▄▄  ","██▀▀▀██ ","▀   ▄█▀ ","  ▀▀▀█▄ ","▄    ██ ","▀█████▀ ","        ","        " }},
                new Letter { Character = "4",  ASCII = new List<string> { "   ▄▄   ","   ██   ","  ▄██   ","▄██▀ █  ","███████ ","    ██  ","        ","        " }},
                new Letter { Character = "5",  ASCII = new List<string> { "▄▄▄▄▄▄▄ ","██▀▀▀▀▀ ","██▄▄▄   ","▀▀▀▀██▄ ","▄   ▄██ ","▀████▀  ","        ","        " }},
                new Letter { Character = "6",  ASCII = new List<string> { "  ▄▄▄▄  ","▄██▀▀▀█ ","██      ","██▄███▄ ","██▀  ██ ","▀█████▀ ","        ","        " }},
                new Letter { Character = "7",  ASCII = new List<string> { "▄▄▄▄▄▄▄ ","▀▀▀▀▀██ ","    ██  "," ▄▄██▄▄ ","   ██   ","   ██   ","        ","        " }},
                new Letter { Character = "8",  ASCII = new List<string> { " ▄▄▄▄▄  ","██▀▀▀██ ","▀█▄▄▄█▀ ","▄█████▄ ","██   ██ ","▀█████▀ ","        ","        " }},
                new Letter { Character = "9",  ASCII = new List<string> { " ▄▄▄▄▄  ","██▀▀▀██ ","██ ▄▄██ "," ▀▀▀▀██ ","▄   ▄██ ","▀████▀  ","        ","        " }},
                new Letter { Character = "0",  ASCII = new List<string> { "  ▄▄▄   ","▄██▀▀▀  ","██ ▄▀█▄ ","██   ██ ","██  ▄██ "," ▀███▀  ","        ","        " }},
                new Letter { Character = "`",  ASCII = new List<string> { "   ","   ","   ","▀▄ ","   ","   ","   ","   " }},
                new Letter { Character = "~",  ASCII = new List<string> { "        ","        ","        ","        ","▄▀▀▄  ▄ ","▀  ▀▄▄▀ ","        ","        " }},
                new Letter { Character = "!",  ASCII = new List<string> { "   ","▄▄ ","██ ","██ ","██ ","   ","██ ","   " }},
                new Letter { Character = "@",  ASCII = new List<string> { "              ","              ","  ▄███████▄   "," ██   ▀█▄ ▀█  ","██  ▄█▀██  ██ ","██  ██ ██ ▄█  ","▀█▄  ▀▀▀▀▀▀   ","  ▀██████▀▀   " }},
                new Letter { Character = "$",  ASCII = new List<string> { "   ▄ ▄   "," ▄▄█▄█   ","██▀█▀██▄ ","▀███ █ ▀ ","  ▀███▄  ","▄  █ ██▄ ","▀██████▀ ","   █ █   " }},
                new Letter { Character = "%",  ASCII = new List<string> { "          ","          ","▄▀▀▄  █   ","▀▄▄▀ █    ","    █     ","   █ ▄▀▀▄ ","  █  ▀▄▄▀ ","          " }},
                new Letter { Character = "&",  ASCII = new List<string> { "           ","  ▄▄▄▄     "," ██  ██    "," ▀██▄█▀    "," ▄████▄▄█▀ ","██▀  ██▄   ","▀█▄▄██▀▀█▄ ","           " }},
                new Letter { Character = "*",  ASCII = new List<string> { "      ","      ","▄ ▄ ▄ ","▄███▄ ","▄▀█▀▄ ","      ","      ","      " }},
                new Letter { Character = "(",  ASCII = new List<string> { "    "," ▄█ ","██  ","██  ","██  ","██  ","██  "," ▀█ " }},
                new Letter { Character = ")",  ASCII = new List<string> { "    ","█▄  "," ██ "," ██ "," ██ "," ██ "," ██ ","█▀  " }},
                new Letter { Character = "-",  ASCII = new List<string> { "     ","     ","     ","     ","     ","▀▀▀▀ ","     ","     " }},
                new Letter { Character = "_",  ASCII = new List<string> { "        ","        ","        ","        ","        ","        ","        ","▄▄▄▄▄▄▄ " }},
                new Letter { Character = "=",  ASCII = new List<string> { "       ","       ","       ","       ","▀▀▀▀▀▀ ","▄▄▄▄▄▄ ","       ","       " }},
                new Letter { Character = "+",  ASCII = new List<string> { "        ","        ","        ","   ▄    ","   █    ","▀▀▀█▀▀▀ ","   █    ","        " }},
                new Letter { Character = "\\",  ASCII = new List<string> { "█      "," █     ","  █    ","   █   ","    █  ","     █ ","       ","       " }},
                new Letter { Character = "|",  ASCII = new List<string> { "█ ","█ ","█ ","█ ","█ ","█ ","█ ","  " }},
                new Letter { Character = "[",  ASCII = new List<string> { "█▀▀ ","█   ","█   ","█   ","█   ","█   ","█▄▄ ","    " }},
                new Letter { Character = "{",  ASCII = new List<string> { " ▄█ "," █  "," █  ","██  "," █  "," █  "," ▀█ ","    " }},
                new Letter { Character = "]",  ASCII = new List<string> { "▀▀█ ","  █ ","  █ ","  █ ","  █ ","  █ ","▄▄█ ","    " }},
                new Letter { Character = "}",  ASCII = new List<string> { "█▄  "," █  "," █  "," ██ "," █  "," █  ","█▀  ","    " }},
                new Letter { Character = ";",  ASCII = new List<string> { "   ","   ","   "," ▀ ","   "," ▄ ","▄▀ ","   " }},
                new Letter { Character = ":",  ASCII = new List<string> { "  ","  ","  ","▀ ","  ","▄ ","  ","  " }},
                new Letter { Character = "'",  ASCII = new List<string> { "  ","▄ ","▀ ","  ","  ","  ","  ","  " }},
                new Letter { Character = "\"",  ASCII = new List<string> { "▄ ▄ ","▀ ▀ ","    ","    ","    ","    ","    ","    " }},
                new Letter { Character = ",",  ASCII = new List<string> { "   ","   ","   ","   ","   "," ▄ ","▄█ ","   " }},
                new Letter { Character = ".",  ASCII = new List<string> { "   ","   ","   ","   ","   ","██ ","   ","   " }},
                new Letter { Character = "/",  ASCII = new List<string> { "     █ ","    █  ","   █   ","  █    "," █     ","█      ","       ","       " }},
                new Letter { Character = "?",  ASCII = new List<string> { " ▄▄▄▄▄  ","██▀▀▀██ ","   ▄██▀ ","  ██    ","        ","  ██    ","        ","        " }},
            };
        }
    }

    public static List<Letter> Letters_CoderMini {
        get {
            return new List<Letter>() {
                new Letter { Character = " ",  ASCII = new List<string> { "     ", "     ", "     ", "     ", "     ", "     ", "     " }},
                new Letter { Character = "A",  ASCII = new List<string> { "  ▄▄▄▄   ","▄██▀▀██▄ ","███  ███ ","███▀▀███ ","███  ███ ","         " }},
                new Letter { Character = "B",  ASCII = new List<string> { "▄▄▄▄▄▄▄   ","███▀▀███▄ ","███▄▄███▀ ","███  ███▄ ","████████▀ ","          " }},
                new Letter { Character = "C",  ASCII = new List<string> { " ▄▄▄▄▄▄▄ ","███▀▀▀▀▀ ","███      ","███      ","▀███████ ","         " }},
                new Letter { Character = "D",  ASCII = new List<string> { "▄▄▄▄▄▄   ","███▀▀██▄ ","███  ███ ","███  ███ ","██████▀  ","         " }},
                new Letter { Character = "E",  ASCII = new List<string> { " ▄▄▄▄▄▄▄ ","███▀▀▀▀▀ ","███▄▄    ","███      ","▀███████ ","         " }},
                new Letter { Character = "F",  ASCII = new List<string> { " ▄▄▄▄▄▄▄ ","███▀▀▀▀▀ ","███▄▄    ","███▀▀    ","███      ","         " }},
                new Letter { Character = "G",  ASCII = new List<string> { " ▄▄▄▄▄▄▄  ","███▀▀▀▀▀  ","███       ","███  ███▀ ","▀██████▀  ","          " }},
                new Letter { Character = "H",  ASCII = new List<string> { "▄▄▄   ▄▄▄ ","███   ███ ","█████████ ","███▀▀▀███ ","███   ███ ","          " }},
                new Letter { Character = "I",  ASCII = new List<string> { "▄▄▄▄▄ "," ███  "," ███  "," ███  ","▄███▄ ","      " }},
                new Letter { Character = "J",  ASCII = new List<string> { "     ▄▄▄ ","     ███ ","     ███ ","▄▄▄  ███ "," ▀████▀  ","         " }},
                new Letter { Character = "K",  ASCII = new List<string> { "▄▄▄   ▄▄▄ ","███ ▄███▀ ","███████   ","███▀███▄  ","███  ▀███ ","          " }},
                new Letter { Character = "L",  ASCII = new List<string> { "▄▄▄      ","███      ","███      ","███      ","████████ ","         " }},
                new Letter { Character = "M",  ASCII = new List<string> { "▄▄▄      ▄▄▄ ","████▄  ▄████ ","███▀████▀███ ","███  ▀▀  ███ ","███      ███ ","             " }},
                new Letter { Character = "N",  ASCII = new List<string> { "▄▄▄    ▄▄▄ ","████▄  ███ ","███▀██▄███ ","███  ▀████ ","███    ███ ","           " }},
                new Letter { Character = "O",  ASCII = new List<string> { "  ▄▄▄▄▄   ","▄███████▄ ","███   ███ ","███▄▄▄███ "," ▀█████▀  ","          " }},
                new Letter { Character = "P",  ASCII = new List<string> { "▄▄▄▄▄▄▄   ","███▀▀███▄ ","███▄▄███▀ ","███▀▀▀▀   ","███       ","          " }},
                new Letter { Character = "Q",  ASCII = new List<string> { "  ▄▄▄▄▄   ","▄███████▄ ","███   ███ ","███▄█▄███ "," ▀█████▀  ","      ▀▀  " }},
                new Letter { Character = "R",  ASCII = new List<string> { "▄▄▄▄▄▄▄   ","███▀▀███▄ ","███▄▄███▀ ","███▀▀██▄  ","███  ▀███ ","          " }},
                new Letter { Character = "S",  ASCII = new List<string> { " ▄▄▄▄▄▄▄ ","█████▀▀▀ "," ▀████▄  ","   ▀████ ","███████▀ ","         " }},
                new Letter { Character = "T",  ASCII = new List<string> { "▄▄▄▄▄▄▄▄▄ ","▀▀▀███▀▀▀ ","   ███    ","   ███    ","   ███    ","          " }},
                new Letter { Character = "U",  ASCII = new List<string> { "▄▄▄  ▄▄▄ ","███  ███ ","███  ███ ","███▄▄███ ","▀██████▀ ","         " }},
                new Letter { Character = "V",  ASCII = new List<string> { "▄▄▄▄  ▄▄▄▄ ","▀███  ███▀ "," ███  ███  "," ███▄▄███  ","  ▀████▀   ","           " }},
                new Letter { Character = "W",  ASCII = new List<string> { "▄▄▄▄  ▄▄▄  ▄▄▄▄ ","▀███  ███  ███▀ "," ███  ███  ███  "," ███▄▄███▄▄███  ","  ▀████▀████▀   ","                " }},
                new Letter { Character = "X",  ASCII = new List<string> { "▄▄▄   ▄▄▄ ","████▄████ "," ▀█████▀  ","▄███████▄ ","███▀ ▀███ ","          " }},
                new Letter { Character = "Y",  ASCII = new List<string> { "▄▄▄   ▄▄▄ ","███   ███ ","▀███▄███▀ ","  ▀███▀   ","   ███    ","          " }},
                new Letter { Character = "Z",  ASCII = new List<string> { "▄▄▄▄▄▄▄▄▄ ","▀▀▀▀▀████ ","   ▄███▀  "," ▄███▀    ","█████████ ","          " }},
                new Letter { Character = "1",  ASCII = new List<string> { "  ▄▄▄▄ ","▄█████ ","   ███ ","   ███ ","   ███ ","       " }},
                new Letter { Character = "2",  ASCII = new List<string> { "▄▄▄▄▄▄▄  ","▀▀▀▀████ ","   ▄██▀  "," ▄███▄▄▄ ","████████ ","         " }},
                new Letter { Character = "3",  ASCII = new List<string> { "▄▄▄▄▄▄▄  ","▀▀▀▀████ ","  ▄▄██▀  ","    ███▄ ","███████▀ ","         " }},
                new Letter { Character = "4",  ASCII = new List<string> { "▄▄▄  ▄▄▄ ","███  ███ ","███▄▄███ "," ▀▀▀████ ","    ████ ","         " }},
                new Letter { Character = "5",  ASCII = new List<string> { "▄▄▄▄▄▄▄▄ ","███▀▀▀▀▀ ","██████▄  ","    ████ ","██████▀  ","         " }},
                new Letter { Character = "6",  ASCII = new List<string> { "  ▄▄▄▄▄▄ ","▄███▀▀▀▀ ","███████▄ ","███  ███ ","▀██████▀ ","         " }},
                new Letter { Character = "7",  ASCII = new List<string> { "▄▄▄▄▄▄▄▄ ","████████ ","    ▄██▀ ","   ███   ","   ███   ","         " }},
                new Letter { Character = "8",  ASCII = new List<string> { " ▄▄▄▄▄▄  ","███▀▀███ "," ██▄▄██  ","███  ███ ","▀██████▀ ","         " }},
                new Letter { Character = "9",  ASCII = new List<string> { " ▄▄▄▄▄▄  ","███▀▀███ ","███▄▄███ "," ▀▀▀████ ","██████▀  ","         " }},
                new Letter { Character = "0",  ASCII = new List<string> { "  ▄▄▄▄   ","▄██████▄ ","███  ███ ","███▄▄███ "," ▀████▀  ","         " }},
                new Letter { Character = "`",  ASCII = new List<string> { "   ","▀▄ ","   ","   ","   ","   " }},
                new Letter { Character = "~",  ASCII = new List<string> { "           ","           ","▄████▄  ▄▄ ","▀▀  ▀████▀ ","           ","           " }},
                new Letter { Character = "!",  ASCII = new List<string> { "▄▄ ","██ ","██ ","▀▀ ","██ ","   " }},
                new Letter { Character = "@",  ASCII = new List<string> { "  ▄███████▄  "," ██     ▀█▄  ","██  ▄█▀▀▀██  ","██  ██   ██  "," ██▄ ▀▀▀▀▀▀  ","  ▀▀██████▀▀ " }},
                new Letter { Character = "$",  ASCII = new List<string> { " ▄▄██▄▄ ","████▀▀▀ ","▀████▄  ","  ▀████ ","██████▀ ","   ▀▀   " }},
                new Letter { Character = "%",  ASCII = new List<string> { " ▄▄       ","█  █ ██   "," ▀▀ ██    ","   ██ ▄▄  ","  ██ █  █ ","      ▀▀  " }},
                new Letter { Character = "&",  ASCII = new List<string> { "  ▄▄▄▄     "," ██▀▀▀█    "," ▄███▄▄▄█▀ ","██  ▀███   "," ▀████ ▀█▄ ","           " }},
                new Letter { Character = "*",  ASCII = new List<string> { "▄ ▄ ▄ ","▄███▄ ","▄▀█▀▄ ","      ","      ","      " }},
                new Letter { Character = "(",  ASCII = new List<string> { " ▄█ ","██  ","██  ","██  ","██  "," ▀█ " }},
                new Letter { Character = ")",  ASCII = new List<string> { "█▄  "," ██ "," ██ "," ██ "," ██ ","█▀  " }},
                new Letter { Character = "-",  ASCII = new List<string> { "      ","      ","      ","▀▀▀▀▀ ","      ","      " }},
                new Letter { Character = "_",  ASCII = new List<string> { "         ","         ","         ","         ","         ","▄▄▄▄▄▄▄▄ " }},
                new Letter { Character = "=",  ASCII = new List<string> { "        ","███████ ","        ","███████ ","        ","        " }},
                new Letter { Character = "+",  ASCII = new List<string> { "        ","   ▄    ","   █    ","▀▀▀█▀▀▀ ","   █    ","        " }},
                new Letter { Character = "\\",  ASCII = new List<string> { " ██     ","  ██    ","   ██   ","    ██  ","     ██ ","        " }},
                new Letter { Character = "|",  ASCII = new List<string> { "██ ","██ ","██ ","██ ","██ ","██ " }},
                new Letter { Character = "[",  ASCII = new List<string> { "██▀▀ ","██   ","██   ","██   ","██   ","██▄▄ " }},
                new Letter { Character = "{",  ASCII = new List<string> { " ▄██ "," ██  ","▄██  ","▀██  "," ██  "," ▀██ " }},
                new Letter { Character = "]",  ASCII = new List<string> { "▀▀██ ","  ██ ","  ██ ","  ██ ","  ██ ","▄▄██ " }},
                new Letter { Character = "}",  ASCII = new List<string> { "▀█▄  "," ██  "," ██▄ "," ██▀ "," ██  ","▄█▀  " }},
                new Letter { Character = ";",  ASCII = new List<string> { "    ","    "," ██ ","    "," ▄▄ ","▄█▀ " }},
                new Letter { Character = ":",  ASCII = new List<string> { "   ","   ","██ ","   ","██ ","   " }},
                new Letter { Character = "'",  ASCII = new List<string> { "▄ ","▀ ","  ","  ","  ","  " }},
                new Letter { Character = "\"",  ASCII = new List<string> { "▄ ▄ ","▀ ▀ ","    ","    ","    ","    " }},
                new Letter { Character = ",",  ASCII = new List<string> { "    ","    ","    ","    "," ▄▄ ","▄█▀ " }},
                new Letter { Character = ".",  ASCII = new List<string> { "   ","   ","   ","   ","██ ","   " }},
                new Letter { Character = "/",  ASCII = new List<string> { "    ██ ","   ██  ","  ██   "," ██    ","██     ","       " }},
                new Letter { Character = "?",  ASCII = new List<string> { " ▄▄▄▄  ","██▀▀██ ","  ▄██▀ ","  ██   ","  ▄▄   ","       " }},
            };
        }
    }

    public static List<Letter> Letters_CyberMedium {
        get {
            return new List<Letter>() {
                new Letter { Character = " ",  ASCII = new List<string> { "     ", "     ", "     ", "     ", "     ", "     ", "     " }},
                new Letter { Character = "A",  ASCII = new List<string> { "____ ","|__| ","|  | ", }},
                new Letter { Character = "B",  ASCII = new List<string> { "___  ","|__] ","|__] ", }},
                new Letter { Character = "C",  ASCII = new List<string> { "____ ","|    ","|___ ", }},
                new Letter { Character = "D",  ASCII = new List<string> { "___  ","|  \\ ","|__/ ", }},
                new Letter { Character = "E",  ASCII = new List<string> { "____ ","|___ ","|___ ", }},
                new Letter { Character = "F",  ASCII = new List<string> { "____ ","|___ ","|    ", }},
                new Letter { Character = "G",  ASCII = new List<string> { "____ ","| __ ","|__] ", }},
                new Letter { Character = "H",  ASCII = new List<string> { "_  _ ","|__| ","|  | ", }},
                new Letter { Character = "I",  ASCII = new List<string> { "_ ",   "| ",   "| ",    }},
                new Letter { Character = "J",  ASCII = new List<string> { " _ ",  " | ",  "_| ",   }},
                new Letter { Character = "K",  ASCII = new List<string> { "_  _ ","|_/  ","| \\_ ", }},
                new Letter { Character = "L",  ASCII = new List<string> { "_    ","|    ","|___ ", }},
                new Letter { Character = "M",  ASCII = new List<string> { "_  _ ","|\\/| ","|  | ", }},
                new Letter { Character = "N",  ASCII = new List<string> { "_  _ ","|\\ | ","| \\| ", }},
                new Letter { Character = "O",  ASCII = new List<string> { "____ ","|  | ","|__| ", }},
                new Letter { Character = "P",  ASCII = new List<string> { "___  ","|__] ","|    ", }},
                new Letter { Character = "Q",  ASCII = new List<string> { "____ ","|  | ","|_\\| ", }},
                new Letter { Character = "R",  ASCII = new List<string> { "____ ","|__/ ","|  \\ ", }},
                new Letter { Character = "S",  ASCII = new List<string> { "____ ","[__  ","___] ", }},
                new Letter { Character = "T",  ASCII = new List<string> { "___ ", " |  ", " |  ",  }},
                new Letter { Character = "U",  ASCII = new List<string> { "_  _ ","|  | ","|__| ", }},
                new Letter { Character = "V",  ASCII = new List<string> { "_  _ ","|  | "," \\/  ", }},
                new Letter { Character = "W",  ASCII = new List<string> { "_ _ _ ","| | | ","|_|_| ", }},
                new Letter { Character = "X",  ASCII = new List<string> { "_  _ "," \\/  ","_/\\_ ", }},
                new Letter { Character = "Y",  ASCII = new List<string> { "_   _ "," \\_/  ","  |   ", }},
                new Letter { Character = "Z",  ASCII = new List<string> { "___  ","  /  "," /__ ", }},
                new Letter { Character = "`",  ASCII = new List<string> { ". ", "` ", "  ", "  ",  }},
                new Letter { Character = "!",  ASCII = new List<string> { "  / "," /  ",".   ","    ", }},
                new Letter { Character = "-",  ASCII = new List<string> { "   ","__ ","   ","   ", }},
                new Letter { Character = "_",  ASCII = new List<string> { "    ","    ","___ ","    ", }},
                new Letter { Character = "\\",  ASCII = new List<string> { "\\   "," \\  ","  \\ ","    ", }},
                new Letter { Character = "|",  ASCII = new List<string> { "| ", "| ", "| ", "| ",  }},
                new Letter { Character = ";",  ASCII = new List<string> { "  ", ". ", ", ", "  ",  }},
                new Letter { Character = ":",  ASCII = new List<string> { "  ", ". ", ". ", "  ",  }},
                new Letter { Character = "'",  ASCII = new List<string> { ". ", "' ", "  ", "  ",  }},
                new Letter { Character = "\"",  ASCII = new List<string> { ".. ","'' ","   ","   ", }},
                new Letter { Character = ",",  ASCII = new List<string> { "  ", "  ", ". ", "' ",  }},
                new Letter { Character = ".",  ASCII = new List<string> { "  ", "  ", ". ", "  ",  }},
                new Letter { Character = "/",  ASCII = new List<string> { "  / "," /  ","/   ","    ", }},
                new Letter { Character = "?",  ASCII = new List<string> { "__. "," _] "," .  ","    ", }},
            };
        }
    }

    // https://patorjk.com/software/taag/#p=display&f=Future+Smooth&t=ABCDEFGHIJKLMNOPQRSTUVWXYZ%0A1234567890%0A%60%7e!%40%24%25%26*()-_%3d%2b%0A%5c%7c%5b%7b%5d%7d%3b%3a%27%22%2c.%2f%3f
    public static List<Letter> Letters_FutureSmooth {
        get {
            return new List<Letter>() {
                new Letter { Character = " ",  ASCII = new List<string> { "  "  , "  "  , "  "   }},
                new Letter { Character = "A",  ASCII = new List<string> { "╭─╮" , "├─┤" , "╵ ╵"  }},
                new Letter { Character = "B",  ASCII = new List<string> { "╭╮ " , "├┴╮" , "╰─╯"  }},
                new Letter { Character = "C",  ASCII = new List<string> { "╭─╴" , "│  " , "╰─╴"  }},
                new Letter { Character = "D",  ASCII = new List<string> { "╶┬╮" , " ││" , "╶┴╯"  }},
                new Letter { Character = "E",  ASCII = new List<string> { "╭─╴" , "├╴ " , "╰─╴"  }},
                new Letter { Character = "F",  ASCII = new List<string> { "╭─╴" , "├╴ " , "╵  "  }},
                new Letter { Character = "G",  ASCII = new List<string> { "╭─╴" , "│╶╮" , "╰─╯"  }},
                new Letter { Character = "H",  ASCII = new List<string> { "╷ ╷" , "├─┤" , "╵ ╵"  }},
                new Letter { Character = "I",  ASCII = new List<string> { "╷"   , "│"   , "╵"    }},
                new Letter { Character = "J",  ASCII = new List<string> { " ╭╮" , "  │" , "╰─╯"  }},
                new Letter { Character = "K",  ASCII = new List<string> { "╷╭ " , "├┴╮" , "╵ ╵"  }},
                new Letter { Character = "L",  ASCII = new List<string> { "╷  " , "│  " , "╰─╴"  }},
                new Letter { Character = "M",  ASCII = new List<string> { "╭┬╮" , "│││" , "╵ ╵"  }},
                new Letter { Character = "N",  ASCII = new List<string> { "╭╮╷" , "│╰┤" , "╵ ╵"  }},
                new Letter { Character = "O",  ASCII = new List<string> { "╭─╮" , "│ │" , "╰─╯"  }},
                new Letter { Character = "P",  ASCII = new List<string> { "╭─╮" , "├─╯" , "╵  "  }},
                new Letter { Character = "Q",  ASCII = new List<string> { "╭─╮" , "│╮│" , "╰┴╯"  }},
                new Letter { Character = "R",  ASCII = new List<string> { "╭─╮" , "├┬╯" , "╵╰╴"  }},
                new Letter { Character = "S",  ASCII = new List<string> { "╭─╮" , "╰─╮" , "╰─╯"  }},
                new Letter { Character = "T",  ASCII = new List<string> { "╶┬╴" , " │ " , " ╵ "  }},
                new Letter { Character = "U",  ASCII = new List<string> { "╷ ╷" , "│ │" , "╰─╯"  }},
                new Letter { Character = "V",  ASCII = new List<string> { "╷ ╷" , "│╭╯" , "╰╯ "  }},
                new Letter { Character = "W",  ASCII = new List<string> { "╷ ╷" , "│╷│" , "╰┴╯"  }},
                new Letter { Character = "X",  ASCII = new List<string> { "╷ ╷" , "╭┼╯" , "╵ ╵"  }},
                new Letter { Character = "Y",  ASCII = new List<string> { "╷ ╷" , "╰┬╯" , " ╵ "  }},
                new Letter { Character = "Z",  ASCII = new List<string> { "╶─╮" , "╭─╯" , "╰─╴"  }},
                new Letter { Character = "1",  ASCII = new List<string> { "╶╮ " , " │ " , "╶┴╴"  }},
                new Letter { Character = "2",  ASCII = new List<string> { "╭─╮" , "╭─╯" , "╰─╴"  }},
                new Letter { Character = "3",  ASCII = new List<string> { "╭─╮" , "╶─┤" , "╰─╯"  }},
                new Letter { Character = "4",  ASCII = new List<string> { "╷ ╷" , "╰─┤" , "  ╵"  }},
                new Letter { Character = "5",  ASCII = new List<string> { "╭─╴" , "╰─╮" , "╰─╯"  }},
                new Letter { Character = "6",  ASCII = new List<string> { "╭─╮" , "├─╮" , "╰─╯"  }},
                new Letter { Character = "7",  ASCII = new List<string> { "╭─╮" , "  │" , "  ╵"  }},
                new Letter { Character = "8",  ASCII = new List<string> { "╭─╮" , "├─┤" , "╰─╯"  }},
                new Letter { Character = "9",  ASCII = new List<string> { "╭─╮" , "╰─┤" , "╰─╯"  }},
                new Letter { Character = "0",  ASCII = new List<string> { "╭─╮" , "│││" , "╰─╯"  }},
                new Letter { Character = "`",  ASCII = new List<string> { " ╮"  , "  "  , "  "   }},
                new Letter { Character = "~",  ASCII = new List<string> { "   " , "╭─╯" , "   "  }},
                new Letter { Character = "!",  ASCII = new List<string> { "╷"   , "╵"   , "╵"    }},
                new Letter { Character = "@",  ASCII = new List<string> { "╭─╮" , "│├╯" , "╰─╴"  }},
                new Letter { Character = "$",  ASCII = new List<string> { "╭┬╮" , "╰┼╮" , "╰┴╯"  }},
                new Letter { Character = "%",  ASCII = new List<string> { "╭╮╷" , "╭─╯" , "╵╰╯"  }},
                new Letter { Character = "&",  ASCII = new List<string> { "╭╮  ", "│╶┼╴", "╰─╯ " }},
                new Letter { Character = "*",  ASCII = new List<string> { "╷ ╷" , "╶┼╴" , "╵ ╵"  }},
                new Letter { Character = "(",  ASCII = new List<string> { "╭╴"  , "│ "  , "╰╴"   }},
                new Letter { Character = ")",  ASCII = new List<string> { "╶╮"  , " │"  , "╶╯"   }},
                new Letter { Character = "-",  ASCII = new List<string> { "   " , "╶─╴" , "   "  }},
                new Letter { Character = "_",  ASCII = new List<string> { "   " , "   " , "╶─╴"  }},
                new Letter { Character = "=",  ASCII = new List<string> { "   " , "╶─╴" , "╶─╴"  }},
                new Letter { Character = "+",  ASCII = new List<string> { " ╷ " , "╶┼╴" , " ╵ "  }},
                new Letter { Character = @"\", ASCII = new List<string> { "╷ "  , "╰╮"  , " ╵"   }},
                new Letter { Character = "|",  ASCII = new List<string> { "╷"   , "│"   , "╵"    }},
                new Letter { Character = "[",  ASCII = new List<string> { "╭─ " , "│  " , "╰─ "  }},
                new Letter { Character = "{",  ASCII = new List<string> { " ╭╴" , "╶┤ " , " ╰╴"  }},
                new Letter { Character = "]",  ASCII = new List<string> { " ─╮" , "  │" , " ─╯"  }},
                new Letter { Character = "}",  ASCII = new List<string> { "╶╮ " , " ├╴" , "╶╯ "  }},
                new Letter { Character = ";",  ASCII = new List<string> { "  "  , " ╵"  , " ╯"   }},
                new Letter { Character = ":",  ASCII = new List<string> { " "   , "╵"   , "╵"    }},
                new Letter { Character = "'",  ASCII = new List<string> { "╷"   , " "   , " "    }},
                new Letter { Character = "\"", ASCII = new List<string> { "╷╷"  , "  "  , "  "   }},
                new Letter { Character = ",",  ASCII = new List<string> { "  "  , "  "  , " ╯"   }},
                new Letter { Character = ".",  ASCII = new List<string> { " "   , " "   , "╵"    }},
                new Letter { Character = "/",  ASCII = new List<string> { " ╷"  , "╭╯"  , "╵ "   }},
                new Letter { Character = "?",  ASCII = new List<string> { "╭─╮" , " ╶╯" , "╵  "  }},
            };
        }
    }

    public static void SetFont(Font font)
    {
        _font = font;
    }

    public static string TextToAsciiArt(string? text, string replaceMissingCharactersWith = "!")
    {
        var output = new System.Text.StringBuilder();

        var lines = TextToAsciiArtLines(text, replaceMissingCharactersWith);

        foreach (var line in lines) {
            output.AppendLine(line);
        }

        return output.ToString();
    }

    public static List<string> TextToAsciiArtLines(string? text, string replaceMissingCharactersWith = "")
    {
        List<string> output = new List<string>();

        List<Letter> Letters = Letters_FutureSmooth;

        if (_font != Font.FutureSmooth) {
            switch (_font) {
                case Font.AnsiRegular:
                    Letters = Letters_AnsiRegular;
                    break;

                case Font.AnsiShadow:
                    Letters = Letters_AnsiShadow;
                    break;

                case Font.Ascii9:
                    Letters = Letters_ASCII9;
                    break;

                case Font.Classy:
                    Letters = Letters_Classy;
                    break;

                case Font.CoderMini:
                    Letters = Letters_CoderMini;
                    break;

                case Font.CyberMedium:
                    Letters = Letters_CyberMedium;
                    break;
            }
        }

        if (!String.IsNullOrWhiteSpace(text)) {
            var letterHeight = Letters.Max(x => x.ASCII.Count);

            for (int i = 0; i < letterHeight; i++) {
                output.Add(String.Empty);
            }

            foreach (char c in text) {
                var letter = Letters.FirstOrDefault(x => x.Character == c.ToString().ToUpper());

                if (letter == null && !String.IsNullOrEmpty(replaceMissingCharactersWith)) {
                    letter = Letters.FirstOrDefault(x => x.Character == replaceMissingCharactersWith.ToUpper());
                }

                if (letter != null) {
                    for (int i = 0; i < letterHeight; i++) {
                        if (letter.ASCII.Count > i) {
                            output[i] += letter.ASCII[i];
                        }
                    }
                }
            }
        }

        return output;
    }

    public class Letter
    {
        public string Character { get; set; } = String.Empty;
        public List<string> ASCII { get; set; } = new List<string>();
    }
}