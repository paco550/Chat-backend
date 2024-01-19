//using Chat.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace Chat.Controllers
//{

//    [ApiController]
//    [Route("api/[controller]")]
//    public class SalasController : ControllerBase
//    {
//        private readonly HistorialChat_Service _historialChatService;

//        public SalasController(HistorialChat_Service historialChatService)
//        {
//            _historialChatService = historialChatService;
//        }

//        [HttpGet("salas-disponibles")]
//        public async Task<ActionResult> GetSalasDisponibles()
//        {
//            try
//            {
//                var salas = _historialChatService.GetSalasDisponibles();
//                return Ok(salas);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Error al obtener las salas disponibles", ex);
//            }
//        }

//        [HttpGet("historial/{sala}")]
//        public async Task<ActionResult> GetHistorialChat(string sala)
//        {

//            try
//            {
//                var historial = _historialChatService.GetHistorialChat(sala);
//                return Ok(historial);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Error interno del servidor", ex);
//            }
//        }





//    }

//}




using Chat.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SalasController : ControllerBase
{
    private readonly HistorialChat_Service _historialChatService;

    public SalasController(HistorialChat_Service historialChatService)
    {
        _historialChatService = historialChatService;
    }

    [HttpGet("salas-disponibles")]
    public ActionResult<IEnumerable<string>> GetSalasDisponibles()
    {
        try
        {
            var salas = _historialChatService.GetSalasDisponibles();
            return Ok(salas);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener las salas disponibles: {ex.Message}");
          
        }
    }

    [HttpGet("historial/{sala}")]
    public ActionResult<IEnumerable<Message>> GetHistorialChat(string sala)
    {
        try
        {
            var historial = _historialChatService.GetHistorialChat(sala);
            return Ok(historial);
        }
        catch (Exception ex)
        {

            throw new Exception($"Error al obtener el historial del chatttt: {ex.Message}");

        }
    }
}
