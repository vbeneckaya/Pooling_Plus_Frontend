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
using Domain.Services.FieldProperties;
using Domain.Services.History;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Articles
{
    public class ArticlesService : DictionaryServiceBase<Article, ArticleDto>, IArticlesService
    {
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;
        private readonly IChangeTrackerFactory _changeTrackerFactory;

        public ArticlesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, IValidationService validationService,
                               IHistoryService historyService, IFieldDispatcherService fieldDispatcherService, IFieldSetterFactory fieldSetterFactory, IChangeTrackerFactory changeTrackerFactory) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
            _changeTrackerFactory = changeTrackerFactory;
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Article, ArticleDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.CompanyId, e => e.MapFrom((s, t) => s.CompanyId == null ? null : new LookUpDto(s.CompanyId.ToString())));

                cfg.CreateMap<ArticleDto, Article>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToGuid()))
                    .ForMember(t => t.CompanyId, e => e.Condition((s) => s.CompanyId != null))
                    .ForMember(t => t.CompanyId, e => e.MapFrom((s) => s.CompanyId.Value.ToGuid()));
            });
            return result;
        }

        protected override IFieldSetter<Article> ConfigureHandlers(IFieldSetter<Article> setter, ArticleDto dto)
        {
            return setter
                .AddHandler(e => e.Spgr, new SpgrHandler(_dataService, _historyService))
                .AddHandler(e => e.Description, new DescriptionHandler(_dataService, _historyService))
                .AddHandler(e => e.CountryOfOrigin, new CountryOfOriginHandler(_dataService, _historyService))
                .AddHandler(e => e.ShelfLife, new ShelfLifeHandler(_dataService, _historyService))
                .AddHandler(e => e.Ean, new EanHandler(_dataService, _historyService));
        }

        protected override IChangeTracker ConfigureChangeTacker()
        {
            return _changeTrackerFactory.CreateChangeTracker()
                .TrackAll<Article>()
                .Remove<Article>(i => i.Id);
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

        protected override IEnumerable<ArticleDto> FillLookupNames(IEnumerable<ArticleDto> dtos)
        {
            var companyIds = dtos.Where(x => !string.IsNullOrEmpty(x.CompanyId?.Value))
                         .Select(x => x.CompanyId.Value.ToGuid())
                         .ToList();

            var companies = _dataService.GetDbSet<Company>()
                                           .Where(x => companyIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.CompanyId?.Value)
                    && companies.TryGetValue(dto.CompanyId.Value, out Company company))
                {
                    dto.CompanyId.Name = company.Name;
                }
                yield return dto;
            }
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
        public override Article FindByKey(ArticleDto dto)
        {
            return _dataService.GetDbSet<Article>()
                .FirstOrDefault(i => i.Nart == dto.Nart);
        }

        protected override IQueryable<Article> ApplySort(IQueryable<Article> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Description)
                .ThenBy(i => i.Id);
        }
    }
}