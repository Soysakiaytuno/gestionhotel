namespace HotelBackend.Models.ModuloEstadias
{
    public interface CobroBase
    {
        public decimal calcularCobro(Estadia estadia, decimal precioTotal);
    }
}