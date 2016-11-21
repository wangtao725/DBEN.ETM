/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160316
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160316 11:19
 * 
*******************************************************/

using Rafy.Accounts;
using Rafy.Data;
using Rafy.Domain;
using Rafy.Domain.ORM;
using Rafy.Domain.ORM.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafy.RBAC
{
    /// <summary>
    /// 用户仓库扩展
    /// </summary>
    public class UserRepositoryExt : EntityRepositoryExt<Accounts.UserRepository>
    {
        /// <summary>
        /// 通过岗位ID获取已授权的用户
        /// </summary>
        /// <param name="permissionId">岗位ID</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual UserList GetAuthUserByPermissionId(long permissionId)
        {
            FormattedSql sql =
@"SELECT *
FROM T_USERS T
    INNER JOIN T_RBAC_ORGPERMISSIONUSER T1 ON T.ID = T1.USERID
WHERE T.DBI_ISPHANTOM = 0 AND T1.DBI_ISPHANTOM = 0 AND T1.ORGPERMISSIONID = {0}";

            sql.Parameters.Add(permissionId);

            return (UserList)(this.DataQueryer as RdbDataQueryer).QueryData(sql);
        }

        /// <summary>
        /// 通过岗位ID获取未授权的用户
        /// </summary>
        /// <param name="permisssionId">岗位ID</param>
        /// <returns></returns>
        [RepositoryQuery]
        public virtual UserList GetUnAuthByPermissionId(long permisssionId)
        {
            FormattedSql sql =
@"SELECT *
FROM T_USERS T
WHERE T.DBI_ISPHANTOM = 0 AND T.ID NOT IN(
    SELECT T1.USERID
    FROM T_RBAC_ORGPERMISSIONUSER T1
    WHERE T1.DBI_ISPHANTOM = 0 AND T1.ORGPERMISSIONID = {0}
)";

            sql.Parameters.Add(permisssionId);

            return (UserList)(this.DataQueryer as RdbDataQueryer).QueryData(sql);
        }

        /// <summary>
        /// 通过用户查询对象，查询用户信息
        /// </summary>
        /// <param name="criteria">用户查询对象</param>
        /// <returns></returns>
        public UserList GetByUserCriteria(UserCriteria criteria)
        {
            var cqc = new CommonQueryCriteria();

            //Id
            if (criteria.Id != null)
            {
                cqc.Add(new PropertyMatch(User.IdProperty, criteria.Id));
            }

            //登录名
            if (!string.IsNullOrEmpty(criteria.UserName))
            {
                cqc.Add(new PropertyMatch(User.UserNameProperty, criteria.UserName));
            }

            //密码
            if (!string.IsNullOrEmpty(criteria.Password))
            {
                cqc.Add(new PropertyMatch(User.PasswordProperty, criteria.Password));
            }

            //员工号
            if (!string.IsNullOrEmpty(criteria.EmployeeNumber))
            {
                cqc.Add(new PropertyMatch(UserExt.EmployeeNumberProperty, criteria.EmployeeNumber));
            }

            return this.Repository.GetBy(cqc);
        }

        
        public UserList GetByUserNameAndEmployeeNumber(string userName,string employeeNumber)
        {
            var cqc = new CommonQueryCriteria(BinaryOperator.Or);


            //登录名
            if (!string.IsNullOrEmpty(userName))
            {
                cqc.Add(new PropertyMatch(User.UserNameProperty, userName));
            }

            //员工号
            if (!string.IsNullOrEmpty(employeeNumber))
            {
                cqc.Add(new PropertyMatch(UserExt.EmployeeNumberProperty, employeeNumber));
            }

            return this.Repository.GetBy(cqc);
        }

        public bool IsExistsByUserName(User user)
        {
            var cqc = new CommonQueryCriteria();

            cqc.Add(new PropertyMatch(User.IdProperty,PropertyOperator.NotEqual,user.Id));
            //登录名
            if (!string.IsNullOrEmpty(user.UserName))
            {
                cqc.Add(new PropertyMatch(User.UserNameProperty, user.UserName));
            }
            var list = this.Repository.GetBy(cqc);

            return list != null && list.Count > 0;
        }

        ///// <summary>
        ///// 通过员工号查询指定的员工号
        ///// </summary>
        ///// <param name="employeeNumber"></param>
        ///// <param name="eagerLoad"></param>
        ///// <returns></returns>
        //public User GetEmployeeNumber(string employeeNumber, EagerLoadOptions eagerLoad = null)
        //{
        //    var criteria = new CommonQueryCriteria
        //    {
        //        new PropertyMatch(User.EmployeeNumberProperty, employeeNumber)
        //    };
        //    criteria.EagerLoad = eagerLoad;
        //    return this.GetFirstBy(criteria);
        //}

        #region PatrickLiu 增加的代码

        /// <summary>
        /// 根据角色编码和销方主键获取用户列表
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="saleInfoID">销方的主键</param>
        /// <returns>返回符合条件的用户列表</returns>
        [RepositoryQuery]
        public virtual UserList GetUserListByRoleCodeAndSaleInfoID(string roleCode, long saleInfoID)
        {
            if (string.IsNullOrEmpty(roleCode))
            {
                throw new ArgumentNullException("角色编码不能为空！");
            }
            if (saleInfoID == 0)
            {
                throw new ArgumentNullException("销方的主键不能为空！");
            }
            //查找消息的接收人，接收人必须满足2个条件
            //1、接受用户必须是认证管理员
            //2、接受用户必须是该销方下的用户
            FormattedSql sql =
                @"select * from t_users t inner join (select t1.UserID from (select UserID from t_rbac_organizationuser
                                          where organizationid =(select id from t_rbac_organization
                                                  where id = (select Organizationid from t_saleinfo where";

            sql.Append(" id = " + saleInfoID.ToString());
            sql.Append(@"and wf_approvalstatus = 300 and dbi_isphantom = 0) and wf_approvalstatus = 300 and dbi_isphantom = 0)
                          and dbi_isphantom = 0) t1  inner join(select UserID from t_rbac_userrole where RoleID in(select id from t_rbac_role where ");
            sql.Append("Code = '" + roleCode + "'");
            sql.Append("and dbi_isphantom = 0) and dbi_isphantom = 0) t2 on t1.UserID = t2.UserID) t3 on t.id = t3.UserID");

            return (UserList)(this.DataQueryer as RdbDataQueryer).QueryData(sql);
        }

        #endregion
    }

    /// <summary>
    /// 用户查询对象
    /// </summary>
    public partial class UserCriteria : Criteria
    {
        public string EmployeeNumber { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
