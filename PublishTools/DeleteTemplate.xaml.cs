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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PublishTools
{
    /// <summary>
    /// DeleteTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class DeleteTemplate : Window
    {
        Dictionary<string, List<Template>> template;
        Action SaveConfig;

        public DeleteTemplate(Dictionary<string, List<Template>> template, Action SaveConfig)
        {
            InitializeComponent();
            this.template = template;
            this.SaveConfig = SaveConfig;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReFlashSelect();
        }
        private void ClearErr()
        {
            SetErr("");
        }
        private void SetErr(string err)
        {
            labelErr.Content = err.Trim();
        }

        private void ReFlashSelect()
        {
            comTemplate.Items.Clear();
            foreach (var item in template)
            {
                comTemplate.Items.Add(item.Key);
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            ClearErr();
            if (comTemplate.SelectedItem == null)
            {
                return;
            }
            comTemplate.Items.Remove(comTemplate.SelectedItem);

            try
            {
                //template.Remove(key);
                SaveConfig();
            }
            catch (Exception)
            {

            }

            SetErr("移除成功");
        }

        private void cobTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comTemplate.SelectedItem != null)
            {
                var key = comTemplate.SelectedItem.ToString();

                if (template.TryGetValue(key, out var value))
                {
                    txtGrid.DataContext = value;
                }
            }
            else
            {

            }

        }
    }
}
