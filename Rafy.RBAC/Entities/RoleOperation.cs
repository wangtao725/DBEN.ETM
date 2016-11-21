/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160417
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160417 13:51
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
    /// 角色的功能
    /// </summary>
    [ChildEntity, Serializable]
    public partial class RoleOperation : RBACEntity
    {
        #region 构造函数

        public RoleOperation() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected RoleOperation(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        public static readonly IRefIdProperty RoleIdProperty =
            P<RoleOperation>.RegisterRefId(e => e.RoleId, ReferenceType.Parent);
        public long RoleId
        {
            get { return (long)this.GetRefId(RoleIdProperty); }
            set { this.SetRefId(RoleIdProperty, value); }
        }
        public static readonly RefEntityProperty<Role> RoleProperty =
            P<RoleOperation>.RegisterRef(e => e.Role, RoleIdProperty);
        public Role Role
        {
            get { return this.GetRefEntity(RoleProperty); }
            set { this.SetRefEntity(RoleProperty, value); }
        }

        public static readonly IRefIdProperty OperationIdProperty =
            P<RoleOperation>.RegisterRefId(e => e.OperationId, ReferenceType.Normal);
        public long OperationId
        {
            get { return (long)this.GetRefId(OperationIdProperty); }
            set { this.SetRefId(OperationIdProperty, value); }
        }
        public static readonly RefEntityProperty<ResourceOperation> OperationProperty =
            P<RoleOperation>.RegisterRef(e => e.Operation, OperationIdProperty);
        /// <summary>
        /// 资源操作
        /// </summary>
        public ResourceOperation Operation
        {
            get { return this.GetRefEntity(OperationProperty); }
            set { this.SetRefEntity(OperationProperty, value); }
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
    /// 角色的功能 列表类。
    /// </summary>
    [Serializable]
    public partial class RoleOperationList : RBACEntityList { }

    /// <summary>
    /// 角色的功能 仓库类。
    /// 负责 角色的功能 类的查询、保存。
    /// </summary>
    public partial class RoleOperationRepository : RBACEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected RoleOperationRepository() { }

        /// <summary>
        /// 根据角色ID数组查询角色功能
        /// </summary>
        /// <param name="roleIds">角色ID数组</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual RoleOperationList GetByRoleIds(long[] roleIds)
        {
            var q = this.CreateLinqQuery();
            q = q.Where(e => roleIds.Contains(e.RoleId));
            return (RoleOperationList)this.QueryData(q);
        }

        /// <summary>
        /// 根据当前roleId获取角色操作
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public RoleOperationList GetOperationsByRoleId(string roleId)
        {
            return this.GetBy(new CommonQueryCriteria
            {
                new PropertyMatch(RoleOperation.RoleIdProperty, roleId),
                
            });
        }

        /// <summary>
        /// 此方法通过角色ID贪婪加载操作和资源。
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual RoleOperationList GetByRoleId(long id)
        {
            EagerLoadOptions eagerload = new EagerLoadOptions();
            eagerload.LoadWith(RoleOperation.OperationProperty).LoadWith(ResourceOperation.ResourceProperty);

            var q = this.CreateLinqQuery();
            q = q.Where(e => e.RoleId == id);
            return (RoleOperationList)this.QueryData(q, null, eagerload);
        }
    }

    /// <summary>
    /// 角色的功能 配置类。
    /// 负责 角色的功能 类的实体元数据的配置。
    /// </summary>
    internal class RoleOperationConfig : RBACEntityConfig<RoleOperation>
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