/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160618
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160618 16:49
 * 
*******************************************************/

using System.Linq;
using System.Collections.Generic;
using Rafy.Accounts;
using Rafy.Domain;
using Rafy.RBAC.Enum;
using Rafy.RBAC.Cache;

namespace Rafy.RBAC.Controllers.Authentication
{
    /// <summary>
    /// 此方法用于认证用户的数据权限
    /// </summary>
    public class DataAuthenticationController : DomainController
    {
        #region 私有字段 构造方法

        /// <summary>
        /// 用户角色仓库
        /// </summary>
        private readonly UserRoleRepository _userRoleRepository;

        /// <summary>
        /// 角色操作仓库
        /// </summary>
        private readonly RoleOperationRepository _roleOperationRepository;

        /// <summary>
        /// 资源操作仓库
        /// </summary>
        private readonly ResourceOperationRepository _operationRepository;

        /// <summary>
        /// 组织仓库
        /// </summary>
        private readonly OrganizationRepository _organizationRepository;

        /// <summary>
        /// 组织用户仓库
        /// </summary>
        private readonly OrganizationUserRepository _orgUserRepository;

        public DataAuthenticationController()
        {
            this._userRoleRepository = RF.Concrete<UserRoleRepository>();
            this._roleOperationRepository = RF.Concrete<RoleOperationRepository>();
            this._operationRepository = RF.Concrete<ResourceOperationRepository>();
            this._organizationRepository = RF.Concrete<OrganizationRepository>();
            this._orgUserRepository = RF.Concrete<OrganizationUserRepository>();
        }

        #endregion

        /// <summary>
        /// 获取自己组织的用户
        /// </summary>
        /// <param name="organizationId">组织ID</param>
        /// <returns></returns>
        public List<long> GetOwnerOrg(long organizationId)
        {
            var list = new List<long>();
            var orgUserList = this._orgUserRepository.GetByOrganizationId(organizationId);

            foreach (var orgUser in orgUserList)
            {
                list.Add(orgUser.UserId);
            }
            
            return list;
        }

        /// <summary>
        /// 获取直接上级组织的用户
        /// </summary>
        /// <param name="organizationId">组织ID</param>
        /// <returns></returns>
        public List<long> GetImmediateSuperior(long organizationId)
        {
            var org = this._organizationRepository.GetById(organizationId);
            return this.GetOwnerOrg(org.TreePId.Value);
        }
        
        /// <summary>
        /// 获取自己和直接上级组织的用户
        /// </summary>
        /// <param name="organizationId">组织ID</param>
        /// <returns></returns>
        public List<long> GetOwnerAndImmediateSuperior(long organizationId)
        {
            //获取自己组织的用户
            List<long> list = this.GetOwnerOrg(organizationId);

            //获取上级组织的用户
            list.AddRange(this.GetImmediateSuperior(organizationId));

            return list;
        }

        /// <summary>
        /// 通过角色ID获取用户。
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="currentUserId">申请人</param>
        /// <returns></returns>
        public List<long> GetUserByRole(long roleId,long currentUserId)
        {
            List<long> list = new List<long>();
            UserRoleList userRoleList = this._userRoleRepository.GetByRoleId(roleId);
            foreach (var userRole in userRoleList)
            {
                if (userRole.UserId != currentUserId) list.Add(userRole.UserId);
            }

            return list;
        }

        /// <summary>
        /// 此方法认证用户在资源下的数据权限。
        /// 认证成功后，返回该用户，可以看到数据的创建人列表。
        /// </summary>
        /// <param name="user">需要认证的用户</param>
        /// <param name="resource">需要认证的资源</param>
        /// <param name="organization">需要认证的用户的当前组织。</param>
        /// <returns></returns>
        public UserList AuthenticateUserDataOperation(User user, Resource resource,Organization organization)
        {
            if (null != user && null != resource && null != organization)
            {
                return this.GetUserListByOperation(this.GetOwnerOperation(user,resource), user, organization);
            }

            return null;
        }

        /// <summary>
        /// 此方法认证用户在资源下的数据权限。
        /// 认证成功后，返回该用户可以看到数据的组织列表。
        /// </summary>
        /// <param name="resource">需要认证的资源。</param>
        /// <param name="organization">需要认证的组织。</param>
        /// <param name="user">需要认证的用户。</param>
        /// <returns></returns>
        public OrganizationList AuthenticateOrgDataOperation(User user,Resource resource,Organization organization)
        {
            if (null != user && null != resource && null != organization)
            {
                List<ResourceOperation> list = this.GetOwnerOperation(user, resource);

                return this.GetOrganizationByOperation(list, organization);
            }

            return null;
        }

