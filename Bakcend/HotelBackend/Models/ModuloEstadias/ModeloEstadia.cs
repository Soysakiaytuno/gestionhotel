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
        public const string EstadoProgramada = "Programada";
        public const string EstadoEnCurso = "En Curso";
        public const string EstadoFinalizada = "Finalizada";
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
        public static void ValidarFechaEstadia(DateTime ingreso, DateTime salida)
        {
            if (salida <= ingreso)
            {
                throw new ArgumentException("La fecha de salida no puede ser mayor a la de ingreso");
            }
        }
        public static Estadia CrearNuevaReserva(DateTime ingreso, DateTime salida)
        {
            ValidarFechaEstadia(ingreso, salida);
            return new Estadia(0, ingreso, salida, null, null, EstadoProgramada, null, null);
        }

        public static Estadia CargarDesdeBd(int id, DateTime ing, DateTime sal, DateTime? inReal, DateTime? outReal, string est, int? dias, decimal? monto)
        {
            return new Estadia(id, ing, sal, inReal, outReal, est, dias, monto);
        }

        public void MarcarCheckIn()
        {
            if (Estado != EstadoProgramada)
            {
                throw new InvalidOperationException("Solo se puede hacer Check-In a una estadía programada.");   
            }
            FechaCheckInReal = DateTime.Now;
            Estado = EstadoEnCurso;
        }
        private void Validaciones()
        {
            if (Estado == EstadoFinalizada)
            {
                throw new InvalidOperationException("La estadía ya ha sido finalizada.");
            }
            if (Estado != EstadoEnCurso)
            {
                throw new Exception("El huésped debe haber hecho Check-In primero.");
            }
            if (!FechaCheckInReal.HasValue)
            {
                throw new Exception("No existe una fecha de Check-In válida.");
            }
        }

        public void MarcarCheckOut(decimal precioTotalPorNoche)
        {
            Validaciones();
            FechaCheckOutReal = DateTime.Now;
            Estado = EstadoFinalizada;
            MontoTotal = new Cobro().calcularCobro(this, precioTotalPorNoche);
        }

    }
}