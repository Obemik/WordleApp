using SupabaseService.Models;
using Newtonsoft.Json;

namespace SupabaseService.Repository;

public class JsonRepository(string filePath)
{
    private string FilePath { get; set; } = filePath;
    
    public async Task<SupabaseConfigModel?> ReadJsonAsync()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException("File not found", FilePath);
            }
            string json = await File.ReadAllTextAsync(FilePath);
            var model = JsonConvert.DeserializeObject<SupabaseConfigModel>(json);
            return model;
        }
        catch (Exception e)
        {
            throw new Exception("Error reading JSON file", e);
        }
    }
}