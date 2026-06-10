namespace FreeCRM_Utilities;

public static class SourceCode
{
    private static string _beginNote = "BEGIN CODE INSERTED BY THE UPDATER UTILITY";
    private static string _endNote = "END CODE INSERTED BY THE UPDATER UTILITY";

    public static string BeginNote {
        get { return _beginNote; }
    }

    public static string EndNote {
        get { return _endNote; }
    }

    public static List<string> CleanCodeList(List<string> code) {
        List<string> output = new List<string>();

        if (code.Any()) {
            bool firstEmptyLine = false;

            foreach (var line in code) {
                if (!firstEmptyLine && String.IsNullOrWhiteSpace(line)) {
                    firstEmptyLine = true;
                } else {
                    output.Add(line);
                }
            }
        }

        return output;
    }

    //public static ClassCode ConvertCodeToClassCode(List<string> code)
    //{
    //	ClassCode output = new ClassCode();

    //	int currentLine = 0;

    //	if (code.Any()) {
    //		// First, see if this starts with comments.
    //		if (code[0].Trim().StartsWith("//")) {
    //			foreach(var line in code.Index()) {
    //				currentLine = line.Index;

    //				if (line.Item.Trim().StartsWith("//")) {
    //					output.Comments.Add(line.Item);
    //				} else {
    //					break;
    //				}
    //			}
    //		}

    //		// Next, add lines to the IntroCode until we hit a line with a { character.
    //		for (int i = currentLine; i < code.Count; i++) {
    //			var line = code[i];

    //			output.IntroCode.Add(line);

    //			if (line.Contains("{")) {
    //				currentLine = i + 1;
    //				break;
    //			}
    //		}

    //		// Next, find the first method.
    //		int firstMethod = -1;

    //		for (int i = currentLine; i < code.Count; i++) {
    //			var line = code[i];

    //			if (line.Trim().StartsWith("//")) {
    //				// This is a comment, so keep going.
    //			} else if (line.Contains("=")) {
    //				// This is a local variable.
    //			} else if (!String.IsNullOrWhiteSpace(line)) {
    //				firstMethod = i;
    //				break;
    //			}
    //		}

    //		if (firstMethod > -1) {
    //			// If there are properties add those first.
    //			List<string> comments = new List<string>();
    //			List<string> c = new List<string>();
    //			int start = -1;

    //			if (firstMethod > currentLine) {
    //				for (int i = currentLine; i < firstMethod; i++) {
    //					currentLine = i;
    //					var line = code[i];

    //					if (line.Trim().StartsWith("//")) {
    //						comments.Add(line);
    //					} else if (String.IsNullOrWhiteSpace(line)) {
    //						// We hit an empty line, so add this item and clear values.
    //						var cleanedCode = CleanCodeList(c);

    //						output.Properties.Add(new ClassItem {
    //							Comments = comments.ToList(),
    //							Code = cleanedCode,
    //							SortItem = SortItem(cleanedCode),
    //							Start = start,
    //							End = i - 1,
    //						});

    //						comments = new List<string>();
    //						c = new List<string>();
    //						start = -1;
    //					} else {
    //						c.Add(line);
    //						start = i;
    //					}
    //				}

    //				if (c.Any()) {
    //					var cleanedCode = CleanCodeList(c);

    //					output.Properties.Add(new ClassItem {
    //						Comments = comments.ToList(),
    //						Code = cleanedCode,
    //						SortItem = SortItem(cleanedCode),
    //						Start = start,
    //						End = currentLine,
    //					});
    //				}
    //			}

    //			// Now add the remaining items.
    //			comments = new List<string>();
    //			c = new List<string>();
    //			bool inMethod = false;
    //			int brace = 0;
    //			bool foundOpenBrace = false;
    //			start = -1;

    //			for (int i = currentLine; i < code.Count; i++) {
    //				var line = code[i];

    //				if (line.Trim().StartsWith("//")) {
    //					comments.Add(line);
    //				} else {
    //					c.Add(line);

    //					if(start == -1) {
    //						start = i;
    //					}

    //					if (line.Contains("{")) {
    //						brace += CountOfCharacterInString(line, "{");
    //						if (!foundOpenBrace) {
    //							foundOpenBrace = true;
    //						}
    //					}

    //					if (line.Contains("}")) {
    //						brace -= CountOfCharacterInString(line, "}");

