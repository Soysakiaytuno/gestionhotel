using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using HotelBackend.Models.ModuloUsuarios;

namespace HotelBackend.Repository
{
    public class RepositorioHuesped
    {
        private readonly string _cadenaConexion;

        public RepositorioHuesped(IConfiguration configuracion)
        {
            _cadenaConexion = configuracion.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<Huesped>> BuscarPorTerminoAsync(string termino)
        {
            var huespedes = new List<Huesped>();

            string sql = @"
                SELECT h.id_huesped, u.id_usuario, u.documento_identidad, u.nombre, u.apellido, u.telefono
                FROM Huesped h
                INNER JOIN Usuario u ON h.id_usuario = u.id_usuario
                WHERE u.documento_identidad LIKE '%' + @Termino + '%'
                   OR u.nombre LIKE '%' + @Termino + '%'
                   OR u.apellido LIKE '%' + @Termino + '%'
            ";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Termino", termino);

                    await conexion.OpenAsync();
                    
                    using (SqlDataReader lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            var huesped = new Huesped
                            {
                                IdHuesped = Convert.ToInt32(lector["id_huesped"]),
                                IdUsuario = Convert.ToInt32(lector["id_usuario"]),
                                Usuario = new Usuario
                                {
                                    IdUsuario = Convert.ToInt32(lector["id_usuario"]),
                                    DocumentoIdentidad = lector["documento_identidad"].ToString()!,
                                    Nombre = lector["nombre"].ToString()!,
                                    Apellido = lector["apellido"].ToString()!,
                                    Telefono = lector["telefono"] != DBNull.Value ? lector["telefono"].ToString() : null
                                }
                            };
                            huespedes.Add(huesped);
                        }
                    }
                }
            }
            return huespedes;
        }
    }
}