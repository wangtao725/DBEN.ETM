/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160417
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160417 13:44
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
using Rafy.RBAC.Enum;

namespace Rafy.RBAC
{
    /// <summary>
    /// 功能
    /// </summary>
    [ChildEntity, Serializable]
    public partial class ResourceOperation : RBACEntity
    {
        #region 构造函数

        public ResourceOperation() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected ResourceOperation(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        public static readonly IRefIdProperty ResourceIdProperty =
            P<ResourceOperation>.RegisterRefId(e => e.ResourceId, ReferenceType.Parent);
        public long ResourceId
        {
            get { return (long)this.GetRefId(ResourceIdProperty); }
            set { this.SetRefId(ResourceIdProperty, value); }
        }
        public static readonly RefEntityProperty<Resource> ResourceProperty =
            P<ResourceOperation>.RegisterRef(e => e.Resource, ResourceIdProperty);
        public Resource Resource
        {
            get { return this.GetRefEntity(ResourceProperty); }
            set { this.SetRefEntity(ResourceProperty, value); }
        }

        #endregion

        #region 组合子属性

        #endregion

        #region 一般属性

        public static readonly Property<string> NameProperty = P<ResourceOperation>.Register(e => e.Name);
        /// <summary>
        /// 功能名称
        /// </summary>
        public string Name
        {
            get { return this.GetProperty(NameProperty); }
            set { this.SetProperty(NameProperty, value); }
        }

        public static readonly Property<string> CodeProperty = P<ResourceOperation>.Register(e => e.Code);
        /// <summary>
        /// 功能编码
        /// </summary>
        public string Code
        {
            get { return this.GetProperty(CodeProperty); }
            set { this.SetProperty(CodeProperty, value); }
        }


        public static readonly Property<string> DescriptionProperty = P<ResourceOperation>.Register(e => e.Description);
        /// <summary>
        /// 功能描述
        /// </summary>
        public string Description
        {
            get { return this.GetProperty(DescriptionProperty); }
            set { this.SetProperty(DescriptionProperty, value); }
        }

        public static readonly Property<OperationType> OperationTypeProperty = P<ResourceOperation>.Register(e => e.OperationType);
        /// <summary>
        /// 功能类型
        /// （资源默认功能，有具体用途的功能）
        /// </summary>
        public OperationType OperationType
        {
            get { return this.GetProperty(OperationTypeProperty); }
            set { this.SetProperty(OperationTypeProperty, value); }
        }

        public static readonly Property<string> OperationNameProperty = P<ResourceOperation>.Register(e => e.OperationName);
        /// <summary>
        /// 按钮名称
        /// 此属性，对应界面按钮对象的operationName属性。
        /// 用于对按钮进行权限控制。
        /// </summary>
        public string OperationName
        {
            get { return this.GetProperty(OperationNameProperty); }
            set { this.SetProperty(OperationNameProperty, value); }
        }

        #endregion

        #region 只读属性

        #endregion
    }

    /// <summary>
    /// 功能 列表类。
    /// </summary>
    [Serializable]
    public partial class ResourceOperationList : RBACEntityList { }

    /// <summary>
    /// 功能 仓库类。
    /// 负责 功能 类的查询、保存。
    /// </summary>
    public partial class ResourceOperationRepository : RBACEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected ResourceOperationRepository() { }

        /// <summary>
        /// 通过功能ID和资源ID获取功能
        /// </summary>
        /// <param name="operationId">功能ID数组</param>
        /// <param name="resourceId">资源ID</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual ResourceOperationList GetByOperationIdAndResourceId(long[] operationId, long resourceId)
        {
            return (ResourceOperationList)this.QueryInBatches(operationId, (ids) =>
            {
                var q = this.CreateLinqQuery();
                q = q.Where(e => ids.Contains(e.Id) && e.ResourceId == resourceId);
                return (ResourceOperationList)this.QueryData(q);
            });
        }
    }

    /// <summary>
    /// 功能 配置类。
    /// 负责 功能 类的实体元数据的配置。
    /// </summary>
    internal class ResourceOperationConfig : RBACEntityConfig<ResourceOperation>
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
    /// 资源功能查询
    /// </summary>
    public class ResourceOperationCriteria : Criteria
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 资源ID
        /// </summary>
        public long? ResourceId { get; set; }

        /// <summary>
        /// 资源ID
        /// </summary>
        public long? OperationId { get; set; }
    }
}