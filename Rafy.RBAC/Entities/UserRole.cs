/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160417
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160417 21:56
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
using Rafy.RBAC;

namespace Rafy.RBAC
{
    /// <summary>
    /// 用户角色
    /// </summary>
    [ChildEntity, Serializable]
    public partial class UserRole : RBACEntity
    {
        #region 构造函数

        public UserRole() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected UserRole(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        public static readonly IRefIdProperty UserIdProperty =
            P<UserRole>.RegisterRefId(e => e.UserId, ReferenceType.Parent);
        public long UserId
        {
            get { return (long)this.GetRefId(UserIdProperty); }
            set { this.SetRefId(UserIdProperty, value); }
        }
        public static readonly RefEntityProperty<User> UserProperty =
            P<UserRole>.RegisterRef(e => e.User, UserIdProperty);
        public User User
        {
            get { return this.GetRefEntity(UserProperty); }
            set { this.SetRefEntity(UserProperty, value); }
        }

        public static readonly IRefIdProperty RoleIdProperty =
            P<UserRole>.RegisterRefId(e => e.RoleId, ReferenceType.Normal);
        public long RoleId
        {
            get { return (long)this.GetRefId(RoleIdProperty); }
            set { this.SetRefId(RoleIdProperty, value); }
        }
        public static readonly RefEntityProperty<Role> RoleProperty =
            P<UserRole>.RegisterRef(e => e.Role, RoleIdProperty);
        /// <summary>
        /// 角色
        /// </summary>
        public Role Role
        {
            get { return this.GetRefEntity(RoleProperty); }
            set { this.SetRefEntity(RoleProperty, value); }
        }

        #endregion

        #region 组合子属性

        #endregion

        #region 一般属性

        #endregion

        #region 只读属性

        /// <summary>
        /// 员工号
        /// </summary>
        public static readonly Property<string> RO_EmployeeNumberProperty = P<UserRole>.RegisterReadOnly(
            e => e.RO_EmployeeNumber, e => e.GetRO_EmployeeNumber());
        public string RO_EmployeeNumber
        {
            get { return this.GetProperty(RO_EmployeeNumberProperty); }
        }
        private string GetRO_EmployeeNumber()
        {
            return this.User.GetEmployeeNumber();
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public static readonly Property<string> RO_UserNameProperty = P<UserRole>.RegisterReadOnly(
            e => e.RO_UserName, e => e.GetRO_UserName());
        public string RO_UserName
        {
            get { return this.GetProperty(RO_UserNameProperty); }
        }
        private string GetRO_UserName()
        {
            return this.User.UserName;
        }

        /// <summary>
        /// 角色名称
        /// </summary>
        public static readonly Property<string> RO_RoleNameProperty = P<UserRole>.RegisterReadOnly(
            e => e.RO_RoleName, e => e.GetRO_RoleName());
        public string RO_RoleName
        {
            get { return this.GetProperty(RO_RoleNameProperty); }
        }
        private string GetRO_RoleName()
        {
            return this.Role.Name;
        }

        #endregion
    }

    /// <summary>
    /// 用户角色 列表类。
    /// </summary>
    [Serializable]
    public partial class UserRoleList : RBACEntityList { }

    /// <summary>
    /// 用户角色 仓库类。
    /// 负责 用户角色 类的查询、保存。
    /// </summary>
    public partial class UserRoleRepository : RBACEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected UserRoleRepository() { }

        /// <summary>
        /// 根据UserID获取RoleId
        /// </summary>
        /// <param name="userIds">用户Id集合</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual UserRoleList GetByUserId(List<long> userIds)
        {
            var query = this.CreateLinqQuery();
            query = query.Where(e => userIds.Contains(e.UserId));
            return (UserRoleList)this.QueryData(query);
        }

        /// <summary>
        /// 根据 RoleId 获取 UserId
        /// </summary>
        /// <param name="roleId">用户Id集合</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual UserRoleList GetByRoleId(long roleId)
        {
            var query = this.CreateLinqQuery();
            query = query.Where(e => e.RoleId == roleId);
            return (UserRoleList)this.QueryData(query);
        }

        /// <summary>
        /// 通过查询对象查询用户角色
        /// </summary>
        /// <param name="criteria">用户角色查询对象</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual UserRoleList GetByCriteria(UserRoleCriteria criteria)
        {
            var q = this.CreateLinqQuery();

            if (criteria.UserId.HasValue)
            {
                q = q.Where(e => e.UserId == criteria.UserId.Value);
            }

            if (criteria.RoleId.HasValue)
            {
                q = q.Where(e => e.RoleId == criteria.RoleId.Value);
            }

            return (UserRoleList)this.QueryData(q);
        }
    }


    /// <summary>
    /// 用户角色 配置类。
    /// 负责 用户角色 类的实体元数据的配置。
    /// </summary>
    internal class UserRoleConfig : RBACEntityConfig<UserRole>
    {
        /// <summary>
        /// 配置实体的元数据
        /// </summary>
        protected override void ConfigMeta()
        {
            //配置实体的所有属性都映射到数据表中。
            Meta.MapTable().MapAllProperties();
        }
    }

    /// <summary>
    /// 用户角色查询对象
    /// </summary>
    public class UserRoleCriteria : Criteria
    {
        public long? RoleId { get; set; }

        public string EmployeeNumber { get; set; }

        public long? UserId { get; set; }

        public long[] UserRoleId { get; set; }
    }
}