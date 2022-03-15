﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            startDirectory.Text = openFolderDialog.SelectedPath;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            clearOutputScreen();

            string start = startDirectory.Text;                     // Nama strating directory
            string fileName = ipFileName.Text;                      // Nama file yang ingin dicari
            bool Occurence = (bool) ipFindAllOccurence.IsChecked;   // Mode pencarian (semua kemunculan (true) / kemunculan pertama (false))
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (btnBFS.IsChecked == true)
            {
                // ALGORITMA BFS
                
            } 
            else if (btnDFS.IsChecked == true)
            {
                // ALGORITMA DFS

            }
            stopWatch.Stop();

            // Output
            opPathList.Items.Add(@"C:\");   // Contoh, parameter string bisa diganti dengan directory hasil pencarian
            opTimeSpent.Text += stopWatch.ElapsedMilliseconds.ToString() + " ms";
        }

        private void pathFile_Click(object sender, RoutedEventArgs e)
        {
            // Sementara
            for (int i = 0; i < opPathList.Items.Count; i++)
            {
                //var appPath1 = System.AppDomain.CurrentDomain.BaseDirectory;
                var appPath = opPathList.Items[i].ToString();
                Process.Start(appPath);
            }
        }

        private void clearOutputScreen()
        {
            opPathList.Items.Clear();
            opTimeSpent.Text = "Time Spent: ";
        }
    }
}