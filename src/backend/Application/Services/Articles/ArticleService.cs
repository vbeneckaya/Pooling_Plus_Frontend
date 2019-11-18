using Application.BusinessModels.Articles.Handlers;
using Application.Shared;
using AutoMapper;
using DAL.Services;
using Domain.Persistables;
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

        public ArticlesService(ICommonDataService dataService, IUserProvider userProvider, IHistoryService historyService) 
            : base(dataService, userProvider)
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

        public override ValidateResult MapFromDtoToEntity(Article entity, ArticleDto dto)
        {
            var setter = new FieldSetter<Article>(entity, _historyService);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id), ignoreChanges: true);
            setter.UpdateField(e => e.Spgr, dto.Spgr, new SpgrHandler(_dataService, _historyService));
            setter.UpdateField(e => e.Description, dto.Description, new DescriptionHandler(_dataService, _historyService));
            setter.UpdateField(e => e.Nart, dto.Nart);
            setter.UpdateField(e => e.CountryOfOrigin, dto.CountryOfOrigin, new CountryOfOriginHandler(_dataService, _historyService));
            setter.UpdateField(e => e.ShelfLife, dto.ShelfLife, new ShelfLifeHandler(_dataService, _historyService));
            setter.UpdateField(e => e.Status, dto.Status);
            setter.UpdateField(e => e.Ean, dto.Ean, new EanHandler(_dataService, _historyService));
            setter.UpdateField(e => e.UnitLengthGoodsMm, dto.UnitLengthGoodsMm);
            setter.UpdateField(e => e.WidthUnitsGoodsMm, dto.WidthUnitsGoodsMm);
            setter.UpdateField(e => e.UnitHeightGoodsMm, dto.UnitHeightGoodsMm);
            setter.UpdateField(e => e.WeightUnitsGrossProductG, dto.WeightUnitsGrossProductG);
            setter.UpdateField(e => e.WeightUnitsNetGoodsG, dto.WeightUnitsNetGoodsG);
            setter.UpdateField(e => e.EanShrink, dto.EanShrink);
            setter.UpdateField(e => e.PiecesInShrink, dto.PiecesInShrink);
            setter.UpdateField(e => e.LengthShrinkMm, dto.LengthShrinkMm);
            setter.UpdateField(e => e.WidthShrinkMm, dto.WidthShrinkMm);
            setter.UpdateField(e => e.HeightShrinkMm, dto.HeightShrinkMm);
            setter.UpdateField(e => e.GrossShrinkWeightG, dto.GrossShrinkWeightG);
            setter.UpdateField(e => e.NetWeightShrinkG, dto.NetWeightShrinkG);
            setter.UpdateField(e => e.EanBox, dto.EanBox);
            setter.UpdateField(e => e.PiecesInABox, dto.PiecesInABox);
            setter.UpdateField(e => e.BoxLengthMm, dto.BoxLengthMm);
            setter.UpdateField(e => e.WidthOfABoxMm, dto.WidthOfABoxMm);
            setter.UpdateField(e => e.BoxHeightMm, dto.BoxHeightMm);
            setter.UpdateField(e => e.GrossBoxWeightG, dto.GrossBoxWeightG);
            setter.UpdateField(e => e.NetBoxWeightG, dto.NetBoxWeightG);
            setter.UpdateField(e => e.PiecesInALayer, dto.PiecesInALayer);
            setter.UpdateField(e => e.LayerLengthMm, dto.LayerLengthMm);
            setter.UpdateField(e => e.LayerWidthMm, dto.LayerWidthMm);
            setter.UpdateField(e => e.LayerHeightMm, dto.LayerHeightMm);
            setter.UpdateField(e => e.GrossLayerWeightMm, dto.GrossLayerWeightMm);
            setter.UpdateField(e => e.NetWeightMm, dto.NetWeightMm);
            setter.UpdateField(e => e.EanPallet, dto.EanPallet);
            setter.UpdateField(e => e.PiecesOnAPallet, dto.PiecesOnAPallet);
            setter.UpdateField(e => e.PalletLengthMm, dto.PalletLengthMm);
            setter.UpdateField(e => e.WidthOfPalletsMm, dto.WidthOfPalletsMm);
            setter.UpdateField(e => e.PalletHeightMm, dto.PalletHeightMm);
            setter.UpdateField(e => e.GrossPalletWeightG, dto.GrossPalletWeightG);
            setter.UpdateField(e => e.NetWeightPalletsG, dto.NetWeightPalletsG);

            setter.ApplyAfterActions();
            setter.SaveHistoryLog();

            string errors = setter.ValidationErrors;
            return new ValidateResult(errors, entity.Id.ToString());
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
            });
            return result;
        }
    }
}