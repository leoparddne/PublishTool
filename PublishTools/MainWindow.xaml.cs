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
                        File.Delete(file);
                    }
                }
            }

            //替换文件
            if (templateList.TryGetValue(cobTemplate.SelectedItem.ToString(), out var templates))
            {
                foreach (var item in templates)
                {
                    var resultPath = System.IO.Path.Combine(txtResource.Text.Trim(), item.ResultFileName);
                    File.Copy(item.FileName, resultPath, true);
                    var cmd = new CMDEx(txtResult);
                    cmd.RunCmd("", txtPack.Text.Trim());
                }
            }
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
            //SaveConfig();
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
            TemplateEx.SaveConfig(txtResource.Text, txtPack.Text, templateList);
        }
    }
}
