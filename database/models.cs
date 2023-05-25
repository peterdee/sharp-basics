using Microsoft.EntityFrameworkCore;

namespace SharpBasics.Models
{
    public class Password
    {
          public string Hash { get; set; } = "";
          public int Id { get; set; }
          public int UserId { get; set; }
    }
    public class User
    {
          public string Email { get; set; } = "";
          public int Id { get; set; }
          public string Name { get; set; } = "";
    }
    class DB : DbContext
    {
      public DB(DbContextOptions options) : base(options) {
        Database.EnsureCreated();
      }

      public DbSet<Password> Passwords { get; set; } = null!;
      public DbSet<User> Users { get; set; } = null!;
  }
}
