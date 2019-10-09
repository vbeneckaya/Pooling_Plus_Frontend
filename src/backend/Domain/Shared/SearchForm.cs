using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Shared
{
    /// <summary>
    /// Basic Search Form
    /// </summary>
    public class SearchForm: PagingForm
    {
        public string Search { get; set; }
    }
}
