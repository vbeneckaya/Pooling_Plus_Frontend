using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.Tariffs
{
    public class TariffDto : IDto
    {
        public string Id { get; set; }

        [IgnoreOnRoleType(RoleTypes.Provider)]
        [FieldType(FieldType.Select, source: nameof(Providers)), OrderNumber(0), IsFixedPosition, IsRequired]
        public LookUpDto ProviderId { get; set; }

        [FieldType(FieldType.Select, source: nameof(TransportCompanies)), OrderNumber(1), IsFixedPosition, IsRequired]
        public LookUpDto CarrierId { get; set; }
        
        [FieldType(FieldType.Select, source: nameof(ShippingWarehouses)), OrderNumber(2), IsFixedPosition, IsRequired]
        public LookUpDto ShippingWarehouseId { get; set; }

        [FieldType(FieldType.Select, source: nameof(Warehouses)), OrderNumber(3), IsFixedPosition, IsRequired]
        public LookUpDto DeliveryWarehouseId { get; set; }

        [FieldType(FieldType.Select, source: nameof(VehicleTypes)), OrderNumber(4)]
        public LookUpDto VehicleTypeId { get; set; }

        [FieldType(FieldType.Enum, source: nameof(Enums.TarifficationType)), OrderNumber(5)]
        public LookUpDto TarifficationType { get; set; }

        public LookUpDto BodyTypeId { get; set; }

        public string StartWinterPeriod { get; set; }

        public string EndWinterPeriod { get; set; }

        [FieldType(FieldType.Date), OrderNumber(8), IsRequired]
        public string EffectiveDate { get; set; }

        [FieldType(FieldType.Date), OrderNumber(9), IsRequired]
        public string ExpirationDate { get; set; }

        public string WinterAllowance { get; set; }

        [FieldType(FieldType.Number), OrderNumber(11)]
        public decimal? FtlRate { get; set; }

        [FieldType(FieldType.Number), OrderNumber(12)]
        public decimal? LtlRate1 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(13)]
        public decimal? LtlRate2 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(14)]
        public decimal? LtlRate3 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(15)]
        public decimal? LtlRate4 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(16)]
        public decimal? LtlRate5 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(17)]
        public decimal? LtlRate6 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(18)]
        public decimal? LtlRate7 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(19)]
        public decimal? LtlRate8 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(20)]
        public decimal? LtlRate9 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(21)]
        public decimal? LtlRate10 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(22)]
        public decimal? LtlRate11 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(23)]
        public decimal? LtlRate12 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(24)]
        public decimal? LtlRate13 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(25)]
        public decimal? LtlRate14 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(26)]
        public decimal? LtlRate15 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(27)]
        public decimal? LtlRate16 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(28)]
        public decimal? LtlRate17 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(29)]
        public decimal? LtlRate18 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(30)]
        public decimal? LtlRate19 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(31)]
        public decimal? LtlRate20 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(32)]
        public decimal? LtlRate21 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(33)]
        public decimal? LtlRate22 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(34)]
        public decimal? LtlRate23 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(35)]
        public decimal? LtlRate24 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(36)]
        public decimal? LtlRate25 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(37)]
        public decimal? LtlRate26 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(38)]
        public decimal? LtlRate27 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(39)]
        public decimal? LtlRate28 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(40)]
        public decimal? LtlRate29 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(41)]
        public decimal? LtlRate30 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(42)]
        public decimal? LtlRate31 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(43)]
        public decimal? LtlRate32 { get; set; }

        [FieldType(FieldType.Number), OrderNumber(44)]
        public decimal? LtlRate33 { get; set; }

        public bool IsEditable { get; set; }
    }
}