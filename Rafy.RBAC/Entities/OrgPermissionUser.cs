/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160319
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160319 14:55
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Rafy;
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
using Rafy.Accounts;

namespace Rafy.RBAC
{
    /// <summary>
    /// 岗位用户
    /// </summary>
    [ChildEntity, Serializable]
    public partial class OrgPermissionUser : RBACEntity
    {
        #region 构造函数

        public OrgPermissionUser() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected OrgPermissionUser(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        public static readonly IRefIdProperty OrgPermissionIdProperty =
            P<OrgPermissionUser>.RegisterRefId(e => e.OrgPermissionId, ReferenceType.Parent);
        public long OrgPermissionId
        {
            get { return (long)this.GetRefId(OrgPermissionIdProperty); }
            set { this.SetRefId(OrgPermissionIdProperty, value); }
        }
        public static readonly RefEntityProperty<OrgPermission> OrgPermissionProperty =
            P<OrgPermissionUser>.RegisterRef(e => e.OrgPermission, OrgPermissionIdProperty);
        public OrgPermission OrgPermission
        {
            get { return this.GetRefEntity(OrgPermissionProperty); }
            set { this.SetRefEntity(OrgPermissionProperty, value); }
        }

        public static readonly IRefIdProperty UserIdProperty =
            P<OrgPermissionUser>.RegisterRefId(e => e.UserId, ReferenceType.Normal);
        public long UserId
        {
            get { return (long)this.GetRefId(UserIdProperty); }
            set { this.SetRefId(UserIdProperty, value); }
        }
        public static readonly RefEntityProperty<User> UserProperty =
            P<OrgPermissionUser>.RegisterRef(e => e.User, UserIdProperty);
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
    /// 岗位用户 列表类。
    /// </summary>
    [Serializable]
    public partial class OrgPermissionUserList : RBACEntityList { }

    /// <summary>
    /// 岗位用户 仓库类。
    /// 负责 岗位用户 类的查询、保存。
    /// </summary>
    public partial class OrgPermissionUserRepository : RBACEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected OrgPermissionUserRepository() { }

        /// <summary>
        /// 通过岗位ID获取岗位用户信息。
        /// </summary>
        /// <param name="permissionId">岗位ID</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual OrgPermissionUserList GetByPermissionId(long permissionId)
        {
            var q = this.CreateLinqQuery();
            q = q.Where(e => e.OrgPermissionId == permissionId);
            return (OrgPermissionUserList)this.QueryData(q);
        }
    }

    /// <summary>
    /// 岗位用户 配置类。
    /// 负责 岗位用户 类的实体元数据的配置。
    /// </summary>
    internal class OrgPermissionUserConfig : RBACEntityConfig<OrgPermissionUser>
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