using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBackend.Models.ModuloUsuarios
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key, Column("id_usuario")] 
        public int IdUsuario { get; set; }
        
        [Column("documento_identidad")] 
        public string DocumentoIdentidad { get; set; } = null!;
        
        [Column("nombre")] 
        public string Nombre { get; set; } = null!;
        
        [Column("apellido")] 
        public string Apellido { get; set; } = null!;
        
        [Column("telefono")] 
        public string? Telefono { get; set; }
    }
}