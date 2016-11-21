/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160319
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160319 14:44
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
using Rafy.Domain;
using Rafy.Domain.ORM;
using Rafy.Domain.Validation;
using Rafy.MetaModel;
using Rafy.MetaModel.Attributes;
using Rafy.MetaModel.View;
using Rafy.ManagedProperty;

namespace Rafy.RBAC
{
    /// <summary>
    /// 权限管理领域模型的基类
    /// </summary>
    [Serializable]
    public abstract class RBACEntity : LongEntity
    {
        #region 构造函数

        protected RBACEntity() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected RBACEntity(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion
    }

    [Serializable]
    public abstract class RBACEntityList : EntityList { }

    public abstract class RBACEntityRepository : EntityRepository
    {
        protected RBACEntityRepository() { }
    }

    [DataProviderFor(typeof(RBACEntityRepository))]
    public class RBACEntityRepositoryDataProvider : RdbDataProvider
    {
        protected override string ConnectionStringSettingName
        {
            get { return RBACPlugin.DbSettingName; }
        }
    }

    public abstract class RBACEntityConfig<TEntity> : EntityConfig<TEntity> { }
}