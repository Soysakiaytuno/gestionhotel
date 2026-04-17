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

        // ==========================================
        // DTOs (Data Transfer Objects)
        // ==========================================
        public record RespuestaHabitacionDisponible(int IdHabitacion, string Numero, string Tipo, int Capacidad, decimal PrecioNoche);

        // ==========================================
        // RF01: Consulta de Disponibilidad
        // GET: api/habitaciones/disponibles?ingreso=2023-10-01&salida=2023-10-05
        // ==========================================
        [HttpGet("disponibles")]
        public async Task<IActionResult> ObtenerDisponibles([FromQuery] DateTime ingreso, [FromQuery] DateTime salida)
        {
            if (ingreso >= salida)
            {
                return BadRequest(new { error = "La fecha de salida debe ser mayor a la fecha de ingreso." });
            }

            var habitaciones = await _repositorioHabitacion.ObtenerDisponiblesAsync(ingreso, salida);

            // Mapeamos las entidades de la base de datos a un JSON limpio para el Frontend
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