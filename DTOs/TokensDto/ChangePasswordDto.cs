namespace BibliotecaApi2.DTOs.Auth
{
    public class ChangePasswordDto
    {
        public string PasswordActual { get; set; } = null!;
        public string PasswordNueva { get; set; } = null!;
    }
}
