using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using XAct;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(opciones => 
                    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));


//Soporte para autenticación con .NET Identity
builder.Services.AddIdentity<AppUsuario, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();


//Soporte para cache
var apiVersioningBuilder = builder.Services.AddApiVersioning(opcion =>
{
    opcion.AssumeDefaultVersionWhenUnspecified = true; // Asume la versión por defecto si no se especifica
    opcion.DefaultApiVersion = new ApiVersion(1, 0); // Establece la versión por defecto
    opcion.ReportApiVersions = true; // Informa las versiones de la API en las respuestas
    //opcion.ApiVersionReader = ApiVersionReader.Combine(
    //    new QueryStringApiVersionReader("api-version")
    //); 
});

apiVersioningBuilder.AddApiExplorer(

    opciones =>
    {
        opciones.GroupNameFormat = "'v'VVV"; // Formato de versión
        opciones.SubstituteApiVersionInUrl = true; // Sustituye la versión en la URL    
    }
);

//Agregamos los repositorios
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

//Soporte para versionamiento
builder.Services.AddApiVersioning();

//Agregamos el AutoMapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

//Aquí se configura la autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:Secreta"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = ClaimTypes.Role // Esto es importante
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers(opcion =>
{
    //Cache profile. Un cache global y asi no tener que agregarlo a cada controlador
    opcion.CacheProfiles.Add("PorDefecto30Segundos", new CacheProfile() { Duration = 30});

});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description =
            "Autenticacion JWT usando el esquema Bearer. \r\n\r\n " +
            "Ingresa la palabra 'Bearer' seguida de un espacio y despues tu token JWT en el campo de abajo. \r\n\r\n" +
            "Ejemplo: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1.0",
            Title = "Peliculas Api V1",
            Description = "API de películas Versión 1",
            TermsOfService = new Uri("https://render2web.com/promociones"),
            Contact = new OpenApiContact
            {
                Name = "Render2Web",
                Url = new Uri("https://render2web.com/promociones"),
            },
            License = new OpenApiLicense
            {
                Name = "Licencia de uso",
                Url = new Uri("https://render2web.com/promociones"),
            }
        }
    );
        options.SwaggerDoc("v2", new OpenApiInfo
        {
            Version = "v2.0",
            Title = "Peliculas Api V2",
            Description = "API de películas Versión 2",
            TermsOfService = new Uri("https://render2web.com/promociones"),
            Contact = new OpenApiContact
            {
                Name = "Render2Web",
                Url = new Uri("https://render2web.com/promociones"),
            },
            License = new OpenApiLicense
            {
                Name = "Licencia de uso",
                Url = new Uri("https://render2web.com/promociones"),
            }
        }
     );
    }
);

//Soporte para CORS - Configuración simple y funcional
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    build.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader();
}));

// ALTERNATIVA: Si necesitas AllowCredentials (para cookies, headers de autorización)
// builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
// {
//     build.SetIsOriginAllowed(origin => true)
//          .AllowAnyMethod()
//          .AllowAnyHeader()
//          .AllowCredentials();
// }));


var app = builder.Build();
app.UseSwagger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiPeliculas v1");
        opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiPeliculas v2");
    });
}
else
{
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiPeliculas v1");
        opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiPeliculas v2");
        opciones.RoutePrefix = ""; 
    });
}
    //Soporte para archivos estáticos como imagen
    app.UseStaticFiles();
app.UseHttpsRedirection();

//Soporte para CORS - Debe ir antes de Authentication y Authorization
app.UseCors("PoliticaCors");

app.UseAuthentication(); // Habilita la autenticación
app.UseAuthorization();

app.MapControllers();

app.Run();
