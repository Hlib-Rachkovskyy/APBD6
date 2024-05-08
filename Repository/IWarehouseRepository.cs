using APBD6.Models;

namespace APBD6.Repository;

public interface IWarehouseRepository
{
    public Task<int> InsertOrder(ReceiveData receiveData);
}