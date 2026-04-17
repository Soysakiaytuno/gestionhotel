using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using HotelBackend.Models.ModuloEstadias;

namespace HotelBackend.Repository
{
    public record EstadiaDashboardDto(int IdEstadia, string Estado, DateTime Ingreso, DateTime Salida, string Titular, string Habitaciones);

    public class RepositorioEstadia
    {
        private readonly string _cadenaConexion;

        public RepositorioEstadia(IConfiguration configuracion)
        {
            _cadenaConexion = configuracion.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<EstadiaDashboardDto>> ObtenerActivasAsync()
        {
            var lista = new List<EstadiaDashboardDto>();
            string sql = @"
                SELECT e.id_estadia, e.estado, e.fecha_ingreso_programada, e.fecha_salida_programada,
                    (SELECT TOP 1 u.nombre + ' ' + u.apellido FROM Estadia_Huesped eh INNER JOIN Huesped h ON eh.id_huesped = h.id_huesped INNER JOIN Usuario u ON h.id_usuario = u.id_usuario WHERE eh.id_estadia = e.id_estadia AND eh.es_titular = 1) AS Titular,
                    (SELECT STRING_AGG(ha.numero_habitacion, ', ') FROM Estadia_Habitacion eha INNER JOIN Habitacion ha ON eha.id_habitacion = ha.id_habitacion WHERE eha.id_estadia = e.id_estadia) AS Habitaciones
                FROM Estadia e WHERE e.estado IN ('Programada', 'En Curso') ORDER BY e.fecha_ingreso_programada";

            using var conexion = new SqlConnection(_cadenaConexion);
            await conexion.OpenAsync();
            using var comando = new SqlCommand(sql, conexion);
            using var lector = await comando.ExecuteReaderAsync();
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
            return lista;
        }

        public async Task<(Estadia? estadia, decimal precioPorNoche)> ObtenerPorIdAsync(int id)
        {
            Estadia? estadia = null;
            decimal precioPorNoche = 0;
            string sql = @"SELECT e.*, ISNULL((SELECT SUM(th.precio_base_noche) FROM Estadia_Habitacion eh INNER JOIN Habitacion h ON eh.id_habitacion = h.id_habitacion INNER JOIN TipoHabitacion th ON h.id_tipo_habitacion = th.id_tipo_habitacion WHERE eh.id_estadia = e.id_estadia), 0) AS precio_total_noche FROM Estadia e WHERE e.id_estadia = @id";

            using var conexion = new SqlConnection(_cadenaConexion);
            await conexion.OpenAsync();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("@id", id);
            using var lector = await comando.ExecuteReaderAsync();
            if (await lector.ReadAsync())
            {
                precioPorNoche = lector.GetDecimal(lector.GetOrdinal("precio_total_noche"));
                estadia = Estadia.CargarDesdeBd(
                    lector.GetInt32(lector.GetOrdinal("id_estadia")),
                    lector.GetDateTime(lector.GetOrdinal("fecha_ingreso_programada")),
                    lector.GetDateTime(lector.GetOrdinal("fecha_salida_programada")),
                    lector.IsDBNull(lector.GetOrdinal("fecha_check_in_real")) ? null : lector.GetDateTime(lector.GetOrdinal("fecha_check_in_real")),
                    lector.IsDBNull(lector.GetOrdinal("fecha_check_out_real")) ? null : lector.GetDateTime(lector.GetOrdinal("fecha_check_out_real")),
                    lector.GetString(lector.GetOrdinal("estado")),
                    lector.IsDBNull(lector.GetOrdinal("dias_cobrados")) ? null : lector.GetInt32(lector.GetOrdinal("dias_cobrados")),
                    lector.IsDBNull(lector.GetOrdinal("monto_total")) ? null : lector.GetDecimal(lector.GetOrdinal("monto_total"))
                );
            }
            return (estadia, precioPorNoche);
        }

        public async Task<int> CrearAsync(Estadia estadia, List<int> idsHab, List<int> idsHues, int idTitular)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            await conexion.OpenAsync();
            using var trans = conexion.BeginTransaction();
            try {
                string sqlE = "INSERT INTO Estadia (fecha_ingreso_programada, fecha_salida_programada, estado) OUTPUT INSERTED.id_estadia VALUES (@ing, @sal, @est)";
                int id;
                using (var cmd = new SqlCommand(sqlE, conexion, trans)) {
                    cmd.Parameters.AddWithValue("@ing", estadia.FechaIngresoProgramada);
                    cmd.Parameters.AddWithValue("@sal", estadia.FechaSalidaProgramada);
                    cmd.Parameters.AddWithValue("@est", estadia.Estado);
                    id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
                foreach (var h in idsHab) {
                    using var cmd = new SqlCommand("INSERT INTO Estadia_Habitacion (id_estadia, id_habitacion) VALUES (@idE, @idH)", conexion, trans);
                    cmd.Parameters.AddWithValue("@idE", id); cmd.Parameters.AddWithValue("@idH", h);
                    await cmd.ExecuteNonQueryAsync();
                }
                foreach (var hu in idsHues) {
                    using var cmd = new SqlCommand("INSERT INTO Estadia_Huesped (id_estadia, id_huesped, es_titular) VALUES (@idE, @idHu, @t)", conexion, trans);
                    cmd.Parameters.AddWithValue("@idE", id); cmd.Parameters.AddWithValue("@idHu", hu);
                    cmd.Parameters.AddWithValue("@t", hu == idTitular ? 1 : 0);
                    await cmd.ExecuteNonQueryAsync();
                }
                trans.Commit();
                return id;
            } catch { trans.Rollback(); throw; }
        }

        public async Task ActualizarAsync(Estadia estadia)
        {
            string sql = "UPDATE Estadia SET fecha_check_in_real = @in, fecha_check_out_real = @out, estado = @est, dias_cobrados = @d, monto_total = @m WHERE id_estadia = @id";
            using var conexion = new SqlConnection(_cadenaConexion);
            await conexion.OpenAsync();
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@in", (object?)estadia.FechaCheckInReal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@out", (object?)estadia.FechaCheckOutReal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@est", estadia.Estado);
            cmd.Parameters.AddWithValue("@d", (object?)estadia.DiasCobrados ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@m", (object?)estadia.MontoTotal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", estadia.IdEstadia);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}