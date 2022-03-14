using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            this.DataContext = this;
        }

        private void btnChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new FolderBrowserDialog();
            openFolderDialog.ShowDialog();
            StartDirectory.Text = openFolderDialog.SelectedPath;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pathFile_Click(object sender, RoutedEventArgs e)
        {
            // Sementara
            var appPath = System.AppDomain.CurrentDomain.BaseDirectory;
            System.Diagnostics.Process.Start(appPath);
        }
    }
}
