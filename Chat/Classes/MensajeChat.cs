namespace Chat.Classes;

public class MensajeChat
{
    public Guid Id { get; set; }

    public required UsuarioChat User { get; set; }

    public required string Text { get; set; }
    public required string Room { get; set; }
    public DateTime Fecha { get; set; }
}
