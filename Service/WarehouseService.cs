using APBD6.Models;
using APBD6.Repository;

namespace APBD6.Service;

public class WarehouseService : IWarehouseService
{
    private IWarehouseRepository _warehouseRepository;
    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    public async Task<int> InsertOrder(ReceiveData receiveData)
    {
        return await _warehouseRepository.InsertOrder(receiveData);
    }
}