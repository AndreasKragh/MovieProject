using Microsoft.EntityFrameworkCore;

public class LibraryContext : DbContext
{
public LibraryContext(DbContextOptions options) : base(options)
{
}
}