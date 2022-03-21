using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
                            && !string.IsNullOrEmpty(ipFileName.Text) && (btnBFS.IsChecked == true || btnDFS.IsChecked == true);
            if (inputValid)
            {
                ClearOutputScreen();

                string start = startDirectory.Text;                     // Nama starting directory
                string fileName = ipFileName.Text;                      // Nama file yang ingin dicari
                bool Occurence = (bool) ipFindAllOccurence.IsChecked; // Mode pencarian (semua kemunculan (true) / kemunculan pertama (false))

                Stopwatch stopWatch = new Stopwatch();

                DirectoryInfo diSource = new DirectoryInfo(start);      // Strating directory
                List<string> path = new List<string>();                 // List berisi path file yang dicari (result)
                NTree<string> pohon = new NTree<string>(diSource.Name,0,diSource.FullName);
                stopWatch.Start();
                if (btnBFS.IsChecked == true)
                {
                    // ALGORITMA BFS

                }
                else if (btnDFS.IsChecked == true)
                {
                    // ALGORITMA DFS
                    bool found = false;
                    pohon = DepthFirstSearch.searchFolder(diSource, fileName, path,out found,Occurence);
                    
                }
                stopWatch.Stop();

                /* Output */
                // Menampilkan gambar pohon
                ViewerSample.drawTree(pohon);
                opTreeVisual.Source = new BitmapImage(new Uri(ViewerSample.treeImagePath));

                // Menampilkan path dari file yang dicari
                if (path.Count > 0)
                {
                    textBlockPathList.Text += " (Double click to open folder)";
                }    
                for (int i = 0; i < path.Count; i++)
                {
                    opPathList.Items.Add(path[i]);
                }
                // Menampilkan waktu yang diperlukan selama pencarian
                opTimeSpent.Text += stopWatch.ElapsedMilliseconds.ToString() + " ms";

                // Menampilkan tombol untuk membuka window baru untuk gambar
                btnOpenInNewWindow.Visibility = Visibility.Visible;
            }

        }

        private void BtnOpenInNewWindow_Click(object sender, RoutedEventArgs e)
        {
            //create a form 
            Form form = new Form();
            //associate the viewer with the form 
            form.SuspendLayout();
            ViewerSample.viewer.Dock = DockStyle.Fill;
            form.Controls.Add(ViewerSample.viewer);
            form.ResumeLayout();
            //show the form
            form.ShowDialog();
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
            textBlockPathList.Text = "Path List:";
            opPathList.Items.Clear();
            opTimeSpent.Text = "Time Spent: ";
        }
        
    }
    class ViewerSample
    {
        public static string treeImagePath = "";
        public static List<string> treeImagePathList = new List<string>();
        //create a viewer object 
        public static GViewer viewer = new GViewer();

        /*  TESTING
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
        */
        public static void drawTree(NTree<string> tree)
        {
            //create a graph object 
            Graph graph = new Graph("graph");
            //create a graph renderer object
            GraphRenderer renderer = new GraphRenderer(graph);
            //create the graph content 
            int id=0;
            graph = createTree(graph, tree);
            //bind the graph to the viewer 
            viewer.Graph = graph;

            //bind the graph to the renderer 
            renderer.CalculateLayout();
            int width = (int) graph.Width;
            int height = (int) graph.Height;
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            renderer.Render(bitmap);
            //save graph as image in png format
            Random rnd = new Random();
            int num = rnd.Next();
            treeImagePath = Directory.GetCurrentDirectory() + "/treeImage" + num + ".png";
            treeImagePathList.Add(treeImagePath);
            bitmap.Save(treeImagePath);
        }
        public static Graph createTree(Graph graph, NTree<string> tree)
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
            Node parent = new Node(tree.path);
            parent.LabelText = tree.data;
            graph.AddNode(parent);
            foreach (NTree<string> kid in tree.children)
            {
                Node child = new Node(kid.path);
                child.LabelText = kid.data;
                graph.AddNode(child);
                graph.AddEdge(parent.Id, child.Id);
                if (kid.colour == 1) graph.FindNode(child.Id).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Blue;
                else if (kid.colour == 0) graph.FindNode(child.Id).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                createTree(graph, kid);

            }
            if (tree.colour == 1) graph.FindNode(parent.Id).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Blue;
            else if (tree.colour == 0) graph.FindNode(parent.Id).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;

            return graph;
        }
    }
}
