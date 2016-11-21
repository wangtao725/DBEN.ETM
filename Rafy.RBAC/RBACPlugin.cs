using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rafy;
using Rafy.ComponentModel;
using Rafy.DbMigration;
using Rafy.Domain;
using Rafy.Domain.ORM.DbMigration;

namespace Rafy.RBAC
{
    public class RBACPlugin : DomainPlugin
    {
        /// <summary>
        /// 数据库连接配置
        /// </summary>
        public static string DbSettingName { get; set; } = "RBAC";

        /// <summary>
        /// 权限管理模式
        /// </summary>
        public static PermissionMode PermissionMode { get; set; }

        public override void Initialize(IApp app)
        {
        }
    }
}
