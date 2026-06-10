using System.Xml;
using System.Xml.Linq;

namespace FreeCRM_Utilities;

public static class UpgradeTools
{
    private static string _filePath = "";

    public static void AddMissingProjectReferenceToCsprojFile(string PathToProjectFile, string ProjectReference) {
        if (System.IO.File.Exists(PathToProjectFile)) {
            var content = System.IO.File.ReadAllText(PathToProjectFile);

            if (!String.IsNullOrWhiteSpace(content)) {
                if (!content.Contains(ProjectReference, StringComparison.InvariantCultureIgnoreCase)) {
                    // Parse the contents as XML
                    XDocument doc = XDocument.Load(PathToProjectFile);

                    // Find all <ItemGroup> elements in the XML
                    var itemGroups = doc.Descendants().Where(e => e.Name.LocalName == "ItemGroup");

                    System.Xml.Linq.XElement? itemGroup = null;

                    if (itemGroups.Any()) {
                        // See if there is an item group that already contains PackageReference items.
                        itemGroup = itemGroups.FirstOrDefault(ig => ig.Elements().Any(e => e.Name.LocalName == "ProjectReference"));

                        if (itemGroup == null) {
                            // Just use the first ItemGroup.
                            itemGroup = itemGroups.First();
                        }
                    }

                    if (itemGroup != null) {
                        // Add the project reference to the group.
                        var projectReference = new XElement(itemGroup.GetDefaultNamespace() + "ProjectReference");
                        projectReference.SetAttributeValue("Include", ProjectReference);
                        itemGroup.Add(projectReference);

                        // Sort the child items in the itemGroup with ProjectReferences first, then by the item's name, then by the Include value.
                        var sortedItems = itemGroup.Elements()
                            .OrderBy(e => e.Name.LocalName == "ProjectReference" ? 0 : 1)
                            .ThenBy(e => e.Name.LocalName)
                            .ThenBy(e => e.Attribute("Include")?.Value);

                        itemGroup.ReplaceNodes(sortedItems);
                    } else {
                        // This xml document does not contain any <ItemGroup> elements, so add one to the Project element
                        var rootElement = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Project");
                        if (rootElement != null) {
                            // Add a new child object called ItemGroup
                            var newItemGroup = new XElement(rootElement.GetDefaultNamespace() + "ItemGroup");
                            rootElement.Add(newItemGroup);

                            var newProjectReference = new XElement(newItemGroup.GetDefaultNamespace() + "ProjectReference");
                            newProjectReference.SetAttributeValue("Include", ProjectReference);
                            newItemGroup.Add(newProjectReference);
                        }
                    }

                    // Write out the updated file xml
                    var xws = new XmlWriterSettings {
                        OmitXmlDeclaration = true,
                        Indent = true,
                        IndentChars = "  ",
                    };

                    using (XmlWriter xw = XmlWriter.Create(PathToProjectFile, xws)) {
                        doc.Save(xw);
                    }
                }
            }
        }
    }

    public static void AddMissingProjectReferencesToSolutionFile(string solutionFile, List<ProjectReference> projectReferences) {
        if (System.IO.File.Exists(solutionFile)) {
            // Parse the contents as XML
            XDocument doc = XDocument.Load(solutionFile);

            // Find all <ItemGroup> elements in the XML
            var solutions = doc.Descendants().Where(e => e.Name.LocalName == "Solution");

            if (solutions != null && solutions.Any()) {
                var solution = solutions.First();

                foreach (var proj in projectReferences) {
                    var newItem = new XElement(solution.GetDefaultNamespace() + "Project");
                    newItem.SetAttributeValue("Path", proj.Path);
                    if (!String.IsNullOrWhiteSpace(proj.Id)) {
                        newItem.SetAttributeValue("Id", proj.Id);
                    }
                    solution.Add(newItem);
                }
            }

            // Write out the updated file xml
            var xws = new XmlWriterSettings {
                OmitXmlDeclaration = true,
                Indent = true,
                IndentChars = "  ",
            };

            using (XmlWriter xw = XmlWriter.Create(solutionFile, xws)) {
                doc.Save(xw);
            }
        }
    }

