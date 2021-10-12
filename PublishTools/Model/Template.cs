using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishTools.Model
{
    public class Template
    {
        public string FileName { get; set; }

        public string ResultFileName { get; set; }

        /// <summary>
        /// 是否为目录
        /// </summary>
        public bool ISDir { get;  set; }
    }
}
