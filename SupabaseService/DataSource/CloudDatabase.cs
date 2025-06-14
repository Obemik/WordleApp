namespace SupabaseService.DataSource;

public class CloudDatabase
{
    public Supabase.Client SupabaseClient { get; private set; }
    
    public CloudDatabase(string url, string key)
    {
        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = true
        };
        SupabaseClient = new Supabase.Client(url, key, options);
    }
}