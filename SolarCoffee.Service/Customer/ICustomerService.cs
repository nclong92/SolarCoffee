using System.Collections.Generic;

namespace SolarCoffee.Services.Customer
{
    public interface ICustomerService
    {
        List<Data.Models.Customer> GetAllCustomer();

        ServiceResponse<Data.Models.Customer> CreateCustomer(Data.Models.Customer customer);

        ServiceResponse<bool> DeleteCustomer(int id);

        Data.Models.Customer GetById(int id);
    }
}