    static public void CopyFolder(string sourceFolder, string destFolder) {
        if (!Directory.Exists(destFolder)) {
            Directory.CreateDirectory(destFolder);
        }

        string[] files = Directory.GetFiles(sourceFolder);

        foreach (string file in files) {
            string name = Path.GetFileName(file);
            string dest = Path.Combine(destFolder, name);
            File.Copy(file, dest, true);
        }

        string[] folders = Directory.GetDirectories(sourceFolder);
        foreach (string folder in folders) {
            string name = Path.GetFileName(folder);
            string dest = Path.Combine(destFolder, name);
            CopyFolder(folder, dest);
        }
    }

    public static string CopyFile(string file, string appPath, string targetPath) {
        var fileName = file.Substring(appPath.Length + 1);

        var copyTo = System.IO.Path.Combine(targetPath, fileName);

        // Copy the file to the new location

        // Make sure the target folder exists.
        var folder = System.IO.Path.GetDirectoryName(copyTo);
        if (!String.IsNullOrWhiteSpace(folder) && !System.IO.Directory.Exists(folder)) {
            System.IO.Directory.CreateDirectory(folder);
        }

        System.IO.File.Copy(file, copyTo, true);


        var output = "Copied '" + file + "' to '" + copyTo + "'";
        return output;
    }

    public static List<string> GetAllProjectReferencesInProject(string PathToProjectFile) {
        List<string> output = new List<string>();

        if (System.IO.File.Exists(PathToProjectFile)) {
            var content = System.IO.File.ReadAllText(PathToProjectFile);

            if (!String.IsNullOrWhiteSpace(content)) {
                XDocument doc = XDocument.Load(PathToProjectFile);

                // Find all <ItemGroup> elements in the XML
                var itemGroups = doc.Descendants().Where(e => e.Name.LocalName == "ItemGroup");

                if (itemGroups.Any()) {
                    // Find any ProjectReference items in this ItemGroup
                    var projectReferences = itemGroups.Elements().Where(e => e.Name.LocalName == "ProjectReference");

                    foreach (var projectReference in projectReferences) {
                        var includeValue = projectReference.Attribute("Include")?.Value;
                        if (!String.IsNullOrWhiteSpace(includeValue)) {
                            output.Add(includeValue);
                        }
                    }
                }
            }
        }

        return output;
    }

    public static List<ProjectReference> GetAllProjectReferencesInSolution(string PathToSolutionFile) {
        List<ProjectReference> output = new List<ProjectReference>();

        try {
            if (System.IO.File.Exists(PathToSolutionFile)) {
                var content = System.IO.File.ReadAllText(PathToSolutionFile);

                if (!String.IsNullOrWhiteSpace(content)) {
                    XDocument doc = XDocument.Load(PathToSolutionFile);

                    // Find all <ItemGroup> elements in the XML
                    var solutions = doc.Descendants().Where(e => e.Name.LocalName == "Solution");

                    if (solutions != null && solutions.Any()) {
                        var solution = solutions.First();
                        // Find any Project items in this Solution
                        var projectReferences = solution.Elements().Where(e => e.Name.LocalName == "Project");

                        foreach (var projectReference in projectReferences) {
                            var path = projectReference.Attribute("Path")?.Value;
                            if (!String.IsNullOrWhiteSpace(path)) {
                                output.Add(new ProjectReference {
                                    Path = path,
                                    Id = projectReference.Attribute("Id")?.Value,
                                });
                            }
                        }
                    }
                }
            }
        } catch { }


        return output;
    }

    public static Enums GetEnums(string fileName, string fileContents) {
        var output = new Enums {
            Filename = fileName,
        };

        // Split the file into lines.
        var lines = fileContents.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        bool inEnum = false;
        string enumName = String.Empty;
        var properties = new List<string>();

        foreach (var line in lines) {
            var thisLine = line.Trim();

            if (inEnum) {
                if (thisLine == "}") {
                    inEnum = false;

                    // Add this to the output.
                    output.EnumValues.Add(new EnumValues {
                        Name = enumName,
                        Properties = properties,
                    });
                } else {
                    // We are in an enum tag, so add any actual enum values.
                    if (thisLine == "{") {
                        // Ignore this starting tag
                    } else if (thisLine.StartsWith("//")) {
                        // This is a comment.
                    } else if (!String.IsNullOrWhiteSpace(thisLine)) {
                        properties.Add(thisLine.Replace(",", ""));
                    }
                }
            } else if (thisLine.StartsWith("public enum ")) {
                inEnum = true;

                enumName = thisLine.Replace("public enum ", "").Trim();
                properties = new List<string>();
            }
        }

        return output;
    }

