using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HotelBackend.Repository;

namespace HotelBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HuespedesController : ControllerBase
    {
        private readonly RepositorioHuesped _repositorioHuesped;

        public HuespedesController(RepositorioHuesped repositorioHuesped)
        {
            _repositorioHuesped = repositorioHuesped;
        }

        public record RespuestaBusquedaHuesped(int IdHuesped, string Documento, string NombreCompleto, string Telefono);

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
            {
                return BadRequest(new { error = "Debe ingresar un documento o nombre para buscar." });
            }

            var huespedes = await _repositorioHuesped.BuscarPorTerminoAsync(termino);

            if (!huespedes.Any())
            {
                return NotFound(new { mensaje = "Usuario no encontrado." }); // Regla estricta: Mostrar aviso si no existe
            }

            var respuesta = huespedes.Select(h => new RespuestaBusquedaHuesped(
                h.IdHuesped,
                h.Usuario.DocumentoIdentidad,
                $"{h.Usuario.Nombre} {h.Usuario.Apellido}",
                h.Usuario.Telefono ?? "Sin registro"
            ));

            return Ok(respuesta);
        }
    }
}