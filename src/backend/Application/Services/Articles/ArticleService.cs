using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Extensions;
using Domain.Services.Articles;
using Microsoft.EntityFrameworkCore;
using Domain.Shared;
using System.Collections.Generic;
using System.Linq;

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

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = db.Articles.OrderBy(x => x.Nart).ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Nart,
                    Value = entity.Id.ToString()
                };
            }
        }

        public override void MapFromDtoToEntity(Article entity, ArticleDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.SPGR = dto.SPGR;
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
            entity.EANShrink = dto.EANShrink;
            entity.PiecesInShrink = dto.PiecesInShrink;
            entity.LengthShrinkMm = dto.LengthShrinkMm;
            entity.WidthShrinkMm = dto.WidthShrinkMm;
            entity.HeightShrinkMm = dto.HeightShrinkMm;
            entity.GrossShrinkWeightG = dto.GrossShrinkWeightG;
            entity.NetWeightShrinkG = dto.NetWeightShrinkG;
            entity.EANBox = dto.EANBox;
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
            entity.EANPallet = dto.EANPallet;
            entity.PiecesOnAPallet = dto.PiecesOnAPallet;
            entity.PalletLengthMm = dto.PalletLengthMm;
            entity.WidthOfPalletsMm = dto.WidthOfPalletsMm;
            entity.PalletHeightMm = dto.PalletHeightMm;
            entity.GrossPalletWeightG = dto.GrossPalletWeightG;
            entity.NetWeightPalletsG = dto.NetWeightPalletsG;
            /*end of map dto to entity fields*/
        }

        public override ArticleDto MapFromEntityToDto(Article entity)
        {
            return new ArticleDto
            {
                Id = entity.Id.ToString(),
                SPGR = entity.SPGR,
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
                EANShrink = entity.EANShrink,
                PiecesInShrink = entity.PiecesInShrink,
                LengthShrinkMm = entity.LengthShrinkMm,
                WidthShrinkMm = entity.WidthShrinkMm,
                HeightShrinkMm = entity.HeightShrinkMm,
                GrossShrinkWeightG = entity.GrossShrinkWeightG,
                NetWeightShrinkG = entity.NetWeightShrinkG,
                EANBox = entity.EANBox,
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
                EANPallet = entity.EANPallet,
                PiecesOnAPallet = entity.PiecesOnAPallet,
                PalletLengthMm = entity.PalletLengthMm,
                WidthOfPalletsMm = entity.WidthOfPalletsMm,
                PalletHeightMm = entity.PalletHeightMm,
                GrossPalletWeightG = entity.GrossPalletWeightG,
                NetWeightPalletsG = entity.NetWeightPalletsG,
                /*end of map entity to dto fields*/
            };
        }
    }
}