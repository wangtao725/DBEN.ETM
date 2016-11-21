//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using DBEN.VATEngine;
//using DBEN.WF;
//using Rafy;
//using Rafy.Accounts;
//using Rafy.Data;
//using Rafy.Domain.EntityPhantom;
//using Rafy.Domain.Stamp;
//using Rafy.RBAC;
//using Rafy.Web;

//namespace DBEN.ETM.Web
//{
//    class ETMWebApp : WebApp
//    {
//        protected override void InitEnvironment()
//        {
//            RafyEnvironment.DomainPlugins.Add(new StampPlugin());
//            RafyEnvironment.DomainPlugins.Add(new EntityPhantomPlugin());
//            RafyEnvironment.DomainPlugins.Add(new AccountsPlugin());
//            RafyEnvironment.DomainPlugins.Add(new RBACPlugin());
//            RafyEnvironment.DomainPlugins.Add(new WFPlugin());
//            RafyEnvironment.DomainPlugins.Add(new DBIPlugin());
//            RafyEnvironment.DomainPlugins.Add(new VATEnginePlugin());

//            this.AddCustomiedProject();

//            AccountsPlugin.DbSettingName =
//                WFPlugin.DbSettingName =
//                RBACPlugin.DbSettingName =
//                VATEnginePlugin.DbSettingName;

//            this.DecryptConnectionString();

//            base.InitEnvironment();
//        }

//        protected virtual void AddCustomiedProject() { }

//        /// <summary>
//        /// 如果配置文件的给connection加密属性设置为ture，那么程序运行会走这个方法
//        /// 这个方法通过AES加解密来保护connection不被发现，默认的key是DBEN
//        /// </summary>
//        protected virtual void DecryptConnectionString()
//        {
//            if (ConfigurationHelper.GetAppSettingOrDefault("DBEN.DBI.Data.UseEncryptedConnectionString", false))
//            {
//                var settingName = VATEnginePlugin.DbSettingName;
//                var settings = DbSetting.FindOrCreate(settingName);
//                var encriptionStr = settings.ConnectionString;//获取connection
//                var decriptionStr = Common.DescriptAES.AESDecrypt(encriptionStr, "DBEN");//解密后的connection
//                DbSetting.SetSetting(settingName, decriptionStr, settings.ProviderName);
//            }
//        }

//        protected override void OnRuntimeStarting()
//        {
//            base.OnRuntimeStarting();

//            DbAutoUpdater.Update();
//        }
//        protected override void OnStartupCompleted()
//        {
//            base.OnStartupCompleted();
//            RafyEntityJScriptHandler rafyEntityScript = new RafyEntityJScriptHandler();
//            rafyEntityScript.GenerateScript();
//        }
//    }
//}