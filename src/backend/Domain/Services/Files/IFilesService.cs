using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Domain.Services.Files
{
    public interface IFilesService : IService
    {
        Task<ValidateResult> UploadAsync(IFormFile formFile);
    }
}
