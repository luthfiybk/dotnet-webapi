using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cs_webapi.Dtos.Stock;
using cs_webapi.Models;

namespace cs_webapi.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllStockAsync();
        Task<Stock?> GetStockById(int id);
        Task<Stock> CreateStockAsync(Stock stockModel);
        Task<Stock?> UpdateStockAsync(int id, UpdateStockRequestDto stockRequestDto);
        Task<Stock?> DeleteStockAsync(int id);
    }
}