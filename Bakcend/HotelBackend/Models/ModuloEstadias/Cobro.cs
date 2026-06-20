namespace HotelBackend.Models.ModuloEstadias
{
public class Cobro: CobroBase
{
    private void validacionCobro(Estadia estadia)
    {
        if(estadia.FechaCheckOutReal.Value.Date < estadia.FechaCheckInReal.Value.Date)
        {
            throw new ArgumentException("La fecha de Check-Out real no puede ser anterior a la de Check-In real.");
        }
    }
    public decimal calcularCobro(Estadia estadia, decimal precioTotal)
    {   
        if(estadia.FechaCheckOutReal == null || estadia.FechaCheckInReal == null)
        {
            throw new Exception("No se han registrado las fechas de Check-In y Check-Out.");
        }
        validacionCobro(estadia);
        var diferencia = (estadia.FechaCheckOutReal.Value.Date - estadia.FechaCheckInReal.Value.Date).Days;
        int DiasCobrados = diferencia < 1 ? 1 : diferencia;
        decimal MontoTotal = DiasCobrados * precioTotal;
        return MontoTotal;
    }
}
}