namespace Util;

public static class RemoveModulesTools
{
    private static string _filePath = "";

    private static List<Module> AllModules {
        get {
            var baseName = BaseAppName;

            return new List<Module> {
                    new Module {
                        Name = "About",
                        FilesToRemove = new List<string> {
                            baseName + @".Client\Pages\About.razor",
                            baseName + @".Client\Shared\AppComponents\About.App.razor",
                        },
                    },

                    new Module {
                        Name = "Appointments",
                        FilesToRemove = new List<string> {
                            baseName + @"\Controllers\DataController.Appointments.cs",
                            baseName + @".Client\Pages\Invoices\AppointmentInvoices.razor",
                            baseName + @".Client\Shared\AppComponents\EditAppointment.App.razor",
                            baseName + @".DataAccess\DataAccess.Appointments.cs",
                            baseName + @".DataObjects\DataObjects.Appointments.cs",
                            baseName + @".EFModels\EFModels\Appointment.cs",
                            baseName + @".EFModels\EFModels\AppointmentNote.cs",
                            baseName + @".EFModels\EFModels\AppointmentService.cs",
                            baseName + @".EFModels\EFModels\AppointmentUser.cs",
                        },
                        FoldersToRemove = new List<string> {
                            baseName + @".Client\Pages\Scheduling",
                        },
                        ReplaceLinesInFiles = new List<LineInFile> {
                            new LineInFile {
                                File = baseName + @".DataAccess\DataAccess.EmailTemplates.cs",
                                OriginalLine = "string subject = ReplaceTagsInText(_details.Subject, user, appt);",
                                NewLine = "string subject = ReplaceTagsInText(_details.Subject, user);",
                            },
                            new LineInFile {
                                File = baseName + @".DataAccess\DataAccess.EmailTemplates.cs",
                                OriginalLine = "string body = ReplaceTagsInText(_details.Body, user, appt);",
                                NewLine = "string body = ReplaceTagsInText(_details.Body, user);",
                            },
                        },
                    },

                    new Module {
                        Name = "EmailTemplates",
                        FilesToRemove = new List<string> {
                            baseName + @"\Controllers\DataController.EmailTemplates.cs",
                            baseName + @".DataAccess\DataAccess.EmailTemplates.cs",
                            baseName + @".DataObjects\DataObjects.EmailTemplates.cs",
                            baseName + @".EFModels\EFModels\EmailTemplate.cs",
                        },
                        FoldersToRemove = new List<string> {
                            baseName + @".Client\Pages\Settings\Email",
                        },
                    },

                    new Module {
                        Name = "Invoices",
                        FilesToRemove = new List<string> {
                            baseName + @"\Controllers\DataController.Invoices.cs",
                            baseName + @".DataAccess\DataAccess.Invoices.cs",
                            baseName + @".DataAccess\DataAccess.PDF.cs",
                            baseName + @".DataObjects\DataObjects.Invoices.cs",
                            baseName + @".EFModels\EFModels\Invoice.cs",
                        },
                        FoldersToRemove = new List<string> {
                            baseName + @".Client\Pages\Invoices",
                        },
                    },

                    new Module {
                        Name = "Locations",
                        FilesToRemove = new List<string> {
                            baseName + @"\Controllers\DataController.Locations.cs",
                            baseName + @".DataAccess\DataAccess.Locations.cs",
                            baseName + @".DataObjects\DataObjects.Locations.cs",
                            baseName + @".EFModels\EFModels\Location.cs",
                        },
                        FoldersToRemove = new List<string> {
                            baseName + @".Client\Pages\Settings\Locations",
                        },
                    },

                    new Module {
                        Name = "Logo",
                    },

                    new Module {
                        Name = "Payments",
                        FilesToRemove = new List<string> {
                            baseName + @"\Controllers\DataController.Payments.cs",
                            baseName + @".DataAccess\DataAccess.Payments.cs",
                            baseName + @".DataObjects\DataObjects.Payments.cs",
                            baseName + @".EFModels\EFModels\Payment.cs",
                        },
                        FoldersToRemove = new List<string> {
                            baseName + @".Client\Pages\Payments",
                        },
                    },

                    new Module {
                        Name = "SamplePages",
                        FoldersToRemove = new List<string> {
                            baseName + @".Client\Pages\TestPages",
                        },
                    },

                    new Module {
                        Name = "SamplePlugins",
                        FilesToRemove = new List<string> {
                            baseName + @"\PluginFiles\Example1.cs",
                            baseName + @"\PluginFiles\Example2.cs",
                            baseName + @"\PluginFiles\Example3.cs",
                            baseName + @"\PluginFiles\ExampleBackgroundProcess.cs",
                            baseName + @"\PluginFiles\HelloWorld.assemblies",
                            baseName + @"\PluginFiles\HelloWorld.plugin",
                            baseName + @"\PluginFiles\LoginWithPrompts.cs",
                            baseName + @"\PluginFiles\UserUpdate.cs",
                            baseName + @"\PluginFiles\BlazorComponents\Button_Testing_TestButton.json",
                            baseName + @"\PluginFiles\BlazorComponents\Button_Testing_TestButton.razor",
                            baseName + @"\PluginFiles\BlazorComponents\Top_Testing_Sample.json",
                            baseName + @"\PluginFiles\BlazorComponents\Top_Testing_Sample.razor",
                        },
                        FoldersToRemove = new List<string> {
                            baseName + @"\PluginFiles\HelloWorld",
                        }
                    },

                    new Module {
                        Name = "Services",
                        FilesToRemove = new List<string> {
                            baseName + @"\Controllers\DataController.Services.cs",
                            baseName + @".DataAccess\DataAccess.Services.cs",
                            baseName + @".DataObjects\DataObjects.Services.cs1",
                            baseName + @".EFModels\EFModels\Service.cs",
                        },
                        FoldersToRemove = new List<string> {
                            baseName + @".Client\Pages\Services",
                            baseName + @".Client\Pages\Settings\Services",
                        },
                    },

                    new Module {
                        Name = "Tags",
                        FilesToRemove = new List<string> {
                            baseName + @"\Controllers\DataController.Tags.cs",
                            baseName + @".Client\Shared\TagSelector.razor",
                            baseName + @".Client\Shared\AppComponents\EditTag.App.razor",
                            baseName + @".Client\Shared\AppComponents\TagSelector.razor",
                            baseName + @".DataAccess\DataAccess.Tags.cs",
                            baseName + @".DataObjects\DataObjects.Tags.cs",
                            baseName + @".EFModels\EFModels\Tag.cs",
                            baseName + @".EFModels\EFModels\TagItem.cs",
                        },
                        FoldersToRemove = new List<string> {
                            baseName + @".Client\Pages\Settings\Tags",
                        },
                    },

                    new Module {
                        Name = "Workflows",
                        FilesToRemove = new List<string> {
                            baseName + @".Client\Shared\WorkflowEditor.razor",
                            baseName + @".Client\Shared\WorkflowViewer.razor",
                            baseName + @".Client\WorkflowEngine.cs",
                            baseName + @".DataObjects\DataObjects.Workflows.cs",
                            baseName + @"",
                            baseName + @"",
                        },
                    }
                };
        }
    }

