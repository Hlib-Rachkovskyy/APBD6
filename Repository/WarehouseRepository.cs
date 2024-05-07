using System.Data;
using APBD6.Models;
using Microsoft.Data.SqlClient;

namespace APBD6.Repository;

public class WarehouseRepository : IWarehouseRepository
{
    
    private IConfiguration _configuration;
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<int> InsertOrder(Order order)
    {
        using var SqlConnection = new SqlConnection(_configuration["Default"]);
        if (SqlConnection.State != ConnectionState.Open)
        {
            await SqlConnection.OpenAsync();
        }

        int id = order.IdOrder;
        var comand = new SqlCommand();
        comand.Connection = SqlConnection;
        comand.CommandText = "SELECT * FROM ORDERS o where @id = o.id";
        comand.Parameters.AddWithValue("@id", id);
        var reader = await comand.ExecuteReaderAsync();

        throw new NotImplementedException();
    }
}