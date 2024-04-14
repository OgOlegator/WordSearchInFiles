using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WordSearchInFiles.Services;

namespace WordSearchInFiles.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        [HttpGet]
        [Route("search/{word}")]
        public async Task<IActionResult> GetFilesWithWord(string word)
        {
            try
            {
                return Ok(await new SearchIWordService().Execute(word));
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
