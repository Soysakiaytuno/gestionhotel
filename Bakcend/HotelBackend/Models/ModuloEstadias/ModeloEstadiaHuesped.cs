using System.ComponentModel.DataAnnotations.Schema;
using HotelBackend.Models.ModuloUsuarios;

namespace HotelBackend.Models.ModuloEstadias
{
    [Table("Estadia_Huesped")]
    public class EstadiaHuesped
    {
        [Column("id_estadia")] 
        public int IdEstadia { get; set; }
        
        [Column("id_huesped")] 
        public int IdHuesped { get; set; }
        
        [Column("es_titular")] 
        public bool EsTitular { get; set; }

        // Navegación
        public virtual Estadia Estadia { get; set; } = null!;
        public virtual Huesped Huesped { get; set; } = null!;
    }
}