using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HotelBackend.Models.ModuloEstadias;
using HotelBackend.Repository;

namespace HotelBackend.Controllers
{
    [ApiController]
    [Route("api/estadias")]
    public class ControladorEstadias : ControllerBase
    {
        private readonly RepositorioEstadia _repositorioEstadia;

        public ControladorEstadias(RepositorioEstadia repoEstadia)
        {
            _repositorioEstadia = repoEstadia;
        }

        public record PeticionCrearEstadia(DateTime Ingreso, DateTime Salida, List<int> IdsHabitaciones, List<int> IdsHuespedes, int IdHuespedTitular);
        public record RespuestaCheckOut(int IdEstadia, int DiasCobrados, decimal TotalCobrado);

        [HttpGet("activas")]
        public async Task<IActionResult> ObtenerActivas()
        {
            var estadias = await _repositorioEstadia.ObtenerActivasAsync();
            return Ok(estadias);
        }

        [HttpPost]
        public async Task<IActionResult> CrearReserva([FromBody] PeticionCrearEstadia peticion)
        {
            if (!peticion.IdsHabitaciones.Any() || !peticion.IdsHuespedes.Any())
                return BadRequest(new { error = "Datos incompletos." });

            var nuevaEstadia = Estadia.CrearNuevaReserva(peticion.Ingreso, peticion.Salida);

            int idGenerado = await _repositorioEstadia.CrearAsync(
                nuevaEstadia, 
                peticion.IdsHabitaciones, 
                peticion.IdsHuespedes, 
                peticion.IdHuespedTitular
            );

            return Ok(new { mensaje = "Reserva programada.", idEstadia = idGenerado });
        }

        [HttpPost("{id}/checkin")]
        public async Task<IActionResult> MarcarCheckIn(int id)
        {
            var (estadia, _) = await _repositorioEstadia.ObtenerPorIdAsync(id);
            if (estadia == null) return NotFound(new { error = "No encontrada." });

            try {
                estadia.MarcarCheckIn(); 
                await _repositorioEstadia.ActualizarAsync(estadia);
                return Ok(new { mensaje = "Check-in exitoso." });
            } catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPost("{id}/checkout")]
        public async Task<IActionResult> MarcarCheckOut(int id)
        {
            var (estadia, precio) = await _repositorioEstadia.ObtenerPorIdAsync(id);
            if (estadia == null) return NotFound(new { error = "No encontrada." });

            try {
                estadia.MarcarCheckOut(precio); 
                await _repositorioEstadia.ActualizarAsync(estadia);
                return Ok(new RespuestaCheckOut(estadia.IdEstadia, estadia.DiasCobrados ?? 0, estadia.MontoTotal ?? 0));
            } catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        }
    }
}