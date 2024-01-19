using Chat.DTOs;
using Chat.Models;
using Chat.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRChat.Classes;
using SignalRChat.Hubs;



public class ChatHub : Hub<IChat>
{
    public static List<Connection> conexiones { get; set; } = new List<Connection>();
    private static List<string> palabrasProhibidas = new List<string> { "tonto", "feo", "capullo" };

    private readonly ChatContext _dbContext;
    private readonly HistorialChat_Service _historialChatService;

    public ChatHub(ChatContext dbContext, HistorialChat_Service historialChatService)
    {
        _dbContext = dbContext;
        _historialChatService = historialChatService;
    }


    //public async Task SendMessage(Message message)
    //{
    //    await Clients.All.GetMessage(message);
    //}

    public async Task SendMessage(Message message)
    {
        if (!string.IsNullOrEmpty(message.Text))
        {
            if (ContienePalabraProhibida(message.Text))
            {
                string mensajeAnulado = $": Al usuario {message.User} se le ha anulado un mensaje por vocabulario inapropiado";
                await Clients.Client(Context.ConnectionId).GetMessage(new Message() { User = "Sistema", Text = mensajeAnulado });

                // Notificar al cliente que reproduzca el sonido
                await Clients.Client(Context.ConnectionId).SonidoPalabrota();
            }
            else
            {
                try
                {
                    await Clients.Group(message.Room).GetMessage(message);
                    await Clients.All.GetMessage(message);
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Error en SendMessage: {ex.Message}");
                    throw new Exception($"Errorrrrrr en SendMessage: {ex.Message}");
                }

                // esto me crea un objeto para poder guardar estos datos en la bd !!
                _dbContext.Mensajes.Add(new Mensaje
                {
                    Id = message.Id,
                    Nombre = message.User,
                    Texto = message.Text,              
                    Rol = message.Room,
                    FechaMensaje = DateTime.Now

                });
                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Error al guardar en la base de datos: {ex.Message}");
                    throw new Exception($"Errorrrrrr al guardar en la base de datos: {ex.Message}");
                }



            }
        }
        else if (!string.IsNullOrEmpty(message.User))
        {
            conexiones.Add(new Connection { Id = Context.ConnectionId, User = message.User, Avatar = message.Avatar, Room = message.Room });
            await Groups.AddToGroupAsync(Context.ConnectionId, message.Room);
            // await Clients.Group(message.Room).GetMessage(new Message() { User = message.User, Text = " se ha conectado!", Avatar = message.Avatar });
            await Clients.All.GetUsers(conexiones);
        }
    }






    public List<Mensaje> ObtenerHistorialChat(string sala)
    {
        try
        {
            return _historialChatService.GetHistorialChat(sala);
        }
        catch (Exception ex)
        {
            throw new Exception($"Errorrrrrr al obtener el historial del chat: {ex.Message}");
        }
    }




    public async Task ConnectUser(Message message)
    {
        await ConectarUsuario(new Message 
        { 
            User = message.User, 
            Room = message.Room, 
            Avatar = message.Avatar, 
            Text= message.Text,
            Id = message.Id,
            Fecha = message.Fecha,
        });
    }



    //public async Task ChangeRoom(string newRoom)
    //{
    //    var userConnection = conexiones.FirstOrDefault(c => c.Id == Context.ConnectionId);

    //    if (userConnection != null)
    //    {
    //        await Groups.RemoveFromGroupAsync(Context.ConnectionId, userConnection.Room);
    //        userConnection.Room = newRoom;
    //        await Groups.AddToGroupAsync(Context.ConnectionId, newRoom);
    //        await Clients.Group(newRoom).GetMessage(new Message() { User = userConnection.User, Text = " se ha cambiado a la sala " + newRoom, Avatar = userConnection.Avatar });
    //    }
    //}




    private async Task TransmitirMensaje(Message message)
    {
        await Clients.Group(message.Room).GetMessage(message);
    }




    private async Task ConectarUsuario(Message message)
    {
        conexiones.Add(new Connection { Id = Context.ConnectionId, User = message.User, Avatar = message.Avatar, Room = message.Room });
        await Groups.AddToGroupAsync(Context.ConnectionId, message.Room);
        await Clients.Group(message.Room).GetMessage(new Message() { User = message.User, Text = " se ha conectado!", Avatar = message.Avatar });
        await Clients.All.GetUsers(conexiones);
    }



    public override async Task OnConnectedAsync()
    {
        await Clients.Client(Context.ConnectionId).GetMessage(new Message() { User = "Host", Text = "Hola, Bienvenido al Chat" });
        await base.OnConnectedAsync();
    }




    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var conexion = conexiones.FirstOrDefault(x => x.Id == Context.ConnectionId);

        if (conexion != null)
        {
            await Clients.GroupExcept(conexion.Room, Context.ConnectionId).GetMessage(new Message() { User = "Host", Text = $"{conexion.User} ha salido del chat" });
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conexion.Room);
            conexiones.Remove(conexion);
            await Clients.All.GetUsers(conexiones);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private bool ContienePalabraProhibida(string mensaje)
    {
        foreach (var palabraProhibida in palabrasProhibidas)
        {
            if (mensaje.Contains(palabraProhibida, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return base.ToString();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }





    //+ copia del send funcional sin historial del chat

    //public async Task SendMessage(Message message)
    //{
    //    if (!string.IsNullOrEmpty(message.Text))
    //    {
    //        if (ContienePalabraProhibida(message.Text))
    //        {
    //            string mensajeAnulado = $": Al usuario {message.User} se le ha anulado un mensaje por vocabulario inapropiado";
    //            await Clients.Client(Context.ConnectionId).GetMessage(new Message() { User = "Sistema", Text = mensajeAnulado });
    //            await Clients.Client(Context.ConnectionId).SonidoPalabrota();
    //        }
    //        else
    //        {
    //            using (var dbContext = new ChatContext())
    //            {
    //                dbContext.Messages.Add(new Mensaje
    //                {
    //                    Nombre = message.User,
    //                    Texto = message.Text,
    //                    Avatar = message.Avatar,
    //                    Room = message.Room,
    //                    FechaMensaje = message.Fecha,
    //                });

    //                await dbContext.SaveChangesAsync();
    //            }

    //            // Transmitir el mensaje al cliente y a todos los clientes conectados en la sala
    //            await Clients.Group(message.Room).GetMessage(message);
    //            await Clients.All.GetMessage(message);

    //        }
    //    }
    //    else if (!string.IsNullOrEmpty(message.User))
    //    {
    //        conexiones.Add(new Connection { Id = Context.ConnectionId, User = message.User, Avatar = message.Avatar, Room = message.Room });
    //        await Groups.AddToGroupAsync(Context.ConnectionId, message.Room);
    //        await Clients.All.GetUsers(conexiones);
    //    }


    //}

}