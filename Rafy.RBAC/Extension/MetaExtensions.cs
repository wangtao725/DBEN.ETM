/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160319
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160319 13:55
 * 
*******************************************************/

using Rafy.MetaModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafy.RBAC
{
    internal static class MetaExtensions
    {
        /// <summary>
        /// 指定某实体映射的表名。
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static EntityMeta MapTable(this EntityMeta meta, string tableName = null)
        {
            tableName = tableName ?? "RBAC_" + meta.EntityType.Name;

            Rafy.MetaModel.MetaExtension.MapTable(meta, tableName);

            return meta;
        }
    }
}
