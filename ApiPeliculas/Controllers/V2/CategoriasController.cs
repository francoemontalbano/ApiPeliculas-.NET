using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V2
{

    //[ResponseCache(Duration = 20)]
    [ResponseCache(CacheProfileName = "PorDefecto30Segundos")]
    [Route("api/v{version:apiVersion}/[controller]")] // Opción estática
    //[Authorize(Roles = "Admin")]
    //[Route("api/[categorias]")] // Opción dinámica, permite cambiar el nombre del controlador sin afectar las rutas
    [ApiController]
    //[EnableCors("PoliticaCors")] // Habilita CORS para todo el controlador
    [ApiVersion("2.0")] // Versión 1.0 de la API

    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;
        public CategoriasController(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        [HttpGet("GetString")]
        //[MapToApiVersion("2.0")]
        public IEnumerable<string> Get()
        {
            return new string[] { "jose", "render2web" };
        }
    }
}
