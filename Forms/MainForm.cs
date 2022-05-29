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
        public const int MemSize = 0x20000000; // 512 MB allowed to take

        /// <summary>
        /// FOnline file list.
        /// </summary>
        private List<FOnlineFile> fOnlineFiles = new List<FOnlineFile>();

        /// <summary>
        /// Currently SELECTED FOnlineFile from left side in tree view..
        /// It's a must have cuz we need to call corresponding FOnline methods.
        /// </summary>
        private FOnlineFile leftSelectedFOFile;


        /// <summary>
        /// Currently SELECTED FOnlineFile from right side in list view..
        /// It's a must have cuz we need to call corresponding FOnline methods.
        /// </summary>
        private FOnlineFile rightSelectedFOFile;


        /// <summary>
        /// Currently PROCESSED FOnlineFile.
        /// </summary>
        private FOnlineFile currFOFile;

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
        /// Defines the ClosedRootIndex.
        /// </summary>
        internal const int ClosedArchiveIndex = 2;

        /// <summary>
        /// Defines the OpenedRootIndex.
        /// </summary>
        internal const int OpenedArchiveIndex = 3;

        /// <summary>
        /// Defines the ClosedDirIndex.
        /// </summary>
        internal const int ClosedDirIndex = 4;

        /// <summary>
        /// Defines the OpenedDirIndex.
        /// </summary>
        internal const int OpenedDirIndex = 5;

        /// <summary>
        /// Defines the ACMIndex.
        /// </summary>
        internal const int ACMIndex = 6;

        /// <summary>
        /// Defines the FRMIndex.
        /// </summary>
        internal const int FRMIndex = 7;

        /// <summary>
        /// Defines the FileIndex.
        /// </summary>
        internal const int FileIndex = 8;

        /// <summary>
        /// Input files from the filesystem {FRM, ACM or DAT}
        /// </summary>
        private string[] inputFiles;

        /// <summary>
        /// Defines the output directory to extract the data.
        /// </summary>
        private string outDir;

        /// <summary>
        /// Defines the stopwatch.
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Defines the loading worker.............
        /// </summary>
        private readonly BackgroundWorker reader = new BackgroundWorker();

        /// <summary>
        /// Defines the extractor worker.............
        /// </summary>
        private readonly BackgroundWorker extractor = new BackgroundWorker();

        /// <summary>
        /// Defines the datCache.
        /// </summary>
        private ListViewItem[] datCache;

        /// <summary>
        /// Tells if cache miss has occurred
        /// </summary>
        private bool datCacheMiss = true;

        /// <summary>
        /// Defines the begin index of the item block and it's always 1000 in length
        /// </summary>
        private int datCacheIndex = 0;

        /// <summary>
        /// Defines the the dat list view items
        /// </summary>
        private readonly List<ListViewItem> datListViewItems = new List<ListViewItem>(2000);

        /// <summary>
        /// Defines the seconds.
        /// </summary>
        private double seconds = 0.0;

        /// <summary>
        /// Extract specific files..
        /// </summary>
        private readonly BackgroundWorker miniExtractor = new BackgroundWorker();

        /// <summary>
        /// List of specific files for extraction..
        /// </summary>
        private readonly List<Node<string>> files2Extract = new List<Node<string>>();

        /// <summary>
        /// Root of Tree View.
        /// </summary>
        private readonly TreeNode rootNode = new TreeNode("/", ClosedRootIndex, ClosedRootIndex);

        /// <summary>
        /// My root of the virtual system. In the root are all loaded files.
        /// </summary>
        private readonly Node<string> myRoot = new Node<string>("/");
                        
        /// <summary>
        /// Target for operation Extract All
        /// </summary>
        private Dat extrTargDat;

        private readonly StringBuilder glblErrMsg = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        /// 
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
            this.cntxtMenuStripShort.Renderer = new DarkRenderer();
            this.cntxtMenuStripLong.Renderer = new DarkRenderer();
        }

        /// <summary>
        /// The MiniExtractor_DoWork.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DoWorkEventArgs"/>.</param>
        private void MiniExtractor_DoWork(object sender, DoWorkEventArgs e)
        {
            if (leftSelectedFOFile != null && leftSelectedFOFile.GetFOFileType() == FOnlineFile.FOType.DAT)
            {
                Dat dat = (Dat)leftSelectedFOFile;
                currFOFile = dat;
                DatDoExtract(dat);
            } 
            else if (rightSelectedFOFile != null && rightSelectedFOFile.GetFOFileType() == FOnlineFile.FOType.DAT)
            {
                Dat dat = (Dat)rightSelectedFOFile;
                currFOFile = dat;
                DatDoExtract(dat);
            }

        }

        /// <summary>
        /// The InitDarkTheme.
        /// </summary>
        /// <param name="root">The root<see cref="Control"/>.</param>
        public static void InitDarkTheme(Control root)
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
                this.taskProgressBar.Value = (currFOFile == null) ? 100 : (int)Math.Round(currFOFile.GetProgress());
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
           
            TreeNodeCollection selected = rootNode.Nodes;
            TreeNode selectedNode = null;

            foreach (Node<string> datNode in datPreOrderList)
            {
                // position yourself always to be the parent of the datNode
                if (datNode.Parent != null)
                {                    
                    if (selectedNode == null)
                    {
                        selectedNode = this.rootNode;
                    } 
                    else
                    {
                        int level = datNode.Level() + 1; // +1 comes from pre-existing root
                        while (selectedNode.Level >= level)
                        {
                            selectedNode = selectedNode.Parent;
                        }
                        selected = selectedNode.Nodes;
                    }                    
                }

                // these elifs are for choosing corresponding image
                if (datNode == dat.Tree.Root)
                {
                    selectedNode = new TreeNode(datNode.Data, ClosedArchiveIndex, ClosedArchiveIndex);
                    selectedNode.Tag = new KeyValuePair<FOnlineFile, Node<string>>(dat, datNode);
                    selected.Add(selectedNode);
                }
                else if (datNode.Children.Count != 0)
                {
                    selectedNode = new TreeNode(datNode.Data, ClosedDirIndex, ClosedDirIndex);
                    selectedNode.Tag = new KeyValuePair<FOnlineFile, Node<string>>(dat, datNode);
                    selected.Add(selectedNode);
                }
            }

            treeViewDat.CollapseAll();

            treeViewDat.EndUpdate();
        }

        /// <summary>
        /// Builds the list view.
        /// Tree child nodes are displayed as items in the view.
        /// </summary>
        /// <param name="dat">dat archive.</param>
        /// <param name="node"> dat node to start building from .</param>
        private void BuildListView(FOnlineFile fOnlineFile, Node<string> node)
        { 
            txtBoxPathInfo.Text = "";

            datListViewItems.Clear();
            listViewDat.Items.Clear();

            // if no children (like files) make it no effect
            if (node.Children.Count == 0)
            {
                listViewDat.VirtualListSize = 0;
                return;
            }

            if (fOnlineFile == null && node == myRoot)
            {
                txtBoxPathInfo.Text = "/";

                foreach (FOnlineFile inFoFile in fOnlineFiles)
                {
                    // image index is chosen on these circumstances
                    int imageIndex;
                    if (inFoFile.GetFOFileType() == FOnlineFile.FOType.DAT)
                    {
                        imageIndex = ClosedArchiveIndex;
                    }
                    else if (inFoFile.GetFOFileType() == FOnlineFile.FOType.ACM)
                    {
                        imageIndex = ACMIndex;
                    }
                    else if (inFoFile.GetFOFileType() == FOnlineFile.FOType.FRM)
                    {
                        imageIndex = FRMIndex;
                    }
                    else
                    {
                        imageIndex = FileIndex;
                    }
                    

                    // create new list view item and add it to the big list
                    ListViewItem item = new ListViewItem(inFoFile.GetTag(), imageIndex);
                    // tag tree node from the dat instance                    

                    Node<string> child = node.Children.Find(m => m.Data.Equals(inFoFile.GetTag()));
                    if (child != null)
                    {
                        item.Tag = new KeyValuePair<FOnlineFile, Node<string>>(inFoFile, child);
                        datListViewItems.Add(item);
                    }                    
                }
                txtBoxFileCount.Text = String.Format("0 directories and {0} files", node.Children.Count);                
            }
            else if (fOnlineFile.GetFOFileType() == FOnlineFile.FOType.DAT)
            {
                Dat dat = (Dat)fOnlineFile;
                datCacheMiss = true;
                // set path info to the full file name generated from datnode
                txtBoxPathInfo.Text = "/" + Dat.GetFileName(node);

                int dirCount = 0;
                int fileCount = 0;
                foreach (Node<string> child in node.Children)
                {
                    // image index is chosen on these circumstances
                    int imageIndex;
                    if (child == dat.Tree.Root)
                    {
                        imageIndex = ClosedArchiveIndex;
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
                    item.Tag = new KeyValuePair<FOnlineFile, Node<string>>(fOnlineFile, child);
                    datListViewItems.Add(item);
                }
                txtBoxFileCount.Text = String.Format("{0} directories and {1} files", dirCount, fileCount);

                
            }
            
            listViewDat.VirtualListSize = datListViewItems.Count;
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
            foreach (Node<string> node in this.files2Extract)
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
        /// Do something with FOnline File(s).
        /// </summary>
        private void ProcessFOnlineFiles()
        {           
            treeViewDat.Nodes.Clear();
            datListViewItems.Clear();
            myRoot.Children.Clear();
            rootNode.Nodes.Clear();

            if (fOnlineFiles.Count != 0)
            {
                treeViewDat.Nodes.Add(rootNode);
                
                rootNode.Tag = new KeyValuePair<FOnlineFile, Node<string>>(null, myRoot);            

                ListViewItem item;
                Node<string> node;
                foreach (FOnlineFile fOnlineFile in fOnlineFiles)
                {
                    // add to the tree and to the list view only the files without error
                    if (!fOnlineFile.IsError())
                    {
                        switch (fOnlineFile.GetFOFileType())
                        {
                            case FOnlineFile.FOType.DAT:
                                Dat dat = (Dat)fOnlineFile;
                                BuildTreeView(dat); // build left side, tree view
                                item = new ListViewItem(dat.Tag, ClosedArchiveIndex);
                                node = dat.Tree.Root;
                                node.Parent = myRoot;
                                myRoot.Children.Add(node);
                                item.Tag = new KeyValuePair<FOnlineFile, Node<string>>(dat, node);
                                datListViewItems.Add(item);
                                break;
                            case FOnlineFile.FOType.ACM:
                                ACM acm = (ACM)fOnlineFile;
                                item = new ListViewItem(acm.Tag, ACMIndex);
                                node = new Node<string>(acm.Tag);
                                node.Parent = myRoot;
                                myRoot.Children.Add(node);
                                item.Tag = new KeyValuePair<FOnlineFile, Node<string>>(acm, node);
                                datListViewItems.Add(item);
                                break;
                            case FOnlineFile.FOType.FRM:
                                FRM frm = (FRM)fOnlineFile;
                                item = new ListViewItem(frm.Tag, FRMIndex);
                                node = new Node<string>(frm.Tag);
                                node.Parent = myRoot;
                                myRoot.Children.Add(node);
                                item.Tag = new KeyValuePair<FOnlineFile, Node<string>>(frm, node);
                                datListViewItems.Add(item);
                                break;
                        }
                    }
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
                    string[] inputFiles = openFileDialog.FileNames;
                    
                    this.txtBoxPathInfo.Text = "";
                    this.txtBoxFileCount.Text = "";

                    this.cmbBoxFOFiles.Items.Clear();

                    foreach (string inputFile in inputFiles)
                    {
                        listBoxInputFiles.Items.Add(inputFile);
                        if (inputFile.ToLower().EndsWith(".dat"))
                        {
                            cmbBoxFOFiles.Items.Add(Path.GetFileName(inputFile));
                        }
                    }                    

                    reader.RunWorkerAsync(inputFiles);
                }
            }
        }

        /// <summary>
        /// File Add (Include File)
        /// </summary>
        private void FileAdd()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Fallout 2 dat files (*.dat)|*.dat|FRM files (*.frm)|*.frm|ACM files (*.acm)|*.acm|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] inputFiles = openFileDialog.FileNames;

                    this.txtBoxPathInfo.Text = "";
                    this.txtBoxFileCount.Text = "";

                    this.cmbBoxFOFiles.Items.Clear();

                    foreach (string inputFile in inputFiles)
                    {
                        listBoxInputFiles.Items.Add(inputFile);
                        if (inputFile.ToLower().EndsWith(".dat"))
                        {
                            cmbBoxFOFiles.Items.Add(Path.GetFileName(inputFile));
                        }
                    }

                    reader.RunWorkerAsync(inputFiles);
                }
            }
        }

        private void FileRemove()
        {            
            var selectedItems = listBoxInputFiles.SelectedItems;

            foreach (var selItem in selectedItems)
            {
                listBoxInputFiles.Items.Remove(selItem);                
            }
            var selectedNode = treeViewDat.SelectedNode;
            this.fOnlineFiles.RemoveAll(foFile => foFile.GetTag().Equals(selectedNode.Text));
            myRoot.Children.RemoveAll(node => node.Data.Equals(selectedNode.Text));   

            this.rootNode.Nodes.Remove(selectedNode);
            this.cmbBoxFOFiles.Items.Remove(selectedNode.Text);
            this.BuildListView(null, myRoot);        
                        
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
            // Display Error Message
            if (glblErrMsg.Length == 0)
            {
                MessageBox.Show("File(s) sucessfully loaded in " + seconds + " seconds!", "Reading File(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("File(s) loaded in (" + seconds + " seconds) with errors." + "\n" + glblErrMsg.ToString(), "Reading File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.taskProgressBar.Value = 0;

            // do something with FOnline File(s)!
            ProcessFOnlineFiles();

            // build list view items from the root "/"
            BuildListView(null, myRoot);
        }

        /// <summary>
        /// Reads the dat file, builds dat tree
        /// Basically does all. Doesn't involve any windows controls 
        /// Invoked from another thread beacuase it's expensive for perfomance.
        /// Called from Background worker.
        /// </summary>
        /// <param name="dat">The dat<see cref="Dat"/>.</param>
        /// <param name="inputFile">The inputFile<see cref="string"/>.</param>
        /// <returns> Is the operation OK </returns>
        private bool DatDoAll(Dat dat, string inputFile)
        {
            // sub to the event (dat is fresh spawn so no stacking events)
            dat.OnProgressUpdate += FOnlineFile_OnProgressUpdate;

            // read the input dat file
            dat.ReadFile(inputFile);

            // call dat to buld tree structure
            dat.BuildTreeStruct();

            // return negation of error flag - OK
            return !dat.Error;
        }

        /// <summary>
        /// Do Relevant stuff with FRM.
        /// Called from Background worker.
        /// </summary>
        /// <param name="frm">.</param>
        /// <param name="inputFile">The inputFile<see cref="string"/>.</param>
        private bool FRMDoAll(FRM frm, string inputFile)
        {
            // sub to the event (frm is fresh spawn so no stacking events)
            frm.OnProgressUpdate += FOnlineFile_OnProgressUpdate;

            // read FRM
            frm.ReadFile(inputFile);

            // return negation of error flag - OK
            return !frm.Error;
        }

        /// <summary>
        /// Do relevant stuff with ACM
        /// Called from Background worker.
        /// </summary>
        /// <param name="acm">.</param>
        /// <param name="inputFile">The inputFile<see cref="string"/>.</param>
        private bool ACMDoAll(ACM acm, string inputFile)
        {
            acm.OnProgressUpdate += FOnlineFile_OnProgressUpdate;

            // read acm
            acm.ReadFile(inputFile);

            // return negation of error flag - OK
            return !acm.Error;
        }

        /// <summary>
        /// The Worker_DoWork.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DoWorkEventArgs"/>.</param>
        private void Reader_DoWork(object sender, DoWorkEventArgs e)
        {
            leftSelectedFOFile = null;
            rightSelectedFOFile = null;
            currFOFile = null;

            inputFiles = (string[])e.Argument;

            // reset global error message
            glblErrMsg.Length = 0;

            // start measuring the time
            stopwatch.Start();            

            foreach (string inputFile in inputFiles)
            {
                // choose behaviour based on file extension
                string extension = Path.GetExtension(inputFile);
                bool ok = false;
                // fonlinefile in the switch scope
                switch (extension.ToLower())
                {
                    case ".acm":
                        currFOFile = new ACM(inputFile);
                        ok = ACMDoAll((ACM)currFOFile, inputFile);
                        if (ok)
                        {
                            fOnlineFiles.Add(currFOFile);
                        } 
                        else
                        {
                            glblErrMsg.Append("@" + currFOFile.GetTag() + " : " + currFOFile.GetErrorMessage() + "\n");
                        }
                        break;
                    case ".frm":
                        currFOFile = new FRM(inputFile);
                        ok = FRMDoAll((FRM)currFOFile, inputFile);
                        if (ok)
                        {
                            fOnlineFiles.Add(currFOFile);
                        } 
                        else
                        {
                            glblErrMsg.Append("@" + currFOFile.GetTag() + " : " + currFOFile.GetErrorMessage() + "\n");
                        }
                        break;
                    case ".dat":
                        currFOFile = new Dat(inputFile);                        
                        ok = DatDoAll((Dat)currFOFile, inputFile); // read dat file, build dat tree  
                        if (ok)
                        {
                            fOnlineFiles.Add(currFOFile);
                        } 
                        else
                        {
                            glblErrMsg.Append("@" + currFOFile.GetTag() + " : " + currFOFile.GetErrorMessage() + "\n");
                        }
                        break;
                    default:
                        glblErrMsg.Append("@" + inputFile + " : Unknown extension \"" + extension + "\"!\n");
                        break;
                }

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
            if (e.Node.Level == 0)
            {
                e.Node.ImageIndex = OpenedRootIndex;
            } 
            else 
            {
                KeyValuePair<FOnlineFile, Node<string>> kvp = (KeyValuePair<FOnlineFile, Node<string>>)e.Node.Tag;
                Node<string> node = kvp.Value;
                if (node.Data.ToLower().Contains(".dat"))
                {
                    e.Node.ImageIndex = OpenedArchiveIndex;
                } 
                else
                {
                    e.Node.ImageIndex = OpenedDirIndex;
                }
            }           
        }

        /// <summary>
        /// The treeViewDat_AfterCollapse.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="TreeViewEventArgs"/>.</param>
        private void treeViewDat_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                e.Node.ImageIndex = ClosedRootIndex;
            }
            else
            {
                KeyValuePair<FOnlineFile, Node<string>> kvp = (KeyValuePair<FOnlineFile, Node<string>>)e.Node.Tag;
                Node<string> node = kvp.Value;
                if (node.Data.ToLower().Contains(".dat"))
                {
                    e.Node.ImageIndex = ClosedArchiveIndex;
                }
                else
                {
                    e.Node.ImageIndex = ClosedDirIndex;
                }
            }
        }

        /// <summary>
        /// The treeViewDat_AfterSelect.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="TreeViewEventArgs"/>.</param>
        private void treeViewDat_AfterSelect(object sender, TreeViewEventArgs e)
        {
            leftSelectedFOFile = null;
            rightSelectedFOFile = null;
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
                if (e.Node.Tag != null)
                {
                    KeyValuePair<FOnlineFile, Node<string>> pair = (KeyValuePair<FOnlineFile, Node<string>>)e.Node.Tag;
                    leftSelectedFOFile = pair.Key;
                    Node<string> node = pair.Value;
                    BuildListView(leftSelectedFOFile, node);
                }
            }));
            treeViewDat.EndInvoke(asyncResult);

            var selectedNode = this.treeViewDat.SelectedNode;
            if (selectedNode != null)
            {
                cntxtMenuStripShort.Items[0].Enabled = (this.treeViewDat.SelectedNode == rootNode);
                cntxtMenuStripShort.Items[1].Enabled = (this.treeViewDat.SelectedNode != null && this.treeViewDat.SelectedNode.Level == 1);
                cntxtMenuStripShort.Items[2].Enabled = (this.treeViewDat.SelectedNode != null && this.treeViewDat.SelectedNode.Level >= 1);
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
                // add guards against index break
                e.Item = (e.ItemIndex >= 0 && e.ItemIndex < datListViewItems.Count) ? this.datListViewItems[e.ItemIndex] : new ListViewItem();
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

        private void BuildPreview()
        {
            ListView.SelectedIndexCollection selectedIndices = listViewDat.SelectedIndices;
            // create ACM list (sound files)
            List<ACM> acms = new List<ACM>();
            // create FRM list (image files)
            List<FRM> fRMs = new List<FRM>();

            int totalMem = 0;

            if (rightSelectedFOFile != null && rightSelectedFOFile.GetFOFileType() == FOnlineFile.FOType.DAT)
            {
                Dat dat = (Dat)rightSelectedFOFile;
                foreach (int selectedIndex in selectedIndices)
                {
                    ListViewItem selItem = datListViewItems[selectedIndex];
                    KeyValuePair<FOnlineFile, Node<string>> kvp = (KeyValuePair<FOnlineFile, Node<string>>)selItem.Tag;
                    Node<string> datNode = kvp.Value;

                    // protection of dat node
                    if (datNode != null)
                    {
                        DataBlock dataBlock = dat.GetDataBlock(datNode);
                        byte[] bytes = dat.Data(dataBlock);
                        string filename = dataBlock.Filename;
                        if (filename.ToLower().EndsWith(".acm"))
                        {
                            totalMem += ACM.BufferSize;
                            ACM acm = new ACM(dataBlock.Filename);
                            acm.ReadBytes(bytes);
                            acms.Add(acm);
                        }
                        else if (filename.ToLower().EndsWith(".frm"))
                        {
                            totalMem += FRM.BufferSize;
                            FRM frm = new FRM(dataBlock.Filename);
                            frm.ReadBytes(bytes);
                            fRMs.Add(frm);
                        }
                    }

                    if (totalMem > MemSize)
                    {
                        MessageBox.Show("Memory maxed out due to large amount of files!", "Memory Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                }

                if (fRMs.Count != 0 && totalMem <= MemSize)
                {
                    // create and use the subform
                    using (FRMForm frmForm = new FRMForm(fRMs))
                    {
                        frmForm.ShowDialog();
                    }
                }

                if (acms.Count != 0 && totalMem <= MemSize)
                {
                    // create and use the subform
                    using (ACMForm acmForm = new ACMForm(acms))
                    {
                        acmForm.ShowDialog();
                    }
                }

            }
            else if (rightSelectedFOFile != null)
            {
                foreach (int selectedIndex in selectedIndices)
                {
                    ListViewItem selItem = listViewDat.Items[selectedIndex];
                    KeyValuePair<FOnlineFile, Node<string>> kvp = (KeyValuePair<FOnlineFile, Node<string>>)selItem.Tag;
                    Node<string> itemNode = kvp.Value;
                    string filename = itemNode.Data;
                    if (filename.ToLower().EndsWith(".acm"))
                    {
                        totalMem += ACM.BufferSize;
                        ACM acm = new ACM(inputFiles[selectedIndex]);
                        acm.ReadFile(inputFiles[selectedIndex]);
                        acms.Add(acm);
                    }
                    else if (filename.ToLower().EndsWith(".frm"))
                    {
                        totalMem += FRM.BufferSize;
                        FRM frm = new FRM(inputFiles[selectedIndex]);
                        frm.ReadFile(inputFiles[selectedIndex]);
                        fRMs.Add(frm);
                    }

                    if (totalMem > MemSize)
                    {
                        MessageBox.Show("Memory maxed out due to large amount of files!", "Memory Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                }

                if (fRMs.Count != 0 && totalMem <= MemSize)
                {
                    // create and use the subform
                    using (FRMForm frmForm = new FRMForm(fRMs))
                    {
                        frmForm.ShowDialog();
                    }
                }

                if (acms.Count != 0 && totalMem <= MemSize)
                {
                    // create and use the subform
                    using (ACMForm acmForm = new ACMForm(acms))
                    {
                        acmForm.ShowDialog();
                    }
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
            BuildPreview();
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
            var subList = this.fOnlineFiles.FindAll(f => f.GetFOFileType() == FOnlineFile.FOType.DAT);
            var datList = subList.ConvertAll<Dat>(f => new Dat(f.GetTag()));
            // create and use the subform
            using (ExtractorForm extForm = new ExtractorForm(datList))
            {
                extForm.ShowDialog();
            }

            //if (inputFiles == null)
            //{
            //    MessageBox.Show("There's no input file. Make sure it's loaded first!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //else if (outDir == null)
            //{
            //    MessageBox.Show("Output directory is not selected!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //else if (cmbBoxFOFiles.SelectedItem == null)
            //{
            //    MessageBox.Show("There is no selected files for extraction! Please select one.", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            ///*else if (rightSelectedFOFile.GetFOFileType() != FOnlineFile.FOType.DAT)
            //{
            //    MessageBox.Show("Selected file is not dat archive!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}*/
            //else
            //{
            //    Predicate<FOnlineFile> predicate = new Predicate<FOnlineFile>(t => t.GetTag().Equals(cmbBoxFOFiles.SelectedItem));
            //    this.extrTargDat = (Dat)fOnlineFiles.Find(predicate);
            //    if (extrTargDat != null)
            //    {
            //        DialogResult dialogResult = MessageBox.Show("This is time consuming operation, are you sure you want to continue?", "Extracting File(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //        if (dialogResult == DialogResult.Yes)
            //        {
            //            extractor.RunWorkerAsync();
            //        }
            //    }
            //}
        }

        /// <summary>
        /// The Extractor_DoWork.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DoWorkEventArgs"/>.</param>
        private void Extractor_DoWork(object sender, DoWorkEventArgs e)
        {
            currFOFile = extrTargDat;
            // extract only archives                        
            DatDoExtractAll(extrTargDat);
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
            sb.Append("VERSION v1.0 - ETERNAL - BETA1\n");
            sb.Append("\n");
            sb.Append("PUBLIC BUILD reviewed on 2021-06-23 at 15:30).\n");
            sb.Append("This software is free software.\n");
            sb.Append("Licensed under GNU General Public License (GPL).\n");
            sb.Append("\n");
            sb.Append("Changelog for v1.0 ETERNAL:\n");
            sb.Append("\t- Fixed Out of Memory error by disallowing too many files to be opened.\n");
            sb.Append("\t- Fixed permitting unknown extensions with file open.\n");
            sb.Append("\n");
            sb.Append("Changelog for v0.4 DEUTERIUM:\n");
            sb.Append("\t- Initial pre-release.\n");
            sb.Append("\n");
            sb.Append("Purpose:\n");
            sb.Append("FOnlineRipper combines archiver/viewer/converter\n");
            sb.Append("of FOnline/Fallout 2 files in one software.\n");
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
            sb.Append("\t3. View the file(s), enjoy.\n");
            sb.Append("\n");
            sb.Append("- Extracting archive file(s):\n");
            sb.Append("\t1. Locate Fallout 2 archive (*.dat) on your filesystem.\n");
            sb.Append("\t2. Perform step \"Opening file\".\n");
            sb.Append("\t3. Choose \"Output...\" by clicking on the button.\n");
            sb.Append("\t4. Using hierarchical tree view or (and) list view find\n");
            sb.Append("\t   and select designated files for extraction.\n");
            sb.Append("\t5. Right click to extract them.\n");
            sb.Append("\t6. Wait for extraction to complete.\n");
            sb.Append("\n");
            sb.Append("- Extracting all archive files:\n");
            sb.Append("\t1. Locate Fallout 2 archive (*.dat) on your filesystem.\n");
            sb.Append("\t2. Perform step \"Opening file\".\n");
            sb.Append("\t3. Choose \"Output...\" by clicking on the button.\n");
            sb.Append("\t4. Click on \"Extract All\".\n");
            sb.Append("\t5. You will be prompted that \n");
            sb.Append("\t   this operation may take time.\n");
            sb.Append("\t6. Accept it with \"Yes\" and operation will commence.\n");
            sb.Append("\n");
            sb.Append("- Preview FRM and play ACM file(s):\n");
            sb.Append("\t1. Locate FRM, ACM whether in DAT archive\n");
            sb.Append("\t   or as standalone file(s).\n");
            sb.Append("\t2. Perform step \"Opening file\".\n");
            sb.Append("\t3. In list view right click on selected file(s)\n");
            sb.Append("\t   and choose preview.\n");
            sb.Append("\t4. FRM file(s) are gonna appear in tabs\n");
            sb.Append("\t   and ACM file(s) in one playlist.\n");
            sb.Append("\n");
            sb.Append("- Convert ACM file(s):\n");
            sb.Append("\t1. Locate ACM files whether in DAT archive\n");
            sb.Append("\t   or as standalone file(s).\n");
            sb.Append("\t2. Perform step \"Opening file\".\n");
            sb.Append("\t3. In list view right click on\n");
            sb.Append("\t   selected audio file(s) and convert.\n");
            sb.Append("\t4. ACM file(s) are gonna appear in one checklist.\n");
            sb.Append("\t5. Choose which one do you want\n");
            sb.Append("\t   to convert and press button \"CONVERT\".\n");
            sb.Append("\t6. Confirm by choosing the Output directory\n");
            sb.Append("\t   where to store them.\n");

            MessageBox.Show(sb.ToString(), "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// The convertToolStripMenuItem_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void convertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection selectedIndices = listViewDat.SelectedIndices;
            // create ACM list (sound files)
            List<ACM> acms = new List<ACM>();

            int totalMem = 0;

            if (rightSelectedFOFile != null && rightSelectedFOFile.GetFOFileType() == FOnlineFile.FOType.DAT)
            {
                Dat dat = (Dat)rightSelectedFOFile;
                foreach (int selectedIndex in selectedIndices)
                {
                    ListViewItem selItem = datListViewItems[selectedIndex];
                    KeyValuePair<FOnlineFile, Node<string>> kvp = (KeyValuePair<FOnlineFile, Node<string>>)selItem.Tag;
                    Node<string> datNode = kvp.Value;
                    DataBlock dataBlock = dat.GetDataBlock(datNode);
                    byte[] bytes = dat.Data(dataBlock);
                    string filename = dataBlock.Filename;
                    if (filename.ToLower().EndsWith(".acm"))
                    {
                        totalMem += ACM.BufferSize;
                        ACM acm = new ACM(dataBlock.Filename);                        
                        acm.ReadBytes(bytes);
                        acms.Add(acm);
                    }

                    if (totalMem > MemSize)
                    {
                        MessageBox.Show("Memory maxed out due to large amount of files!", "Memory Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                }

                if (acms.Count != 0 && totalMem <= MemSize)
                {
                    // create and use the subform
                    using (ACMConversionForm cacmForm = new ACMConversionForm(acms))
                    {
                        cacmForm.ShowDialog();
                    }
                }

            }
            else
            {
                foreach (int selectedIndex in selectedIndices)
                {
                    ListViewItem selItem = listViewDat.Items[selectedIndex];
                    KeyValuePair<FOnlineFile, Node<string>> kvp = (KeyValuePair<FOnlineFile, Node<string>>)selItem.Tag;
                    Node<string> itemNode = (Node<string>)kvp.Value;
                    string filename = itemNode.Data;
                    if (filename.ToLower().EndsWith(".acm"))
                    {
                        totalMem += ACM.BufferSize;
                        ACM acm = new ACM(inputFiles[selectedIndex]);
                        acm.ReadFile(inputFiles[selectedIndex]);
                        acms.Add(acm);
                    }

                    if (totalMem > MemSize)
                    {
                        MessageBox.Show("Memory maxed out due to large amount of files!", "Memory Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                }

                if (acms.Count != 0 && totalMem <= MemSize)
                {
                    // create and use the subform
                    using (ACMConversionForm cacmForm = new ACMConversionForm(acms))
                    {
                        cacmForm.ShowDialog();
                    }
                }
            }
        }

        /// <summary>
        /// The listViewDat_DoubleClick.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void listViewDat_DoubleClick(object sender, EventArgs e)
        {            
            IAsyncResult asyncResult = listViewDat.BeginInvoke(new Action(() =>
            {
                ListView.SelectedIndexCollection selectedIndices = listViewDat.SelectedIndices;
                foreach (int selectedIndex in selectedIndices)
                {
                    ListViewItem selItem = datListViewItems[selectedIndex];
                    KeyValuePair<FOnlineFile, Node<string>> kvp = (KeyValuePair<FOnlineFile, Node<string>>)selItem.Tag;
                    // build list view of children files for right selected fofile (if it has children)
                    if (rightSelectedFOFile != null)
                    {
                        BuildListView(rightSelectedFOFile, kvp.Value);
                    }
                    // build preview if double click the files
                    if (kvp.Value != null && kvp.Value.Children.Count == 0)
                    {
                        BuildPreview();
                    }
                }
            }));
            listViewDat.EndInvoke(asyncResult);
        }

        /// <summary>
        /// The extractToolStripMenuItem_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (outDir == null)
            {
                MessageBox.Show("Output directory is not selected!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!miniExtractor.IsBusy)
            {
                files2Extract.Clear();
                ListView.SelectedIndexCollection selectedIndices = listViewDat.SelectedIndices;
                foreach (int selectedIndex in selectedIndices)
                {
                    ListViewItem selItem = listViewDat.Items[selectedIndex];
                    KeyValuePair<FOnlineFile, Node<string>> pair = (KeyValuePair<FOnlineFile, Node<string>>)selItem.Tag;
                    if (pair.Key != null && pair.Key.GetFOFileType() == FOnlineFile.FOType.DAT)
                    {
                        files2Extract.Add(pair.Value);
                    }
                }

                if (files2Extract.Count != 0)
                {
                    miniExtractor.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show("Extracting currently in progress, Please Wait!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// The extractShortToolStripMenuItem_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void extractShortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (outDir == null)
            {
                MessageBox.Show("Output directory is not selected!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!miniExtractor.IsBusy)
            {
                files2Extract.Clear();
                // call dat to extract all
                if (treeViewDat.SelectedNode != null)
                {
                    KeyValuePair<FOnlineFile, Node<string>> pair = (KeyValuePair<FOnlineFile, Node<string>>)treeViewDat.SelectedNode.Tag;
                    leftSelectedFOFile = pair.Key;
                    Node<string> target = pair.Value;
                    files2Extract.Add(target);
                }

                if (files2Extract.Count != 0)
                {
                    miniExtractor.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show("Extracting currently in progress, Please Wait!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listViewDat_SelectedIndexChanged(object sender, EventArgs e)
        {
            leftSelectedFOFile = null;
            rightSelectedFOFile = null;
            ListView.SelectedIndexCollection selectedIndices = listViewDat.SelectedIndices;
            if (selectedIndices.Count != 0)
            {
                ListViewItem selItem = listViewDat.Items[selectedIndices[0]];
                KeyValuePair<FOnlineFile, Node<string>> pair = (KeyValuePair<FOnlineFile, Node<string>>)selItem.Tag;
                rightSelectedFOFile = pair.Key;
                Node<string> value = pair.Value;
                FOnlineFile.FOType fOType = pair.Key.GetFOFileType();
                cntxtMenuStripLong.Items[0].Enabled = fOType == FOnlineFile.FOType.ACM || fOType == FOnlineFile.FOType.FRM || value.Data.ToLower().EndsWith(".acm") || value.Data.ToLower().EndsWith(".frm");
                cntxtMenuStripLong.Items[1].Enabled = fOType == FOnlineFile.FOType.ACM || value.Data.ToLower().EndsWith(".acm");
                cntxtMenuStripLong.Items[2].Enabled = fOType == FOnlineFile.FOType.DAT || value.Data.ToLower().EndsWith(".dat");
            } 
            else
            {
                cntxtMenuStripLong.Items[0].Enabled = false;
                cntxtMenuStripLong.Items[1].Enabled = false;
                cntxtMenuStripLong.Items[2].Enabled = false;
            }
        }

        private void listViewDat_MouseClick(object sender, MouseEventArgs e)
        {
            leftSelectedFOFile = null;
            rightSelectedFOFile = null;
            ListView.SelectedIndexCollection selectedIndices = listViewDat.SelectedIndices;
            if (selectedIndices.Count != 0)
            {
                ListViewItem selItem = listViewDat.Items[selectedIndices[0]];
                KeyValuePair<FOnlineFile, Node<string>> pair = (KeyValuePair<FOnlineFile, Node<string>>)selItem.Tag;
                rightSelectedFOFile = pair.Key;
                Node<string> value = pair.Value;
                FOnlineFile.FOType fOType = pair.Key.GetFOFileType();
                cntxtMenuStripLong.Items[0].Enabled = fOType == FOnlineFile.FOType.ACM || fOType == FOnlineFile.FOType.FRM || value.Data.ToLower().EndsWith(".acm") || value.Data.ToLower().EndsWith(".frm");
                cntxtMenuStripLong.Items[1].Enabled = fOType == FOnlineFile.FOType.ACM || value.Data.ToLower().EndsWith(".acm");
                cntxtMenuStripLong.Items[2].Enabled = fOType == FOnlineFile.FOType.DAT || value.Data.ToLower().EndsWith(".dat");
            } 
            else
            {
                cntxtMenuStripLong.Items[0].Enabled = false;
                cntxtMenuStripLong.Items[1].Enabled = false;
                cntxtMenuStripLong.Items[2].Enabled = false;
            }
        }       

        private void addShortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileAdd();
        }

        private void removeShortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileRemove();
        }

        private void treeViewDat_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            
        }
    }
}
