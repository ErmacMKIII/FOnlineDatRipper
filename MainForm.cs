// Copyright (C) 2021 Alexander Stojanovich
//
// This file is part of FOnlineDatRipper.
//
// FOnlineDatRipper is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// FOnlineDatRipper is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with FOnlineDatRipper. If not, see http://www.gnu.org/licenses/.

namespace FOnlineDatRipper
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MainForm" />.
    /// </summary>
    internal partial class MainForm : Form
    {
        /// <summary>
        /// FOnline file list
        /// </summary>
        private List<FOnlineFile> fOnlineFiles = new List<FOnlineFile>();

        /// <summary>
        /// Currently processed FOnlineFile
        /// </summary>
        private FOnlineFile currFOFile;

        /// <summary>
        /// Selected FOnline file from the tree view
        /// </summary>
        private FOnlineFile selectedFOFile;                   

        /// <summary>
        /// Defines the DarkBackground.
        /// </summary>
        public static readonly Color DarkBackground = Color.FromArgb(0x1E, 0x1E, 0x1E);

        /// <summary>
        /// Defines the DarkForeground.
        /// </summary>
        public static readonly Color DarkForeground = Color.FromArgb(0xFF, 0x7F, 0x50);

        /// <summary>
        /// Defines the ClosedRootIndex.
        /// </summary>
        internal const int ClosedRootIndex = 0;

        /// <summary>
        /// Defines the OpenedRootIndex.
        /// </summary>
        internal const int OpenedRootIndex = 1;

        /// <summary>
        /// Defines the ClosedDirIndex.
        /// </summary>
        internal const int ClosedDirIndex = 2;

        /// <summary>
        /// Defines the OpenedDirIndex.
        /// </summary>
        internal const int OpenedDirIndex = 3;

        /// <summary>
        /// Defines the ACMIndex.
        /// </summary>
        internal const int ACMIndex = 4;

        /// <summary>
        /// Defines the FRMIndex.
        /// </summary>
        internal const int FRMIndex = 5;

        /// <summary>
        /// Defines the FileIndex.
        /// </summary>
        internal const int FileIndex = 6;

        /// <summary>
        /// Defines the input file.....
        /// </summary>
        private string[] inputFiles;

        /// <summary>
        /// Current input file in process
        /// </summary>
        private int currInFileIndex = 0;

        /// <summary>
        /// Defines the output directory to extract the data...........
        /// </summary>
        private string outDir;

        /// <summary>
        /// Defines the stopwatch.
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Defines the loading worker............
        /// </summary>
        private readonly BackgroundWorker reader = new BackgroundWorker();

        /// <summary>
        /// Defines the extractor worker............
        /// </summary>
        private readonly BackgroundWorker extractor = new BackgroundWorker();

        /// <summary>
        /// Defines the datCache.
        /// </summary>
        private ListViewItem[] datCache;

        /// <summary>
        /// Tells if cache miss has occurred.............
        /// </summary>
        private bool datCacheMiss = true;

        /// <summary>
        /// Defines the begin index of the item block and it's always 1000 in length.............
        /// </summary>
        private int datCacheIndex = 0;

        /// <summary>
        /// Defines the the dat list view items.............
        /// </summary>
        private readonly List<ListViewItem> datListViewItems = new List<ListViewItem>(2000);

        /// <summary>
        /// Defines the seconds.
        /// </summary>
        private double seconds = 0.0;

        /// <summary>
        /// Extract specific files.
        /// </summary>
        private readonly BackgroundWorker miniExtractor = new BackgroundWorker();

        /// <summary>
        /// List of specific files for extraction.
        /// </summary>
        // for extranction from dat
        private List<Node<string>> files4Extract = new List<Node<string>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            reader.DoWork += Reader_DoWork;
            reader.RunWorkerCompleted += Reader_RunWorkerCompleted;

            extractor.DoWork += Extractor_DoWork;
            extractor.RunWorkerCompleted += Extractor_RunWorkerCompleted;

            miniExtractor.DoWork += MiniExtractor_DoWork;
            miniExtractor.RunWorkerCompleted += Extractor_RunWorkerCompleted;

            InitDarkTheme(this);

            // dark renderer to the menu strip
            this.mainMenuStrip.Renderer = new DarkRenderer();
            this.cntxtMenuStrip.Renderer = new DarkRenderer();
        }

        private void MiniExtractor_DoWork(object sender, DoWorkEventArgs e)
        {
            if (selectedFOFile != null && selectedFOFile.GetFOFileType() == FOnlineFile.FOType.DAT)
            {
                Dat dat = (Dat)selectedFOFile;
                DatDoExtract(dat);
            }
        }

        /// <summary>
        /// The InitDarkTheme.
        /// </summary>
        /// <param name="root">The root<see cref="Control"/>.</param>
        private void InitDarkTheme(Control root)
        {
            root.BackColor = DarkBackground;
            root.ForeColor = DarkForeground;
            foreach (Control ctrl in root.Controls)
            {
                InitDarkTheme(ctrl);
            }
        }

        /// <summary>
        /// The Dat_OnProgressUpdate.
        /// </summary>
        /// <param name="value">The value<see cref="double"/>.</param>
        private void FOnlineFile_OnProgressUpdate(double value)
        {
            // its in another thread so invoke back to UI thread
            base.Invoke((Action)delegate
            {
                // fonline file index
                int index = fOnlineFiles.IndexOf(currFOFile);
                this.taskProgressBar.Value = (currFOFile == null) ? 100 : (int)Math.Round((index + 1) * currFOFile.GetProgress() / (double) fOnlineFiles.Count);
            });
        }

        /// <summary>
        /// Builds tree for the form based on data tree structure.
        /// </summary>
        /// <param name="dat">dat archive.</param>
        private void BuildTreeView(Dat dat)
        {
            treeViewDat.BeginUpdate();
            // then call preorder
            List<Node<string>> datPreOrderList = dat.Tree.Preorder();

            TreeNodeCollection selected = treeViewDat.Nodes;
            TreeNode selectedNode = null;

            foreach (Node<string> datNode in datPreOrderList)
            {
                // position yourself always to be the parent of the datNode
                if (datNode.Parent != null)
                {
                    int level = datNode.Level();
                    while (selectedNode.Level >= level)
                    {
                        selectedNode = selectedNode.Parent;
                    }
                    selected = selectedNode.Nodes;
                }

                // these elifs are for choosing corresponding image
                if (datNode == dat.Tree.Root)
                {
                    selectedNode = new TreeNode(datNode.Data, ClosedRootIndex, ClosedRootIndex);
                }
                else if (datNode.Children.Count != 0)
                {
                    selectedNode = new TreeNode(datNode.Data, ClosedDirIndex, ClosedDirIndex);
                }
                else if (datNode.Data.ToLower().EndsWith(".acm"))
                {
                    selectedNode = new TreeNode(datNode.Data, ACMIndex, ACMIndex);
                }
                else if (datNode.Data.ToLower().EndsWith(".frm"))
                {
                    selectedNode = new TreeNode(datNode.Data, FRMIndex, FRMIndex);
                }
                else
                {
                    selectedNode = new TreeNode(datNode.Data, FileIndex, FileIndex);
                }

                // associate with the node dat tree
                selectedNode.Tag = new KeyValuePair<FOnlineFile, Node<string>>(dat, datNode);              
                selected.Add(selectedNode);
            }

            treeViewDat.CollapseAll();

            treeViewDat.EndUpdate();
        }

        /// <summary>
        /// Builds the list view.
        /// Tree child nodes are displayed as items in the view.
        /// </summary>
        /// <param name="dat">dat archive.</param>
        /// <param name="datNode"> dat node to start building from .</param>
        private void BuildListView(Dat dat, Node<string> datNode)
        {
            datCacheMiss = true;
            datListViewItems.Clear();

            // set path info to the full file name generated from datnode
            txtBoxPathInfo.Text = Dat.GetFileName(datNode);

            int dirCount = 0;
            int fileCount = 0;
            foreach (Node<string> child in datNode.Children)
            {
                // image index is chosen on these circumstances
                int imageIndex;
                if (child == dat.Tree.Root)
                {
                    imageIndex = ClosedRootIndex;
                }
                else if (child.Children.Count != 0)
                {
                    imageIndex = ClosedDirIndex;
                    dirCount++;
                }
                else if (child.Data.ToLower().EndsWith(".acm"))
                {
                    imageIndex = ACMIndex;
                    fileCount++;
                }
                else if (child.Data.ToLower().EndsWith(".frm"))
                {
                    imageIndex = FRMIndex;
                    fileCount++;
                }
                else
                {
                    imageIndex = FileIndex;
                    fileCount++;
                }

                // create new list view item and add it to the big list
                ListViewItem item = new ListViewItem(child.Data, imageIndex);
                // tag tree node from the dat instance
                item.Tag = child;
                datListViewItems.Add(item);
            }
            txtBoxFileCount.Text = String.Format("{0} directories and {1} files", dirCount, fileCount);

            // enable virtual (for better performance)            
            listViewDat.VirtualListSize = datListViewItems.Count;
        }

        /// <summary>
        /// Reads the dat file, builds dat tree
        /// Basically does all. Doesn't involve any windows controls 
        /// Invoked from another thread beacuase it's expensive for perfomance.
        /// Called from Background worker.
        /// </summary>
        /// <param name="dat">The dat<see cref="Dat"/>.</param>
        private void DatDoAll(Dat dat, string inputFile)
        {
            // sub to the event (dat is fresh spawn so no stacking events)
            dat.OnProgressUpdate += FOnlineFile_OnProgressUpdate;

            // read the input dat file
            dat.ReadFile(inputFile);

            // call dat to buld tree structure
            dat.BuildTreeStruct();
        }

        /// <summary>
        /// Do Relevant stuff with FRM.
        /// Called from Background worker.
        /// </summary>
        /// <param name="frm">.</param>
        private void FRMDoAll(FRM frm, string inputFile)
        {
            // sub to the event (frm is fresh spawn so no stacking events)
            frm.OnProgressUpdate += FOnlineFile_OnProgressUpdate;

            // read FRM
            frm.ReadFile(inputFile);
        }

        /// <summary>
        /// Do relevant stuff with ACM
        /// Called from Background worker.
        /// </summary>
        /// <param name="acm">.</param>
        private void ACMDoAll(ACM acm, string inputFile)
        {
            acm.OnProgressUpdate += FOnlineFile_OnProgressUpdate;

            // read acm
            acm.ReadFile(inputFile);
        }

        /// <summary>
        /// Extract all from the dat.
        /// Called from Background worker.
        /// </summary>
        /// <param name="dat">The dat<see cref="Dat"/>.</param>
        private void DatDoExtractAll(Dat dat)
        {
            stopwatch.Start();

            // call dat to extract all
            dat.ExtractAll(outDir);

            // stop measuring the time
            stopwatch.Stop();

            // set displayed elapsed time (in the message), measured in seconds
            seconds = stopwatch.ElapsedMilliseconds / 1000.0;

            // reset for another read or any op
            stopwatch.Reset();
        }

        /// <summary>
        /// Extract all from the selected treeview/listview of dat archive.
        /// Called from Background worker.
        /// </summary>
        /// <param name="dat">The dat<see cref="Dat"/>.</param>
        private void DatDoExtract(Dat dat)
        {
            stopwatch.Start();

            // call dat to extract all
            foreach (Node<string> node in this.files4Extract)
            {
                dat.Extract(outDir, node);
            }

            // stop measuring the time
            stopwatch.Stop();

            // set displayed elapsed time (in the message), measured in seconds
            seconds = stopwatch.ElapsedMilliseconds / 1000.0;

            // reset for another read or any op
            stopwatch.Reset();
        }

        /// <summary>
        /// Do something with FOnline File.
        /// </summary>
        private void DoWith()
        {    
            treeViewDat.Nodes.Clear();
            datListViewItems.Clear();        
            foreach (FOnlineFile fOnlineFile in fOnlineFiles)
            {
                TreeNode treeNode;
                switch (fOnlineFile.GetFOFileType())
                {
                    case FOnlineFile.FOType.DAT:
                        Dat dat = (Dat)fOnlineFile;
                        BuildTreeView(dat); // build left side, tree view                        
                        break;
                    case FOnlineFile.FOType.ACM:
                        ACM acm = (ACM)fOnlineFile;
                        treeNode = new TreeNode(acm.Tag, ACMIndex, ACMIndex);
                        treeNode.Tag = new KeyValuePair<FOnlineFile, Node<string>>(acm, new Node<string>(acm.Tag));
                        treeViewDat.Nodes.Add(treeNode);
                        break;
                    case FOnlineFile.FOType.FRM:
                        FRM frm = (FRM)fOnlineFile;
                        treeNode = new TreeNode(frm.Tag, FRMIndex, FRMIndex);
                        treeNode.Tag = new KeyValuePair<FOnlineFile, Node<string>>(frm, new Node<string>(frm.Tag));
                        treeViewDat.Nodes.Add(treeNode);
                        break;
                }
            }
        }

        /// <summary>
        /// The FileOpen.
        /// </summary>
        private void FileOpen()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Fallout 2 dat files (*.dat)|*.dat|FRM files (*.frm)|*.frm|ACM files (*.acm)|*.acm|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;                

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fOnlineFiles.Clear();
                    listBoxInputFiles.Items.Clear();
                    inputFiles = openFileDialog.FileNames;

                    foreach (string inputFile in inputFiles)
                    {
                        listBoxInputFiles.Items.Add(inputFile);
                    }                    

                    reader.RunWorkerAsync();
                }
            }
        }

        /// <summary>
        /// The btnInDir_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void btnInArch_Click(object sender, EventArgs e)
        {
            FileOpen();
        }

        /// <summary>
        /// Executes after Reader complete it's work.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e">.</param>
        private void Reader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Detect errors!
            int errNum = 0;
            StringBuilder errMsg = new StringBuilder();
            foreach (FOnlineFile fOnlineFile in fOnlineFiles)
            {
                if (fOnlineFile.IsError())
                {
                    errMsg.Append("@" + fOnlineFile.GetTag() + " : " + fOnlineFile.GetErrorMessage() + "\n");
                    errNum++;
                }
            }

            if (errNum == 0)
            {
                MessageBox.Show("File(s) sucessfully loaded in " + seconds + " seconds!", "Reading File(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }            
            else
            {
                MessageBox.Show("File(s) loaded in (" + seconds + " seconds) with errors." + "\n" + errMsg.ToString(), "Reading File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.taskProgressBar.Value = 0;

            // do something with FOnline File(s)!
            DoWith();
        }

        /// <summary>
        /// The Worker_DoWork.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DoWorkEventArgs"/>.</param>
        private void Reader_DoWork(object sender, DoWorkEventArgs e)
        {
            currInFileIndex = 0;

            // start measuring the time
            stopwatch.Start();

            foreach (string inputFile in inputFiles)
            {
                // choose behaviour based on file extension
                string extension = Path.GetExtension(inputFile);  
                
                // fonlinefile in the switch scope
                switch (extension.ToLower())
                {
                    case ".acm":
                        currFOFile = new ACM(inputFile);
                        fOnlineFiles.Add(currFOFile);
                        ACMDoAll((ACM)currFOFile, inputFile);
                        break;
                    case ".frm":
                        currFOFile = new FRM(inputFile);
                        fOnlineFiles.Add(currFOFile);
                        FRMDoAll((FRM)currFOFile, inputFile);
                        break;
                    case ".dat":
                        currFOFile = new Dat(inputFile);
                        fOnlineFiles.Add(currFOFile);
                        DatDoAll((Dat)currFOFile, inputFile); // read dat file, build dat tree  
                        break;
                }                

                // inc current file index
                currInFileIndex++;
            }

            // stop measuring the time
            stopwatch.Stop();

            // set displayed elapsed time (in the message), measured in seconds
            seconds = stopwatch.ElapsedMilliseconds / 1000.0;

            // reset for another read or any op
            stopwatch.Reset();
        }

        /// <summary>
        /// The treeViewDat_BeforeExpand.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="TreeViewCancelEventArgs"/>.</param>
        private void treeViewDat_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.ImageIndex = (e.Node.Level == 0) ? OpenedRootIndex : OpenedDirIndex;
        }

        /// <summary>
        /// The treeViewDat_AfterCollapse.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="TreeViewEventArgs"/>.</param>
        private void treeViewDat_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = (e.Node.Level == 0) ? ClosedRootIndex : ClosedDirIndex;
        }

        /// <summary>
        /// The treeViewDat_AfterSelect.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="TreeViewEventArgs"/>.</param>
        private void treeViewDat_AfterSelect(object sender, TreeViewEventArgs e)
        {
            KeyValuePair<FOnlineFile, Node<string>> pair = (KeyValuePair<FOnlineFile, Node<string>>)e.Node.Tag;
            selectedFOFile = pair.Key;
            Node<string> node = pair.Value;

            if (selectedFOFile.GetFOFileType() == FOnlineFile.FOType.DAT) {
                IAsyncResult asyncResult = treeViewDat.BeginInvoke(new Action(() =>
                {
                    if (e.Node.IsExpanded)
                    {
                        e.Node.Collapse();
                    }
                    else
                    {
                        e.Node.Expand();
                    }
                    BuildListView((Dat)selectedFOFile, node);
                }));
                treeViewDat.EndInvoke(asyncResult);
            }            
        }        

        /// <summary>
        /// The listViewDat_RetrieveVirtualItem.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RetrieveVirtualItemEventArgs"/>.</param>
        private void listViewDat_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            // if cache hit
            if (datCache != null && e.ItemIndex >= datCacheIndex && e.ItemIndex < datCacheIndex + datCache.Length)
            {
                // cache hit occurred, set miss to false
                datCacheMiss = false;
                // simply retrieve it from the cache
                e.Item = datCache[e.ItemIndex];
            }
            // otherwise if cache miss occurs
            else
            {
                // cache miss occured, set to true
                datCacheMiss = true;
                // fetch new one, wait to see what happens   
                e.Item = this.datListViewItems[e.ItemIndex];
            }
        }

        /// <summary>
        /// The listViewDat_CacheVirtualItems.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="CacheVirtualItemsEventArgs"/>.</param>
        private void listViewDat_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            // base case - no need to rebuild cache
            if (e.StartIndex >= datCacheIndex || e.EndIndex <= datCacheIndex + datCache.Length)
            {
                return;
            }

            // if file is not in the cache (cache miss occurred)
            if (datCacheMiss)
            {
                datCacheIndex = e.StartIndex;
                int length = e.EndIndex - e.StartIndex + 1;
                datCache = new ListViewItem[length];
                for (int i = 0; i < length; i++)
                {
                    datCache[i] = datListViewItems[e.StartIndex + i];
                }

            }
        }

        /// <summary>
        /// The previewToolStripMenuItem_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedFOFile != null) {

                // create ACM list (sound files)
                List<ACM> acms = new List<ACM>();
                // create FRM list (image files)
                List<FRM> fRMs = new List<FRM>();

                switch (selectedFOFile.GetFOFileType()) 
                {
                    case FOnlineFile.FOType.DAT:
                        // make a cast
                        Dat dat = (Dat)selectedFOFile;                        

                        ListView.SelectedIndexCollection selectedIndices = listViewDat.SelectedIndices;
                        foreach (int selectedIndex in selectedIndices)
                        {
                            ListViewItem selItem = datListViewItems[selectedIndex];
                            Node<string> datNode = (Node<string>)selItem.Tag;
                            DataBlock dataBlock = dat.GetDataBlock(datNode);
                            byte[] bytes = dat.Data(dataBlock);
                            string filename = dataBlock.Filename;
                            if (filename.ToLower().EndsWith(".acm"))
                            {
                                ACM acm = new ACM(dataBlock.Filename);
                                acm.ReadBytes(bytes);
                                acms.Add(acm);
                            }
                            else if (filename.ToLower().EndsWith(".frm"))
                            {
                                FRM frm = new FRM(dataBlock.Filename);
                                frm.ReadBytes(bytes);
                                fRMs.Add(frm);
                            }

                        }

                        if (fRMs.Count != 0)
                        {
                            // create and use the subform
                            using (FRMForm frmForm = new FRMForm(fRMs))
                            {
                                frmForm.ShowDialog();
                            }
                        }

                        if (acms.Count != 0)
                        {
                            // create and use the subform
                            using (ACMForm acmForm = new ACMForm(acms))
                            {
                                acmForm.ShowDialog();
                            }
                        }
                        break;

                    case FOnlineFile.FOType.ACM:
                        acms.Add((ACM)selectedFOFile);
                        if (acms.Count != 0)
                        {
                            // create and use the subform
                            using (ACMForm acmForm = new ACMForm(acms))
                            {
                                acmForm.ShowDialog();
                            }
                        }
                        break;

                    case FOnlineFile.FOType.FRM:
                        fRMs.Add((FRM)selectedFOFile);
                        if (fRMs.Count != 0)
                        {
                            // create and use the subform
                            using (FRMForm frmForm = new FRMForm(fRMs))
                            {
                                frmForm.ShowDialog();
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// The btnOutDir_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void btnOutDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog openDirDialog = new FolderBrowserDialog())
            {
                if (openDirDialog.ShowDialog() == DialogResult.OK)
                {
                    outDir = openDirDialog.SelectedPath;
                    this.txtBoxOutDir.Text = outDir;
                }
            }
        }

        /// <summary>
        /// The btnExtract_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void btnExtractAll_Click(object sender, EventArgs e)
        {
            if (inputFiles == null)
            {
                MessageBox.Show("There's no input file. Make sure it's loaded first!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (outDir == null)
            {
                MessageBox.Show("Output directory is not selected!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (selectedFOFile == null)
            {
                MessageBox.Show("There is no selected files for extraction! Please select one.", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
            else if(selectedFOFile.GetFOFileType() != FOnlineFile.FOType.DAT)
            {
                MessageBox.Show("Selected file is not dat archive!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("This is time consuming operation, are you sure you want to continue?", "Extracting File(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    extractor.RunWorkerAsync();
                }
            }
        }

        /// <summary>
        /// The Extractor_DoWork.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DoWorkEventArgs"/>.</param>
        private void Extractor_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (FOnlineFile fOnlineFile in fOnlineFiles)
            {
                // extract only archives
                if (fOnlineFile.GetFOFileType() == FOnlineFile.FOType.DAT)
                {
                    DatDoExtractAll((Dat)(fOnlineFile));
                }
            }
        }

        /// <summary>
        /// The Extractor_RunWorkerCompleted.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RunWorkerCompletedEventArgs"/>.</param>
        private void Extractor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Dat File(s) sucessfully extracted in " + seconds + " seconds!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.taskProgressBar.Value = 0;
        }

        /// <summary>
        /// The aboutToolStripMenuItem_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("VERSION v0.4 - DEUTERIUM\n");
            sb.Append("\n");
            sb.Append("PUBLIC BUILD reviewed on 2021-02-04 at 07:15).\n");
            sb.Append("This software is free software.\n");
            sb.Append("Licensed under GNU General Public License (GPL).\n");
            sb.Append("\n");
            sb.Append("Changelog for v0.3 CHROMIUM:\n");
            sb.Append("\t- Initial pre-release.\n");
            sb.Append("\n");
            sb.Append("\n");
            sb.Append("Copyright © 2021\n");
            sb.Append("Alexander \"Ermac\" Stojanovich\n");
            sb.Append("\n");

            MessageBox.Show(sb.ToString(), "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// The exitToolStripMenuItem_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// The openToolStripMenuItem_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileOpen();
        }

        /// <summary>
        /// The HowToToolStripMenuItem_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void HowToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("- FOR THE PURPOSE ABOUT THIS PROGRAM, \n");
            sb.Append("check About. Make sure that you checked it first.\n");
            sb.Append("\n");
            sb.Append("- Opening file(s):\n");
            sb.Append("\t1. Choose input input file\n");
            sb.Append("\t   by clicking the button \"Open...\".\n");
            sb.Append("\t   Or via the menus File > Open.\n");
            sb.Append("\t   Supported files are Fallout 2 archive (*.dat),\n");
            sb.Append("\t   FRM image file (*.frm) or ACM sound file (*.acm).\n");
            sb.Append("\t2. Wait for task to complete\n");
            sb.Append("\tand you will be prompted with message.\n");
            sb.Append("\t3. View the file, enjoy.\n");
            sb.Append("\n");
            sb.Append("- Extracting file(s):\n");
            sb.Append("\t1. Locate Fallout 2 archive (*.dat) on your filesystem.\n");
            sb.Append("\t2. Perform step \"Opening file\".\n");
            sb.Append("\t3. Using hierarchical tree view and list view find\n");
            sb.Append("\t   and select designated files for extraction.\n");
            sb.Append("\t4. You will be prompted where to extract them.\n");
            sb.Append("\t5. Extract them.\n");
            sb.Append("\n");
            sb.Append("- Extracting all files:\n");
            sb.Append("\t1. Locate Fallout 2 archive (*.dat) on your filesystem.\n");
            sb.Append("\t2. Perform step \"Opening file\".\n");
            sb.Append("\t3. Choose \"Output...\" by clicking on the button.\n");
            sb.Append("\t4. Click on \"Extract All\".\n");
            sb.Append("\t5. You will be prompted that \n");
            sb.Append("\t   this operation may take time.\n");
            sb.Append("\t6. Accept it with \"Yes\" and operation will commence.\n");
            sb.Append("\n");

            MessageBox.Show(sb.ToString(), "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// The convertToolStripMenuItem_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void convertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedFOFile != null)
            {
                // create ACM list (sound files)
                List<ACM> acms = new List<ACM>();                

                switch (selectedFOFile.GetFOFileType())
                {
                    case FOnlineFile.FOType.DAT:
                        // make a cast
                        Dat dat = (Dat)selectedFOFile;

                        ListView.SelectedIndexCollection selectedIndices = listViewDat.SelectedIndices;
                        foreach (int selectedIndex in selectedIndices)
                        {
                            ListViewItem selItem = datListViewItems[selectedIndex];
                            Node<string> datNode = (Node<string>)selItem.Tag;
                            DataBlock dataBlock = dat.GetDataBlock(datNode);
                            byte[] bytes = dat.Data(dataBlock);
                            string filename = dataBlock.Filename;
                            if (filename.ToLower().EndsWith(".acm"))
                            {
                                ACM acm = new ACM(dataBlock.Filename);
                                acm.ReadBytes(bytes);
                                acms.Add(acm);
                            }                            

                        }                        

                        if (acms.Count != 0)
                        {
                            // create and use the subform
                            using (ACMConversionForm cacmForm = new ACMConversionForm(acms))
                            {
                                cacmForm.ShowDialog();
                            }
                        }
                        break;

                    case FOnlineFile.FOType.ACM:
                        acms.Add((ACM)selectedFOFile);
                        if (acms.Count != 0)
                        {
                            // create and use the subform
                            using (ACMConversionForm cacmForm = new ACMConversionForm(acms))
                            {
                                cacmForm.ShowDialog();
                            }
                        }
                        break;

                    case FOnlineFile.FOType.FRM:
                        MessageBox.Show("Conversion is not supported for FRM(s)!", "File(s) Conversion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
        }

        private void listViewDat_DoubleClick(object sender, EventArgs e)
        {            
            if (selectedFOFile != null && selectedFOFile.GetFOFileType() == FOnlineFile.FOType.DAT)
            {
                IAsyncResult asyncResult = listViewDat.BeginInvoke(new Action(() =>
                {
                    ListView.SelectedIndexCollection selectedIndices = listViewDat.SelectedIndices;
                    foreach (int selectedIndex in selectedIndices)
                    {
                        ListViewItem selItem = datListViewItems[selectedIndex];
                        BuildListView((Dat)selectedFOFile, (Node<string>)selItem.Tag);
                    }
                }));
                listViewDat.EndInvoke(asyncResult);
            }
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (outDir == null)
            {
                MessageBox.Show("Output directory is not selected!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!miniExtractor.IsBusy)
            {
                // call dat to extract all
                if (treeViewDat.SelectedNode != null)
                {
                    KeyValuePair<FOnlineFile, Node<string>> pair = (KeyValuePair<FOnlineFile, Node<string>>)treeViewDat.SelectedNode.Tag;
                    Node<string> target = pair.Value;
                    files4Extract.Add(target);
                }
                else
                {
                    ListView.SelectedIndexCollection selectedIndices = listViewDat.SelectedIndices;
                    foreach (int selectedIndex in selectedIndices)
                    {
                        ListViewItem selItem = datListViewItems[selectedIndex];
                        files4Extract.Add((Node<string>)selItem.Tag); 
                    }
                }
                miniExtractor.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Extracting currently in progress, Please Wait!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
    }
}
