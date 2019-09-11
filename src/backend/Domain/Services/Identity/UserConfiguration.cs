using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Services.Identity
{
    public class UserConfiguration
    {
        public IEnumerable<UserConfigurationGridItem> Grids { get; set; }
        public IEnumerable<UserConfigurationDictionaryItem> Dictionaries { get; set; }
        public string UserName { get; set; }

        public string UserRole { get; set; }
    }

    public class UserConfigurationGridItem
    {
        public string Name { get; set; }
        public bool CanCreateByForm { get; set; }
        public bool CanImportFromExcel { get; set; }
        public IEnumerable<UserConfigurationGridColumn> Columns { get; set; }
    }

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

    public class UserConfigurationDictionaryItem
    {
        public string Name { get; set; }
        public bool CanCreateByForm { get; set; }
        public bool CanImportFromExcel { get; set; }
        public IEnumerable<UserConfigurationGridColumn> Columns { get; set; }
    }
}