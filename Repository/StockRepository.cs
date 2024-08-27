using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cs_webapi.Data;
using cs_webapi.Dtos.Stock;
using cs_webapi.Helpers;
using cs_webapi.Interfaces;
using cs_webapi.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cs_webapi.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;

        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateStockAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteStockAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (stockModel == null)
            {
                return null;
            }
            
            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            
            return stockModel;
        }

        public async Task<List<Stock>> GetAllStockAsync(QueryObject query)
        {
            var stocks = _context.Stocks.Include(c => c.Comments).AsQueryable();

            if(!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if(!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if(!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if(query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetStockById(int id)
        {
            return await _context.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<bool> StockExists(int id)
        {
            return await _context.Stocks.AnyAsync(e => e.Id == id);
        }

        public async Task<Stock?> UpdateStockAsync(int id, UpdateStockRequestDto stockRequestDto)
        {
            var existingStock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (existingStock == null)
            {
                return null;
            }

            existingStock.Symbol = stockRequestDto.Symbol;
            existingStock.CompanyName = stockRequestDto.CompanyName;
            existingStock.Purchase = stockRequestDto.Purchase;
            existingStock.LastDiv = stockRequestDto.LastDiv;
            existingStock.Industry = stockRequestDto.Industry;
            existingStock.MarketCap = stockRequestDto.MarketCap;

            await _context.SaveChangesAsync();

            return existingStock;
        }
    }
}