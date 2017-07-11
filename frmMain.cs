using System;
using System.IO;
using System.Windows.Forms;

namespace directoryWatcher
{
    public partial class frmMain : Form
    {
        #region Declare

        private FileSystemWatcher fsw;
        private SaveFileDialog sfd = new SaveFileDialog();
        private FolderBrowserDialog fbd = new FolderBrowserDialog();
        private readonly ListViewColumnSorter lv_sorter = new ListViewColumnSorter();
        private int oldWidth;

        #endregion

        #region Init

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            listView.Columns.Add("Path", 350);
            listView.Columns.Add("Type", 100);
            listView.Columns.Add("Time", 150);
            listView.View = View.Details;
            listView.FullRowSelect = true;
            oldWidth = Width;
        }
        #endregion

        #region Event
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            string[] rows = { e.FullPath, e.ChangeType.ToString(), DateTime.Now.ToString() };
            ListViewItem lvi = new ListViewItem(rows);
            addItemToListView(listView, lvi);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            string[] rows = { e.OldFullPath, e.ChangeType.ToString(), DateTime.Now.ToString() };
            ListViewItem lvi = new ListViewItem(rows);
            addItemToListView(listView, lvi);
        }

        #endregion

        #region listView

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var lv = (ListView)sender;
            lv.ListViewItemSorter = lv_sorter;

            if (e.Column != lv_sorter.SortColumn)
            {
                lv_sorter.SortColumn = e.Column;
                lv_sorter.Order = SortOrder.Ascending;
            }
            else
                lv_sorter.Order = lv_sorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

            lv.Sort();
        }

        private delegate void addToListViewDelegate(ListView lv, ListViewItem lvi);
        private void addItemToListView(ListView lv, ListViewItem lvi)
        {
            if (lv.InvokeRequired)
            {
                Invoke(new addToListViewDelegate(addItemToListView), new object[] { lv, lvi });
                return;
            }
            lv.Items.Add(lvi);
        }
        #endregion

        #region Button
        private void btnWatch_Click(object sender, EventArgs e)
        {
            if (btnWatch.Text == "Start")
            {
                btnWatch.Text = "Stop";

                fsw = new FileSystemWatcher();

                fsw.Filter = "*.*";
                fsw.Path = txtPath.Text;
                fsw.IncludeSubdirectories = true;

                fsw.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                  | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                fsw.Changed += new FileSystemEventHandler(OnChanged);
                fsw.Created += new FileSystemEventHandler(OnChanged);
                fsw.Deleted += new FileSystemEventHandler(OnChanged);
                fsw.Renamed += new RenamedEventHandler(OnRenamed);
                fsw.EnableRaisingEvents = true;

            }
            else
            {
                fsw.EnableRaisingEvents = false;
                btnWatch.Text = "Start";
                fsw.Dispose();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = fbd.SelectedPath;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            sfd.InitialDirectory = Environment.CurrentDirectory;
            sfd.Filter = "Text Files (*.txt)|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (var tw = new StreamWriter(new FileStream(sfd.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite), System.Text.Encoding.Default))
                {
                    foreach (ListViewItem lvi in listView.Items)
                    {
                        tw.WriteLine(lvi.Text + ";" + lvi.SubItems[1].Text + ";" + lvi.SubItems[2].Text);
                    }
                }
            }
        }

        #endregion

        #region Resize

        private void frmMain_Resize(object sender, EventArgs e)
        {
            try
            {
                listView.Columns[0].Width -= oldWidth - Width;
                oldWidth = Width;
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}