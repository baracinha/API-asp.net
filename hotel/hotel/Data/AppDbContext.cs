using hotel.Controllers;
using Microsoft.EntityFrameworkCore;
using hotel.Models;

namespace hotel.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<utilizadores> utilizadores{ get; set; }
        public DbSet<mensagens> mensagens { get; set; }
        public DbSet<propriedades> propriedades { get; set; }
        public DbSet<imagens_propriedades> imagens_propriedades { get; set; }
        public DbSet<caracteristicas> caracteristicas { get; set; }
        public DbSet<propriedades_caracteristicas> propriedades_caracteristicas { get; set; }
        public DbSet<conversas> conversas { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<imagens_propriedades>()
                .HasOne(i => i.propriedade)
                .WithMany(p => p.imagens)
                .HasForeignKey(i => i.id_propriedade);
            modelBuilder.Entity<propriedades_caracteristicas>()
                .HasOne(pc => pc.propriedade)
                .WithMany(p => p.caracteristicas)
                .HasForeignKey(pc => pc.id_propriedade);

            modelBuilder.Entity<propriedades_caracteristicas>()
                .HasOne(pc => pc.caracteristica)
                .WithMany(c => c.propriedades_caracteristicas)
                .HasForeignKey(pc => pc.id_caracteristica);
        }
    }
}
