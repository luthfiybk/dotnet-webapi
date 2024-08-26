using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cs_webapi.Dtos.Comment;
using cs_webapi.Interfaces;
using cs_webapi.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace cs_webapi.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController: ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;
        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepository.GetAllCommentsAsync();
            var commentDto = comments.Select(c => c.ToCommentDto());

            if(comments == null) {
                return NotFound();
            }

            return Ok(commentDto);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentRepository.GetCommentById(id);

            if(comment == null) {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentRequestDto commentRequestDto)
        {
            if(!await _stockRepository.StockExists(stockId)) {
                return BadRequest("Stock not found");
            }

            var commentModel = commentRequestDto.ToCommentFromCreateDTO(stockId);
            await _commentRepository.CreateCommentAsync(commentModel);
            
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateCommentRequestDto)
        {
            var commentModel = await _commentRepository.UpdateCommentAsync(id, updateCommentRequestDto.ToCommentFromUpdateDTO());

            if(commentModel == null) {
                return NotFound();
            }

            return Ok(commentModel.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var commentModel = await _commentRepository.DeleteCommentAsync(id);

            if(commentModel == null) {
                return NotFound();
            }

            return NoContent();
        }
    }
}