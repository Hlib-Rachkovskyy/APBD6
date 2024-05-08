using APBD6.Models;
using APBD6.Repository;
using APBD6.Service;
using Microsoft.AspNetCore.Mvc;

namespace APBD6;
[ApiController]
[Route("api/Warehouse")]
public class WarehouseControllers : ControllerBase
{
    private IWarehouseRepository _warehouseService;
    public WarehouseControllers(IWarehouseRepository warehouseService)
    {
        _warehouseService = warehouseService;
    }
    [HttpPost]
    public async Task<IActionResult> insertOrder(ReceiveData receiveData)
    {
        int result = await _warehouseService.InsertOrder(receiveData);
        switch (result)
        {
            case -1: 
                return BadRequest($"Amount: {receiveData.Amount} is lower form 0");
            case -2: 
                return BadRequest("There's no product with such id");
            case -3:
                return BadRequest("There's no warehouse with such id");
            case -4:
                return BadRequest("Date of request is earlier from date in database");
            case -5:
                return BadRequest("Order was fulfilled before");
            case -6:
                return BadRequest("Table Product_Warehouse already have data with same IdOrder");
        }

        return Ok(result);
    }
}