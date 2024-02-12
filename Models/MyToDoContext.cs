using Microsoft.EntityFrameworkCore;

using MyToDo.Models;

namespace MyToDo.Models;


	public class MyToDoContext : DbContext
{
		public MyToDoContext(DbContextOptions<MyToDoContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<ToDo> ToDos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<ToDo>().ToTable("ToDo");

    }

    //     protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //     {
    //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
    //     }
}

