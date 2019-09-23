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
        public string SPGR { get; set; }
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
        /// Срок годности
        /// </summary>
        public string ShelfLife { get; set; }
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
        public string UnitLengthGoodsMm { get; set; }
        /// <summary>
        /// Ширина ед. товара, мм
        /// </summary>
        public int WidthUnitsGoodsMm { get; set; }
        /// <summary>
        /// Высота ед. товара, мм
        /// </summary>
        public string UnitHeightGoodsMm { get; set; }
        /// <summary>
        /// Вес ед. товара брутто, г
        /// </summary>
        public string WeightUnitsGrossProductG { get; set; }
        /// <summary>
        /// Вес ед. товара нетто, г
        /// </summary>
        public int WeightUnitsNetGoodsG { get; set; }
        /// <summary>
        /// EAN, shrink
        /// </summary>
        public string EANShrink { get; set; }
        /// <summary>
        /// Штук в shrink
        /// </summary>
        public string PiecesInShrink { get; set; }
        /// <summary>
        /// Длина shrink, мм
        /// </summary>
        public string LengthShrinkMm { get; set; }
        /// <summary>
        /// Ширина shrink, мм
        /// </summary>
        public string WidthShrinkMm { get; set; }
        /// <summary>
        /// Высота shrink, мм
        /// </summary>
        public string HeightShrinkMm { get; set; }
        /// <summary>
        /// Вес shrink брутто, г
        /// </summary>
        public string GrossShrinkWeightG { get; set; }
        /// <summary>
        /// Вес shrink нетто, г
        /// </summary>
        public string NetWeightShrinkG { get; set; }
        /// <summary>
        /// EAN, короб
        /// </summary>
        public string EANBox { get; set; }
        /// <summary>
        /// Штук в коробе
        /// </summary>
        public string PiecesInABox { get; set; }
        /// <summary>
        /// Длина короба, мм
        /// </summary>
        public string BoxLengthMm { get; set; }
        /// <summary>
        /// Ширина короба, мм
        /// </summary>
        public string WidthOfABoxMm { get; set; }
        /// <summary>
        /// Высота короба, мм
        /// </summary>
        public string BoxHeightMm { get; set; }
        /// <summary>
        /// Вес короба брутто, г
        /// </summary>
        public string GrossBoxWeightG { get; set; }
        /// <summary>
        /// Вес короба нетто, г
        /// </summary>
        public string NetBoxWeightG { get; set; }
        /// <summary>
        /// Штук в слое
        /// </summary>
        public string PiecesInALayer { get; set; }
        /// <summary>
        /// Длина слоя, мм
        /// </summary>
        public string LayerLengthMm { get; set; }
        /// <summary>
        /// Ширина слоя, мм
        /// </summary>
        public string LayerWidthMm { get; set; }
        /// <summary>
        /// Высота слоя, мм
        /// </summary>
        public string LayerHeightMm { get; set; }
        /// <summary>
        /// Вес слоя брутто, мм
        /// </summary>
        public string GrossLayerWeightMm { get; set; }
        /// <summary>
        /// Вес слоя нетто, мм
        /// </summary>
        public string NetWeightMm { get; set; }
        /// <summary>
        /// EAN, паллета
        /// </summary>
        public string EANPallet { get; set; }
        /// <summary>
        /// Штук на паллете
        /// </summary>
        public string PiecesOnAPallet { get; set; }
        /// <summary>
        /// Длина паллеты, мм
        /// </summary>
        public string PalletLengthMm { get; set; }
        /// <summary>
        /// Ширина паллеты, мм
        /// </summary>
        public string WidthOfPalletsMm { get; set; }
        /// <summary>
        /// Высота паллеты, мм
        /// </summary>
        public string PalletHeightMm { get; set; }
        /// <summary>
        /// Вес паллеты брутто, г
        /// </summary>
        public string GrossPalletWeightG { get; set; }
        /// <summary>
        /// Вес паллеты нетто, г
        /// </summary>
        public string NetWeightPalletsG { get; set; }
        /*end of fields*/
    }
}