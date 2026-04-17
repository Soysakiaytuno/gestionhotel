using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HotelBackend.Repository;

namespace HotelBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HabitacionesController : ControllerBase
    {
        private readonly RepositorioHabitacion _repositorioHabitacion;

        public HabitacionesController(RepositorioHabitacion repositorioHabitacion)
        {
            _repositorioHabitacion = repositorioHabitacion;
        }

        public record RespuestaHabitacionDisponible(int IdHabitacion, string Numero, string Tipo, int Capacidad, decimal PrecioNoche);

        [HttpGet("disponibles")]
        public async Task<IActionResult> ObtenerDisponibles([FromQuery] DateTime ingreso, [FromQuery] DateTime salida)
        {
            if (ingreso >= salida)
            {
                return BadRequest(new { error = "La fecha de salida debe ser mayor a la fecha de ingreso." });
            }

            var habitaciones = await _repositorioHabitacion.ObtenerDisponiblesAsync(ingreso, salida);

            var respuesta = habitaciones.Select(h => new RespuestaHabitacionDisponible(
                h.IdHabitacion,
                h.NumeroHabitacion,
                h.TipoHabitacion.Nombre,
                h.TipoHabitacion.CapacidadMaxima,
                h.TipoHabitacion.PrecioBaseNoche
            ));

            return Ok(respuesta);
        }
    }
}