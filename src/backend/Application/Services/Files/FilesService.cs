using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Files;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Application.Services.Files
{
    public class FilesService : DbSetAndContext<FileStorage>, IFilesService
    {
        private const long minFileSize = 0;
        private const long maxFileSize = 31457280;

        public FilesService(AppDbContext context) : base(context) { }

        public async Task<ValidateResult> UploadAsync(IFormFile formFile)
        {
            ValidateResult validateResult = Validation(formFile);
            if (validateResult.IsError)
            {
                return validateResult;
            }

            var file = new FileStorage
            {
                Name = formFile.FileName
            };

            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);

                file.Data = stream.ToArray();
            }

            Create(file);

            return new ValidateResult
            {
                Id = file.Id.ToString()
            };
        }

        private void Create(FileStorage file)
        {
            DbSet.Add(file);
            Context.SaveChanges();
        }

        private ValidateResult Validation(IFormFile file)
        {
            if (file == null)
            {
                return new ValidateResult("nullFile");
            }

            if (file.Length < minFileSize)
            {
                return new ValidateResult("minFileSize");
            }

            if (file.Length > maxFileSize)
            {
                return new ValidateResult("maxFileSize");
            }

            return new ValidateResult();
        }
    }
}
