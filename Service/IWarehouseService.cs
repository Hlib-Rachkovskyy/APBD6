using APBD6.Models;

namespace APBD6.Service;

public interface IWarehouseService
{
    public Task<int> InsertOrder(ReceiveData receiveData);
}