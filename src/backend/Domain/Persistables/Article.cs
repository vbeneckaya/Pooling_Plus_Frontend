using System;

namespace Domain.Persistables
{
    public class Article : IPersistable
    {
        public Guid Id { get; set; }
        public string SPGR { get; set; }
        public string Description { get; set; }
        public string Nart { get; set; }
        public string CountryOfOrigin { get; set; }
        public string ShelfLife { get; set; }
        public string Status { get; set; }
        public string Ean { get; set; }
        public string UnitLengthGoodsMm { get; set; }
        public int WidthUnitsGoodsMm { get; set; }
        public string UnitHeightGoodsMm { get; set; }
        public string WeightUnitsGrossProductG { get; set; }
        public int WeightUnitsNetGoodsG { get; set; }
        public string EANShrink { get; set; }
        public string PiecesInShrink { get; set; }
        public string LengthShrinkMm { get; set; }
        public string WidthShrinkMm { get; set; }
        public string HeightShrinkMm { get; set; }
        public string GrossShrinkWeightG { get; set; }
        public string NetWeightShrinkG { get; set; }
        public string EANBox { get; set; }
        public string PiecesInABox { get; set; }
        public string BoxLengthMm { get; set; }
        public string WidthOfABoxMm { get; set; }
        public string BoxHeightMm { get; set; }
        public string GrossBoxWeightG { get; set; }
        public string NetBoxWeightG { get; set; }
        public string PiecesInALayer { get; set; }
        public string LayerLengthMm { get; set; }
        public string LayerWidthMm { get; set; }
        public string LayerHeightMm { get; set; }
        public string GrossLayerWeightMm { get; set; }
        public string NetWeightMm { get; set; }
        public string EANPallet { get; set; }
        public string PiecesOnAPallet { get; set; }
        public string PalletLengthMm { get; set; }
        public string WidthOfPalletsMm { get; set; }
        public string PalletHeightMm { get; set; }
        public string GrossPalletWeightG { get; set; }
        public string NetWeightPalletsG { get; set; }
        /*end of fields*/
    }
}