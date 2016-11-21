/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160417
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160417 21:57
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
    /// <summary>
    /// 组织用户
    /// </summary>
    [ChildEntity, Serializable]
    public partial class OrganizationUser : RBACEntity
    {
        #region 构造函数

        public OrganizationUser() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected OrganizationUser(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        public static readonly IRefIdProperty OrganizationIdProperty =
            P<OrganizationUser>.RegisterRefId(e => e.OrganizationId, ReferenceType.Parent);
        public long OrganizationId
        {
            get { return (long)this.GetRefId(OrganizationIdProperty); }
            set { this.SetRefId(OrganizationIdProperty, value); }
        }
        public static readonly RefEntityProperty<Organization> OrganizationProperty =
            P<OrganizationUser>.RegisterRef(e => e.Organization, OrganizationIdProperty);
        public Organization Organization
        {
            get { return this.GetRefEntity(OrganizationProperty); }
            set { this.SetRefEntity(OrganizationProperty, value); }
        }

        public static readonly IRefIdProperty UserIdProperty =
            P<OrganizationUser>.RegisterRefId(e => e.UserId, ReferenceType.Normal);
        public long UserId
        {
            get { return (long)this.GetRefId(UserIdProperty); }
            set { this.SetRefId(UserIdProperty, value); }
        }
        public static readonly RefEntityProperty<User> UserProperty =
            P<OrganizationUser>.RegisterRef(e => e.User, UserIdProperty);
        /// <summary>
        /// 用户
        /// </summary>
        public User User
        {
            get { return this.GetRefEntity(UserProperty); }
            set { this.SetRefEntity(UserProperty, value); }
        }

        #endregion

        #region 组合子属性

        #endregion

        #region 一般属性

        #endregion

        #region 只读属性

        #endregion
    }

    /// <summary>
    /// 组织用户 列表类。
    /// </summary>
    [Serializable]
    public partial class OrganizationUserList : RBACEntityList { }

    /// <summary>
    /// 组织用户 仓库类。
    /// 负责 组织用户 类的查询、保存。
    /// </summary>
    public partial class OrganizationUserRepository : RBACEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected OrganizationUserRepository() { }

        /// <summary>
        /// 此方法通过组织ID获取组织用户数据。
        /// </summary>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual OrganizationUserList GetByOrganizationId(long id)
        {
            var q = this.CreateLinqQuery();
            q = q.Where(e => e.OrganizationId == id);
            return (OrganizationUserList)this.QueryData(q);
        }

        /// <summary>
        /// 此方法通过多个组织的ID获取组织用户的数据。
        /// </summary>
        /// <param name="ids">存储了组织ID的数组。</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual OrganizationUserList GetByOrganizationId(long[] ids)
        {
            var q = this.CreateLinqQuery();
            q = q.Where(e => ids.Contains(e.OrganizationId));
            return (OrganizationUserList)this.QueryData(q);
        }

        /// <summary>
        /// 此方法通过用户ID获取组织用户的数据。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual OrganizationUserList GetByUserId(long userId)
        {
            var q = this.CreateLinqQuery();
            q = q.Where(e => e.UserId == userId);
            return (OrganizationUserList)this.QueryData(q);
        }
    }

    /// <summary>
    /// 组织用户 配置类。
    /// 负责 组织用户 类的实体元数据的配置。
    /// </summary>
    internal class OrganizationUserConfig : RBACEntityConfig<OrganizationUser>
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
}