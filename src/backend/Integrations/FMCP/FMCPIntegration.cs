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
            _accessToken = Post("user/login", new
                {
                    email = user.FmCPLogin, 
                    password = user.FmCPPassword
                })
                .Get("$.data.token");
        }

        public string CreateWaybill(Shipping shipping)
        {
            var id = Post("waybill/create").Get("$.id");
            return id;
        }

        public IEnumerable<string> Update(Shipping shipping)
        {
            GetArr("waybills").Get("$.data.");//.data.Where(x=>x.id == shipping.Id);
            return null;
        }

        public byte[] GetSticker(Shipping shipping)
        {
            var downloadFileUrl = Get($"waybills/{shipping.FmcpWaybillId}/stickers")
                .Get("data.url");
            return DownloadFile(downloadFileUrl);
        }

        public void Sinhronize()
        {
            GetArr("waybills").Get("$.data.");//.data.Where(x=>x.id == shipping.Id);
        }


        
    }
}