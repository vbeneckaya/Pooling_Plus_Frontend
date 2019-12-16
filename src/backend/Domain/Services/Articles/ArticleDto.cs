using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.Articles
{
    public class ArticleDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1)]
        public string Spgr { get; set; }

        [FieldType(FieldType.Text), OrderNumber(2)]
        public string Description { get; set; }

        [FieldType(FieldType.Text), OrderNumber(3)]
        public string Nart { get; set; }

        [FieldType(FieldType.Text), OrderNumber(4)]
        public string CountryOfOrigin { get; set; }

        [FieldType(FieldType.Number), OrderNumber(5)]
        public int? ShelfLife { get; set; }

        [FieldType(FieldType.Text), OrderNumber(6)]
        public string Status { get; set; }

        [FieldType(FieldType.Text), OrderNumber(7)]
        public string Ean { get; set; }

        [FieldType(FieldType.Number), OrderNumber(8)]
        public int? UnitLengthGoodsMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(9)]
        public int? WidthUnitsGoodsMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(10)]
        public int? UnitHeightGoodsMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(11)]
        public int? WeightUnitsGrossProductG { get; set; }

        [FieldType(FieldType.Number), OrderNumber(12)]
        public int? WeightUnitsNetGoodsG { get; set; }

        [FieldType(FieldType.Text), OrderNumber(13)]
        public string EanShrink { get; set; }

        [FieldType(FieldType.Number), OrderNumber(14)]
        public int? PiecesInShrink { get; set; }

        [FieldType(FieldType.Number), OrderNumber(15)]
        public int? LengthShrinkMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(16)]
        public int? WidthShrinkMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(17)]
        public int? HeightShrinkMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(18)]
        public int? GrossShrinkWeightG { get; set; }

        [FieldType(FieldType.Number), OrderNumber(19)]
        public int? NetWeightShrinkG { get; set; }

        [FieldType(FieldType.Text), OrderNumber(20)]
        public string EanBox { get; set; }

        [FieldType(FieldType.Number), OrderNumber(21)]
        public int? PiecesInABox { get; set; }

        [FieldType(FieldType.Number), OrderNumber(22)]
        public int? BoxLengthMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(23)]
        public int? WidthOfABoxMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(24)]
        public int? BoxHeightMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(25)]
        public int? GrossBoxWeightG { get; set; }

        [FieldType(FieldType.Number), OrderNumber(26)]
        public int? NetBoxWeightG { get; set; }

        [FieldType(FieldType.Number), OrderNumber(27)]
        public int? PiecesInALayer { get; set; }

        [FieldType(FieldType.Number), OrderNumber(28)]
        public int? LayerLengthMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(29)]
        public int? LayerWidthMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(30)]
        public int? LayerHeightMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(31)]
        public int? GrossLayerWeightMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(32)]
        public int? NetWeightMm { get; set; }

        [FieldType(FieldType.Text), OrderNumber(33)]
        public string EanPallet { get; set; }

        [FieldType(FieldType.Number), OrderNumber(34)]
        public int? PiecesOnAPallet { get; set; }

        [FieldType(FieldType.Number), OrderNumber(35)]
        public int? PalletLengthMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(36)]
        public int? WidthOfPalletsMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(37)]
        public int? PalletHeightMm { get; set; }

        [FieldType(FieldType.Number), OrderNumber(38)]
        public int? GrossPalletWeightG { get; set; }

        [FieldType(FieldType.Number), OrderNumber(39)]
        public int? NetWeightPalletsG { get; set; }

        [FieldType(FieldType.Select, source: nameof(Companies)), OrderNumber(40)]
        public LookUpDto CompanyId { get; set; }
    }
}