using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using DFS;
using BFS;


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

                /* INPUT */
                string start = startDirectory.Text;                     // Nama (string) starting directory
                DirectoryInfo diSource = new DirectoryInfo(start);      // Starting directory
                string fileName = ipFileName.Text;                      // Nama file yang ingin dicari
                bool Occurence = (bool) ipFindAllOccurence.IsChecked;   // Mode pencarian (semua kemunculan (true) / kemunculan pertama (false))
                NTree<FileSystemInfo> pohon = new NTree<FileSystemInfo>(diSource, 0);
                List<string> path = new List<string>();                                         // List berisi path file yang dicari (result)
                     // Pohon pencarian yang terbentuk
                long elapsedTime = 0L;                                                            // Waktu pencarian 

                /* ALGORITMA BFS dan DFS*/
                if (btnBFS.IsChecked == true)
                {
                    // BFS
                    BreadthFirstSearch bfsSearch = new BreadthFirstSearch(Occurence);
                    (path, pohon, elapsedTime) = bfsSearch.BreadthSearchFile(diSource, fileName);
                }
                else if (btnDFS.IsChecked == true)
                {
                    // DFS
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    found found = new found(false);
                    pohon = DepthFirstSearch.searchFolder(diSource, fileName, path,found,Occurence);
                    stopWatch.Stop();
                    elapsedTime = stopWatch.ElapsedMilliseconds;
                }

                /* OUTPUT */
                // Menampilkan gambar pohon
                ViewerSample.drawTree(pohon);
                opTreeVisual.Source = new BitmapImage(new Uri(ViewerSample.treeImagePath));

                // Menampilkan path dari file yang dicari
                if (path.Count > 0)
                {
                    textBlockPathList.Text += "   (Double click to open folder)";
                } else if (path.Count == 0)
                {
                    textBlockPathList.Text += "   FILE NOT FOUND!";
                }
                for (int i = 0; i < path.Count; i++)
                {
                    opPathList.Items.Add(path[i]);
                }

                // Menampilkan waktu yang diperlukan selama pencarian
                opTimeSpent.Text += elapsedTime.ToString() + " ms";

                // Menampilkan tombol untuk membuka window baru untuk gambar
                btnOpenInNewWindow.Visibility = Visibility.Visible;
            }

        }

        private void BtnOpenInNewWindow_Click(object sender, RoutedEventArgs e)
        // Membuka window baru untuk menampilkan tree
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
        public static string treeImagePath = "";                                // path file gambar tree
        public static List<string> treeImagePathList = new List<string>();      // list semua path file gambar tree selama program dijalankan
        //create a viewer object 
        public static GViewer viewer = new GViewer();

        public static void drawTree(NTree<FileSystemInfo> tree)
        // Menggambar tree
        {
            //create a graph object 
            Graph graph = new Graph("graph");
            //create a graph renderer object
            GraphRenderer renderer = new GraphRenderer(graph);
            //create the graph content 
            graph = createTree(graph, tree);
            //bind the graph to the viewer 
            viewer.Graph = graph;
            //bind the graph to the renderer 
            renderer.CalculateLayout();
            int width = (int)graph.Width;
            int height = (int)graph.Height;
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            renderer.Render(bitmap);
            //save graph as image in png format
            Random rnd = new Random();
            int num = rnd.Next();
            treeImagePath = Directory.GetCurrentDirectory() + "/treeImage" + num + ".png";
            treeImagePathList.Add(treeImagePath);
            bitmap.Save(treeImagePath);
        }

        public static Graph createTree(Graph graph, NTree<FileSystemInfo> tree)
        // Membuat tree (msagl)
        {
            // Membuat node parent
            Node parent = new Node(tree.data.FullName);
            parent.LabelText = tree.data.Name;
            graph.AddNode(parent);
            // Iterasi setiap node children
            foreach (NTree<FileSystemInfo> kid in tree.children)
            {
                Node child = new Node(kid.data.FullName);
                child.LabelText = kid.data.Name;
                graph.AddNode(child);
                graph.AddEdge(parent.Id, child.Id);
                // Pewarnaan node children
                if (kid.colour == 1) graph.FindNode(child.Id).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Blue;
                else if (kid.colour == 0) graph.FindNode(child.Id).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                // Memroses Children secara rekursif
                createTree(graph, kid);

            }
            // Pewarnaan node parent
            if (tree.colour == 1) graph.FindNode(parent.Id).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Blue;
            else if (tree.colour == 0) graph.FindNode(parent.Id).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;

            return graph;
        }
    }
}
