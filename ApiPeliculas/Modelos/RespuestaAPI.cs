using System.Net;

namespace ApiPeliculas.Modelos
{
    public class RespuestaAPI
    {
        public RespuestaAPI()
        {
            ErrorMessage = new List<string>();
        }

        public HttpStatusCode StatusCode { get; set; }
        public bool isSuccess { get; set; } = true;
        public List<string> ErrorMessage { get; set; }
        public object Result { get; set; }
    }
}
