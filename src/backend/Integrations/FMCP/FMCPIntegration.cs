using System.Collections;
using System.Collections.Generic;
using DAL.Services;
using Domain.Persistables;

namespace Integrations.FMCP
{
    public class FMCPIntegration : ConnectorBase
    {
        public FMCPIntegration(User user, ICommonDataService dataService) : base(
            "https://fm-client.devlogics.ru/api/", 
            user.FmCPLogin, 
            user.PoolingPassword, 
            dataService)
        {
            _accessToken = Post<dynamic>("user/login", new {email = user.FmCPLogin, user.FmCPPassword}).data.token;
        }

        public string CreateWaybill(Shipping shipping)
        {
            return Get<dynamic>("waybill/create").data.id;
        }

        public IEnumerable<string> Waybills(Shipping shipping)
        {
            Get<dynamic>("waybills");//.data.Where(x=>x.id == shipping.Id);
            return null;
        }

        public byte[] DownloadFile(Shipping shipping)
        {
            return null;
        }
    }
}