using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using HotelBackend.Models.ModuloEstadias;

namespace HotelBackend.Repository
{
    // DTO específico para mandar los datos planos al panel de la web
    public record EstadiaDashboardDto(int IdEstadia, string Estado, DateTime Ingreso, DateTime Salida, string Titular, string Habitaciones);

    public class RepositorioEstadia
    {
        private readonly string _cadenaConexion;

        public RepositorioEstadia(IConfiguration configuracion)
        {
            _cadenaConexion = configuracion.GetConnectionString("DefaultConnection")!;
        }

        // RF04: Obtener Panel
        public async Task<List<EstadiaDashboardDto>> ObtenerActivasAsync()
        {
            var lista = new List<EstadiaDashboardDto>();
            string sql = @"
                SELECT 
                    e.id_estadia, e.estado, e.fecha_ingreso_programada, e.fecha_salida_programada,
                    (SELECT TOP 1 u.nombre + ' ' + u.apellido FROM Estadia_Huesped eh INNER JOIN Huesped h ON eh.id_huesped = h.id_huesped INNER JOIN Usuario u ON h.id_usuario = u.id_usuario WHERE eh.id_estadia = e.id_estadia AND eh.es_titular = 1) AS Titular,
                    (SELECT STRING_AGG(ha.numero_habitacion, ', ') FROM Estadia_Habitacion eha INNER JOIN Habitacion ha ON eha.id_habitacion = ha.id_habitacion WHERE eha.id_estadia = e.id_estadia) AS Habitaciones
                FROM Estadia e
                WHERE e.estado IN ('Programada', 'En Curso')
                ORDER BY e.fecha_ingreso_programada";

            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                await conexion.OpenAsync();
                using (var comando = new SqlCommand(sql, conexion))
                using (var lector = await comando.ExecuteReaderAsync())
                {
                    while (await lector.ReadAsync())
                    {
                        lista.Add(new EstadiaDashboardDto(
                            lector.GetInt32(lector.GetOrdinal("id_estadia")),
                            lector.GetString(lector.GetOrdinal("estado")),
                            lector.GetDateTime(lector.GetOrdinal("fecha_ingreso_programada")),
                            lector.GetDateTime(lector.GetOrdinal("fecha_salida_programada")),
                            lector.IsDBNull(lector.GetOrdinal("Titular")) ? "Sin Titular" : lector.GetString(lector.GetOrdinal("Titular")),
                            lector.IsDBNull(lector.GetOrdinal("Habitaciones")) ? "" : lector.GetString(lector.GetOrdinal("Habitaciones"))
                        ));
                    }
                }
            }
            return lista;
        }

