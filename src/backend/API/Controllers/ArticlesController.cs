using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Articles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("Articles")]
    public class ArticlesController : DictonaryController<IArticlesService, Article, ArticleDto> 
    {
        public ArticlesController(IArticlesService articlesService) : base(articlesService)
        {
        }
    }
}