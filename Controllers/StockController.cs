using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cs_webapi.Data;
using cs_webapi.Dtos.Stock;
using cs_webapi.Interfaces;
using cs_webapi.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cs_webapi.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController: ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IStockRepository _stockRepository;
        public StockController(ApplicationDBContext context, IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var stocks = await _stockRepository.GetAllStockAsync();
            
            var stockDto = stocks.Select(s => s.ToStockDto());

            if(stocks == null) {
                return NotFound();
            }

            return Ok(stocks);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var stock = await _stockRepository.GetStockById(id);

            if(stock == null) {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockRequestDto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var stockModel = stockRequestDto.ToStockFromCreateDTO();
            await _stockRepository.CreateStockAsync(stockModel);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateStockRequestDto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var stockModel = await _stockRepository.UpdateStockAsync(id, updateStockRequestDto);

            if(stockModel == null) {
                return NotFound();
            }

            return Ok(stockModel.ToStockDto());
        }

        [HttpDelete]
        [Route("{id}:int")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            
            var stockModel = await _stockRepository.DeleteStockAsync(id);

            if(stockModel == null) {
                return NotFound();
            }

            return NoContent();
        }
    }
}