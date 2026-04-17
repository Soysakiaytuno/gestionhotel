using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using HotelBackend.Models.ModuloHabitaciones;

namespace HotelBackend.Repository
{
    public class RepositorioHabitacion
    {
        private readonly string _cadenaConexion;

        public RepositorioHabitacion(IConfiguration configuracion)
        {
            _cadenaConexion = configuracion.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<Habitacion>> ObtenerDisponiblesAsync(DateTime fechaIngreso, DateTime fechaSalida)
        {
            var disponibles = new List<Habitacion>();

            string sql = @"
                SELECT h.id_habitacion, h.numero_habitacion, h.id_estado,
                       t.id_tipo_habitacion, t.nombre AS tipo_nombre, t.capacidad_maxima, t.precio_base_noche
                FROM Habitacion h
                INNER JOIN TipoHabitacion t ON h.id_tipo_habitacion = t.id_tipo_habitacion
                WHERE h.id_estado = 1
                AND h.id_habitacion NOT IN (
                    SELECT eh.id_habitacion
                    FROM Estadia e
                    INNER JOIN Estadia_Habitacion eh ON e.id_estadia = eh.id_estadia
                    WHERE e.estado IN ('Programada', 'En Curso')
                      AND e.fecha_ingreso_programada < @FechaSalida 
                      AND e.fecha_salida_programada > @FechaIngreso
                )";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@FechaIngreso", fechaIngreso);
                    comando.Parameters.AddWithValue("@FechaSalida", fechaSalida);

                    await conexion.OpenAsync();
                    
                    using (SqlDataReader lector = await comando.ExecuteReaderAsync())
                    {
                        // Leemos fila por fila y armamos el objeto
                        while (await lector.ReadAsync())
                        {
                            var habitacion = new Habitacion
                            {
                                IdHabitacion = Convert.ToInt32(lector["id_habitacion"]),
                                NumeroHabitacion = lector["numero_habitacion"].ToString()!,
                                IdEstado = Convert.ToInt32(lector["id_estado"]),
                                TipoHabitacion = new TipoHabitacion
                                {
                                    IdTipoHabitacion = Convert.ToInt32(lector["id_tipo_habitacion"]),
                                    Nombre = lector["tipo_nombre"].ToString()!,
                                    CapacidadMaxima = Convert.ToInt32(lector["capacidad_maxima"]),
                                    PrecioBaseNoche = Convert.ToDecimal(lector["precio_base_noche"])
                                }
                            };
                            disponibles.Add(habitacion);
                        }
                    }
                }
            }
            return disponibles;
        }
    }
}