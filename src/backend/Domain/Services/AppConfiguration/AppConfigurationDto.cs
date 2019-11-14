using System.Collections.Generic;

namespace Domain.Services.AppConfiguration
{
    public class AppConfigurationDto
    {
        public IEnumerable<UserConfigurationGridItem> Grids { get; set; }
        public IEnumerable<UserConfigurationDictionaryItem> Dictionaries { get; set; }
        public bool EditUsers { get; set; }
        public bool EditRoles { get; set; }
        public bool EditFieldProperties { get; set; }
    }
}