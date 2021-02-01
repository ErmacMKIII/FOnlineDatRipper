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
    /// <summary>
    /// Defines the <see cref="MainForm" />.
    /// </summary>
    internal partial class MainForm
    {
        /// <summary>
        /// Required designer variable............
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.imgLstDatStruct = new System.Windows.Forms.ImageList(this.components);
            this.cntxtMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.previewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlTree = new System.Windows.Forms.Panel();
            this.treeViewDat = new System.Windows.Forms.TreeView();
            this.txtBoxPathInfo = new System.Windows.Forms.TextBox();
            this.pnlView = new System.Windows.Forms.Panel();
            this.listViewDat = new System.Windows.Forms.ListView();
            this.txtBoxFileCount = new System.Windows.Forms.TextBox();
            this.groupBoxDatArchive = new System.Windows.Forms.GroupBox();
            this.btnOutDir = new System.Windows.Forms.Button();
            this.txtBoxOutDir = new System.Windows.Forms.TextBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.lblOutDir = new System.Windows.Forms.Label();
            this.txtBoxInArch = new System.Windows.Forms.TextBox();
            this.lblInArchive = new System.Windows.Forms.Label();
            this.btnInArch = new System.Windows.Forms.Button();
            this.groupBoxPaths = new System.Windows.Forms.GroupBox();
            this.taskProgressBar = new System.Windows.Forms.ProgressBar();
            this.groupBoxWorkProgress = new System.Windows.Forms.GroupBox();
            this.cntxtMenuStrip.SuspendLayout();
            this.pnlTree.SuspendLayout();
            this.pnlView.SuspendLayout();
            this.groupBoxDatArchive.SuspendLayout();
            this.groupBoxPaths.SuspendLayout();
            this.groupBoxWorkProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgLstDatStruct
            // 
            this.imgLstDatStruct.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLstDatStruct.ImageStream")));
            this.imgLstDatStruct.TransparentColor = System.Drawing.Color.Transparent;
            this.imgLstDatStruct.Images.SetKeyName(0, "root_icon_closed.png");
            this.imgLstDatStruct.Images.SetKeyName(1, "root_icon_opened.png");
            this.imgLstDatStruct.Images.SetKeyName(2, "dir_icon_closed.png");
            this.imgLstDatStruct.Images.SetKeyName(3, "dir_icon_opened.png");
            this.imgLstDatStruct.Images.SetKeyName(4, "acm_icon.png");
            this.imgLstDatStruct.Images.SetKeyName(5, "frm_icon.png");
            this.imgLstDatStruct.Images.SetKeyName(6, "file_icon.png");
            // 
            // cntxtMenuStrip
            // 
            this.cntxtMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.previewToolStripMenuItem,
            this.extractToolStripMenuItem});
            this.cntxtMenuStrip.Name = "cntxtMenuStrip";
            resources.ApplyResources(this.cntxtMenuStrip, "cntxtMenuStrip");
            // 
            // previewToolStripMenuItem
            // 
            this.previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            resources.ApplyResources(this.previewToolStripMenuItem, "previewToolStripMenuItem");
            this.previewToolStripMenuItem.Click += new System.EventHandler(this.previewToolStripMenuItem_Click);
            // 
            // extractToolStripMenuItem
            // 
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            resources.ApplyResources(this.extractToolStripMenuItem, "extractToolStripMenuItem");
            // 
            // pnlTree
            // 
            resources.ApplyResources(this.pnlTree, "pnlTree");
            this.pnlTree.Controls.Add(this.treeViewDat);
            this.pnlTree.Controls.Add(this.txtBoxPathInfo);
            this.pnlTree.Name = "pnlTree";
            // 
            // treeViewDat
            // 
            resources.ApplyResources(this.treeViewDat, "treeViewDat");
            this.treeViewDat.ImageList = this.imgLstDatStruct;
            this.treeViewDat.Name = "treeViewDat";
            this.treeViewDat.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDat_AfterCollapse);
            this.treeViewDat.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewDat_BeforeExpand);
            this.treeViewDat.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDat_AfterSelect);
            // 
            // txtBoxPathInfo
            // 
            resources.ApplyResources(this.txtBoxPathInfo, "txtBoxPathInfo");
            this.txtBoxPathInfo.Name = "txtBoxPathInfo";
            this.txtBoxPathInfo.ReadOnly = true;
            // 
            // pnlView
            // 
            resources.ApplyResources(this.pnlView, "pnlView");
            this.pnlView.Controls.Add(this.listViewDat);
            this.pnlView.Controls.Add(this.txtBoxFileCount);
            this.pnlView.Name = "pnlView";
            // 
            // listViewDat
            // 
            this.listViewDat.ContextMenuStrip = this.cntxtMenuStrip;
            resources.ApplyResources(this.listViewDat, "listViewDat");
            this.listViewDat.GridLines = true;
            this.listViewDat.HideSelection = false;
            this.listViewDat.LargeImageList = this.imgLstDatStruct;
            this.listViewDat.Name = "listViewDat";
            this.listViewDat.SmallImageList = this.imgLstDatStruct;
            this.listViewDat.UseCompatibleStateImageBehavior = false;
            this.listViewDat.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.listViewDat_CacheVirtualItems);
            this.listViewDat.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listViewDat_RetrieveVirtualItem);
            this.listViewDat.DoubleClick += new System.EventHandler(this.listViewDat_DoubleClick);
            // 
            // txtBoxFileCount
            // 
            resources.ApplyResources(this.txtBoxFileCount, "txtBoxFileCount");
            this.txtBoxFileCount.Name = "txtBoxFileCount";
            this.txtBoxFileCount.ReadOnly = true;
            // 
            // groupBoxDatArchive
            // 
            resources.ApplyResources(this.groupBoxDatArchive, "groupBoxDatArchive");
            this.groupBoxDatArchive.Controls.Add(this.pnlView);
            this.groupBoxDatArchive.Controls.Add(this.pnlTree);
            this.groupBoxDatArchive.Name = "groupBoxDatArchive";
            this.groupBoxDatArchive.TabStop = false;
            // 
            // btnOutDir
            // 
            resources.ApplyResources(this.btnOutDir, "btnOutDir");
            this.btnOutDir.Name = "btnOutDir";
            this.btnOutDir.UseVisualStyleBackColor = true;
            this.btnOutDir.Click += new System.EventHandler(this.btnOutDir_Click);
            // 
            // txtBoxOutDir
            // 
            resources.ApplyResources(this.txtBoxOutDir, "txtBoxOutDir");
            this.txtBoxOutDir.Name = "txtBoxOutDir";
            this.txtBoxOutDir.ReadOnly = true;
            // 
            // btnExtract
            // 
            resources.ApplyResources(this.btnExtract, "btnExtract");
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // lblOutDir
            // 
            resources.ApplyResources(this.lblOutDir, "lblOutDir");
            this.lblOutDir.Name = "lblOutDir";
            // 
            // txtBoxInArch
            // 
            resources.ApplyResources(this.txtBoxInArch, "txtBoxInArch");
            this.txtBoxInArch.Name = "txtBoxInArch";
            this.txtBoxInArch.ReadOnly = true;
            // 
            // lblInArchive
            // 
            resources.ApplyResources(this.lblInArchive, "lblInArchive");
            this.lblInArchive.Name = "lblInArchive";
            // 
            // btnInArch
            // 
            resources.ApplyResources(this.btnInArch, "btnInArch");
            this.btnInArch.Name = "btnInArch";
            this.btnInArch.UseVisualStyleBackColor = true;
            this.btnInArch.Click += new System.EventHandler(this.btnInArch_Click);
            // 
            // groupBoxPaths
            // 
            resources.ApplyResources(this.groupBoxPaths, "groupBoxPaths");
            this.groupBoxPaths.Controls.Add(this.btnInArch);
            this.groupBoxPaths.Controls.Add(this.lblInArchive);
            this.groupBoxPaths.Controls.Add(this.txtBoxInArch);
            this.groupBoxPaths.Controls.Add(this.lblOutDir);
            this.groupBoxPaths.Controls.Add(this.btnExtract);
            this.groupBoxPaths.Controls.Add(this.txtBoxOutDir);
            this.groupBoxPaths.Controls.Add(this.btnOutDir);
            this.groupBoxPaths.Name = "groupBoxPaths";
            this.groupBoxPaths.TabStop = false;
            // 
            // taskProgressBar
            // 
            resources.ApplyResources(this.taskProgressBar, "taskProgressBar");
            this.taskProgressBar.Name = "taskProgressBar";
            this.taskProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // groupBoxWorkProgress
            // 
            resources.ApplyResources(this.groupBoxWorkProgress, "groupBoxWorkProgress");
            this.groupBoxWorkProgress.Controls.Add(this.taskProgressBar);
            this.groupBoxWorkProgress.Name = "groupBoxWorkProgress";
            this.groupBoxWorkProgress.TabStop = false;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxDatArchive);
            this.Controls.Add(this.groupBoxPaths);
            this.Controls.Add(this.groupBoxWorkProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MainForm";
            this.cntxtMenuStrip.ResumeLayout(false);
            this.pnlTree.ResumeLayout(false);
            this.pnlTree.PerformLayout();
            this.pnlView.ResumeLayout(false);
            this.pnlView.PerformLayout();
            this.groupBoxDatArchive.ResumeLayout(false);
            this.groupBoxDatArchive.PerformLayout();
            this.groupBoxPaths.ResumeLayout(false);
            this.groupBoxPaths.PerformLayout();
            this.groupBoxWorkProgress.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// Defines the imgLstDatStruct.
        /// </summary>
        private System.Windows.Forms.ImageList imgLstDatStruct;

        /// <summary>
        /// Defines the cntxtMenuStrip.
        /// </summary>
        private System.Windows.Forms.ContextMenuStrip cntxtMenuStrip;

        /// <summary>
        /// Defines the previewToolStripMenuItem.
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem previewToolStripMenuItem;

        /// <summary>
        /// Defines the extractToolStripMenuItem.
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;

        /// <summary>
        /// Defines the pnlTree.
        /// </summary>
        private System.Windows.Forms.Panel pnlTree;

        /// <summary>
        /// Defines the treeViewDat.
        /// </summary>
        private System.Windows.Forms.TreeView treeViewDat;

        /// <summary>
        /// Defines the txtBoxPathInfo.
        /// </summary>
        private System.Windows.Forms.TextBox txtBoxPathInfo;

        /// <summary>
        /// Defines the pnlView.
        /// </summary>
        private System.Windows.Forms.Panel pnlView;

        /// <summary>
        /// Defines the listViewDat.
        /// </summary>
        private System.Windows.Forms.ListView listViewDat;

        /// <summary>
        /// Defines the txtBoxFileCount.
        /// </summary>
        private System.Windows.Forms.TextBox txtBoxFileCount;

        /// <summary>
        /// Defines the groupBoxDatArchive.
        /// </summary>
        private System.Windows.Forms.GroupBox groupBoxDatArchive;

        /// <summary>
        /// Defines the btnOutDir.
        /// </summary>
        private System.Windows.Forms.Button btnOutDir;

        /// <summary>
        /// Defines the txtBoxOutDir.
        /// </summary>
        private System.Windows.Forms.TextBox txtBoxOutDir;

        /// <summary>
        /// Defines the btnExtract.
        /// </summary>
        private System.Windows.Forms.Button btnExtract;

        /// <summary>
        /// Defines the lblOutDir.
        /// </summary>
        private System.Windows.Forms.Label lblOutDir;

        /// <summary>
        /// Defines the txtBoxInArch.
        /// </summary>
        private System.Windows.Forms.TextBox txtBoxInArch;

        /// <summary>
        /// Defines the lblInArchive.
        /// </summary>
        private System.Windows.Forms.Label lblInArchive;

        /// <summary>
        /// Defines the btnInArch.
        /// </summary>
        private System.Windows.Forms.Button btnInArch;

        /// <summary>
        /// Defines the groupBoxPaths.
        /// </summary>
        private System.Windows.Forms.GroupBox groupBoxPaths;

        /// <summary>
        /// Defines the taskProgressBar.
        /// </summary>
        private System.Windows.Forms.ProgressBar taskProgressBar;

        /// <summary>
        /// Defines the groupBoxWorkProgress.
        /// </summary>
        private System.Windows.Forms.GroupBox groupBoxWorkProgress;
    }
}
