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
        /// Required designer variable........
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
            this.lblInArchive = new System.Windows.Forms.Label();
            this.txtBoxInArch = new System.Windows.Forms.TextBox();
            this.treeViewDat = new System.Windows.Forms.TreeView();
            this.imgLstDatStruct = new System.Windows.Forms.ImageList(this.components);
            this.txtBoxOutDir = new System.Windows.Forms.TextBox();
            this.lblOutDir = new System.Windows.Forms.Label();
            this.listViewDat = new System.Windows.Forms.ListView();
            this.btnInDir = new System.Windows.Forms.Button();
            this.btnOutDir = new System.Windows.Forms.Button();
            this.btnExtract = new System.Windows.Forms.Button();
            this.taskProgressBar = new System.Windows.Forms.ProgressBar();
            this.groupBoxDatArchive = new System.Windows.Forms.GroupBox();
            this.txtBoxFileCount = new System.Windows.Forms.TextBox();
            this.txtBoxPathInfo = new System.Windows.Forms.TextBox();
            this.groupBoxWorkProgress = new System.Windows.Forms.GroupBox();
            this.groupBoxDatArchive.SuspendLayout();
            this.groupBoxWorkProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInArchive
            // 
            resources.ApplyResources(this.lblInArchive, "lblInArchive");
            this.lblInArchive.Name = "lblInArchive";
            // 
            // txtBoxInArch
            // 
            resources.ApplyResources(this.txtBoxInArch, "txtBoxInArch");
            this.txtBoxInArch.Name = "txtBoxInArch";
            this.txtBoxInArch.ReadOnly = true;
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
            // imgLstDatStruct
            // 
            this.imgLstDatStruct.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLstDatStruct.ImageStream")));
            this.imgLstDatStruct.TransparentColor = System.Drawing.Color.Transparent;
            this.imgLstDatStruct.Images.SetKeyName(0, "root_icon_closed.png");
            this.imgLstDatStruct.Images.SetKeyName(1, "root_icon_opened.png");
            this.imgLstDatStruct.Images.SetKeyName(2, "dir_icon_closed.png");
            this.imgLstDatStruct.Images.SetKeyName(3, "dir_icon_opened.png");
            this.imgLstDatStruct.Images.SetKeyName(4, "file_icon.png");
            // 
            // txtBoxOutDir
            // 
            resources.ApplyResources(this.txtBoxOutDir, "txtBoxOutDir");
            this.txtBoxOutDir.Name = "txtBoxOutDir";
            this.txtBoxOutDir.ReadOnly = true;
            // 
            // lblOutDir
            // 
            resources.ApplyResources(this.lblOutDir, "lblOutDir");
            this.lblOutDir.Name = "lblOutDir";
            // 
            // listViewDat
            // 
            this.listViewDat.HideSelection = false;
            this.listViewDat.LargeImageList = this.imgLstDatStruct;
            resources.ApplyResources(this.listViewDat, "listViewDat");
            this.listViewDat.Name = "listViewDat";
            this.listViewDat.SmallImageList = this.imgLstDatStruct;
            this.listViewDat.UseCompatibleStateImageBehavior = false;
            this.listViewDat.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.listViewDat_CacheVirtualItems);
            this.listViewDat.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listViewDat_RetrieveVirtualItem);
            this.listViewDat.DoubleClick += new System.EventHandler(this.listViewDat_DoubleClick);
            // 
            // btnInDir
            // 
            resources.ApplyResources(this.btnInDir, "btnInDir");
            this.btnInDir.Name = "btnInDir";
            this.btnInDir.UseVisualStyleBackColor = true;
            this.btnInDir.Click += new System.EventHandler(this.btnInDir_Click);
            // 
            // btnOutDir
            // 
            resources.ApplyResources(this.btnOutDir, "btnOutDir");
            this.btnOutDir.Name = "btnOutDir";
            this.btnOutDir.UseVisualStyleBackColor = true;
            // 
            // btnExtract
            // 
            resources.ApplyResources(this.btnExtract, "btnExtract");
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.UseVisualStyleBackColor = true;
            // 
            // taskProgressBar
            // 
            resources.ApplyResources(this.taskProgressBar, "taskProgressBar");
            this.taskProgressBar.Name = "taskProgressBar";
            // 
            // groupBoxDatArchive
            // 
            this.groupBoxDatArchive.Controls.Add(this.txtBoxFileCount);
            this.groupBoxDatArchive.Controls.Add(this.txtBoxPathInfo);
            this.groupBoxDatArchive.Controls.Add(this.treeViewDat);
            this.groupBoxDatArchive.Controls.Add(this.listViewDat);
            resources.ApplyResources(this.groupBoxDatArchive, "groupBoxDatArchive");
            this.groupBoxDatArchive.Name = "groupBoxDatArchive";
            this.groupBoxDatArchive.TabStop = false;
            // 
            // txtBoxFileCount
            // 
            resources.ApplyResources(this.txtBoxFileCount, "txtBoxFileCount");
            this.txtBoxFileCount.Name = "txtBoxFileCount";
            this.txtBoxFileCount.ReadOnly = true;
            // 
            // txtBoxPathInfo
            // 
            resources.ApplyResources(this.txtBoxPathInfo, "txtBoxPathInfo");
            this.txtBoxPathInfo.Name = "txtBoxPathInfo";
            this.txtBoxPathInfo.ReadOnly = true;
            // 
            // groupBoxWorkProgress
            // 
            this.groupBoxWorkProgress.Controls.Add(this.taskProgressBar);
            resources.ApplyResources(this.groupBoxWorkProgress, "groupBoxWorkProgress");
            this.groupBoxWorkProgress.Name = "groupBoxWorkProgress";
            this.groupBoxWorkProgress.TabStop = false;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxWorkProgress);
            this.Controls.Add(this.groupBoxDatArchive);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.btnOutDir);
            this.Controls.Add(this.btnInDir);
            this.Controls.Add(this.txtBoxOutDir);
            this.Controls.Add(this.lblOutDir);
            this.Controls.Add(this.txtBoxInArch);
            this.Controls.Add(this.lblInArchive);
            this.Name = "MainForm";
            this.groupBoxDatArchive.ResumeLayout(false);
            this.groupBoxDatArchive.PerformLayout();
            this.groupBoxWorkProgress.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /// <summary>
        /// Defines the lblInDir.
        /// </summary>
        private System.Windows.Forms.Label lblInArchive;

        /// <summary>
        /// Defines the txtBoxInDir.
        /// </summary>
        private System.Windows.Forms.TextBox txtBoxInArch;

        /// <summary>
        /// Defines the treeViewDat.
        /// </summary>
        private System.Windows.Forms.TreeView treeViewDat;

        /// <summary>
        /// Defines the txtBoxOutDir.
        /// </summary>
        private System.Windows.Forms.TextBox txtBoxOutDir;

        /// <summary>
        /// Defines the lblOutDir.
        /// </summary>
        private System.Windows.Forms.Label lblOutDir;

        /// <summary>
        /// Defines the listViewDat.
        /// </summary>
        private System.Windows.Forms.ListView listViewDat;

        /// <summary>
        /// Defines the btnInDir.
        /// </summary>
        private System.Windows.Forms.Button btnInDir;

        /// <summary>
        /// Defines the btnOutDir.
        /// </summary>
        private System.Windows.Forms.Button btnOutDir;

        /// <summary>
        /// Defines the btnExtract.
        /// </summary>
        private System.Windows.Forms.Button btnExtract;

        /// <summary>
        /// Defines the imgLstDatStruct.
        /// </summary>
        private System.Windows.Forms.ImageList imgLstDatStruct;

        /// <summary>
        /// Defines the taskProgressBar.
        /// </summary>
        private System.Windows.Forms.ProgressBar taskProgressBar;
        private System.Windows.Forms.GroupBox groupBoxDatArchive;
        private System.Windows.Forms.TextBox txtBoxPathInfo;
        private System.Windows.Forms.TextBox txtBoxFileCount;
        private System.Windows.Forms.GroupBox groupBoxWorkProgress;
    }
}
