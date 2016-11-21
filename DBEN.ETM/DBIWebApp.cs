using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rafy;
using Rafy.Data;
using Rafy.RBAC;
using Rafy.Web;
using DBEN.ETM.Common.Descript;

namespace DBEN.ETM.Web
{
    class ETMWebApp : WebApp
    {
        protected override void InitEnvironment()
        {

            RafyEnvironment.DomainPlugins.Add(new RBACPlugin());

            this.AddCustomiedProject();


            RBACPlugin.DbSettingName = "ETM";

            this.DecryptConnectionString();

            base.InitEnvironment();
        }

        protected virtual void AddCustomiedProject() { }

        /// <summary>
        /// 如果配置文件的给connection加密属性设置为ture，那么程序运行会走这个方法
        /// 这个方法通过AES加解密来保护connection不被发现，默认的key是DBEN
        /// </summary>
        protected virtual void DecryptConnectionString()
        {
            if (ConfigurationHelper.GetAppSettingOrDefault("DBEN.DBI.Data.UseEncryptedConnectionString", false))
            {
                var settingName = RBACPlugin.DbSettingName;// VATEnginePlugin.DbSettingName;
                var settings = DbSetting.FindOrCreate(settingName);
                var encriptionStr = settings.ConnectionString;//获取connection
                var decriptionStr = DescriptAES.AESDecrypt(encriptionStr, "DBEN");//解密后的connection
                DbSetting.SetSetting(settingName, decriptionStr, settings.ProviderName);
            }
        }

        //protected override void OnRuntimeStarting()
        //{
        //    base.OnRuntimeStarting();

        //    DbAutoUpdater.Update();
        //}
        //protected override void OnStartupCompleted()
        //{
        //    base.OnStartupCompleted();
        //    RafyEntityJScriptHandler rafyEntityScript = new RafyEntityJScriptHandler();
        //    rafyEntityScript.GenerateScript();
        //}
    }
}