    //						if (brace < 1) {
    //							var cleanedCode = CleanCodeList(c);

    //							output.Methods.Add(new ClassItem{
    //								Comments = comments.ToList(),
    //								Code = cleanedCode,
    //								SortItem = SortItem(cleanedCode),
    //								Start = start,
    //								End = i,
    //							});

    //							comments = new List<string>();
    //							c = new List<string>();
    //							start = -1;
    //						}
    //					}
    //				}
    //			}

    //			if (c.Any()) {
    //				if (c.Count == 1 && c[1].Trim() == "}") {
    //					// Skip this
    //				} else {
    //					var cleanedCode = CleanCodeList(c);

    //					output.Methods.Add(new ClassItem {
    //						Comments = comments.ToList(),
    //						Code = cleanedCode,
    //						SortItem = SortItem(cleanedCode),
    //						Start = start,
    //						End = code.Count - 1,
    //					});
    //				}
    //			}
    //		}
    //	}

    //	return output;
    //}

    public static int CountOfCharacterInString(string? input, string character) {
        int output = 0;

        if (!String.IsNullOrWhiteSpace(input) && !String.IsNullOrWhiteSpace(character)) {
            foreach (char c in input) {
                if (c.ToString() == character) {
                    output++;
                }
            }
        }

        return output;
    }

    public static List<LanguageItem> FindMissingLanguageItems(string pathToOldApp, string appPath, bool outputResults = true) {
        List<LanguageItem> output = new List<LanguageItem>();

        var oldLanguageItems = SourceCode.GetLanguageItems(pathToOldApp);
        if (outputResults) {
            Utils.ConsoleLog(oldLanguageItems.Count.ToString() + " Old Language Items");
            Utils.ConsoleLog("");
        }

        var newLanguageItems = SourceCode.GetLanguageItems(appPath);
        if (outputResults) {
            Utils.ConsoleLog(newLanguageItems.Count.ToString() + " New Language Items");
            Utils.ConsoleLog("");
        }

        foreach (var item in oldLanguageItems) {
            if (!newLanguageItems.Any(x => x.Tag.ToLower() == item.Tag.ToLower())) {
                output.Add(item);
            }
        }

        return output;
    }

    public static string FirstLinePadding(List<string> items) {
        string output = String.Empty;

        if (items.Any()) {
            var firstItem = items[0];

            foreach (char c in firstItem) {
                if (Char.IsWhiteSpace(c)) {
                    output += c;
                } else {
                    break;
                }
            }
        }

        return output;
    }

    public static string GetAllSourceCode(string path, bool excludeLanguage = false) {
        System.Text.StringBuilder output = new System.Text.StringBuilder();

        var lines = GetAllSourceCodeLines(path, excludeLanguage);
        foreach (var line in lines) {
            output.AppendLine(line);
        }

        return output.ToString();
    }

