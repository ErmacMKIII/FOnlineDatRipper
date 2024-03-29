﻿// Copyright (C) 2021 Alexander Stojanovich
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
        /// Required designer variable....................
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
            this.cntxtMenuStripLong = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.previewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlTree = new System.Windows.Forms.Panel();
            this.treeViewDat = new System.Windows.Forms.TreeView();
            this.cntxtMenuStripShort = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractShortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtBoxPathInfo = new System.Windows.Forms.TextBox();
            this.pnlView = new System.Windows.Forms.Panel();
            this.txtBoxFileCount = new System.Windows.Forms.TextBox();
            this.listViewDat = new System.Windows.Forms.ListView();
            this.groupBoxDatArchive = new System.Windows.Forms.GroupBox();
            this.toolStripForParentDir = new System.Windows.Forms.ToolStrip();
            this.parentToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.txtBoxOutDir = new System.Windows.Forms.TextBox();
            this.btnExtractAll = new System.Windows.Forms.Button();
            this.lblOutDir = new System.Windows.Forms.Label();
            this.lblInArchive = new System.Windows.Forms.Label();
            this.btnInArch = new System.Windows.Forms.Button();
            this.groupBoxPaths = new System.Windows.Forms.GroupBox();
            this.listBoxInputFiles = new System.Windows.Forms.ListBox();
            this.cntxtMenuListBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.remToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tblLayOutBtnPnl = new System.Windows.Forms.TableLayoutPanel();
            this.btnFilter = new System.Windows.Forms.Button();
            this.lblFilter = new System.Windows.Forms.Label();
            this.txtBoxFilter = new System.Windows.Forms.TextBox();
            this.pnlInputFiles = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxOutput = new System.Windows.Forms.GroupBox();
            this.tblLayOutPnl = new System.Windows.Forms.TableLayoutPanel();
            this.lblCurrExtrFile = new System.Windows.Forms.Label();
            this.txtBoxCurrProcFile = new System.Windows.Forms.TextBox();
            this.groupBoxWorkProgress = new System.Windows.Forms.GroupBox();
            this.taskProgressBar = new System.Windows.Forms.ProgressBar();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HowToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tblLayOutPnlBig = new System.Windows.Forms.TableLayoutPanel();
            this.cntxtMenuStripLong.SuspendLayout();
            this.pnlTree.SuspendLayout();
            this.cntxtMenuStripShort.SuspendLayout();
            this.pnlView.SuspendLayout();
            this.groupBoxDatArchive.SuspendLayout();
            this.toolStripForParentDir.SuspendLayout();
            this.groupBoxPaths.SuspendLayout();
            this.cntxtMenuListBox.SuspendLayout();
            this.tblLayOutBtnPnl.SuspendLayout();
            this.groupBoxOutput.SuspendLayout();
            this.tblLayOutPnl.SuspendLayout();
            this.groupBoxWorkProgress.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.tblLayOutPnlBig.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgLstDatStruct
            // 
            this.imgLstDatStruct.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLstDatStruct.ImageStream")));
            this.imgLstDatStruct.TransparentColor = System.Drawing.Color.Transparent;
            this.imgLstDatStruct.Images.SetKeyName(0, "root_icon_closed.png");
            this.imgLstDatStruct.Images.SetKeyName(1, "root_icon_opened.png");
            this.imgLstDatStruct.Images.SetKeyName(2, "archive_icon_closed.png");
            this.imgLstDatStruct.Images.SetKeyName(3, "archive_icon_opened.png");
            this.imgLstDatStruct.Images.SetKeyName(4, "dir_icon_closed.png");
            this.imgLstDatStruct.Images.SetKeyName(5, "dir_icon_opened.png");
            this.imgLstDatStruct.Images.SetKeyName(6, "acm_icon.png");
            this.imgLstDatStruct.Images.SetKeyName(7, "frm_icon.png");
            this.imgLstDatStruct.Images.SetKeyName(8, "file_icon.png");
            // 
            // cntxtMenuStripLong
            // 
            this.cntxtMenuStripLong.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.previewToolStripMenuItem,
            this.convertToolStripMenuItem,
            this.extractToolStripMenuItem});
            this.cntxtMenuStripLong.Name = "cntxtMenuStrip";
            resources.ApplyResources(this.cntxtMenuStripLong, "cntxtMenuStripLong");
            // 
            // previewToolStripMenuItem
            // 
            resources.ApplyResources(this.previewToolStripMenuItem, "previewToolStripMenuItem");
            this.previewToolStripMenuItem.Image = global::FOnlineDatRipper.Properties.Resources.eye_icon;
            this.previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            this.previewToolStripMenuItem.Click += new System.EventHandler(this.previewToolStripMenuItem_Click);
            // 
            // convertToolStripMenuItem
            // 
            resources.ApplyResources(this.convertToolStripMenuItem, "convertToolStripMenuItem");
            this.convertToolStripMenuItem.Image = global::FOnlineDatRipper.Properties.Resources.convert_icon;
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            this.convertToolStripMenuItem.Click += new System.EventHandler(this.convertToolStripMenuItem_Click);
            // 
            // extractToolStripMenuItem
            // 
            resources.ApplyResources(this.extractToolStripMenuItem, "extractToolStripMenuItem");
            this.extractToolStripMenuItem.Image = global::FOnlineDatRipper.Properties.Resources.extract_icon;
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            this.extractToolStripMenuItem.Click += new System.EventHandler(this.extractToolStripMenuItem_Click);
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
            this.treeViewDat.ContextMenuStrip = this.cntxtMenuStripShort;
            resources.ApplyResources(this.treeViewDat, "treeViewDat");
            this.treeViewDat.ImageList = this.imgLstDatStruct;
            this.treeViewDat.Name = "treeViewDat";
            this.treeViewDat.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDat_AfterCollapse);
            this.treeViewDat.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewDat_BeforeExpand);
            this.treeViewDat.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewDat_BeforeSelect);
            this.treeViewDat.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDat_AfterSelect);
            // 
            // cntxtMenuStripShort
            // 
            this.cntxtMenuStripShort.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractShortToolStripMenuItem});
            this.cntxtMenuStripShort.Name = "cntxtMenuStrip";
            resources.ApplyResources(this.cntxtMenuStripShort, "cntxtMenuStripShort");
            // 
            // extractShortToolStripMenuItem
            // 
            resources.ApplyResources(this.extractShortToolStripMenuItem, "extractShortToolStripMenuItem");
            this.extractShortToolStripMenuItem.Image = global::FOnlineDatRipper.Properties.Resources.extract_icon;
            this.extractShortToolStripMenuItem.Name = "extractShortToolStripMenuItem";
            this.extractShortToolStripMenuItem.Click += new System.EventHandler(this.extractShortToolStripMenuItem_Click);
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
            this.pnlView.Controls.Add(this.txtBoxFileCount);
            this.pnlView.Name = "pnlView";
            // 
            // txtBoxFileCount
            // 
            resources.ApplyResources(this.txtBoxFileCount, "txtBoxFileCount");
            this.txtBoxFileCount.Name = "txtBoxFileCount";
            this.txtBoxFileCount.ReadOnly = true;
            // 
            // listViewDat
            // 
            this.listViewDat.ContextMenuStrip = this.cntxtMenuStripLong;
            resources.ApplyResources(this.listViewDat, "listViewDat");
            this.listViewDat.GridLines = true;
            this.listViewDat.HideSelection = false;
            this.listViewDat.LargeImageList = this.imgLstDatStruct;
            this.listViewDat.Name = "listViewDat";
            this.listViewDat.SmallImageList = this.imgLstDatStruct;
            this.listViewDat.UseCompatibleStateImageBehavior = false;
            this.listViewDat.VirtualMode = true;
            this.listViewDat.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.listViewDat_CacheVirtualItems);
            this.listViewDat.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listViewDat_RetrieveVirtualItem);
            this.listViewDat.SelectedIndexChanged += new System.EventHandler(this.listViewDat_SelectedIndexChanged);
            this.listViewDat.DoubleClick += new System.EventHandler(this.listViewDat_DoubleClick);
            this.listViewDat.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewDat_MouseClick);
            // 
            // groupBoxDatArchive
            // 
            resources.ApplyResources(this.groupBoxDatArchive, "groupBoxDatArchive");
            this.groupBoxDatArchive.Controls.Add(this.listViewDat);
            this.groupBoxDatArchive.Controls.Add(this.toolStripForParentDir);
            this.groupBoxDatArchive.Controls.Add(this.pnlView);
            this.groupBoxDatArchive.Controls.Add(this.pnlTree);
            this.groupBoxDatArchive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBoxDatArchive.Name = "groupBoxDatArchive";
            this.groupBoxDatArchive.TabStop = false;
            // 
            // toolStripForParentDir
            // 
            this.toolStripForParentDir.CanOverflow = false;
            resources.ApplyResources(this.toolStripForParentDir, "toolStripForParentDir");
            this.toolStripForParentDir.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripForParentDir.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.parentToolStripButton});
            this.toolStripForParentDir.Name = "toolStripForParentDir";
            // 
            // parentToolStripButton
            // 
            this.parentToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.parentToolStripButton.Image = global::FOnlineDatRipper.Properties.Resources.dir_icon_parent;
            resources.ApplyResources(this.parentToolStripButton, "parentToolStripButton");
            this.parentToolStripButton.Name = "parentToolStripButton";
            this.parentToolStripButton.Click += new System.EventHandler(this.parentToolStripButton_Click);
            // 
            // txtBoxOutDir
            // 
            resources.ApplyResources(this.txtBoxOutDir, "txtBoxOutDir");
            this.txtBoxOutDir.Name = "txtBoxOutDir";
            this.txtBoxOutDir.ReadOnly = true;
            // 
            // btnExtractAll
            // 
            resources.ApplyResources(this.btnExtractAll, "btnExtractAll");
            this.btnExtractAll.Image = global::FOnlineDatRipper.Properties.Resources.extract_all_icon;
            this.btnExtractAll.Name = "btnExtractAll";
            this.btnExtractAll.UseVisualStyleBackColor = true;
            this.btnExtractAll.Click += new System.EventHandler(this.btnExtractAll_Click);
            // 
            // lblOutDir
            // 
            resources.ApplyResources(this.lblOutDir, "lblOutDir");
            this.lblOutDir.Name = "lblOutDir";
            // 
            // lblInArchive
            // 
            resources.ApplyResources(this.lblInArchive, "lblInArchive");
            this.lblInArchive.Name = "lblInArchive";
            // 
            // btnInArch
            // 
            resources.ApplyResources(this.btnInArch, "btnInArch");
            this.btnInArch.Image = global::FOnlineDatRipper.Properties.Resources.dir_icon_opened;
            this.btnInArch.Name = "btnInArch";
            this.btnInArch.UseVisualStyleBackColor = true;
            this.btnInArch.Click += new System.EventHandler(this.btnInArch_Click);
            // 
            // groupBoxPaths
            // 
            resources.ApplyResources(this.groupBoxPaths, "groupBoxPaths");
            this.groupBoxPaths.Controls.Add(this.listBoxInputFiles);
            this.groupBoxPaths.Controls.Add(this.lblInArchive);
            this.groupBoxPaths.Controls.Add(this.tblLayOutBtnPnl);
            this.groupBoxPaths.Controls.Add(this.lblFilter);
            this.groupBoxPaths.Controls.Add(this.txtBoxFilter);
            this.groupBoxPaths.Controls.Add(this.pnlInputFiles);
            this.groupBoxPaths.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBoxPaths.Name = "groupBoxPaths";
            this.groupBoxPaths.TabStop = false;
            // 
            // listBoxInputFiles
            // 
            this.listBoxInputFiles.ContextMenuStrip = this.cntxtMenuListBox;
            resources.ApplyResources(this.listBoxInputFiles, "listBoxInputFiles");
            this.listBoxInputFiles.FormattingEnabled = true;
            this.listBoxInputFiles.Name = "listBoxInputFiles";
            this.listBoxInputFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxInputFiles_SelectedIndexChanged);
            // 
            // cntxtMenuListBox
            // 
            this.cntxtMenuListBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.remToolStripMenuItem});
            this.cntxtMenuListBox.Name = "cntxtMenuListBox";
            resources.ApplyResources(this.cntxtMenuListBox, "cntxtMenuListBox");
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Image = global::FOnlineDatRipper.Properties.Resources.plus_file_icon;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            resources.ApplyResources(this.addToolStripMenuItem, "addToolStripMenuItem");
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // remToolStripMenuItem
            // 
            resources.ApplyResources(this.remToolStripMenuItem, "remToolStripMenuItem");
            this.remToolStripMenuItem.Image = global::FOnlineDatRipper.Properties.Resources.minus_file_icon;
            this.remToolStripMenuItem.Name = "remToolStripMenuItem";
            this.remToolStripMenuItem.Click += new System.EventHandler(this.remToolStripMenuItem_Click);
            // 
            // tblLayOutBtnPnl
            // 
            resources.ApplyResources(this.tblLayOutBtnPnl, "tblLayOutBtnPnl");
            this.tblLayOutBtnPnl.Controls.Add(this.btnInArch, 0, 0);
            this.tblLayOutBtnPnl.Controls.Add(this.btnExtractAll, 0, 1);
            this.tblLayOutBtnPnl.Controls.Add(this.btnFilter, 0, 2);
            this.tblLayOutBtnPnl.Name = "tblLayOutBtnPnl";
            // 
            // btnFilter
            // 
            resources.ApplyResources(this.btnFilter, "btnFilter");
            this.btnFilter.Image = global::FOnlineDatRipper.Properties.Resources.file_filter_icon;
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // lblFilter
            // 
            resources.ApplyResources(this.lblFilter, "lblFilter");
            this.lblFilter.Name = "lblFilter";
            // 
            // txtBoxFilter
            // 
            this.txtBoxFilter.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtBoxFilter.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            resources.ApplyResources(this.txtBoxFilter, "txtBoxFilter");
            this.txtBoxFilter.Name = "txtBoxFilter";
            this.txtBoxFilter.TextChanged += new System.EventHandler(this.txtBoxFilter_TextChanged);
            // 
            // pnlInputFiles
            // 
            resources.ApplyResources(this.pnlInputFiles, "pnlInputFiles");
            this.pnlInputFiles.Name = "pnlInputFiles";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // groupBoxOutput
            // 
            resources.ApplyResources(this.groupBoxOutput, "groupBoxOutput");
            this.groupBoxOutput.Controls.Add(this.tblLayOutPnl);
            this.groupBoxOutput.Name = "groupBoxOutput";
            this.groupBoxOutput.TabStop = false;
            // 
            // tblLayOutPnl
            // 
            resources.ApplyResources(this.tblLayOutPnl, "tblLayOutPnl");
            this.tblLayOutPnl.Controls.Add(this.lblCurrExtrFile, 0, 1);
            this.tblLayOutPnl.Controls.Add(this.txtBoxOutDir, 1, 0);
            this.tblLayOutPnl.Controls.Add(this.txtBoxCurrProcFile, 1, 1);
            this.tblLayOutPnl.Controls.Add(this.lblOutDir, 0, 0);
            this.tblLayOutPnl.Name = "tblLayOutPnl";
            // 
            // lblCurrExtrFile
            // 
            resources.ApplyResources(this.lblCurrExtrFile, "lblCurrExtrFile");
            this.lblCurrExtrFile.Name = "lblCurrExtrFile";
            // 
            // txtBoxCurrProcFile
            // 
            resources.ApplyResources(this.txtBoxCurrProcFile, "txtBoxCurrProcFile");
            this.txtBoxCurrProcFile.Name = "txtBoxCurrProcFile";
            this.txtBoxCurrProcFile.ReadOnly = true;
            // 
            // groupBoxWorkProgress
            // 
            resources.ApplyResources(this.groupBoxWorkProgress, "groupBoxWorkProgress");
            this.groupBoxWorkProgress.Controls.Add(this.taskProgressBar);
            this.groupBoxWorkProgress.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBoxWorkProgress.Name = "groupBoxWorkProgress";
            this.groupBoxWorkProgress.TabStop = false;
            // 
            // taskProgressBar
            // 
            resources.ApplyResources(this.taskProgressBar, "taskProgressBar");
            this.taskProgressBar.Name = "taskProgressBar";
            this.taskProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.mainMenuStrip, "mainMenuStrip");
            this.mainMenuStrip.Name = "mainMenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::FOnlineDatRipper.Properties.Resources.dir_icon_opened;
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::FOnlineDatRipper.Properties.Resources.exit_icon;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.HowToToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // HowToToolStripMenuItem
            // 
            this.HowToToolStripMenuItem.Name = "HowToToolStripMenuItem";
            resources.ApplyResources(this.HowToToolStripMenuItem, "HowToToolStripMenuItem");
            this.HowToToolStripMenuItem.Click += new System.EventHandler(this.HowToToolStripMenuItem_Click);
            // 
            // tblLayOutPnlBig
            // 
            resources.ApplyResources(this.tblLayOutPnlBig, "tblLayOutPnlBig");
            this.tblLayOutPnlBig.Controls.Add(this.groupBoxWorkProgress, 0, 3);
            this.tblLayOutPnlBig.Controls.Add(this.groupBoxDatArchive, 0, 1);
            this.tblLayOutPnlBig.Controls.Add(this.groupBoxOutput, 0, 2);
            this.tblLayOutPnlBig.Controls.Add(this.groupBoxPaths, 0, 0);
            this.tblLayOutPnlBig.Name = "tblLayOutPnlBig";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblLayOutPnlBig);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mainMenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.cntxtMenuStripLong.ResumeLayout(false);
            this.pnlTree.ResumeLayout(false);
            this.pnlTree.PerformLayout();
            this.cntxtMenuStripShort.ResumeLayout(false);
            this.pnlView.ResumeLayout(false);
            this.pnlView.PerformLayout();
            this.groupBoxDatArchive.ResumeLayout(false);
            this.groupBoxDatArchive.PerformLayout();
            this.toolStripForParentDir.ResumeLayout(false);
            this.toolStripForParentDir.PerformLayout();
            this.groupBoxPaths.ResumeLayout(false);
            this.groupBoxPaths.PerformLayout();
            this.cntxtMenuListBox.ResumeLayout(false);
            this.tblLayOutBtnPnl.ResumeLayout(false);
            this.tblLayOutBtnPnl.PerformLayout();
            this.groupBoxOutput.ResumeLayout(false);
            this.groupBoxOutput.PerformLayout();
            this.tblLayOutPnl.ResumeLayout(false);
            this.tblLayOutPnl.PerformLayout();
            this.groupBoxWorkProgress.ResumeLayout(false);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.tblLayOutPnlBig.ResumeLayout(false);
            this.tblLayOutPnlBig.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /// <summary>
        /// Defines the imgLstDatStruct.
        /// </summary>
        private System.Windows.Forms.ImageList imgLstDatStruct;

        /// <summary>
        /// Defines the cntxtMenuStripLong.
        /// </summary>
        private System.Windows.Forms.ContextMenuStrip cntxtMenuStripLong;

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
        /// Defines the txtBoxOutDir.
        /// </summary>
        private System.Windows.Forms.TextBox txtBoxOutDir;

        /// <summary>
        /// Defines the btnExtractAll.
        /// </summary>
        private System.Windows.Forms.Button btnExtractAll;

        /// <summary>
        /// Defines the lblOutDir.
        /// </summary>
        private System.Windows.Forms.Label lblOutDir;

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

        /// <summary>
        /// Defines the mainMenuStrip.
        /// </summary>
        private System.Windows.Forms.MenuStrip mainMenuStrip;

        /// <summary>
        /// Defines the helpToolStripMenuItem.
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;

        /// <summary>
        /// Defines the aboutToolStripMenuItem.
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;

        /// <summary>
        /// Defines the HowToToolStripMenuItem.
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem HowToToolStripMenuItem;

        /// <summary>
        /// Defines the convertToolStripMenuItem.
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem convertToolStripMenuItem;

        /// <summary>
        /// Defines the pnlInputFiles.
        /// </summary>
        private System.Windows.Forms.Panel pnlInputFiles;

        /// <summary>
        /// Defines the listBoxInputFiles.
        /// </summary>
        private System.Windows.Forms.ListBox listBoxInputFiles;

        /// <summary>
        /// Defines the cntxtMenuStripShort.
        /// </summary>
        private System.Windows.Forms.ContextMenuStrip cntxtMenuStripShort;

        /// <summary>
        /// Defines the extractShortToolStripMenuItem.
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem extractShortToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.TextBox txtBoxFilter;
        private System.Windows.Forms.TableLayoutPanel tblLayOutPnl;
        private System.Windows.Forms.Label lblCurrExtrFile;
        private System.Windows.Forms.TextBox txtBoxCurrProcFile;
        private System.Windows.Forms.GroupBox groupBoxOutput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tblLayOutPnlBig;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.TableLayoutPanel tblLayOutBtnPnl;
        private System.Windows.Forms.ContextMenuStrip cntxtMenuListBox;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem remToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStripForParentDir;
        private System.Windows.Forms.ToolStripButton parentToolStripButton;
    }
}
