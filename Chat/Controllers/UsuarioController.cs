using Chat.DTOs;
using Chat.Models;
using Chat.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private readonly ChatContext _context;
        private readonly HashService _hashService;
        private readonly TokenService _tokenService;


        public UsuarioController(ChatContext context, HashService hashService, TokenService tokenService)
        {
            _context = context;
            _hashService = hashService;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<ActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios);
        }


        #region HASH

        [HttpPost("hash/nuevousuario")]
        public async Task<ActionResult> PostNuevoUsuarioHash([FromBody] DTOUsuario usuario)
        {

            var existeUsuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (existeUsuario != null)
            {
                return BadRequest("El email ya está en uso.");
            }
            var existeUsuario2 = await _context.Usuarios.FirstOrDefaultAsync(x => x.Nombre == usuario.Nombre);
            if (existeUsuario2 != null)
            {
                return BadRequest("El nombre ya está en uso.");
            }
            var resultadoHash = _hashService.Hash(usuario.Password);
            var newUsuario = new Usuario
            {
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Password = resultadoHash.Hash,
                Salt = resultadoHash.Salt,
                Rol = usuario.Rol
            };

            await _context.Usuarios.AddAsync(newUsuario);
            await _context.SaveChangesAsync();

            return Ok(newUsuario);
        }


        [HttpPost("hash/checkusuario")]
        public async Task<ActionResult> CheckUsuarioHash([FromBody] DTOUsuario usuario)
        {
            var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (usuarioDB == null)
            {
                return Unauthorized("usuario no existe");
            }

            var resultadoHash = _hashService.Hash(usuario.Password, usuarioDB.Salt);
            if (usuarioDB.Password == resultadoHash.Hash)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }

        }

        #endregion

        #region LOGIN

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] DTOUsuario usuario)
        {
            var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (usuarioDB == null)
            {
                return BadRequest();
            }

            var resultadoHash = _hashService.Hash(usuario.Password, usuarioDB.Salt);
            if (usuarioDB.Password == resultadoHash.Hash)
            {
                // Si el login es exitoso devolvemos el token y el email (DTOLoginResponse) 
                var response = _tokenService.GenerarToken(usuarioDB); // aqui devolvemos el usuario de la base de datos, con el rol, nombre, etc
                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }




        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUsuarios(int id, [FromBody] DTOUsuario usuario)
        {
            var usuarioDB = await _context.Usuarios.FindAsync(id);

            if (usuarioDB == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Actualizar propiedades del usuario según los datos recibidos en el DTO
            usuarioDB.Nombre = usuario.Nombre;
            usuarioDB.Email = usuario.Email;
            usuarioDB.Rol = usuario.Rol;

            _context.Entry(usuarioDB).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(usuarioDB);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Error al intentar actualizar el usuario");
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUsuarios(int id)
        {
            var usuarioDB = await _context.Usuarios.FindAsync(id);

            if (usuarioDB == null)
            {
                return NotFound("Usuario no encontrado");
            }

            _context.Usuarios.Remove(usuarioDB);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        #endregion
    }
}
