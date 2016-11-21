/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160618
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160618 18:25
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
    /// 操作类型
    /// </summary>
    public enum OperationType
    {
        [Label("按钮")]
        Button = 0,

        [Label("数据")]
        Data = 1
    }
}
