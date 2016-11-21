/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160319
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160319 14:53
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

namespace Rafy.RBAC
{
    /// <summary>
    /// 岗位角色
    /// </summary>
    [ChildEntity, Serializable]
    public partial class OrgPermissionRole : RBACEntity
    {
        #region 构造函数

        public OrgPermissionRole() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected OrgPermissionRole(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        public static readonly IRefIdProperty OrgPermissionIdProperty =
            P<OrgPermissionRole>.RegisterRefId(e => e.OrgPermissionId, ReferenceType.Parent);
        public long OrgPermissionId
        {
            get { return (long)this.GetRefId(OrgPermissionIdProperty); }
            set { this.SetRefId(OrgPermissionIdProperty, value); }
        }
        public static readonly RefEntityProperty<OrgPermission> OrgPermissionProperty =
            P<OrgPermissionRole>.RegisterRef(e => e.OrgPermission, OrgPermissionIdProperty);
        public OrgPermission OrgPermission
        {
            get { return this.GetRefEntity(OrgPermissionProperty); }
            set { this.SetRefEntity(OrgPermissionProperty, value); }
        }


        public static readonly IRefIdProperty RoleIdProperty =
            P<OrgPermissionRole>.RegisterRefId(e => e.RoleId, ReferenceType.Normal);
        public long RoleId
        {
            get { return (long)this.GetRefId(RoleIdProperty); }
            set { this.SetRefId(RoleIdProperty, value); }
        }
        public static readonly RefEntityProperty<Role> RoleProperty =
            P<OrgPermissionRole>.RegisterRef(e => e.Role, RoleIdProperty);
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

        #endregion
    }

    /// <summary>
    /// 岗位角色 列表类。
    /// </summary>
    [Serializable]
    public partial class OrgPermissionRoleList : RBACEntityList { }

    /// <summary>
    /// 岗位角色 仓库类。
    /// 负责 岗位角色 类的查询、保存。
    /// </summary>
    public partial class OrgPermissionRoleRepository : RBACEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected OrgPermissionRoleRepository() { }

        /// <summary>
        /// 通过岗位ID获取岗位角色信息
        /// </summary>
        /// <param name="permissionId">岗位ID</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual OrgPermissionRoleList GetByPermissionId(long permissionId)
        {
            var q = this.CreateLinqQuery();
            q = q.Where(e => e.OrgPermissionId == permissionId);
            return (OrgPermissionRoleList)this.QueryData(q);
        }
    }

    /// <summary>
    /// 岗位角色 配置类。
    /// 负责 岗位角色 类的实体元数据的配置。
    /// </summary>
    internal class OrgPermissionRoleConfig : RBACEntityConfig<OrgPermissionRole>
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