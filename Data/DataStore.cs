using ContosoUniversity.Data;
using Microsoft.AspNetCore.Identity;

public class DataStore
{
    public List<ContosoUser> Users { get; set; } = new();
    public List<IdentityRole> Roles { get; set; } = new();
}
