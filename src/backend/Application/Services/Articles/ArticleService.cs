using Application.BusinessModels.Articles.Handlers;
using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using AutoMapper;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.Articles;
using Domain.Services.History;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Articles
{
    public class ArticlesService : DictonaryServiceBase<Article, ArticleDto>, IArticlesService
    {
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;

        public ArticlesService(IAuditDataService dataService, IUserProvider userProvider, ITriggersService triggersService, IValidationService validationService,
                               IHistoryService historyService, IFieldSetterFactory fieldSetterFactory) 
            : base(dataService, userProvider, triggersService, validationService, fieldSetterFactory)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<Article>().OrderBy(x => x.Nart).ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Nart,
                    Value = entity.Id.ToString()
                };
            }
        }

        protected override IFieldSetter<Article> ConfigureHandlers(IFieldSetter<Article> setter)
        {
            return setter
                .AddHandler(e => e.Spgr, new SpgrHandler(_dataService, _historyService))
                .AddHandler(e => e.Description, new DescriptionHandler(_dataService, _historyService))
                .AddHandler(e => e.CountryOfOrigin, new CountryOfOriginHandler(_dataService, _historyService))
                .AddHandler(e => e.ShelfLife, new ShelfLifeHandler(_dataService, _historyService))
                .AddHandler(e => e.Ean, new EanHandler(_dataService, _historyService));
        }

        public override DetailedValidationResult MapFromDtoToEntity(Article entity, ArticleDto dto)
        {
            _mapper.Map(dto, entity);
            return null;
        }

        public override ArticleDto MapFromEntityToDto(Article entity)
        {
            if (entity == null)
            {
                return null;
            }
            return _mapper.Map<ArticleDto>(entity);
        }

        protected override IQueryable<Article> ApplySort(IQueryable<Article> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Description)
                .ThenBy(i => i.Id);
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Article, ArticleDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()));

                cfg.CreateMap<ArticleDto, Article>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToGuid()));
            });
            return result;
        }
    }
}