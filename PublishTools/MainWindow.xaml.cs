using Microsoft.Win32;
using Newtonsoft.Json;
using PublishTools.Ex;
using PublishTools.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PublishTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Dictionary<string, List<Template>> templateList = new Dictionary<string, List<Template>>();

        //发布结果
        private int errorCount;
        //发布错误信息
        StringBuilder error = new StringBuilder();


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //获取历史配置信息
            if (File.Exists(TemplateEx.CFGFile))
            {
                var cfgStr = File.ReadAllText(TemplateEx.CFGFile);
                try
                {
                    var cfg = JsonConvert.DeserializeObject<TempPath>(cfgStr);
                    txtPack.Text = cfg.PackCMDPath;
                    txtResource.Text = cfg.ResourcePath;
                    templateList = cfg.Template;

                    ReflashSelect();
                }
                catch (Exception)
                {
                }
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog file = new Microsoft.Win32.OpenFileDialog();
            if (file.ShowDialog() == true)
            {
                txtPack.Text = file.FileName;
            }
        }
        private void btnPack_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBeforePack.Text))
            {
                var beforeCMD = new CMDEx(txtResult);
                beforeCMD.RunCmd("", txtBeforePack.Text.Trim());
            }

            errorCount = 0;
            if (cobTemplate.SelectedItem == null)
            {
                return;
            }
            var directorys = Directory.GetDirectories(txtResource.Text.Trim()).ToList();
            directorys.Add(txtResource.Text.Trim());
            foreach (var item in directorys)
            {
                //删除
                foreach (var file in Directory.GetFiles(item))
                {
                    if (file.EndsWith(".pdb"))
                    {
                        if (!File.Exists(file))
                        {
                            CallErr($"{file}调试文件无法删除");
                            continue;
                        }
                        File.Delete(file);
                    }
                }
            }


            //替换文件
            if (templateList.TryGetValue(cobTemplate.SelectedItem.ToString(), out var templates))
            {
                foreach (var item in templates)
                {
                    if (item.ISDir)
                    {
                        if (!Directory.Exists(item.FileName))
                        {
                            CallErr($"{item.FileName}目录不存在");
                            continue;
                        }
                        var resultPath = System.IO.Path.Combine(txtResource.Text.Trim(), item.ResultFileName);
                        if (!Directory.Exists(resultPath))
                        {
                            Directory.CreateDirectory(resultPath);
                        }

                        CopyDir(item.FileName, resultPath);
                    }
                    else
                    {
                        var resultPath = System.IO.Path.Combine(txtResource.Text.Trim(), item.ResultFileName);
                        if (!File.Exists(item.FileName))
                        {
                            CallErr($"{item.FileName}文件不存在");
                            continue;
                        }
                        File.Copy(item.FileName, resultPath, true);

                    }
                }
            }

            if (errorCount > 0)
            {
                var errFile = $"{DateTime.Now.ToString("yyyyMMdd")}.packError.txt";
                File.WriteAllText(errFile, error.ToString());
                System.Windows.MessageBox.Show($"存在{errorCount}条错误信息,请确认,文件【{errFile}】");
            }

            if (!string.IsNullOrEmpty(txtPack.Text))
            {
                var cmd = new CMDEx(txtResult);
                cmd.RunCmd("", txtPack.Text.Trim());
            }
        }

        private void CopyDir(string dir, string des)
        {
            if (!Directory.Exists(des))
            {
                Directory.CreateDirectory(des);
            }
            var dirs = Directory.GetDirectories(dir);
            var files = Directory.GetFiles(dir);
            if (files.Any())
            {

                foreach (var filePath in files)
                {
                    var fileInfo = new FileInfo(filePath);

                    var resultFile = System.IO.Path.Combine(des, fileInfo.Name);
                    File.Copy(filePath, resultFile, true);
                }
            }

            if (!dirs.Any())
            {
                return;
            }
            foreach (var subDir in dirs)
            {
                var fileInfo = new FileInfo(subDir);
                var resultDir = System.IO.Path.Combine(des, fileInfo.Name);

                CopyDir(subDir, resultDir);
            }
        }

        private void CallErr(string err)
        {
            errorCount++;
            AddErr(err);

            error.AppendLine(err);
        }

        private void AddErr(string err)
        {
            AddLog($"【Error】{err}");
        }

        private void AddLog(string log)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtResult.AppendText($"【{DateTime.Now.ToString("yyyy--MM-dd HH:mm:ss")}】{log}\r\n");
                txtResult.ScrollToEnd();
            });
        }

        private void btnResource_Click(object sender, RoutedEventArgs e)
        {
            var file = new FolderBrowserDialog();
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtResource.Text = file.SelectedPath;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //替换操作在具体的配置页面完成
            SaveConfig();
        }
        private void addTemplate_Click(object sender, RoutedEventArgs e)
        {
            new AddManager(templateList, SaveConfig).ShowDialog();
            ReflashSelect();
        }

        private void ReflashSelect()
        {
            cobTemplate.Items.Clear();
            foreach (var item in templateList)
            {
                cobTemplate.Items.Add(item.Key);
            }
        }

        private void delTemplate_Click(object sender, RoutedEventArgs e)
        {
            new DeleteTemplate(templateList, SaveConfig).ShowDialog();
            ReflashSelect();
        }

        private void SaveConfig()
        {
            TemplateEx.SaveConfig(txtResource.Text, txtBeforePack.Text, txtPack.Text, templateList);
        }

        private void btnBeforeOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog file = new Microsoft.Win32.OpenFileDialog();
            if (file.ShowDialog() == true)
            {
                txtBeforePack.Text = file.FileName;
            }
        }
    }
}
