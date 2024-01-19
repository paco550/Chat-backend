using System;
using System.Collections.Generic;

namespace Chat.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Rol { get; set; }

    public byte[]? Salt { get; set; }

    public string? EnlaceCambioPass { get; set; }

    public DateTime? FechaEnvioEnlace { get; set; }
}
