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
    [Route("api/[controller]")]
    public class ControladorEstadias : ControllerBase
    {
        private readonly RepositorioEstadia _repositorioEstadia;

        // Ya no necesitamos inyectar los otros repositorios porque el RepositorioEstadia ahora hace todo el trabajo en SQL
        public ControladorEstadias(RepositorioEstadia repoEstadia)
        {
            _repositorioEstadia = repoEstadia;
        }

        public record PeticionCrearEstadia(DateTime Ingreso, DateTime Salida, List<int> IdsHabitaciones, List<int> IdsHuespedes, int IdHuespedTitular);
        public record RespuestaCheckOut(int IdEstadia, int DiasCobrados, decimal TotalCobrado);

        [HttpGet("activas")]
        public async Task<IActionResult> ObtenerActivas()
        {
            // El repositorio en SQL Puro ya nos devuelve el DTO exacto que necesitamos para el dashboard
            var estadias = await _repositorioEstadia.ObtenerActivasAsync();
            return Ok(estadias);
        }

        [HttpPost]
        public async Task<IActionResult> CrearReserva([FromBody] PeticionCrearEstadia peticion)
        {
            if (!peticion.IdsHabitaciones.Any() || !peticion.IdsHuespedes.Any() || !peticion.IdsHuespedes.Contains(peticion.IdHuespedTitular))
            {
                return BadRequest(new { error = "Datos incompletos. Debe haber al menos una habitación y un huésped titular válido." });
            }

            var nuevaEstadia = new Estadia(peticion.Ingreso, peticion.Salida);

            // Delegamos la creación al repositorio que usa una Transacción SQL para guardar todo de forma segura
            int idGenerado = await _repositorioEstadia.CrearAsync(
                nuevaEstadia, 
                peticion.IdsHabitaciones, 
                peticion.IdsHuespedes, 
                peticion.IdHuespedTitular
            );

            return Ok(new { mensaje = "Reserva programada con éxito.", idEstadia = idGenerado });
        }

        [HttpPost("{id}/checkin")]
        public async Task<IActionResult> MarcarCheckIn(int id)
        {
            // El nuevo repositorio devuelve una tupla: (estadia, precioPorNoche)
            var resultado = await _repositorioEstadia.ObtenerPorIdAsync(id);
            
            if (resultado.estadia == null) return NotFound(new { error = "Estadía no encontrada." });

            try
            {
                resultado.estadia.MarcarCheckIn(); 
                
                // Usamos el nuevo método de actualización SQL directa
                await _repositorioEstadia.ActualizarAsync(resultado.estadia);
                
                return Ok(new { mensaje = "Check-in realizado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/checkout")]
        public async Task<IActionResult> MarcarCheckOut(int id)
        {
            var resultado = await _repositorioEstadia.ObtenerPorIdAsync(id);
            
            if (resultado.estadia == null) return NotFound(new { error = "Estadía no encontrada." });

            try
            {
                // Ahora le pasamos el precio total por noche que calculó el SQL al Modelo Rico
                resultado.estadia.MarcarCheckOut(resultado.precioPorNoche); 
                
                await _repositorioEstadia.ActualizarAsync(resultado.estadia);
                
                return Ok(new RespuestaCheckOut(
                    resultado.estadia.IdEstadia, 
                    resultado.estadia.DiasCobrados ?? 0, 
                    resultado.estadia.MontoTotal ?? 0
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}