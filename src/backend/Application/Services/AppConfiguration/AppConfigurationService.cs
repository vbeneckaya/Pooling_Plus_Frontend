using System.Collections.Generic;
using Application.Services.Orders;
using Domain.Enums;
using Domain.Services.AppConfiguration;

namespace Application.Services.AppConfiguration
{
    
    public class AppConfigurationService : IAppConfigurationService 
    {
        
        public AppConfigurationService()
        {
        }
        
        public AppConfigurationDto GetConfiguration()
        {
            return new AppConfigurationDto
            {
                EditUsers = true,
                EditRoles = true,
                Grids = new List<UserConfigurationGridItem>
                {
                    new UserConfigurationGridItem
                    {
                        Name = GetName<OrdersService>(), 
                        CanCreateByForm = true, 
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn("Incoming", FiledType.Text)
                        }
                    },
                    new UserConfigurationGridItem
                    {
                        Name = "Transportations", 
                        CanCreateByForm = false, 
                        CanImportFromExcel = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn("From", FiledType.Text),
                            new UserConfigurationGridColumn("To", FiledType.Text)
                        }
                    },
                }, 
                Dictionaries = new List<UserConfigurationDictionaryItem>
                {
                    new UserConfigurationDictionaryItem
                    {
                        Name = "Tariff",
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn("Name", FiledType.Text),
                        }
                    },
                }                
            };
        }

        private string GetName<T>()
        {
            return typeof(T).Name.Replace("Service", "");
        }
    }     
}