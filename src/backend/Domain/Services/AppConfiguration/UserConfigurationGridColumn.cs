using Domain.Extensions;
using Domain.Services.FieldProperties;

namespace Domain.Services.AppConfiguration
{
    public class UserConfigurationGridColumn
    {

        public UserConfigurationGridColumn(FieldInfo field)
        {
            Name = field.Name.ToLowerFirstLetter();
            DisplayNameKey = field.DisplayNameKey;
            Type = field.FieldType.ToString();
            IsDefault = field.IsDefault;
            IsFixedPosition = field.IsFixedPosition;
            IsRequired = field.IsRequired;
            IsReadOnly = field.IsReadOnly;
        }

        public string Name { get; set; }
        public string DisplayNameKey { get; set; }
        public string Type { get; set; }
        public bool IsDefault { get; set; }
        public bool IsFixedPosition { get; set; }
        public bool IsRequired { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public class UserConfigurationGridColumnWhitchSource : UserConfigurationGridColumn
    {
        public string Source { get; }
        public bool ShowRawValue { get; set; }

        public UserConfigurationGridColumnWhitchSource(FieldInfo field) 
            : base(field)
        {
            Source = field.ReferenceSource.Replace("Service", "").ToLowerFirstLetter();
            ShowRawValue = field.ShowRawReferenceValue;
        }
    }
}