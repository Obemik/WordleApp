using SupabaseService.DataSource;

namespace SupabaseService.Repository;

public partial class SupabaseRepository
{
    private CloudDatabase? CloudDatabase { get; set; } = null;
    private JsonRepository? JsonRepository { get; set; } = null;
    private bool _isInitialized = false;
    
    public SupabaseRepository()
    {
        //Get solution directory
        string solutionDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\"));
        //Get file path
        string filePath = Path.Combine(solutionDirectory, @"SupabaseService\.env\supabase_keys.json");
        
        JsonRepository = new JsonRepository(filePath);
    }
    
    public async Task InitializeAsync()
    {
        if (_isInitialized) return;
        
        try
        {
            var supabaseConfigs = await JsonRepository!.ReadJsonAsync();
            if (supabaseConfigs == null || supabaseConfigs.Url == null || supabaseConfigs.Key == null)
            {
                throw new Exception("Supabase configs are null");
            }
            CloudDatabase = new CloudDatabase(supabaseConfigs.Url, supabaseConfigs.Key);
            _isInitialized = true;
        }
        catch (Exception e)
        {
            throw new Exception("Error initializing Supabase client", e);
        }
    }
    
    private async Task EnsureInitializedAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
    }
}
