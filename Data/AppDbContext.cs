using DotaRPG.DbModel;
using Microsoft.EntityFrameworkCore;

namespace DotaRPG.Data
{
	public class AppDbContext : DbContext
	{
		public DbSet<CharacterClass> CharacterClasses { get; set; }
		public DbSet<Character> Characters { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Называть таблички во множественном числе как бы кринжик
			modelBuilder.Entity<CharacterClass>().ToTable("CharacterClass");
			modelBuilder.Entity<Character>().ToTable("Character");
		}
	}
}
