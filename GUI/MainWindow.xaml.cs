using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new FolderBrowserDialog();
            openFolderDialog.ShowDialog();
            //    txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
        }

        /*private void ShowOpenFolderDialog()
        {
            RadOpenFolderDialog openFolderDialog = new RadOpenFolderDialog();
            openFolderDialog.Owner = this;
            openFolderDialog.ShowDialog();
            if (openFolderDialog.DialogResult == true)
            {
                string folderName = openFolderDialog.FileName;
            }
        }*/
    }
}
