using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _bd;
        private string claveSecreta;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UsuarioRepositorio(ApplicationDbContext bd, IConfiguration config, UserManager<AppUsuario> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _bd = bd;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

       
        public AppUsuario GetUsuarios(string usuarioId)
        {
            return _bd.AppUsuario.FirstOrDefault(c => c.Id == usuarioId);
        }

        public ICollection<AppUsuario> GetUsuarios()
        {
            return _bd.AppUsuario.OrderBy(c => c.UserName).ToList();
        }

        public bool IsUniqueUser(string usuario)
        {
            var usuarioBd = _bd.AppUsuario.FirstOrDefault(u => u.UserName == usuario);
            if (usuarioBd == null)
            {
                return true; // Usuario único
            }
            return false; // Usuario ya existe
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            var usuario = _bd.AppUsuario.FirstOrDefault(
                u => u.UserName.ToLower() == usuarioLoginDto.NombreUsuario.ToLower());

            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = string.Empty,
                    Usuario = null
                };
            }

            bool isValid = await _userManager.CheckPasswordAsync(usuario, usuarioLoginDto.Password);

            if (!isValid)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = string.Empty,
                    Usuario = null
                };
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            var manejadorToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, usuario.UserName),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = manejadorToken.CreateToken(tokenDescriptor);

            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
               Token = manejadorToken.WriteToken(token),
               Usuario = _mapper.Map<UsuarioDatosDto>(usuario),
            };

            return usuarioLoginRespuestaDto;
        }

        public async Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            AppUsuario usuario = new AppUsuario()
            {
                UserName = usuarioRegistroDto.NombreUsuario,
                Email = usuarioRegistroDto.Email,
                NormalizedEmail = usuarioRegistroDto.Email?.ToUpper(),
                Nombre = usuarioRegistroDto.Nombre,
            };

            var result = await _userManager.CreateAsync(usuario, usuarioRegistroDto.Password);
            if (result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Registrado"));
                }

                await _userManager.AddToRoleAsync(usuario, "Registrado");
                var usuarioRetornado = _bd.AppUsuario.FirstOrDefault(u => u.UserName == usuarioRegistroDto.NombreUsuario);
                return _mapper.Map<UsuarioDatosDto>(usuarioRetornado);
            }

            // Manejo de errores comunes de Identity (email/usuario duplicado, etc.)
            var duplicateEmail = result.Errors.FirstOrDefault(e => e.Code.Equals("DuplicateEmail", StringComparison.OrdinalIgnoreCase));
            if (duplicateEmail != null)
            {
                throw new InvalidOperationException("El email ya está en uso por otro usuario.");
            }
            var duplicateUser = result.Errors.FirstOrDefault(e => e.Code.Equals("DuplicateUserName", StringComparison.OrdinalIgnoreCase));
            if (duplicateUser != null)
            {
                throw new InvalidOperationException("El nombre de usuario ya está en uso.");
            }

            // Mensaje genérico si no se reconoce la causa
            throw new InvalidOperationException("No se pudo registrar el usuario.");
        }
    }
}
