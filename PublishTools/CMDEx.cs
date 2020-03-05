using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PublishTools
{
    public class CMDEx
    {
        TextBox textBox;
        public CMDEx(TextBox textBox)
        {
            this.textBox = textBox;
        }
        /// <summary>
        /// 运行cmd命令
        /// 不显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        public void RunCmd(string cmdExe, string cmdStr)
        {
            try
            {
                using (Process myPro = new Process())
                {
                    myPro.StartInfo.UseShellExecute = false;

                    myPro.StartInfo.FileName = "cmd.exe";
                    myPro.StartInfo.UseShellExecute = false;
                    myPro.StartInfo.RedirectStandardInput = true;
                    myPro.StartInfo.RedirectStandardOutput = true;
                    myPro.StartInfo.RedirectStandardError = true;
                    myPro.StartInfo.CreateNoWindow = true;
                    myPro.Start();
                    //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                    string str = string.Format(@"{0} {1} {2}", cmdExe, cmdStr, "&exit");

                    myPro.StandardInput.WriteLine(str);
                    myPro.StandardInput.AutoFlush = true;

                    //异步获取命令行内容  
                    myPro.BeginOutputReadLine();
                    //为异步获取订阅事件  
                    myPro.OutputDataReceived += MyPro_OutputDataReceived;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void MyPro_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Action addendAction = new Action(() =>
              {
                  textBox.AppendText(e.Data + "\r\n");
                  textBox.ScrollToEnd();
              });
            textBox.Dispatcher.BeginInvoke(addendAction);
        }
    }
}
