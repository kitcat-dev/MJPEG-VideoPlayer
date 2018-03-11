﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using System.Xml;
using System.Threading;

namespace MacroscopPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string idEntrance = "2016897c-8be5-4a80-b1a3-7f79a9ec729c";
        string idCorridor = "6ea0cf2f-2bd6-4f14-aa12-46d643719727";
        string idRoad = "2410f79c-8f7e-4cd4-8baf-f7be29869a7e";
        string idParking = "e6f2848c-f361-44b9-bbec-1e54eae777c0";
        string projectPath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\";
        string configAddress = "http://demo.macroscop.com:8080/configex?login=root";


        public void MPJEGViewer(string id)
        {

        }
        
        public MainWindow()
        {
            InitializeComponent();

            RenderIDs(configAddress);
        }

        private void RenderIDs (string xmlAddress)
        {
            XmlDocument doc = MakeConfigRequest(configAddress);
            XmlElement root = doc.DocumentElement;

            int attrCount = 0;
            foreach (XmlNode node in root)
            {
                foreach (XmlNode node2 in node)
                {
                    attrCount++;
                    if (node2.Attributes.Count > 0)
                    {
                        XmlNode id = node2.Attributes.GetNamedItem("Id");
                        XmlNode name = node2.Attributes.GetNamedItem("Name");
                        if (id != null)
                        {
                            Logs.AppendText(id.Value + " — " + name.Value + "\n");
                        }
                    }
                }
            }
            doc.Save("cams.xml");
        }

        private XmlDocument MakeConfigRequest(string query)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(query);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(new StreamReader(response.GetResponseStream(), Encoding.UTF8));
                return doc;
            }
        }
        private void DrawBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= GetFilesInDirectory(projectPath + @"frames\").Length; i++)
            {
                Thread.Sleep(40); // 1000 ms / 25 frames per second

                BitmapImage b = new BitmapImage();

                b.BeginInit();
                b.UriSource = new Uri(projectPath + @"frames\frame" + i + ".jpg", UriKind.RelativeOrAbsolute);
                b.EndInit();

                Logs.AppendText("Rendered: " + Convert.ToString(b.UriSource) + "\n");
                VideoPlayer.Source = b; // почему поток тупит, не отрисовывает, а только по прошествии цикла рисует последнюю картинку?
            }
        }

        private FileInfo[] GetFilesInDirectory (string path)
        {
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(path);
            System.IO.DirectoryInfo[] dirs = info.GetDirectories();
            return info.GetFiles();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Frames.SaveFrames("no_url_yet", projectPath);
        }
    }


}