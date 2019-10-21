using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.AppConfiguration
{
    public class UserConfigurationGridColumn
    {

        public UserConfigurationGridColumn(string name, FiledType type, bool isDefault = false)
        {
            Name = name.ToLowerfirstLetter();
            Type = type.ToString();
            IsDefault = isDefault;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsDefault { get; set; }
    }

    public class UserConfigurationGridColumnWhitchSource : UserConfigurationGridColumn
    {
        public string Source { get; }
        public bool ShowRawValue { get; set; }

        public UserConfigurationGridColumnWhitchSource(string name, FiledType type, string source, bool isDefault = false, bool showRawValue = false) 
            : base(name, type, isDefault)
        {
            Source = source.Replace("Service", "").ToLowerfirstLetter();
            ShowRawValue = showRawValue;
        }
    }
}