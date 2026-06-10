namespace FreeCRM_Utilities;

public class Enums
{
    public string Filename { get; set; } = String.Empty;
    public List<EnumValues> EnumValues { get; set; } = new List<EnumValues>();
}

public class EnumValues
{
    public string Name { get; set; } = String.Empty;
    public List<string> Properties { get; set; } = new List<string>();
}

public class LanguageItem
{
    public string? Section { get; set; }
    public string Tag { get; set; } = String.Empty;
    public string Value { get; set; } = String.Empty;
}

public class LineInFile
{
    public string File { get; set; } = String.Empty;
    public string OriginalLine { get; set; } = String.Empty;
    public string NewLine { get; set; } = String.Empty;
}

public class MessageItem
{
    public bool Error { get; set; }
    public string Message { get; set; } = String.Empty;
}

public class Module
{
    public string Name { get; set; } = String.Empty;
    public List<string> FilesToRemove { get; set; } = new List<string>();
    public List<string> FoldersToRemove { get; set; } = new List<string>();
    public List<LineInFile> ReplaceLinesInFiles { get; set; } = new List<LineInFile>();
}

public class NugetPackageDetails
{
    public string Name { get; set; } = String.Empty;
    public string Version { get; set; } = String.Empty;
}

public class ProjectNugetPackages
{
    public string ProjectFile { get; set; } = String.Empty;
    public List<NugetPackageDetails> Packages { get; set; } = new List<NugetPackageDetails>();
    public int LastProjectFilePackageLine { get; set; }
}

public class ProjectReference
{
    public string Path { get; set; } = String.Empty;
    public string? Id { get; set; }
}

public class RequiredElement
{
    public string RelativePath { get; set; } = String.Empty;
    public List<RequiredElementItem> Items { get; set; } = new List<RequiredElementItem>();
    public string? Module { get; set; }
}

public class RequiredElementItem
{
    public string Item { get; set; } = String.Empty;
    public string? Module { get; set; }
    public string Target { get; set; } = String.Empty;
    public string? Parent { get; set; }
    public int SortOrder { get; set; }
}