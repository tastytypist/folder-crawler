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
using DFS;

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

        private void BtnChooseFolder_Click(object sender, RoutedEventArgs e)
        // Menampilkan window file explorer ketika menekan tombol search
        {
            var openFolderDialog = new FolderBrowserDialog();
            openFolderDialog.ShowDialog();
            startDirectory.Text = openFolderDialog.SelectedPath;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        // Mengeksekusi program ketika menekan submit
        {
            bool inputValid = !string.IsNullOrEmpty(startDirectory.Text) && !string.IsNullOrWhiteSpace(startDirectory.Text) && Directory.Exists(startDirectory.Text)
                            && !string.IsNullOrEmpty(ipFileName.Text) && (btnBFS.IsChecked.HasValue == true || btnDFS.IsChecked.HasValue == true);
            if (inputValid)
            {
                ClearOutputScreen();

                string start = startDirectory.Text;                     // Nama starting directory
                string fileName = ipFileName.Text;                      // Nama file yang ingin dicari
                bool Occurence = ipFindAllOccurence.IsChecked.HasValue; // Mode pencarian (semua kemunculan (true) / kemunculan pertama (false))

                Stopwatch stopWatch = new Stopwatch();

                DirectoryInfo diSource = new DirectoryInfo(start);      // Strating directory
                List<string> path = new List<string>();                 // List berisi path file yang dicari (result)
                NTree<string> pohon = new NTree<string>(diSource.FullName,0);
                stopWatch.Start();
                if (btnBFS.IsChecked == true)
                {
                    // ALGORITMA BFS

                }
                else if (btnDFS.IsChecked == true)
                {
                    // ALGORITMA DFS
                    Occurence = true;
                    bool found = false;
                    pohon = SearchDir.searchFolder(diSource, fileName, path,out found,Occurence);
                    
                }
                stopWatch.Stop();

                /* Output */
                // Menampilkan gambar pohon (Sementara)
                //string gambarPohon = @"D:\Personal\OneDrive - Institut Teknologi Bandung\Documents\Programming\GitHub\folder-crawler\src\GUI\dummy.png";
                //opTreeVisual.Source = new BitmapImage(new Uri(gambarPohon));
                // Menampilkan path dari file yang dicari
                textBlockPathList.Text += " (Double click to open folder)";
                for (int i = 0; i < path.Count; i++)
                {
                    opPathList.Items.Add(path[i]);
                }
                // Menampilkan waktu yang diperlukan selama pencarian
                opTimeSpent.Text += stopWatch.ElapsedMilliseconds.ToString() + " ms";
                ViewerSample.testGraph(pohon);
            }

        }

        private void PathFile_Click(object sender, RoutedEventArgs e)
        // Hyperlink ke path file yang diklik
        {
            if (opPathList.SelectedItem != null)
            {
                string filePath = opPathList.SelectedItem.ToString();
                string folderPath = new DirectoryInfo(System.IO.Path.GetDirectoryName(filePath)).FullName;
                if (Directory.Exists(folderPath))
                {
                    Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", folderPath);
                }
            }
        }

        private void ClearOutputScreen()
        // Inisialisasi output screen (hapus semua value)
        {
            opTreeVisual.Source = null;
            opPathList.Items.Clear();
            opTimeSpent.Text = "Time Spent: ";
        }
        
    }
    class ViewerSample
    {
        public static void drawGraph()
        {
            //create a form 
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            //create a viewer object 
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
            //create the graph content 
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("A", "C").Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
            graph.AddEdge("B", "D").Attr.Color = Microsoft.Msagl.Drawing.Color.Aqua;
            graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.MistyRose;
            Microsoft.Msagl.Drawing.Node c = graph.FindNode("C");
            c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;
            //bind the graph to the viewer 
            viewer.Graph = graph;
            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            //show the form 
            form.ShowDialog();
        }
        public static void testGraph(NTree<string> tree)
        {
            //create a form 
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            //create a viewer object 
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
            //create the graph content 
            graph = createTree(graph, tree);
            //bind the graph to the viewer 
            viewer.Graph = graph;
            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            //show the form 
            form.ShowDialog();
        }
        public static Microsoft.Msagl.Drawing.Graph createTree(Microsoft.Msagl.Drawing.Graph graph, NTree<string> tree)
        {
            /*
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("A", "C").Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
            graph.AddEdge("B", "D").Attr.Color = Microsoft.Msagl.Drawing.Color.Aqua;
            graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.MistyRose;
            Microsoft.Msagl.Drawing.Node c = graph.FindNode("C");
            c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;*/
            foreach (NTree<string> kid in tree.children)
            {
                graph.AddEdge(tree.data, kid.data);
                if (kid.colour == 1) graph.FindNode(kid.data).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Blue;
                else if (kid.colour == 0) graph.FindNode(kid.data).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                createTree(graph, kid);

            }
            if (tree.colour == 1) graph.FindNode(tree.data).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Blue;
            else if (tree.colour == 0) graph.FindNode(tree.data).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;

            return graph;
        }
    }
}
