using Microsoft.EntityFrameworkCore;
using BibliotecaApi2.Models;
using System.Security.Cryptography.X509Certificates;

public class BibliotecaApi2DbContext : DbContext
{
    public DbSet<Libro> Libros { get ; set; } = null!;
    public DbSet<Prestamo> Prestamos { get ; set; } = null!;
    public DbSet<Usuario> Usuarios { get ; set; } = null!;


    public BibliotecaApi2DbContext(DbContextOptions<BibliotecaApi2DbContext> options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>().HasKey(u=> u.Id);
        modelBuilder.Entity<Libro>().HasKey(l=> l.Id);
        modelBuilder.Entity<Prestamo>().HasKey(p=> p.Id);

        //Relaciones

        //Usuario - Prestamo
        modelBuilder.Entity<Usuario>()
        .HasMany(u=> u.Prestamos)
        .WithOne(p=> p.Usuario)
        .HasForeignKey(p=> p.UsuarioId)
        .OnDelete(DeleteBehavior.Cascade);

        //Libro - Prestamo
        modelBuilder.Entity<Libro>()
        .HasMany(l=> l.Prestamos)
        .WithOne(p=> p.Libro)
        .HasForeignKey(p=> p.LibroId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}