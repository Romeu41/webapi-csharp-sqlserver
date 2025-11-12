using Microsoft.EntityFrameworkCore;
using SimplePeopleApi.Models;

namespace SimplePeopleApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Models.Usuario> Usuarios { get; set; } = null!;
        public DbSet<Models.Pessoa> Pessoas { get; set; } = null!;
    public DbSet<Models.ContaAPagar> ContasAPagar { get; set; } = null!;
    public DbSet<Models.ContaPaga> ContasPagas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configure Usuario
            modelBuilder.Entity<Models.Usuario>(b =>
            {
                b.HasKey(u => u.Codigo);
                b.Property(u => u.DataCriacao).HasDefaultValueSql("GETDATE()");
                b.Property(u => u.Senha).HasMaxLength(250);
                b.HasIndex(u => u.Nome).IsUnique(false);
            });

            // configure Pessoa
            modelBuilder.Entity<Models.Pessoa>(b =>
            {
                b.HasKey(p => p.Codigo);
                b.Property(p => p.Nome).HasMaxLength(250);
                b.Property(p => p.CPF).HasMaxLength(11);
                b.Property(p => p.UF).HasMaxLength(2).HasColumnType("varchar(2)");
                b.HasIndex(p => p.CPF).IsUnique();
                b.Property(p => p.DataDeCriacao).HasDefaultValueSql("GETDATE()");
            });

            // configure ContasAPagar
            modelBuilder.Entity<Models.ContaAPagar>(b =>
            {
                b.HasKey(c => c.Numero);
                b.Property(c => c.Valor).HasColumnType("decimal(18,6)");
                b.Property(c => c.Acrescimo).HasColumnType("decimal(18,6)");
                b.Property(c => c.Desconto).HasColumnType("decimal(18,6)");
                b.Property(c => c.InseridoPor).HasMaxLength(250).HasColumnType("varchar(250)");
                b.HasOne<Models.Pessoa>().WithMany().HasForeignKey(c => c.CodigoFornecedor).OnDelete(DeleteBehavior.Restrict);
            });

            // configure ContasPagas
            modelBuilder.Entity<Models.ContaPaga>(b =>
            {
                b.HasKey(c => c.Numero);
                b.Property(c => c.Valor).HasColumnType("decimal(18,6)");
                b.Property(c => c.Acrescimo).HasColumnType("decimal(18,6)");
                b.Property(c => c.Desconto).HasColumnType("decimal(18,6)");
                b.Property(c => c.InseridoPor).HasMaxLength(250).HasColumnType("varchar(250)");
                b.HasOne<Models.Pessoa>().WithMany().HasForeignKey(c => c.CodigoFornecedor).OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
