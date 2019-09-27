using Domain.Services.Files;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
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

        [Route("{id}")]
        [HttpGet]
        public IActionResult Get(Guid id)
        {
            try
            {
                FileDto dto = filesService.Get(id);

                return File(dto.Data, MimeTypes.GetMimeType(dto.Name));
            }
            catch(Exception e)
            {
                Log.Error(e, $"Failed to Get file {id}");
                return StatusCode(500);
            }
        }
    }
}