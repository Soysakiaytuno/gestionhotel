using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBackend.Models.ModuloHabitaciones
{
    [Table("TipoHabitacion")]
    public class TipoHabitacion
    {
        [Key, Column("id_tipo_habitacion")] 
        public int IdTipoHabitacion { get; set; }
        
        [Column("nombre")] 
        public string Nombre { get; set; } = null!;
        
        [Column("capacidad_maxima")] 
        public int CapacidadMaxima { get; set; }
        
        [Column("precio_base_noche")] 
        public decimal PrecioBaseNoche { get; set; }
    }
}