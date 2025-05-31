using Microsoft.AspNetCore.Mvc;
using Hospital_BE.BLL.Interfaces;
using System;
using System.Threading.Tasks;

namespace Hospital_BE.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarkdownController : ControllerBase
    {
        private readonly IMarkdownService _markdownService;
        public MarkdownController(IMarkdownService markdownService)
        {
            _markdownService = markdownService;
        }

        /// <summary>
        /// Lấy thông tin Markdown theo Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarkdownById(Guid id)
        {
            var markdown = await _markdownService.GetMarkdownByIdAsync(id);
            if (markdown == null)
                return NotFound();
            return Ok(markdown);
        }
    }
}
