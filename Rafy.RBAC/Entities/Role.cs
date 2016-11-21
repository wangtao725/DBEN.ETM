/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160417
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160417 13:50
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
    /// 角色
    /// </summary>
    [RootEntity, Serializable]
    public partial class Role : RBACEntity
    {
        #region 构造函数

        public Role() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected Role(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        #endregion

        #region 组合子属性

        public static readonly ListProperty<RoleOperationList> RoleOperationListProperty = P<Role>.RegisterList(e => e.RoleOperationList);
        public RoleOperationList RoleOperationList
        {
            get { return this.GetLazyList(RoleOperationListProperty); }
        }

        #endregion

        #region 一般属性

        public static readonly Property<string> NameProperty = P<Role>.Register(e => e.Name);
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name
        {
            get { return this.GetProperty(NameProperty); }
            set { this.SetProperty(NameProperty, value); }
        }

        public static readonly Property<string> CodeProperty = P<Role>.Register(e => e.Code);
        /// <summary>
        /// 角色编码
        /// </summary>
        public string Code
        {
            get { return this.GetProperty(CodeProperty); }
            set { this.SetProperty(CodeProperty, value); }
        }

        public static readonly Property<string> DescriptionProperty = P<Role>.Register(e => e.Description);
        /// <summary>
        /// 角色描述
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
    /// 角色 列表类。
    /// </summary>
    [Serializable]
    public partial class RoleList : RBACEntityList { }

    /// <summary>
    /// 角色 仓库类。
    /// 负责 角色 类的查询、保存。
    /// </summary>
    public partial class RoleRepository : RBACEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected RoleRepository() { }

        /// <summary>
        /// 通过角色查询对象查询角色信息
        /// </summary>
        /// <param name="criteria">角色查询对象</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual RoleList GetByCriteria(RoleCriteria criteria)
        {
            var q = this.CreateLinqQuery();
            if (!string.IsNullOrEmpty(criteria.Name) && !string.IsNullOrEmpty(criteria.Code))
            {
                q = q.Where(e => e.Name == criteria.Name || e.Code == criteria.Code);
                return (RoleList)this.QueryData(q);
            }
            if (!string.IsNullOrEmpty(criteria.Name))
            {
                q = q.Where(e => e.Name == criteria.Name);
            }

            if (!string.IsNullOrEmpty(criteria.Code))
            {
                q = q.Where(e => e.Code == criteria.Code);
            }
            return (RoleList)this.QueryData(q);
        }

        [RepositoryQuery]
        public virtual RoleList GetByCode(Role role)
        {
            var f = QueryFactory.Instance;
            var t = f.Table<Role>();

            var q = f.Query(
                    selection: f.SelectAll(),
                    from: t,
                    where: f.And(t.Column(Role.IdProperty).NotEqual(role.Id),
                    f.Or(t.Column(Role.CodeProperty).Equal(role.Code)))
                );

            var roleList = (RoleList)this.QueryData(q);
            return roleList;
        }

        /// <summary>
        /// 通过原角色ID查询角色数据
        /// </summary>
        /// <param name="ids">原角色ID</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual RoleList GetByRealDataId(string[] ids)
        {
            if (ids.Length > 0)
            {
                ODataQueryCriteria criteria = new ODataQueryCriteria();

                for (int i = 0; i < ids.Length; i++)
                {
                    string id = ids[i];
                    criteria.Filter = " WF_REALDATAID eq " + id;
                    if (i < ids.Length - 1)
                    {
                        criteria.Filter = " or ";
                    }
                }
                return (RoleList)this.GetBy(criteria);
            }
            else
            {
                return new RoleList();
            }
        }
    }

    /// <summary>
    /// 角色 配置类。
    /// 负责 角色 类的实体元数据的配置。
    /// </summary>
    internal class RoleConfig : RBACEntityConfig<Role>
    {
        /// <summary>
        /// 配置实体的元数据
        /// </summary>
        protected override void ConfigMeta()
        {
            //配置实体的所有属性都映射到数据表中。
            Meta.MapTable().MapAllProperties();
            //角色名称
            Meta.Property(Role.NameProperty).MapColumn().DataTypeLength = "50";
        }
        protected override void AddValidations(IValidationDeclarer rules)
        {
            base.AddValidations(rules);

            rules.AddRule(new Rafy.Domain.Validation.HandlerRule()
            {
                Handler = (entity, e) =>
                {
                    try
                    {
                        if (entity.GetType().Name == typeof(Role).Name)
                        {
                            var repo = RF.Concrete<RoleRepository>();
                            Role bII = entity as Role;
                            int count = 0;

                            if (bII.Id > 0)
                            {
                                count = repo.GetByCode(bII).Count;
                            }
                            else
                            {
                                count = repo.GetByCriteria(new RoleCriteria() { Code=bII.Code}).Count;
                            }
                            e.BrokenDescription = count == 0 ? string.Empty : "角色编号已经存在!";

                        }
                    }
                    catch (Exception ex)
                    {
                        e.BrokenDescription = ex.Message;
                    }
                }
            }
            );
        }
    }

    /// <summary>
    /// 角色查询对象
    /// </summary>
    public partial class RoleCriteria : Criteria
    {
        public string Name { get; set; }

        public string Code { get; set; }
    }
}