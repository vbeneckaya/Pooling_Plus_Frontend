using System;
using Domain.Enums;

namespace Domain.Persistables
{   
    /// <summary>
    /// Артикул
    /// </summary>
    public class Article : IPersistable
    {
        /// <summary>
        /// Db primary key
        /// </summary>    
        public Guid Id { get; set; }
        /// <summary>
        /// SPGR
        /// </summary>
        public string Spgr { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// NART
        /// </summary>
        public string Nart { get; set; }
        /// <summary>
        /// Страна происхождения
        /// </summary>
        public string CountryOfOrigin { get; set; }
        /// <summary>
        /// Срок годности, дней
        /// </summary>
        public int? ShelfLife { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// EAN
        /// </summary>
        public string Ean { get; set; }
        /// <summary>
        /// Длина ед. товара, мм
        /// </summary>
        public int? UnitLengthGoodsMm { get; set; }
        /// <summary>
        /// Ширина ед. товара, мм
        /// </summary>
        public int? WidthUnitsGoodsMm { get; set; }
        /// <summary>
        /// Высота ед. товара, мм
        /// </summary>
        public int? UnitHeightGoodsMm { get; set; }
        /// <summary>
        /// Вес ед. товара брутто, г
        /// </summary>
        public int? WeightUnitsGrossProductG { get; set; }
        /// <summary>
        /// Вес ед. товара нетто, г
        /// </summary>
        public int? WeightUnitsNetGoodsG { get; set; }
        /// <summary>
        /// EAN, shrink
        /// </summary>
        public string EanShrink { get; set; }
        /// <summary>
        /// Штук в shrink
        /// </summary>
        public int? PiecesInShrink { get; set; }
        /// <summary>
        /// Длина shrink, мм
        /// </summary>
        public int? LengthShrinkMm { get; set; }
        /// <summary>
        /// Ширина shrink, мм
        /// </summary>
        public int? WidthShrinkMm { get; set; }
        /// <summary>
        /// Высота shrink, мм
        /// </summary>
        public int? HeightShrinkMm { get; set; }
        /// <summary>
        /// Вес shrink брутто, г
        /// </summary>
        public int? GrossShrinkWeightG { get; set; }
        /// <summary>
        /// Вес shrink нетто, г
        /// </summary>
        public int? NetWeightShrinkG { get; set; }
        /// <summary>
        /// EAN, короб
        /// </summary>
        public string EanBox { get; set; }
        /// <summary>
        /// Штук в коробе
        /// </summary>
        public int? PiecesInABox { get; set; }
        /// <summary>
        /// Длина короба, мм
        /// </summary>
        public int? BoxLengthMm { get; set; }
        /// <summary>
        /// Ширина короба, мм
        /// </summary>
        public int? WidthOfABoxMm { get; set; }
        /// <summary>
        /// Высота короба, мм
        /// </summary>
        public int? BoxHeightMm { get; set; }
        /// <summary>
        /// Вес короба брутто, г
        /// </summary>
        public int? GrossBoxWeightG { get; set; }
        /// <summary>
        /// Вес короба нетто, г
        /// </summary>
        public int? NetBoxWeightG { get; set; }
        /// <summary>
        /// Штук в слое
        /// </summary>
        public int? PiecesInALayer { get; set; }
        /// <summary>
        /// Длина слоя, мм
        /// </summary>
        public int? LayerLengthMm { get; set; }
        /// <summary>
        /// Ширина слоя, мм
        /// </summary>
        public int? LayerWidthMm { get; set; }
        /// <summary>
        /// Высота слоя, мм
        /// </summary>
        public int? LayerHeightMm { get; set; }
        /// <summary>
        /// Вес слоя брутто, мм
        /// </summary>
        public int? GrossLayerWeightMm { get; set; }
        /// <summary>
        /// Вес слоя нетто, мм
        /// </summary>
        public int? NetWeightMm { get; set; }
        /// <summary>
        /// EAN, паллета
        /// </summary>
        public string EanPallet { get; set; }
        /// <summary>
        /// Штук на паллете
        /// </summary>
        public int? PiecesOnAPallet { get; set; }
        /// <summary>
        /// Длина паллеты, мм
        /// </summary>
        public int? PalletLengthMm { get; set; }
        /// <summary>
        /// Ширина паллеты, мм
        /// </summary>
        public int? WidthOfPalletsMm { get; set; }
        /// <summary>
        /// Высота паллеты, мм
        /// </summary>
        public int? PalletHeightMm { get; set; }
        /// <summary>
        /// Вес паллеты брутто, г
        /// </summary>
        public int? GrossPalletWeightG { get; set; }
        /// <summary>
        /// Вес паллеты нетто, г
        /// </summary>
        public int? NetWeightPalletsG { get; set; }

        /// <summary>
        /// Юр. лицо
        /// </summary>
        public Guid? CompanyId { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}