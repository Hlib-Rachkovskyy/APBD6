using System.Data;
using APBD6.Models;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.CompilerServices;

namespace APBD6.Repository;

public class WarehouseRepository(IConfiguration configuration) : IWarehouseRepository
{
    public async Task<int> InsertOrder(ReceiveData receiveData)
    {
        int insertedRecordId;

        int number;
        if (receiveData.Amount <= 0)
            return -1;
        
        await using var sqlConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        if (sqlConnection.State != ConnectionState.Open)
        {
            await sqlConnection.OpenAsync();
        }
        await using var transaction = await sqlConnection.BeginTransactionAsync();
        try
        {

            var command = new SqlCommand();
            command.Connection = sqlConnection;
            command.CommandText = """
                                  SELECT o.Price
                                  FROM Product o
                                  WHERE o.IdProduct = @IdProduct
                                  """;
            command.Transaction = (SqlTransaction)transaction;
            command.Parameters.AddWithValue("@IdProduct", receiveData.IdProduct);
            var price = await command.ExecuteScalarAsync();
            

            if (price is null)
            {
                throw new CustomException(-2);
            }

            var command1 = new SqlCommand();
            command1.Connection = sqlConnection;
            command1.CommandText = """
                                  SELECT COUNT(*)
                                  FROM Warehouse o
                                  WHERE o.IdWarehouse = @IdWarehouse
                                  """;
            command1.Transaction = (SqlTransaction)transaction;
            command1.Parameters.AddWithValue("@IdWarehouse", receiveData.IdWarehouse);
            number = (int) (await command1.ExecuteScalarAsync())!;
            if (number is 0)
            {
                throw new CustomException(-3);
            }
            
            
            var command3 = new SqlCommand();
            command3.Connection = sqlConnection;
            command3.CommandText = """
                                   SELECT *
                                   FROM "Order"
                                   WHERE IdProduct = @IdProduct
                                   AND Amount = @Amount
                                   """;
            command3.Transaction = (SqlTransaction)transaction;
            command3.Parameters.AddWithValue("@IdProduct", receiveData.IdProduct);
            command3.Parameters.AddWithValue("@Amount", receiveData.Amount);
            await using var reader3 = await command3.ExecuteReaderAsync();
            var order = new Order();
            while (await reader3.ReadAsync())
            {
                order.IdOrder = reader3.GetInt32(0);
                order.IdProduct = reader3.GetInt32(1);
                order.Amount = reader3.GetInt32(2);
                order.CreatedAt = reader3.GetDateTime(3);
                if (!await reader3.IsDBNullAsync(4))
                {
                    order.FulfilledAt = reader3.GetDateTime(4);
                }
                else
                {
                    order.FulfilledAt = null;
                }

            }
            await reader3.CloseAsync();


           

            if (order.FulfilledAt is not null)
            {
                throw new CustomException(-5);
            }
            
            if (!(order.CreatedAt > receiveData.CreatedAt))
            {
                throw new CustomException(-4);
            }
            var command4 = new SqlCommand();
            command4.Connection = sqlConnection;
            command4.CommandText = """
                                   SELECT COUNT(*)
                                   FROM Product_Warehouse
                                   WHERE IdOrder = @IdOrder
                                   """;
            command4.Transaction = (SqlTransaction)transaction;
            command4.Parameters.AddWithValue("@IdOrder", order.IdOrder);
            var orderCount = (int)await command4.ExecuteScalarAsync();
            if (orderCount > 0)
                throw new CustomException(-6);


            var updateCommand = new SqlCommand();
            updateCommand.Connection = sqlConnection;
            updateCommand.CommandText = """
                                        UPDATE "Order"
                                        SET FulfilledAt = GETDATE()
                                        WHERE IdOrder = @IdOrder
                                        """;
            updateCommand.Transaction = (SqlTransaction)transaction;

            updateCommand.Parameters.AddWithValue("@IdOrder", order.IdOrder);
            await updateCommand.ExecuteNonQueryAsync();
            var insertCommand = new SqlCommand();
            insertCommand.Connection = sqlConnection;
            insertCommand.CommandText = """
                                        INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                                        VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, GETDATE());
                                        SELECT SCOPE_IDENTITY()
                                        """;
            insertCommand.Transaction = (SqlTransaction)transaction;
            insertCommand.Parameters.AddWithValue("@IdWarehouse", receiveData.IdWarehouse);
            insertCommand.Parameters.AddWithValue("@IdProduct", order.IdProduct);
            insertCommand.Parameters.AddWithValue("@IdOrder", order.IdOrder);
            insertCommand.Parameters.AddWithValue("@Amount", order.Amount);
            insertCommand.Parameters.AddWithValue("@Price", order.Amount * Convert.ToInt32(price));

            insertedRecordId = Convert.ToInt32(await insertCommand.ExecuteScalarAsync());

            await transaction.CommitAsync();
        }
        catch (CustomException e)
        {
            await transaction.RollbackAsync();
            return e.ErrorCode;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }



        return insertedRecordId;
    }
}

public class CustomException(int errorCode) : Exception
{
    public int ErrorCode { get; } = errorCode;
}