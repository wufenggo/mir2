using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryEditor
{
    public partial class LMain : Form
    {
        private readonly Dictionary<int, int> _indexList = new Dictionary<int, int>();
        private MLibraryV2 _library;
        private MLibraryV2.MImage _selectedImage, _exportImage;
        private Image _originalImage;

        protected bool ImageTabActive = true;
        protected bool FrameTabActive = false;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        public LMain()
        {
            InitializeComponent();

            this.FrameAction.ValueType = typeof(MirAction);
            this.FrameAction.DataSource = Enum.GetValues(typeof(MirAction));


            SendMessage(PreviewListView.Handle, 4149, 0, 5242946); //80 x 66

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);

            if (Program.openFileWith.Length > 0 && File.Exists(Program.openFileWith))
            {
                OpenLibrary(Program.openFileWith);
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (Path.GetExtension(files[0]).ToUpper() == ".WIL" ||
                Path.GetExtension(files[0]).ToUpper() == ".WZL" ||
                Path.GetExtension(files[0]).ToUpper() == ".MIZ")
            {
                try
                {
                    ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 8 };
                    Parallel.For(0, files.Length, options, i =>
                    {
                        if (Path.GetExtension(files[i]) == ".wtl")
                        {
                            WTLLibrary WTLlib = new WTLLibrary(files[i]);
                            WTLlib.ToMLibrary();
                        }
                        else
                        {
                            WeMadeLibrary WILlib = new WeMadeLibrary(files[i]);
                            WILlib.ToMLibrary();
                        }
                        toolStripProgressBar.Value++;
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                toolStripProgressBar.Value = 0;

                MessageBox.Show(
                    string.Format("Successfully converted {0} {1}",
                    (OpenWeMadeDialog.FileNames.Length).ToString(),
                    (OpenWeMadeDialog.FileNames.Length > 1) ? "libraries" : "library"));
            }
            else if (Path.GetExtension(files[0]).ToUpper() == ".LIB")
            {
                ClearInterface();
                ImageList.Images.Clear();
                PreviewListView.Items.Clear();
                _indexList.Clear();

                if (_library != null) _library.Close();
                _library = new MLibraryV2(files[0]);
                PreviewListView.VirtualListSize = _library.Images.Count;
                PreviewListView.RedrawItems(0, PreviewListView.Items.Count - 1, true);

                // Show .Lib path in application title.
                this.Text = files[0].ToString();
            }
            else
            {
                return;
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void ClearInterface()
        {
            _selectedImage = null;
            ImageBox.Image = null;
            ZoomTrackBar.Value = 1;

            WidthLabel.Text = "<No Image>";
            HeightLabel.Text = "<No Image>";
            OffSetXTextBox.Text = string.Empty;
            OffSetYTextBox.Text = string.Empty;
            OffSetXTextBox.BackColor = SystemColors.Window;
            OffSetYTextBox.BackColor = SystemColors.Window;
        }

        private void PreviewListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PreviewListView.SelectedIndices.Count == 0)
            {
                ClearInterface();
                return;
            }

            _selectedImage = _library.GetMImage(PreviewListView.SelectedIndices[0]);

            if (_selectedImage == null)
            {
                ClearInterface();
                return;
            }

            WidthLabel.Text = _selectedImage.Width.ToString();
            HeightLabel.Text = _selectedImage.Height.ToString();

            OffSetXTextBox.Text = _selectedImage.X.ToString();
            OffSetYTextBox.Text = _selectedImage.Y.ToString();

            ImageBox.Image = _selectedImage.Image;

            // Keep track of what image/s are selected.
            if (PreviewListView.SelectedIndices.Count > 1)
            {
                toolStripStatusLabel.ForeColor = Color.Red;
                toolStripStatusLabel.Text = "Multiple images selected.";
            }
            else
            {
                toolStripStatusLabel.ForeColor = SystemColors.ControlText;
                toolStripStatusLabel.Text = "Selected Image: " + string.Format("{0} / {1}",
                PreviewListView.SelectedIndices[0].ToString(),
                (PreviewListView.Items.Count - 1).ToString());
            }

            nudJump.Value = PreviewListView.SelectedIndices[0];
        }

        private void PreviewListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            int index;

            if (_indexList.TryGetValue(e.ItemIndex, out index))
            {
                e.Item = new ListViewItem { ImageIndex = index, Text = e.ItemIndex.ToString() };
                return;
            }

            _indexList.Add(e.ItemIndex, ImageList.Images.Count);
            ImageList.Images.Add(_library.GetPreview(e.ItemIndex));
            e.Item = new ListViewItem { ImageIndex = index, Text = e.ItemIndex.ToString() };
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;

            if (ImportImageDialog.ShowDialog() != DialogResult.OK) return;

            List<string> fileNames = new List<string>(ImportImageDialog.FileNames);

            //fileNames.Sort();
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = fileNames.Count;

            for (int i = 0; i < fileNames.Count; i++)
            {
                string fileName = fileNames[i];
                Bitmap image;

                try
                {
                    image = new Bitmap(fileName);
                }
                catch
                {
                    continue;
                }

                fileName = Path.Combine(Path.GetDirectoryName(fileName), "Placements", Path.GetFileNameWithoutExtension(fileName));
                fileName = Path.ChangeExtension(fileName, ".txt");

                short x = 0;
                short y = 0;

                if (File.Exists(fileName))
                {
                    string[] placements = File.ReadAllLines(fileName);

                    if (placements.Length > 0)
                        short.TryParse(placements[0], out x);
                    if (placements.Length > 1)
                        short.TryParse(placements[1], out y);
                }

                _library.AddImage(image, x, y);
                toolStripProgressBar.Value++;
                //image.Dispose();
            }

            PreviewListView.VirtualListSize = _library.Images.Count;
            toolStripProgressBar.Value = 0;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;

            if (_library != null) _library.Close();
            _library = new MLibraryV2(SaveLibraryDialog.FileName);
            PreviewListView.VirtualListSize = 0;
            _library.Save();

            UpdateFrameGridView();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenLibraryDialog.ShowDialog() != DialogResult.OK) return;

            OpenLibrary(OpenLibraryDialog.FileName);
        }

        private void OpenLibrary(string filename)
        {
            ClearInterface();
            ImageList.Images.Clear();
            PreviewListView.Items.Clear();
            _indexList.Clear();

            if (_library != null) _library.Close();
            _library = new MLibraryV2(filename);
            PreviewListView.VirtualListSize = _library.Images.Count;

            // Show .Lib path in application title.
            this.Text = filename;

            PreviewListView.SelectedIndices.Clear();

            if (PreviewListView.Items.Count > 0)
                PreviewListView.Items[0].Selected = true;

            UpdateFrameGridView();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;

            UpdateFrameGridData();

            _library.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;

            UpdateFrameGridData();

            _library.FileName = SaveLibraryDialog.FileName;
            _library.Save();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;

            if (MessageBox.Show("Are you sure you want to delete the selected Image?",
                "Delete Selected.",
                MessageBoxButtons.YesNoCancel) != DialogResult.Yes) return;

            List<int> removeList = new List<int>();

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
                removeList.Add(PreviewListView.SelectedIndices[i]);

            removeList.Sort();

            for (int i = removeList.Count - 1; i >= 0; i--)
                _library.RemoveImage(removeList[i]);

            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize -= removeList.Count;
        }

        private void convertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenWeMadeDialog.ShowDialog() != DialogResult.OK) return;

            toolStripProgressBar.Maximum = OpenWeMadeDialog.FileNames.Length;
            toolStripProgressBar.Value = 0;

            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 8 };
                Parallel.For(0, OpenWeMadeDialog.FileNames.Length, options, i =>
                            {
                                if (Path.GetExtension(OpenWeMadeDialog.FileNames[i]) == ".wtl")
                                {
                                    WTLLibrary WTLlib = new WTLLibrary(OpenWeMadeDialog.FileNames[i]);
                                    WTLlib.ToMLibrary();
                                }
                                else if (Path.GetExtension(OpenWeMadeDialog.FileNames[i]) == ".Lib")
                                {
                                    MLibraryV1 v1Lib = new MLibraryV1(OpenWeMadeDialog.FileNames[i]);
                                    v1Lib.ToMLibrary();
                                }
                                else
                                {
                                    WeMadeLibrary WILlib = new WeMadeLibrary(OpenWeMadeDialog.FileNames[i]);
                                    WILlib.ToMLibrary();
                                }
                                toolStripProgressBar.Value++;
                            });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            toolStripProgressBar.Value = 0;

            MessageBox.Show(string.Format("Successfully converted {0} {1}",
                (OpenWeMadeDialog.FileNames.Length).ToString(),
                (OpenWeMadeDialog.FileNames.Length > 1) ? "libraries" : "library"));
        }

        private void copyToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PreviewListView.SelectedIndices.Count == 0) return;
            if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;

            MLibraryV2 tempLibrary = new MLibraryV2(SaveLibraryDialog.FileName);

            List<int> copyList = new List<int>();

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
                copyList.Add(PreviewListView.SelectedIndices[i]);

            copyList.Sort();

            for (int i = 0; i < copyList.Count; i++)
            {
                MLibraryV2.MImage image = _library.GetMImage(copyList[i]);
                tempLibrary.AddImage(image.Image, image.MaskImage, image.X, image.Y);
            }

            tempLibrary.Save();
        }

        private void removeBlanksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove the blank images?",
                "Remove Blanks",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            _library.RemoveBlanks();
            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize = _library.Count;
        }

        private void countBlanksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLibraryDialog.Multiselect = true;

            if (OpenLibraryDialog.ShowDialog() != DialogResult.OK)
            {
                OpenLibraryDialog.Multiselect = false;
                return;
            }

            OpenLibraryDialog.Multiselect = false;

            MLibraryV2.Load = false;

            int count = 0;

            for (int i = 0; i < OpenLibraryDialog.FileNames.Length; i++)
            {
                MLibraryV2 library = new MLibraryV2(OpenLibraryDialog.FileNames[i]);

                for (int x = 0; x < library.Count; x++)
                {
                    if (library.Images[x].Length <= 8)
                        count++;
                }

                library.Close();
            }

            MLibraryV2.Load = true;
            MessageBox.Show(count.ToString());
        }

        private void OffSetXTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;

            if (control == null || !control.Focused) return;

            short temp;

            if (!short.TryParse(control.Text, out temp))
            {
                control.BackColor = Color.Red;
                return;
            }

            control.BackColor = SystemColors.Window;

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
            {
                MLibraryV2.MImage image = _library.GetMImage(PreviewListView.SelectedIndices[i]);
                image.X = temp;
            }
        }

        private void OffSetYTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;

            if (control == null || !control.Focused) return;

            short temp;

            if (!short.TryParse(control.Text, out temp))
            {
                control.BackColor = Color.Red;
                return;
            }

            control.BackColor = SystemColors.Window;

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
            {
                MLibraryV2.MImage image = _library.GetMImage(PreviewListView.SelectedIndices[i]);
                image.Y = temp;
            }
        }

        private void InsertImageButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;
            if (ImportImageDialog.ShowDialog() != DialogResult.OK) return;

            List<string> fileNames = new List<string>(ImportImageDialog.FileNames);

            //fileNames.Sort();

            int index = PreviewListView.SelectedIndices[0];

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = fileNames.Count;

            for (int i = fileNames.Count - 1; i >= 0; i--)
            {
                string fileName = fileNames[i];

                Bitmap image;

                try
                {
                    image = new Bitmap(fileName);
                }
                catch
                {
                    continue;
                }

                fileName = Path.Combine(Path.GetDirectoryName(fileName), "Placements", Path.GetFileNameWithoutExtension(fileName));
                fileName = Path.ChangeExtension(fileName, ".txt");

                short x = 0;
                short y = 0;

                if (File.Exists(fileName))
                {
                    string[] placements = File.ReadAllLines(fileName);

                    if (placements.Length > 0)
                        short.TryParse(placements[0], out x);
                    if (placements.Length > 1)
                        short.TryParse(placements[1], out y);
                }

                _library.InsertImage(index, image, x, y);

                toolStripProgressBar.Value++;
            }

            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize = _library.Images.Count;
            toolStripProgressBar.Value = 0;
            _library.Save();
        }

        private void safeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove the blank images?",
                "Remove Blanks", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            _library.RemoveBlanks(true);
            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize = _library.Count;
        }

        private const int HowDeepToScan = 6;

        public static void ProcessDir(string sourceDir, int recursionLvl, string outputDir)
        {
            if (recursionLvl <= HowDeepToScan)
            {
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(sourceDir);
                foreach (string fileName in fileEntries)
                {
                    if (Directory.Exists(outputDir) != true) Directory.CreateDirectory(outputDir);
                    MLibraryV0 OldLibrary = new MLibraryV0(fileName);
                    MLibraryV2 NewLibrary = new MLibraryV2(outputDir + Path.GetFileName(fileName)) { Images = new List<MLibraryV2.MImage>(), IndexList = new List<int>(), Count = OldLibrary.Images.Count }; ;
                    for (int i = 0; i < OldLibrary.Images.Count; i++)
                        NewLibrary.Images.Add(null);
                    for (int j = 0; j < OldLibrary.Images.Count; j++)
                    {
                        MLibraryV0.MImage oldimage = OldLibrary.GetMImage(j);
                        NewLibrary.Images[j] = new MLibraryV2.MImage(oldimage.FBytes, oldimage.Width, oldimage.Height) { X = oldimage.X, Y = oldimage.Y };
                    }
                    NewLibrary.Save();
                    for (int i = 0; i < NewLibrary.Images.Count; i++)
                    {
                        if (NewLibrary.Images[i].Preview != null)
                            NewLibrary.Images[i].Preview.Dispose();
                        if (NewLibrary.Images[i].Image != null)
                            NewLibrary.Images[i].Image.Dispose();
                        if (NewLibrary.Images[i].MaskImage != null)
                            NewLibrary.Images[i].MaskImage.Dispose();
                    }
                    for (int i = 0; i < OldLibrary.Images.Count; i++)
                    {
                        if (OldLibrary.Images[i].Preview != null)
                            OldLibrary.Images[i].Preview.Dispose();
                        if (OldLibrary.Images[i].Image != null)
                            OldLibrary.Images[i].Image.Dispose();
                    }
                    NewLibrary.Images.Clear();
                    NewLibrary.IndexList.Clear();
                    OldLibrary.Images.Clear();
                    OldLibrary.IndexList.Clear();
                    NewLibrary.Close();
                    OldLibrary.Close();
                    NewLibrary = null;
                    OldLibrary = null;
                }

                // Recurse into subdirectories of this directory.
                string[] subdirEntries = Directory.GetDirectories(sourceDir);
                foreach (string subdir in subdirEntries)
                {
                    // Do not iterate through re-parse points.
                    if (Path.GetFileName(Path.GetFullPath(subdir).TrimEnd(Path.DirectorySeparatorChar)) == Path.GetFileName(Path.GetFullPath(outputDir).TrimEnd(Path.DirectorySeparatorChar))) continue;
                    if ((File.GetAttributes(subdir) &
                         FileAttributes.ReparsePoint) !=
                             FileAttributes.ReparsePoint)
                        ProcessDir(subdir, recursionLvl + 1, outputDir + " \\" + Path.GetFileName(Path.GetFullPath(subdir).TrimEnd(Path.DirectorySeparatorChar)) + "\\");
                }
            }
        }

        // Export a single image.
        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;

            string _fileName = Path.GetFileName(OpenLibraryDialog.FileName);
            string _newName = _fileName.Remove(_fileName.IndexOf('.'));
            string _folder = Application.StartupPath + "\\Exported\\" + _newName + "\\";

            Bitmap blank = new Bitmap(1, 1);

            // Create the folder if it doesn't exist.
            (new FileInfo(_folder)).Directory.Create();

            ListView.SelectedIndexCollection _col = PreviewListView.SelectedIndices;

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = _col.Count;

            for (int i = _col[0]; i < (_col[0] + _col.Count); i++)
            {
                _exportImage = _library.GetMImage(i);
                if (_exportImage.Image == null)
                {
                    blank.Save(_folder + i.ToString() + ".bmp", ImageFormat.Bmp);
                }
                else
                {
                    _exportImage.Image.Save(_folder + i.ToString() + ".bmp", ImageFormat.Bmp);
                }

                toolStripProgressBar.Value++;

                if (!Directory.Exists(_folder + "/Placements/"))
                    Directory.CreateDirectory(_folder + "/Placements/");

                File.WriteAllLines(_folder + "/Placements/" + i.ToString() + ".txt", new string[] { _exportImage.X.ToString(), _exportImage.Y.ToString() });
            }

            toolStripProgressBar.Value = 0;
            MessageBox.Show("Saving to " + _folder + "...", "Image Saved", MessageBoxButtons.OK);
        }

        // Don't let the splitter go out of sight on resizing.
        private void LMain_Resize(object sender, EventArgs e)
        {
            if (splitContainer1.SplitterDistance <= this.Height - 150) return;
            if (this.Height - 150 > 0)
            {
                splitContainer1.SplitterDistance = this.Height - 150;
            }
        }

        // Resize the image(Zoom).
        private Image ImageBoxZoom(Image image, Size size)
        {
            _originalImage = _selectedImage.Image;
            Bitmap _bmp = new Bitmap(_originalImage, Convert.ToInt32(_originalImage.Width * size.Width), Convert.ToInt32(_originalImage.Height * size.Height));
            Graphics _gfx = Graphics.FromImage(_bmp);
            return _bmp;
        }

        // Zoom in and out.
        private void ZoomTrackBar_Scroll(object sender, EventArgs e)
        {
            if (ImageBox.Image == null)
            {
                ZoomTrackBar.Value = 1;
            }
            if (ZoomTrackBar.Value > 0)
            {
                try
                {
                    PreviewListView.Items[(int)nudJump.Value].EnsureVisible();

                    Bitmap _newBMP = new Bitmap(_selectedImage.Width * ZoomTrackBar.Value, _selectedImage.Height * ZoomTrackBar.Value);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_newBMP))
                    {
                        if (checkBoxPreventAntiAliasing.Checked == true)
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingMode = CompositingMode.SourceCopy;
                        }

                        if (checkBoxQuality.Checked == true)
                        {
                            g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        }

                        g.DrawImage(_selectedImage.Image, new Rectangle(0, 0, _newBMP.Width, _newBMP.Height));
                    }
                    ImageBox.Image = _newBMP;

                    toolStripStatusLabel.ForeColor = SystemColors.ControlText;
                    toolStripStatusLabel.Text = "Selected Image: " + string.Format("{0} / {1}",
                        PreviewListView.SelectedIndices[0].ToString(),
                        (PreviewListView.Items.Count - 1).ToString());
                }
                catch
                {
                    return;
                }
            }
        }

        // Swap the image panel background colour Black/White.
        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (panel.BackColor == Color.Black)
            {
                panel.BackColor = Color.GhostWhite;
            }
            else
            {
                panel.BackColor = Color.Black;
            }
        }

        private void PreviewListView_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
            // Keep track of what image/s are selected.
            ListView.SelectedIndexCollection _col = PreviewListView.SelectedIndices;

            if (_col.Count > 1)
            {
                toolStripStatusLabel.ForeColor = Color.Red;
                toolStripStatusLabel.Text = "Multiple images selected.";
            }
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();

            if (ofd.FileName == "") return;

            Bitmap newBmp = new Bitmap(ofd.FileName);

            ImageList.Images.Clear();
            _indexList.Clear();
            _library.ReplaceImage(PreviewListView.SelectedIndices[0], newBmp, 0, 0);
            PreviewListView.VirtualListSize = _library.Images.Count;

            try
            {
                PreviewListView.RedrawItems(0, PreviewListView.Items.Count - 1, true);
                ImageBox.Image = _library.Images[PreviewListView.SelectedIndices[0]].Image;
            }
            catch (Exception)
            {
                return;
            }
        }

        private void previousImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (PreviewListView.Visible && PreviewListView.Items.Count > 0)
                {
                    int index = PreviewListView.SelectedIndices[0];
                    index = index - 1;
                    PreviewListView.SelectedIndices.Clear();
                    this.PreviewListView.Items[index].Selected = true;
                    PreviewListView.Items[index].EnsureVisible();

                    if (_selectedImage.Height == 1 && _selectedImage.Width == 1 && PreviewListView.SelectedIndices[0] != 0)
                    {
                        previousImageToolStripMenuItem_Click(null, null);
                    }
                }
            }
            catch (Exception)
            {
                PreviewListView.SelectedIndices.Clear();
                this.PreviewListView.Items[PreviewListView.Items.Count - 1].Selected = true;
            }
        }

        private void nextImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (PreviewListView.Visible && PreviewListView.Items.Count > 0)
                {
                    int index = PreviewListView.SelectedIndices[0];
                    index = index + 1;
                    PreviewListView.SelectedIndices.Clear();
                    this.PreviewListView.Items[index].Selected = true;
                    PreviewListView.Items[index].EnsureVisible();

                    if (_selectedImage.Height == 1 && _selectedImage.Width == 1 && PreviewListView.SelectedIndices[0] != 0)
                    {
                        nextImageToolStripMenuItem_Click(null, null);
                    }
                }
            }
            catch (Exception)
            {
                PreviewListView.SelectedIndices.Clear();
                this.PreviewListView.Items[0].Selected = true;
            }
        }

        // Move Left and Right through images.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!ImageTabActive) return false;

            if (keyData == Keys.Left)
            {
                previousImageToolStripMenuItem_Click(null, null);
                return true;
            }

            if (keyData == Keys.Right)
            {
                nextImageToolStripMenuItem_Click(null, null);
                return true;
            }

            if (keyData == Keys.Up) //Not 100% accurate but works for now.
            {
                double d = Math.Floor((double)(PreviewListView.Width / 67));
                int index = PreviewListView.SelectedIndices[0] - (int)d;

                PreviewListView.SelectedIndices.Clear();
                if (index < 0)
                    index = 0;

                this.PreviewListView.Items[index].Selected = true;

                return true;
            }

            if (keyData == Keys.Down) //Not 100% accurate but works for now.
            {
                double d = Math.Floor((double)(PreviewListView.Width / 67));
                int index = PreviewListView.SelectedIndices[0] + (int)d;

                PreviewListView.SelectedIndices.Clear();
                if (index > PreviewListView.Items.Count - 1)
                    index = PreviewListView.Items.Count - 1;

                this.PreviewListView.Items[index].Selected = true;

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void buttonSkipNext_Click(object sender, EventArgs e)
        {
            nextImageToolStripMenuItem_Click(null, null);
        }

        private void buttonSkipPrevious_Click(object sender, EventArgs e)
        {
            previousImageToolStripMenuItem_Click(null, null);
        }

        private void checkBoxQuality_CheckedChanged(object sender, EventArgs e)
        {
            ZoomTrackBar_Scroll(null, null);
        }

        private void checkBoxPreventAntiAliasing_CheckedChanged(object sender, EventArgs e)
        {
            ZoomTrackBar_Scroll(null, null);
        }

        private void nudJump_ValueChanged(object sender, EventArgs e)
        {
            if (PreviewListView.Items.Count - 1 >= nudJump.Value)
            {
                PreviewListView.SelectedIndices.Clear();
                PreviewListView.Items[(int)nudJump.Value].Selected = true;
                PreviewListView.Items[(int)nudJump.Value].EnsureVisible();
            }
        }

        private void nudJump_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //Enter key is down.
                if (PreviewListView.Items.Count - 1 >= nudJump.Value)
                {
                    PreviewListView.SelectedIndices.Clear();
                    PreviewListView.Items[(int)nudJump.Value].Selected = true;
                    PreviewListView.Items[(int)nudJump.Value].EnsureVisible();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        #region Frames

        private void defaultMonsterFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _library.Frames.Clear();
            _library.Frames = new FrameSet(FrameSet.DefaultMonsterFrameSet);

            UpdateFrameGridView();
        }

        private void defaultNPCFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _library.Frames.Clear();
            _library.Frames = new FrameSet(FrameSet.DefaultNPCFrameSet);

            UpdateFrameGridView();
        }

        private void defaultPlayerFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0: //Images
                    ImageTabActive = true;
                    FrameTabActive = false;
                    break;
                case 1: //Frames
                    ImageTabActive = false;
                    FrameTabActive = true;
                    break;
            }
        }

        private void autofillNpcFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FolderLibraryDialog.ShowDialog() != DialogResult.OK) return;

            var path = FolderLibraryDialog.SelectedPath;

            var files = Directory.GetFiles(path, "*.Lib");

            if (MessageBox.Show($"Are you sure you want to populate {files.Count()} Libs with their matching FrameSet?",
                "Autofill Libs.",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            foreach (var file in files)
            {
                if (_library != null) _library.Close();
                _library = new MLibraryV2(file);

                // Show .Lib path in application title.
                this.Text = file;

                var name = Path.GetFileNameWithoutExtension(file);

                if (!int.TryParse(name, out int imageNumber)) continue;

                _library.Frames = GetFrameSetByImage((Monster)imageNumber);
                _library.Save();
            }
        }

        private void frameGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            frameGridView.Rows[e.RowIndex].ErrorText = "";

            if (frameGridView.Rows[e.RowIndex].IsNewRow) { return; }

            if (e.ColumnIndex >= 1 && e.ColumnIndex <= 8)
            {
                if (!int.TryParse(e.FormattedValue.ToString(), out _))
                {
                    e.Cancel = true;
                    frameGridView.Rows[e.RowIndex].ErrorText = "the value must be an integer";
                }
            }
        }

        private void frameGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["FrameStart"].Value = 0;
            e.Row.Cells["FrameCount"].Value = 0;
            e.Row.Cells["FrameSkip"].Value = 0;
            e.Row.Cells["FrameInterval"].Value = 0;
            e.Row.Cells["FrameEffectStart"].Value = 0;
            e.Row.Cells["FrameEffectCount"].Value = 0;
            e.Row.Cells["FrameEffectSkip"].Value = 0;
            e.Row.Cells["FrameEffectInterval"].Value = 0;
            e.Row.Cells["FrameReverse"].Value = false;
            e.Row.Cells["FrameBlend"].Value = false;
        }


        private void UpdateFrameGridView()
        {
            frameGridView.Rows.Clear();

            foreach (var action in _library.Frames.Keys)
            {
                var frame = _library.Frames[action];

                int rowIndex = frameGridView.Rows.Add();

                var row = frameGridView.Rows[rowIndex];

                row.Cells["FrameAction"].Value = action;
                row.Cells["FrameStart"].Value = frame.Start;
                row.Cells["FrameCount"].Value = frame.Count;
                row.Cells["FrameSkip"].Value = frame.Skip;
                row.Cells["FrameInterval"].Value = frame.Interval;
                row.Cells["FrameEffectStart"].Value = frame.EffectStart;
                row.Cells["FrameEffectCount"].Value = frame.EffectCount;
                row.Cells["FrameEffectSkip"].Value = frame.EffectSkip;
                row.Cells["FrameEffectInterval"].Value = frame.EffectInterval;
                row.Cells["FrameReverse"].Value = frame.Reverse;
                row.Cells["FrameBlend"].Value = frame.Blend;
            }
        }

        private void UpdateFrameGridData()
        {
            if (_library == null) return;

            _library.Frames.Clear();

            foreach (DataGridViewRow row in frameGridView.Rows)
            {
                var cells = row.Cells;

                if (cells["FrameAction"].Value == null) continue;

                var action = (MirAction)row.Cells["FrameAction"].Value;

                if (_library.Frames.ContainsKey(action))
                {
                    MessageBox.Show(string.Format($"The action '{action}' exists more than once so will not be saved."));
                    continue;
                }

                var frame = new Frame(cells["FrameStart"].Value.ValueOrDefault<int>(),
                                        cells["FrameCount"].Value.ValueOrDefault<int>(),
                                        cells["FrameSkip"].Value.ValueOrDefault<int>(),
                                        cells["FrameInterval"].Value.ValueOrDefault<int>(),
                                        cells["FrameEffectStart"].Value.ValueOrDefault<int>(),
                                        cells["FrameEffectCount"].Value.ValueOrDefault<int>(),
                                        cells["FrameEffectSkip"].Value.ValueOrDefault<int>(),
                                        cells["FrameEffectInterval"].Value.ValueOrDefault<int>())
                {
                    Reverse = cells["FrameReverse"].Value.ValueOrDefault<bool>(),
                    Blend = cells["FrameBlend"].Value.ValueOrDefault<bool>()
                };

                _library.Frames.Add(action, frame);
            }
        }

        /// <summary>
        /// List of monsters and matching frames
        /// Method MUST be edited before use. The existing code is only here as an example.
        /// READ THE COMMENTS WITHIN THIS METHOD BEFORE USE
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private FrameSet GetFrameSetByImage(Monster image)
        {
            //REMOVE THE BELOW EXCEPTION ONCE THE DESIRED CODE HAS BEEN ADDED
            //throw new NotImplementedException("The method 'GetFrameSetByImage' must be updated before this function can be used");

            //  UNCOMMENT THE CODE BELOW, IT SERVES AS AN EXAMPLE OF HOW TO MATCH IMAGES UP TO THE CORRECT FRAMES
            List<FrameSet> FrameList = new List<FrameSet>();
            FrameSet frame;

            //ADD LIST OF FRAMES (CAN BE COPIED FROM THE CLIENTS FRAME.CS)
            #region Monster Frames

            //0 - Guard, Guard2
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));

            //1 - Hen, Deer, Sheep, Wolf, Pig, Bull, DarkBrownWolf
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Skeleton, new Frame(224, 1, 0, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //2 - Regular
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //3 - CannibalPlant 食人花
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, -4, 500));
            frame.Add(MirAction.Show, new Frame(4, 8, -8, 200));
            frame.Add(MirAction.Hide, new Frame(12, 8, -8, 200) { Reverse = true });
            frame.Add(MirAction.Attack1, new Frame(20, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(68, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(84, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(93, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(84, 10, 0, 100) { Reverse = true });

            //4 - ForestYeti, CaveMaggot, FrostYeti
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 4, 0, 100));
            frame.Add(MirAction.Dead, new Frame(147, 1, 3, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 4, 0, 100) { Reverse = true });

            //5 - Scorpion
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(128, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(176, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(192, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(201, 1, 9, 1000));

            //6 - ChestnutTree, EbonyTree, LargeMushroom, CherryTree, ChristmasTree, SnowTree
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, -10, 500));
            frame.Add(MirAction.Struck, new Frame(10, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(12, 10, -10, 100));
            frame.Add(MirAction.Dead, new Frame(21, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(12, 10, -10, 100) { Reverse = true });

            //7 - EvilCentipede
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, -4, 1000));
            frame.Add(MirAction.Show, new Frame(22, 10, -10, 150));
            frame.Add(MirAction.Hide, new Frame(31, 10, -10, 150) { Reverse = true });
            frame.Add(MirAction.Attack1, new Frame(4, 6, -6, 100));
            frame.Add(MirAction.Struck, new Frame(10, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(12, 10, -10, 150));
            frame.Add(MirAction.Dead, new Frame(21, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(12, 10, -10, 150) { Reverse = true });

            //8 - BugBatMaggot
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, -4, 1000));
            frame.Add(MirAction.Attack1, new Frame(4, 6, -6, 100));
            frame.Add(MirAction.Struck, new Frame(10, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(12, 10, -10, 150));
            frame.Add(MirAction.Dead, new Frame(21, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(12, 10, -10, 150) { Reverse = true });

            //9 - CrystalSpider, WhiteFoxman, LightTurtle, CrystalWeaver
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Attack2, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //10 - RedMoonEvil
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, -4, 1000));
            frame.Add(MirAction.Attack1, new Frame(4, 6, -6, 100));
            frame.Add(MirAction.Struck, new Frame(10, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(12, 20, -20, 100));
            frame.Add(MirAction.Dead, new Frame(31, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(12, 20, -20, 100) { Reverse = true });

            //11 - ZumaStatue, ZumaGuardian, FrozenZumaStatue, FrozenZumaGuardian
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Stoned, new Frame(0, 1, 5, 100));
            frame.Add(MirAction.Show, new Frame(0, 6, 0, 100));
            frame.Add(MirAction.Hide, new Frame(5, 6, 0, 100) { Reverse = true });
            frame.Add(MirAction.Standing, new Frame(48, 4, 0, 1000));
            frame.Add(MirAction.Walking, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(128, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(176, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(192, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(201, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(192, 10, 0, 100) { Reverse = true });

            //12 - ZumaTaurus
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Stoned, new Frame(0, 1, -1, 100));
            frame.Add(MirAction.Show, new Frame(0, 20, -20, 100));
            frame.Add(MirAction.Standing, new Frame(20, 4, 0, 1000));
            frame.Add(MirAction.Walking, new Frame(52, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(100, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(148, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(164, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(173, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(164, 10, 0, 100) { Reverse = true });

            //13 - RedThunderZuma, FrozenRedZuma
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Stoned, new Frame(272, 1, 5, 100));
            frame.Add(MirAction.Show, new Frame(272, 6, 0, 100));
            frame.Add(MirAction.Hide, new Frame(277, 6, 0, 100) { Reverse = true });
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //14 - KingScorpion, DarkDevil, RightGuard, LeftGuard, MinotaurKing
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //15 - BoneFamilar
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Appear, new Frame(224, 10, -10, 100) { Blend = true });
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //16 - Shinsu
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Appear, new Frame(0, 10, 0, 100));
            frame.Add(MirAction.Standing, new Frame(80, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(112, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(160, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 10, 0, 100));
            frame.Add(MirAction.Show, new Frame(256, 10, 0, 100));
            frame.Add(MirAction.Hide, new Frame(265, 10, 0, 100) { Reverse = true });
            frame.Add(MirAction.Revive, new Frame(176, 10, 0, 100) { Reverse = true });

            //17 - DigOutZombie
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Show, new Frame(224, 10, 0, 200));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //18 - ClZombie, NdZombie, CrawlerZombie
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(224, 10, 0, 100));

            //19 - ShamanZombie
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //20 - Khazard, FinialTurtle
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //21 - BoneLord
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(128, 6, 0, 200));
            frame.Add(MirAction.AttackRange1, new Frame(176, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(224, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(240, 20, 0, 150));
            frame.Add(MirAction.Dead, new Frame(259, 1, 19, 1000));
            frame.Add(MirAction.Revive, new Frame(240, 20, 0, 150) { Reverse = true });

            //22 - FrostTiger, FlameTiger
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });
            frame.Add(MirAction.SitDown, new Frame(272, 4, 0, 500));

            //23 Yimoogi, RedYimoogi, Snake10-17
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //24 - HolyDeva
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Appear, new Frame(216, 10, -10, 100) { Blend = true });
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500) { Blend = true });
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100) { Blend = true });
            frame.Add(MirAction.AttackRange1, new Frame(80, 6, 0, 100) { Blend = true });
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200) { Blend = true });
            frame.Add(MirAction.Die, new Frame(144, 9, 0, 100) { Blend = true });
            frame.Add(MirAction.Revive, new Frame(144, 9, 0, 100) { Blend = true, Reverse = true });

            //25 - GreaterWeaver, RootSpider
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 18, 1000));
            frame.Add(MirAction.Attack1, new Frame(4, 6, 16, 100));
            frame.Add(MirAction.Struck, new Frame(10, 2, 20, 200));
            frame.Add(MirAction.Die, new Frame(12, 10, 12, 150));
            frame.Add(MirAction.Dead, new Frame(21, 1, 21, 1000));
            frame.Add(MirAction.Revive, new Frame(12, 10, 12, 150) { Reverse = true });

            //26 - BombSpider, MutatedHugger
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 1, 5, 1000));
            frame.Add(MirAction.Walking, new Frame(0, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(0, 1, 5, 200));
            frame.Add(MirAction.Die, new Frame(48, 10, 0, 150));
            frame.Add(MirAction.Dead, new Frame(57, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(48, 10, 0, 150) { Reverse = true });

            //27 - CrossbowOma, DarkCrossbowOma
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 1000));
            frame.Add(MirAction.Walking, new Frame(32, 6, 1, 100));
            frame.Add(MirAction.AttackRange1, new Frame(88, 6, 1, 100));
            frame.Add(MirAction.Struck, new Frame(144, 1, 0, 200));
            frame.Add(MirAction.Die, new Frame(160, 10, 0, 150));
            frame.Add(MirAction.Dead, new Frame(169, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(160, 10, 0, 150) { Reverse = true });

            //28 - YinDevilNode, YangDevilNode
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, -4, 1000));
            frame.Add(MirAction.Attack1, new Frame(4, 6, -6, 180));
            frame.Add(MirAction.Struck, new Frame(10, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(12, 10, -10, 150));
            frame.Add(MirAction.Dead, new Frame(21, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(12, 10, -10, 150) { Reverse = true });

            //29 - OmaKing
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 1000));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(464, 20, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 20, 0, 150));
            frame.Add(MirAction.Dead, new Frame(163, 1, 19, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 20, 0, 150) { Reverse = true });

            //30 - BlackFoxman, RedFoxman
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //31 - TrapRock
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, -4, 500));
            frame.Add(MirAction.Show, new Frame(4, 5, -5, 200));
            frame.Add(MirAction.Attack1, new Frame(9, 5, -5, 100));
            frame.Add(MirAction.Struck, new Frame(14, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(16, 10, -10, 100));
            frame.Add(MirAction.Dead, new Frame(25, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(16, 10, -10, 100) { Reverse = true });

            //32 - GuardianRock
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, -4, 500));
            frame.Add(MirAction.Attack1, new Frame(4, 4, -4, 200));

            //33 - ThunderElement, CloudElement
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, -10, 800));
            frame.Add(MirAction.Walking, new Frame(10, 10, -10, 80));
            frame.Add(MirAction.Attack1, new Frame(20, 10, -10, 80));
            frame.Add(MirAction.Struck, new Frame(30, 4, -4, 200));
            frame.Add(MirAction.Die, new Frame(34, 10, -10, 120));
            frame.Add(MirAction.Dead, new Frame(43, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(34, 10, -10, 150) { Reverse = true });

            //34 - GreatFoxSpirit level 0
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 20, -20, 100));
            frame.Add(MirAction.Attack1, new Frame(22, 8, -8, 120));
            frame.Add(MirAction.Struck, new Frame(20, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(300, 18, -18, 120));
            frame.Add(MirAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //35 - GreatFoxSpirit level 1
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(60, 20, -20, 100));
            frame.Add(MirAction.Attack1, new Frame(82, 8, -8, 120));
            frame.Add(MirAction.Struck, new Frame(80, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(300, 18, -18, 120));
            frame.Add(MirAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //36 - GreatFoxSpirit level 2
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(120, 20, -20, 100));
            frame.Add(MirAction.Attack1, new Frame(142, 8, -8, 120));
            frame.Add(MirAction.Struck, new Frame(140, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(300, 18, -18, 120));
            frame.Add(MirAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //37 - GreatFoxSpirit level 3
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(180, 20, -20, 100));
            frame.Add(MirAction.Attack1, new Frame(202, 8, -8, 120));
            frame.Add(MirAction.Struck, new Frame(200, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(300, 18, -18, 120));
            frame.Add(MirAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //38 - GreatFoxSpirit level 4
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(240, 20, -20, 100));
            frame.Add(MirAction.Attack1, new Frame(262, 8, -8, 120));
            frame.Add(MirAction.Struck, new Frame(260, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(300, 18, -18, 120));
            frame.Add(MirAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //39 - HedgeKekTal, BigHedgeKekTal
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 4, 100));
            frame.Add(MirAction.Attack1, new Frame(112, 6, 4, 100));
            frame.Add(MirAction.Struck, new Frame(192, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(208, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(217, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(208, 10, 0, 100) { Reverse = true });
            frame.Add(MirAction.AttackRange1, new Frame(288, 6, 0, 100));

            //40 - EvilMir
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, -10, 1000));
            frame.Add(MirAction.Attack1, new Frame(42, 8, -8, 120));
            frame.Add(MirAction.AttackRange1, new Frame(10, 6, 4, 120));
            frame.Add(MirAction.Struck, new Frame(40, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(42, 7, -7, 120));
            frame.Add(MirAction.Dead, new Frame(48, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(42, 7, -7, 120) { Reverse = true });

            //41 - DragonStatue 1
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(300, 1, -1, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(300, 1, -1, 120));
            frame.Add(MirAction.Struck, new Frame(300, 1, -1, 200));

            //42 - DragonStatue 2
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(301, 1, -1, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(301, 1, -1, 120));
            frame.Add(MirAction.Struck, new Frame(301, 1, -1, 200));

            //43 - DragonStatue 3
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(302, 1, -1, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(302, 1, -1, 120));
            frame.Add(MirAction.Struck, new Frame(302, 1, -1, 200));

            //44 - DragonStatue 4
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(320, 1, -1, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(320, 1, -1, 120));
            frame.Add(MirAction.Struck, new Frame(320, 1, -1, 200));

            //45 - DragonStatue 5
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(321, 1, -1, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(321, 1, -1, 120));
            frame.Add(MirAction.Struck, new Frame(321, 1, -1, 200));

            //46 - DragonStatue 6
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(322, 1, -1, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(322, 1, -1, 120));
            frame.Add(MirAction.Struck, new Frame(322, 1, -1, 200));

            //47 - ArcherGuard
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 3, 100));
            frame.Add(MirAction.Attack1, new Frame(104, 6, 3, 100));
            frame.Add(MirAction.Struck, new Frame(176, 2, 0, 100));
            frame.Add(MirAction.Die, new Frame(192, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(201, 1, 9, 1000));

            //48 - TaoistGuard
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Attack1, new Frame(32, 6, 0, 100));

            //49 - VampireSpider (Archer SummonVampire)
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Show, new Frame(24, 6, 0, 150));
            frame.Add(MirAction.Hide, new Frame(29, 6, 0, 150) { Reverse = true });
            frame.Add(MirAction.Standing, new Frame(72, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(104, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(152, 5, 0, 100));
            frame.Add(MirAction.Struck, new Frame(192, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(216, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(225, 1, 9, 1000));

            //50 - SpittingToad (Archer SummonToad)
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.AttackRange1, new Frame(32, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(104, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(128, 10, 0, 100));
            frame.Add(MirAction.Hide, new Frame(208, 4, 0, 100));
            frame.Add(MirAction.Show, new Frame(211, 4, 0, 100) { Reverse = true });
            frame.Add(MirAction.Dead, new Frame(137, 1, 9, 1000));

            //51 - SnakeTotem (Archer SummonSnakes Totem)
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 2, -2, 500));
            frame.Add(MirAction.Walking, new Frame(0, 2, -2, 100));
            frame.Add(MirAction.Struck, new Frame(0, 1, -1, 100));
            frame.Add(MirAction.Die, new Frame(0, 1, -1, 100));
            frame.Add(MirAction.Dead, new Frame(0, 1, -1, 100));

            //52 - CharmedSnake (Archer SummonSnakes Snake)
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 5, 0, 200));
            frame.Add(MirAction.Walking, new Frame(0, 5, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(0, 5, 0, 100));
            frame.Add(MirAction.Struck, new Frame(0, 5, 0, 100));
            frame.Add(MirAction.Die, new Frame(52, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(59, 1, 7, 1000));

            //53 - HighAssassin
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(152, 4, 0, 100));
            frame.Add(MirAction.Dead, new Frame(155, 1, 3, 1000));
            frame.Add(MirAction.Revive, new Frame(152, 4, 0, 100) { Reverse = true });

            //54 - DarkDustPile, MudPile, SnowPile, Treasurebox
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 3, -3, 1000));
            frame.Add(MirAction.Struck, new Frame(3, 3, -3, 200));
            frame.Add(MirAction.Die, new Frame(3, 7, -7, 150));
            frame.Add(MirAction.Dead, new Frame(9, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(3, 7, -7, 150) { Reverse = true });

            //55 - Football
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 1, 0, 1000));
            frame.Add(MirAction.Walking, new Frame(8, 6, 0, 100));

            //56 - GingerBreadman
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(152, 6, 0, 100));
            frame.Add(MirAction.Dead, new Frame(157, 1, 5, 1000));
            frame.Add(MirAction.Revive, new Frame(152, 6, 0, 100) { Reverse = true });

            //57 - DreamDevourer
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(208, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(151, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 8, 0, 100) { Reverse = true });

            //58 - TailedLion
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 8, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(120, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(96, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(168, 6, 0, 100));
            frame.Add(MirAction.Dead, new Frame(173, 1, 5, 1000));
            frame.Add(MirAction.Revive, new Frame(168, 6, 0, 100) { Reverse = true });

            //59 - Behemoth
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(352, 7, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(272, 10, 0, 100));
            frame.Add(MirAction.AttackRange2, new Frame(408, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //60 - Hugger, ManectricSlave
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(208, 6, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(256, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(151, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 8, 0, 100) { Reverse = true });

            //61 - DarkDevourer
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(208, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(151, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 8, 0, 100) { Reverse = true });

            //62 - Snowman
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 2, 0, 500));
            frame.Add(MirAction.Struck, new Frame(16, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(32, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(39, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(32, 8, 0, 100) { Reverse = true });

            //63 - GiantEgg, IcePillar
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 1, -1, 1000));
            frame.Add(MirAction.Struck, new Frame(1, 4, -4, 200));
            frame.Add(MirAction.Attack1, new Frame(0, 1, -1, 1000));
            frame.Add(MirAction.Die, new Frame(5, 7, -7, 150));
            frame.Add(MirAction.Dead, new Frame(11, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(5, 7, -7, 150) { Reverse = true });

            //64 - BlueSanta
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 8, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(144, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(168, 5, 0, 100));
            frame.Add(MirAction.Dead, new Frame(172, 1, 4, 1000));
            frame.Add(MirAction.Revive, new Frame(168, 5, 0, 100) { Reverse = true });

            //65 - BattleStandard
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 8, -8, 500));
            frame.Add(MirAction.Struck, new Frame(8, 3, -3, 200));
            frame.Add(MirAction.Die, new Frame(11, 8, -8, 100));
            frame.Add(MirAction.Dead, new Frame(17, 1, -1, 1000));

            //66 - WingedTigerLord
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 10, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(328, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(288, 5, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(392, 5, 0, 150));
            frame.Add(MirAction.AttackRange1, new Frame(432, 8, 0, 150));
            frame.Add(MirAction.Struck, new Frame(192, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(216, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(224, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(216, 9, 0, 100) { Reverse = true });

            //67 - TurtleKing
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 10, 0, 150));
            frame.Add(MirAction.Attack2, new Frame(248, 6, 0, 150));
            frame.Add(MirAction.Struck, new Frame(160, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(1000, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(176, 9, 0, 100) { Reverse = true });
            frame.Add(MirAction.AttackRange1, new Frame(296, 6, 0, 100));
            frame.Add(MirAction.AttackRange2, new Frame(344, 6, 0, 100));
            frame.Add(MirAction.AttackRange3, new Frame(392, 8, 0, 100));

            //68 - Bush
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, -4, 500));
            frame.Add(MirAction.Struck, new Frame(4, 4, -4, 200));
            frame.Add(MirAction.Die, new Frame(8, 4, -4, 100));
            frame.Add(MirAction.Dead, new Frame(15, 1, -1, 1000));

            //-----------------------
            //--ABOVE FRAMES LOCKED, NO NEED TO TEST ABOVE--
            //-----------------------

            //69 - HellSlasher, HellCannibal, ManectricClub
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(224, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //70 - HellPirate
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(224, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //71 - HellBolt, WitchDoctor
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(160, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(185, 1, 9, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(256, 6, 0, 100));
            frame.Add(MirAction.Revive, new Frame(176, 10, 0, 100) { Reverse = true });

            //72 - Hellkeeper         
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, -4, 500));
            frame.Add(MirAction.Attack1, new Frame(4, 8, -8, 100));
            frame.Add(MirAction.Attack2, new Frame(22, 10, -10, 100));
            frame.Add(MirAction.Struck, new Frame(12, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(14, 8, -8, 100));
            frame.Add(MirAction.Dead, new Frame(21, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(21, 1, 0, 100) { Reverse = true });

            //73 - ManectricHammer
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 7, 0, 100));
            frame.Add(MirAction.Dead, new Frame(150, 1, 6, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 7, 0, 100) { Reverse = true });

            //74 - ManectricStaff
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 10, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(248, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(160, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(184, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(176, 9, 0, 100) { Reverse = true });

            //75 - ManectricBlest, NamelessGhost, DarkGhost, ChaosGhost, TrollHammer, TrollBomber, TrollStoner, MutatedManworm, CrazyManworm
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(208, 8, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(272, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(151, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 8, 0, 100) { Reverse = true });

            //76 - ManectricKing, TrollKing
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 120));
            frame.Add(MirAction.Attack2, new Frame(224, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(288, 9, 0, 100));
            frame.Add(MirAction.AttackRange2, new Frame(224, 8, 0, 100));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //77 - FlameSpear, FlameMage, FlameScythe, FlameAssassin
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //78 - FlameQueen
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(224, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(296, 8, 0, 100));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //79 - HellKnight1~4
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Appear, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(176, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 4, 0, 100));
            frame.Add(MirAction.Dead, new Frame(147, 1, 3, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.AttackRange2, new Frame(176, 6, 0, 100));
            frame.Add(MirAction.Revive, new Frame(144, 4, 0, 100) { Reverse = true });

            //80 - HellLord
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, -4, 500));
            frame.Add(MirAction.Attack1, new Frame(4, 6, -6, 200));
            frame.Add(MirAction.Struck, new Frame(0, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(10, 5, -5, 100));
            frame.Add(MirAction.Dead, new Frame(14, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(10, 5, -5, 100) { Reverse = true });

            //81 - WaterGuard
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(208, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(151, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 8, 0, 100) { Reverse = true });

            //82 - IceGuard
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(208, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(151, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 8, 0, 100) { Reverse = true });

            //83 - DemonGuard
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 4, 0, 100));
            frame.Add(MirAction.Struck, new Frame(112, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(128, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(135, 1, 7, 1000));
            frame.Add(MirAction.Attack2, new Frame(192, 6, 0, 100));
            frame.Add(MirAction.Show, new Frame(240, 6, 0, 200));

            //84 - KingGuard
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(224, 6, 0, 100));//bugy
            frame.Add(MirAction.AttackRange1, new Frame(272, 8, 0, 100));//ragneg ?
            frame.Add(MirAction.AttackRange2, new Frame(336, 7, 0, 100));//ragneg ?
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //--------------------
            //--CHECK NO FURTHER UNTIL ABOVE HAS BEEN LOCKED--
            //--------------------

            //85 - Bunny, Bunny2
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 5, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(72, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 6, 0, 100));
            frame.Add(MirAction.Dead, new Frame(149, 1, 5, 1000));

            //86 - DarkBeast, LightBeast(已调整) 
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(152, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(184, 1, 8, 1000));
            frame.Add(MirAction.Attack2, new Frame(248, 6, 0, 100));

            //87 - HardenRhino 铁甲犀牛(已调整)
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 10, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(128, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(184, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(208, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(217, 1, 9, 1000));
            frame.Add(MirAction.Attack2, new Frame(288, 7, 0, 100, 392, 5, -5, 100) );
            frame.Add(MirAction.Attack3, new Frame(344, 6, 0, 200, 397, 6, 0, 200));

            //88 - AncientBringer 丹墨 BOSS
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(112, 10, 0, 100, 512, 6, 0, 200));
            frame.Add(MirAction.Attack2, new Frame(304, 10, 0, 100, 568, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(192, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(224, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(233, 1, 9, 1000));
            //frame.Add(MirAction.AttackRange1, new Frame(384, 8, 0, 100,648,5,0,200) { EStart2 = 720, ECount2=10, ESkip2=-10 , EInterval2 =100});
            frame.Add(MirAction.AttackRange1, new Frame(384, 8, 0, 100, 648, 5, 0, 200));
            frame.Add(MirAction.AttackRange2, new Frame(448, 8, 0, 100, 730, 10, 0, 100));

            //89 - Jar1
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Struck, new Frame(18, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(50, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(59, 1, 0, 1000));

            //90 - SeedingsGeneral 灵猫圣兽
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.SitDown, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Standing, new Frame(32, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(64, 7, 0, 200));
            //frame.Add(MirAction.Runing, new Frame(120, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(168, 9, 0, 100, 1072, 9, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(240, 9, 0, 100, 1192, 9, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(312, 8, 0, 100, 1144, 6, 0, 100)); //stupple 08/04
            frame.Add(MirAction.AttackRange2, new Frame(376, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(448, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(472, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(479, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(472, 10, 0, 100) { Reverse = true });

            //91 - Tucson, TucsonFighter
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(168, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(192, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(200, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(192, 10, 0, 100) { Reverse = true });

            //92 - FrozenDoor
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 1, -1, 1000));
            frame.Add(MirAction.Struck, new Frame(1, 3, -3, 200));
            frame.Add(MirAction.Die, new Frame(4, 7, -7, 150));
            frame.Add(MirAction.Dead, new Frame(10, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(4, 7, -7, 150) { Reverse = true });

            //93 - TucsonMage
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 7, 0, 500));
            frame.Add(MirAction.Walking, new Frame(56, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(104, 7, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(160, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(216, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(240, 7, 0, 100));
            frame.Add(MirAction.Dead, new Frame(246, 1, 6, 1000));
            frame.Add(MirAction.Revive, new Frame(240, 7, 0, 100) { Reverse = true });

            //94 - TucsonWarrior
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 5, 0, 500));
            frame.Add(MirAction.Walking, new Frame(40, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(88, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(152, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(216, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(240, 7, 0, 100));
            frame.Add(MirAction.Dead, new Frame(246, 1, 6, 1000));
            frame.Add(MirAction.Revive, new Frame(240, 7, 0, 100) { Reverse = true });

            //95 - Armadillo
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(160, 10, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(240, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(312, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(336, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(345, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(336, 10, 0, 100) { Reverse = true });
            frame.Add(MirAction.Show, new Frame(416, 7, 0, 200));

            //96 - ArmadilloElder
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 5, 0, 500));
            frame.Add(MirAction.Walking, new Frame(40, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(88, 10, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(168, 10, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(248, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(320, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(344, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(353, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(336, 10, 0, 100) { Reverse = true });
            frame.Add(MirAction.Show, new Frame(424, 7, 0, 200));

            //97 - TucsonEgg
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 1, -1, 500));
            frame.Add(MirAction.Struck, new Frame(1, 1, -1, 200));
            frame.Add(MirAction.Die, new Frame(10, 10, -10, 100));
            frame.Add(MirAction.Dead, new Frame(11, 1, -1, 1000));

            //98 - PlaguedTucson 
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(160, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(184, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(191, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(184, 8, 0, 100) { Reverse = true });

            //99 - SandSnail 
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 9, 0, 500));
            frame.Add(MirAction.Walking, new Frame(72, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(120, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(184, 10, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(264, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(344, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(368, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(377, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(368, 10, 0, 100) { Reverse = true });

            //100 - CannibalTentacles
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 9, 0, 500));
            frame.Add(MirAction.Walking, new Frame(72, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(120, 10, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(184, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(344, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(368, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(377, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(368, 10, 0, 100) { Reverse = true });

            //101 - TucsonGeneral  
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 8, 0, 500));
            frame.Add(MirAction.Walking, new Frame(64, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(112, 7, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(168, 7, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(224, 7, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(280, 8, 0, 100));
            frame.Add(MirAction.AttackRange2, new Frame(344, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(408, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(440, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(448, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(440, 8, 0, 100) { Reverse = true });

            //102 - GasToad 蛤蟆，如何神殿怪物
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 8, 0, 500));
            frame.Add(MirAction.Walking, new Frame(64, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(112, 10, 0, 100));//吐气攻击
            frame.Add(MirAction.Attack2, new Frame(192, 8, 0, 100));//这是跳跃吧
            frame.Add(MirAction.Attack3, new Frame(256, 10, 0, 100, 440, 9, -9, 100));//放毒？
            frame.Add(MirAction.Struck, new Frame(336, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(360, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(369, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(360, 10, 0, 100) { Reverse = true });

            //103 - Mantis 螳螂
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 7, 0, 500));
            frame.Add(MirAction.Walking, new Frame(56, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(104, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(168, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(224, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(248, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(255, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(248, 8, 0, 100) { Reverse = true });

            //104 - SwampWarrior 神殿树人
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(80, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(128, 10, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(208, 10, 0, 100));//攻击2，远程攻击，放蘑菇
            frame.Add(MirAction.Struck, new Frame(288, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(312, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(321, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(312, 10, 0, 100) { Reverse = true });

            //105 - AssassinBird 神殿刺鸟
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 7, 0, 500));
            frame.Add(MirAction.Walking, new Frame(56, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(104, 7, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(232, 9, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(160, 9, 0, 100));//禁锢，眩晕禁锢
            frame.Add(MirAction.Struck, new Frame(304, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(328, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(335, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(328, 8, 0, 100) { Reverse = true });

            //106 - RhinoWarrior 犀牛勇士
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(80, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(128, 7, 0, 100, 320, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(184, 7, 0, 100, 447, 8, 0, 100) );//水泡效果，减速
            frame.Add(MirAction.AttackRange1, new Frame(184, 7, 0, 100, 383, 8, 0, 100) );//砸地板，身前1格范围
            frame.Add(MirAction.Struck, new Frame(240, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(264, 7, 0, 100));
            frame.Add(MirAction.Dead, new Frame(270, 1, 6, 1000));
            frame.Add(MirAction.Revive, new Frame(264, 7, 0, 100) { Reverse = true });

            //107 - RhinoPriest  犀牛的牧师
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 7, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(208, 9, 0, 100, 448, 7, 0, 100) );
            frame.Add(MirAction.Attack3, new Frame(152, 7, 0, 100, 376, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(280, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(304, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(312, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(304, 9, 0, 100) { Reverse = true });

            //108 - SwampSlime 泥战士
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(80, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(128, 10, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(208, 10, 0, 100, 368, 9, -9, 100) );
            frame.Add(MirAction.Struck, new Frame(288, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(312, 7, 0, 100));
            frame.Add(MirAction.Dead, new Frame(318, 1, 6, 1000));
            frame.Add(MirAction.Revive, new Frame(312, 7, 0, 100) { Reverse = true });

            //109 - RockGuard 石巨人，类似僵尸，满血复活？
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 9, 0, 500));
            frame.Add(MirAction.Walking, new Frame(72, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(120, 10, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(200, 10, 0, 100, 368, 8, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(280, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(304, 8, 0, 200));
            frame.Add(MirAction.Dead, new Frame(311, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(304, 8, 0, 100) { Reverse = true });

            //110 - MudWarrior 土巨人
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 8, 0, 500));
            frame.Add(MirAction.Walking, new Frame(64, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(112, 10, 0, 100, 432, 9, -9, 100));//拉人
            frame.Add(MirAction.Attack2, new Frame(192, 10, 0, 100));
            //frame.Add(MirAction.Attack3, new Frame(272, 10, 0, 100));
            //frame.Add(MirAction.AttackRange1, new Frame(848, 8, 0, 100)); 
            //frame.Add(MirAction.AttackRange2, new Frame(912, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(272, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(296, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(303, 1, 7, 1000));
            //frame.Add(MirAction.Revive, new Frame(396, 8, 0, 100) { Reverse = true });
            frame.Add(MirAction.Revive, new Frame(360, 9, 0, 200));//复活
            frame.Add(MirAction.Show, new Frame(360, 9, 0, 200));//复活

            //111 - SmallPot 如何使者 小BOSS,也会复活？靠
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(24, 9, 0, 500));
            frame.Add(MirAction.Walking, new Frame(96, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(144, 10, 0, 100));//拳击
            frame.Add(MirAction.Attack2, new Frame(224, 10, 0, 100, 568, 8, 0, 100));//拍击
            frame.Add(MirAction.Attack3, new Frame(304, 10, 0, 100, 632, 5, 5, 100));//仗击
            frame.Add(MirAction.AttackRange1, new Frame(384, 10, 0, 100, 708, 10, 0, 100) );//唱歌，释放魔法
            frame.Add(MirAction.Struck, new Frame(464, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(488, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(497, 1, 9, 1000, 0, 12, -12, 200));
            frame.Add(MirAction.Revive, new Frame(488, 10, 0, 200, 0, 12, -12, 200) { Reverse = true });
            frame.Add(MirAction.Show, new Frame(372, 9, 0, 200));

            //112 - TreeQueen 树女王 攻击1推开 攻击2火雨，攻击3束缚 攻击4地刺，攻击5群地刺
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, -10, 500));
            frame.Add(MirAction.Attack1, new Frame(11, 10, -10, 100, 75, 16, -16, 100));//推开
            frame.Add(MirAction.Attack2, new Frame(11, 10, -10, 100, 92, 14, -14, 100));//地刺

            //frame.Add(MirAction.Attack1, new Frame(11, 10, -10, 100,66,9,-9,100));
            frame.Add(MirAction.AttackRange1, new Frame(11, 10, -10, 100, 66, 9, -9, 100));//蜘蛛网，麻痹 1段
            frame.Add(MirAction.AttackRange2, new Frame(11, 10, -10, 100, 106, 14, -14, 100));//火雨 3段伤害
            frame.Add(MirAction.Struck, new Frame(21, 3, -3, 200));
            frame.Add(MirAction.Die, new Frame(24, 11, -11, 100));
            frame.Add(MirAction.Dead, new Frame(34, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(24, 11, -11, 100) { Reverse = true });

            //113 - ShellFighter 斗争者 蚂蚁司令官？ 5种攻击手段？
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 9, 0, 500));
            frame.Add(MirAction.Walking, new Frame(72, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(120, 9, 0, 100));//普通攻击
            frame.Add(MirAction.Attack2, new Frame(192, 10, 0, 100, 592, 9, 0, 100) );//隔位攻击
            frame.Add(MirAction.Attack3, new Frame(272, 9, 0, 100, 592, 9, 0, 100) );//隔位攻击
            frame.Add(MirAction.AttackRange1, new Frame(344, 9, 0, 100, 755, 21, -21, 100));//放群毒，周围都中毒
            frame.Add(MirAction.AttackRange2, new Frame(416, 10, 0, 100, 664, 10, 0, 100));//放蜘蛛网，束缚，麻痹
            frame.Add(MirAction.Struck, new Frame(496, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(520, 9, 1, 100));
            frame.Add(MirAction.Dead, new Frame(528, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(520, 9, 1, 100) { Reverse = true });

            //114 - DarkBaboon 黑暗的狒狒 3种攻击,2和3一样的啊。靠
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 7, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(232, 8, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(296, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(152, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 7, 0, 100));
            frame.Add(MirAction.Dead, new Frame(182, 1, 6, 1000));
            frame.Add(MirAction.Revive, new Frame(176, 7, 0, 100) { Reverse = true });

            //115 - TwinHeadBeast 双头兽 OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 9, 0, 500));
            frame.Add(MirAction.Walking, new Frame(72, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(120, 9, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(296, 7, 0, 100, 352, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(192, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(216, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(225, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(216, 10, 0, 100) { Reverse = true });

            //116 - OmaCannibal 奥玛食人族 OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 9, 0, 500));
            frame.Add(MirAction.Walking, new Frame(72, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(120, 8, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(184, 9, 0, 100));//喷毒
            frame.Add(MirAction.Struck, new Frame(256, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(280, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(289, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(280, 10, 0, 100) { Reverse = true });

            //117 - OmaSlasher 奥玛斧头兵 OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(80, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(128, 10, 0, 100, 304, 4, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(208, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(232, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(240, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(232, 9, 0, 100) { Reverse = true });

            //118 - OmaAssassin 奥玛刺客 OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(80, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(128, 10, 0, 100, 312, 5, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(208, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(232, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(241, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(232, 10, 0, 100) { Reverse = true });

            //119 - OmaMage //DUPE of 104 奥玛法师 OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(80, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(128, 10, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(208, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(288, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(312, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(321, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(312, 10, 0, 100) { Reverse = true });

            //120 - OmaWitchDoctor 奥玛巫医 OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 9, 0, 500));
            frame.Add(MirAction.Walking, new Frame(72, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(120, 7, 0, 100, 520, 7, 0, 100) );
            frame.Add(MirAction.Attack2, new Frame(176, 7, 0, 100, 576, 7, 0, 100) );
            frame.Add(MirAction.Attack3, new Frame(232, 9, 0, 100, 632, 9, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(304, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(328, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(336, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(280, 9, 0, 100) { Reverse = true });

            //121 - OmaBlest //DUPE of 104 奥玛祝福 普通攻击，砸地板
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(80, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(128, 10, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(208, 10, 0, 100, 392, 5, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(288, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(312, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(321, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(312, 10, 0, 100) { Reverse = true });

            //122 - LightningBead, HealingBead, PowerUpBead
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 7, -7, 800));
            frame.Add(MirAction.Walking, new Frame(7, 7, -7, 80));
            frame.Add(MirAction.Attack1, new Frame(8, 5, -5, 80));
            frame.Add(MirAction.Struck, new Frame(14, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(16, 8, -8, 120));
            frame.Add(MirAction.Dead, new Frame(23, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(34, 10, -10, 150) { Reverse = true });

            //123 - DarkOmaKing
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(80, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(128, 9, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(200, 34, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(472, 8, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(536, 9, 0, 100));
            frame.Add(MirAction.AttackRange2, new Frame(608, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(680, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(704, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(703, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(704, 10, 0, 100) { Reverse = true });

            //124 - CaveMage 洞穴法师？
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 2, -2, 800));
            frame.Add(MirAction.AttackRange1, new Frame(0, 2, -2, 80));
            frame.Add(MirAction.Struck, new Frame(0, 2, -2, 200, 10, 7, -7, 100));
            frame.Add(MirAction.Die, new Frame(2, 8, -8, 120));
            frame.Add(MirAction.Dead, new Frame(9, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(2, 8, -8, 150) { Reverse = true });

            //125 - Mandrill 长鼻猴 普通攻击 攻击并净化，回血
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 7, 0, 100, 264, 2, 0, 200) );
            frame.Add(MirAction.Attack2, new Frame(136, 3, 0, 200, 280, 10, -10, 100));
            frame.Add(MirAction.Struck, new Frame(160, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(184, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(193, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(184, 10, 0, 100) { Reverse = true });

            //126 - PlagueCrab 瘟疫蟹
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500, 248, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200, 280, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 8, 0, 100, 328, 8, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(144, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(168, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(177, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(168, 10, 0, 100) { Reverse = true });

            //127 - CreeperPlant 攀缘花
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Attack1, new Frame(32, 7, 0, 100, 250, 6, -6, 100));
            frame.Add(MirAction.AttackRange1, new Frame(88, 7, 0, 100, 266, 6, -6, 100));
            frame.Add(MirAction.Struck, new Frame(136, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(160, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(168, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(160, 9, 0, 100) { Reverse = true });
            frame.Add(MirAction.Show, new Frame(232, 9, -9, 150));
            frame.Add(MirAction.Hide, new Frame(241, 9, -9, 150) { Reverse = true });

            //128 - SackWarrior
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(144, 9, 0, 200));
            frame.Add(MirAction.Struck, new Frame(216, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(240, 13, 0, 100));
            frame.Add(MirAction.Dead, new Frame(252, 1, 12, 1000));
            frame.Add(MirAction.Revive, new Frame(240, 13, 0, 100) { Reverse = true });

            //129 - WereTiger
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 10, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(160, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(240, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(264, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(263, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(264, 10, 0, 100) { Reverse = true });

            //130 - KingHydrax
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(144, 7, 0, 200));
            frame.Add(MirAction.Attack3, new Frame(200, 8, 0, 200));
            frame.Add(MirAction.Struck, new Frame(264, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(287, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(288, 10, 0, 100) { Reverse = true });

            //131 - FloatingWraith 幽灵射手 --OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(80, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(144, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(168, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(177, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(168, 10, 0, 100) { Reverse = true });

            //132 - ArmedPlant 幽灵厨子 - OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 6, 0, 100, 256, 2, 0, 200));
            frame.Add(MirAction.Struck, new Frame(144, 6, 0, 200));
            frame.Add(MirAction.Die, new Frame(192, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(199, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(192, 8, 0, 100) { Reverse = true });

            //133 - AvengerPlant 幽灵船员 -OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 7, 0, 100, 224, 3, 0, 200));
            frame.Add(MirAction.Struck, new Frame(136, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(160, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(167, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(160, 8, 0, 100) { Reverse = true });

            //134 - Nadz, AvengingSpirit  ---错误，在后面覆盖实现
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(140, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //135 - AvengingWarrior--复仇的勇士
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100, 272, 5, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(128, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(178, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(192, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(201, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(192, 10, 0, 100) { Reverse = true });

            //136 - AxePlant, ClawBeast 黑暗头目-OK  ClawBeast（有问题，重写）
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 10, 0, 100, 256, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(160, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(184, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(192, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(184, 9, 0, 100) { Reverse = true });

            //137 - WoodBox，爆炸箱子
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 1, 0, 500));
            frame.Add(MirAction.Struck, new Frame(8, 3, 9, 200));
            frame.Add(MirAction.Die, new Frame(11, 9, 3, 100, 104, 6, -6, 100) );
            frame.Add(MirAction.Dead, new Frame(19, 1, 11, 1000));

            //138 - KillerPlant ,黑暗船长
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 300, 584, 10, 0, 300) );
            frame.Add(MirAction.Walking, new Frame(80, 8, 0, 100, 664, 8, 0, 100));
            //1.挥刀砍，范围伤害
            frame.Add(MirAction.Attack1, new Frame(144, 7, 0, 100, 1168, 4, 0, 100) );
            //攻击2解毒，加血
            frame.Add(MirAction.Attack2, new Frame(256, 7, 0, 100, 840, 7, 0, 100) );
            //3.踏地板，眩晕，麻痹
            frame.Add(MirAction.Attack3, new Frame(256, 7, 0, 100, 840, 7, 0, 100) );

            //4.没调试好
            frame.Add(MirAction.Attack4, new Frame(200, 7, 0, 100, 784, 7, 0, 100) );

            //放雷电，追踪雷电
            frame.Add(MirAction.AttackRange1, new Frame(368, 7, 0, 100, 952, 7, 0, 100));

            //放雷电，满天的雷电
            //frame.Add(MirAction.AttackRange2, new Frame(424, 7, 0, 100, 896, 7, 0, 100));-OK
            frame.Add(MirAction.AttackRange2, new Frame(424, 7, 0, 100, 1008, 7, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(480, 3, 0, 200, 1064, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(504, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(513, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(504, 10, 0, 100) { Reverse = true });

            //139 - Hydrax
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(144, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(168, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(167, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(168, 9, 0, 100) { Reverse = true });

            //140 - Basiloid
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 1, -1, 500));
            frame.Add(MirAction.Struck, new Frame(0, 2, -2, 200));
            frame.Add(MirAction.Die, new Frame(2, 7, -7, 100));
            frame.Add(MirAction.Dead, new Frame(8, 1, -1, 1000));

            //141 - HornedMage
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(144, 9, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(216, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(280, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(304, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(313, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(304, 10, 0, 100) { Reverse = true });

            //142 - HornedArcher, ColdArcher
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 10, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(160, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(232, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(256, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(265, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(256, 10, 0, 100) { Reverse = true });

            //143 - HornedWarrior
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(144, 9, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(216, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(280, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(304, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(312, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(304, 9, 0, 100) { Reverse = true });

            //144 - FloatingRock
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 500));
            frame.Add(MirAction.Attack1, new Frame(80, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(80, 8, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 7, -7, 100));
            frame.Add(MirAction.Dead, new Frame(1502, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 7, -7, 100) { Reverse = true });

            //145 - ScalyBeast
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 10, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(112, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(176, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(256, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(272, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(270, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(272, 9, 0, 100) { Reverse = true });

            //146 - HornedSorceror
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(160, 9, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(224, 9, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(295, 8, 0, 200));
            frame.Add(MirAction.AttackRange2, new Frame(359, 9, 0, 200));
            frame.Add(MirAction.Struck, new Frame(432, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(456, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(455, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(456, 10, 0, 100) { Reverse = true });

            //147 - BoulderSpirit
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 8, -8, 500));
            frame.Add(MirAction.Attack1, new Frame(8, 3, 0, 100));
            frame.Add(MirAction.Struck, new Frame(32, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(56, 8, -8, 100));
            frame.Add(MirAction.Dead, new Frame(63, 1, -1, 1000));

            //148 - HornedCommander
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 10, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(176, 8, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(240, 10, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(320, 8, 0, 200));
            frame.Add(MirAction.AttackRange2, new Frame(384, 8, 0, 200));
            frame.Add(MirAction.AttackRange3, new Frame(448, 8, 0, 200));
            //frame.Add(MirAction.AttackRange4, new Frame(512, 10, 0, 200));
            //frame.Add(MirAction.AttackRange5, new Frame(592, 8, 0, 200));
            frame.Add(MirAction.Struck, new Frame(656, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(680, 13, 0, 100));
            frame.Add(MirAction.Dead, new Frame(692, 1, 12, 1000));
            frame.Add(MirAction.Revive, new Frame(680, 13, 0, 100) { Reverse = true });

            //149 - MoonStone, SunStone, LightningStone
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 2, -2, 500));

            //150 - Turtlegrass //  use black fox ai -- 1turtlegrass
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Hide, new Frame(0, 1, 0, 500));
            frame.Add(MirAction.Show, new Frame(8, 4, 0, 200) { Reverse = true });
            frame.Add(MirAction.Standing, new Frame(40, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(72, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(120, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(256, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(280, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(289, 1, 0, 1000));
            frame.Add(MirAction.Attack2, new Frame(184, 9, 0, 100));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //151 - Mantree
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(64, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(96, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(144, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(208, 10, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(368, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(392, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(401, 1, 0, 1000));
            //frame.Add(MirAction.Standing, new Frame(1, 1, 0, 500)); neeed codeing
            //frame.Add(MirAction.Walking, new Frame(8, 7, 0, 100));neeed codeing

            //152 - Bear //AI BLACKFOX
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(216, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(240, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(248, 1, 0, 1000));
            frame.Add(MirAction.Attack2, new Frame(152, 8, 0, 100));

            //153 - Leopard
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(80, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(160, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(184, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(191, 1, 0, 1000));

            //154 - ChieftainArcher
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 9, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(152, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(216, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(240, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(248, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(240, 9, 0, 100) { Reverse = true });

            //155 - ChieftainSword 阳龙王(已调整)
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 1348, 5, 0, 200));//普通砍(半月)
            frame.Add(MirAction.Attack2, new Frame(392, 9, 0, 200, 1082, 5, 0, 200) );//冲撞
            frame.Add(MirAction.Attack3, new Frame(1196, 9, 0, 100, 1187, 9, -9, 100));//隐身
            frame.Add(MirAction.Attack4, new Frame(1268, 10, 0, 100, 1348, 5, 0, 200));//现身杀
            frame.Add(MirAction.AttackRange1, new Frame(312, 10, 0, 200, 752, 7, 0, 100) );//开天劈地
            frame.Add(MirAction.AttackRange2, new Frame(232, 10, 0, 100, 864, 10, 0, 100) );//释放魔法，放火
            frame.Add(MirAction.AttackRange3, new Frame(160, 9, 0, 100, 1002, 8, 0, 100) );//撂倒

            frame.Add(MirAction.Struck, new Frame(658, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(672, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(681, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(672, 10, 0, 100) { Reverse = true });

            //156 - StoningSpider //Archer Summon
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 2, -0, 500));
            frame.Add(MirAction.Attack1, new Frame(16, 4, 0, 100));
            frame.Add(MirAction.Die, new Frame(48, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(57, 1, 9, 1000));

            //157 - FrozenSoldier 雪原士兵-OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 9, 0, 100, 256, 10, -10, 100));
            frame.Add(MirAction.Struck, new Frame(152, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(185, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(176, 10, 0, 100) { Reverse = true });

            //158 - FrozenFighter //DUPE of 142  雪原战士-OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 10, 0, 100, 336, 6, 0, 100) );
            frame.Add(MirAction.Attack2, new Frame(160, 9, 0, 100, 384, 5, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(232, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(256, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(265, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(256, 10, 0, 100) { Reverse = true });

            //159 - FrozenArcher 雪原弓手-OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.AttackRange1, new Frame(80, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(160, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(184, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(193, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(184, 10, 0, 100) { Reverse = true });

            //160 - FrozenKnight 雪原勇士
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 10, 0, 100, 360, 3, 0, 200) );
            frame.Add(MirAction.Attack2, new Frame(176, 10, 0, 100, 384, 9, 0, 100));
            frame.Add(MirAction.Struck, new Frame(256, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(280, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(289, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(280, 10, 0, 100) { Reverse = true });

            //161 - FrozenGolem 雪原鬼尊
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 200, 264, 4, 0, 200));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200, 296, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 8, 0, 100, 344, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(168, 12, 0, 100, 408, 6, 0, 200) );
            frame.Add(MirAction.Struck, new Frame(144, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(456, 7, -7, 200));
            frame.Add(MirAction.Dead, new Frame(458, 1, -1, 1000));
            frame.Add(MirAction.Revive, new Frame(456, 7, -7, 200) { Reverse = true });

            //162 - IcePhantom 雪原恶鬼 - OK 隐身
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 200, 320, 4, 0, 200));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200, 352, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 9, 0, 100, 400, 9, 0, 100) );
            frame.Add(MirAction.AttackRange1, new Frame(152, 8, 0, 100, 472, 8, 0, 100) );
            frame.Add(MirAction.AttackRange2, new Frame(152, 8, 0, 100, 472, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(216, 3, 0, 200, 536, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(240, 10, 0, 100, 560, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(249, 1, 9, 1000));
            frame.Add(MirAction.Hide, new Frame(692, 10, 0, 100));
            frame.Add(MirAction.Appear, new Frame(692, 10, 0, 100) { Reverse = true });
            frame.Add(MirAction.Show, new Frame(692, 10, 0, 100) { Reverse = true });
            frame.Add(MirAction.Revive, new Frame(240, 10, 0, 100) { Reverse = true });

            //163 - SnowWolf 雪原冰狼-OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 10, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(160, 9, 0, 100, 328, 9, -9, 100));
            frame.Add(MirAction.Struck, new Frame(232, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(256, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(264, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(256, 9, 0, 100) { Reverse = true });

            //164 - SnowWolfKing 雪太狼
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 456, 3, 0, 200) );
            frame.Add(MirAction.Attack2, new Frame(160, 9, 0, 100, 489, 9, 0, 100) );
            frame.Add(MirAction.Attack3, new Frame(232, 10, 0, 100, 581, 3, 0, 200) );
            frame.Add(MirAction.AttackRange1, new Frame(312, 5, 0, 100, 456, 3, 0, 200) );
            frame.Add(MirAction.AttackRange2, new Frame(312, 5, 0, 100, 456, 3, 0, 200) );
            frame.Add(MirAction.Struck, new Frame(352, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(376, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(385, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(376, 10, 0, 100) { Reverse = true });

            //165 - WaterDragon //ec ai
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Hide, new Frame(0, 8, 0, 500, 400, 8, 0, 500) { Reverse = true });
            frame.Add(MirAction.Standing, new Frame(64, 6, 0, 300));
            frame.Add(MirAction.Attack1, new Frame(112, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(176, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(256, 3, 0, 200, 548, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(280, 15, 0, 100));
            frame.Add(MirAction.Dead, new Frame(294, 1, 14, 1000));
            //frame.Add(MirAction.Walking, new Frame(72, 6, 0, 100,500,6,0,100));
            frame.Add(MirAction.Walking, new Frame(0, 8, 0, 200, 500, 6, 0, 100));

            //166 - BlackTortoise
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            //frame.Add(MirAction.StandingAlt, new Frame(32, 10, 0, 200));
            frame.Add(MirAction.Walking, new Frame(112, 4, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(144, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(192, 7, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(248, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(296, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(312, 6, 0, 100));
            frame.Add(MirAction.Dead, new Frame(326, 1, 5, 1000));
            frame.Add(MirAction.Revive, new Frame(312, 6, 0, 100) { Reverse = true });

            //167 - Manticore //mage ai needed black fox will do 4 now
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(112, 8, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(176, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(240, 7, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(296, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(352, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(376, 15, 0, 100));
            frame.Add(MirAction.Dead, new Frame(390, 1, 0, 1000));
            //frame.Add(MirAction.Flying, new Frame(32, 9, 0, 100));

            //168 - DragonWarrior:
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            //frame.Add(MirAction.Standing2, new Frame(32, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(112, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(176, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(240, 10, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(320, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(376, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(400, 13, 0, 100));
            frame.Add(MirAction.Dead, new Frame(412, 1, 12, 1000));
            frame.Add(MirAction.Revive, new Frame(400, 13, 0, 100) { Reverse = true });

            //169 - DragonArcher
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            //frame.Add(MirAction.Standing2, new Frame(32, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(112, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(176, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(240, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(288, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(312, 13, 0, 100));
            frame.Add(MirAction.Dead, new Frame(324, 1, 12, 1000));
            frame.Add(MirAction.Revive, new Frame(312, 13, 0, 100) { Reverse = true });

            //170 - Kirin
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            //frame.Add(MirAction.Standing2, new Frame(32, 9, 0, 500));
            frame.Add(MirAction.Walking, new Frame(104, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(152, 7, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(208, 12, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(304, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(352, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(376, 2, 0, 100));
            frame.Add(MirAction.Dead, new Frame(377, 1, 0, 1000));
            frame.Add(MirAction.Revive, new Frame(376, 2, 0, 100) { Reverse = true });

            //171 - Guard3
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 7, 0, 500));
            frame.Add(MirAction.Attack1, new Frame(56, 7, 0, 100));

            //172 - ArcherGuard3
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 4, 500));
            frame.Add(MirAction.Attack1, new Frame(80, 7, 3, 100));

            //173 - BLANK
            FrameList.Add(frame = new FrameSet());

            //174 - FrozenMiner 冰魄矿工-OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 10, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(112, 6, 0, 100));//这个攻击不要了，没什么用
            frame.Add(MirAction.Attack2, new Frame(160, 10, 0, 100, 432, 3, 0, 200) );
            frame.Add(MirAction.Attack3, new Frame(240, 10, 0, 100, 462, 5, 0, 200) );
            frame.Add(MirAction.Struck, new Frame(320, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(352, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(361, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(352, 10, 0, 100) { Reverse = true });

            //175 - FrozenAxeman 冰魄斧兵-OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            //frame.Add(MirAction.Standing2, new Frame(32, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(112, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(176, 10, 0, 100, 528, 3, 0, 200) );
            frame.Add(MirAction.Attack2, new Frame(256, 10, 0, 100, 558, 3, 0, 200) );
            frame.Add(MirAction.Attack3, new Frame(336, 10, 0, 100, 588, 3, 0, 200) );
            frame.Add(MirAction.Struck, new Frame(416, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(448, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(457, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(448, 10, 0, 100) { Reverse = true });

            //176 - FrozenMagician 冰魄法师-OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            //frame.Add(MirAction.Standing2, new Frame(32, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(112, 6, 0, 200));
            frame.Add(MirAction.AttackRange1, new Frame(160, 10, 0, 100, 512, 6, 0, 100) );
            frame.Add(MirAction.AttackRange2, new Frame(240, 10, 0, 100, 662, 8, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(320, 10, 0, 100, 840, 4, 0, 200) );
            frame.Add(MirAction.Struck, new Frame(400, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(432, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(441, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(432, 10, 0, 100) { Reverse = true });

            //177 - SnowYeti 冰魄雪人
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 7, 0, 500));
            //frame.Add(MirAction.Standing2, new Frame(56, 10, 0, 500));
            frame.Add(MirAction.Walking, new Frame(136, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(200, 9, 0, 100, 504, 4, 0, 200));
            frame.Add(MirAction.Attack2, new Frame(272, 9, 0, 100, 536, 3, 0, 200));
            frame.Add(MirAction.AttackRange1, new Frame(344, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(408, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(432, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(440, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(432, 9, 0, 100) { Reverse = true });

            //178 - IceCrystalSoldier 冰晶战士-OK
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(112, 10, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(192, 10, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(272, 10, 0, 100, 476, 8, -8, 100) );
            frame.Add(MirAction.Struck, new Frame(352, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(384, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(393, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(384, 10, 0, 100) { Reverse = true });

            //179 - DarkWraith 暗黑战士
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500, 360, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200, 392, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 8, 0, 100, 440, 8, 0, 100) );
            frame.Add(MirAction.Attack2, new Frame(144, 10, 0, 100, 504, 10, 0, 100) );
            frame.Add(MirAction.AttackRange1, new Frame(224, 4, 0, 200, 584, 4, 0, 200));
            frame.Add(MirAction.Struck, new Frame(256, 3, 0, 200, 616, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(280, 10, 0, 100, 640, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(289, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(280, 10, 0, 100) { Reverse = true });

            //180 - CrystalBeast 水晶兽 冰雪守护神
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500, 496, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 200, 544, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 10, 0, 100, 592, 10, 0, 100) );
            frame.Add(MirAction.Attack2, new Frame(176, 8, 0, 100, 672, 8, 0, 100) );
            frame.Add(MirAction.Attack3, new Frame(240, 6, 0, 100, 736, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(288, 5, 0, 100, 784, 5, 0, 100));
            frame.Add(MirAction.AttackRange2, new Frame(328, 2, 0, 100, 824, 2, 0, 100));
            frame.Add(MirAction.AttackRange3, new Frame(344, 8, 0, 100, 840, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(408, 3, 0, 200, 904, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(432, 8, 0, 100, 928, 6, 0, 100));
            frame.Add(MirAction.Dead, new Frame(440, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(432, 8, 0, 100, 928, 6, 0, 100) { Reverse = true });

            //181 - RedOrb, BlueOrb, YellowOrb, GreenOrb, WhiteOrb
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 5, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(120, 5, 0, 100));
            frame.Add(MirAction.Struck, new Frame(160, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 5, 0, 100));
            frame.Add(MirAction.Dead, new Frame(180, 1, 4, 1000));
            frame.Add(MirAction.Revive, new Frame(176, 5, 0, 100) { Reverse = true });

            //182 - FatalLotus
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(160, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(224, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(240, 6, 0, 100));
            frame.Add(MirAction.Dead, new Frame(245, 1, 5, 1000));
            frame.Add(MirAction.Revive, new Frame(240, 6, 0, 100) { Reverse = true });

            //183 - AntCommander
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(112, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(176, 8, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(240, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(304, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(320, 6, 0, 100));
            frame.Add(MirAction.Dead, new Frame(393, 1, 5, 1000));
            frame.Add(MirAction.Revive, new Frame(320, 6, 0, 100) { Reverse = true });

            //184 - CargoBoxwithlogo
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 1, 0, 500));
            frame.Add(MirAction.Struck, new Frame(8, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(24, 6, 0, 100));
            frame.Add(MirAction.Dead, new Frame(29, 1, 0, 1000));

            //185 - Doe
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(144, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(160, 6, 0, 100));
            frame.Add(MirAction.Dead, new Frame(165, 1, 5, 1000));
            frame.Add(MirAction.Revive, new Frame(160, 6, 0, 100) { Reverse = true });

            //186 - AngryReindeer
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 7, 0, 500));
            frame.Add(MirAction.Walking, new Frame(56, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(120, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(184, 9, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(256, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(312, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(328, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(335, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(328, 8, 0, 100) { Reverse = true });

            //187 - DeathCrawler 死灵 (已核对)
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 7, 0, 100, 248, 3, 0, 200));
            frame.Add(MirAction.Attack2, new Frame(96, 7, 0, 100, 272, 4, 0, 200));
            frame.Add(MirAction.Struck, new Frame(152, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(184, 1, 8, 1000));
            //隐身，显现出来,参考食人花
            frame.Add(MirAction.Show, new Frame(313, 11, -11, 100) { Reverse = true });
            frame.Add(MirAction.Appear, new Frame(313, 11, 0, 100) { Reverse = true });
            frame.Add(MirAction.Hide, new Frame(313, 11, -11, 100));


            //188 - UndeadWolf //Dupe of 126 寒冰狼
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 8, 0, 100));
            frame.Add(MirAction.Struck, new Frame(144, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(168, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(177, 1, 9, 1000));

            //189 - BurningZombie //FrozenZombie ，火焰僵尸，暴雪僵尸
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 8, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 7, 0, 100, 312, 5, 0, 100));
            frame.Add(MirAction.Struck, new Frame(152, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(185, 1, 9, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(256, 7, 0, 100, 352, 6, -6, 200));//fozzesrange

            //190 - MudZombie 
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 10, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(128, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(176, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(200, 6, 0, 100));
            frame.Add(MirAction.Dead, new Frame(205, 1, 5, 1000));
            frame.Add(MirAction.Attack2, new Frame(248, 1, 7, 1000));

            //191 - BloodBaboon 血狒狒
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(96, 7, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(240, 9, 0, 100));
            frame.Add(MirAction.Attack3, new Frame(312, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(152, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(183, 1, 7, 1000));

            //192 - FightingCat, PoisonHugger
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(151, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(144, 8, 0, 100) { Reverse = true });

            //193 - FireCat 火焰灵猫(已调整)
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 5, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(120, 6, 0, 100, 248, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(168, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(184, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(191, 1, 7, 1000));

            //194 - CatWidow 长枪灵猫,已调整
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(128, 6, 0, 100, 256, 3, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(176, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(192, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(199, 1, 7, 1000));

            //195 - StainHammerCat 铁锤猫卫
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 7, 0, 100, 240, 4, 0, 100));
            frame.Add(MirAction.Struck, new Frame(136, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(160, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(169, 1, 9, 1000));

            //196 - BlackHammerCat 黑镐猫卫
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 7, 0, 100, 655, 4, 7, 200));
            frame.Add(MirAction.Attack2, new Frame(136, 12, 0, 100, 648, 11, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(232, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(256, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(265, 1, 9, 1000));

            //197 - StrayCat 双刃猫卫,已调整
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            //frame.Add(MirAction.Walking, new Frame(32, 10, 0, 200));
            frame.Add(MirAction.Walking, new Frame(112, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(160, 10, 0, 100, 528, 7, 0, 100) );
            frame.Add(MirAction.Attack2, new Frame(240, 10, 0, 100, 632, 12, 0, 100) );
            //frame.Add(MirAction.Attack2, new Frame(320, 13, 0, 200));
            frame.Add(MirAction.Struck, new Frame(424, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(448, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(457, 1, 9, 1000));

            //198 - CatShaman 灵猫法师
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 10, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(112, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(160, 7, 0, 200));
            //frame.Add(MirAction.Attack2, new Frame(216, 7, 0, 200));
            frame.Add(MirAction.AttackRange1, new Frame(160, 7, 0, 200));
            frame.Add(MirAction.Struck, new Frame(272, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(288, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(296, 1, 8, 1000));

            //199 - Jar2
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 500));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(128, 10, 0, 200));
            frame.Add(MirAction.Struck, new Frame(208, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(232, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(241, 1, 9, 1000));

            //200 - RestlessJar（有问题，已改）
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 300));
            frame.Add(MirAction.AttackRange2, new Frame(48, 7, 0, 100, 384, 7, -7, 100));
            frame.Add(MirAction.AttackRange1, new Frame(152, 7, 0, 200, 471, 5, -5, 200));
            frame.Add(MirAction.Struck, new Frame(108, 6, 0, 200));
            frame.Add(MirAction.Walking, new Frame(208, 8, 0, 200));
            frame.Add(MirAction.Die, new Frame(304, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(313, 1, 9, 1000));

            //201 - FlamingMutant, FlyingStatue, ManectricClaw
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Add(MirAction.AttackRange1, new Frame(224, 10, 0, 100));
            frame.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //202 - StoningStatue
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(304, 20, 0, 100));
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Add(MirAction.Die, new Frame(144, 20, 0, 100));
            frame.Add(MirAction.Dead, new Frame(163, 1, 19, 1000));

            //203 - ArcherGuard2
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 3, 100));
            frame.Add(MirAction.AttackRange1, new Frame(104, 10, 3, 100));
            frame.Add(MirAction.Struck, new Frame(208, 3, 0, 100));
            frame.Add(MirAction.Die, new Frame(232, 4, 0, 200));
            frame.Add(MirAction.Dead, new Frame(235, 1, 3, 1000));

            //204 - Tornado
            FrameList.Add(frame = new FrameSet());
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500) { Blend = true });
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100) { Blend = true });
            frame.Add(MirAction.Attack1, new Frame(80, 6, 0, 100) { Blend = true });
            frame.Add(MirAction.AttackRange1, new Frame(116, 8, 0, 100) { Blend = true });
            frame.Add(MirAction.Struck, new Frame(128, 2, 0, 200) { Blend = true });
            frame.Add(MirAction.Die, new Frame(200, 9, 0, 100) { Blend = true });
            frame.Add(MirAction.Dead, new Frame(271, 1, 9, 1000) { Blend = true });
            frame.Add(MirAction.Revive, new Frame(208, 9, 0, 100) { Blend = true, Reverse = true });

            //自定义 Monster403 紫花仙子 205
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 9, 0, 300));
            frame.Add(MirAction.Walking, new Frame(72, 7, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(184, 8, 0, 100, 436, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(128, 7, 0, 100, 328, 3, -3, 200) );
            frame.Add(MirAction.Struck, new Frame(248, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(272, 7, 0, 100));
            frame.Add(MirAction.Dead, new Frame(278, 1, 6, 1000));
            frame.Add(MirAction.Revive, new Frame(272, 7, 0, 100) { Reverse = true });



            //自定义 Monster404 冰焰鼠206
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 7, 0, 300));
            frame.Add(MirAction.Walking, new Frame(56, 6, 0, 100));
            //1.普通攻击
            frame.Add(MirAction.Attack1, new Frame(104, 7, 0, 100, 256, 4, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(160, 5, 0, 200));
            frame.Add(MirAction.Die, new Frame(200, 7, 0, 100, 288, 8, -8, 100));
            frame.Add(MirAction.Dead, new Frame(206, 1, 6, 1000));
            frame.Add(MirAction.Revive, new Frame(200, 7, 0, 100) { Reverse = true });


            //自定义 Monster405 冰蜗牛207
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 7, 0, 300));
            frame.Add(MirAction.Walking, new Frame(56, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(104, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(168, 10, 0, 100, 344, 5, 0, 200));
            frame.Add(MirAction.Struck, new Frame(248, 5, 0, 100));
            frame.Add(MirAction.Die, new Frame(288, 7, 0, 100));
            frame.Add(MirAction.Dead, new Frame(294, 1, 6, 1000));
            frame.Add(MirAction.Revive, new Frame(288, 7, 0, 100) { Reverse = true });

            //自定义 Monster406 冰宫战士208
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 8, 0, 100));
            //-素材有问题，后面几张错误了,先屏蔽掉了
            //frame.Add(MirAction.Attack1, new Frame(96, 9, 0, 100, 320, 5, 0, 100) );
            frame.Add(MirAction.Attack1, new Frame(96, 9, 0, 100) );
            frame.Add(MirAction.Attack2, new Frame(168, 7, 0, 100, 360, 5, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(224, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(248, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(256, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(248, 9, 0, 100) { Reverse = true });



            //自定义 Monster407 冰宫射手209
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.AttackRange2, new Frame(128, 7, 0, 100));
            frame.Add(MirAction.Struck, new Frame(232, 4, 0, 100));
            frame.Add(MirAction.Die, new Frame(264, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(273, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(264, 10, 0, 100) { Reverse = true });


            //自定义 Monster408 冰宫护卫210
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 400));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100));
            frame.Add(MirAction.Attack2, new Frame(160, 8, 0, 100, 325, 4, 0, 100));
            frame.Add(MirAction.Struck, new Frame(224, 5, 0, 100));
            frame.Add(MirAction.Die, new Frame(264, 7, 0, 100));
            frame.Add(MirAction.Dead, new Frame(270, 1, 6, 1000));
            frame.Add(MirAction.Revive, new Frame(264, 7, 0, 100) { Reverse = true });






            //自定义 Monster409 红花仙子211
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 8, 0, 300));
            frame.Add(MirAction.Walking, new Frame(64, 6, 0, 100));
            //1.普通攻击
            frame.Add(MirAction.Attack1, new Frame(112, 8, 0, 100, 556, 3, 0, 200));
            //2.抖花粉
            frame.Add(MirAction.Attack2, new Frame(224, 9, 0, 100, 587, 10, -10, 100));
            //3.净化
            frame.Add(MirAction.Attack3, new Frame(296, 7, 0, 100, 605, 10, -10, 100) );
            //远程1
            frame.Add(MirAction.AttackRange1, new Frame(176, 6, 0, 100, 464, 3, 0, 200) );
            //远程2
            frame.Add(MirAction.AttackRange2, new Frame(296, 6, 0, 100, 597, 8, 0, 100) );

            frame.Add(MirAction.Struck, new Frame(352, 5, 0, 200));
            frame.Add(MirAction.Die, new Frame(392, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(400, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(392, 9, 0, 100) { Reverse = true });


            //自定义 Monster410 冰宫鼠卫212
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 400));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(96, 7, 0, 100));
            frame.Add(MirAction.AttackRange2, new Frame(152, 9, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(224, 6, 0, 100, 414, 6, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(272, 6, 0, 100));
            frame.Add(MirAction.Die, new Frame(320, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(327, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(320, 8, 0, 100) { Reverse = true });


            //自定义 Monster411 冰宫骑士213
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 5, 0, 400));
            frame.Add(MirAction.Walking, new Frame(40, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(88, 6, 0, 100, 304, 6, -6, 100) );
            frame.Add(MirAction.AttackRange1, new Frame(136, 7, 0, 100, 310, 4, 0, 100));
            frame.Add(MirAction.Struck, new Frame(192, 4, 0, 100));
            frame.Add(MirAction.Die, new Frame(224, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(233, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(224, 10, 0, 100) { Reverse = true });


            //自定义 Monster412 冰宫刀卫214
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 400));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(96, 7, 0, 100, 296, 3, 0, 100) );
            frame.Add(MirAction.AttackRange2, new Frame(176, 4, 0, 100, 350, 3, 0, 100));
            //这个是瞬移的特效，瞬间接近目标，类似洪洞的怪
            frame.Add(MirAction.Attack1, new Frame(152, 3, 0, 200, 326, 3, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(208, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(232, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(239, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(232, 8, 0, 100) { Reverse = true });


            //自定义 Monster413 冰宫护法215
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 400));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            //1.普通攻击
            frame.Add(MirAction.Attack1, new Frame(96, 6, 0, 100, 384, 4, 0, 100) );
            //2.砸地板
            frame.Add(MirAction.Attack2, new Frame(144, 8, 0, 100, 422, 6, 0, 100));
            //3.远程冰冻
            //frame.Add(MirAction.Attack3, new Frame(208, 9, 0, 100,496,8,-8,100));
            frame.Add(MirAction.AttackRange1, new Frame(208, 9, 0, 100));
            //4.恢复护盾
            frame.Add(MirAction.Attack3, new Frame(208, 9, 0, 100, 485, 11, -11, 100) );
            //5.护盾破灭
            frame.Add(MirAction.Attack4, new Frame(208, 9, 0, 100, 504, 8, -8, 100) );
            frame.Add(MirAction.Struck, new Frame(280, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(304, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(313, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(304, 10, 0, 100) { Reverse = true });



            //自定义 Monster414 冰宫画卷216
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 5, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(72, 6, 0, 100, 296, 8, -8, 100) );
            frame.Add(MirAction.Attack2, new Frame(120, 9, 0, 100, 304, 10, -10, 100) );
            frame.Add(MirAction.Struck, new Frame(192, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(224, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(232, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(224, 9, 0, 100) { Reverse = true });


            //自定义 Monster415 冰宫画卷217
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 5, 0, 200));
            frame.Add(MirAction.Attack1, new Frame(72, 6, 0, 100, 272, 10, -10, 100) );
            frame.Add(MirAction.Attack2, new Frame(120, 9, 0, 100, 282, 11, -11, 100) );
            frame.Add(MirAction.Struck, new Frame(192, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(224, 6, 0, 100));
            frame.Add(MirAction.Dead, new Frame(229, 1, 5, 1000));
            frame.Add(MirAction.Revive, new Frame(224, 6, 0, 100) { Reverse = true });



            //自定义 Monster416 冰宫画卷218
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 5, 0, 200));
            frame.Add(MirAction.AttackRange1, new Frame(72, 6, 0, 100, 296, 4, -4, 100) );
            frame.Add(MirAction.Attack2, new Frame(120, 9, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(192, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(224, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(232, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(224, 9, 0, 100) { Reverse = true });



            //自定义 Monster417 冰宫画卷219
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Add(MirAction.Walking, new Frame(32, 5, 0, 200));
            frame.Add(MirAction.AttackRange1, new Frame(72, 6, 0, 100, 296, 3, -3, 100) );
            frame.Add(MirAction.Attack2, new Frame(120, 9, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(192, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(224, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(232, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(224, 9, 0, 100) { Reverse = true });


            //自定义 Monster418 冰宫学者220
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 8, 0, 500));
            frame.Add(MirAction.Walking, new Frame(64, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(112, 9, 0, 100, 352, 12, -12, 100) );
            frame.Add(MirAction.AttackRange1, new Frame(184, 9, 0, 100, 364, 10, -10, 100) );
            frame.Add(MirAction.Struck, new Frame(256, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(280, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(288, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(280, 9, 0, 100) { Reverse = true });


            //自定义 Monster419 冰宫巫师221
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(152, 7, 0, 100, 427, 10, -10, 100) );
            frame.Add(MirAction.AttackRange1, new Frame(96, 7, 0, 100, 368, 3, 0, 200));
            frame.Add(MirAction.AttackRange2, new Frame(208, 7, 0, 100, 399, 8, -8, 100) );
            frame.Add(MirAction.Struck, new Frame(264, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(297, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(288, 10, 0, 100) { Reverse = true });



            //自定义 Monster420 冰宫祭师222
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 7, 0, 100, 360, 4, 0, 100) );
            frame.Add(MirAction.Attack2, new Frame(152, 7, 0, 100, 398, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(208, 6, 0, 100, 457, 6, -6, 100));
            frame.Add(MirAction.Struck, new Frame(256, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(280, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(289, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(280, 10, 0, 100) { Reverse = true });


            //自定义 Monster421 冰女223
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 8, 0, 200));
            frame.Add(MirAction.Walking, new Frame(64, 6, 0, 100));
            //frame.Add(MirAction.Attack1, new Frame(112, 7, 0, 100,424,5,0,100,637,8,-8,100) { EffectStartTime = 200, ETime2=400 });
            frame.Add(MirAction.Attack1, new Frame(112, 7, 0, 100, 424, 5, 0, 100) );
            frame.Add(MirAction.Attack2, new Frame(168, 10, 0, 100, 471, 10, 0, 100) );
            //frame.Add(MirAction.Attack3, new Frame(248, 8, 0, 100,551,8,0,100, 645, 7, 0, 100) { EffectStartTime = 100, ETime2 = 200 });
            //frame.Add(MirAction.Attack3, new Frame(248, 8, 0, 100,  645, 7, 0, 100) );
            frame.Add(MirAction.Attack3, new Frame(248, 8, 0, 100, 551, 8, 0, 100) );
            frame.Add(MirAction.Struck, new Frame(312, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(344, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(353, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(344, 10, 0, 100) { Reverse = true });


            //自定义 Monster424 昆仑虎 2种攻击手段224
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 336, 8, 0, 100) );//无敌
            frame.Add(MirAction.AttackRange1, new Frame(160, 9, 0, 100, 400, 10, 0, 100));//无敌
            frame.Add(MirAction.Struck, new Frame(232, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(256, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(265, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(256, 10, 0, 100) { Reverse = true });


            //自定义 Monster425 部落道士 3种攻击手段225
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            //frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 484, 18, -18, 100) );//无敌
            //frame.Add(MirAction.Attack3, new Frame(160, 8, 0, 100, 464, 16, -16, 100));//范围减速，范围攻击，范围治疗
            frame.Add(MirAction.Attack1, new Frame(160, 8, 0, 100, 484, 18, -18, 100) );//无敌
            frame.Add(MirAction.Attack2, new Frame(288, 10, 0, 100, 464, 16, -16, 100));//范围减速，范围攻击，范围治疗
            frame.Add(MirAction.AttackRange1, new Frame(224, 8, 0, 100, 532, 5, -5, 200));//远程噬血
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(368, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(392, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(400, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(392, 9, 0, 100) { Reverse = true });


            //自定义 Monster426 部落法师 3种攻击手段226
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 392, 6, 4, 100) );//普通攻击
            frame.Add(MirAction.Attack2, new Frame(224, 8, 0, 100, 500, 7, -7, 100) );//范围攻击
            frame.Add(MirAction.AttackRange1, new Frame(160, 8, 0, 100, 470, 5, -5, 100) );//远程访问攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(288, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(312, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(321, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(312, 10, 0, 100) { Reverse = true });


            //自定义 Monster427 部落刺客 4种攻击手段227
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 10, 0, 100, 432, 10, 0, 100) );//普通攻击
            frame.Add(MirAction.Attack2, new Frame(216, 5, 0, 100, 541, 4, -4, 200) );//范围AOE
            frame.Add(MirAction.Attack3, new Frame(256, 10, 0, 100, 531, 10, -10, 100) );//范围AOE2
            frame.Add(MirAction.AttackRange1, new Frame(176, 5, 0, 100) );//冰冻，麻痹
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(336, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(360, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(368, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(360, 9, 0, 100) { Reverse = true });


            //自定义 Monster428 老树盘根 2种攻击手段228
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(5, 1, 5, 500));
            frame.Add(MirAction.Show, new Frame(0, 6, 0, 200));
            frame.Add(MirAction.Hide, new Frame(0, 6, 0, 200) { Reverse = true });
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100));//普通攻击
            frame.Add(MirAction.AttackRange1, new Frame(160, 9, 0, 100, 336, 6, 0, 100) );//范围AOE
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(232, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(256, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(265, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(256, 10, 0, 100) { Reverse = true });


            //自定义 Monster429 昆仑道士 5种攻击手段229
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 394, 5, 5, 100) );//普通攻击
            frame.Add(MirAction.Attack2, new Frame(160, 8, 0, 100) );//加buf
            frame.Add(MirAction.AttackRange1, new Frame(224, 8, 0, 100) );//普通符
            frame.Add(MirAction.AttackRange2, new Frame(224, 8, 0, 100) );//减速符
            frame.Add(MirAction.AttackRange3, new Frame(224, 8, 0, 100) );//高攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(288, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(312, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(321, 1, 9, 1000));



            //自定义 Monster430 千面妖王 毒妖林小BOSS  3种攻击手段230
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 10, 0, 100, 354, 10, 0, 100) );//普通攻击
            frame.Add(MirAction.AttackRange1, new Frame(176, 10, 0, 100) );//冰
            frame.Add(MirAction.AttackRange2, new Frame(176, 10, 0, 100) );//火
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(256, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(280, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(288, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(280, 9, 0, 100) { Reverse = true });




            //自定义 Monster434 昆仑叛军法师 3种攻击手段231
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 385, 5, 5, 100) );//普通攻击
            frame.Add(MirAction.AttackRange1, new Frame(160, 8, 0, 100, 463, 6, -6, 100) );//放冰控
            frame.Add(MirAction.AttackRange2, new Frame(224, 8, 0, 100, 463, 6, -6, 100) );//放球控
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(288, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(312, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(320, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(312, 9, 0, 100) { Reverse = true });



            //自定义 Monster435 九尾狐狸 3种攻击手段232
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 7, 0, 100, 400, 7, 0, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange1, new Frame(152, 9, 0, 100,467, 6, 4, 100) );//
            frame.Add(MirAction.Attack2, new Frame(224, 10, 0, 100, 456, 11, -11, 100) );//
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(304, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(328, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(336, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(328, 9, 0, 100) { Reverse = true });



            //自定义 Monster436 昆仑叛军刺客 3种攻击手段233
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 419, 4, 6, 100) );//普通攻击
            frame.Add(MirAction.Attack2, new Frame(160, 10, 0, 100, 496, 8, -2, 100) );//普通攻击
            frame.Add(MirAction.Attack3, new Frame(240, 9, 0, 100, 574, 5, -5, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(312, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(336, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(345, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(336, 10, 0, 100) { Reverse = true });



            //自定义 Monster437 昆仑叛军和尚 3种攻击手段234
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 8, 0, 500));
            frame.Add(MirAction.Walking, new Frame(64, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(112, 8, 0, 100, 443, 3, 7, 100) );//普通攻击
            frame.Add(MirAction.Attack2, new Frame(176, 10, 0, 100, 518, 10, 0, 100) );//普通攻击
            frame.Add(MirAction.Attack3, new Frame(256, 10, 0, 100, 676, 10, 0, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(336, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(360, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(369, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(360, 10, 0, 100) { Reverse = true });


            //自定义 Monster438 盘蟹花 3种攻击手段235
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(5, 1, 5, 500));
            frame.Add(MirAction.Show, new Frame(0, 6, 0, 200));
            frame.Add(MirAction.Hide, new Frame(0, 6, 0, 200) { Reverse = true });
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(96, 9, 0, 100, 384, 5, 0, 100) );//普通攻击
            frame.Add(MirAction.AttackRange2, new Frame(168, 8, 0, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(232, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(256, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(265, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(256, 10, 0, 100) { Reverse = true });





            //自定义 Monster439 昆仑叛军武士 3种攻击手段236
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 384, 6, 0, 100));//普通攻击
            frame.Add(MirAction.Attack2, new Frame(160, 9, 0, 100, 432, 7, 0, 100));//普通攻击
            frame.Add(MirAction.Attack3, new Frame(232, 8, 0, 100, 536, 13, -13, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(296, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(320, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(327, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(320, 8, 0, 100) { Reverse = true });



            //自定义 Monster440 昆仑叛军射手 2种攻击手段237
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(96, 7, 0, 100));//普通攻击
            frame.Add(MirAction.AttackRange2, new Frame(152, 10, 0, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(232, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(256, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(263, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(256, 8, 0, 100) { Reverse = true });


            //自定义 Monster441 昆仑叛军战神 小BOSS 238
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 400, 3, 0, 100));//普通攻击
            frame.Add(MirAction.AttackRange1, new Frame(160, 10, 0, 100, 424, 8, -8, 100) );//普通攻击
            frame.Add(MirAction.AttackRange2, new Frame(240, 8, 0, 100, 438, 6, -6, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(304, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(336, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(343, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(336, 8, 0, 100) { Reverse = true });
 

            //自定义 Monster442 昆仑叛军箭神 小BOSS 239
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(96, 9, 0, 100));//普通攻击
            frame.Add(MirAction.AttackRange2, new Frame(168, 10, 0, 100, 496, 8, -8, 100));//普通攻击
            frame.Add(MirAction.AttackRange3, new Frame(248, 8, 0, 100, 504, 8, -8, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(312, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(336, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(343, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(336, 8, 0, 100) { Reverse = true });



            //自定义 Monster443 昆仑叛军道尊 小BOSS 240
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(96, 8, 0, 100, 400, 5, 0, 100) );//普通攻击
            frame.Add(MirAction.AttackRange2, new Frame(160, 8, 0, 100, 454, 6, 0, 100));//普通攻击
            frame.Add(MirAction.AttackRange3, new Frame(224, 9, 0, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(296, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(320, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(329, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(320, 10, 0, 100) { Reverse = true });


            //自定义 Monster444 昆仑叛军刺客 小BOSS 241
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 408, 5, 0, 100));//普通攻击
            frame.Add(MirAction.Attack2, new Frame(160, 9, 0, 100, 448, 6, 0, 100) );//普通攻击
            frame.Add(MirAction.Attack3, new Frame(232, 9, 0, 100, 496, 6, -6, 100));//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(304, 4, 0, 200));
            frame.Add(MirAction.Die, new Frame(336, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(344, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(336, 9, 0, 100) { Reverse = true });


            //自定义 Monster446 昆仑终极BOSS 242
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100));//普通攻击
            frame.Add(MirAction.Attack2, new Frame(160, 9, 0, 100) );//放火
            frame.Add(MirAction.AttackRange1, new Frame(232, 8, 0, 100));//远程攻击
            frame.Add(MirAction.Struck, new Frame(296, 3, 0, 200));
            frame.Add(MirAction.Attack5, new Frame(320, 19, 0, 100, 975, 15, 4, 100));//变身，升级
            //形态2的攻击

            frame.Add(MirAction.Die, new Frame(816, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(825, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(816, 10, 0, 100) { Reverse = true });




            //自定义 Monster447 毒妖花 孽火花 243
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 300));
            frame.Add(MirAction.Walking, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(128, 10, 0, 100, 312, 10, -10, 100));//普通攻击
            frame.Add(MirAction.Struck, new Frame(208, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(232, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(241, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(232, 10, 0, 100) { Reverse = true });


            //自定义 Monster448 毒妖花 孽冰花 244
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 10, 0, 300));
            frame.Add(MirAction.Walking, new Frame(80, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(128, 10, 0, 100, 312, 10, -10, 100));//普通攻击
            frame.Add(MirAction.Struck, new Frame(208, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(232, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(241, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(232, 10, 0, 100) { Reverse = true });


            //自定义 Monster449 毒妖武士 245
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 7, 0, 500));
            frame.Add(MirAction.Walking, new Frame(56, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(104, 10, 0, 100, 272, 9, -9, 100));//普通攻击
            frame.Add(MirAction.Attack2, new Frame(104, 10, 0, 100, 281, 10, -10, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(184, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(208, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(215, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(208, 8, 0, 100) { Reverse = true });


            //自定义 Monster450 毒妖射手 246
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.AttackRange1, new Frame(96, 7, 0, 100));//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(152, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(176, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(183, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(176, 8, 0, 100) { Reverse = true });


            //自定义 Monster451 碑石妖 毒妖林小BOSS 247
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 7, 0, 100, 392, 7, -7, 100) );//旋转
            frame.Add(MirAction.AttackRange1, new Frame(152, 9, 0, 100, 522, 5, 0, 100) );//普通攻击
            frame.Add(MirAction.AttackRange2, new Frame(224, 8, 0, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(288, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(312, 10, 0, 100));
            frame.Add(MirAction.Dead, new Frame(321, 1, 9, 1000));
            frame.Add(MirAction.Revive, new Frame(312, 10, 0, 100) { Reverse = true });


            //自定义 Monster452 多毒妖 毒妖林小BOSS 248
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100) );//普通攻击
            frame.Add(MirAction.Attack2, new Frame(160, 10, 0, 100, 407, 11, -11, 100) );//普通攻击
            frame.Add(MirAction.Attack3, new Frame(240, 8, 0, 100, 432, 9, -9, 100) );//普通攻击
            //frame.Add(MirAction.AttackRange2, new Frame(288, 10, 0, 100));
            frame.Add(MirAction.Struck, new Frame(304, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(328, 9, 0, 100));
            frame.Add(MirAction.Dead, new Frame(336, 1, 8, 1000));
            frame.Add(MirAction.Revive, new Frame(328, 9, 0, 100) { Reverse = true });


            //自定义 Monster453 巨斧妖 毒妖林小BOSS 249
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 500));
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 536, 6, 0, 100));//普通攻击
            frame.Add(MirAction.Attack2, new Frame(160, 9, 0, 100, 528, 8, -8, 100));//旋转
            frame.Add(MirAction.Running, new Frame(160, 9, 0, 100, 528, 8, -8, 100));//旋转，跑到某个点
            frame.Add(MirAction.Attack3, new Frame(232, 9, 0, 100) );//释放技能，举手
            frame.Add(MirAction.Attack4, new Frame(304, 10, 0, 100, 584, 8, 0, 100) );//开天斩
            frame.Add(MirAction.AttackRange1, new Frame(384, 8, 0, 100) );//飞虎头
            frame.Add(MirAction.Struck, new Frame(448, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(472, 7, 0, 100));
            frame.Add(MirAction.Dead, new Frame(478, 1, 6, 1000));
            frame.Add(MirAction.Revive, new Frame(472, 7, 0, 100) { Reverse = true });

             
            //自定义 Monster454 毒妖女皇 毒妖林小BOSS 250
            frame = new FrameSet();
            frame.Add(MirAction.Standing, new Frame(0, 6, 0, 300, 528, 6, 0, 300) );
            frame.Add(MirAction.Walking, new Frame(48, 6, 0, 100, 576, 6, 0, 100));
            frame.Add(MirAction.Attack1, new Frame(96, 8, 0, 100, 624, 8, 0, 100) );//普通攻击
            frame.Add(MirAction.Attack2, new Frame(160, 9, 0, 100, 688, 9, 0, 100));//脉冲，范围麻痹
            frame.Add(MirAction.Attack3, new Frame(232, 9, 0, 100, 760, 9, 0, 100) );//撒花,放鬼头
            frame.Add(MirAction.Attack4, new Frame(304, 9, 0, 100, 832, 9, 0, 100));//无敌，护盾
            frame.Add(MirAction.Attack5, new Frame(304, 9, 0, 100, 832, 9, 0, 100));//闪现，瞬移
            frame.Add(MirAction.AttackRange1, new Frame(376, 8, 0, 100, 904, 8, 0, 100) );//远程单体雷电，禁魔
            frame.Add(MirAction.Struck, new Frame(440, 3, 0, 200, 968, 3, 0, 200));
            frame.Add(MirAction.Die, new Frame(464, 8, 0, 100));
            frame.Add(MirAction.Dead, new Frame(471, 1, 7, 1000));
            frame.Add(MirAction.Revive, new Frame(464, 8, 0, 100) { Reverse = true });



            #endregion



            //ADD SWITCH OF IMAGE TO CORRECT FRAME (CAN BE COPIED FROM THE MONSTEROBJECT.CS FRAME LIST)
            FrameSet Frames = new FrameSet();
            switch (image)
            {
                case Monster.Guard:
                case Monster.Guard2:
                    Frames = FrameList[0];
                    break;
                case Monster.Hen:
                case Monster.Deer:
                case Monster.Sheep:
                case Monster.Wolf:
                case Monster.Pig:
                case Monster.Bull:
                case Monster.DarkBrownWolf:
                    Frames = FrameList[1];
                    break;
                case Monster.Scarecrow:
                case Monster.HookingCat:
                case Monster.RakingCat:
                case Monster.Yob:
                case Monster.Oma:
                case Monster.SpittingSpider:
                case Monster.OmaFighter:
                case Monster.OmaWarrior:
                case Monster.CaveBat:
                case Monster.Skeleton:
                case Monster.BoneFighter:
                case Monster.AxeSkeleton:
                case Monster.BoneWarrior:
                case Monster.BoneElite:
                case Monster.Dung:
                case Monster.Dark:
                case Monster.WoomaSoldier:
                case Monster.WoomaFighter:
                case Monster.WoomaWarrior:
                case Monster.FlamingWooma:
                case Monster.WoomaGuardian:
                case Monster.WoomaTaurus:
                case Monster.WhimperingBee:
                case Monster.GiantWorm:
                case Monster.Centipede:
                case Monster.BlackMaggot:
                case Monster.Tongs:
                case Monster.EvilTongs:
                case Monster.BugBat:
                case Monster.WedgeMoth:
                case Monster.RedBoar:
                case Monster.BlackBoar:
                case Monster.SnakeScorpion:
                case Monster.WhiteBoar:
                case Monster.EvilSnake:
                case Monster.SpiderBat:
                case Monster.VenomSpider:
                case Monster.GangSpider:
                case Monster.GreatSpider:
                case Monster.LureSpider:
                case Monster.BigApe:
                case Monster.EvilApe:
                case Monster.GrayEvilApe:
                case Monster.RedEvilApe:
                case Monster.BigRat:
                case Monster.ZumaArcher:
                case Monster.Ghoul:
                case Monster.KingHog:
                case Monster.Shinsu1:
                case Monster.SpiderFrog:
                case Monster.HoroBlaster:
                case Monster.BlueHoroBlaster:
                case Monster.KekTal:
                case Monster.VioletKekTal:
                case Monster.RoninGhoul:
                case Monster.ToxicGhoul:
                case Monster.BoneCaptain:
                case Monster.BoneSpearman:
                case Monster.BoneBlademan:
                case Monster.BoneArcher:
                case Monster.Minotaur:
                case Monster.IceMinotaur:
                case Monster.ElectricMinotaur:
                case Monster.WindMinotaur:
                case Monster.FireMinotaur:
                case Monster.ShellNipper:
                case Monster.Keratoid:
                case Monster.GiantKeratoid:
                case Monster.SkyStinger:
                case Monster.SandWorm:
                case Monster.VisceralWorm:
                case Monster.RedSnake:
                case Monster.TigerSnake:
                case Monster.GiantWhiteSnake:
                case Monster.BlueSnake:
                case Monster.YellowSnake:
                case Monster.AxeOma:
                case Monster.SwordOma:
                case Monster.WingedOma:
                case Monster.FlailOma:
                case Monster.OmaGuard:
                case Monster.KatanaGuard:
                case Monster.RedFrogSpider:
                case Monster.BrownFrogSpider:
                case Monster.HalloweenScythe:
                case Monster.GhastlyLeecher:
                case Monster.CyanoGhast:
                case Monster.RedTurtle:
                case Monster.GreenTurtle:
                case Monster.BlueTurtle:
                case Monster.TowerTurtle:
                case Monster.DarkTurtle:
                case Monster.DarkSwordOma:
                case Monster.DarkAxeOma:
                case Monster.DarkWingedOma:
                case Monster.BoneWhoo:
                case Monster.DarkSpider:
                case Monster.ViscusWorm:
                case Monster.ViscusCrawler:
                case Monster.CrawlerLave:
                case Monster.DarkYob:
                case Monster.ValeBat:
                case Monster.Weaver:
                case Monster.VenomWeaver:
                case Monster.CrackingWeaver:
                case Monster.ArmingWeaver:
                case Monster.SpiderWarrior:
                case Monster.SpiderBarbarian:
                    Frames = FrameList[2];
                    break;
                case Monster.CannibalPlant:
                    Frames = FrameList[3];
                    break;
                case Monster.ForestYeti:
                case Monster.CaveMaggot:
                case Monster.FrostYeti:
                    Frames = FrameList[4];
                    break;
                case Monster.Scorpion:
                    Frames = FrameList[5];
                    break;
                case Monster.ChestnutTree:
                case Monster.EbonyTree:
                case Monster.LargeMushroom:
                case Monster.CherryTree:
                case Monster.ChristmasTree:
                case Monster.SnowTree:
                    Frames = FrameList[6];
                    break;
                case Monster.EvilCentipede:
                    Frames = FrameList[7];
                    break;
                case Monster.BugBatMaggot:
                    Frames = FrameList[8];
                    break;
                case Monster.CrystalSpider:
                case Monster.WhiteFoxman:
                case Monster.LightTurtle:
                case Monster.CrystalWeaver:
                    Frames = FrameList[9];
                    break;
                case Monster.RedMoonEvil:
                    Frames = FrameList[10];
                    break;
                case Monster.ZumaStatue:
                case Monster.ZumaGuardian:
                case Monster.FrozenZumaStatue:
                case Monster.FrozenZumaGuardian:

                    Frames = FrameList[11];
                    break;
                case Monster.ZumaTaurus:

                    Frames = FrameList[12];
                    break;
                case Monster.RedThunderZuma:
                case Monster.FrozenRedZuma:

                    Frames = FrameList[13];
                    break;
                case Monster.KingScorpion:
                case Monster.DarkDevil:
                case Monster.RightGuard:
                case Monster.LeftGuard:
                case Monster.MinotaurKing:
                    Frames = FrameList[14];
                    break;
                case Monster.BoneFamiliar:
                    Frames = FrameList[15];

                    break;
                case Monster.Shinsu:
                    Frames = FrameList[16];

                    break;
                case Monster.DigOutZombie:
                    Frames = FrameList[17];
                    break;
                case Monster.ClZombie:
                case Monster.NdZombie:
                case Monster.CrawlerZombie:
                    Frames = FrameList[18];
                    break;
                case Monster.ShamanZombie:
                    Frames = FrameList[19];
                    break;
                case Monster.Khazard:
                case Monster.FinialTurtle:
                    Frames = FrameList[20];
                    break;
                case Monster.BoneLord:
                    Frames = FrameList[21];
                    break;
                case Monster.FrostTiger:
                case Monster.FlameTiger:

                    Frames = FrameList[22];
                    break;
                case Monster.Yimoogi:
                case Monster.RedYimoogi:
                case Monster.Snake10:
                case Monster.Snake11:
                case Monster.Snake12:
                case Monster.Snake13:
                case Monster.Snake14:
                case Monster.Snake15:
                case Monster.Snake16:
                case Monster.Snake17:
                    Frames = FrameList[23];
                    break;
                case Monster.HolyDeva:
                    Frames = FrameList[24];

                    break;
                case Monster.GreaterWeaver:
                case Monster.RootSpider:
                    Frames = FrameList[25];
                    break;
                case Monster.BombSpider:
                case Monster.MutatedHugger:
                    Frames = FrameList[26];
                    break;
                case Monster.CrossbowOma:
                case Monster.DarkCrossbowOma:
                    Frames = FrameList[27];
                    break;
                case Monster.YinDevilNode:
                case Monster.YangDevilNode:
                    Frames = FrameList[28];
                    break;
                case Monster.OmaKing:
                    Frames = FrameList[29];
                    break;
                case Monster.BlackFoxman:
                case Monster.RedFoxman:
                    Frames = FrameList[30];
                    break;
                case Monster.TrapRock:
                    Frames = FrameList[31];
                    break;
                case Monster.GuardianRock:
                    Frames = FrameList[32];
                    break;
                case Monster.ThunderElement:
                case Monster.CloudElement:
                    Frames = FrameList[33];
                    break;
                case Monster.GreatFoxSpirit:
                    Frames = FrameList[34];
                    break;
                case Monster.HedgeKekTal:
                case Monster.BigHedgeKekTal:
                    Frames = FrameList[39];
                    break;
                case Monster.EvilMir:
                    Frames = FrameList[40];
                    break;
                case Monster.DragonStatue:
                    Frames = FrameList[41];
                    break;
                case Monster.ArcherGuard:
                    Frames = FrameList[47];
                    break;
                case Monster.TaoistGuard:
                    Frames = FrameList[48];
                    break;
                case Monster.VampireSpider://SummonVampire
                    Frames = FrameList[49];
                    break;
                case Monster.SpittingToad://SummonToad
                    Frames = FrameList[50];
                    break;
                case Monster.SnakeTotem://SummonSnakes Totem
                    Frames = FrameList[51];
                    break;
                case Monster.CharmedSnake://SummonSnakes Snake
                    Frames = FrameList[52];
                    break;
                case Monster.HighAssassin:
                    Frames = FrameList[53];
                    break;
                case Monster.DarkDustPile:
                case Monster.MudPile:
                case Monster.Treasurebox:
                case Monster.SnowPile:
                    Frames = FrameList[54];
                    break;
                case Monster.Football:
                    Frames = FrameList[55];
                    break;
                case Monster.GingerBreadman:
                    Frames = FrameList[56];
                    break;
                case Monster.DreamDevourer:
                    Frames = FrameList[57];
                    break;
                case Monster.TailedLion:
                    Frames = FrameList[58];
                    break;
                case Monster.Behemoth:
                    Frames = FrameList[59];
                    break;
                case Monster.Hugger:
                case Monster.ManectricSlave:
                    Frames = FrameList[60];
                    break;
                case Monster.DarkDevourer:
                    Frames = FrameList[61];
                    break;
                case Monster.Snowman:
                    Frames = FrameList[62];
                    break;
                case Monster.GiantEgg:
                case Monster.IcePillar:
                    Frames = FrameList[63];
                    break;
                case Monster.BlueSanta:
                    Frames = FrameList[64];
                    break;
                case Monster.BattleStandard:
                    Frames = FrameList[65];
                    break;
                case Monster.WingedTigerLord:
                    Frames = FrameList[66];
                    break;
                case Monster.TurtleKing:
                    Frames = FrameList[67];
                    break;
                case Monster.Bush:
                    Frames = FrameList[68];
                    break;
                case Monster.HellSlasher:
                case Monster.HellCannibal:
                case Monster.ManectricClub:
                    Frames = FrameList[69];
                    break;
                case Monster.HellPirate:
                    Frames = FrameList[70];
                    break;
                case Monster.HellBolt:
                case Monster.WitchDoctor:
                    Frames = FrameList[71];
                    break;
                case Monster.HellKeeper:
                    Frames = FrameList[72];
                    break;
                case Monster.ManectricHammer:
                    Frames = FrameList[73];
                    break;
                case Monster.ManectricStaff:
                    Frames = FrameList[74];
                    break;
                case Monster.NamelessGhost:
                case Monster.DarkGhost:
                case Monster.ChaosGhost:
                case Monster.ManectricBlest:
                case Monster.TrollHammer:
                case Monster.TrollBomber:
                case Monster.TrollStoner:
                case Monster.MutatedManworm:
                case Monster.CrazyManworm:
                    Frames = FrameList[75];
                    break;
                case Monster.ManectricKing:
                case Monster.TrollKing:
                    Frames = FrameList[76];
                    break;
                case Monster.FlameSpear:
                case Monster.FlameMage:
                case Monster.FlameScythe:
                case Monster.FlameAssassin:
                    Frames = FrameList[77];
                    break;
                case Monster.FlameQueen:
                    Frames = FrameList[78];
                    break;
                case Monster.HellKnight1:
                case Monster.HellKnight2:
                case Monster.HellKnight3:
                case Monster.HellKnight4:
                    Frames = FrameList[79];

                    break;
                case Monster.HellLord:
                    Frames = FrameList[80];
                    break;
                case Monster.WaterGuard:
                    Frames = FrameList[81];
                    break;
                case Monster.IceGuard:
                case Monster.ElementGuard:
                    Frames = FrameList[82];
                    break;
                case Monster.DemonGuard:
                    Frames = FrameList[83];
                    break;
                case Monster.KingGuard:
                    Frames = FrameList[84];
                    break;
                case Monster.Bunny2:
                case Monster.Bunny:
                    Frames = FrameList[85];
                    break;
                case Monster.DarkBeast:
                case Monster.LightBeast:
                    Frames = FrameList[86];
                    break;
                case Monster.HardenRhino:
                    Frames = FrameList[87];
                    break;
                case Monster.AncientBringer:
                    Frames = FrameList[88];
                    break;
                case Monster.Jar1:
                    Frames = FrameList[89];
                    break;
                case Monster.SeedingsGeneral:
                    Frames = FrameList[90];
                    break;
                case Monster.Tucson:
                case Monster.TucsonFighter:
                    Frames = FrameList[91];
                    break;
                case Monster.FrozenDoor:
                    Frames = FrameList[92];
                    break;
                case Monster.TucsonMage:
                    Frames = FrameList[93];
                    break;
                case Monster.TucsonWarrior:
                    Frames = FrameList[94];
                    break;
                case Monster.Armadillo:
                    Frames = FrameList[95];
                    break;
                case Monster.ArmadilloElder:
                    Frames = FrameList[96];
                    break;
                case Monster.TucsonEgg:
                    Frames = FrameList[97];
                    break;
                case Monster.PlaguedTucson:
                    Frames = FrameList[98];
                    break;
                case Monster.SandSnail:
                    Frames = FrameList[99];
                    break;
                case Monster.CannibalTentacles:
                    Frames = FrameList[100];
                    break;
                case Monster.TucsonGeneral:
                    Frames = FrameList[101];
                    break;
                case Monster.GasToad:
                    Frames = FrameList[102];
                    break;
                case Monster.Mantis:
                    Frames = FrameList[103];
                    break;
                case Monster.SwampWarrior:
                    Frames = FrameList[104];
                    break;
                case Monster.AssassinBird:
                    Frames = FrameList[105];
                    break;
                case Monster.RhinoWarrior:
                    Frames = FrameList[106];
                    break;
                case Monster.RhinoPriest:
                    Frames = FrameList[107];
                    break;
                case Monster.SwampSlime:
                    Frames = FrameList[108];
                    break;
                case Monster.RockGuard:
                    Frames = FrameList[109];
                    break;
                case Monster.MudWarrior:
                    Frames = FrameList[110];
                    break;
                case Monster.SmallPot:
                    Frames = FrameList[111];
                    break;
                case Monster.TreeQueen:
                    Frames = FrameList[112];
                    break;
                case Monster.ShellFighter:
                    Frames = FrameList[113];
                    break;
                case Monster.DarkBaboon:
                    Frames = FrameList[114];
                    break;
                case Monster.TwinHeadBeast:
                    Frames = FrameList[115];
                    break;
                case Monster.OmaCannibal:
                    Frames = FrameList[116];
                    break;

                case Monster.OmaSlasher:
                    Frames = FrameList[117];
                    break;
                case Monster.OmaAssassin:
                    Frames = FrameList[118];
                    break;
                case Monster.OmaMage:
                    Frames = FrameList[119];
                    break;
                case Monster.OmaWitchDoctor:
                    Frames = FrameList[120];
                    break;
                case Monster.OmaBlest:
                    Frames = FrameList[121];
                    break;
                case Monster.LightningBead:
                case Monster.HealingBead:
                case Monster.PowerUpBead:
                    Frames = FrameList[122];
                    break;
                case Monster.DarkOmaKing:
                    Frames = FrameList[123];
                    break;
                case Monster.CaveMage:
                    Frames = FrameList[124];
                    break;
                case Monster.Mandrill:
                    Frames = FrameList[125];
                    break;
                case Monster.PlagueCrab:
                    Frames = FrameList[126];
                    break;
                case Monster.CreeperPlant:
                    Frames = FrameList[127];
                    break;
                case Monster.SackWarrior:
                    Frames = FrameList[128];
                    break;
                case Monster.WereTiger:
                    Frames = FrameList[129];
                    break;
                case Monster.KingHydrax:
                    Frames = FrameList[130];
                    break;
                case Monster.FloatingWraith:
                    Frames = FrameList[131];
                    break;
                case Monster.ArmedPlant:
                    Frames = FrameList[132];
                    break;
                case Monster.AvengerPlant:
                    Frames = FrameList[133];
                    break;
                case Monster.Nadz:
                case Monster.AvengingSpirit:
                    Frames = FrameList[134];
                    break;
                case Monster.AvengingWarrior:
                    Frames = FrameList[135];
                    break;
                case Monster.AxePlant:
                case Monster.ClawBeast:
                    Frames = FrameList[136];
                    break;
                case Monster.WoodBox:
                    Frames = FrameList[137];
                    break;
                case Monster.KillerPlant:
                    Frames = FrameList[138];
                    break;
                case Monster.Hydrax:
                    Frames = FrameList[139];
                    break;
                case Monster.Basiloid:
                    Frames = FrameList[140];
                    break;
                case Monster.HornedMage:
                    Frames = FrameList[141];
                    break;
                case Monster.HornedArcher:
                case Monster.ColdArcher:
                    Frames = FrameList[142];
                    break;
                case Monster.HornedWarrior:
                    Frames = FrameList[143];
                    break;
                case Monster.FloatingRock:
                    Frames = FrameList[144];
                    break;
                case Monster.ScalyBeast:
                    Frames = FrameList[145];
                    break;
                case Monster.HornedSorceror:
                    Frames = FrameList[146];
                    break;
                case Monster.BoulderSpirit:
                    Frames = FrameList[147];
                    break;
                case Monster.HornedCommander:
                    Frames = FrameList[148];
                    break;
                case Monster.MoonStone:
                case Monster.SunStone:
                case Monster.LightningStone:
                    Frames = FrameList[149];
                    break;
                case Monster.Turtlegrass:
                    Frames = FrameList[150];
                    break;
                case Monster.Mantree:
                    Frames = FrameList[151];
                    break;
                case Monster.Bear:
                    Frames = FrameList[152];
                    break;
                case Monster.Leopard:
                    Frames = FrameList[153];
                    break;
                case Monster.ChieftainArcher:
                    Frames = FrameList[154];
                    break;
                case Monster.ChieftainSword:
                    Frames = FrameList[155];
                    break;
                case Monster.StoningSpider: //StoneTrap
                    Frames = FrameList[156];
                    break;
                case Monster.DarkSpirit:
                case Monster.FrozenSoldier:
                    Frames = FrameList[157];
                    break;
                case Monster.FrozenFighter:
                    Frames = FrameList[158];
                    break;
                case Monster.FrozenArcher:
                    Frames = FrameList[159];
                    break;
                case Monster.FrozenKnight:
                    Frames = FrameList[160];
                    break;
                case Monster.FrozenGolem:
                    Frames = FrameList[161];
                    break;
                case Monster.IcePhantom:
                    Frames = FrameList[162];
                    break;
                case Monster.SnowWolf:
                    Frames = FrameList[163];
                    break;
                case Monster.SnowWolfKing:
                    Frames = FrameList[164];
                    break;
                case Monster.WaterDragon:
                    Frames = FrameList[165];
                    break;
                case Monster.BlackTortoise:
                    Frames = FrameList[166];
                    break;
                case Monster.Manticore:
                    Frames = FrameList[167];
                    break;
                case Monster.DragonWarrior:
                    Frames = FrameList[168];
                    break;
                case Monster.DragonArcher:
                    Frames = FrameList[169];
                    break;
                case Monster.Kirin:
                    Frames = FrameList[170];
                    break;
                case Monster.Guard3:
                    Frames = FrameList[171];
                    break;
                case Monster.ArcherGuard3:
                    Frames = FrameList[172];
                    break;

                //173 blank

                case Monster.FrozenMiner:
                    Frames = FrameList[174];
                    break;
                case Monster.FrozenAxeman:
                    Frames = FrameList[175];
                    break;
                case Monster.FrozenMagician:
                    Frames = FrameList[176];
                    break;
                case Monster.SnowYeti:
                    Frames = FrameList[177];
                    break;
                case Monster.IceCrystalSoldier:
                    Frames = FrameList[178];
                    break;
                case Monster.DarkWraith:
                    Frames = FrameList[179];
                    break;
                case Monster.CrystalBeast:
                    Frames = FrameList[180];
                    break;
                case Monster.RedOrb:
                case Monster.BlueOrb:
                case Monster.YellowOrb:
                case Monster.GreenOrb:
                case Monster.WhiteOrb:
                    Frames = FrameList[181];
                    break;
                case Monster.FatalLotus:
                    Frames = FrameList[182];
                    break;
                case Monster.AntCommander:
                    Frames = FrameList[183];
                    break;
                case Monster.CargoBoxwithlogo:
                case Monster.CargoBox:
                    Frames = FrameList[184];
                    break;
                case Monster.Doe:
                    Frames = FrameList[185];
                    break;
                case Monster.AngryReindeer:
                    Frames = FrameList[186];
                    break;
                case Monster.DeathCrawler:
                    Frames = FrameList[187];
                    break;
                case Monster.UndeadWolf:
                    Frames = FrameList[188];
                    break;
                case Monster.BurningZombie:
                case Monster.FrozenZombie://
                    Frames = FrameList[189];
                    break;
                case Monster.MudZombie:
                    Frames = FrameList[190];
                    break;
                case Monster.BloodBaboon:
                    Frames = FrameList[191];
                    break;
                case Monster.PoisonHugger:
                    Frames = FrameList[192];
                    break;
                case Monster.FireCat:
                    Frames = FrameList[193];
                    break;
                case Monster.CatWidow:
                    Frames = FrameList[194];
                    break;
                case Monster.StainHammerCat:
                    Frames = FrameList[195];
                    break;
                case Monster.BlackHammerCat:
                    Frames = FrameList[196];
                    break;
                case Monster.StrayCat:
                    Frames = FrameList[197];
                    break;
                case Monster.CatShaman:
                    Frames = FrameList[198];
                    break;
                case Monster.Jar2:
                    Frames = FrameList[199];
                    break;
                case Monster.RestlessJar:
                    Frames = FrameList[200];
                    break;
                case Monster.FlamingMutant:
                case Monster.FlyingStatue:
                case Monster.ManectricClaw:
                    Frames = FrameList[201];
                    break;
                case Monster.StoningStatue:
                    Frames = FrameList[202];
                    break;
                case Monster.ArcherGuard2:
                    Frames = FrameList[203];
                    break;
                case Monster.Tornado:
                    Frames = FrameList[204];
                    break;


                case Monster.PurpleFaeFlower:
                    Frames = FrameList[205];
                    break;
                case Monster.Furball:
                    Frames = FrameList[206];
                    break;
                case Monster.GlacierSnail:
                    Frames = FrameList[207];
                    break;
                case Monster.FurbolgWarrior:
                    Frames = FrameList[208];
                    break;
                case Monster.FurbolgArcher:
                    Frames = FrameList[209];
                    break;
                case Monster.FurbolgCommander:
                    Frames = FrameList[210];
                    break;
                case Monster.RedFaeFlower:
                    Frames = FrameList[211];
                    break;
                case Monster.FurbolgGuard:
                    Frames = FrameList[212];
                    break;
                case Monster.GlacierBeast:
                    Frames = FrameList[213];
                    break;
                case Monster.GlacierWarrior:
                    Frames = FrameList[214];
                    break;
                case Monster.ShardGuardian:
                    Frames = FrameList[215];
                    break;
                case Monster.WarriorScroll:
                    Frames = FrameList[216];
                    break;

                case Monster.TaoistScroll:
                    Frames = FrameList[217];
                    break;

                case Monster.WizardScroll:
                    Frames = FrameList[218];
                    break;

                case Monster.AssassinScroll:
                    Frames = FrameList[219];
                    break;

                case Monster.HoodedSummoner:
                    Frames = FrameList[220];
                    break;

                case Monster.HoodedIceMage:
                    Frames = FrameList[221];
                    break;

                case Monster.HoodedPriest:
                    Frames = FrameList[222];
                    break;
                case Monster.ShardMaiden:
                    Frames = FrameList[223];
                    break;
                case Monster.KingKong:
                    Frames = FrameList[223];
                    break;
                case Monster.WarBear:
                    Frames = FrameList[224];
                    break;
                case Monster.ReaperPriest:
                    Frames = FrameList[225];
                    break;
                case Monster.ReaperWizard:
                    Frames = FrameList[226];
                    break;
                case Monster.ReaperAssassin:
                    Frames = FrameList[227];
                    break;
                case Monster.LivingVines:
                    Frames = FrameList[228];
                    break;
                case Monster.BlueMonk:
                    Frames = FrameList[230];
                    break;
                case Monster.MutantBeserker:
                    Frames = FrameList[231];
                    break;
                case Monster.MutantGuardian:
                    Frames = FrameList[232];
                    break;
                case Monster.MutantHighPriest:
                    Frames = FrameList[233];
                    break;
                case Monster.MysteriousMage:
                    Frames = FrameList[234];
                    break;
                case Monster.FeatheredWolf:
                    Frames = FrameList[235];
                    break;
                case Monster.MysteriousAssassin:
                    Frames = FrameList[236];
                    break;
                case Monster.MysteriousMonk:
                    Frames = FrameList[237];
                    break;
                case Monster.ManEatingPlant:
                    Frames = FrameList[238];
                    break;
                case Monster.HammerDwarf:
                    Frames = FrameList[239];
                    break;
                case Monster.ArcherDwarf:
                    Frames = FrameList[240];
                    break;
                case Monster.NobleWarrior:
                    Frames = FrameList[241];
                    break;
                case Monster.NobleArcher:
                    Frames = FrameList[242];
                    break;
                case Monster.NoblePriest:
                    Frames = FrameList[243];
                    break;
                case Monster.NobleAssassin:
                    Frames = FrameList[244];
                    break;
                case Monster.Swain:
                    Frames = FrameList[245];
                    break;
                case Monster.RedMutantPlant:
                    Frames = FrameList[246];
                    break;
                case Monster.BlueMutantPlant:
                    Frames = FrameList[247];
                    break;
                case Monster.UndeadHammerDwarf:
                    Frames = FrameList[248];
                    break;
                case Monster.UndeadDwarfArcher:
                    Frames = FrameList[249];
                    break;
                case Monster.AncientStoneGolem:
                    Frames = FrameList[250];
                    break;
                //case Monster.Serpentirian:

                //case Monster.Butcher:

                //case Monster.Riklebites:

                //case Monster.FeralTundraFurbolg:

                //case Monster.FeralFlameFurbolg:

                //case Monster.ArcaneTotem:

                //case Monster.SpectralWraith:

                //case Monster.BabyMagmaDragon:

                //case Monster.BloodLord:
                    
                    
                //case Monster.SerpentLord:

                //case Monster.MirEmperor:

                //case Monster.MutantManEatingPlant:

                //case Monster.MutantWarg:

                //case Monster.GrassElemental:

                //case Monster.RockElemental:

                //    Frames = FrameList[259];
                //    break;

            }

            return Frames;
        }
        #endregion
    }
}