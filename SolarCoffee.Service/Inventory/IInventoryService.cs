using SolarCoffee.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolarCoffee.Services.Inventory
{
    public interface IInventoryService
    {
        List<ProductInventory> GetCurrentInventory();
        ServiceResponse<ProductInventory> UpdateUnitsAvaiable(int id, int adjustment);
        ProductInventory GetByProductId(int productId);
        List<ProductInventorySnapshot> GetSnapshotHistory();
    }
}
