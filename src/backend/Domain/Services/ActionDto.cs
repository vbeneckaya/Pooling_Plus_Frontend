using System.Collections.Generic;

namespace Domain.Services
{
    public class ActionDto
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string Color { get; set; }
        public IEnumerable<string> Ids { get; set; }
    }
}