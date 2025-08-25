using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ApiPeliculas.Controllers
{
    [ResponseCache(CacheProfileName = "PorDefecto30Segundos")]
    [Route("api/v{version:apiVersion}/usuarios")]
    [ApiController]
    //[ApiVersion("1.0")] // Versión 1.0 de la API
    [ApiVersionNeutral]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usRepo;
        protected RespuestaAPI _respuestaApi;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsuariosController(IUsuarioRepositorio usRepo, IMapper mapper, IConfiguration configuration)
        {
            _usRepo = usRepo;
            _mapper = mapper;
            _configuration = configuration;
            this._respuestaApi = new();
        }

        [Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _usRepo.GetUsuarios();

            var listaUsuariosDto = new List<UsuarioDto>();

            foreach (var list in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map<UsuarioDto>(list));
            }
            return Ok(listaUsuariosDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{usuarioId}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult GetUsuario(string usuarioId)
        {
            // Verificar si el usuario está autenticado
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Usuario no autenticado");
            }

            // Verificar si el usuario tiene el rol Admin
            if (!User.IsInRole("Admin"))
            {
                return Forbid("Usuario no tiene permisos de administrador");
            }

            var itemUsuario = _usRepo.GetUsuarios(usuarioId);

            if (itemUsuario == null)
            {
                return NotFound();
            }

            var itemUsuarioDto = _mapper.Map<UsuarioDto>(itemUsuario);

            return Ok(itemUsuarioDto);
        }

        
        [AllowAnonymous]
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto)
        {
            bool validarNombreUsuarioUnico = _usRepo.IsUniqueUser(usuarioRegistroDto.NombreUsuario);
            if (!validarNombreUsuarioUnico)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.isSuccess = false;
                _respuestaApi.ErrorMessage.Add("El nombre de usuario ya existe");
                return BadRequest(_respuestaApi);
            }

            try
            {
                var usuario = await _usRepo.Registro(usuarioRegistroDto);
                if (usuario == null)
                {
                    _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                    _respuestaApi.isSuccess = false;
                    _respuestaApi.ErrorMessage.Add("Error al registrar el usuario");
                    return StatusCode(500, _respuestaApi);
                }

                _respuestaApi.StatusCode = HttpStatusCode.OK;
                _respuestaApi.isSuccess = true;
                _respuestaApi.Result = "Usuario registrado con éxito";
                return Ok(_respuestaApi);
            }
            catch (InvalidOperationException ex)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.isSuccess = false;
                _respuestaApi.ErrorMessage.Add(ex.Message);
                return BadRequest(_respuestaApi);
            }
        }


        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLoginDto)
        {
            var respuestaLogin = await _usRepo.Login(usuarioLoginDto);
            if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.isSuccess = false;
                _respuestaApi.ErrorMessage.Add("El nombre de usuario o password son incorrectos");
                return BadRequest(_respuestaApi);
            }
            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.isSuccess = true;
            _respuestaApi.Result = respuestaLogin;
            return Ok(_respuestaApi);
        }
    }
}

