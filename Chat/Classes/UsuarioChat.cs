namespace Chat.Classes;

public class UsuarioChat
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public RolesEnum Rol { get; set; }
    public string? Avatar { get; set; }
}

public enum RolesEnum
{
    Admin,
    Grupo1,
    Grupo2,
    Grupo3,
    Grupo4,
    Grupo5,
    Grupo6,
}