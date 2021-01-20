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
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MainForm" />.
    /// </summary>
    internal partial class MainForm : Form
    {
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
        /// Defines the FileIndex.
        /// </summary>
        internal const int FileIndex = 4;

        /// <summary>
        /// Defines the dat.
        /// </summary>
        private readonly Dat dat = new Dat();

        /// <summary>
        /// Defines the datfile.
        /// </summary>
        private string datfile = "";

        /// <summary>
        /// Defines the stopwatch.
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Defines the worker.
        /// </summary>
        private readonly BackgroundWorker worker = new BackgroundWorker();

        /// <summary>
        /// Defines the datCache.
        /// </summary>
        private ListViewItem[] datCache;

        /// <summary>
        /// Tells if cache miss has occurred..
        /// </summary>
        private bool datCacheMiss = true;

        /// <summary>
        /// Defines the begin index of the item block and it's always 1000 in length..
        /// </summary>
        private int datCacheIndex = 0;

        /// <summary>
        /// Defines the the dat list view items..
        /// </summary>
        private readonly List<ListViewItem> datListViewItems = new List<ListViewItem>(2000);

        /// <summary>
        /// Defines the seconds.
        /// </summary>
        private double seconds = 0.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Builds tree for the form based on data tree structure.
        /// </summary>
        private void BuildTreeView()
        {
            treeViewDat.BeginUpdate();
            // then call preorder
            List<Node<string>> datPreOrderList = dat.Tree.Preorder();

            treeViewDat.Nodes.Clear();

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
                else
                {
                    selectedNode = new TreeNode(datNode.Data, FileIndex, FileIndex);
                }

                // associate with the node dat tree
                selectedNode.Tag = datNode;
                selected.Add(selectedNode);
            }

            treeViewDat.CollapseAll();

            treeViewDat.EndUpdate();
        }

        /// <summary>
        /// Builds the list view.
        /// Tree child nodes are displayed as items in the view.
        /// </summary>
        /// <param name="datNode"> dat node to start building from .</param>
        private void BuildListView(Node<string> datNode)
        {
            datCacheMiss = true;
            datListViewItems.Clear();

            // create path list in rever se order
            Node<string> inode = datNode;
            List<string> pathList = new List<string>();
            while (inode != null)
            {
                pathList.Add(inode.Data);
                inode = inode.Parent;
            }
            pathList.Reverse();

            // build string from the list -> txtBoxPathInfo
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (string part in pathList)
            {
                sb.Append(part);
                if (index != pathList.Count)
                {
                    sb.Append("/");
                }
                index++;
            }
            txtBoxPathInfo.Text = sb.ToString();

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
            listViewDat.VirtualMode = true;
            listViewDat.VirtualListSize = datListViewItems.Count;
        }

        /// <summary>
        /// Reads the dat file, builds dat tree
        /// Basically does all. Doesn't involve any windows controls 
        /// Invoked from another thread beacuase it's expensive for perfomance.
        /// </summary>
        private void DatDoAll()
        {
            // start measuring the time
            stopwatch.Start();
            // call dat to read the file
            dat.Read(datfile);

            // call dat to buld tree structure
            dat.BuildTreeStruct();

            // stop measuring the time
            stopwatch.Stop();

            // set displayed elapsed time (in the message), measured in seconds
            seconds = stopwatch.ElapsedMilliseconds / 1000.0;

            // reset for another read or any op
            stopwatch.Reset();
        }

        /// <summary>
        /// The btnInDir_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void btnInDir_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Fallout 2 dat files (*.dat)|*.dat|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    datfile = openFileDialog.FileName;

                    worker.DoWork += Worker_DoWork;
                    worker.ProgressChanged += Worker_ProgressChanged;
                    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                    worker.WorkerReportsProgress = true;
                    worker.RunWorkerAsync();

                    IAsyncResult asyncResult = btnInDir.BeginInvoke(new Action(
                        () =>
                        {
                            DatDoAll();
                            txtBoxInArch.Text = datfile;
                            BuildTreeView();
                            BuildListView(dat.Tree.Root);
                        }
                    )
                    );
                    btnInDir.EndInvoke(asyncResult);
                }
            }
        }

        /// <summary>
        /// The Worker_RunWorkerCompleted.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RunWorkerCompletedEventArgs"/>.</param>
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Dat File sucessfully loaded in " + seconds + " seconds!", "Reading Dat File");
            this.taskProgressBar.Value = 0;
        }

        /// <summary>
        /// The Worker_ProgressChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="ProgressChangedEventArgs"/>.</param>
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.taskProgressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// The Worker_DoWork.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DoWorkEventArgs"/>.</param>
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;
            while (dat.Progress < 100.0)
            {
                int value = (int)Math.Round(dat.Progress);
                backgroundWorker.ReportProgress(value);
            }
            backgroundWorker.ReportProgress(100);
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
                BuildListView((Node<string>)e.Node.Tag);
            }));
            treeViewDat.EndInvoke(asyncResult);
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
                    BuildListView((Node<string>)selItem.Tag);
                }
            }));
            listViewDat.EndInvoke(asyncResult);
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
            if (e.StartIndex >= datCacheIndex || e.EndIndex <= datCacheIndex + datCache.Length)
            {
                return;
            }

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
    }
}
