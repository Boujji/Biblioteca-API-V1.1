using BibliotecaApi2.Models;
using BibliotecaApi2.Utils;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi2.Seeder
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync (BibliotecaApi2DbContext context)
        {
            // =========
            // Usuarios
            // =========
            if (!await context.Usuarios.AnyAsync())
            {
                context.Usuarios.Add(new Usuario
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    eMail = "admin@gmail.com",
                    PasswordHash = PasswordHelper.Hash("admin"),
                    Rol = Enum.RolUsuario.Admin,
                    DescNextPrest = 0,
                    PenalizacionPendiente = 0,
                     FechaRegistro = DateTime.UtcNow
                });
            }

            // =======
            // Libros
            // =======
            if (!await context.Libros.AnyAsync())
            {
                context.Libros.Add(new Libro
                {
                    Titulo = "Primer Libro",
                    Categoria = "Libro Base",
                    Disponible = true,
                    CantidadLibro = 10
                });
            }

            await context.SaveChangesAsync();
        }
    }
}