using LoanReadExcelChunksFuncApp.Mapping.LoanExcelFunctionApp.Mapping;
using Newtonsoft.Json;
using ReadExcelChunksFuncColumnMappingTemplate.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LoanReadExcelChunksFuncApp.Mapping
{
    /// <summary>
    /// Loads a single column-mapping template from the Templates folder.
    ///
    /// Which template to load is controlled by the "CurrentTemplate" environment
    /// variable / app setting (e.g. "Template1" or "Template2").
    ///
    /// File resolution order for the Templates folder (first existing path wins):
    ///   1. TEMPLATE_FOLDER_PATH environment variable
    ///   2. Templates\ next to the executing assembly   (bin\Debug\net48\Templates)
    ///   3. Templates\ in the project root              (walks up looking for *.csproj)
    ///   4. Templates\ in the current working directory
    /// </summary>
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

        // ----------------------------------------------------------------
        // Public API
        // ----------------------------------------------------------------

        /// <summary>
        /// Reads the "CurrentTemplate" app setting, finds the matching JSON file
        /// in the Templates folder, and returns a <see cref="ColumnMappingOptions"/>
        /// that contains exactly that one template.
        /// </summary>
        public static ColumnMappingOptions Load()
        {
            // 1. Which template should be loaded?
            string templateName = Environment.GetEnvironmentVariable(CurrentTemplateKey);

            if (string.IsNullOrWhiteSpace(templateName))
                throw new InvalidOperationException(
                    $"The '{CurrentTemplateKey}' app setting is not set.\n" +
                    $"Add it to local.settings.json:\n" +
                    $"  \"CurrentTemplate\": \"Template1\"");

            // 2. Where is the Templates folder?
            string folder = ResolveFolder();
            ResolvedFolderPath = folder;

            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException(
                    $"Templates folder not found.\n" +
                    $"  Candidates tried:\n{FormatCandidates()}\n" +
                    $"  Quick fix: add \"TEMPLATE_FOLDER_PATH\": \"<absolute path>\" " +
                    $"to local.settings.json Values.");

            // 3. Find the JSON file whose TemplateName matches
            return LoadTemplate(folder, templateName);
        }

        /// <summary>
        /// Load from an explicit folder — used by unit tests.
        /// </summary>
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
            // Try the obvious filename first  (Template1.json, Template2.json …)
            string expectedFile = Path.Combine(folder, $"{templateName}.json");

            if (File.Exists(expectedFile))
            {
                var def = DeserialiseAndValidate(expectedFile, templateName);
                return WrapInOptions(def);
            }

            // Fall back: scan all *.json files looking for matching TemplateName field
            foreach (string filePath in Directory.GetFiles(folder, "*.json"))
            {
                TemplateDefinition candidate = TryDeserialise(filePath);
                if (candidate != null &&
                    string.Equals(candidate.TemplateName, templateName,
                                  StringComparison.OrdinalIgnoreCase))
                {
                    ValidateDefinition(candidate, filePath);
                    return WrapInOptions(candidate);
                }
            }

            // Nothing matched
            throw new InvalidOperationException(
                $"No template named '{templateName}' found in '{folder}'.\n" +
                $"  Files present: {string.Join(", ", Directory.GetFiles(folder, "*.json"))}\n" +
                $"  Check the 'CurrentTemplate' setting matches the 'TemplateName' " +
                $"field inside one of the JSON files.");
        }

        private static ColumnMappingOptions WrapInOptions(TemplateDefinition def)
        {
            LoadedTemplateName = def.TemplateName;

            var options = new ColumnMappingOptions();
            options.Templates[def.TemplateName] = def;
            return options;
        }

        // ----------------------------------------------------------------
        // Deserialisation helpers
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
                    $"File '{filePath}' contains TemplateName '{def.TemplateName}' " +
                    $"but CurrentTemplate is set to '{expectedName}'. " +
                    $"Check the TemplateName field inside the JSON file.");

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
                        $"Template '{def?.TemplateName ?? filePath}' has no column mappings.\n\n" +
                        $"  The JSON was read but 'Mapping' is empty. Common causes:\n" +
                        $"    1) Key is not exactly \"Mapping\" (capital M)\n" +
                        $"    2) File was not saved after editing — check your editor\n" +
                        $"    3) BOM/encoding issue — save as UTF-8 without BOM\n\n" +
                        $"  Raw JSON:\n{raw}");

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

            // 1. Explicit env var
            string env = Environment.GetEnvironmentVariable("TEMPLATE_FOLDER_PATH");
            if (!string.IsNullOrWhiteSpace(env))
            {
                candidates.Add($"[EnvVar]   {env}  <- SELECTED");
                CandidatePaths = candidates;
                return env;
            }

            // 2. Next to the executing assembly
            string asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string asmCandidate = Path.Combine(asmDir, FolderName);
            if (Directory.Exists(asmCandidate))
            {
                candidates.Add($"[Assembly] {asmCandidate}  <- SELECTED");
                CandidatePaths = candidates;
                return asmCandidate;
            }
            candidates.Add($"[Assembly] {asmCandidate}  (not found)");

            // 3. Project root
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

            // 4. Current working directory
            string cwdCandidate = Path.Combine(Directory.GetCurrentDirectory(), FolderName);
            if (Directory.Exists(cwdCandidate))
            {
                candidates.Add($"[CWD]      {cwdCandidate}  <- SELECTED");
                CandidatePaths = candidates;
                return cwdCandidate;
            }
            candidates.Add($"[CWD]      {cwdCandidate}  (not found)");

            CandidatePaths = candidates;
            return asmCandidate; // most likely intended path for the error message
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