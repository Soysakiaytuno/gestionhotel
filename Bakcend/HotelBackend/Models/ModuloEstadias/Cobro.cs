namespace HotelBackend.Models.ModuloEstadias
{
public class Cobro: CobroBase
{
    public decimal calcularCobro(Estadia estadia, decimal precioTotal)
    {   
        if(estadia.FechaCheckOutReal == null || estadia.FechaCheckInReal == null)
        {
            throw new Exception("No se han registrado las fechas de Check-In y Check-Out.");
        }
        var diferencia = (estadia.FechaCheckOutReal.Value.Date - estadia.FechaCheckInReal.Value.Date).Days;
        int DiasCobrados = diferencia < 1 ? 1 : diferencia;
        decimal MontoTotal = DiasCobrados * precioTotal;
        return MontoTotal;
    }
}
}