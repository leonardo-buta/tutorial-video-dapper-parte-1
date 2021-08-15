using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TutorialDapperClientCRUD.Models;

namespace TutorialDapperClientCRUD.Repository
{
    public class CustomerRepository
    {
        private readonly IConfiguration configuration;

        public CustomerRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Customers WHERE CustomerId = @Id";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            connection.Open();

            var result = await connection.QuerySingleOrDefaultAsync<Customer>(sql, new { Id = id });
            return result;
        }

        public async Task<IReadOnlyList<Customer>> GetAllAsync()
        {
            var sql = "SELECT * FROM Customers";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            connection.Open();

            var result = await connection.QueryAsync<Customer>(sql);
            return result.ToList();
        }

        public async Task<Customer> AddAsync(Customer entity)
        {
            entity.CreationDate = DateTime.Now;
            var sql = "INSERT INTO Customers (Name, Phone, CreationDate) VALUES (@name, @phone, @creationDate);" +
                      "SELECT CAST(SCOPE_IDENTITY() as int)";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            connection.Open();

            entity.CustomerId = await connection.QuerySingleAsync<int>(sql, entity);
            return entity;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Customers WHERE CustomerId = @id";
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            connection.Open();

            var result = await connection.ExecuteAsync(sql, new { id });
            return result;
        }

        public async Task<int> UpdateAsync(Customer entity)
        {
            var sql = "UPDATE Customers SET Name = @name, Phone = @phone WHERE CustomerId = @CustomerId";
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            connection.Open();

            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}
