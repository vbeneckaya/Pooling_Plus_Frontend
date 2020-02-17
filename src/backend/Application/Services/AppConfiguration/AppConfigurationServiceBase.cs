using System;

namespace Application.Services.AppConfiguration
{
    public class AppConfigurationServiceBase
    {
        protected string GetName<T>()
        {
            return ToLowerfirstLetter(typeof(T).Name.Replace("Service", ""));
        }
                
        protected static string ToLowerfirstLetter(string input)
        {
            return Char.ToLowerInvariant(input[0]) + input.Substring(1);
        }        

    }
}