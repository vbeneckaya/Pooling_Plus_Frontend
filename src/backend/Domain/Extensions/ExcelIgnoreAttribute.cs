using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.Excel.Columns
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelIgnoreAttribute: Attribute
    {
    }
}
