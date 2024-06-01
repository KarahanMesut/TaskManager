using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data
{
    internal class GenericRepository<T> where T : class, new()
    {
        private readonly string _connectionString;

        public GenericRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<T>> GetAllAsync(string query)
        {
            return await GetAllAsync(query, null);
        }

        public async Task<List<T>> GetAllAsync(string query, SqlParameter[] parameters = null)
        {
            List<T> items = new List<T>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand command = new SqlCommand(query, conn);

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                SqlDataReader reader = await command.ExecuteReaderAsync();
                DataTable schemaTable = reader.GetSchemaTable();

                while (await reader.ReadAsync())
                {
                    T item = new T();
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        string columnName = row["ColumnName"].ToString();
                        var property = typeof(T).GetProperty(columnName);
                        if (property != null && !reader.IsDBNull(reader.GetOrdinal(columnName)))
                        {
                            object value = reader[columnName];
                            if (property.PropertyType != value.GetType())
                            {
                                value = Convert.ChangeType(value, property.PropertyType);
                            }
                            property.SetValue(item, value);
                        }
                    }
                    items.Add(item);
                }
            }

            return items;
        }

        public async Task<T> GetSingleAsync(string query, SqlParameter[] parameters)
        {
            T item = default;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        item = new T();
                        var schemaTable = reader.GetSchemaTable();
                        var properties = typeof(T).GetProperties();

                        foreach (DataRow row in schemaTable.Rows)
                        {
                            var columnName = row["ColumnName"].ToString();
                            var property = typeof(T).GetProperty(columnName);
                            if (property != null && !reader.IsDBNull(reader.GetOrdinal(columnName)))
                            {
                                property.SetValue(item, reader[columnName]);
                            }
                        }
                    }
                }
            }

            return item;
        }

        public async Task AddAsync(string query, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddRange(parameters);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(string query, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddRange(parameters);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(string query, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddRange(parameters);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
