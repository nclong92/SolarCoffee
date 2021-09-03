using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarCoffee.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly SolarDbContext _db;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(SolarDbContext dbContext,
            ILogger<InventoryService> logger)
        {
            _db = dbContext;
            _logger = logger;
        }

        private void CreateSnapshot(ProductInventory inventory)
        {
            var now = DateTime.UtcNow;

            var snapshot = new ProductInventorySnapshot()
            {
                SnapshotTime = now,
                Product = inventory.Product,
                QuantityOnHand = inventory.QuantityOnHand
            };

            _db.Add(snapshot);
        }

        public ProductInventory GetByProductId(int productId)
        {
            return _db.ProductInventories
                .Include(m => m.Product)
                .FirstOrDefault(m => m.Product.Id == productId);
        }

        public List<ProductInventory> GetCurrentInventory()
        {
            return _db.ProductInventories
                .Include(m => m.Product)
                .Where(m => !m.Product.IsArchived)
                .ToList();
        }

        public List<ProductInventorySnapshot> GetSnapshotHistory()
        {
            var earliest = DateTime.UtcNow - TimeSpan.FromHours(6);
            return _db.ProductInventorySnapshots
                .Include(snap => snap.Product)
                .Where(snap => snap.SnapshotTime > earliest && !snap.Product.IsArchived)
                .ToList();
        }

        public ServiceResponse<ProductInventory> UpdateUnitsAvaiable(int id, int adjustment)
        {
            var now = DateTime.Now;

            try
            {
                var inventory = _db.ProductInventories
                    .Include(m => m.Product)
                    .First(m => m.Product.Id == id);

                inventory.QuantityOnHand += adjustment;

                try
                {
                    CreateSnapshot(inventory);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error creating inventory snapshot.");
                    _logger.LogError(ex.StackTrace);
                }

                _db.SaveChanges();

                return new ServiceResponse<ProductInventory>()
                {
                    IsSuccess = true,
                    Data = inventory,
                    Message = $"Product {id} inventory adjusted",
                    Time = now
                };
            }
            catch
            {
                return new ServiceResponse<ProductInventory>()
                {
                    IsSuccess = false,
                    Data = null,
                    Message = $"Error updating ProductInventory QuantityOnhand",
                    Time = now
                };
            }
        }
    }
}