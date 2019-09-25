using Domain.Services.Files;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/files")]
    public class FilesController : Controller
    {
        private readonly IFilesService filesService;

        public FilesController(IFilesService filesService)
        {
            this.filesService = filesService;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile formFile)
        {
            try
            {
                ValidateResult result = await filesService.UploadAsync(formFile);

                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to upload file");
                return StatusCode(500);
            }
        }
    }
}