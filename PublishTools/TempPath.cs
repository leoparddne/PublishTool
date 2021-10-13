using PublishTools.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishTools
{
    public class TempPath
    {
        public string ResourcePath { get; set; }
        public string PackCMDPath { get; set; }
        public Dictionary<string, List<Template>> Template { get; set; }
        public string BeforePackCMD { get;  set; }
    }
}
