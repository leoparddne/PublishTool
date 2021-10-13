using Newtonsoft.Json;
using PublishTools.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishTools.Ex
{
    public static class TemplateEx
    {
        public static string CFGFile = "tmp.cfg";

        public static void SaveConfig(string ResourcePath,string beforePackCMD, string PackCMDPath, Dictionary<string, List<Template>> Template)
        {
            var cfg = new TempPath
            {
                ResourcePath = ResourcePath,
                BeforePackCMD= beforePackCMD,
                PackCMDPath = PackCMDPath,
                Template = Template
            };
            File.WriteAllText(CFGFile, JsonConvert.SerializeObject(cfg));
        }
    }
}