    public static List<string> GetAllSourceCodeLines(string path, bool excludeLanguage = false) {
        List<string> output = new List<string>();

        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        List<string> includeExtensions = new List<string> { ".cs", ".cshtml", ".razor", ".plugin" };
        List<string> blockPaths = new List<string> { @"\obj\", @"\bin\" };

        if (files.Any()) {
            foreach (var file in files) {
                string fileLower = file.ToLower();
                string fileName = System.IO.Path.GetFileName(fileLower);

                bool blockedPath = false;
                foreach (var item in blockPaths) {
                    if (!blockedPath && fileLower.Contains(item)) {
                        blockedPath = true;
                    }
                }

                if (!blockedPath) {
                    string ext = Path.GetExtension(fileLower);
                    if (!String.IsNullOrWhiteSpace(ext) && includeExtensions.Contains(ext)) {
                        var contents = File.ReadAllLines(file).ToList();

                        if (excludeLanguage) {
                            switch (fileName) {
                                case "dataaccess.languages.cs":
                                    contents = GetCodeBlock("public DataObjects.Language GetDefaultLanguage()", contents, false, true);
                                    break;

                                case "dataaccess.app.cs":
                                    contents = GetCodeBlock("private Dictionary<string, string> AppLanguage", contents, false, true);
                                    break;
                            }
                        }

                        if (contents.Any()) {
                            output.Add("*** BEGIN SOURCE CODE FROM " + file + " ***");
                            //foreach (var line in contents) {
                            //	output.AppendLine(line);
                            //}
                            output.AddRange(contents);
                            output.Add("*** END SOURCE CODE FROM " + file + " ***");
                            output.Add("");
                        }
                    }
                }
            }
        }

        return output;
    }

    public static List<string> GetAtBlock(List<string> code) {
        List<string> output = new List<string>();

        if (code.Any()) {
            bool foundBlock = false;
            int brace = 0;

            for (int i = 0; i < code.Count; i++) {
                var line = code[i];

                if (!foundBlock) {
                    if (line.Trim().Replace(" ", "") == "@{") {
                        foundBlock = true;
                        output.Add(line);

                        brace += CountOfCharacterInString(line, "{");
                    }
                } else {
                    output.Add(line);

                    if (line.Contains("{")) {
                        brace += CountOfCharacterInString(line, "{");
                    }

                    if (line.Contains("}")) {
                        brace -= CountOfCharacterInString(line, "}");
                    }

                    if (brace < 1) {
                        break;
                    }
                }
            }
        }

        return output;
    }

    public static List<string> GetAtCodeBlock(List<string> code) {
        List<string> output = new List<string>();

        if (code.Any()) {
            bool foundBlock = false;
            int brace = 0;

            for (int i = 0; i < code.Count; i++) {
                var line = code[i];

                if (!foundBlock) {
                    if (line.Trim().Replace(" ", "") == "@code{") {
                        foundBlock = true;
                        output.Add(line);

                        brace += CountOfCharacterInString(line, "{");
                    }
                } else {
                    output.Add(line);

                    if (line.Contains("{")) {
                        brace += CountOfCharacterInString(line, "{");
                    }

                    if (line.Contains("}")) {
                        brace -= CountOfCharacterInString(line, "}");
                    }

                    if (brace < 1) {
                        break;
                    }
                }
            }
        }

        return output;
    }

    public static List<string> GetAttributesAboveLine(int line, List<string> source) {
        List<string> output = new List<string>();

        if (line > 0) {
            // If the line before this starts with a [ character, then get the attributes.
            var lineBefore = source[line - 1];
            if (lineBefore != null && lineBefore.Trim().StartsWith("[")) {
                // Find the start of the comments.
                var startOfAttributes = line - 1;

                for (int i = startOfAttributes; i > -1; i--) {
                    var lineToCheck = source[i];

                    if (String.IsNullOrWhiteSpace(lineToCheck)) {
                        break;
                    } else if (lineToCheck.Trim().StartsWith("[")) {
                        // Still in the attributes so keep going.
                        startOfAttributes = i;
                    }
                }

                for (int i = startOfAttributes; i < line; i++) {
                    output.Add(source[i]);
                }
            }
        }

        return output;
    }

    public static List<string> GetCodeBlock(string? identifier, List<string> source, bool includeComments = true, bool inverseOutput = false) {
        List<string> output = new List<string>();

        if (!String.IsNullOrWhiteSpace(identifier) && source.Any()) {
            bool foundIdentifier = false;
            bool foundOpenBrace = false;
            int brace = 0;

            int startLine = -1;
            int endLine = -1;

            foreach (var line in source.Index()) {
                var lineItem = line.Item;
                var index = 0 + line.Index;

                if (!lineItem.Contains("{{ModuleItem")) {
                    bool foundOpenBraceInThisLine = false;

                    if (!foundIdentifier && lineItem.Trim().StartsWith(identifier.Trim())) {
                        foundIdentifier = true;

                        if (lineItem.Contains("{")) {
                            foundOpenBrace = true;
                            foundOpenBraceInThisLine = true;

                            brace += CountOfCharacterInString(lineItem, "{");
                        }

                        startLine = index;
                    }

                    if (foundIdentifier) {
                        if (foundOpenBrace) {
                            if (lineItem.Contains("{") && !foundOpenBraceInThisLine) {
                                brace = brace + CountOfCharacterInString(lineItem, "{");
                            }

                            if (lineItem.Contains("}")) {
                                brace = brace - CountOfCharacterInString(lineItem, "}");
                            }

                            if (brace < 1) {
                                endLine = index;
                                break;
                            }
                        } else if (lineItem.Contains("{")) {
                            foundOpenBrace = true;
                            brace = brace + CountOfCharacterInString(lineItem, "{");
                        }
                    }
                }
            }

            if (startLine > -1 && endLine > startLine) {
                if (inverseOutput) {
                    foreach (var line in source.Index()) {
                        if (line.Index < startLine || line.Index > endLine) {
                            output.Add(line.Item);
                        }
                    }
                } else {
                    var attributes = GetAttributesAboveLine(startLine, source);
                    if (attributes.Any()) {
                        output.AddRange(attributes);
                    }

                    // Never include any ModuleItem comment lines.
                    // Since we are only ever copying code for modules that were not removed
                    // there is no need to copy these lines.
                    // And this prevents likely problems from not finding the closing tag.
                    if (includeComments) {
                        var comments = GetCommentsAboveLine(startLine, source);
                        if (comments.Any()) {
                            foreach (var c in comments) {
                                if (!c.Contains("{{ModuleItem")) {
                                    output.Add(c);
                                }
                            }
                        }
                    }

                    for (int i = startLine; i <= endLine; i++) {
                        var thisLine = source[i];
                        if (!thisLine.Contains("{{ModuleItem")) {
                            output.Add(source[i]);
                        }
                    }
                }
            }
        }

        return output;
    }

    public static List<string> GetCommentsAboveLine(int line, List<string> source) {
        List<string> output = new List<string>();

        if (line > 0) {
            // If the line before this starts with a comment character, then get the comments.
            var lineBefore = source[line - 1];
            if (lineBefore != null && lineBefore.Trim().StartsWith("//")) {
                // Find the start of the comments.
                var startOfComments = line - 1;

                for (int i = startOfComments; i > -1; i--) {
                    var lineToCheck = source[i];

                    if (String.IsNullOrWhiteSpace(lineToCheck)) {
                        break;
                    } else if (lineToCheck.Trim().StartsWith("//")) {
                        // Still in the comment so keep going.
                        startOfComments = i;
                    }
                }

                for (int i = startOfComments; i < line; i++) {
                    output.Add(source[i]);
                }
            }
        }

        return output;
    }

    public static List<string> GetInterface(string? identifier, List<string> source) {
        List<string> output = new List<string>();

        if (!String.IsNullOrWhiteSpace(identifier) && source.Any()) {
            bool found = false;
            int firstLine = -1;

            foreach (var line in source.Index()) {
                if (line.Item.Trim().StartsWith(identifier.Trim())) {
                    found = true;
                    firstLine = line.Index;
                }

                if (found) {
                    output.Add(line.Item);

                    if (line.Item.Trim().StartsWith("}")) {
                        break;
                    }
                }
            }

            if (found && firstLine > -1) {
                var comments = GetCommentsAboveLine(firstLine, source);
                if (comments.Any()) {
                    output.InsertRange(0, comments);
                }
            }
        }

        return output;
    }

    public static List<LanguageItem> GetLanguageItems(string path) {
        List<LanguageItem> output = new List<LanguageItem>();

        if (System.IO.Path.Exists(path)) {
            var appNameSpace = String.Empty;

            // Find the .sln or .slnx file in the folder.
            var slnFile = System.IO.Directory.GetFiles(path, "*.sln*").FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(slnFile)) {
                var slnFileName = System.IO.Path.GetFileNameWithoutExtension(slnFile);
                appNameSpace = slnFileName.Replace(".", "_");
            }

            if (!String.IsNullOrWhiteSpace(appNameSpace)) {
                var dataAccessApp = System.IO.Path.Combine(path, appNameSpace + ".DataAccess", "DataAccess.App.cs");
                var dataAccessLanguage = System.IO.Path.Combine(path, appNameSpace + ".DataAccess", "DataAccess.Language.cs");

                var dataAccessItemsApp = GetLanguageItemsFromFile(dataAccessApp, "private Dictionary<string, string> AppLanguage");
                if (dataAccessItemsApp.Any()) {
                    //dataAccessItemsApp.Dump("dataAccessItemsApp");
                    output.AddRange(dataAccessItemsApp);
                }

                var dataAccessItemsLanguage = GetLanguageItemsFromFile(dataAccessLanguage, "public DataObjects.Language GetDefaultLanguage()");
                if (dataAccessItemsLanguage.Any()) {
                    foreach (var item in dataAccessItemsLanguage) {
                        if (!output.Any(i => i.Tag.ToLower() == item.Tag.ToLower())) {
                            output.Add(item);
                        }
                    }
                }
            } else {
                Utils.ConsoleLog("Could not find solution file in '" + path + "'. Cannot determine namespace for language items.");
            }
        } else {
            Utils.ConsoleLog("Invalid Path '" + path + "'");
        }

        return output;
    }

