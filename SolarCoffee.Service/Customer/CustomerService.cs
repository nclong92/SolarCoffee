﻿using Microsoft.EntityFrameworkCore;
using SolarCoffee.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarCoffee.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly SolarDbContext _db;

        public CustomerService(SolarDbContext dbContext)
        {
            _db = dbContext;
        }
        public ServiceResponse<Data.Models.Customer> CreateCustomer(Data.Models.Customer customer)
        {
            try
            {
                _db.Customers.Add(customer);
                _db.SaveChanges();

                return new ServiceResponse<Data.Models.Customer>()
                {
                    IsSuccess = true,
                    Message = "New customer added",
                    Time = DateTime.UtcNow,
                    Data = customer
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Data.Models.Customer>()
                {
                    IsSuccess = false,
                    Message = ex.StackTrace,
                    Time = DateTime.UtcNow,
                    Data = customer
                };
            }
        }

        public ServiceResponse<bool> DeleteCustomer(int id)
        {
            var customer = _db.Customers.Find(id);
            var now = DateTime.UtcNow;

            if (customer == null)
                return new ServiceResponse<bool>
                {
                    Time = now,
                    IsSuccess = false,
                    Message = "Customer to delete not found",
                    Data = false
                };

            try
            {
                _db.Customers.Remove(customer);
                _db.SaveChanges();

                return new ServiceResponse<bool>
                {
                    Time = now,
                    IsSuccess = true,
                    Message = "Customer deleted",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Time = now,
                    IsSuccess = false,
                    Message = ex.StackTrace,
                    Data = false
                };
            }
        }

        public List<Data.Models.Customer> GetAllCustomer()
        {
            return _db.Customers
                .Include(m => m.PrimaryAddress)
                .OrderBy(m => m.LastName)
                .ToList();
        }

        public Data.Models.Customer GetById(int id)
        {
            return _db.Customers.Find(id);
        }
    }
}
