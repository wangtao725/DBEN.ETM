/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160417
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160417 13:46
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
    /// 组织
    /// </summary>
    [RootEntity, Serializable]
    public partial class Organization : RBACEntity
    {
        #region 构造函数

        public Organization() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected Organization(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        #endregion

        #region 组合子属性

        public static readonly ListProperty<OrganizationUserList> OrganizationUserListProperty = P<Organization>.RegisterList(e => e.OrganizationUserList);
        public OrganizationUserList OrganizationUserList
        {
            get { return this.GetLazyList(OrganizationUserListProperty); }
        }

        public static readonly ListProperty<OrgPositionList> OrgPositionListProperty = P<Organization>.RegisterList(e => e.OrgPositionList);
        public OrgPositionList OrgPositionList
        {
            get { return this.GetLazyList(OrgPositionListProperty); }
        }

        #endregion

        #region 一般属性

        public static readonly Property<string> NameProperty = P<Organization>.Register(e => e.Name);
        /// <summary>
        /// 组织名称
        /// </summary>
        public string Name
        {
            get { return this.GetProperty(NameProperty); }
            set { this.SetProperty(NameProperty, value); }
        }

        public static readonly Property<string> CodeProperty = P<Organization>.Register(e => e.Code);
        /// <summary>
        /// 组织编码
        /// </summary>
        public string Code
        {
            get { return this.GetProperty(CodeProperty); }
            set { this.SetProperty(CodeProperty, value); }
        }

        public static readonly Property<string> DescriptionProperty = P<Organization>.Register(e => e.Description);
        /// <summary>
        /// 组织描述
        /// </summary>
        public string Description
        {
            get { return this.GetProperty(DescriptionProperty); }
            set { this.SetProperty(DescriptionProperty, value); }
        }

        public static readonly Property<string> FullNameProperty = P<Organization>.Register(e => e.FullName);
        /// <summary>
        /// 组织全名
        /// </summary>
        public string FullName
        {
            get { return this.GetProperty(FullNameProperty); }
            set { this.SetProperty(FullNameProperty, value); }
        }

        #endregion

        #region 只读属性

        #endregion
    }

    /// <summary>
    /// 组织 列表类。
    /// </summary>
    [Serializable]
    public partial class OrganizationList : RBACEntityList { }

    /// <summary>
    /// 组织 仓库类。
    /// 负责 组织 类的查询、保存。
    /// </summary>
    public partial class OrganizationRepository : RBACEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected OrganizationRepository() { }

        public virtual OrganizationList GetAllOrganization(bool autoTreeIndexEnabled = false)
        {
            const string sql = @"SELECT org.Id, org.Name,org.treeindex
                      FROM T_RBAC_Organization org
                     where org.wf_approvalstatus = 300
                       and (org.DBI_IsPhantom = 0 or org.DBI_IsPhantom is null)";
            var dt = (this.DataQueryer as RdbDataQueryer).QueryTable(sql);

            var result = new OrganizationList {
                AutoTreeIndexEnabled = autoTreeIndexEnabled
            };
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (var row in dt.Rows)
                {
                    result.Add(new Organization {
                        Id = Convert.ToInt64(row["Id"]),
                        Name = Convert.ToString(row["Name"]),
                        TreeIndex = Convert.ToString(row["treeindex"])
                    });
                }
            }

            return result;
        }


        /// <summary>
        /// 通过组织查询对象查询
        /// </summary>
        /// <param name="criteria">组织查询对象</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual OrganizationList GetByOrganizationCriteria(OrganizationCriteria criteria)
        {
            var q = this.CreateLinqQuery();

            if (!string.IsNullOrEmpty(criteria.Name))
            {
                q = q.Where(e => e.Name == criteria.Name);
            }

            if (!string.IsNullOrEmpty(criteria.Code))
            {
                q = q.Where(e => e.Code == criteria.Code);
            }

            return (OrganizationList)this.QueryData(q);
        }

        [RepositoryQuery]
        public virtual Organization GetFullNameById(long id)
        {
            var q = this.CreateLinqQuery();
            q = q.Where(e => e.Id == id);
            //
            Organization org = (Organization)this.QueryData(q);
            string fullName = org.Name;
            while (org.TreeParent != null)
            {
                Organization pOrg = (Organization)org.TreeParent;
                fullName = pOrg.Name + " - " + fullName;
                org = pOrg;
            }

            org= (Organization)this.QueryData(q);
            org.FullName = fullName;
            return org;
        }

        public virtual List<OrganizationNotTree> GetByUserName(string userName)
        {
            FormattedSql sql =
                @"SELECT org.Id,org.Name
            FROM T_RBAC_Organization org
            LEFT JOIN T_RBAC_OrganizationUser ou on (org.id = ou.organizationid and ou.dbi_isphantom = 0)
            LEFT JOIN T_Users u on u.id = ou.userid  and u.dbi_isphantom = 0 and u.wf_approvalstatus = 300
            where org.wf_approvalstatus = 300 and(org.DBI_IsPhantom = 0 or org.DBI_IsPhantom is null) ";

            if(!string.IsNullOrEmpty(userName))
            {
                sql.Append(string.Format("and u.username = '{0}' ", userName));
            }
            sql.Append(" order by org.treeindex ");
            LiteDataTable dt = (this.DataQueryer as RdbDataQueryer).QueryTable(sql);
            List<OrganizationNotTree> result = new List<OrganizationNotTree>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (var row in dt.Rows)
                {
                    result.Add(new OrganizationNotTree { Id = Convert.ToInt64(row["Id"]), Name = Convert.ToString(row["Name"]) });
                }
            }

            return result;
            //return (OrganizationList)(this.DataQueryer as RdbDataQueryer).QueryData(sql);
        }

        public virtual List<OrganizationNotTree> GetByOrgIdAndUserNameContainChild(long orgId, string userName = "")
        {
            FormattedSql sql = new FormattedSql();
            var dbSettingName = RBACPlugin.DbSettingName;
            var dbSetting = DbSetting.FindOrCreate(dbSettingName);
            if (DbSetting.IsOracleProvider(dbSetting))
            {
                    sql =
                        @"select oorg.Id,oorg.Name from T_RBAC_ORGANIZATION oorg
                LEFT JOIN T_RBAC_OrganizationUser ou on(oorg.id = ou.organizationid and ou.dbi_isphantom = 0)
                LEFT JOIN T_Users u on(u.id = ou.userid and u.dbi_isphantom = 0)
                where(oorg.dbi_isphantom = 0 or oorg.dbi_isphantom is null) and oorg.wf_approvalstatus = 300
                and oorg.treeindex like (select treeindex || '%' from T_RBAC_ORGANIZATION org where org.id = {0}) ";

            }
            else
            {
                    sql =
                    @"select oorg.Id,oorg.Name from T_RBAC_ORGANIZATION oorg
                LEFT JOIN T_RBAC_OrganizationUser ou on(oorg.id = ou.organizationid and ou.dbi_isphantom = 0)
                LEFT JOIN T_Users u on(u.id = ou.userid and u.dbi_isphantom = 0)
                where(oorg.dbi_isphantom = 0 or oorg.dbi_isphantom is null) and oorg.wf_approvalstatus = 300
                and oorg.treeindex like (select treeindex + '%' from T_RBAC_ORGANIZATION org where org.id = {0}) ";
            }

            if (!string.IsNullOrEmpty(userName))
            {
                sql.Append(string.Format("and u.username = '{0}' ", userName));
            }
            sql.Parameters.Add(orgId);

            LiteDataTable dt = (this.DataQueryer as RdbDataQueryer).QueryTable(sql);
            List<OrganizationNotTree> result = new List<OrganizationNotTree>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (var row in dt.Rows)
                {
                    result.Add(new OrganizationNotTree { Id = Convert.ToInt64(row["Id"]), Name = Convert.ToString(row["Name"]) });
                }
            }

            return result;
            //return (OrganizationList)(this.DataQueryer as RdbDataQueryer).QueryData(sql);
        }
    }

    /// <summary>
    /// 组织 配置类。
    /// 负责 组织 类的实体元数据的配置。
    /// </summary>
    internal class OrganizationConfig : RBACEntityConfig<Organization>
    {
        /// <summary>
        /// 配置实体的元数据
        /// </summary>
        protected override void ConfigMeta()
        {
            //配置当前实体实树型实体
            Meta.SupportTree();

            //配置实体的所有属性都映射到数据表中。
            Meta.MapTable().MapAllProperties();
            //组织名称
            Meta.Property(Organization.NameProperty).MapColumn().DataTypeLength = "100";
        }
    }

    /// <summary>
    /// 组织查询对象
    /// </summary>
    public class OrganizationCriteria : Criteria
    {
        public string Name { get; set; }

        public string Code { get; set; }
    }

    public class OrganizationNotTree
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}