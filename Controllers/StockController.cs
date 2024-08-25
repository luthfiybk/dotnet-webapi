using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cs_webapi.Data;
using cs_webapi.Dtos.Stock;
using cs_webapi.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace cs_webapi.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController: ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public StockController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var stocks = _context.Stocks.ToList()
                    .Select(s => s.ToStockDto());

            if(stocks == null) {
                return NotFound();
            }

            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var stock = _context.Stocks.Find(id);

            if(stock == null) {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateStockRequestDto stockRequestDto)
        {
            var stockModel = stockRequestDto.ToStockFromCreateDTO();
            _context.Stocks.Add(stockModel);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateStockRequestDto)
        {
            var stock = _context.Stocks.Find(id);

            if(stock == null) {
                return NotFound();
            }

            stock.Symbol = updateStockRequestDto.Symbol;
            stock.CompanyName = updateStockRequestDto.CompanyName;
            stock.Purchase = updateStockRequestDto.Purchase;
            stock.LastDiv = updateStockRequestDto.LastDiv;
            stock.Industry = updateStockRequestDto.Industry;
            stock.MarketCap = updateStockRequestDto.MarketCap;

            _context.SaveChanges();

            return Ok(stock.ToStockDto());
        }
    }
}