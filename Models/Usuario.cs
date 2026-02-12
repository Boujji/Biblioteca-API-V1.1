namespace BibliotecaApi2.Models
{
    public class Usuario
    {
        public Guid Id { get ; set; }
        public string Name { get ; set; } = null!;
        public string eMail { get ; set ; } = null!;
        public DateTime FechaRegistro { get ; set ;}
        public decimal PenalizacionPendiente { get ; set; }
        public ICollection<Prestamo> Prestamos { get ; set; } = new List<Prestamo>();
        public decimal DescNextPrest {get ; set ;}
    }
}