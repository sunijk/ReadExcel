using LoanReadExcelChunksFuncApp.Mapping.LoanExcelFunctionApp.Mapping;
using Newtonsoft.Json;
using ReadExcelChunksFuncColumnMappingTemplate.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LoanReadExcelChunksFuncApp.Mapping
{
    public static class TemplateLoader
    {
        private const string FolderName = "Templates";
        private const string CurrentTemplateKey = "CurrentTemplate";

        public static string ResolvedFolderPath { get; private set; }
        public static string LoadedTemplateName { get; private set; }
        public static IReadOnlyList<string> CandidatePaths { get; private set; }

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static ColumnMappingOptions Load()
        {
            // 1. Which template?
            string templateName = Environment.GetEnvironmentVariable(CurrentTemplateKey);

            if (string.IsNullOrWhiteSpace(templateName))
                throw new InvalidOperationException(
                    $"The '{CurrentTemplateKey}' app setting is not set.\n" +
                    $"Add it to local.settings.json Values:\n" +
                    $"  \"CurrentTemplate\": \"Template1\"");

            // 2. Where is the Templates folder?
            string folder = ResolveFolder();
            ResolvedFolderPath = folder;

            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException(
                    $"Templates folder not found.\n" +
                    $"Candidates tried:\n{FormatCandidates()}\n" +
                    $"Add \"TEMPLATE_FOLDER_PATH\": \"<absolute path>\" to local.settings.json.");

            // 3. Load the matching template file
            return LoadTemplate(folder, templateName);
        }

        public static ColumnMappingOptions LoadFromFolder(string folder, string templateName)
        {
            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException($"Folder not found: {folder}");

            return LoadTemplate(folder, templateName);
        }

        // ----------------------------------------------------------------
        // Core loading
        // ----------------------------------------------------------------

        private static ColumnMappingOptions LoadTemplate(string folder, string templateName)
        {
            // Try exact filename first: Template1.json
            string expectedFile = Path.Combine(folder, $"{templateName}.json");

            if (File.Exists(expectedFile))
            {
                var def = DeserialiseAndValidate(expectedFile, templateName);
                return WrapInOptions(def, templateName);
            }

            // Fall back: scan all *.json files for matching TemplateName field
            foreach (string filePath in Directory.GetFiles(folder, "*.json"))
            {
                var candidate = TryDeserialise(filePath);
                if (candidate != null &&
                    string.Equals(candidate.TemplateName, templateName,
                                  StringComparison.OrdinalIgnoreCase))
                {
                    ValidateDefinition(candidate, filePath);
                    return WrapInOptions(candidate, templateName);
                }
            }

            throw new InvalidOperationException(
                $"No template named '{templateName}' found in '{folder}'.\n" +
                $"Files present: {string.Join(", ", Directory.GetFiles(folder, "*.json"))}\n" +
                $"Check 'CurrentTemplate' matches the 'TemplateName' field in one of the JSON files.");
        }

        private static ColumnMappingOptions WrapInOptions(
            TemplateDefinition def, string templateName)
        {
            LoadedTemplateName = def.TemplateName;

            var options = new ColumnMappingOptions
            {
                ActiveTemplateName = def.TemplateName   // ← tells ExcelReaderService which one to use
            };
            options.Templates[def.TemplateName] = def;
            return options;
        }

        // ----------------------------------------------------------------
        // Deserialisation
        // ----------------------------------------------------------------

        private static TemplateDefinition DeserialiseAndValidate(
            string filePath, string expectedName)
        {
            var def = TryDeserialise(filePath)
                      ?? throw new InvalidOperationException(
                             $"File '{filePath}' deserialised to null.");

            ValidateDefinition(def, filePath);

            if (!string.Equals(def.TemplateName, expectedName,
                                StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"File '{filePath}' has TemplateName='{def.TemplateName}' " +
                    $"but CurrentTemplate='{expectedName}'. " +
                    $"Fix the TemplateName field inside the JSON file.");

            return def;
        }

        private static TemplateDefinition TryDeserialise(string filePath)
        {
            try
            {
                string raw = File.ReadAllText(filePath);
                var def = JsonConvert.DeserializeObject<TemplateDefinition>(raw, JsonSettings);

                if (def?.Mapping == null || def.Mapping.Count == 0)
                    throw new InvalidOperationException(
                        $"Template '{def?.TemplateName ?? filePath}' Mapping is empty.\n" +
                        $"Common causes:\n" +
                        $"  1) Key is not exactly \"Mapping\" (capital M)\n" +
                        $"  2) File not saved after editing\n" +
                        $"  3) BOM/encoding — save as UTF-8 without BOM\n\n" +
                        $"Raw JSON:\n{raw}");

                return def;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Invalid JSON in '{filePath}': {ex.Message}", ex);
            }
        }

        private static void ValidateDefinition(TemplateDefinition def, string filePath)
        {
            if (string.IsNullOrWhiteSpace(def.TemplateName))
                throw new InvalidOperationException(
                    $"File '{filePath}' is missing the 'TemplateName' field.");

            if (def.Mapping == null || def.Mapping.Count == 0)
                throw new InvalidOperationException(
                    $"Template '{def.TemplateName}' in '{filePath}' has no column mappings.");
        }

        // ----------------------------------------------------------------
        // Path resolution
        // ----------------------------------------------------------------

        private static string ResolveFolder()
        {
            var candidates = new List<string>();

            string env = Environment.GetEnvironmentVariable("TEMPLATE_FOLDER_PATH");
            if (!string.IsNullOrWhiteSpace(env))
            {
                candidates.Add($"[EnvVar]   {env}  <- SELECTED");
                CandidatePaths = candidates;
                return env;
            }

            string asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string asmCandidate = Path.Combine(asmDir, FolderName);
            if (Directory.Exists(asmCandidate))
            {
                candidates.Add($"[Assembly] {asmCandidate}  <- SELECTED");
                CandidatePaths = candidates;
                return asmCandidate;
            }
            candidates.Add($"[Assembly] {asmCandidate}  (not found)");

            string projectRoot = FindProjectRoot(AppDomain.CurrentDomain.BaseDirectory);
            if (projectRoot != null)
            {
                string projCandidate = Path.Combine(projectRoot, FolderName);
                if (Directory.Exists(projCandidate))
                {
                    candidates.Add($"[Project]  {projCandidate}  <- SELECTED");
                    CandidatePaths = candidates;
                    return projCandidate;
                }
                candidates.Add($"[Project]  {projCandidate}  (not found)");
            }

            string cwdCandidate = Path.Combine(Directory.GetCurrentDirectory(), FolderName);
            if (Directory.Exists(cwdCandidate))
            {
                candidates.Add($"[CWD]      {cwdCandidate}  <- SELECTED");
                CandidatePaths = candidates;
                return cwdCandidate;
            }
            candidates.Add($"[CWD]      {cwdCandidate}  (not found)");

            CandidatePaths = candidates;
            return asmCandidate;
        }

        private static string FindProjectRoot(string startDir)
        {
            string dir = startDir;
            while (!string.IsNullOrEmpty(dir))
            {
                if (Directory.GetFiles(dir, "*.csproj").Length > 0) return dir;
                string parent = Path.GetDirectoryName(dir);
                if (parent == dir) break;
                dir = parent;
            }
            return null;
        }

        private static string FormatCandidates()
        {
            if (CandidatePaths == null) return "    (none)";
            return "    " + string.Join("\n    ", CandidatePaths);
        }
    }
}