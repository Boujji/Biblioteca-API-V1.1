using BibliotecaApi2.Enum;

namespace BibliotecaApi2.Models
{
    public class Usuario
    {
        public Guid Id { get ; set; }
        public string Name { get ; set; } = null!;
        public string eMail { get ; set ; } = null!;
        public DateTime FechaRegistro { get ; set ;}
        public string PasswordHash { get; set; } = null!; // Para login
        public RolUsuario Rol { get ; set; } // Admin, Usuario, Contable, etc.
        public decimal PenalizacionPendiente { get ; set; }
        public ICollection<Prestamo> Prestamos { get ; set; } = new List<Prestamo>();
        public decimal DescNextPrest {get ; set ;}
    }
}