    public static List<LanguageItem> GetLanguageItemsFromFile(string file, string identifier) {
        List<LanguageItem> output = new List<LanguageItem>();

        if (System.IO.File.Exists(file)) {
            var lines = System.IO.File.ReadAllLines(file).ToList();

            var classCode = GetCodeBlock(identifier, lines, false);
            if (classCode.Any()) {
                // Now find just the dictionary
                List<string> dictionaryLines = new List<string>();

                // First see if this is the main language routine.

                dictionaryLines = GetCodeBlock("Dictionary<string, string> language = new Dictionary<string, string>", classCode, false);

                if (!dictionaryLines.Any()) {
                    // See if this is the language from the app file.
                    dictionaryLines = GetCodeBlock("return new Dictionary<string, string>", classCode, false);
                }

                if (dictionaryLines.Any()) {
                    var lastCommentLine = String.Empty;

                    foreach (var line in dictionaryLines) {
                        var trimmed = line.Trim();

                        if (trimmed.StartsWith("//")) {
                            lastCommentLine = trimmed;
                        } else if (trimmed.StartsWith("{") && trimmed.Contains("}") && trimmed.Contains(",")) {
                            if (trimmed.EndsWith(",")) {
                                trimmed = trimmed.Substring(0, trimmed.Length - 1).Trim();
                            }

                            // Remove the first {
                            trimmed = trimmed.Substring(1);

                            // Remove the final }
                            if (trimmed.EndsWith("}")) {
                                trimmed = trimmed.Substring(0, trimmed.Length - 1).Trim();
                            }

                            // Find the first comma in the string
                            var commaIndex = trimmed.IndexOf(",");

                            var tag = trimmed
                                .Substring(1, commaIndex - 1)
                                .Trim();

                            if (tag.EndsWith("\"")) {
                                tag = tag.Substring(0, tag.Length - 1);
                            }

                            var value = trimmed.Substring(commaIndex + 1).Trim().Substring(1);
                            value = value.Substring(0, value.Length - 1);

                            output.Add(new LanguageItem {
                                Section = lastCommentLine,
                                Tag = tag,
                                Value = value,
                            });
                        }
                    }
                }
            }
        }

        return output;
    }

