using System.ComponentModel.DataAnnotations.Schema;
using HotelBackend.Models.ModuloHabitaciones;

namespace HotelBackend.Models.ModuloEstadias
{
    [Table("Estadia_Habitacion")]
    public class EstadiaHabitacion
    {
        [Column("id_estadia")] 
        public int IdEstadia { get; set; }
        
        [Column("id_habitacion")] 
        public int IdHabitacion { get; set; }
        public virtual Estadia Estadia { get; set; } = null!;
        public virtual Habitacion Habitacion { get; set; } = null!;
    }
}