        private OrganizationList GetOrganizationByOperation(List<ResourceOperation> list, Organization organization)
        {
            OrganizationList orgList = new OrganizationList();
            orgList.AutoTreeIndexEnabled = false;

            DataConstraintRule constraintRule = DataConstraintRule.All;

            if (null != list)
            {
                foreach (var operation in list)
                {
                    DataConstraintRule currentConstraintRule;
                    currentConstraintRule = System.Enum.TryParse(operation.OperationName, out currentConstraintRule) ? currentConstraintRule : DataConstraintRule.Own;
                    var value = constraintRule.CompareTo(currentConstraintRule);
                    if(value < 0)
                        constraintRule = currentConstraintRule;
                }
            }

            if(constraintRule == DataConstraintRule.OwnCompany)
            {
                orgList.Add(organization);
            }
            else if(constraintRule == DataConstraintRule.Lower)
            {
                orgList.AddRange(HierarchicalStructureDataCache.OrganizationList.Where(o =>o.TreeIndex.StartsWith(organization.TreeIndex)));
            }
            else if(constraintRule == DataConstraintRule.All)
            {
                orgList.AddRange(HierarchicalStructureDataCache.OrganizationList);
            }

            return orgList;
        }

        /// <summary>
        /// 通过数据过滤规则获取用户。
        /// </summary>
        /// <param name="rol">数据过滤规则操作。</param>
        /// <param name="organization">待过滤的组织数据。</param>
        /// <param name="user">待过滤的用户数据。</param>
        /// <returns></returns>
        private UserList GetUserListByOperation(List<ResourceOperation> rol,User user,Organization organization)
        {
            UserList userList = new UserList();
            DataConstraintRule constraintRule = DataConstraintRule.All;

            if (null != rol)
            {
                foreach (var operation in rol)
                {
                    DataConstraintRule currentConstraintRule;
                    currentConstraintRule = System.Enum.TryParse(operation.OperationName, out currentConstraintRule) ? currentConstraintRule : DataConstraintRule.Own;
                    var value = constraintRule.CompareTo(currentConstraintRule);
                    if(value < 0)
                        constraintRule = currentConstraintRule;
                }
            }

            if (constraintRule == DataConstraintRule.Own)
            {
                userList.Add(user);
            }
            else if(constraintRule == DataConstraintRule.OwnCompany)
            {
                userList.AddRange(this.GetUserListByOwnCompanyRule(organization));
            }
            else if(constraintRule == DataConstraintRule.Lower)
            {
                userList.AddRange(this.GetUserListByLowerRule(organization));
            }

            return userList;
        }

        public DataConstraintRule GetByOperationName(string operationName)
        {
            DataConstraintRule value;

            return System.Enum.TryParse(operationName, out value) ? value : DataConstraintRule.Own;
        }

        /// <summary>
        /// 获取自己组织的用户。
        /// </summary>
        /// <param name="organization">需要获取用户的组织。</param>
        /// <returns></returns>
        private UserList GetUserListByOwnCompanyRule(Organization organization)
        {
            UserList userList = new UserList();
            OrganizationUserList oul = this._orgUserRepository.GetByOrganizationId(organization.Id);
            foreach (var orgUser in oul)
            {
                userList.Add(orgUser.User);
            }
            return userList;
        }

        /// <summary>
        /// 获取自己及下级组织的用户。
        /// </summary>
        /// <param name="organization">需要获取用户的组织。</param>
        /// <returns></returns>
        private UserList GetUserListByLowerRule(Organization organization)
        {
            UserList userList = new UserList();
            List<long> orgId = new List<long>();
            OrganizationList orgList = this._organizationRepository.GetByTreeParentIndex(organization.TreeIndex);
            foreach (var org in orgList)
            {
                orgId.Add(org.Id);
            }
            orgId.Add(organization.Id);

            OrganizationUserList oul = this._orgUserRepository.GetByOrganizationId(orgId.ToArray());

            foreach (var orgUser in oul)
            {
                if (null != orgUser.User)
                {
                    userList.Add(orgUser.User);
                }
            }

            return userList;
        }

        /// <summary>
        /// 此方法用于获取用户在某个资源下的数据操作。
        /// </summary>
        /// <param name="user">用户数据。</param>
        /// <param name="resource">资源数据。</param>
        /// <returns></returns>
        public List<ResourceOperation> GetOwnerOperation(User user, Resource resource)
        {
            //1、通过用户获取角色。
            var rolesId = HierarchicalStructureDataCache.UserRoleList.Where(u => u.UserId == user.Id).Select(u=>u.RoleId).ToList();

            //2、通过角色获取操作。
            var roleOperationsId = HierarchicalStructureDataCache.RoleOperationList.Where(ro => rolesId.Contains(ro.RoleId)).Select(ro => ro.OperationId).ToList();

            //3、通过操作ID和资源ID获取操作。
            var operationList = HierarchicalStructureDataCache.ResourceOperationList.Where(ro => roleOperationsId.Contains(ro.Id) && ro.ResourceId == resource.Id && ro.OperationType == OperationType.Data).ToList();

            return operationList;
        }
    }
}
