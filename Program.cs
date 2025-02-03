using System.CommandLine;
using Mutagen.Bethesda.Skyrim;

namespace TatsConverter;

class Program
{
    private static string _tatsRelativePath = @"actors\character\slavetats";

    static void Main(string[] args)
    {
        var command = new RootCommand("TatsConverter: convert SlaveTats to RaceMenu overlays");
        
        var contextOption = new Option<string>("--context", "Conversion context - the name of result plugin")
        {
            IsRequired = true,
        };
        command.AddOption(contextOption);

        var outPathOption = new Option<string>("--out", "Output path where plugin will be created")
        {
            IsRequired = true,
        };
        command.AddOption(outPathOption);

        var formatOption = new Option<string>("--format", "Output plugin format")
            .FromAmong("esp", "esl");
        formatOption.SetDefaultValue("esp");
        command.AddOption(formatOption);

        var versionOption = new Option<string>("--skyrim", "Skyrim version")
            .FromAmong("se", "le", "segog");
        versionOption.SetDefaultValue("se");
        command.AddOption(versionOption);

        var dataPathOption = new Option<string[]>("--data-path",
            "Path to tats data directory (where \"textures\" directory is)");
        command.AddOption(dataPathOption);

        var jsonPathOption = new Option<string[]>("--json-path",
            "Path to tats json directory (where *.json files are)");
        command.AddOption(jsonPathOption);

        var listOption = new Option<bool>("--list", "Only list found tattoos, do not create anything");
        command.AddOption(listOption);

        command.SetHandler((contextOptionValue, dataPathOptionValue, jsonPathOptionValue, listOptionValue,
            outPathOptionValue, formatOptionValue, versionOptionValue) =>
        {
            if (dataPathOptionValue.Length == 0 && jsonPathOptionValue.Length == 0)
            {
                Console.WriteLine("Either --data-path or --json-path must be specified");
                command.Invoke("--help");
                return;
            }

            var collector = new TattooCollector();
            foreach (var dataPath in dataPathOptionValue)
                collector.CollectFromPathDataPath(dataPath);
            foreach (var jsonPath in jsonPathOptionValue)
                collector.CollectFromJsonPath(jsonPath);

            if (collector.Tattoos.Count == 0)
            {
                Console.WriteLine("No tattoos found");
                return;
            }

            var tattoos = collector.Tattoos.DistinctBy(t => t.Texture.ToLower()).ToList();
            
            if (listOptionValue)
            {
                foreach (var tattoo in collector.Tattoos)
                    Console.WriteLine($"[{tattoo.Area}][{tattoo.Section}][{tattoo.Name}] \"{tattoo.Texture}\"");
                Console.WriteLine($"Total tattoos: {collector.Tattoos.Count}");
                Console.WriteLine($"Distinct tattoos: {tattoos.Count}");
                return;
            }
            
            Console.WriteLine($"Distinct tattoos: {tattoos.Count}");

            if (!Directory.Exists(outPathOptionValue))
            {
                Console.WriteLine("Output path does not exist");
                return;
            }

            var version = SkyrimRelease.SkyrimSE;
            switch (versionOptionValue)
            {
                case "se": version = SkyrimRelease.SkyrimSE; break;
                case "le": version = SkyrimRelease.SkyrimLE; break;
                case "segog": version = SkyrimRelease.SkyrimSEGog; break;
            }

            var pluginPath = Path.Combine(outPathOptionValue,
                Helpers.NormalizeId(contextOptionValue) + "." + formatOptionValue);
            var c = new PluginBuilder(contextOptionValue,
                pluginPath,
                version);

            var scriptContent = ScriptGenerator.Generate(c.ScriptName, contextOptionValue, collector._tatsRelativePath,
                tattoos);

            Console.WriteLine($"Writing plugin to \"{pluginPath}\", version: {version}");
            c.Save(tattoos.Select(t => t.Section).Distinct());

            var sourceDirectory = Path.Combine(outPathOptionValue, @"scripts\source");
            if (!Directory.Exists(sourceDirectory))
                Directory.CreateDirectory(sourceDirectory);

            var scriptPath = Path.Combine(sourceDirectory, c.ScriptName.ToLower() + ".psc");
            Console.WriteLine($"Writing script to \"{scriptPath}\"");
            File.WriteAllText(scriptPath, scriptContent);
            
            Console.WriteLine($"Overlays will have namings: \"{contextOptionValue} - <Tattoo section> - <Tattoo name>\"");
        }, contextOption, dataPathOption, jsonPathOption, listOption, outPathOption, formatOption, versionOption);

        try
        {
            command.Invoke(args);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error, {e.GetType()}: {e.Message}");
        }
    }
}
