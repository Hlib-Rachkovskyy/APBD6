using APBD6.Models;
using APBD6.Service;
using Microsoft.AspNetCore.Mvc;

namespace APBD6;

public class WarehouseControllers : ControllerBase
{
    private IWarehouseService _warehouseService;
    public WarehouseControllers(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    [HttpPost]
    public Task<int> insertOrder(Order order)
    {
        return _warehouseService.InsertOrder(order);
    }
}