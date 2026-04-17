using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBackend.Models.ModuloHabitaciones
{
    [Table("Habitacion")]
    public class Habitacion
    {
        [Key, Column("id_habitacion")] 
        public int IdHabitacion { get; set; }
        
        [Column("id_hotel")] 
        public int IdHotel { get; set; }
        
        [Column("id_tipo_habitacion")] 
        public int IdTipoHabitacion { get; set; }
        
        [Column("id_estado")] 
        public int IdEstado { get; set; }
        
        [Column("numero_habitacion")] 
        public string NumeroHabitacion { get; set; } = null!;

        // Propiedad de navegación para poder traer el precio
        public virtual TipoHabitacion TipoHabitacion { get; set; } = null!;
    }
}