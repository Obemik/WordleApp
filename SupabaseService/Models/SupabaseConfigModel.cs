namespace SupabaseService.Models;

/// <summary>
/// Represents the configuration details for a Supabase service, including the URL and API key.
/// </summary>
public class SupabaseConfigModel
{
    /// <summary>
    /// Gets or sets the URL of the Supabase service.
    /// This property is nullable and defaults to null.
    /// </summary>
    public string? Url { get; set; } = null;

    /// <summary>
    /// Gets or sets the API key for the Supabase service.
    /// This property is nullable and defaults to null.
    /// </summary>
    public string? Key { get; set; } = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="SupabaseConfigModel"/> class with the specified URL and API key.
    /// </summary>
    /// <param name="url">The URL of the Supabase service.</param>
    /// <param name="key">The API key for the Supabase service.</param>
    public SupabaseConfigModel(string url, string key)
    {
        Url = url;
        Key = key;
    }
}