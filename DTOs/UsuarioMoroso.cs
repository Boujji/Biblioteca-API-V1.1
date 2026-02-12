namespace BibliotecaApi2.Dtos
{
    public class UsuarioMorosDto
    {
        public string name { get ; set ; } = null!;
        public decimal ScoreMorosidad { get ; set ; }
        public double DiasRetraso { get ; set ;}
    }
}