    public static string BaseAppName {
        get {
            string output = String.Empty;

            var files = System.IO.Directory.EnumerateFiles(_filePath);
            foreach (var file in files) {
                if (file.ToLower().EndsWith(".sln") || file.ToLower().EndsWith(".slnx")) {
                    output = System.IO.Path.GetFileNameWithoutExtension(file);
                }
            }

            return output;
        }
    }

    public static List<Module> Modules {
        get {
            var output = new List<Module>();

            // include items if any of the files or folders still exist.
            foreach (var item in AllModules) {
                bool include = false;

                foreach (var file in item.FilesToRemove) {
                    if (System.IO.File.Exists(System.IO.Path.Combine(_filePath, file))) {
                        include = true;
                        break;
                    }
                }

                if (!include) {
                    foreach (var folder in item.FoldersToRemove) {
                        if (System.IO.Directory.Exists(System.IO.Path.Combine(_filePath, folder))) {
                            include = true;
                            break;
                        }
                    }
                }

                if (!include) {
                    // If this item did not have any FileToRemove or FoldersToRemove
                    // then check if any app files contain {{ModuleItemStart:MODULE}} items.
                    if (!item.FilesToRemove.Any() && !item.FoldersToRemove.Any()) {
                        var sourceCode = SourceCode.GetAllSourceCode(_filePath);
                        if (sourceCode.Contains("{{ModuleItemStart:" + item.Name + "}}")) {
                            include = true;
                        }
                    }
                }

                if (include) {
                    output.Add(item);
                }
            }

            return output;
        }
    }

    private static string RemoveExtraEmptyLines(string input)
    {
        var output = new System.Text.StringBuilder();

        bool wasLastLineEmpty = false;
        var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (var line in lines) {
            // Check if the current line is empty or contains only whitespace
            bool isCurrentLineEmpty = String.IsNullOrWhiteSpace(line);

            // If both the current and the last line were empty, skip the current line.
            if (isCurrentLineEmpty && wasLastLineEmpty) {
                continue;
            }

            output.AppendLine(line);
            wasLastLineEmpty = isCurrentLineEmpty;
        }

        return output.ToString().Trim();
    }

