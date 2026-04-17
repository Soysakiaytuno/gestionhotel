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

        // Constructor privado para obligar el uso de los Factory Methods
        private Estadia(int id, DateTime ing, DateTime sal, DateTime? inReal, DateTime? outReal, string est, int? dias, decimal? monto)
        {
            IdEstadia = id; 
            FechaIngresoProgramada = ing; 
            FechaSalidaProgramada = sal;
            FechaCheckInReal = inReal; 
            FechaCheckOutReal = outReal; 
            Estado = est;
            DiasCobrados = dias; 
            MontoTotal = monto;
        }

        // Factory para CREAR una nueva reserva
        public static Estadia CrearNuevaReserva(DateTime ingreso, DateTime salida)
        {
            return new Estadia(0, ingreso, salida, null, null, "Programada", null, null);
        }

        // Factory para RECONSTRUIR desde la base de datos
        public static Estadia CargarDesdeBd(int id, DateTime ing, DateTime sal, DateTime? inReal, DateTime? outReal, string est, int? dias, decimal? monto)
        {
            return new Estadia(id, ing, sal, inReal, outReal, est, dias, monto);
        }

        public void MarcarCheckIn()
        {
            if (Estado != "Programada") throw new Exception("Solo se puede hacer Check-In a una estadía programada.");
            FechaCheckInReal = DateTime.Now;
            Estado = "En Curso";
        }

        public void MarcarCheckOut(decimal precioTotalPorNoche)
        {
            if (Estado != "En Curso") throw new Exception("El huésped debe haber hecho Check-In primero.");
            if (!FechaCheckInReal.HasValue) throw new Exception("No existe una fecha de Check-In válida.");

            FechaCheckOutReal = DateTime.Now;
            Estado = "Finalizada";

            var diferencia = (FechaCheckOutReal.Value.Date - FechaCheckInReal.Value.Date).Days;
            DiasCobrados = diferencia < 1 ? 1 : diferencia;
            MontoTotal = DiasCobrados * precioTotalPorNoche;
        }
    }
}