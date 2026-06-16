using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Util;

public static class UpgradeTools
{
    private static string _appNamespace = String.Empty;
    private static string _filePath = String.Empty;
    private static string _oldNamespace = String.Empty;

    public static void DoUpgrade(string? pathToOldApp, bool fromProgramStart)
    {
        bool checkForMissingProjectsInSolutions = true;
        bool copyAppFiles = true;
        bool copyEntityFrameworkFiles = true;
        bool copyEnums = true;
        bool updateNugetPackages = true;
        bool checkForMissingCode = true;
        bool checkLanguageItems = true;

        List<MessageItem> returnMessages = new List<MessageItem>();

        if (!String.IsNullOrWhiteSpace(pathToOldApp)) {
            if (!System.IO.Path.Exists(pathToOldApp)) {
                returnMessages.Add(new MessageItem { Message = "Unable to upgrade, the specified path", Error = true });
                returnMessages.Add(new MessageItem { Message = "'" + pathToOldApp + "'", Error = true });
                returnMessages.Add(new MessageItem { Message = "does not exist.", Error = true });
                Utils.HomeScreen(returnMessages);
                return;
            }
        } else {
            returnMessages.Add(new MessageItem { Message = "Unable to upgrade, no source path specified.", Error = true });
            Utils.HomeScreen(returnMessages);
            return;
        }

        Console.Clear();
        Utils.ConsoleLog("Current directory:         " + _filePath);

        // Find the solution file in the directory.
        var oldSolutionFile = String.Empty;
        var oldSolutionFiles = System.IO.Directory.GetFiles(pathToOldApp, "*.sln*");
        if (oldSolutionFiles.Count() == 1) {
            _oldNamespace = System.IO.Path.GetFileNameWithoutExtension(oldSolutionFiles[0]);
            Utils.ConsoleLog("Old application namespace: " + _oldNamespace);
            oldSolutionFile = oldSolutionFiles[0];
        } else {
            returnMessages.Add(new MessageItem { Message = "Unable to upgrade, no solution file (.sln/.slnx) found in", Error = true });
            returnMessages.Add(new MessageItem { Message = "'" + pathToOldApp + "'.", Error = true });
            Utils.HomeScreen(returnMessages);
            return;
        }

        var solutionFile = String.Empty;
        var newSolutionFiles = System.IO.Directory.GetFiles(_filePath, "*.sln*");
        if (newSolutionFiles.Count() == 1) {
            _appNamespace = System.IO.Path.GetFileNameWithoutExtension(newSolutionFiles[0]);
            Utils.ConsoleLog("New application namespace: " + _appNamespace);
            solutionFile = newSolutionFiles[0];
        } else {
            returnMessages.Add(new MessageItem { Message = "Unable to upgrade, unable to find the solution file (.sln/.slnx) found in the new directory", Error = true });
            returnMessages.Add(new MessageItem { Message = "'" + _filePath + "'.", Error = true });
            Utils.HomeScreen(returnMessages);
            return;
        }

        if (_appNamespace != _oldNamespace) {
            returnMessages.Add(new MessageItem { Message = "Unable to upgrade, The old namespace '" + _oldNamespace + "' does not match the new namespace '" + _appNamespace + "'.", Error = true });
            returnMessages.Add(new MessageItem { Message = "Run the rename command first.", Error = true });
            Utils.HomeScreen(returnMessages);
            return;
        }

        Utils.ConsoleLog("");
        Utils.ConsoleLog("Beginning upgrade process...");

        var html = new StringBuilder();

        html.AppendLine("<!doctype html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("  <style>");
        html.AppendLine("    html, body, h1, h2, p, span, ul, li, * {");
        html.AppendLine("      font-family: 'Trebuchet MS', Tahoma, sans-serif;");
        html.AppendLine("      background-color:#fff;");
        html.AppendLine("      color:#000;");
        html.AppendLine("    }");
        html.AppendLine("");
        html.AppendLine("    .mb-2 {");
        html.AppendLine("      margin-bottom:1em;");
        html.AppendLine("    }");
        html.AppendLine("");
        html.AppendLine("  code {");
        html.AppendLine("    font-family: Monaco, Consolas, \"Courier New\", monospace;");
        html.AppendLine("    font-size: 0.875em;");
        html.AppendLine("    word-wrap: break-word;");
        html.AppendLine("    color:blue;");
        html.AppendLine("  }");
        html.AppendLine("");
        html.AppendLine("  .comment {");
        html.AppendLine("    color:#008000;");
        html.AppendLine("  }");
        html.AppendLine("");
        html.AppendLine("    .module {");
        html.AppendLine("      color:red;");
        html.AppendLine("      font-weight: bold;");
        html.AppendLine("      font-size:.8em;");
        html.AppendLine("      margin-left:.5em;");
        html.AppendLine("    }");
        html.AppendLine("  </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        html.AppendLine("<h1>Upgrade Report</h1>");

        var upgradeReportWarnings = new List<string> {
                "This utility overwrites built-in FreeCRM files with those files from the current version of your application. " +
                "Some of those files may have had new items added that are required for the updated version of the FreeCRM application. " +
                "In those cases, an attempt will be made to download the source file from github and inject the missing items. " +
                "Try to build your application and if you get build failures refer to this report for items that " +
                "you may need to manually copy from a fresh copy the FreeCRM solution. " +
                "Some of the missing items may be related to modules that have been removed from your application. " +
                "In those cases you can ignore these warnings if your application builds without any errors.",

                "This tool will also completely copy your EFModels and overwrite any EFModels in the target application. " +
                "This can also cause breaking changes if any new items have been added to the updated application. " +
                "In those cases you will need to try and build your application and find any errors related to missing " +
                "EF changes and you would need to manually update your database and regenerate your EF Models following " +
                "the instructions found in the Setup.txt file in the EFModels project.",
            };

        if (upgradeReportWarnings.Any()) {
            foreach (var item in upgradeReportWarnings) {
                html.AppendLine("<div class=\"mb-2\">");
                html.AppendLine("  " + item);
                html.AppendLine("</div>");
            }
        }

        List<string> removedModules = new List<string>();
        var removedModulesLog = System.IO.Path.Combine(_filePath, "RemovedModules.log");

        if (System.IO.File.Exists(removedModulesLog)) {
            var lines = System.IO.File.ReadAllLines(removedModulesLog);
            if (lines != null && lines.Length > 0) {
                foreach (var line in lines) {
                    if (!String.IsNullOrWhiteSpace(line)) {
                        removedModules.Add(line.Trim());
                    }
                }
            }
        }

        // Need to copy the appsettings.json and EFModels setup.txt files from the old solution to the new solution.
        html.AppendLine("<h2>Migrated Previous Files and Settings</h2>");
        html.AppendLine("<ul>");

        var appSettingsFile = System.IO.Path.Combine(pathToOldApp, _appNamespace, "appsettings.json");
        var copiedAppSettingsFile = CopyFile(appSettingsFile, pathToOldApp, _filePath);
        Utils.ConsoleLog(" - " + copiedAppSettingsFile);
        html.AppendLine("<li>");
        html.AppendLine(copiedAppSettingsFile);
        html.AppendLine(" -");
        html.AppendLine(" If any new settings have been added to the new appsettings.json file you will");
        html.AppendLine(" need to manually merge those new items into your existing appsettings.json file.");
        html.AppendLine("</li>");

        var filesToCopyFromCurrentVersion = new List<string> {
            System.IO.Path.Combine(pathToOldApp, "README.md"),
            System.IO.Path.Combine(pathToOldApp, _appNamespace + ".EFModels", "Setup.txt"),
            System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "tsconfig.json"),
            System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "wwwroot", "apple-touch-icon.png"),
            System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "wwwroot", "favicon.ico"),
            System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "wwwroot", "favicon.svg"),
            System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "wwwroot", "favicon-96x96.png"),
            System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "wwwroot", "favicon-dev.ico"),
            System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "wwwroot", "site.webmanifest"),
            System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "wwwroot", "web-app-manifest-192x192.png"),
            System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "wwwroot", "web-app-manifest-512x512.png"),
            System.IO.Path.Combine(pathToOldApp, _appNamespace, "web.config"),
        };

        foreach (var file in filesToCopyFromCurrentVersion) {
            if (System.IO.File.Exists(file)) {
                var copiedFile = CopyFile(file, pathToOldApp, _filePath);
                Utils.ConsoleLog(" - " + copiedFile);
                html.AppendLine("<li>" + copiedFile + "</li>");
            }
        }

        // Copy any additional CSS files other than the built-in files.
        var cssFolder = System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "wwwroot", "css");
        var cssFiles = System.IO.Directory.GetFiles(cssFolder, "*.css");
        if (cssFiles.Any()) {
            foreach (var cssFile in cssFiles) {
                var cssFileName = System.IO.Path.GetFileName(cssFile);

                switch (cssFileName.ToLower()) {
                    // Ignore the built-in files.
                    case "site.App.css":
                    case "site.css":
                    case "tags.css":
                    case "themes.css":
                        break;

                    default:
                        var copiedCssFile = CopyFile(cssFile, pathToOldApp, _filePath);
                        Utils.ConsoleLog(" - " + copiedCssFile);
                        html.AppendLine("<li>" + copiedCssFile + "</li>");
                        break;
                }
            }
        }

        // Copy any additional folders from the wwwroot/js and wwwroot/bin folders other than the defaults.
        var jsFolder = System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "wwwroot", "js");
        if (System.IO.Directory.Exists(jsFolder)) {
            var jsFolders = System.IO.Directory.EnumerateDirectories(jsFolder);
            if (jsFolders.Any()) {
                foreach (var folder in jsFolders) {
                    var folderName = System.IO.Path.GetFileName(folder);

                    switch (folderName.ToLower()) {
                        // Ignore the built-in folders
                        case "bootstrap":
                        case "jquery":
                        case "sortablejs":
                            break;

                        default:
                            var copyFolderTo = System.IO.Path.Combine(_filePath, _appNamespace + ".Client", "wwwroot", "js", folderName);
                            CopyFolder(folder, copyFolderTo);

                            Utils.ConsoleLog(" - Copied Folder '" + folder + "' to '" + copyFolderTo + "'");
                            html.AppendLine("<li>Copied Folder '" + folder + "' to '" + copyFolderTo + "'</li>");
                            break;
                    }
                }
            }

            var jsFiles = System.IO.Directory.EnumerateFiles(jsFolder);
            if (jsFiles.Any()) {
                foreach (var file in jsFiles) {
                    var copiedJsFile = CopyFile(file, pathToOldApp, _filePath);

                    Utils.ConsoleLog(" - " + copiedJsFile);
                    html.AppendLine("<li>" + copiedJsFile + "</li>");
                }
            }
        }

        var libFolder = System.IO.Path.Combine(pathToOldApp, _appNamespace + ".Client", "wwwroot", "lib");
        if (System.IO.Directory.Exists(libFolder)) {
            var libFolders = System.IO.Directory.EnumerateDirectories(libFolder);
            if (libFolders.Any()) {
                foreach (var folder in libFolders) {
                    var folderName = System.IO.Path.GetFileName(folder);

                    switch (folderName.ToLower()) {
                        // Ignore the built-in folders
                        case "highcharts":
                            break;

                        default:
                            var copyFolderTo = System.IO.Path.Combine(_filePath, _appNamespace + ".Client", "wwwroot", "js", folderName);
                            CopyFolder(folder, copyFolderTo);

                            Utils.ConsoleLog(" - Copied Folder '" + folder + "' to '" + copyFolderTo + "'");
                            html.AppendLine("<li>Copied Folder '" + folder + "' to '" + copyFolderTo + "'</li>");
                            break;
                    }
                }
            }
        }

        // Copy any plugin files and folders from the source including any files in the BlazorComponents folder.
        var pluginsFolder = System.IO.Path.Combine(pathToOldApp, _appNamespace, "PluginFiles");
        if (System.IO.Directory.Exists(pluginsFolder)) {
            var pluginFiles = System.IO.Directory.EnumerateFiles(pluginsFolder);
            if (pluginFiles.Any()) {
                foreach (var file in pluginFiles) {
                    var pluginFileName = System.IO.Path.GetFileName(file);

                    switch (pluginFileName.ToLower()) {
                        // Ignore the built-in files
                        case "example1.cs":
                        case "example2.cs":
                        case "example3.cs":
                        case "examplebackgroundprocess.cs":
                        case "helloworld.assemblies":
                        case "helloworld.plugin":
                        case "loginwithprompts.cs":
                        case "plugins.md":
                        case "userupdate.cs":
                            break;

                        default:
                            var copyPluginFile = CopyFile(file, pathToOldApp, _filePath);

                            Utils.ConsoleLog(" - " + copyPluginFile);
                            html.AppendLine("<li>" + copyPluginFile + "</li>");
                            break;
                    }
                }
            }

            var blazorComponentsFolder = System.IO.Path.Combine(pluginsFolder, "BlazorComponents");
            if (System.IO.Directory.Exists(blazorComponentsFolder)) {
                var copyBlazorComponentsTo = System.IO.Path.Combine(_filePath, _appNamespace, "PluginFiles", "BlazorComponents");
                CopyFolder(blazorComponentsFolder, copyBlazorComponentsTo);

                Utils.ConsoleLog(" - Copied '" + blazorComponentsFolder + "' to '" + copyBlazorComponentsTo + "'");
                html.AppendLine("<li>Copied '" + blazorComponentsFolder + "' to '" + copyBlazorComponentsTo + "'</li>");
            }
        }

        // Need to restore the UserSecretsId from the CRM\CRM.csproj file if it exists in the old solution
        // and add it to the new solution .csproj.
        var oldRootProjectFile = System.IO.Path.Combine(pathToOldApp, _appNamespace, _appNamespace + ".csproj");
        if (System.IO.File.Exists(oldRootProjectFile)) {
            var oldRootProjectFileLines = System.IO.File.ReadAllLines(oldRootProjectFile);

            var userSecretsId = String.Empty;

            if (oldRootProjectFileLines.Any()) {
                foreach (var line in oldRootProjectFileLines) {
                    if (line.Trim().StartsWith("<UserSecretsId>") && line.Trim().EndsWith("</UserSecretsId>")) {
                        userSecretsId = line.ToLower()
                            .Replace("<usersecretsid>", "")
                            .Replace("</usersecretsid>", "")
                            .Trim();
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(userSecretsId) && IsGuid(userSecretsId)) {
                // We have a valid GUID for the old UserSecretsId, so let's update
                // the target application's .csproj file with this UserSecretsId.
                var newRootProjectFile = System.IO.Path.Combine(_filePath, _appNamespace, _appNamespace + ".csproj");
                if (System.IO.File.Exists(newRootProjectFile)) {
                    var newRootProjectFileLines = System.IO.File.ReadAllLines(newRootProjectFile);

                    if (newRootProjectFileLines.Any()) {
                        var newLines = new System.Text.StringBuilder();

                        foreach (var line in newRootProjectFileLines) {
                            if (line.Trim().StartsWith("<UserSecretsId>") && line.Trim().EndsWith("</UserSecretsId>")) {
                                newLines.AppendLine("    <UserSecretsId>" + userSecretsId + "</UserSecretsId>");
                            } else {
                                newLines.AppendLine(line);
                            }
                        }

                        System.IO.File.WriteAllText(newRootProjectFile, newLines.ToString());

                        Utils.ConsoleLog(" - Updated UserSecretsId in '" + newRootProjectFile + "' to match the old UserSecretsId.");
                        html.AppendLine("<li>Updated UserSecretsId in '" + newRootProjectFile + "' to match the old UserSecretsId.</li>");
                    }
                }
            }
        }

        // Restore the original project GUIDs in the solution file.
        var oldSolutionExtension = System.IO.Path.GetExtension(oldSolutionFile);
        if (!String.IsNullOrWhiteSpace(oldSolutionExtension) && oldSolutionExtension.ToLower() == ".slnx") {
            var oldSolutionReferences = GetAllProjectReferencesInSolution(oldSolutionFile);
            if (oldSolutionReferences.Any()) {
                var restoredSolutionGuids = RestoreSolutionProjectGuids(solutionFile, oldSolutionReferences);
                if (restoredSolutionGuids) {
                    Utils.ConsoleLog(" - Restored original project GUIDs in the solution file to match the old solution.");
                    html.AppendLine("<li>Restored original project GUIDs in the solution file to match the old solution.</li>");
                }
            }
        }

        html.AppendLine("</ul>");

        if (checkForMissingProjectsInSolutions) {
            // Get all directories in the target.
            var appDirectories = System.IO.Directory.GetDirectories(pathToOldApp);
            if (appDirectories != null && appDirectories.Any()) {
                var manuallyMigrateFolders = new List<string>();

                foreach (var appDirectory in appDirectories) {
                    var folderName = appDirectory.Substring(pathToOldApp.Length + 1);
                    var folderLower = folderName.ToLower();

                    if (folderLower.StartsWith(_appNamespace.ToLower())) {
                        if (
                            folderLower == _appNamespace.ToLower() ||
                            folderLower == _appNamespace.ToLower() + ".client" ||
                            folderLower == _appNamespace.ToLower() + ".dataaccess" ||
                            folderLower == _appNamespace.ToLower() + ".dataobjects" ||
                            folderLower == _appNamespace.ToLower() + ".efmodels" ||
                            folderLower == _appNamespace.ToLower() + ".plugins"
                        ) {
                            // These are the built-in folders
                        } else {
                            manuallyMigrateFolders.Add(folderName);
                        }
                    }
                }

                if (manuallyMigrateFolders.Any()) {
                    Utils.ConsoleLog("");
                    Utils.ConsoleLog("Additional Folders");
                    Utils.ConsoleLog("The following folders were copied from your old solution and an attempt was made to ");
                    Utils.ConsoleLog("add any references to other projects in these folders. ");
                    Utils.ConsoleLog("You will still need to attempt to build to make sure there aren't additional references that need to be added manually.");

                    html.AppendLine("<h2>Additional Folders</h2>");
                    html.AppendLine("<p>");
                    html.AppendLine("  The following folders were copied from your old solution and an attempt was made to ");
                    html.AppendLine("  add any references to other projects in these folders. ");
                    html.AppendLine("  You will still need to attempt to build to make sure there aren't additional references that need to be added manually.");
                    html.AppendLine("</p>");
                    html.AppendLine("<ul>");

                    foreach (var item in manuallyMigrateFolders.Index()) {
                        Utils.ConsoleLog(" - " + item.Item);

                        html.AppendLine("<li>" + item.Item + "</li>");

                        var f = System.IO.Path.Combine(pathToOldApp, item.Item);
                        var t = System.IO.Path.Combine(_filePath, item.Item);

                        CopyFolder(f, t);
                    }

                    html.AppendLine("</ul>");

                    // Find all base .csproj files in the old solution and see if there are missing project references
                    // in the target solution that need to be added and add them if they are missing.
                    var baseProjects = new List<string> {
                            System.IO.Path.Combine(_appNamespace, _appNamespace + ".csproj"),
                            System.IO.Path.Combine(_appNamespace + ".Client", _appNamespace + ".Client.csproj"),
                            System.IO.Path.Combine(_appNamespace + ".DataAccess", _appNamespace + ".DataAccess.csproj"),
                            System.IO.Path.Combine(_appNamespace + ".DataObjects", _appNamespace + ".DataObjects.csproj"),
                            System.IO.Path.Combine(_appNamespace + ".EFModels", _appNamespace + ".EFModels.csproj"),
                            System.IO.Path.Combine(_appNamespace + ".Plugins", _appNamespace + ".Plugins.csproj"),
                        };

                    List<string> addedMissingReferences = new List<string>();

                    foreach (var baseProject in baseProjects) {
                        var oldReferences = GetAllProjectReferencesInProject(System.IO.Path.Combine(pathToOldApp, baseProject));

                        if (oldReferences.Any()) {
                            var targetReferences = GetAllProjectReferencesInProject(System.IO.Path.Combine(_filePath, baseProject));

                            // Find any oldReferences that are not in the targetReferences and add them to the target project file.
                            var missingReferenences = oldReferences.Where(r => !targetReferences.Any(tr => tr.Equals(r, StringComparison.InvariantCultureIgnoreCase)));
                            if (missingReferenences != null && missingReferenences.Any()) {
                                foreach (var missingReference in missingReferenences) {
                                    AddMissingProjectReferenceToCsprojFile(System.IO.Path.Combine(_filePath, baseProject), missingReference);
                                    addedMissingReferences.Add("Added Reference '" + missingReference + "' to '" + baseProject + "'");
                                }
                            }
                        }
                    }

                    // Update projects in the base solution file. (only works for .slnx)
                    if (!String.IsNullOrWhiteSpace(oldSolutionExtension) && oldSolutionExtension.ToLower() == ".slnx") {
                        var oldSolutionReferences = GetAllProjectReferencesInSolution(oldSolutionFile);
                        if (oldSolutionReferences.Any()) {
                            var solutionReferences = GetAllProjectReferencesInSolution(solutionFile);

                            var missingSolutionProjectReferences = new List<ProjectReference>();

                            foreach (var item in oldSolutionReferences) {
                                if (!solutionReferences.Any(r => r.Path.Equals(item.Path, StringComparison.InvariantCultureIgnoreCase))) {
                                    missingSolutionProjectReferences.Add(item);
                                }
                            }

                            if (missingSolutionProjectReferences.Any()) {
                                AddMissingProjectReferencesToSolutionFile(solutionFile, missingSolutionProjectReferences);
                            }
                        }
                    }

                    if (addedMissingReferences.Any()) {
                        Utils.ConsoleLog("");
                        Utils.ConsoleLog("Added Missing References");
                        Utils.ConsoleLog("The following missing references were added:");

                        html.AppendLine("<h2>Added Missing References</h2>");
                        html.AppendLine("<p>The following missing references were added:</p>");
                        html.AppendLine("<ul>");

                        foreach (var addedReference in addedMissingReferences) {
                            Utils.ConsoleLog(" - " + addedReference);

                            html.AppendLine("<li>" + addedReference + "</li>");
                        }

                        html.AppendLine("</ul>");
                    }
                }
            }
        }

        if (copyAppFiles) {
            // Find all files below the appPath directory that contain ".App." in the file name.
            var files = Directory
                .GetFiles(pathToOldApp, "*.*", SearchOption.AllDirectories)
                .Where(f => f.ToLower().Contains(".app."));

            if (files.Any()) {
                Utils.ConsoleLog("");
                Utils.ConsoleLog("Copying .app. files from '" + pathToOldApp + "' to '" + _filePath + "'...");

                html.AppendLine("<h2>Copied .app. Files</h2>");
                html.AppendLine("<ul>");

                foreach (var file in files) {
                    var copyFile = CopyFile(file, pathToOldApp, _filePath);
                    Utils.ConsoleLog(" - " + copyFile);

                    html.AppendLine("<li>" + copyFile + "</li>");
                }

                html.AppendLine("</ul>");
            }

            var appPagesPath = System.IO.Path.Combine(pathToOldApp, _appNamespace + @".Client\Pages\App");
            if (System.IO.Directory.Exists(appPagesPath)) {
                var appPages = Directory
                    .GetFiles(appPagesPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => System.IO.Path.GetFileName(f).ToLower() != "info.txt");

                if (appPages != null && appPages.Any()) {
                    Utils.ConsoleLog("");
                    Utils.ConsoleLog("Copying application pages from the Pages\\App Folder");

                    html.AppendLine("<h2>Copied Application Pages</h2>");
                    html.AppendLine("<ul>");

                    foreach (var file in appPages) {
                        var copyFile = CopyFile(file, pathToOldApp, _filePath);
                        Utils.ConsoleLog(" - " + copyFile);

                        html.AppendLine("<li>" + copyFile + "</li>");
                    }

                    html.AppendLine("</ul>");
                }
            }
        }

        if (copyEntityFrameworkFiles) {
            // Copy the Entity Framework files.
            // Get all folders in the old path that start with EFModels
            var efFoldersPath = System.IO.Path.Combine(pathToOldApp, _appNamespace + ".EFModels");

            var efFolders = System.IO.Directory.GetDirectories(efFoldersPath);
            if (efFolders != null && efFolders.Any()) {
                foreach (var efFolder in efFolders) {
                    var f = efFolder.Substring(efFoldersPath.Length + 1);
                    if (f.ToLower().StartsWith("efmodels")) {
                        //var efPath = System.IO.Path.Combine(pathToOldApp, appNamespace + @".EFModels\EFModels");
                        var efPath = efFolder;

                        if (System.IO.Directory.Exists(efPath)) {
                            var efFiles = Directory.GetFiles(efPath, "*.cs", SearchOption.AllDirectories);

                            if (efFiles.Any()) {
                                var existingEfPath = System.IO.Path.Combine(_filePath, _appNamespace + ".EFModels\\" + f);

                                if (System.IO.Directory.Exists(existingEfPath)) {
                                    var existingEfFiles = Directory.GetFiles(existingEfPath, "*.cs", SearchOption.AllDirectories);

                                    if (existingEfFiles.Any()) {
                                        Utils.ConsoleLog("");
                                        Utils.ConsoleLog("Deleting Existing EF Model Files from '" + f + "'");

                                        html.AppendLine("<h2>Deleted Existing EF Model Files from '" + f + "'</h2>");
                                        html.AppendLine("<ul>");

                                        foreach (var file in existingEfFiles) {
                                            System.IO.File.Delete(file);
                                            Utils.ConsoleLog(" - " + file);

                                            html.AppendLine("<li>" + file + "</li>");
                                        }

                                        html.AppendLine("</ul>");
                                    }
                                }

                                Utils.ConsoleLog("");
                                Utils.ConsoleLog("Copy Entity Framework Models to '" + f + "'");

                                html.AppendLine("<h2>Copied Entity Framework Models to '" + f + "'</h2>");
                                html.AppendLine("<ul>");

                                foreach (var file in efFiles) {
                                    var copyFile = CopyFile(file, pathToOldApp, _filePath);
                                    Utils.ConsoleLog(" - " + copyFile);

                                    html.AppendLine("<li>" + copyFile + "</li>");
                                }

                                html.AppendLine("</ul>");
                            }
                        }
                    }
                }
            }
        }

        if (copyEnums) {
            // Check all files in the DataObjects project for enums to see if any custom enum properties need to be copied into the target.
            var dataObjectsProject = System.IO.Path.Combine(pathToOldApp, _appNamespace + ".DataObjects");
            var doFiles = Directory.GetFiles(dataObjectsProject, "*.cs", SearchOption.TopDirectoryOnly);

            var updatedEnumsHtml = new System.Text.StringBuilder();
            bool updatedEnums = false;

            foreach (var file in doFiles) {
                var doFile = System.IO.File.ReadAllText(file);
                if (doFile.Contains("enum")) {
                    var enums = GetEnums(file, doFile);

                    var targetFilePath = System.IO.Path.Combine(_filePath, file.Substring(pathToOldApp.Length + 1));

                    if (System.IO.File.Exists(targetFilePath)) {
                        var targetEnums = GetEnums(targetFilePath, System.IO.File.ReadAllText(targetFilePath));

                        // Find any items that are not in the targets.
                        var updateEnums = new Enums { Filename = targetFilePath };

                        foreach (var enumItem in enums.EnumValues) {
                            var targetEnumItem = targetEnums.EnumValues.FirstOrDefault(x => x.Name == enumItem.Name);
                            if (targetEnumItem != null) {
                                var missingProps = new List<string>();

                                foreach (var prop in enumItem.Properties) {
                                    if (!targetEnumItem.Properties.Any(x => x == prop)) {
                                        missingProps.Add(prop);
                                    }
                                }

                                if (missingProps.Any()) {
                                    //missingProps.Dump("Target File '" + targetFilePath + "' enum '" + enumItem.Name + "' Missing Properties");

                                    //updatedProps.OrderBy(x => x).Dump("Updated Properties");
                                    var updatedProps = targetEnumItem.Properties.ToList();
                                    updatedProps.AddRange(missingProps);
                                    updatedProps = updatedProps.OrderBy(x => x).ToList();

                                    //updatedProps.Dump("Updated Properties for Enum '" + enumItem.Name + "' in '" + targetFilePath + "'");
                                    updateEnums.EnumValues.Add(new EnumValues {
                                        Name = enumItem.Name,
                                        Properties = updatedProps,
                                    });
                                }
                            }
                        }

                        if (updateEnums.EnumValues.Any()) {
                            updatedEnums = true;

                            var updatedContent = UpdateEnumInFile(updateEnums);

                            Utils.ConsoleLog("");
                            Utils.ConsoleLog("Updated enums in '" + targetFilePath + "'");

                            updatedEnumsHtml.AppendLine("<h3>" + targetFilePath + "</h3>");
                            updatedEnumsHtml.AppendLine("<ul>");

                            foreach (var enumValue in updateEnums.EnumValues) {
                                Utils.ConsoleLog(" - " + enumValue.Name);

                                updatedEnumsHtml.AppendLine("<li>" + enumValue.Name + "</li>");
                            }

                            updatedEnumsHtml.AppendLine("</ul>");
                        }
                    }
                }
            }

            if (updatedEnums) {
                html.AppendLine("<h2>Updated Enums</h2>");
                html.AppendLine(updatedEnumsHtml.ToString());
            }
        }

        if (updateNugetPackages) {
            var projectFiles = Directory
                .GetFiles(pathToOldApp, "*.*", SearchOption.AllDirectories)
                .Where(f => f.ToLower().EndsWith(".csproj"));

            foreach (var projectFile in projectFiles) {
                var packageInfo = GetNugetPackagesFromProjectFile(projectFile);
                var fileName = projectFile.Substring(pathToOldApp.Length + 1);
                var destinationFileName = System.IO.Path.Combine(_filePath, fileName);

                if (System.IO.File.Exists(destinationFileName)) {
                    var targetPackages = GetNugetPackagesFromProjectFile(destinationFileName);

                    var missingPackages = new List<NugetPackageDetails>();

                    foreach (var package in packageInfo.Packages) {
                        if (!targetPackages.Packages.Any(x => x.Name.ToLower() == package.Name.ToLower())) {
                            missingPackages.Add(package);
                        }
                    }

                    if (missingPackages.Any()) {
                        //missingPackages.Dump("Missing Nuget Packages in '" + destinationFileName + "' - Add After Line " + targetPackages.LastProjectFilePackageLine);

                        var destinationFileContents = System.IO.File.ReadAllText(destinationFileName);
                        var updatedDetinationFileContents = new System.Text.StringBuilder();

                        int index = 0;
                        foreach (var line in destinationFileContents.Split(new[] { '\r', '\n' })) {
                            index++;

                            updatedDetinationFileContents.AppendLine(line);

                            if (index == targetPackages.LastProjectFilePackageLine) {
                                foreach (var package in missingPackages) {
                                    updatedDetinationFileContents.AppendLine("    <PackageReference Include=\"" + package.Name + "\" Version=\"" + package.Version + "\" />");
                                }
                            }
                        }

                        Utils.ConsoleLog("");
                        Utils.ConsoleLog("Updated nuget packages in '" + destinationFileName + "'");

                        foreach (var p in missingPackages) {
                            Utils.ConsoleLog(" - Added Package " + p.Name);
                        }

                        System.IO.File.WriteAllText(destinationFileName, updatedDetinationFileContents.ToString());
                    }
                }
            }
        }

        if (checkForMissingCode) {
            var requiredElements = GetRequiredElements();

            var missingElements = new List<RequiredElement>();
            var missingEmptyFiles = new List<RequiredElement>();
            var missingFiles = new List<RequiredElement>();

            foreach (var requiredElement in requiredElements) {
                var relativePath = requiredElement.RelativePath.Replace("CRM", _appNamespace);
                var checkFile = System.IO.Path.Combine(_filePath, relativePath);

                if (System.IO.File.Exists(checkFile)) {
                    var missingItems = new List<RequiredElementItem>();
                    var fileContent = System.IO.File.ReadAllText(checkFile);

                    if (!String.IsNullOrWhiteSpace(fileContent)) {
                        foreach (var item in requiredElement.Items) {
                            bool checkItem = true;

                            if (!String.IsNullOrEmpty(item.Module)) {
                                if (removedModules.Contains(item.Module)) {
                                    checkItem = false;
                                }
                            }

                            if (checkItem) {
                                if (!fileContent.Contains(item.Item)) {
                                    missingItems.Add(item);
                                }
                            }
                        }

                        if (missingItems.Any()) {
                            missingElements.Add(new RequiredElement {
                                RelativePath = relativePath,
                                Items = missingItems,
                                Module = requiredElement.Module,
                            });
                        }
                    } else {
                        bool reportOnFile = true;

                        if (!String.IsNullOrEmpty(requiredElement.Module)) {
                            if (removedModules.Contains(requiredElement.Module)) {
                                reportOnFile = false;
                            }
                        }

                        if (reportOnFile) {
                            missingEmptyFiles.Add(requiredElement);
                        }
                    }
                } else {
                    bool reportOnMissingFile = true;

                    if (!String.IsNullOrEmpty(requiredElement.Module)) {
                        if (removedModules.Contains(requiredElement.Module)) {
                            reportOnMissingFile = false;
                        }
                    }

                    if (reportOnMissingFile) {
                        missingFiles.Add(requiredElement);
                    }
                }
            }

            if (missingEmptyFiles.Any() || missingFiles.Any() || missingElements.Any()) {
                Utils.ConsoleLog("");
                Utils.ConsoleLog("Checking for missing code elements that may need to be added.");
                Utils.ConsoleLog("An attempt will be made to manually insert any missing code.");

                if (missingEmptyFiles.Any()) {
                    html.AppendLine("<h2>Empty Files</h2>");
                    html.AppendLine("<p>");
                    html.AppendLine("  The following files existed but were empty and therefore could not be evaluated.");
                    html.AppendLine("  These files were updated with the default code for the file.");
                    html.AppendLine("</p>");
                    html.AppendLine("<ul>");

                    Utils.ConsoleLog("");
                    Utils.ConsoleLog("Empty Files:");

                    foreach (var item in missingEmptyFiles) {
                        string module = !String.IsNullOrEmpty(item.Module) ? "<span class=\"module\">[" + item.Module + "]</span>" : "";
                        string moduleClean = !String.IsNullOrEmpty(item.Module) ? " [MODULE: " + item.Module + "]" : "";

                        html.AppendLine("<li>" + System.IO.Path.Combine(_filePath, item.RelativePath) + module + "</li>");

                        Utils.ConsoleLog(" - " + System.IO.Path.Combine(_filePath, item.RelativePath) + moduleClean);

                        // Download the source file from git, rename the namespace, and write out the contents.
                        var missingFileSource = SourceCode.GetSourceFile(item.RelativePath);
                        if (!String.IsNullOrWhiteSpace(missingFileSource)) {
                            var updateFileContents =
                                "// " + SourceCode.BeginNote + Environment.NewLine + Environment.NewLine +
                                missingFileSource.Replace("CRM", _appNamespace) +
                                Environment.NewLine + Environment.NewLine + "// " + SourceCode.EndNote;

                            System.IO.File.WriteAllText(System.IO.Path.Combine(_filePath, item.RelativePath), updateFileContents);
                        }
                    }

                    html.AppendLine("</ul>");
                }

                if (missingFiles.Any()) {
                    html.AppendLine("<h2>Missing Files</h2>");
                    html.AppendLine("<p>The following files do not exist and therefore could not be evaluated.</p>");
                    html.AppendLine("<ul>");

                    Utils.ConsoleLog("");
                    Utils.ConsoleLog("Missing Files:");

                    foreach (var item in missingFiles) {
                        string module = !String.IsNullOrEmpty(item.Module) ? "<span class=\"module\">[" + item.Module + "]</span>" : "";
                        string moduleClean = !String.IsNullOrEmpty(item.Module) ? " [MODULE: " + item.Module + "]" : "";

                        html.AppendLine("<li>" + System.IO.Path.Combine(_filePath, item.RelativePath) + module + "</li>");

                        Utils.ConsoleLog(" - " + System.IO.Path.Combine(_filePath, item.RelativePath) + moduleClean);

                        // Download the source file from git, rename the namespace, and write out the contents.
                        var missingFileSource = SourceCode.GetSourceFile(item.RelativePath);
                        if (!String.IsNullOrWhiteSpace(missingFileSource)) {
                            var updateFileContents =
                                "// " + SourceCode.BeginNote + Environment.NewLine + Environment.NewLine +
                                missingFileSource.Replace("CRM", _appNamespace) +
                                Environment.NewLine + Environment.NewLine + "// " + SourceCode.EndNote;

                            System.IO.File.WriteAllText(System.IO.Path.Combine(_filePath, item.RelativePath), updateFileContents);
                        }
                    }

                    html.AppendLine("</ul>");
                }

                if (missingElements.Any()) {
                    html.AppendLine("<h2>Missing Elements</h2>");

                    Utils.ConsoleLog("");
                    Utils.ConsoleLog("Missing Elements:");

                    int updatedItems = 0;

                    foreach (var item in missingElements) {
                        // Local variables that are used to store source code for this file
                        // so they only have to be loaded one time per item.
                        List<string> atBlock = new List<string>();
                        List<string> atCodeBlock = new List<string>();
                        List<string> classBlock = new List<string>();

                        bool updated = false;

                        string module = !String.IsNullOrEmpty(item.Module) ? "<span class=\"module\">[" + item.Module + "]</span>" : "";
                        string moduleClean = !String.IsNullOrEmpty(item.Module) ? " [MODULE: " + item.Module + "]" : "";

                        html.AppendLine("<p>" + item.RelativePath + module + "</p>");
                        html.AppendLine("<ul>");

                        Utils.ConsoleLog("");
                        Utils.ConsoleLog(" - " + item.RelativePath + moduleClean);

                        foreach (var element in item.Items.OrderBy(x => x.SortOrder)) {
                            module = !String.IsNullOrEmpty(element.Module) ? "<span class=\"module\">[" + element.Module + "]</span>" : "";
                            moduleClean = !String.IsNullOrEmpty(element.Module) ? " [MODULE: " + element.Module + "]" : "";

                            html.AppendLine("<li>" + element.Item + module + "</li>");

                            Utils.ConsoleLog("   - " + element.Item + moduleClean);
                        }
                        html.AppendLine("</ul>");

                        var targetContent = System.IO.File.ReadAllText(System.IO.Path.Combine(_filePath, item.RelativePath));

                        // Download the source file from git and rename the namespace.
                        var itemPath = item.RelativePath
                            .Replace(_appNamespace, "CRM")
                            .Replace("\\", "/");

                        var gitUrl = "https://raw.githubusercontent.com/wicketbr/FreeCRM/refs/heads/main/" + itemPath;
                        string sourceFile = SourceCode.GetSourceFile(gitUrl);

                        if (!String.IsNullOrWhiteSpace(sourceFile)) {
                            sourceFile = sourceFile.Replace("CRM", _appNamespace);
                            List<string> sourceLines = SourceCode.SplitTextIntoLines(sourceFile);

                            //sourceLines.Dump("Source Lines");

                            // For the modification of the actual files sort descending as each item gets prepended in order.
                            // This will maintain the correct resulting sorting.
                            foreach (var element in item.Items.OrderByDescending(x => x.SortOrder)) {
                                List<string>? attributes = null;
                                List<string>? comments = null;

                                int elementIndex = -1;

                                foreach (var line in sourceLines.Index()) {
                                    if (line.Item.Trim().ToLower().StartsWith(element.Item.Trim().ToLower())) {
                                        elementIndex = line.Index;
                                        break;
                                    }
                                }

                                switch (element.Target.ToLower()) {
                                    case "blazorparameter":
                                        if (!String.IsNullOrWhiteSpace(element.Parent)) {
                                            switch (element.Parent.ToLower()) {
                                                case "@code{":
                                                    if (!atCodeBlock.Any()) {
                                                        atCodeBlock = SourceCode.GetAtCodeBlock(sourceLines);
                                                    }

                                                    var blazorParameterCodeLine = atCodeBlock.FirstOrDefault(x => x.Trim().ToLower().StartsWith(element.Item.Trim().ToLower()));
                                                    if (blazorParameterCodeLine != null) {
                                                        comments = SourceCode.GetCommentsAboveLine(atCodeBlock.IndexOf(blazorParameterCodeLine), atCodeBlock);
                                                        targetContent = SourceCode.UpdateCode_InsertIntoAtCodeBlock(targetContent, blazorParameterCodeLine, comments);
                                                        updated = true;
                                                        updatedItems++;
                                                    }
                                                    break;
                                            }
                                        }

                                        break;

                                    case "codeblockline":
                                        if (!String.IsNullOrWhiteSpace(element.Parent)) {
                                            switch (element.Parent.ToLower()) {
                                                case "@{":
                                                    if (!atBlock.Any()) {
                                                        atBlock = SourceCode.GetAtBlock(sourceLines);
                                                    }

                                                    var atBlockLine = atBlock.FirstOrDefault(l => l.Trim().ToLower().StartsWith(element.Item.Trim().ToLower()));
                                                    if (atBlockLine != null) {
                                                        comments = SourceCode.GetCommentsAboveLine(atCodeBlock.IndexOf(atBlockLine), atCodeBlock);
                                                        targetContent = SourceCode.UpdateCode_InsertIntoAtBlock(targetContent, atBlockLine, comments);
                                                        updated = true;
                                                        updatedItems++;
                                                    }
                                                    break;

                                                case "@code{":
                                                    if (!atCodeBlock.Any()) {
                                                        atCodeBlock = SourceCode.GetAtCodeBlock(sourceLines);
                                                    }

                                                    var atCodeLine = atCodeBlock.FirstOrDefault(x => x.Trim().ToLower().StartsWith(element.Item.Trim().ToLower()));
                                                    if (atCodeLine != null) {
                                                        comments = SourceCode.GetCommentsAboveLine(atCodeBlock.IndexOf(atCodeLine), atCodeBlock);
                                                        targetContent = SourceCode.UpdateCode_InsertIntoAtCodeBlock(targetContent, atCodeLine, comments);
                                                        updated = true;
                                                        updatedItems++;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;

                                    case "partialclassdefinition":
                                        var partialClassDefinition = SourceCode.GetCodeBlock(element.Item, sourceLines);
                                        if (partialClassDefinition != null && partialClassDefinition.Any()) {
                                            targetContent += Environment.NewLine + "// " + SourceCode.BeginNote + Environment.NewLine;

                                            if (attributes != null && attributes.Any()) {
                                                foreach (var a in attributes) {
                                                    targetContent += a + Environment.NewLine;
                                                }
                                            }

                                            if (comments != null && comments.Any()) {
                                                foreach (var c in comments) {
                                                    targetContent += c + Environment.NewLine;
                                                }
                                            }

                                            foreach (var l in partialClassDefinition) {
                                                targetContent += l + Environment.NewLine;
                                            }

                                            targetContent += "// " + SourceCode.EndNote;

                                            updated = true;
                                            updatedItems++;
                                        }
                                        break;

                                    case "partialclassproperty":
                                        var partialClassProperty = sourceLines.FirstOrDefault(x => x.Trim().ToLower().StartsWith(element.Item.Trim().ToLower()));
                                        if (!String.IsNullOrWhiteSpace(partialClassProperty)) {
                                            targetContent = SourceCode.UpdateCode_InsertIntoElement(element.Parent, targetContent, partialClassProperty);
                                            updated = true;
                                            updatedItems++;
                                        }
                                        break;

                                    case "partialclassmethod":
                                        var partialClassMethod = SourceCode.GetCodeBlock(element.Item, sourceLines);
                                        if (partialClassMethod.Any()) {
                                            switch ((String.Empty + element.Parent).ToLower()) {
                                                case "@code{":
                                                    targetContent = SourceCode.UpdateCode_InsertIntoAtCodeBlock(targetContent, partialClassMethod, comments);
                                                    updated = true;
                                                    updatedItems++;
                                                    break;

                                                default:
                                                    targetContent = SourceCode.UpdateCode_InsertIntoElement(element.Parent, targetContent, partialClassMethod, attributes, comments);
                                                    updated = true;
                                                    updatedItems++;
                                                    break;
                                            }
                                        }

                                        break;

                                    case "partialinterfacedefinition":
                                        var partialInterfaceDefinition = SourceCode.GetCodeBlock(element.Item, sourceLines);
                                        if (partialInterfaceDefinition != null && partialInterfaceDefinition.Any()) {
                                            targetContent += Environment.NewLine + "// " + SourceCode.BeginNote + Environment.NewLine;

                                            if (comments != null && comments.Any()) {
                                                foreach (var c in comments) {
                                                    targetContent += c + Environment.NewLine;
                                                }
                                            }

                                            foreach (var l in partialInterfaceDefinition) {
                                                targetContent += l + Environment.NewLine;
                                            }

                                            targetContent += "// " + SourceCode.EndNote;

                                            updated = true;
                                            updatedItems++;
                                        }
                                        break;

                                    case "partialinterfaceitem":
                                        var partialInterfaceItem = sourceLines.FirstOrDefault(x => x.Trim().ToLower().StartsWith(element.Item.Trim().ToLower()));
                                        if (!String.IsNullOrWhiteSpace(partialInterfaceItem)) {
                                            targetContent = SourceCode.UpdateCode_InsertIntoElement(element.Parent, targetContent, partialInterfaceItem);
                                            updated = true;
                                            updatedItems++;
                                        }
                                        break;

                                    case "rootitem":
                                        var rootItem = SourceCode.GetCodeBlock(element.Item, sourceLines);
                                        if (rootItem != null && rootItem.Any()) {
                                            targetContent += Environment.NewLine + "// " + SourceCode.BeginNote + Environment.NewLine;

                                            if (attributes != null && attributes.Any()) {
                                                foreach (var a in attributes) {
                                                    targetContent += a + Environment.NewLine;
                                                }
                                            }

                                            if (comments != null && comments.Any()) {
                                                foreach (var c in comments) {
                                                    targetContent += c + Environment.NewLine;
                                                }
                                            }

                                            foreach (var l in rootItem) {
                                                targetContent += l + Environment.NewLine;
                                            }

                                            targetContent += "// " + SourceCode.EndNote + Environment.NewLine;

                                            updated = true;
                                            updatedItems++;
                                        }
                                        break;

                                    default:
                                        Utils.ConsoleLog("Unknown Code Target '" + element.Target + "' for Item '" + element.Item + "'");
                                        html.AppendLine("<p>Unknown Code Target '" + element.Target + "' for Item '" + element.Item + "'</p>");
                                        break;
                                }
                            }

                            if (updated) {
                                System.IO.File.WriteAllText(System.IO.Path.Combine(_filePath, item.RelativePath), targetContent);
                            }
                        }
                    }

                    Utils.ConsoleLog("");
                    if (updatedItems > 0) {
                        Utils.ConsoleLog("Updated " + updatedItems + " elements.");
                        Utils.ConsoleLog("");
                        Utils.ConsoleLog("Updated code has been wrapped in comments as:");
                        Utils.ConsoleLog("// " + SourceCode.BeginNote);
                        Utils.ConsoleLog("... code here");
                        Utils.ConsoleLog("// " + SourceCode.EndNote);

                        html.AppendLine("<h2>Updated " + updatedItems + " elements.</h2>");
                        html.AppendLine("<p>");
                        html.AppendLine("  Updated code has been wrapped in comments as:");
                        html.AppendLine("</p>");
                        html.AppendLine("<code>");
                        html.AppendLine("  <div class=\"comment\">// " + SourceCode.BeginNote + "</div>");
                        html.AppendLine("  <div>... code here</div>");
                        html.AppendLine("  <div class=\"comment\">// " + SourceCode.EndNote + "</div>");
                        html.AppendLine("</code>");
                    } else {
                        Utils.ConsoleLog("No elements were updated.");
                        html.AppendLine("<h2>No elements were updated.</h2>");
                    }
                }
            }
        }

        if (checkLanguageItems) {
            // Find any missing language items.
            html.AppendLine("<h2>Language Items Details</h2>");
            var missingLanguageItems = SourceCode.FindMissingLanguageItems(pathToOldApp, _filePath);
            if (missingLanguageItems.Any()) {
                string missingLanguageItemsCount = missingLanguageItems.Count.ToString();
                bool moreThanOne = missingLanguageItems.Count > 1;

                Utils.ConsoleLog(missingLanguageItemsCount + " Missing Language Items");
                html.AppendLine("<h3>" + missingLanguageItemsCount + " Missing Language Item" + (moreThanOne ? "s" : "") + "</h3>");
                html.AppendLine("<p>");
                html.AppendLine("  The following language item" + (moreThanOne ? "s were" : " was") + " found in the old application");
                html.AppendLine("  but have not been added to the new application. For your convenience");
                html.AppendLine("  the item" + (moreThanOne ? "s are" : " is") + " listed below in the format required and can be copied");
                html.AppendLine("  and pasted into your AppLanguage area in the DataAccess.App.cs file.");
                html.AppendLine("</p>");

                var lastSection = String.Empty;
                bool firstSection = true;

                foreach (var item in missingLanguageItems) {
                    if (!String.IsNullOrWhiteSpace(item.Section) && item.Section.ToLower() != lastSection) {
                        Utils.ConsoleLog();
                        Utils.ConsoleLog(item.Section);

                        if (!firstSection) {
                            html.AppendLine("<br />");
                        }

                        html.AppendLine("<div class=\"comment\">" + item.Section + "</div>");

                        lastSection = item.Section.ToLower();
                        firstSection = false;
                    }

                    Utils.ConsoleLog("{ \"" + item.Tag + "\" , \"" + item.Value + "\" },");
                    html.AppendLine("<div>{ \"" + item.Tag + "\" , \"" + System.Web.HttpUtility.HtmlEncode(item.Value) + "\" },</div>");
                }
            } else {
                Utils.ConsoleLog("No Missing Language Items Found");
                Utils.ConsoleLog();
                html.AppendLine("<h3>No Missing Language Items Found</h3>");
            }

            // Find any language items that are no longer in use.
            // Get the entire source code without the language tag definitions.
            var newLanguageItems = SourceCode.GetLanguageItems(_filePath);
            var sourceCode = SourceCode.GetAllSourceCode(_filePath, true);

            if (!String.IsNullOrEmpty(sourceCode)) {
                // Find all language items in the source code. They will either be inside GetLanguageItem("TAG", Text("TAG", or <Language Tag="TAG"
                // So find the tag wrapped in quotes.
                List<string> languageItemsInSource = new List<string>();
                foreach (var item in newLanguageItems) {
                    if (sourceCode.Contains("\"" + item.Tag + "\"")) {
                        languageItemsInSource.Add(item.Tag);
                    }
                }

                List<string> tagsNoLongerInUse = new List<string>();
                foreach (var item in newLanguageItems) {
                    if (!languageItemsInSource.Any(x => x.ToLower() == item.Tag.ToLower())) {
                        tagsNoLongerInUse.Add(item.Tag);
                    }
                }
                if (tagsNoLongerInUse.Any()) {
                    Utils.ConsoleLog("The following tags are defined in your language settings, but are no longer in use:");
                    Utils.ConsoleLog();
                    html.AppendLine("<h3>Unused Language Items</h3>");
                    html.AppendLine("<p>The following tags are defined in your language settings, but are no longer in use:</p>");
                    html.AppendLine("<ul>");
                    foreach (var tag in tagsNoLongerInUse.OrderBy(x => x)) {
                        Utils.ConsoleLog(tag);
                        html.AppendLine("<li>" + tag + "</li>");
                    }
                    Utils.ConsoleLog();
                    html.AppendLine("</ul>");
                }

                // Find tags in the source code that are not defined in language settings.
                // Create a regex to find all items inside GetLanguageItem("TAG" and return the value inside the quotes.


                List<string> allTagsFoundInSource = new List<string>();

                List<string> tagsInGetLanguageItem = Regex.Matches(sourceCode, @"GetLanguageItem\(""([^""]+)""")
                    .Cast<Match>()
                    .Select(m => m.Groups[1].Value)
                    .ToList();

                foreach (var tag in tagsInGetLanguageItem) {
                    if (!allTagsFoundInSource.Any(x => x.ToLower() == tag.ToLower())) {
                        allTagsFoundInSource.Add(tag);
                    }
                }

                List<string> tagsInHelpersText = Regex.Matches(sourceCode, @"Text\(""([^""]+)""")
                    .Cast<Match>()
                    .Select(m => m.Groups[1].Value)
                    .ToList();

                foreach (var tag in tagsInHelpersText) {
                    if (!allTagsFoundInSource.Any(x => x.ToLower() == tag.ToLower())) {
                        allTagsFoundInSource.Add(tag);
                    }
                }

                List<string> tagsInLanguageComponent = Regex.Matches(sourceCode, @"<Language\s+Tag=""([^""]+)""")
                    .Cast<Match>()
                    .Select(m => m.Groups[1].Value)
                    .ToList();

                foreach (var tag in tagsInLanguageComponent) {
                    if (!allTagsFoundInSource.Any(x => x.ToLower() == tag.ToLower())) {
                        allTagsFoundInSource.Add(tag);
                    }
                }

                if (allTagsFoundInSource.Any()) {
                    allTagsFoundInSource = allTagsFoundInSource.OrderBy(x => x).ToList();

                    // Find any tags found in the source that aren't in the newLanguageItems collection
                    List<string> tagsNotInLanguageItems = new List<string>();
                    foreach (var tag in allTagsFoundInSource) {
                        if (!newLanguageItems.Any(x => x.Tag.ToLower() == tag.ToLower())) {
                            // Exlude items that start with @ or end with _, as they are likely used in a compute to generate a dynamic tag.
                            if (!tag.StartsWith("@") && !tag.EndsWith("_")) {
                                tagsNotInLanguageItems.Add(tag);
                            }
                        }
                    }

                    if (tagsNotInLanguageItems.Any()) {
                        var tagsNotInLanguageMessage = "The following language items were found in use in the source code, but are not defined in your language settings. " +
                            "Some of these could be false-positives where an item was being built dynamically and the regex finding these tags " +
                            "did not interpret the tag inside the quotes correctly. Use this list as a reference to determine if you need to " +
                            "add any missing language tags.";

                        Utils.ConsoleLog(tagsNotInLanguageMessage);
                        Utils.ConsoleLog();
                        html.AppendLine("<h3>Undefined Language Items</h3>");
                        html.AppendLine("<p>" + tagsNotInLanguageMessage + "</p>");
                        html.AppendLine("<ul>");

                        foreach (var tag in tagsNotInLanguageItems.OrderBy(x => x)) {
                            Utils.ConsoleLog(tag);
                            html.AppendLine("<li>" + tag + "</li>");
                        }
                        Utils.ConsoleLog();
                        html.AppendLine("</ul>");
                    }
                }
            }
        }

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        var reportFile = System.IO.Path.Combine(_filePath, "UpgradeReport.html");

        System.IO.File.WriteAllText(reportFile, html.ToString());
        Utils.ConsoleLog("");
        Utils.ConsoleLog("Upgrade report generated at '" + reportFile + "'");

        if (!fromProgramStart) {
            FileOpener.OpenInBrowser(reportFile);

            Utils.ConsoleLog();
            Utils.ConsoleLog("Press Any Key to Return to the Main Menu");
            Console.ReadKey();

            Utils.HomeScreen();
        }
    }

    public static void AddMissingProjectReferencesToSolutionFile(string solutionFile, List<ProjectReference> projectReferences)
    {
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

    public static void AddMissingProjectReferenceToCsprojFile(string PathToProjectFile, string ProjectReference)
    {
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

    public static string CopyFile(string file, string appPath, string targetPath)
    {
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

    static public void CopyFolder(string sourceFolder, string destFolder)
    {
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

    public static List<string> GetAllProjectReferencesInProject(string PathToProjectFile)
    {
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

    public static List<ProjectReference> GetAllProjectReferencesInSolution(string PathToSolutionFile)
    {
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

    public static Enums GetEnums(string fileName, string fileContents)
    {
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

    public static ProjectNugetPackages GetNugetPackagesFromProjectFile(string projectFile)
    {
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

    public static string GetPackageReferenceValue(string line, string tag)
    {
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

    public static List<RequiredElement> GetRequiredElements() 
    {
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
                RelativePath = @"CRM.Client\Shared\AppComponents\PluginPrompts.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public Delegate? AppPromptUpdated { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<PluginExecuteResult> ButtonClickHandler { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public string? Class { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public string? DivClass { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool HighlightMissingRequiredFields { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public Plugin Plugin { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public PluginPrompt Prompt { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public List<PluginPromptValue> PromptValues { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public Delegate? OnValuesChange { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool OverrideComponent { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public PluginPromptType? RenderPromptType { get; set; }",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "protected string ElementId {",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public bool OverridePluginPromptType(PluginPromptType promptType)",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "protected void ValueChanged(ChangeEventArgs e, int index = 0)",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                },
            },
            new RequiredElement { 
                RelativePath = @"CRM.Client\Shared\AppComponents\Profile.App.razor",
                Items = new List<RequiredElementItem> {
                    new RequiredElementItem {
                        Item = "[Parameter] public string Area {",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public bool OverridePage {",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem { 
                        Item = "[Parameter] public bool ShowSaveButton {",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public DataObjects.User Value {",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "[Parameter] public EventCallback<DataObjects.User> ValueChanged {",
                        Target = "BlazorParameter",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "public DataObjects.ModuleAction Save(DataObjects.Tag tag)",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
                    new RequiredElementItem {
                        Item = "protected void ValueHasChanged()",
                        Target = "PartialClassMethod",
                        Parent = "@code{",
                    },
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
                    new RequiredElementItem {
                        Item = "public bool OverridePluginPromptType(PluginPromptType promptType)",
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
                        Item = "private object[]? ExecutePluginApp(PluginExecuteRequest request, DataObjects.User? CurrentUser = null)",
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

    public static bool IsGuid(string? value)
    {
        return Guid.TryParse(value, out _);
    }

    public static bool RestoreSolutionProjectGuids(string solutionFile, List<ProjectReference> projectReferences)
    {
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

    public static void SetFilePath(string filePath)
    {
        _filePath = filePath;
    }

    public static string UpdateEnumInContent(EnumValues item, string content)
    {
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

    public static string UpdateEnumInFile(Enums enumItem)
    {
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