namespace Domain.Services.Articles
{
    public class ArticleDto : IDto
    {
        public string Id { get; set; }
        public string SPGR { get; set; }
        public string Description { get; set; }
        public string Nart { get; set; }
        public string CountryOfOrigin { get; set; }
        public int ShelfLife { get; set; }
        public string Status { get; set; }
        public string Ean { get; set; }
        public int UnitLengthGoodsMm { get; set; }
        public int WidthUnitsGoodsMm { get; set; }
        public int UnitHeightGoodsMm { get; set; }
        public int WeightUnitsGrossProductG { get; set; }
        public int WeightUnitsNetGoodsG { get; set; }
        public int EANShrink { get; set; }
        public int PiecesInShrink { get; set; }
        public int LengthShrinkMm { get; set; }
        public int WidthShrinkMm { get; set; }
        public int HeightShrinkMm { get; set; }
        public int GrossShrinkWeightG { get; set; }
        public int NetWeightShrinkG { get; set; }
        public string EANBox { get; set; }
        public int PiecesInABox { get; set; }
        public int BoxLengthMm { get; set; }
        public int WidthOfABoxMm { get; set; }
        public int BoxHeightMm { get; set; }
        public int GrossBoxWeightG { get; set; }
        public int NetBoxWeightG { get; set; }
        public int PiecesInALayer { get; set; }
        public int LayerLengthMm { get; set; }
        public int LayerWidthMm { get; set; }
        public int LayerHeightMm { get; set; }
        public int GrossLayerWeightMm { get; set; }
        public int NetWeightMm { get; set; }
        public int EANPallet { get; set; }
        public int PiecesOnAPallet { get; set; }
        public int PalletLengthMm { get; set; }
        public int WidthOfPalletsMm { get; set; }
        public int PalletHeightMm { get; set; }
        public int GrossPalletWeightG { get; set; }
        public int NetWeightPalletsG { get; set; }
        /*end of fields*/
    }
}