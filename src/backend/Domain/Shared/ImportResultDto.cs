using Domain.Services.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Shared
{
    public class ImportResultDto
    {
        public string CreatedCountMessage { get; set; }

        public string UpdatedCountMessage { get; set; }

        public string RequiredErrorMessage { get; set; }

        public string InvalidDictionaryValueErrorMessage { get; set; }

        public string DuplicatedRecordErrorMessage { get; set; }

        public string InvalidFormatErrorCountMessage { get; set; }
    }
}
