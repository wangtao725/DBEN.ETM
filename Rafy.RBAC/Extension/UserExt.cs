/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160417
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160417 13:55
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Rafy;
using Rafy.Accounts;
using Rafy.ComponentModel;
using Rafy.Data;
using Rafy.Domain;
using Rafy.Domain.ORM;
using Rafy.Domain.ORM.Query;
using Rafy.Domain.Validation;
using Rafy.ManagedProperty;
using Rafy.MetaModel;
using Rafy.MetaModel.Attributes;
using Rafy.MetaModel.View;

namespace Rafy.RBAC
{
    [CompiledPropertyDeclarer]
    public static class UserExt
    {
        #region string EmployeeNumber (员工编号)

        /// <summary>
        /// 员工编号 扩展属性。
        /// </summary>
        public static readonly Property<string> EmployeeNumberProperty =
            P<User>.RegisterExtension<string>("EmployeeNumber", typeof(UserExt));
        /// <summary>
        /// 获取 员工编号 属性的值。
        /// </summary>
        public static string GetEmployeeNumber(this User me)
        {
            return me.GetProperty(EmployeeNumberProperty);
        }
        /// <summary>
        /// 设置 员工编号 属性的值。
        /// </summary>
        public static void SetEmployeeNumber(this User me, string value)
        {
            me.SetProperty(EmployeeNumberProperty, value);
        }

        public static readonly Property<string> TerminalNumberProperty =
            P<User>.RegisterExtension<string>("TerminalNumber", typeof(UserExt));
        public static string GetTerminalNumber(this User me)
        {
            return me.GetProperty(TerminalNumberProperty);
        }
        /// <summary>
        /// 终端号
        /// </summary>
        /// <param name="me"></param>
        /// <param name="value"></param>
        public static void SetTerminalNumber(this User me, string value)
        {
            me.SetProperty(TerminalNumberProperty, value);
        }

        #region string Compellation (姓名)

        /// <summary>
        /// 姓名 扩展属性。
        /// </summary>
        public static readonly Property<string> CompellationProperty =
            P<User>.RegisterExtension<string>("Compellation", typeof(UserExt));
        /// <summary>
        /// 获取 姓名 属性的值。
        /// </summary>
        /// <param name="me">要获取扩展属性值的对象。</param>
        public static string GetCompellation(this User me)
        {
            return me.GetProperty(CompellationProperty);
        }
        /// <summary>
        /// 设置 姓名 属性的值。
        /// </summary>
        /// <param name="me">要设置扩展属性值的对象。</param>
        /// <param name="value">设置的值。</param>
        public static void SetCompellation(this User me, string value)
        {
            me.SetProperty(CompellationProperty, value);
        }

        #endregion

        #endregion

        #region UserRoleList UserRoleList (用户下的角色)

        /// <summary>
        /// 用户下的角色 扩展属性。
        /// </summary>
        public static ListProperty<UserRoleList> UserRoleListProperty =
            P<User>.RegisterListExtension<UserRoleList>("UserRoleList", typeof(UserExt));
        /// <summary>
        /// 获取 用户下的角色 属性的值。
        /// </summary>
        /// <param name="me">要获取扩展属性值的对象。</param>
        public static UserRoleList GetUserRoleList(this User me)
        {
            return me.GetLazyList(UserRoleListProperty) as UserRoleList;
        }

        #endregion
    }
}