    public static List<string> RemoveModule(string module)
    {
        var output = new List<string>();

        var item = AllModules.FirstOrDefault(x => x.Name.ToLower() == module.ToLower());
        if (item != null) {
            var baseName = BaseAppName;

            // Add an entry to the RemovedModules.log file in the root folder with the name of the module and date removed
            // if the file doesn't already contain an entry for that module.
            var logFilePath = System.IO.Path.Combine(_filePath, "RemovedModules.log");
            var removedModules = new List<string>();

            if (System.IO.File.Exists(logFilePath)) {
                var lines = System.IO.File.ReadAllLines(logFilePath);
                if (lines != null && lines.Length > 0) {
                    foreach (var line in lines) {
                        if (!String.IsNullOrWhiteSpace(line) && AllModules.Any(x => x.Name.ToLower() == line.ToLower())) {
                            removedModules.Add(line.Trim());
                        }
                    }
                }
            }

            if (!removedModules.Contains(item.Name)) {
                removedModules.Add(item.Name);

                System.IO.File.WriteAllLines(logFilePath, removedModules.OrderBy(x => x));
            }

            output.Add("Removing Module '" + module + "'...");

            foreach (var file in item.FilesToRemove) {
                var filePath = System.IO.Path.Combine(_filePath, file);
                if (System.IO.File.Exists(filePath)) {
                    System.IO.File.Delete(filePath);
                    output.Add("  Removed File: " + filePath);
                }
            }

            foreach (var folder in item.FoldersToRemove) {
                var folderPath = System.IO.Path.Combine(_filePath, folder);
                if (System.IO.Directory.Exists(folderPath)) {
                    System.IO.Directory.Delete(folderPath, true);
                    output.Add("  Removed Folder: " + folderPath);
                }
            }

            foreach (var lineToReplace in item.ReplaceLinesInFiles) {
                var filePath = System.IO.Path.Combine(_filePath, lineToReplace.File);
                if (System.IO.File.Exists(filePath)) {
                    var fileLines = System.IO.File.ReadAllLines(filePath);
                    var updatedFileContent = new System.Text.StringBuilder();

                    foreach (var line in fileLines) {
                        if (line.Trim() == lineToReplace.OriginalLine.Trim()) {
                            updatedFileContent.AppendLine(lineToReplace.NewLine);
                            output.Add("  Replaced Line in '" + filePath + "': " + lineToReplace.OriginalLine + " -> " + lineToReplace.NewLine);
                        } else {
                            updatedFileContent.AppendLine(line);
                        }
                    }

                    System.IO.File.WriteAllText(filePath, RemoveExtraEmptyLines(updatedFileContent.ToString()));
                }
            }

            // Remove stuff between {{ModuleItemStart:MODULE}} and {{ModuleItemEnd:MODULE}}
            // Clean any .cs and .razor files
            var allFiles = System.IO.Directory.GetFiles(_filePath, "*.*", SearchOption.AllDirectories);
            foreach (var file in allFiles) {
                var ext = System.IO.Path.GetExtension(file).ToLower();
                if (ext == ".cs" || ext == ".razor") {
                    var fileContent = System.IO.File.ReadAllText(file);
                    if (fileContent.Contains("{{ModuleItemStart:" + module + "}}", StringComparison.InvariantCultureIgnoreCase)) {
                        var fileLines = System.IO.File.ReadAllLines(file);

                        bool inRemoveTag = false;
                        var updatedFileContent = new System.Text.StringBuilder();

                        foreach (var fileLine in fileLines) {
                            bool addLine = true;

                            if (fileLine.Contains("{{ModuleItemStart:" + module + "}}", StringComparison.InvariantCultureIgnoreCase)) {
                                inRemoveTag = true;
                                addLine = false;
                            } else if (fileLine.Contains("{{ModuleItemEnd:" + module + "}}", StringComparison.InvariantCultureIgnoreCase)) {
                                inRemoveTag = false;
                                addLine = false;
                            } else {
                                addLine = !inRemoveTag;
                            }

                            if (addLine) {
                                updatedFileContent.AppendLine(fileLine);
                            }
                        }

                        System.IO.File.WriteAllText(file, RemoveExtraEmptyLines(updatedFileContent.ToString()));
                        output.Add("  Removed References in '" + file + "'");
                    }
                }
            }
        } else {
            output.Add("Unable to remove module '" + module + "'.");
        }

        return output;
    }

    public static void SetFilePath(string filePath)
    {
        _filePath = filePath;
    }
}