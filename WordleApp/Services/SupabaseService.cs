using SupabaseService.Repository;

namespace WordleApp.Services;

public class SupabaseService
{
    public SupabaseRepository SupabaseRepository { get; set; }
    
    public SupabaseService()
    {
        SupabaseRepository = new SupabaseRepository();
    }
}