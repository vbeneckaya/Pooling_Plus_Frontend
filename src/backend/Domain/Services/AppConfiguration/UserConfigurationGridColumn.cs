using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.AppConfiguration
{
    public class UserConfigurationGridColumn
    {

        public UserConfigurationGridColumn(string name, FieldType type, bool isDefault = false, bool isFixedPosition = false)
        {
            Name = name.ToLowerFirstLetter();
            Type = type.ToString();
            IsDefault = isDefault;
            IsFixedPosition = isFixedPosition;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsDefault { get; set; }
        public bool IsFixedPosition { get; set; }
    }

    public class UserConfigurationGridColumnWhitchSource : UserConfigurationGridColumn
    {
        public string Source { get; }
        public bool ShowRawValue { get; set; }

        public UserConfigurationGridColumnWhitchSource(string name, FieldType type, string source, bool isDefault = false, bool showRawValue = false, bool isFixedPosition = false) 
            : base(name, type, isDefault, isFixedPosition)
        {
            Source = source.Replace("Service", "").ToLowerFirstLetter();
            ShowRawValue = showRawValue;
        }
    }
}