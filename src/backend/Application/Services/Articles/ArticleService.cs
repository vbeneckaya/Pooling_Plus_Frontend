using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.Articles;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Articles
{
    public class ArticlesService : DictonaryServiceBase<Article, ArticleDto>, IArticlesService
    {
        public ArticlesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

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
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Spgr = dto.Spgr;
            entity.Description = dto.Description;
            entity.Nart = dto.Nart;
            entity.CountryOfOrigin = dto.CountryOfOrigin;
            entity.ShelfLife = dto.ShelfLife;
            entity.Status = dto.Status;
            entity.Ean = dto.Ean;
            entity.UnitLengthGoodsMm = dto.UnitLengthGoodsMm;
            entity.WidthUnitsGoodsMm = dto.WidthUnitsGoodsMm;
            entity.UnitHeightGoodsMm = dto.UnitHeightGoodsMm;
            entity.WeightUnitsGrossProductG = dto.WeightUnitsGrossProductG;
            entity.WeightUnitsNetGoodsG = dto.WeightUnitsNetGoodsG;
            entity.EanShrink = dto.EanShrink;
            entity.PiecesInShrink = dto.PiecesInShrink;
            entity.LengthShrinkMm = dto.LengthShrinkMm;
            entity.WidthShrinkMm = dto.WidthShrinkMm;
            entity.HeightShrinkMm = dto.HeightShrinkMm;
            entity.GrossShrinkWeightG = dto.GrossShrinkWeightG;
            entity.NetWeightShrinkG = dto.NetWeightShrinkG;
            entity.EanBox = dto.EanBox;
            entity.PiecesInABox = dto.PiecesInABox;
            entity.BoxLengthMm = dto.BoxLengthMm;
            entity.WidthOfABoxMm = dto.WidthOfABoxMm;
            entity.BoxHeightMm = dto.BoxHeightMm;
            entity.GrossBoxWeightG = dto.GrossBoxWeightG;
            entity.NetBoxWeightG = dto.NetBoxWeightG;
            entity.PiecesInALayer = dto.PiecesInALayer;
            entity.LayerLengthMm = dto.LayerLengthMm;
            entity.LayerWidthMm = dto.LayerWidthMm;
            entity.LayerHeightMm = dto.LayerHeightMm;
            entity.GrossLayerWeightMm = dto.GrossLayerWeightMm;
            entity.NetWeightMm = dto.NetWeightMm;
            entity.EanPallet = dto.EanPallet;
            entity.PiecesOnAPallet = dto.PiecesOnAPallet;
            entity.PalletLengthMm = dto.PalletLengthMm;
            entity.WidthOfPalletsMm = dto.WidthOfPalletsMm;
            entity.PalletHeightMm = dto.PalletHeightMm;
            entity.GrossPalletWeightG = dto.GrossPalletWeightG;
            entity.NetWeightPalletsG = dto.NetWeightPalletsG;

            return new ValidateResult(entity.Id.ToString());
        }

        public override ArticleDto MapFromEntityToDto(Article entity)
        {
            return new ArticleDto
            {
                Id = entity.Id.ToString(),
                Spgr = entity.Spgr,
                Description = entity.Description,
                Nart = entity.Nart,
                CountryOfOrigin = entity.CountryOfOrigin,
                ShelfLife = entity.ShelfLife,
                Status = entity.Status,
                Ean = entity.Ean,
                UnitLengthGoodsMm = entity.UnitLengthGoodsMm,
                WidthUnitsGoodsMm = entity.WidthUnitsGoodsMm,
                UnitHeightGoodsMm = entity.UnitHeightGoodsMm,
                WeightUnitsGrossProductG = entity.WeightUnitsGrossProductG,
                WeightUnitsNetGoodsG = entity.WeightUnitsNetGoodsG,
                EanShrink = entity.EanShrink,
                PiecesInShrink = entity.PiecesInShrink,
                LengthShrinkMm = entity.LengthShrinkMm,
                WidthShrinkMm = entity.WidthShrinkMm,
                HeightShrinkMm = entity.HeightShrinkMm,
                GrossShrinkWeightG = entity.GrossShrinkWeightG,
                NetWeightShrinkG = entity.NetWeightShrinkG,
                EanBox = entity.EanBox,
                PiecesInABox = entity.PiecesInABox,
                BoxLengthMm = entity.BoxLengthMm,
                WidthOfABoxMm = entity.WidthOfABoxMm,
                BoxHeightMm = entity.BoxHeightMm,
                GrossBoxWeightG = entity.GrossBoxWeightG,
                NetBoxWeightG = entity.NetBoxWeightG,
                PiecesInALayer = entity.PiecesInALayer,
                LayerLengthMm = entity.LayerLengthMm,
                LayerWidthMm = entity.LayerWidthMm,
                LayerHeightMm = entity.LayerHeightMm,
                GrossLayerWeightMm = entity.GrossLayerWeightMm,
                NetWeightMm = entity.NetWeightMm,
                EanPallet = entity.EanPallet,
                PiecesOnAPallet = entity.PiecesOnAPallet,
                PalletLengthMm = entity.PalletLengthMm,
                WidthOfPalletsMm = entity.WidthOfPalletsMm,
                PalletHeightMm = entity.PalletHeightMm,
                GrossPalletWeightG = entity.GrossPalletWeightG,
                NetWeightPalletsG = entity.NetWeightPalletsG,
                /*end of map entity to dto fields*/
            };
        }

        protected override IQueryable<Article> ApplySort(IQueryable<Article> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Spgr)
                .ThenBy(i => i.Id);
        }
    }
}