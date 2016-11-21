/*******************************************************
 * 
 * 作者：崔旭
 * 创建日期：20160828
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.5
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 崔旭 20160828 16:15
 * 
*******************************************************/

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Rafy.Data;
using Rafy.Domain;
using Rafy.Domain.ORM;
using Rafy.MetaModel;
using Rafy.MetaModel.Attributes;


namespace Rafy.RBAC
{
    /// <summary>
    /// 用户权限关系
    /// </summary>
    [RootEntity, Serializable]
    public partial class UserRelation : RBACEntity
    {
        #region 构造函数

        public UserRelation() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected UserRelation(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        #endregion

        #region 组合子属性

        #endregion

        #region 一般属性

        public static readonly Property<string> EmployeeNumberProperty = P<UserRelation>.Register(e => e.EmployeeNumber);
        /// <summary>
        /// 用户编码
        /// </summary>
        public string EmployeeNumber
        {
            get { return this.GetProperty(EmployeeNumberProperty); }
            set { this.SetProperty(EmployeeNumberProperty, value); }
        }

        public static readonly Property<string> UserNameProperty = P<UserRelation>.Register(e => e.UserName);
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName
        {
            get { return this.GetProperty(UserNameProperty); }
            set { this.SetProperty(UserNameProperty, value); }
        }

        public static readonly Property<DateTime?> WF_ApprovedTimeProperty = P<UserRelation>.Register(e => e.WF_ApprovedTime);
        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? WF_ApprovedTime
        {
            get { return this.GetProperty(WF_ApprovedTimeProperty); }
            set { this.SetProperty(WF_ApprovedTimeProperty, value); }
        }


        public static readonly Property<string> RoleNameProperty = P<UserRelation>.Register(e => e.RoleName);
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName
        {
            get { return this.GetProperty(RoleNameProperty); }
            set { this.SetProperty(RoleNameProperty, value); }
        }

        public static readonly Property<string> OrganizationNameProperty = P<UserRelation>.Register(e => e.OrganizationName);
        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrganizationName
        {
            get { return this.GetProperty(OrganizationNameProperty); }
            set { this.SetProperty(OrganizationNameProperty, value); }
        }

        public static readonly Property<string> OrganizationCodeProperty = P<UserRelation>.Register(e => e.OrganizationCode);
        /// <summary>
        /// 组织CODE
        /// </summary>
        public string OrganizationCode
        {
            get { return this.GetProperty(OrganizationCodeProperty); }
            set { this.SetProperty(OrganizationCodeProperty, value); }
        }

        public static readonly Property<string> SaleInfoNameProperty = P<UserRelation>.Register(e => e.SaleInfoName);
        /// <summary>
        /// 销方名称
        /// </summary>
        public string SaleInfoName
        {
            get { return this.GetProperty(SaleInfoNameProperty); }
            set { this.SetProperty(SaleInfoNameProperty, value); }
        }

        #endregion

        #region 只读属性

        #endregion
    }

    /// <summary>
    /// 用户权限关系 列表类。
    /// </summary>
    [Serializable]
    public partial class UserRelationList : RBACEntityList { }

    /// <summary>
    /// 用户权限关系 仓库类。
    /// 负责 用户权限关系 类的查询、保存。
    /// </summary>
    public partial class UserRelationRepository : RBACEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected UserRelationRepository() { }

        /// <summary>
        /// 按用户ID 获取组织
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pagingInfo"></param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual UserRelationList GetRelationByUserId(long userId, PagingInfo pagingInfo = null)
        {
            /*********************** 修改描述 *****************
             * 2016-10-09 
             * 赵朋
             * 注释掉169至178行，增加180至202行
             * 被修改的临时用户数据，也会有查看操作，下面169至178行的SQL只能查询出已生效的用户，
             * 所以在选中临时用户数据点击查看按钮时,弹出窗体的组织列表加载不到数据。
             ********************************************************/
            //FormattedSql sql =
            //     @" SELECT A.EmployeeNumber, A.UserName,A.RealName,A.WF_ApprovedTime,
            //        E.Name AS OrganizationName,E.Code AS OrganizationCode,
            //        NULL AS DBI_CreatedTime, NULL AS DBI_UpdatedTime,NULL AS DBI_CreatedUser,NULL AS DBI_UpdatedUser, 
            //        0 AS DBI_IsPhantom,ROW_NUMBER() over(order by UserName) AS Id,
            //        NULL AS RoleName , NULL AS SaleInfoName
            //        FROM T_Users A
            //        LEFT JOIN T_RBAC_OrganizationUser D ON A.Id=D.UserId AND (D.DBI_IsPhantom=0 OR D.DBI_IsPhantom IS NULL)
            //        LEFT JOIN T_RBAC_Organization E ON D.OrganizationId=E.Id AND E.WF_ApprovalStatus=300 AND (E.DBI_IsPhantom=0 OR E.DBI_IsPhantom IS NULL)
            //        WHERE A.DBI_IsPhantom=0 AND A.WF_ApprovalStatus=300 ";

            FormattedSql sql = @"SELECT T2.EmployeeNumber,
                                       T2.UserName,
                                       T2.RealName,
                                       T2.WF_ApprovedTime,
                                       T.Name AS OrganizationName,
                                       T.Code AS OrganizationCode,
                                       NULL AS DBI_CreatedTime,
                                       NULL AS DBI_UpdatedTime,
                                       NULL AS DBI_CreatedUser,
                                       NULL AS DBI_UpdatedUser,
                                       0 AS DBI_IsPhantom,
                                       ROW_NUMBER() over(order by UserName) AS Id,
                                       NULL AS RoleName,
                                       NULL AS SaleInfoName
                                  FROM T_RBAC_ORGANIZATION T
                                 INNER JOIN T_RBAC_OrganizationUser T1 ON T.ID = T1.ORGANIZATIONID
                                                                      AND (T1.DBI_IsPhantom = 0 OR
                                                                          T1.DBI_IsPhantom IS NULL)
                                 INNER JOIN T_USERS T2 ON T2.ID = T1.USERID
                                                      AND (T2.DBI_IsPhantom = 0 OR T2.DBI_IsPhantom IS NULL)
                                 WHERE T.DBI_IsPhantom = 0
                                   AND T.WF_ApprovalStatus = 300
                                   AND (T.DBI_IsPhantom = 0 OR T.DBI_IsPhantom IS NULL) ";

            if (userId != 0)
            {
                sql.Append("  AND T2.ID =" + userId + "");
            }

            sql.Append(" ORDER BY T2.UserName, T.Name ");
            return (UserRelationList)(this.DataQueryer as RdbDataQueryer).QueryData(sql, pagingInfo);
}

