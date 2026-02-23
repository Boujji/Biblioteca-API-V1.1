using BibliotecaApi2.Enum;

namespace BibliotecaApi2.Dtos
{
    public class PostUsuarioDto
    {
        public string Name { get ; set; } = null!;
        public string eMail { get ; set ; } = null!;

        public string Password { get ; set ; } = null!;
        public RolUsuario Rol { get ; set ; } = RolUsuario.Usuario;
    }
}