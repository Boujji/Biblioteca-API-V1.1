namespace BibliotecaApi2.Dtos
{
    public class PutUsuarioDto
    {
        public string Name { get ; set; } = null!;
        public string eMail { get ; set ; } = null!;
        public decimal PenalizacionPendiente { get ; set; }
        public decimal DescNextPrest {get ; set ;}
    }
}