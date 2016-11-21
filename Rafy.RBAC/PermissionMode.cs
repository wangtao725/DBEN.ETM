/*******************************************************
 * 
 * 作者：刘迁
 * 创建日期：20160316
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 刘迁 20160316 10:28
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafy.RBAC
{
    /// <summary>
    /// 权限管理模式
    /// </summary>
    [Flags]
    public enum PermissionMode
    {
        /// <summary>
        /// 用户授权
        /// </summary>
        User = 1,

        /// <summary>
        /// 组织结构授权
        /// </summary>
        Org = 2,

        /// <summary>
        /// 岗位授权
        /// </summary>
        Position = 4,
    }
}
