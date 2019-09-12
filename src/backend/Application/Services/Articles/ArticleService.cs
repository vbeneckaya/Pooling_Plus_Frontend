using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Articles;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Articles
{
    public class ArticlesService : DictonaryServiceBase<Article, ArticleDto>, IArticlesService
    {
        public ArticlesService(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override DbSet<Article> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Articles;
        }

        public override void MapFromDtoToEntity(Article entity, ArticleDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            /*end of fields*/
        }

        public override ArticleDto MapFromEntityToDto(Article entity)
        {
            return new ArticleDto
            {
                Id = entity.Id.ToString(),
                /*end of fields*/
            };
        }
    }
}