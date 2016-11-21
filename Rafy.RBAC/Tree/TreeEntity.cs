/*******************************************************
 * 
 * 作者：赵朋
 * 创建日期：20160510
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 赵朋 20160510 17:16
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafy.RBAC.Tree
{
    public class TreeEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string PId { get; set; }

        public string Url { get; set; }

        public bool Leaf { get; set; }

        public TreeEntity[] Children { get; set; }
    }
}
