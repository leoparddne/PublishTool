using PublishTools.Model;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PublishTools
{
    /// <summary>
    /// TemplateManager.xaml 的交互逻辑
    /// </summary>
    public partial class AddManager : Window
    {
        List<Template> resultFiles = new List<Template>();
        Dictionary<string, List<Template>> templateList;
        Action SaveConfig;
        public AddManager(Dictionary<string, List<Template>> templateList, Action SaveConfig)
        {
            InitializeComponent();
            this.templateList = templateList;
            this.SaveConfig = SaveConfig;

        }

        //add
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInput())
            {
                var file = txtFile.Text.Trim();
                var resultFile = txtResultFileName.Text.Trim();
                resultFiles.Add(new Model.Template
                {
                    FileName = file,
                    ResultFileName = resultFile
                });
                txtGrid.DataContext = resultFiles;
            }
        }

        private bool CheckInput()
        {
            ClearErr();

            if (string.IsNullOrEmpty(txtFile.Text.Trim()))
            {
                SetErr("请选择文件");
                return false;
            }
            if (string.IsNullOrEmpty(txtResultFileName.Text.Trim()))
            {
                SetErr("请设置替换后的文件名称");
                return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                SetErr("请输入配置模板名称");
                return;
            }
            //if (resultFiles.Count == 0)
            //{
            //    SetErr("请添加配置");
            //    return;
            //}

            templateList.TryGetValue(txtName.Text.Trim(), out var data);
            if (data == null)
            {
                templateList.Add(txtName.Text.Trim(), resultFiles);
            }
            else
            {
                templateList[txtName.Text.Trim()] = resultFiles;
            }
            SaveConfig();

            SetErr("保存成功");
        }
        private void SetErr(string err)
        {
            txtErr.Content = err.Trim();
        }

        private void ClearErr()
        {
            txtErr.Content = "";
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFile.Text = open.FileName;
            }
        }
    }
}
