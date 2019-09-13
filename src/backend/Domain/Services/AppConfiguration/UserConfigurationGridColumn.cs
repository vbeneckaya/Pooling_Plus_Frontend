using Domain.Enums;

namespace Domain.Services.AppConfiguration
{
    public class UserConfigurationGridColumn
    {
        public UserConfigurationGridColumn(string name, FiledType type)
        {
            Name = name;
            Type = type.ToString();
        }

        public string Name { get; set; }
        public string Type { get; set; }
    }
}