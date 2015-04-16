<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main_Screen
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main_Screen))
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
        Me.ToolStripProgressBar1 = New System.Windows.Forms.ToolStripProgressBar
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel
        Me.ToolStripProgressBar2 = New System.Windows.Forms.ToolStripProgressBar
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.CreateFolderButton = New System.Windows.Forms.Button
        Me.DeleteAllFilesButton = New System.Windows.Forms.Button
        Me.DeleteAllContentButton = New System.Windows.Forms.Button
        Me.DeleteAllFoldersButton = New System.Windows.Forms.Button
        Me.DownloadAllFilesButton = New System.Windows.Forms.Button
        Me.DownloadAllFoldersButton = New System.Windows.Forms.Button
        Me.DownloadAllContentButton = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.LoginToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AboutSANAccessToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AutoUpdateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.CurrentSessionVariablesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.CancelUpload = New System.Windows.Forms.Button
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker
        Me.BackgroundWorker2 = New System.ComponentModel.BackgroundWorker
        Me.BackgroundWorker3 = New System.ComponentModel.BackgroundWorker
        Me.BackgroundWorker4 = New System.ComponentModel.BackgroundWorker
        Me.BackgroundWorker5 = New System.ComponentModel.BackgroundWorker
        Me.BackgroundWorker6 = New System.ComponentModel.BackgroundWorker
        Me.BackgroundWorker7 = New System.ComponentModel.BackgroundWorker
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog
        Me.BackgroundWorker8 = New System.ComponentModel.BackgroundWorker
        Me.BackgroundWorker9 = New System.ComponentModel.BackgroundWorker
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog
        Me.StatusStrip1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.ToolStripProgressBar1, Me.ToolStripStatusLabel2, Me.ToolStripProgressBar2})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 626)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(629, 22)
        Me.StatusStrip1.SizingGrip = False
        Me.StatusStrip1.TabIndex = 42
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
        '
        'ToolStripProgressBar1
        '
        Me.ToolStripProgressBar1.Name = "ToolStripProgressBar1"
        Me.ToolStripProgressBar1.Size = New System.Drawing.Size(100, 16)
        Me.ToolStripProgressBar1.Visible = False
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(0, 17)
        '
        'ToolStripProgressBar2
        '
        Me.ToolStripProgressBar2.Name = "ToolStripProgressBar2"
        Me.ToolStripProgressBar2.Size = New System.Drawing.Size(100, 16)
        Me.ToolStripProgressBar2.Step = 1
        Me.ToolStripProgressBar2.Visible = False
        '
        'CreateFolderButton
        '
        Me.CreateFolderButton.Enabled = False
        Me.CreateFolderButton.Location = New System.Drawing.Point(12, 75)
        Me.CreateFolderButton.Name = "CreateFolderButton"
        Me.CreateFolderButton.Size = New System.Drawing.Size(126, 23)
        Me.CreateFolderButton.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.CreateFolderButton, "Create folder in current working directory")
        Me.CreateFolderButton.UseVisualStyleBackColor = True
        '
        'DeleteAllFilesButton
        '
        Me.DeleteAllFilesButton.Enabled = False
        Me.DeleteAllFilesButton.Location = New System.Drawing.Point(144, 75)
        Me.DeleteAllFilesButton.Name = "DeleteAllFilesButton"
        Me.DeleteAllFilesButton.Size = New System.Drawing.Size(126, 23)
        Me.DeleteAllFilesButton.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.DeleteAllFilesButton, "Delete all files in current working directory")
        Me.DeleteAllFilesButton.UseVisualStyleBackColor = True
        '
        'DeleteAllContentButton
        '
        Me.DeleteAllContentButton.Enabled = False
        Me.DeleteAllContentButton.Location = New System.Drawing.Point(408, 75)
        Me.DeleteAllContentButton.Name = "DeleteAllContentButton"
        Me.DeleteAllContentButton.Size = New System.Drawing.Size(126, 23)
        Me.DeleteAllContentButton.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.DeleteAllContentButton, "Delete all files and folders in current working directory")
        Me.DeleteAllContentButton.UseVisualStyleBackColor = True
        '
        'DeleteAllFoldersButton
        '
        Me.DeleteAllFoldersButton.Enabled = False
        Me.DeleteAllFoldersButton.Location = New System.Drawing.Point(276, 75)
        Me.DeleteAllFoldersButton.Name = "DeleteAllFoldersButton"
        Me.DeleteAllFoldersButton.Size = New System.Drawing.Size(126, 23)
        Me.DeleteAllFoldersButton.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.DeleteAllFoldersButton, "Delete all folders in current working directory")
        Me.DeleteAllFoldersButton.UseVisualStyleBackColor = True
        '
        'DownloadAllFilesButton
        '
        Me.DownloadAllFilesButton.Enabled = False
        Me.DownloadAllFilesButton.Location = New System.Drawing.Point(12, 46)
        Me.DownloadAllFilesButton.Name = "DownloadAllFilesButton"
        Me.DownloadAllFilesButton.Size = New System.Drawing.Size(126, 23)
        Me.DownloadAllFilesButton.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.DownloadAllFilesButton, "Download all files from current working directory")
        Me.DownloadAllFilesButton.UseVisualStyleBackColor = True
        '
        'DownloadAllFoldersButton
        '
        Me.DownloadAllFoldersButton.Enabled = False
        Me.DownloadAllFoldersButton.Location = New System.Drawing.Point(144, 46)
        Me.DownloadAllFoldersButton.Name = "DownloadAllFoldersButton"
        Me.DownloadAllFoldersButton.Size = New System.Drawing.Size(126, 23)
        Me.DownloadAllFoldersButton.TabIndex = 7
        Me.ToolTip1.SetToolTip(Me.DownloadAllFoldersButton, "Download all folders from current working directory")
        Me.DownloadAllFoldersButton.UseVisualStyleBackColor = True
        '
        'DownloadAllContentButton
        '
        Me.DownloadAllContentButton.Enabled = False
        Me.DownloadAllContentButton.Location = New System.Drawing.Point(276, 46)
        Me.DownloadAllContentButton.Name = "DownloadAllContentButton"
        Me.DownloadAllContentButton.Size = New System.Drawing.Size(126, 23)
        Me.DownloadAllContentButton.TabIndex = 8
        Me.ToolTip1.SetToolTip(Me.DownloadAllContentButton, "Download all content from current working directory")
        Me.DownloadAllContentButton.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Enabled = False
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LoginToolStripMenuItem, Me.HelpToolStripMenuItem, Me.AboutToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(629, 24)
        Me.MenuStrip1.TabIndex = 55
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'LoginToolStripMenuItem
        '
        Me.LoginToolStripMenuItem.Name = "LoginToolStripMenuItem"
        Me.LoginToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.LoginToolStripMenuItem.Text = "Login"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutSANAccessToolStripMenuItem, Me.AutoUpdateToolStripMenuItem, Me.CurrentSessionVariablesToolStripMenuItem})
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(48, 20)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'AboutSANAccessToolStripMenuItem
        '
        Me.AboutSANAccessToolStripMenuItem.Name = "AboutSANAccessToolStripMenuItem"
        Me.AboutSANAccessToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
        Me.AboutSANAccessToolStripMenuItem.Text = "About SAN Access"
        '
        'AutoUpdateToolStripMenuItem
        '
        Me.AutoUpdateToolStripMenuItem.Name = "AutoUpdateToolStripMenuItem"
        Me.AutoUpdateToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
        Me.AutoUpdateToolStripMenuItem.Text = "AutoUpdate"
        '
        'CurrentSessionVariablesToolStripMenuItem
        '
        Me.CurrentSessionVariablesToolStripMenuItem.Name = "CurrentSessionVariablesToolStripMenuItem"
        Me.CurrentSessionVariablesToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
        Me.CurrentSessionVariablesToolStripMenuItem.Text = "Current Session Variables"
        '
        'WebBrowser1
        '
        Me.WebBrowser1.AllowWebBrowserDrop = False
        Me.WebBrowser1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebBrowser1.IsWebBrowserContextMenuEnabled = False
        Me.WebBrowser1.Location = New System.Drawing.Point(0, 0)
        Me.WebBrowser1.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.Size = New System.Drawing.Size(629, 484)
        Me.WebBrowser1.TabIndex = 1
        Me.WebBrowser1.Url = New System.Uri("", System.UriKind.Relative)
        Me.WebBrowser1.WebBrowserShortcutsEnabled = False
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 250
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.Controls.Add(Me.DownloadAllContentButton)
        Me.Panel1.Controls.Add(Me.DownloadAllFoldersButton)
        Me.Panel1.Controls.Add(Me.DownloadAllFilesButton)
        Me.Panel1.Controls.Add(Me.DeleteAllFoldersButton)
        Me.Panel1.Controls.Add(Me.DeleteAllContentButton)
        Me.Panel1.Controls.Add(Me.DeleteAllFilesButton)
        Me.Panel1.Controls.Add(Me.CreateFolderButton)
        Me.Panel1.Controls.Add(Me.GroupBox1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(629, 114)
        Me.Panel1.TabIndex = 57
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.ForeColor = System.Drawing.SystemColors.ControlDark
        Me.GroupBox1.Location = New System.Drawing.Point(12, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(605, 37)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        '
        'Label2
        '
        Me.Label2.AllowDrop = True
        Me.Label2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoEllipsis = True
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label2.Location = New System.Drawing.Point(6, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(593, 18)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Drag and drop the files or folders you wish to upload here"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Label2.Visible = False
        '
        'SplitContainer1
        '
        Me.SplitContainer1.BackColor = System.Drawing.Color.White
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer1.IsSplitterFixed = True
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.CancelUpload)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Panel2)
        Me.SplitContainer1.Panel1.Controls.Add(Me.WebBrowser1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.Panel1)
        Me.SplitContainer1.Size = New System.Drawing.Size(629, 602)
        Me.SplitContainer1.SplitterDistance = 484
        Me.SplitContainer1.TabIndex = 58
        '
        'CancelUpload
        '
        Me.CancelUpload.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CancelUpload.Location = New System.Drawing.Point(484, 442)
        Me.CancelUpload.Name = "CancelUpload"
        Me.CancelUpload.Size = New System.Drawing.Size(127, 43)
        Me.CancelUpload.TabIndex = 3
        Me.CancelUpload.Text = "Cancel Upload"
        Me.CancelUpload.UseVisualStyleBackColor = True
        Me.CancelUpload.Visible = False
        '
        'Panel2
        '
        Me.Panel2.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.PictureBox1)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Location = New System.Drawing.Point(124, 199)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(389, 48)
        Me.Panel2.TabIndex = 2
        Me.Panel2.Visible = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(328, 3)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(40, 39)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(14, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(316, 20)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "The application is currently busy processing. Please be patient..."
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BackgroundWorker1
        '
        '
        'BackgroundWorker2
        '
        Me.BackgroundWorker2.WorkerSupportsCancellation = True
        '
        'BackgroundWorker3
        '
        '
        'BackgroundWorker4
        '
        '
        'BackgroundWorker5
        '
        '
        'BackgroundWorker6
        '
        '
        'BackgroundWorker7
        '
        '
        'BackgroundWorker8
        '
        '
        'BackgroundWorker9
        '
        '
        'SaveFileDialog1
        '
        Me.SaveFileDialog1.AddExtension = False
        Me.SaveFileDialog1.Filter = "All files|*.*"
        Me.SaveFileDialog1.Title = "Select location for downloaded file:"
        '
        'Main_Screen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(629, 648)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Main_Screen"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WebBrowser1 As System.Windows.Forms.WebBrowser
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents LoginToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CurrentSessionVariablesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutSANAccessToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AutoUpdateToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripProgressBar1 As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripProgressBar2 As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents BackgroundWorker2 As System.ComponentModel.BackgroundWorker
    Friend WithEvents CancelUpload As System.Windows.Forms.Button
    Friend WithEvents CreateFolderButton As System.Windows.Forms.Button
    Friend WithEvents BackgroundWorker3 As System.ComponentModel.BackgroundWorker
    Friend WithEvents DeleteAllFilesButton As System.Windows.Forms.Button
    Friend WithEvents BackgroundWorker4 As System.ComponentModel.BackgroundWorker
    Friend WithEvents DeleteAllContentButton As System.Windows.Forms.Button
    Friend WithEvents BackgroundWorker5 As System.ComponentModel.BackgroundWorker
    Friend WithEvents DeleteAllFoldersButton As System.Windows.Forms.Button
    Friend WithEvents BackgroundWorker6 As System.ComponentModel.BackgroundWorker
    Friend WithEvents DownloadAllFilesButton As System.Windows.Forms.Button
    Friend WithEvents BackgroundWorker7 As System.ComponentModel.BackgroundWorker
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents DownloadAllFoldersButton As System.Windows.Forms.Button
    Friend WithEvents BackgroundWorker8 As System.ComponentModel.BackgroundWorker
    Friend WithEvents DownloadAllContentButton As System.Windows.Forms.Button
    Friend WithEvents BackgroundWorker9 As System.ComponentModel.BackgroundWorker
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog

End Class