    //public static int GetNextLineThatsNotAComment(int start, List<string> code)
    //{
    //	int output = -1;

    //	if (start > -1 && code.Any()) {
    //		for (int i = start; i < code.Count; i++) {
    //			var line = code[i];

    //			if (!String.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("//")) {
    //				output = i;
    //				break;
    //			}
    //		}
    //	}

    //	return output;
    //}

    public static async Task<string> GetSourceFileAsync(string urlOrRelativePath) {
        string output = String.Empty;

        var client = new System.Net.Http.HttpClient();
        client.DefaultRequestHeaders.Accept.Clear();

        var url = urlOrRelativePath;

        if (!url.Contains("https://")) {
            url = "https://raw.githubusercontent.com/wicketbr/FreeCRM/refs/heads/main/" + urlOrRelativePath.Replace("\\", "/");
        }

        try {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode) {
                output += await response.Content.ReadAsStringAsync();
            } else {
                Utils.ConsoleLog("Error '" + response.StatusCode.ToString() + "' downloading '" + url + "'");
            }
        } catch (Exception ex) {
            Utils.ConsoleLog("Exception downloading '" + url + "' - " + ex.Message);
        }

        return output;
    }

    public static string GetSourceFile(string urlOrRelativePath) {
        var output = GetSourceFileAsync(urlOrRelativePath).Result;
        return output;
    }

    //private static string SortItem(List<string> code)
    //{
    //	string output = String.Empty;

    //	if (code.Any()) {
    //		output = code[0].Trim();
    //	}

    //	return output;	
    //}

    public static List<string> SplitTextIntoLines(string? input) {
        List<string> output = new List<string>();

        if (!String.IsNullOrWhiteSpace(input)) {
            var lines = input.Split(new[] { Environment.NewLine, "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            if (lines != null && lines.Any()) {
                output = lines;
            }
        }

        return output;
    }

    public static string UpdateCode_InsertIntoAtBlock(string fileContents, string toInsert, List<string>? comments = null) {
        return UpdateCode_InsertIntoAtBlock(fileContents, new List<string> { toInsert }, comments);
    }

    public static string UpdateCode_InsertIntoAtBlock(string fileContents, List<string> toInsert, List<string>? comments = null) {
        string output = fileContents;

        if (!String.IsNullOrWhiteSpace(fileContents) && toInsert.Any()) {
            var updated = new System.Text.StringBuilder();

            var lines = SplitTextIntoLines(fileContents);

            var padding = FirstLinePadding(toInsert);

            foreach (var line in lines) {
                updated.AppendLine(line);

                if (line.Trim().Replace(" ", "") == "@{") {
                    updated.AppendLine(padding + "// " + _beginNote);

                    if (comments != null && comments.Any()) {
                        foreach (var comment in comments) {
                            updated.AppendLine(comment);
                        }
                    }

                    foreach (var insertLine in toInsert) {
                        updated.AppendLine(insertLine);
                    }

                    updated.AppendLine(padding + "// " + _endNote);
                    updated.AppendLine("");
                }
            }

            output = updated.ToString();
        }

        return output;
    }

    public static string UpdateCode_InsertIntoAtCodeBlock(string fileContents, string toInsert, List<string>? comments = null) {
        return UpdateCode_InsertIntoAtCodeBlock(fileContents, new List<string> { toInsert }, comments);
    }

    public static string UpdateCode_InsertIntoAtCodeBlock(string fileContents, List<string> toInsert, List<string>? comments = null) {
        string output = fileContents;

        if (!String.IsNullOrWhiteSpace(fileContents) && toInsert.Any()) {
            var updated = new System.Text.StringBuilder();

            var lines = SplitTextIntoLines(fileContents);

            var padding = FirstLinePadding(toInsert);

            foreach (var line in lines) {
                updated.AppendLine(line);

                if (line.Trim().Replace(" ", "") == "@code{") {
                    updated.AppendLine(padding + "// " + _beginNote);

                    if (comments != null && comments.Any()) {
                        foreach (var comment in comments) {
                            updated.AppendLine(comment);
                        }
                    }

                    foreach (var insertLine in toInsert) {
                        updated.AppendLine(insertLine);
                    }

                    updated.AppendLine(padding + "// " + _endNote);
                    updated.AppendLine("");
                }
            }

            output = updated.ToString();
        }

        return output;
    }

    public static string UpdateCode_InsertIntoElement(string? identifier, string fileContents, string toInsert, List<string>? attributes = null, List<string>? comments = null) {
        return UpdateCode_InsertIntoElement(identifier, fileContents, new List<string> { toInsert }, attributes, comments);
    }

    public static string UpdateCode_InsertIntoElement(string? identifier, string fileContents, List<string> toInsert, List<string>? attributes = null, List<string>? comments = null) {
        string output = fileContents;

        if (!String.IsNullOrWhiteSpace(identifier) && !String.IsNullOrWhiteSpace(fileContents) && toInsert.Any()) {
            var updated = new System.Text.StringBuilder();
            var lines = SplitTextIntoLines(fileContents);
            var padding = FirstLinePadding(toInsert);
            bool foundIdentifier = false;
            bool foundBrace = false;
            bool addedContent = false;

            foreach (var line in lines) {
                updated.AppendLine(line);

                if (!foundIdentifier && line.Trim().ToLower().StartsWith(identifier.Trim().ToLower())) {
                    foundIdentifier = true;
                }

                if (foundIdentifier && line.Contains("{")) {
                    foundBrace = true;
                }

                if (foundIdentifier && foundBrace && !addedContent) {
                    addedContent = true;

                    updated.AppendLine(padding + "// " + _beginNote);

                    if (comments != null && comments.Any()) {
                        foreach (var comment in comments) {
                            updated.AppendLine(comment);
                        }
                    }

                    if (attributes != null && attributes.Any()) {
                        foreach (var a in attributes) {
                            updated.AppendLine(a);
                        }
                    }

                    foreach (var insertLine in toInsert) {
                        updated.AppendLine(insertLine);
                    }

                    updated.AppendLine(padding + "// " + _endNote);
                    updated.AppendLine("");
                }
            }

            output = updated.ToString();
        }

        return output;
    }
}