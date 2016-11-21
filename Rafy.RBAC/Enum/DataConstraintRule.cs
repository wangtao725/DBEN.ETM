/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160619
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160619 14:29
 * 
*******************************************************/

using Rafy.MetaModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafy.RBAC.Enum
{
    /// <summary>
    /// 此枚举用于
    /// </summary>
    public enum DataConstraintRule
    {
        [Label("查看自己的")]
        Own = 1,

        [Label("自己组织的")]
        OwnCompany = 2,

        [Label("查看下级组织的")]
        Lower = 3,

        [Label("查看全部的")]
        All = 0
    }
}