        /// <summary>
        /// 按UserId获取
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="pagingInfo"></param>
        /// <returns></returns>
        public virtual UserRelationList GetRelationByUserOrder(long UserId, PagingInfo pagingInfo = null)
        {
            return GetRelationBy(UserId, "", "", "", "", pagingInfo);
        }

        /// <summary>
        /// 按其他获取
        /// </summary>
        /// <param name="orgName">组织名称</param>
        /// <param name="saleInfoName">销方名称</param>
        /// <param name="userName">用户名称</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="pagingInfo">分页</param>
        /// <returns></returns>
        public virtual UserRelationList GetRelationByUserOrder(string orgName, string saleInfoName, string userName, string roleName, PagingInfo pagingInfo = null)
        {
            return GetRelationBy(0, orgName, saleInfoName, userName, roleName, pagingInfo);
        }

        /// <summary>
        /// 按用户汇总数据
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="orgName">组织名称</param>
        /// <param name="saleInfoName">销方名称</param>
        /// <param name="userName">用户名称</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="pagingInfo">分页</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual UserRelationList GetRelationBy(long userId, string orgName, string saleInfoName, string userName, string roleName, PagingInfo pagingInfo = null)
        {
            FormattedSql sql =
                 @" SELECT A.EmployeeNumber, A.UserName,A.RealName,A.WF_ApprovedTime,
        C.Name AS RoleName,
        E.Name AS OrganizationName,E.Code AS OrganizationCode,
        F.Name AS SaleInfoName,
        NULL AS DBI_CreatedTime, NULL AS DBI_UpdatedTime,NULL AS DBI_CreatedUser,NULL AS DBI_UpdatedUser, 
        0 AS DBI_IsPhantom,ROW_NUMBER() over(order by UserName) AS Id
        FROM T_Users A
        LEFT JOIN T_RBAC_UserRole B ON A.Id=B.UserId AND (B.DBI_IsPhantom=0 OR B.DBI_IsPhantom IS NULL)
        LEFT JOIN T_RBAC_Role C ON B.RoleId=C.Id AND (C.DBI_IsPhantom=0 OR C.DBI_IsPhantom IS NULL)  AND C.WF_APPROVALSTATUS = 300 
        LEFT JOIN T_RBAC_OrganizationUser D ON A.Id=D.UserId AND (D.DBI_IsPhantom=0 OR D.DBI_IsPhantom IS NULL)
        LEFT JOIN T_RBAC_Organization E ON D.OrganizationId=E.Id AND (E.DBI_IsPhantom=0 OR E.DBI_IsPhantom IS NULL) AND E.WF_APPROVALSTATUS = 300 
        LEFT JOIN T_SaleInfo F ON F.OrganizationId=E.Id AND (F.DBI_IsPhantom=0 OR F.DBI_IsPhantom IS NULL) and F.WF_APPROVALSTATUS = 300 
        WHERE A.DBI_IsPhantom=0 AND A.WF_ApprovalStatus=300 ";

            if (userId != 0)
            {
                sql.Append(" AND A.Id =" + userId + "");
            }

            if (!String.IsNullOrEmpty(orgName))
            {
                sql.Append(" AND E.Name LIKE '%" + orgName + "%' ");
            }

            if (!String.IsNullOrEmpty(saleInfoName))
            {
                sql.Append(" AND F.Name LIKE '%" + saleInfoName + "%' ");
            }

            if (!String.IsNullOrEmpty(userName))
            {
                sql.Append(" AND UserName LIKE '%" + userName + "%' ");
            }

            if (!String.IsNullOrEmpty(roleName))
            {
                sql.Append(" AND C.Name LIKE '%" + roleName + "%' ");
            }
            sql.Append(" ORDER BY A.UserName,C.Name,E.Name,F.Name ");
            return (UserRelationList)(this.DataQueryer as RdbDataQueryer).QueryData(sql, pagingInfo);
        }
    }

    /// <summary>
    /// 用户权限关系 配置类。
    /// 负责 用户权限关系 类的实体元数据的配置。
    /// </summary>
    internal class UserRelationConfig : RBACEntityConfig<UserRelation>
    {
        /// <summary>
        /// 配置实体的元数据
        /// </summary>
        protected override void ConfigMeta()
        {
            //配置实体的所有属性都映射到数据表中。
            Meta.MapTable().MapAllProperties();
            //组织名称
            Meta.Property(UserRelation.OrganizationNameProperty).MapColumn().DataTypeLength = "100";
            //销方名称
            Meta.Property(UserRelation.SaleInfoNameProperty).MapColumn().DataTypeLength = "100";
            //用户名
            Meta.Property(UserRelation.UserNameProperty).MapColumn().DataTypeLength = "100";
            //角色名称
            Meta.Property(UserRelation.RoleNameProperty).MapColumn().DataTypeLength = "100";
        }
    }
}