    public static ProjectNugetPackages GetNugetPackagesFromProjectFile(string projectFile) {
        var output = new ProjectNugetPackages();
        output.ProjectFile = projectFile;

        string contents = System.IO.File.ReadAllText(projectFile);

        if (!String.IsNullOrWhiteSpace(contents) && contents.Contains("<PackageReference Include=")) {
            // Find all lines that are nuget package references
            var lines = contents.Split(new[] { '\r', '\n' });
            int index = 0;

            //var nugetLines = contents.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            //	.Where(line => line.ToLower().Contains("<packagereference include="))
            //	.ToList();

            foreach (var line in lines) {
                index++;

                if (line.ToLower().Contains("<packagereference include=")) {
                    //line.Dump("Nuget Package Reference");
                    var include = GetPackageReferenceValue(line, "Include");
                    var version = GetPackageReferenceValue(line, "Version");

                    if (!String.IsNullOrWhiteSpace(include) && !String.IsNullOrWhiteSpace(version)) {
                        output.Packages.Add(new NugetPackageDetails {
                            Name = include,
                            Version = version,
                        });

                        output.LastProjectFilePackageLine = index;
                    }
                }
            }
        }

        return output;
    }

    public static string GetPackageReferenceValue(string line, string tag) {
        string output = String.Empty;

        var start = line.ToLower().IndexOf(tag.ToLower() + "=");
        if (start > -1) {
            start = line.IndexOf("\"", start + 1);

            if (start > -1) {
                var end = line.IndexOf("\"", start + 1);
                if (end > -1) {
                    output = line.Substring(start + 1, end - start - 1);
                }
            }
        }

        return output;
    }

    public static List<RequiredElement> GetRequiredElements() {
        var output = new List<RequiredElement> {
            new RequiredElement {
                RelativePath = @"CRM\Components\Modules.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "string applicationUrl = data.ApplicationUrl(HttpContextAccessor.HttpContext);",
                        Target = "CodeBlockLine",
                        Parent = "@{",
                    },
                    new RequiredElementItem {
                        Item = "string appVersion = data.Version;",
                        Target = "CodeBlockLine",
                        Parent = "@{",
                    },
                    new RequiredElementItem {
                        Item = "string basePath = !String.IsNullOrWhiteSpace(ConfigurationHelper.BasePath) ? ConfigurationHelper.BasePath :",
                        Target = "CodeBlockLine",
                        Parent = "@{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public string Module { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM\Controllers\DataController.App.cs",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "public partial class DataController",
                        Target = "PartialClassDefinition",
                    },
                    new RequiredElementItem {
                        Item = "private DataObjects.User Authenticate_App()",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataController",
                    },
                    new RequiredElementItem {
                        Item = "private async Task<bool> SignalRUpdateApp(DataObjects.SignalRUpdate update)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataController",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM\Program.App.cs",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "public static List<string> AddServerReferencesApp(List<string> serverReferences)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class Program",
                    },
                    new RequiredElementItem {
                        Item = "public static WebApplicationBuilder AppModifyBuilderEnd(WebApplicationBuilder builder)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class Program",
                    },
                    new RequiredElementItem {
                        Item = "public static WebApplicationBuilder AppModifyBuilderStart(WebApplicationBuilder builder)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class Program",
                    },
                    new RequiredElementItem {
                        Item = "public static WebApplication AppModifyEnd(WebApplication app)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class Program",
                    },
                    new RequiredElementItem {
                        Item = "public static WebApplication AppModifyStart(WebApplication app)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class Program",
                    },
                    new RequiredElementItem {
                        Item = "public static List<string> AuthenticationPoliciesApp",
                        Target = "PartialClassMethod",
                        Parent = "public partial class Program",
                    },
                    new RequiredElementItem {
                        Item = "public static ConfigurationHelperLoader ConfigurationHelpersLoadApp(ConfigurationHelperLoader loader, WebApplicationBuilder builder)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class Program",
                    },
                },
            },





