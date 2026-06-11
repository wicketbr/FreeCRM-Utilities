namespace Util;

public static class RenameTools
{
    private static string _filePath = "";
    private static string _newAppName = "";

    public static bool AllowRename {
        get {
            var slnFile = System.IO.Directory.GetFiles(_filePath, "*.sln*").FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(slnFile)) {
                var slnFileName = System.IO.Path.GetFileNameWithoutExtension(slnFile);

                if (slnFileName.ToLower() != "crm") {
                    return false;
                }
            }

            return true;
        }
    }

    public static void RenameAllFileContents()
    {
        string[] files = Directory.GetFiles(_filePath, "*.*", SearchOption.AllDirectories);

        List<string> updateExtensions = new List<string> { ".cs", ".csproj", ".txt", ".json", ".cshtml", ".ts", ".razor", ".plugin" };
        List<string> blockPaths = new List<string> { @"\obj\", @"\bin\" };

        if (files.Any()) {
            foreach (var file in files) {
                string fileLower = file.ToLower();
                bool blockedPath = false;
                foreach (var item in blockPaths) {
                    if (!blockedPath && fileLower.Contains(item)) {
                        blockedPath = true;
                    }
                }

                if (!blockedPath) {
                    string ext = Path.GetExtension(fileLower);
                    if (!String.IsNullOrWhiteSpace(ext) && updateExtensions.Contains(ext)) {
                        string contents = File.ReadAllText(file);
                        string updated = String.Empty;

                        // Replace both freeCRM and CRM with the new app name. Do freeCRM first, as CRM would find the end of that string.
                        if (contents.Contains("crmHub") || contents.Contains("freeCRM") || contents.Contains("CRM")) {
                            updated = contents
                                .Replace("crmHub", _newAppName.ToLower() + "Hub")
                                .Replace("freeCRM", _newAppName)
                                .Replace("CRM", _newAppName);
                            File.WriteAllText(file, updated);
                        }
                    }
                }
            }
        }
    }

    public static void RenameFile(string currentName, string newName)
    {
        if (File.Exists(Path.Combine(_filePath, currentName))) {
            File.Move(Path.Combine(_filePath, currentName), Path.Combine(_filePath, newName));
        }
    }

    public static void RenameFolder(string currentFolder, string newFolder)
    {
        if (Directory.Exists(Path.Combine(_filePath, currentFolder))) {
            Directory.Move(Path.Combine(_filePath, currentFolder), Path.Combine(_filePath, newFolder));
        }
    }

    public static void RenameFoldersAndSolutionFiles()
    {
        RenameFile("CRM.sln", _newAppName + ".sln");
        RenameFile("CRM.slnx", _newAppName + ".slnx");

        RenameFolder("CRM", _newAppName);
        RenameFolder("CRM.Client", _newAppName + ".Client");
        RenameFolder("CRM.DataAccess", _newAppName + ".DataAccess");
        RenameFolder("CRM.DataObjects", _newAppName + ".DataObjects");
        RenameFolder("CRM.EFModels", _newAppName + ".EFModels");
        RenameFolder("CRM.Plugins", _newAppName + ".Plugins");

        RenameFile(_newAppName + "/CRM.csproj", _newAppName + "/" + _newAppName + ".csproj");
        RenameFile(_newAppName + ".Client/CRM.Client.csproj", _newAppName + ".Client/" + _newAppName + ".Client.csproj");
        RenameFile(_newAppName + ".DataAccess/CRM.DataAccess.csproj", _newAppName + ".DataAccess/" + _newAppName + ".DataAccess.csproj");
        RenameFile(_newAppName + ".DataObjects/CRM.DataObjects.csproj", _newAppName + ".DataObjects/" + _newAppName + ".DataObjects.csproj");
        RenameFile(_newAppName + ".EFModels/CRM.EFModels.csproj", _newAppName + ".EFModels/" + _newAppName + ".EFModels.csproj");
        RenameFile(_newAppName + ".Plugins/CRM.Plugins.csproj", _newAppName + ".Plugins/" + _newAppName + ".Plugins.csproj");
    }

    public static void SetAppName(string newAppName)
    {
        _newAppName = newAppName;
    }

    public static void SetFilePath(string filePath)
    {
        _filePath = filePath;
    }

    public static void UpdateSecretsGuid()
    {
        string file = Path.Combine(_filePath, _newAppName, _newAppName + ".csproj");
        if (File.Exists(file)) {
            string contents = File.ReadAllText(file);

            contents = contents.Replace("<UserSecretsId>c3a4acfd-bf26-4267-98c7-6746a2b80f10</UserSecretsId>", "<UserSecretsId>" + Guid.NewGuid().ToString() + "</UserSecretsId>");

            File.WriteAllText(file, contents);
        }
    }

    public static void UpdateSolutionFile()
    {
        string fileSLN = Path.Combine(_filePath, _newAppName + ".sln");
        if (File.Exists(fileSLN)) {
            string sln = File.ReadAllText(fileSLN);

            // Replace all the project GUIDs
            sln = sln.Replace("AC08C45F-3926-465F-B17A-09E5D26AA100", Guid.NewGuid().ToString().ToUpper()); // CRM
            sln = sln.Replace("DF8E0710-4710-4FA5-905B-0494E3E3051C", Guid.NewGuid().ToString().ToUpper()); // CRM.Client
            sln = sln.Replace("4CCAF2CF-4AAC-4E55-804E-54AD9F123B39", Guid.NewGuid().ToString().ToUpper()); // CRM.DataAccess
            sln = sln.Replace("3DF897AB-E5ED-42FD-869F-E071F1537C71", Guid.NewGuid().ToString().ToUpper()); // CRM.DataObjects
            sln = sln.Replace("6631DCE7-75DC-4044-936C-36D2F527380C", Guid.NewGuid().ToString().ToUpper()); // CRM.EFModels
            sln = sln.Replace("9C1DB5D4-22EA-4368-A5BC-04C7157D0665", Guid.NewGuid().ToString().ToUpper()); // Solution Items
            sln = sln.Replace("A3796CA0-CA85-46AF-A0DE-7641736C95B9", Guid.NewGuid().ToString().ToUpper()); // Solution Items
            sln = sln.Replace("9A19103F-16F7-4668-BE54-9A1E7A4F7556", Guid.NewGuid().ToString().ToUpper()); // Solution Items

            sln = sln.Replace("CRM", _newAppName);

            File.WriteAllText(fileSLN, sln);
        }

        string fileSLNX = Path.Combine(_filePath, _newAppName + ".slnx");
        if (File.Exists(fileSLNX)) {
            string sln = File.ReadAllText(fileSLNX);

            // Replace all the project GUIDs
            sln = sln.Replace("daf00842-3dab-4880-b902-61775be8dbcc", Guid.NewGuid().ToString().ToUpper()); // CRM
            sln = sln.Replace("6a6d9fb3-622f-40c9-965f-b6f91038dd08", Guid.NewGuid().ToString().ToUpper()); // CRM.Client
            sln = sln.Replace("fdef1d0b-1f49-4134-ad53-9650c5aba30a", Guid.NewGuid().ToString().ToUpper()); // CRM.DataAccess
            sln = sln.Replace("541b8602-5d22-4f20-a035-5ff11a50c62c", Guid.NewGuid().ToString().ToUpper()); // CRM.DataObjects

            sln = sln.Replace("CRM", _newAppName);

            File.WriteAllText(fileSLNX, sln);
        }
    }
}