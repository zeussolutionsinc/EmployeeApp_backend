using Microsoft.EntityFrameworkCore;

namespace VacationAppApi.Models;

public class VacationAppContext : DbContext 
{
    public VacationAppContext(DbContextOptions<VacationAppContext> options) : base(options)
    {

    }
    public DbSet<VacationAppItem> VacationAppItems {get; set;} = null!;
}