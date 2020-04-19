using prj3beer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace prj3beer.Services
{
    /// <summary>
    /// Interface for API Managers
    /// </summary>
    public interface IAPIManager
    {
        // Enforce classes that implement this interface to use "GetBrandsAsync"
        Task<List<Brand>> GetBrandsAsync();

        // Enforce classes that implement this interface to use "GetBeveragesAsync"
        Task<List<Beverage>> GetBeveragesAsync();
    }
}
