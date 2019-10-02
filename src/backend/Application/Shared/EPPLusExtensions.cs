using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Domain.Extensions;
using OfficeOpenXml;

namespace Application.Shared
{
    public static class EPPLusExtensions
    {
        public static void ConvertObjectsToSheet<T>(this ExcelWorksheet worksheet, IEnumerable<T> dtos) where T : new()
        {
            var columns = Columns<T>();
            
            columns.ForEach(col =>
            {
                worksheet.Cells[1, col.Column].Value = col.Property.Name;
                worksheet.Cells[1, col.Column].Style.Font.Bold = true;
            });

            
            for (int i = 0; i < dtos.Count(); i++)
            {
                var dto = dtos.ElementAt(i);
                columns.ForEach(col =>
                {
                    var cell = worksheet.Cells[i + 2, col.Column];
                    if (col.Property.PropertyType == typeof(DateTime?))
                    {
                        var numberformatFormat = "dd.MM.yyyy HH:mm:ss";
                        
                        cell.Style.Numberformat.Format =  numberformatFormat;
                        var dateTime = ((DateTime?)col.Property.GetValue(dto));
                        if(dateTime.HasValue)
                            cell.Value = dateTime.Value.ToString(numberformatFormat);
                        return;
                    }

                    cell.Value = col.Property.GetValue(dto);
                    
                });
            }
        }

        public static IEnumerable<T> ConvertSheetToObjects<T>(this ExcelWorksheet worksheet, out string errors) where T : new()
        {
            StringBuilder errorsBuilder = new StringBuilder();

            Func<CustomAttributeData, bool> columnOnly = y => y.AttributeType == typeof(Column);

            var columns = Columns<T>();


            var rows = worksheet.Cells
                .Select(cell => cell.Start.Row)
                .Distinct()
                .OrderBy(x=>x);


            //Create the collection container
            var collection = rows.Skip(1)
                .Select(row =>
                {
                    var tnew = new T();
                    columns.ForEach(col =>
                    {
                        try
                        {
                            //This is the real wrinkle to using reflection - Excel stores all numbers as double including int
                            var val = worksheet.Cells[row, col.Column];
                            //If it is numeric it is a double since that is how excel stores all numbers
                            if (val.Value == null)
                            {
                                col.Property.SetValue(tnew, null);
                                return;
                            }
                            if (col.Property.PropertyType == typeof(Int32))
                            {
                                col.Property.SetValue(tnew, val.GetValue<int>());
                                return;
                            }
                            if (col.Property.PropertyType == typeof(Int32?))
                            {
                                col.Property.SetValue(tnew, val.GetValue<int?>());
                                return;
                            }
                            if (col.Property.PropertyType == typeof(double))
                            {
                                col.Property.SetValue(tnew, val.GetValue<double>());
                                return;
                            }
                            if (col.Property.PropertyType == typeof(DateTime))
                            {
                                col.Property.SetValue(tnew, val.GetValue<DateTime>());
                                return;
                            }
                            if (col.Property.PropertyType == typeof(DateTime?))
                            {
                                var value1 = val.GetValue<DateTime?>();
                                col.Property.SetValue(tnew, value1);
                                return;
                            }
                            //Its a string
                            col.Property.SetValue(tnew, val.GetValue<string>());
                        }
                        catch (Exception ex)
                        {
                            errorsBuilder.Append($"Строка {row}: {ex.Message}.");
                        }
                    });

                    return tnew;
                });


            //Send it back
            errors = errorsBuilder.ToString();
            return collection;
        }

        private static List<CustomColumnProperty> Columns<T>() where T : new()
        {
            var avalibleTypes = new List<Type>
            {
                typeof(string),
                typeof(int),
                typeof(int?),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(double),
            };

            var columns = typeof(T)
                .GetProperties()
                //.Where(x => x.CustomAttributes.Any(columnOnly))
                .Where(x => avalibleTypes.Contains(x.PropertyType))
                .Select((p, index) => new CustomColumnProperty
                {
                    Property = p,
                    Column = index + 1 //p.GetCustomAttributes<Column>().First().ColumnIndex //safe because if where above
                }).ToList();
            return columns;
        }
    }

    internal class CustomColumnProperty
    {
        public PropertyInfo Property { get; set; }
        public int Column { get; set; }
    }
}