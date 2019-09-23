using System;
using System.Collections.Generic;
using Domain.Extensions;
using Domain.Services.AppConfiguration;
using Domain.Services.Orders;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api")]
    public class AppConfigurationController : Controller
    {
        private readonly IAppConfigurationService appConfigurationService;

        public AppConfigurationController(IAppConfigurationService appConfigurationService)
        {
            this.appConfigurationService = appConfigurationService;
        }
        /// <summary>
        /// Получение конфигурации гридов и справочников
        /// </summary>
        [HttpGet("appConfiguration")] 
        public AppConfigurationDto Configuration()
        {
            return appConfigurationService.GetConfiguration();
        }

        /// <summary>
        /// Получение конфигурации гридов и справочников
        /// </summary>
        [HttpGet("getFormFor/{name}/{id}")] 
        public AppForm GetFormFor(string name, Guid id)
        {
            return new AppForm{
                Title = "orderEditForm {{number}}",
                LeftActions = new List<AppFormButton>
                {
                    new AppFormButton
                    {
                        Name = "openShipping {{number}}",
                        Type = AppFormActionType.OpenModal
                    }
                },
                RigthActions = new List<AppFormButton>
                {
                    new AppFormButton
                    {
                        Name = "removeFromShipping",
                        
                        Type = AppFormActionType.OpenModal,
                    },
                    new AppFormButton
                    {
                        Name = "cancel",
                        Type = AppFormActionType.Close
                    },
                    new AppFormButton
                    {
                        Name = "save",
                        Type = AppFormActionType.SaveAndClose,
                    },
                },
                Tabs = new List<AppFormTab>
                {
                    new AppFormTab
                    {
                        Name = "information",
                        Views = new List<IAppFormView>
                        {
                            new AppFormViewRow(new List<IAppFormViewRowField>
                            {
                                new AppFormTextField(nameof(OrderDto.SalesOrderNumber).ToLowerfirstLetter(),true),
                                new AppFormStateField(nameof(OrderDto.Status).ToLowerfirstLetter(), true, "orderState"),
                                new AppFormDateField("createAt", true),
                                new AppFormSelectField("orderType", true, "orderType"),
                            }),
                            new AppFormViewRow(new List<IAppFormViewRowField>
                            {
                                new AppFormTextField("payer",true),
                                new AppFormTextField("client", false),
                                new AppFormTextField("soldTo", false),
                                new AppFormSelectField("complectationType", true, "complectationType"),
                                new AppFormSelectField("termType", true, "termType"),
                            }),
                            new AppFormViewRow(new List<IAppFormViewRowField>
                            {
                                new FieldGroupeer ("Route"){
                                    Fields = new List<IAppFormViewRowField>
                                    {
                                        new AppFormTextField("addressFrom", true),
                                        new AppFormTextField("addressFo", true),
                                    }
                                }
                            }),
                            new AppFormViewRow(new List<IAppFormViewRowField>
                            {
                                new FieldGroupeer ("palletsCount"){
                                    Fields = new List<IAppFormViewRowField>
                                    {
                                        new AppFormNumberField("prepare", true),
                                        new AppFormNumberField("plan", true),
                                        new AppFormNumberField("fact", true),
                                    }
                                },
                                new FieldGroupeer ("boxesCount"){
                                    Fields = new List<IAppFormViewRowField>
                                    {
                                        new AppFormNumberField("prepare", true),
                                        new AppFormNumberField("fact", true),
                                    }
                                },
                                new FieldGroupeer ("weigth"){
                                    Fields = new List<IAppFormViewRowField>
                                    {
                                        new AppFormNumberField("plan", true),
                                        new AppFormNumberField("fact", true),
                                    }
                                },
                            })
                        }
                    },
                }
            };
        }
    }

    public class FieldGroupeer : IAppFormViewRowField
    {
        public FieldGroupeer(string name)
        {
            Name = name;
            Type = AppFormViewRowFieldType.Group.ToString();
        }

        public string Name { get; }
        public string Type { get; }
        public IEnumerable<IAppFormViewRowField> Fields { get; set; }
    }

    public class AppFormDateField : FieldBase, IAppFormViewRowField
    {
        public string Name { get; }
        public string Type { get; }
        public bool IsEdit { get; }
        
        public AppFormDateField(string name, bool isEdit)
        {
            Name = name;
            IsEdit = isEdit;
            Type = AppFormViewRowFieldType.Date.ToString();
        }
    }

    public class AppFormStateField : FieldBase, IAppFormViewRowField
    {
        public string Name { get; }
        public string Type { get; }
        public bool IsEdit { get; }
        public string SourceName { get; }

        public AppFormStateField(string name, bool isEdit, string sourceName)
        {
            Name = name;
            IsEdit = isEdit;
            SourceName = sourceName;
            Type = AppFormViewRowFieldType.State.ToString();
        }
    }

    public class AppFormSelectField : FieldBase, IAppFormViewRowField
    {
        public string Name { get; }
        public string Type { get; }
        public bool IsEdit { get; }
        public string SourceName { get; }

        public AppFormSelectField (string name, bool isEdit, string sourceName)
        {
            Name = name;
            IsEdit = isEdit;
            SourceName = sourceName;
            Type = AppFormViewRowFieldType.Select.ToString();
        }
    }

    public enum AppFormViewRowFieldType
    {
        Number,
        Float,
        Text,
        State,
        Select,
        Date,
        DateTime,
        Time,
        Group
    }

    public class AppFormNumberField : FieldBase, IAppFormViewRowField 
    {
        public string Name { get; }
        public string Type { get; }
        public bool IsEdit { get; }

        public AppFormNumberField (string name, bool isEdit) 
        {
            Name = name;
            Type = Type = AppFormViewRowFieldType.Number.ToString();;
            IsEdit = isEdit;
        }
    }
    public class AppFormFloatField : FieldBase, IAppFormViewRowField 
    {
        public string Name { get; }
        public string Type { get; }
        public bool IsEdit { get; }

        public AppFormFloatField (string name, bool isEdit) 
        {
            Name = name;
            Type = Type = AppFormViewRowFieldType.Float.ToString();;
            IsEdit = isEdit;
        }
    }

    public class AppFormTextField : FieldBase, IAppFormViewRowField 
    {
        public string Name { get; }
        public string Type { get; }
        public bool IsEdit { get; }

        public AppFormTextField(string name, bool isEdit) 
        {
            Name = name;
            Type = Type = AppFormViewRowFieldType.Text.ToString();;
            IsEdit = isEdit;
        }
    }

    public abstract class FieldBase : IAppFormViewRowField
    {
        public string Name { get; }
        public string Type { get; }
        public string IsEdit { get; }
    }


    public interface IAppFormViewRowField
    {
        string Name { get; }
        string Type { get; }
    }

    public class AppFormViewRow : IAppFormView
    {
        public List<IAppFormViewRowField> Fields { get; }
        public string Type { get; set; }


        public AppFormViewRow(List<IAppFormViewRowField> fields)
        {
            Fields = fields;
            Type = "ROW_TAB_TYPE";
        }
    }

    public interface IAppFormView
    {
        string Type { get; set; }
    }
    

    public enum AppFormActionType
    {
        Close,
        Save,
        SaveAndClose,
        OpenModal,
        Action
    }

    public class AppForm
    {
        public string Title { get; set; }
        public IEnumerable<AppFormTab> Tabs { get; set; }
        public IEnumerable<AppFormButton> LeftActions { get; set; }
        public IEnumerable<AppFormButton> RigthActions { get; set; }
    }

    public class AppFormButton
    {
        public string Name { get; set; }
        public AppFormActionType Type { get; set; }
    }

    public class AppFormTab
    {
        public string Name { get; set; }
        public List<IAppFormView> Views { get; set; }
    }
}