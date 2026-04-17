using System;

namespace HotelBackend.Models.ModuloEstadias
{
    public class Estadia
    {
        public int IdEstadia { get; private set; }
        public DateTime FechaIngresoProgramada { get; private set; }
        public DateTime FechaSalidaProgramada { get; private set; }
        public DateTime? FechaCheckInReal { get; private set; }
        public DateTime? FechaCheckOutReal { get; private set; }
        public string Estado { get; private set; } = null!;
        public int? DiasCobrados { get; private set; }
        public decimal? MontoTotal { get; private set; }

        // Constructor para CREAR una nueva reserva desde cero
        public Estadia(DateTime ingreso, DateTime salida)
        {
            FechaIngresoProgramada = ingreso;
            FechaSalidaProgramada = salida;
            Estado = "Programada";
        }

        // Constructor para RECONSTRUIR el objeto cuando lo leemos de la Base de Datos
        public Estadia(int id, DateTime ing, DateTime sal, DateTime? inReal, DateTime? outReal, string est, int? dias, decimal? monto)
        {
            IdEstadia = id; FechaIngresoProgramada = ing; FechaSalidaProgramada = sal;
            FechaCheckInReal = inReal; FechaCheckOutReal = outReal; Estado = est;
            DiasCobrados = dias; MontoTotal = monto;
        }

        public void MarcarCheckIn()
        {
            if (Estado != "Programada") throw new Exception("Solo se puede hacer Check-In a una estadía programada.");
            FechaCheckInReal = DateTime.Now;
            Estado = "En Curso";
        }

        // Ahora recibe el precio por noche como parámetro, ya no depende de buscar en otras tablas
        public void MarcarCheckOut(decimal precioTotalPorNoche)
        {
            if (Estado != "En Curso") throw new Exception("El huésped debe haber hecho Check-In primero.");
            
            if (!FechaCheckInReal.HasValue) throw new Exception("No existe una fecha de Check-In válida para procesar.");

            FechaCheckOutReal = DateTime.Now;
            Estado = "Finalizada";

            var diferencia = (FechaCheckOutReal.Value.Date - FechaCheckInReal.Value.Date).Days;
            DiasCobrados = diferencia < 1 ? 1 : diferencia;
            MontoTotal = DiasCobrados * precioTotalPorNoche;
        }
    }
}