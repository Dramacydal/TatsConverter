using System.Text.Json;

namespace TatsConverter;

public class TattooCollector
{
    public List<TattooData> Tattoos { get; } = new(); 
    
    public readonly string _tatsRelativePath = @"actors\character\slavetats"; 
    
    public void CollectFromPathDataPath(string dataPath)
    {
        var jsonPath = Path.Combine(dataPath, Path.Combine("textures", _tatsRelativePath));
        if (!Directory.Exists(jsonPath))
            throw new Exception("Failed to find json files at " + jsonPath);
        
        CollectFromJsonPath(jsonPath);
    }

    public void CollectFromJsonPath(string jsonPath)
    {
        var jsonFiles = Directory.GetFiles(jsonPath, "*.json");
        Console.WriteLine($"Found *.json files: {jsonFiles.Length}");
        
        foreach (var file in jsonFiles)
        {
            try
            {
                var tattooData = JsonSerializer.Deserialize<List<TattooData>>(File.ReadAllText(file));
                Console.WriteLine($"Loaded {tattooData.Count} tattoos from {file}");
                Tattoos.AddRange(tattooData);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to parse json from {file}: {e.Message}");
            }
        }

        Console.WriteLine($"Total {Tattoos.Count} tattoos loaded");
    }
}