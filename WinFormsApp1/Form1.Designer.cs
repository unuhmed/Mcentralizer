using System.Diagnostics;
using System.Xml.Linq;

/***********************
 * *********************
 *Auteur : Matheo Enfoux
 *Version : 1.0
 *Detail : Coeur de metier de l application recupere les fichiers raccourcis dans une list puis autorise le lancement des raccourcis regroupes*
 *Date de creation : 05/08/2023
 **********************
 **********************/

namespace WinFormsApp1
{
    partial class Form1
    {
        private string GetConf()
        {
            XDocument xConfig = XDocument.Load("./Config.xml");          
            return xConfig.Root.Value;
        }

        private void ListInitializer()
        {
            listView1.Name = "listView1";
            listView1.View = View.Details;
            listView1.Size = new Size(700, 700);
            listView1.Columns.Add("Programme");
            listView1.Columns[0].TextAlign = new HorizontalAlignment();
            listView1.Columns[0].Width = listView1.Width - 4-SystemInformation.HorizontalScrollBarThumbWidth;
         
            listView1.AutoArrange = true;
            listView1.Visible = true;
        }

        private ImageList SetIconsList()
        {
            String PathFolder = GetConf();
            string[] shortcuts = Directory.GetFiles(PathFolder, "*.lnk");
            ListInitializer();
            int index = 0;

            //getAllDataInFolder
            if (!Directory.Exists(PathFolder))
            {
                MessageBox.Show("Le dossier n'existe pas !");
                return null;
            }
            foreach (string shortcutPath in shortcuts)
            {
                // Obtenir l'icône du fichier raccourci
                Icon icon = Icon.ExtractAssociatedIcon(shortcutPath);
       
                // Ajouter l'icône à l'ImageList
                imageList1.ColorDepth = ColorDepth.Depth32Bit;
                imageList1.Images.Add(icon);
                listView1.Items.Add(Path.GetFileName(shortcutPath));
                index++;
            }

            imageList1.ImageSize = new Size(40, 40);
            listView1.SmallImageList = imageList1;
            return imageList1;
        }
        private ListView GetListView()
        {
            imageList1=SetIconsList();
            listView1.SmallImageList= imageList1;

            for (int i = 0; i < imageList1.Images.Count; i++)
            {
                listView1.Items[i].ImageIndex = i;


            }
            return listView1;
        }

        private void listViewShorcut_DoubleClick(object sender, EventArgs e)
        {
                if (listView1.SelectedItems.Count > 0)
                {
                    string selectedShortcut = listView1.SelectedItems[0].Text;

                    // Chemin complet du raccourci sélectionné
                    String PathFolder = Path.Combine(GetConf(),selectedShortcut);

                    // Check if the shorcust EXIST and get the TARGETPATH with window Script Object References
                    if (File.Exists(PathFolder))
                    {

                    IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                    IWshRuntimeLibrary.IWshShortcut link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(PathFolder);

                    // Start the process
                    Process.Start(link.TargetPath);
                    }
                    else
                    {
                        MessageBox.Show("Le raccourci n'existe pas !");
                    }
                }
        }
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
           
            SuspendLayout();
            resources.ApplyResources(this, "$this");

            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(GetListView());
            listView1.DoubleClick += listViewShorcut_DoubleClick;
            Name = "Mcentralizer";
            ResumeLayout(false);
            this.Icon = Icon.ExtractAssociatedIcon("app_icon.ico");
            this.ShowIcon = true;
            this.ShowInTaskbar = true;
           
        }


        private static ListView listView1=new ListView();
        private ListViewItem listItems;
        private static ImageList imageList1=new ImageList();
    }
}