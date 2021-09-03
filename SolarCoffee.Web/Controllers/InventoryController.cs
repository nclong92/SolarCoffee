using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IInventoryService _inventoryService;

        public InventoryController(ILogger<InventoryController> logger,
            IInventoryService inventoryService)
        {
            this._logger = logger;
            this._inventoryService = inventoryService;
        }

        [HttpGet("/api/inventory")]
        public ActionResult GetCurrentInventory()
        {
            _logger.LogInformation("Getting all inventory ...");

            var inventory = _inventoryService.GetCurrentInventory()
                .Select(item => new ProductInventoryModel()
                {
                    Id = item.Id,
                    Product = ProductMapper.SerializeProductModel(item.Product),
                    IdealQuantity = item.IdealQuantity,
                    QuantityOnHand = item.QuantityOnHand
                })
                .OrderBy(m => m.Product.Name)
                .ToList();

            return Ok(inventory);
        }

        [HttpPatch("/api/inventory")]
        public ActionResult UpdateInventory([FromBody] ShipmentModel shipment)
        {
            _logger.LogInformation($"Updating inventory for {shipment.ProductId} - Adjustment: {shipment.Adjustment}");

            var id = shipment.ProductId;
            var adjutment = shipment.Adjustment;
            var inventory = _inventoryService.UpdateUnitsAvaiable(id, adjutment);

            return Ok(inventory);
        }
    }
}
