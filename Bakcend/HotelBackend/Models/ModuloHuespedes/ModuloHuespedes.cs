using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBackend.Models.ModuloUsuarios
{
    [Table("Huesped")]
    public class Huesped
    {
        [Key, Column("id_huesped")] 
        public int IdHuesped { get; set; }
        
        [Column("id_usuario")] 
        public int IdUsuario { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;
    }
}