            new RequiredElement {
                RelativePath = @"CRM.Client\DataModel.App.cs",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "private bool HaveDeletedRecordsApp",
                        Target = "PartialClassMethod",
                        Parent = "public partial class BlazorDataModel",
                    },
                    new RequiredElementItem {
                        Item = "public bool PrecompileBlazorPlugins",
                        Target = "PartialClassMethod",
                        Parent = "public partial class BlazorDataModel",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Helpers.App.cs",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "public static Dictionary<string, List<string>> AppIcons",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                    new RequiredElementItem {
                        Item = "public static List<DataObjects.Tag> AvailableTagListApp(DataObjects.TagModule? Module, List<Guid> ExcludeTags)",
                        Module = "Tags",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                    new RequiredElementItem {
                        Item = "private static List<string> GetDeletedRecordTypesApp()",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                    new RequiredElementItem {
                        Item = "public static List<DataObjects.DeletedRecordItem>? GetDeletedRecordsForAppType(DataObjects.DeletedRecords deletedRecords, string type)",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                    new RequiredElementItem {
                        Item = "public static string GetDeletedRecordsLanguageTagForAppType(string type)",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                    new RequiredElementItem {
                        Item = "public static async Task MainLayoutOnInitializedAsyncApp()",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                    new RequiredElementItem {
                        Item = "public static List<DataObjects.MenuItem> MenuItemsApp",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                    new RequiredElementItem {
                        Item = "public static List<DataObjects.MenuItem> MenuItemsAdminApp",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                    new RequiredElementItem {
                        Item = "public static async Task ProcessSignalRUpdateApp(DataObjects.SignalRUpdate update)",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                    new RequiredElementItem {
                        Item = "public static async Task ProcessSignalRUpdateAppUndelete(DataObjects.SignalRUpdate update)",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                    new RequiredElementItem {
                        Item = "private async static Task ReloadModelApp(DataObjects.BlazorDataModelLoader? blazorDataModelLoader)",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                    new RequiredElementItem {
                        Item = "private static void UpdateModelDeletedRecordCountsForAppItems(DataObjects.DeletedRecords deletedRecords)",
                        Target = "PartialClassMethod",
                        Parent = "public static partial class Helpers",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Layout\MainLayout.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public bool Enabled { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public RenderFragment? BodyContent { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool Loading { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool OverrideCompleteLayout { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\About.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public bool InDialog { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                },
                Module = "About",
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\AppSettings.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public bool Enabled { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public string? Area { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public DataObjects.ApplicationSettings Value { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<DataObjects.ApplicationSettings> ValueChanged { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public DataObjects.ModuleAction Save(DataObjects.ApplicationSettings appSettings)",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\EditAppointment.App.razor",
                Module = "Appointments",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public bool AllowEdit { get; set; }",
                        Module = "Appointments",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool Enabled { get; set; }",
                        Module = "Appointments",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public string? Area { get; set; }",
                        Module = "Appointments",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool ShowAppointmentTab { get; set; }",
                        Module = "Appointments",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public string TabText { get; set; }",
                        Module = "Appointments",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public DataObjects.Appointment Value { get; set; }",
                        Module = "Appointments",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<DataObjects.Appointment> ValueChanged { get; set; }",
                        Module = "Appointments",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public DataObjects.ModuleAction Save(DataObjects.Appointment appointment)",
                        Module = "Appointments",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\EditDepartment.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public DataObjects.Department Value { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<DataObjects.Department> ValueChanged { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\EditDepartmentGroup.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public DataObjects.DepartmentGroup Value { get; set; } = new DataObjects.DepartmentGroup();",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<DataObjects.DepartmentGroup> ValueChanged { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\EditTag.App.razor",
                Module = "Tags",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public string Area { get; set; }",
                        Module = "Tags",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public DataObjects.Tag Value { get; set; }",
                        Module = "Tags",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<DataObjects.Tag> ValueChanged { get; set; }",
                        Module = "Tags",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public DataObjects.ModuleAction Save(DataObjects.Tag tag)",
                        Module = "Tags",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\EditTenant.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public DataObjects.Tenant Value { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<DataObjects.Tenant> ValueChanged { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public DataObjects.ModuleAction Save(DataObjects.Tenant tenant)",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\EditUser.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public string Area { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool OverridePage { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public DataObjects.User Value { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<DataObjects.User> ValueChanged { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public DataObjects.ModuleAction Save(DataObjects.User user)",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\EditUserGroup.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public string groupid { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool OverridePage { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public DataObjects.UserGroup Value { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<DataObjects.UserGroup> ValueChanged { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public DataObjects.ModuleAction Save(DataObjects.User user)",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\Index.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "public bool RequireLogin { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public bool ShowTestPageLinks {get; set; }",
                        Module = "SamplePages",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\NavigationMenu.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public bool Enabled { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool Loading { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\OffcanvasPopoutMenu.App.razor",
                Items = new List<RequiredElementItem> {

                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\Settings.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public bool Enabled { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public string? Area { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool ShowAppSettingsTab { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public DataObjects.Tenant Value { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<DataObjects.Tenant> ValueChanged { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public DataObjects.ModuleAction Save(DataObjects.Tenant tenant)",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\UserGroups.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public bool OverridePage { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public List<DataObjects.UserGroup> Value { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<List<DataObjects.UserGroup>> ValueChanged { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public DataObjects.ModuleAction Save(DataObjects.User user)",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.Client\Shared\AppComponents\Users.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public string Area { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool OverridePage { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public List<DataObjects.User> Value { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<List<DataObjects.User>> ValueChanged { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public DataObjects.ModuleAction Save(DataObjects.User user)",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                },
            },





            new RequiredElement {
                RelativePath = @"CRM.DataAccess\DataAccess.App.cs",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "Task<DataObjects.BooleanResponse> ProcessBackgroundTasksApp(Guid TenantId, long Iteration);",
                        Target = "PartialInterfaceItem",
                        Parent = "public partial interface IDataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private int _accountLockoutMaxAttempts =",
                        Target = "PartialClassProperty",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private int _accountLockoutMinutes =",
                        Target = "PartialClassProperty",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private string _appName =",
                        Target = "PartialClassProperty",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private string _copyright =",
                        Target = "PartialClassProperty",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private DateOnly _released = DateOnly.FromDateTime(Convert.ToDateTime(",
                        Target = "PartialClassProperty",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private bool _tokenAutoRenew =",
                        Target = "PartialClassProperty",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private int _tokenDays =",
                        Target = "PartialClassProperty",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private bool _useMigrations =",
                        Target = "PartialClassProperty",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private string _version =",
                        Target = "PartialClassProperty",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private Dictionary<string, string> AppLanguage",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private void DataAccessAppInit()",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private async Task<DataObjects.BooleanResponse> DeleteAllPendingDeletedRecordsApp(Guid TenantId, DateTime OlderThan)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private async Task<DataObjects.BooleanResponse> DeleteRecordImmediatelyApp(string? Type, Guid RecordId, DataObjects.User CurrentUser)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private async Task<DataObjects.BooleanResponse> DeleteRecordsApp(object Rec, DataObjects.User? CurrentUser = null)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private DataObjects.ApplicationSettings GetApplicationSettingsApp(DataObjects.ApplicationSettings settings)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private DataObjects.ApplicationSettingsUpdate GetApplicationSettingsUpdateApp(DataObjects.ApplicationSettingsUpdate settings)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private async Task<DataObjects.BlazorDataModelLoader> GetBlazorDataModelApp(DataObjects.BlazorDataModelLoader blazorDataModelLoader, DataObjects.User? CurrentUser = null)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private async Task<DataObjects.DeletedRecordCounts> GetDeletedRecordCountsApp(Guid TenantId, DataObjects.DeletedRecordCounts deletedRecordCounts)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private List<DataObjects.FilterColumn> GetFilterColumnsApp(string Type, string Position, DataObjects.Language Language, DataObjects.User? CurrentUser = null)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private async Task<DataObjects.DeletedRecords> GetDeletedRecordsApp(Guid TenantId, DataObjects.DeletedRecords deletedRecords)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private void GetDataApp(object Rec, object DataObject, DataObjects.User? CurrentUser = null)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "public async Task<DataObjects.BooleanResponse> ProcessBackgroundTasksApp(Guid TenantId, long Iteration)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private async Task<DataObjects.ApplicationSettings> SaveApplicationSettingsApp(DataObjects.ApplicationSettings settings, DataObjects.User CurrentUser)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private void SaveDataApp(object Rec, object DataObject, DataObjects.User? CurrentUser = null)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private IQueryable<EFModels.EFModels.User>? SortUsersApp(IQueryable<EFModels.EFModels.User>? recs, string SortBy, bool Ascending)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                    new RequiredElementItem {
                        Item = "private async Task<DataObjects.BooleanResponse> UndeleteRecordApp(string? Type, Guid RecordId, DataObjects.User CurrentUser)",
                        Target = "PartialClassMethod",
                        Parent = "public partial class DataAccess",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.DataAccess\GraphAPI.App.cs",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "public partial class GraphClient",
                        Target = "PartialClassDefinition",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.DataAccess\RandomPasswordGenerator.App.cs",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "public static partial class PasswordGenerator",
                        Target = "PartialClassDefinition",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.DataAccess\Utilities.App.cs",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "public static partial class Utilities",
                        Target = "PartialClassDefinition",
                    },
                },
            },





            new RequiredElement {
                RelativePath = @"CRM.DataObjects\ConfigurationHelper.App.cs",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "public partial interface IConfigurationHelper",
                        Target = "PartialInterfaceDefinition",
                    },
                    new RequiredElementItem {
                        Item = "public partial class ConfigurationHelper : IConfigurationHelper",
                        Target = "RootItem",
                    },
                    new RequiredElementItem {
                        Item = "public partial class ConfigurationHelperLoader",
                        Target = "RootItem",
                    },
                    new RequiredElementItem {
                        Item = "public partial class ConfigurationHelperConnectionStrings",
                        Target = "RootItem",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.DataObjects\DataObjects.App.cs",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "public partial class DataObjects",
                        Target = "PartialClassDefinition",
                    },
                    new RequiredElementItem {
                        Item = "public partial class SignalRUpdateType",
                        Parent = "public partial class DataObjects",
                        Target = "PartialClassMethod",
                    },
                },
            },
            new RequiredElement {
                RelativePath = @"CRM.DataObjects\GlobalSettings.App.cs",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "public static partial class GlobalSettings",
                    },
                },
            },
        };

        int sortOrder = 0;

        foreach (var item in output) {
            foreach (var i in item.Items) {
                i.SortOrder = sortOrder;
                sortOrder++;
            }
        }

        return output;
    }

    public static bool IsGuid(string? value) {
        return Guid.TryParse(value, out _);
    }

    public static bool RestoreSolutionProjectGuids(string solutionFile, List<ProjectReference> projectReferences) {
        bool output = false;

        if (System.IO.File.Exists(solutionFile)) {
            // Parse the contents as XML
            XDocument doc = XDocument.Load(solutionFile);

            // Find all <ItemGroup> elements in the XML
            var solutions = doc.Descendants().Where(e => e.Name.LocalName == "Solution");

            if (solutions != null && solutions.Any()) {
                var solution = solutions.First();

                var projects = solution.Elements().Where(e => e.Name.LocalName == "Project");

                foreach (var project in projects) {
                    var path = project.Attribute("Path")?.Value;
                    if (!String.IsNullOrWhiteSpace(path)) {
                        // Find the matching item.
                        var matchingItem = projectReferences.FirstOrDefault(pr => pr.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase));
                        if (matchingItem != null && !String.IsNullOrWhiteSpace(matchingItem.Id)) {
                            var currentId = project.Attribute("Id")?.Value;

                            if (currentId != matchingItem.Id) {
                                project.Attribute("Id")?.SetValue(matchingItem.Id);
                                output = true;
                            }
                        }
                    }
                }
            }

            // Only need to save if items were updated
            if (output) {
                // Write out the updated file xml
                var xws = new XmlWriterSettings {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    IndentChars = "  ",
                };

                using (XmlWriter xw = XmlWriter.Create(solutionFile, xws)) {
                    doc.Save(xw);
                }
            }
        }

        return output;
    }

    public static void SetFilePath(string filePath) {
        _filePath = filePath;
    }

    public static string UpdateEnumInContent(EnumValues item, string content) {
        var output = new System.Text.StringBuilder();

        bool inEnum = false;
        var lines = content.Split(new string[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.None).ToList();
        foreach (var line in lines) {
            var trimmed = line.Trim();

            if (trimmed.StartsWith("public enum " + item.Name)) {
                inEnum = true;

                // Add the new enum to the output.
                output.AppendLine("\tpublic enum " + item.Name);
                output.AppendLine("\t{");

                foreach (var prop in item.Properties) {
                    output.AppendLine("\t\t" + prop + ",");
                }
                output.AppendLine("\t}");
            } else if (inEnum) {
                if (trimmed == "}") {
                    inEnum = false;
                }
            } else {
                output.AppendLine(line);
            }
        }

        return output.ToString();
    }

    public static string UpdateEnumInFile(Enums enumItem) {
        string output = String.Empty;

        if (System.IO.File.Exists(enumItem.Filename)) {
            output = System.IO.File.ReadAllText(enumItem.Filename);

            foreach (var item in enumItem.EnumValues) {
                output = UpdateEnumInContent(item, output);
            }
        }

        return output;
    }
}