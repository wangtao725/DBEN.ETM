/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160417
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160417 13:40
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
    /// 资源
    /// </summary>
    [RootEntity, Serializable]
    public partial class Resource : RBACEntity
    {
        #region 构造函数

        public Resource() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected Resource(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        #endregion

        #region 组合子属性

        public static readonly ListProperty<ResourceOperationList> ResourceOperationListProperty = P<Resource>.RegisterList(e => e.ResourceOperationList);
        public ResourceOperationList ResourceOperationList
        {
            get { return this.GetLazyList(ResourceOperationListProperty); }
        }

        #endregion

        #region 一般属性

        public static readonly Property<string> NameProperty = P<Resource>.Register(e => e.Name);
        /// <summary>
        /// 资源名称
        /// </summary>
        public string Name
        {
            get { return this.GetProperty(NameProperty); }
            set { this.SetProperty(NameProperty, value); }
        }

        public static readonly Property<string> CodeProperty = P<Resource>.Register(e => e.Code);
        /// <summary>
        /// 资源编码
        /// </summary>
        public string Code
        {
            get { return this.GetProperty(CodeProperty); }
            set { this.SetProperty(CodeProperty, value); }
        }

        public static readonly Property<string> UrlProperty = P<Resource>.Register(e => e.Url);
        /// <summary>
        /// 资源路径
        /// </summary>
        public string Url
        {
            get { return this.GetProperty(UrlProperty); }
            set { this.SetProperty(UrlProperty, value); }
        }

        public static readonly Property<string> ResultTypeProperty = P<Resource>.Register(e => e.ResultType);
        /// <summary>
        /// 资源类型
        /// （该属性以后可能会改为枚举类型）
        /// </summary>
        public string ResultType
        {
            get { return this.GetProperty(ResultTypeProperty); }
            set { this.SetProperty(ResultTypeProperty, value); }
        }

        public static readonly Property<string> DescriptionProperty = P<Resource>.Register(e => e.Description);
        /// <summary>
        /// 资源描述
        /// </summary>
        public string Description
        {
            get { return this.GetProperty(DescriptionProperty); }
            set { this.SetProperty(DescriptionProperty, value); }
        }

        #endregion

        #region 只读属性

        #endregion
    }

    /// <summary>
    /// 资源 列表类。
    /// </summary>
    [Serializable]
    public partial class ResourceList : RBACEntityList { }

    /// <summary>
    /// 资源 仓库类。
    /// 负责 资源 类的查询、保存。
    /// </summary>
    public partial class ResourceRepository : RBACEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected ResourceRepository() { }

        /// <summary>
        /// 通过URL获取资源信息
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual ResourceList GetByUrl(string url)
        {
            var q = this.CreateLinqQuery();
            q = q.Where(e => e.Url.Contains(url));
            return (ResourceList)this.QueryData(q);
        }

        /// <summary>
        /// 根据Name来查询资源信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual ResourceList GetByName(string name)
        {
            var q = this.CreateLinqQuery();
            q = q.Where(e => e.Name == name);
            return (ResourceList)this.QueryData(q);
        }
    }

    /// <summary>
    /// 资源 配置类。
    /// 负责 资源 类的实体元数据的配置。
    /// </summary>
    internal class ResourceConfig : RBACEntityConfig<Resource>
    {
        /// <summary>
        /// 配置实体的元数据
        /// </summary>
        protected override void ConfigMeta()
        {
            //配置实体的所有属性都映射到数据表中。
            Meta.MapTable().MapAllProperties();
            Meta.SupportTree();
        }
    }
}