        // Retorna la Estadía reconstruida y, por separado, cuánto cuestan las habitaciones
        public async Task<(Estadia? estadia, decimal precioPorNoche)> ObtenerPorIdAsync(int id)
        {
            Estadia? estadia = null;
            decimal precioPorNoche = 0;

            string sql = @"
                SELECT e.*, 
                       ISNULL((SELECT SUM(th.precio_base_noche) FROM Estadia_Habitacion eh 
                               INNER JOIN Habitacion h ON eh.id_habitacion = h.id_habitacion 
                               INNER JOIN TipoHabitacion th ON h.id_tipo_habitacion = th.id_tipo_habitacion 
                               WHERE eh.id_estadia = e.id_estadia), 0) AS precio_total_noche
                FROM Estadia e WHERE e.id_estadia = @id";

            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                await conexion.OpenAsync();
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        if (await lector.ReadAsync())
                        {
                            precioPorNoche = lector.GetDecimal(lector.GetOrdinal("precio_total_noche"));
                            
                            // Reconstruimos la Estadía usando el constructor de 8 argumentos que configuraste
                            estadia = new Estadia(
                                lector.GetInt32(lector.GetOrdinal("id_estadia")),
                                lector.GetDateTime(lector.GetOrdinal("fecha_ingreso_programada")),
                                lector.GetDateTime(lector.GetOrdinal("fecha_salida_programada")),
                                lector.IsDBNull(lector.GetOrdinal("fecha_check_in_real")) ? (DateTime?)null : lector.GetDateTime(lector.GetOrdinal("fecha_check_in_real")),
                                lector.IsDBNull(lector.GetOrdinal("fecha_check_out_real")) ? (DateTime?)null : lector.GetDateTime(lector.GetOrdinal("fecha_check_out_real")),
                                lector.GetString(lector.GetOrdinal("estado")),
                                lector.IsDBNull(lector.GetOrdinal("dias_cobrados")) ? (int?)null : lector.GetInt32(lector.GetOrdinal("dias_cobrados")),
                                lector.IsDBNull(lector.GetOrdinal("monto_total")) ? (decimal?)null : lector.GetDecimal(lector.GetOrdinal("monto_total"))
                            );
                        }
                    }
                }
            }
            return (estadia, precioPorNoche);
        }

        // RF03: Crear usando TRANSACCIÓN SQL
        public async Task<int> CrearAsync(Estadia estadia, List<int> idsHab, List<int> idsHuespedes, int idTitular)
        {
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                await conexion.OpenAsync();
                using (var transaccion = conexion.BeginTransaction()) // Si algo falla, se revierte todo
                {
                    try
                    {
                        // 1. Insertar Estadía y recuperar el ID generado
                        string sqlInsertEstadia = "INSERT INTO Estadia (fecha_ingreso_programada, fecha_salida_programada, estado) OUTPUT INSERTED.id_estadia VALUES (@ingreso, @salida, @estado)";
                        int idNuevaEstadia;
                        using (var cmd = new SqlCommand(sqlInsertEstadia, conexion, transaccion))
                        {
                            cmd.Parameters.AddWithValue("@ingreso", estadia.FechaIngresoProgramada);
                            cmd.Parameters.AddWithValue("@salida", estadia.FechaSalidaProgramada);
                            cmd.Parameters.AddWithValue("@estado", estadia.Estado);
                            
                            // SOLUCIÓN AL WARNING: Convert.ToInt32 maneja correctamente la posibilidad de nulos/objetos
                            idNuevaEstadia = Convert.ToInt32(await cmd.ExecuteScalarAsync()); 
                        }

                        // 2. Insertar Habitaciones
                        string sqlInsertHab = "INSERT INTO Estadia_Habitacion (id_estadia, id_habitacion) VALUES (@idE, @idH)";
                        foreach (var id in idsHab)
                        {
                            using (var cmd = new SqlCommand(sqlInsertHab, conexion, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@idE", idNuevaEstadia);
                                cmd.Parameters.AddWithValue("@idH", id);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        // 3. Insertar Huéspedes
                        string sqlInsertHues = "INSERT INTO Estadia_Huesped (id_estadia, id_huesped, es_titular) VALUES (@idE, @idHues, @titular)";
                        foreach (var id in idsHuespedes)
                        {
                            using (var cmd = new SqlCommand(sqlInsertHues, conexion, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@idE", idNuevaEstadia);
                                cmd.Parameters.AddWithValue("@idHues", id);
                                cmd.Parameters.AddWithValue("@titular", id == idTitular ? 1 : 0);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        transaccion.Commit(); // Confirmamos los cambios
                        return idNuevaEstadia;
                    }
                    catch
                    {
                        transaccion.Rollback(); // Si hubo error, cancelamos todo
                        throw;
                    }
                }
            }
        }

        // Actualización directa
        public async Task ActualizarAsync(Estadia estadia)
        {
            string sql = "UPDATE Estadia SET fecha_check_in_real = @chkIn, fecha_check_out_real = @chkOut, estado = @estado, dias_cobrados = @dias, monto_total = @monto WHERE id_estadia = @id";
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                await conexion.OpenAsync();
                using (var cmd = new SqlCommand(sql, conexion))
                {
                    cmd.Parameters.AddWithValue("@chkIn", (object?)estadia.FechaCheckInReal ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@chkOut", (object?)estadia.FechaCheckOutReal ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@estado", estadia.Estado);
                    cmd.Parameters.AddWithValue("@dias", (object?)estadia.DiasCobrados ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@monto", (object?)estadia.MontoTotal ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", estadia.IdEstadia);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}