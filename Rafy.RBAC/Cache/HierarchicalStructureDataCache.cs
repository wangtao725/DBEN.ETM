using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Caching;
using DBEN.ETM.Common.Cache;
using Rafy.Accounts;
using Rafy.Domain;
using System.Linq;
using System.Threading;

namespace Rafy.RBAC.Cache
{
    /// <summary>
    /// 树型菜单数据缓存。
    /// </summary>
    public static class HierarchicalStructureDataCache
    {
        private static readonly string _monitorFilePath;
        private static readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        static HierarchicalStructureDataCache()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var splitChar = path.EndsWith("\\") ? string.Empty : "\\";
            _monitorFilePath = $"{path}{splitChar}CacheMonitorFile.txt";

            if (!File.Exists(_monitorFilePath))
            {
                using(File.Create(_monitorFilePath)) { }
            }
        }

        public static readonly string UserCacheKey = "ACME_USER_CACHE_KEY";
        public static readonly string UserRoleCacheKey = "ACME_USER_ROLE_CACHE_KEY";
        public static readonly string RoleOperationCacheKey = "ACME_ROLE_OPERATION_CACHE_KEY";
        public static readonly string ResourceOperationCacheKey = "ACME_RESOURCE_OPERATION_CACHE_KEY";
        public static readonly string ResourceCacheKey = "ACME_RESOURCE_CACHE_KEY";
        public static readonly string OrganizationCacheKey = "ACME_ORGANIZATION_CACHE_KEY";
        public static readonly string OrganizationUserCacheKey = "ACME_ORGANIZATION_USER_CACHE_KEY";

        public static void Init()
        {
            object x = null;
            x = UserRoleList;
            x = RoleOperationList;
            x = ResourceOperationList;
            x = OrganizationList;
            x = null;
        }

        /// <summary>
        /// 设置缓存过期。
        /// </summary>
        public static void SetCacheExpire()
        {
            if(File.Exists(_monitorFilePath))
            {
                try
                {
                    _readerWriterLockSlim.EnterWriteLock();
                    File.SetLastWriteTime(_monitorFilePath, DateTime.Now.AddSeconds(-60));
                    _readerWriterLockSlim.ExitWriteLock();
                }
                catch (Exception e)
                {
                    throw new Exception($"时间：{DateTime.Now: yyyy-MM-dd HH:mm:ss}， 设置缓存过期失败。", e);
                }
            }
        }

        /// <summary>
        /// 获取一个缓存策略实例。
        /// </summary>
        /// <param name="slidingExpiration">缓存时长。单位：小时</param>
        /// <returns></returns>
        private static CacheItemPolicy _GetCacheItemPolicy(double slidingExpiration = 12)
        {
            try
            {
                var policy = new CacheItemPolicy {
                    SlidingExpiration = TimeSpan.FromHours(slidingExpiration),
                    ChangeMonitors = {
                        new HostFileChangeMonitor(new List<string> {
                            _monitorFilePath
                        })
                    },
                    RemovedCallback = argument => {
                        Debug.WriteLine(argument.CacheItem.Key);
                    }
                };

                return policy;
            }
            catch (Exception e)
            {
                throw new Exception($"时间：{DateTime.Now: yyyy-MM-dd HH:mm:ss}， 获取缓存策略失败。", e);
            }
        }

        /// <summary>
        /// 获取用户集合的缓存项。
        /// </summary>
        public static List<User> UserList
        {
            get
            {
                return CacheManager.Get(UserCacheKey, _GetCacheItemPolicy(), () => {
                    var userRepository = RF.Concrete<UserRepository>();
                    var userList = userRepository.GetAll();

                    return userList.Concrete().ToList();
                });
            }
        }

        /// <summary>
        /// 获取用户角色集合的缓存项。
        /// </summary>
        public static List<UserRole> UserRoleList
        {
            get
            {
                return CacheManager.Get(UserRoleCacheKey, _GetCacheItemPolicy(), () => {
                    var userRoleRepository = RF.Concrete<UserRoleRepository>();
                    var eagerLoad = new EagerLoadOptions().LoadWith(UserRole.UserProperty);
                    var userRoleList = userRoleRepository.GetAll(null, eagerLoad);

                    return userRoleList.Concrete().ToList();
                });
            }
        }

        /// <summary>
        /// 获取角色与按钮映射集合的缓存项。
        /// </summary>
        public static List<RoleOperation> RoleOperationList
        {
            get
            {
                return CacheManager.Get(RoleOperationCacheKey, _GetCacheItemPolicy(), () => {
                    var roleOperationRepository = RF.Concrete<RoleOperationRepository>();
                    var eagerLoad = new EagerLoadOptions()
                        .LoadWith(RoleOperation.RoleProperty)
                        .LoadWith(RoleOperation.OperationProperty);
                    var roleOperationList = roleOperationRepository.GetAll(null, eagerLoad);

                    return roleOperationList.Concrete().ToList();
                });
            }
        }

        /// <summary>
        /// 获取功能按钮集合的缓存项。
        /// </summary>
        public static List<ResourceOperation> ResourceOperationList
        {
            get
            {
                return CacheManager.Get(ResourceOperationCacheKey, _GetCacheItemPolicy(), () => {
                    var roleOperationRepository = RF.Concrete<ResourceOperationRepository>();
                    var eagerLoad = new EagerLoadOptions()
                        .LoadWith(ResourceOperation.ResourceProperty);
                    var resourceOperationList = roleOperationRepository.GetAll(null, eagerLoad);

                    return resourceOperationList.Concrete().ToList();
                });
            }
        }

        /// <summary>
        /// 获取组织集合的缓存项。
        /// </summary>
        public static List<Organization> OrganizationList
        {
            get
            {
                return CacheManager.Get(OrganizationCacheKey, _GetCacheItemPolicy(), () => {
                    var organizationRepository = RF.Concrete<OrganizationRepository>();
                    var organizationList = organizationRepository.GetAllOrganization();

                    return organizationList.Concrete().ToList();
                });
            }
        }

        /// <summary>
        /// 获取组织与用户映射集合的缓存项。
        /// </summary>
        public static List<OrganizationUser> OrganizationUserList
        {
            get
            {
                return CacheManager.Get(OrganizationUserCacheKey, _GetCacheItemPolicy(), () => {
                    var organizationUserRepository = RF.Concrete<OrganizationUserRepository>();
                    var list = organizationUserRepository.GetAll().Concrete().ToList();

                    return list;
                });
            }
        }

        /// <summary>
        /// 获取功能模块集合的缓存项。
        /// </summary>
        public static List<Resource> ResourceList
        {
            get
            {
                return CacheManager.Get(ResourceCacheKey, _GetCacheItemPolicy(), () => {
                    var resourceList = new List<Resource>();
                    var resourceRepository = RF.Concrete<ResourceRepository>();
                    var tempResourceList = resourceRepository.GetAll();

                    tempResourceList.EachNode(resource => {
                        var entity = resource as Resource;

                        if(entity == null)
                            return false;

                        resourceList.Add(entity);

                        return false;
                    });

                    return resourceList;
                });
            }
        }
    }
}
