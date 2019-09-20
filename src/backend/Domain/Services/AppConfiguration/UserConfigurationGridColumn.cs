using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.AppConfiguration
{
    public class UserConfigurationGridColumn
    {
        public UserConfigurationGridColumn(string name, FiledType type)
        {
            Name = name.ToLowerfirstLetter();
            Type = type.ToString();
        }

        public string Name { get; set; }
        public string Type { get; set; }
    }
}