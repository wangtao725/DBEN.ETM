﻿/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160417
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160417 13:48
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
    /// 岗位模型
    /// </summary>
    [ChildEntity, Serializable]
    public partial class OrgPosition : RBACEntity
    {
        #region 构造函数

        public OrgPosition() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected OrgPosition(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        public static readonly IRefIdProperty OrganizationIdProperty =
            P<OrgPosition>.RegisterRefId(e => e.OrganizationId, ReferenceType.Parent);
        public long OrganizationId
        {
            get { return (long)this.GetRefId(OrganizationIdProperty); }
            set { this.SetRefId(OrganizationIdProperty, value); }
        }
        public static readonly RefEntityProperty<Organization> OrganizationProperty =
            P<OrgPosition>.RegisterRef(e => e.Organization, OrganizationIdProperty);
        public Organization Organization
        {
            get { return this.GetRefEntity(OrganizationProperty); }
            set { this.SetRefEntity(OrganizationProperty, value); }
        }

        #endregion

        #region 组合子属性

        public static readonly ListProperty<OrgPositionUserList> OrgPositionUserListProperty = P<OrgPosition>.RegisterList(e => e.OrgPositionUserList);
        public OrgPositionUserList OrgPositionUserList
        {
            get { return this.GetLazyList(OrgPositionUserListProperty); }
        }

        public static readonly ListProperty<OrgPositionRoleList> OrgPositionRoleListProperty = P<OrgPosition>.RegisterList(e => e.OrgPositionRoleList);
        public OrgPositionRoleList OrgPositionRoleList
        {
            get { return this.GetLazyList(OrgPositionRoleListProperty); }
        }

        #endregion

        #region 一般属性

        public static readonly Property<string> NameProperty = P<OrgPosition>.Register(e => e.Name);
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string Name
        {
            get { return this.GetProperty(NameProperty); }
            set { this.SetProperty(NameProperty, value); }
        }

        public static readonly Property<string> CodeProperty = P<OrgPosition>.Register(e => e.Code);
        /// <summary>
        /// 岗位编码
        /// </summary>
        public string Code
        {
            get { return this.GetProperty(CodeProperty); }
            set { this.SetProperty(CodeProperty, value); }
        }

        public static readonly Property<string> DescriptionProperty = P<OrgPosition>.Register(e => e.Description);
        /// <summary>
        /// 岗位描述
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
    /// 岗位模型 列表类。
    /// </summary>
    [Serializable]
    public partial class OrgPositionList : RBACEntityList { }

    /// <summary>
    /// 岗位模型 仓库类。
    /// 负责 岗位模型 类的查询、保存。
    /// </summary>
    public partial class OrgPositionRepository : RBACEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected OrgPositionRepository() { }
    }

    /// <summary>
    /// 岗位模型 配置类。
    /// 负责 岗位模型 类的实体元数据的配置。
    /// </summary>
    internal class OrgPositionConfig : RBACEntityConfig<OrgPosition>
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