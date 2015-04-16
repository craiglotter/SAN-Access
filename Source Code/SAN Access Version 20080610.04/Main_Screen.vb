Imports System.IO
Imports System.Threading
Imports System.ComponentModel
Imports System.Text
Imports System.Security.Cryptography
Imports Microsoft.Win32
Imports System.Net
Imports System.Diagnostics

'System.Net.WebException
'The remote server returned an error: (530) Not logged in.


Public Class Main_Screen

    Private lastnavigated As String = Format(Now, "yyyymmddHHmmss")

    Private currentusername As String = ""
    Private currentpassword As String = ""
    Private currentfolder As String = ""
    Private currentuploadfolder As String = ""
    Private currentstaffswitch As String = ""


    Private tempfile As String = (Application.StartupPath & "\temp").Replace("\\", "\")
    Private md5status As String = (Application.StartupPath & "\md5").Replace("\\", "\")
    Private existstatus As String = (Application.StartupPath & "\exist").Replace("\\", "\")
    Private usagestatus As String = (Application.StartupPath & "\usage").Replace("\\", "\")

    Private WebPageDisplay1 As WebPageDisplay

    Private AutoUpdate As Boolean = False
    Private savesettingsController As Boolean = True

    Private precountsize As Long
    Private precountitems As Integer
    Private precountfiles As Integer
    Private precountfolders As Integer
    Private uploadedprecountfiles As Integer
    Private uploadedprecountfolders As Integer
    Private uploadedprecountitems As Integer

    Private downloadedprecountfiles As Integer
    Private downloadedprecountfolders As Integer
    Private downloadedprecountitems As Integer

    Private isBusy As Boolean = False
    Private last_File_List_URL As String = ""




    Private Sub Error_Handler(ByVal ex As Exception, Optional ByVal identifier_msg As String = "")
        Try
            Dim error_handled As Boolean = False

            If ex.Message.ToString = "The remote server returned an error: (530) Not logged in." Then
                MsgBox("Password Synchronisation Error Encountered: It would seem that you have recently changed your password and there hasn't been enough time for the SAN Access Manager to synchronise the passwords on the system yet. This process may take up to 15 minutes to complete, but please feel free to use the HTTP Upload Component located on the My Files page.", MsgBoxStyle.Exclamation, "Password Synchronisation Error")
                error_handled = True
            End If

            If error_handled = False Then
                If ex.Message.IndexOf("Thread was being aborted") < 0 Then
                    Dim Display_Message1 As New Display_Message()
                    'Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ": " & ex.Message.ToString
                    Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ": " & ex.Message.ToString

                    Display_Message1.Timer1.Interval = 1000
                    Display_Message1.ShowDialog()
                    Dim dir As System.IO.DirectoryInfo = New System.IO.DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
                    If dir.Exists = False Then
                        dir.Create()
                    End If
                    dir = Nothing
                    Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
                    filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & identifier_msg & ": " & ex.ToString)
                    filewriter.WriteLine("")
                    filewriter.Flush()
                    filewriter.Close()
                    filewriter = Nothing
                End If
            End If
            ex = Nothing
            identifier_msg = Nothing
        Catch exc As Exception
            MsgBox("An error occurred in the application's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

    Private Sub Form1_Close(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        Try
            Me.ToolStripStatusLabel1.Text = "Application Closing"
            If savesettingsController = True Then
                SaveSettings()
            End If
            WebBrowser1.Navigate("http://comsan.uct.ac.za/Web/SAN Access/logout.php")
            If AutoUpdate = True Then
                If My.Computer.FileSystem.FileExists((Application.StartupPath & "\AutoUpdate.exe").Replace("\\", "\")) = True Then
                    Dim startinfo As ProcessStartInfo = New ProcessStartInfo
                    startinfo.FileName = (Application.StartupPath & "\AutoUpdate.exe").Replace("\\", "\")
                    startinfo.Arguments = "force"
                    startinfo.CreateNoWindow = False
                    Process.Start(startinfo)
                End If
            End If
        Catch ex As Exception
            Error_Handler(ex, "Application Close")
        End Try
    End Sub

    Private Sub LoadSettings()
        Try
            Dim configfile As String = (Application.StartupPath & "\config.sav").Replace("\\", "\")
            If My.Computer.FileSystem.FileExists(configfile) Then
                Dim reader As StreamReader = New StreamReader(configfile)
                Dim lineread As String
                Dim variablevalue As String
                While reader.Peek <> -1
                    lineread = reader.ReadLine
                    If lineread.IndexOf("=") <> -1 Then

                        variablevalue = lineread.Remove(0, lineread.IndexOf("=") + 1)

                        If lineread.StartsWith("currentusername=") Then
                            currentusername = variablevalue
                            If currentusername.IndexOf("/") <> -1 Then
                                currentusername = currentusername.Substring(0, currentusername.IndexOf("/"))
                            End If
                            If currentusername.IndexOf("\") <> -1 Then
                                currentusername = currentusername.Substring(0, currentusername.IndexOf("\"))
                            End If
                            If currentusername.Length = 9 Then
                                currentstaffswitch = "Students"
                            Else
                                currentstaffswitch = "Staff"
                            End If
                        End If

         
                    End If
                End While
                reader.Close()
                reader = Nothing
            End If
        Catch ex As Exception
            Error_Handler(ex, "Load Settings")
        End Try
    End Sub

    Private Sub SaveSettings()
        Try
            Dim configfile As String = (Application.StartupPath & "\config.sav").Replace("\\", "\")

            Dim writer As StreamWriter = New StreamWriter(configfile, False)

            
            writer.WriteLine("currentusername=" & currentusername)
            

            writer.Flush()
            writer.Close()
            writer = Nothing

        Catch ex As Exception
            Error_Handler(ex, "Save Settings")
        End Try
    End Sub

    Public Function ReturnRegKeyValue(ByVal MainKey As String, ByVal RequestedKey As String, ByVal Value As String) As String
        Dim result As String = "Fail."
        Try
            Dim oReg As RegistryKey
            Dim regkey As RegistryKey
            Try
                Select Case MainKey.ToUpper
                    Case "HKEY_CURRENT_USER"
                        oReg = Registry.CurrentUser
                    Case "HKEY_CLASSES_ROOT"
                        oReg = Registry.ClassesRoot
                    Case "HKEY_LOCAL_MACHINE"
                        oReg = Registry.LocalMachine
                    Case "HKEY_USERS"
                        oReg = Registry.Users
                    Case "HKEY_CURRENT_CONFIG"
                        oReg = Registry.CurrentConfig
                    Case Else
                        oReg = Registry.LocalMachine
                End Select

                regkey = oReg
                oReg.Close()
                If RequestedKey.EndsWith("\") = True Then
                    RequestedKey = RequestedKey.Remove(RequestedKey.Length - 1, 1)
                End If
                Dim subs() As String = (RequestedKey).Split("\")

                Dim doContinue As Boolean = True
                For Each stri As String In subs
                    If doContinue = False Then
                        Exit For
                    End If
                    If regkey Is Nothing = False Then
                        Dim skn As String() = regkey.GetSubKeyNames()
                        Dim strin As String

                        doContinue = False
                        For Each strin In skn
                            If stri = strin Then
                                regkey = regkey.OpenSubKey(stri, True)
                                doContinue = True
                                Exit For
                            End If
                        Next
                    End If
                Next
                If doContinue = True Then
                    If regkey Is Nothing = False Then
                        Dim str As String() = regkey.GetValueNames()
                        Dim val As String
                        Dim foundit As Boolean = False
                        For Each val In str
                            If Value = val Then
                                foundit = True
                                result = regkey.GetValue(Value)
                                Exit For
                            End If
                        Next
                        If foundit = False Then
                            result = "Fail. Could not locate Value within Registry Key"
                        End If
                        regkey.Close()
                    End If
                Else
                    result = "Fail. Key cannot be located"
                End If
            Catch ex As Exception
                Error_Handler(ex, "ReturnRegKeyValue")
                result = "Fail. Check Error Log for further details"
            End Try
        Catch ex As Exception
            Error_Handler(ex, "ReturnRegKeyValue")
            result = "Fail. Check Error Log for further details"
        End Try
        Return result
    End Function

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Control.CheckForIllegalCrossThreadCalls = False
            Me.Text = My.Application.Info.ProductName & " " & Format(My.Application.Info.Version.Major, "0000") & Format(My.Application.Info.Version.Minor, "00") & Format(My.Application.Info.Version.Build, "00") & "." & Format(My.Application.Info.Version.Revision, "00") & ""
            AboutSANAccessToolStripMenuItem.Text = "About " & My.Application.Info.ProductName
            If My.Computer.FileSystem.FileExists((Application.StartupPath & "\Images\Monitoring-Animation.gif").Replace("\\", "\")) = True Then
                Me.PictureBox1.Image = Image.FromFile((Application.StartupPath & "\Images\Monitoring-Animation.gif").Replace("\\", "\"))
            End If
            If Process.GetProcessesByName("SAN Access").Length > 1 Then
                Me.WindowState = FormWindowState.Minimized
                Dim Display_Message1 As New Display_Message()
                Display_Message1.Message_Textbox.Text = "Sorry, but another instance of " & My.Application.Info.ProductName & " appears to be running on your machine. This instance of the application will now automatically shutdown."
                Display_Message1.Timer1.Interval = 1000
                Display_Message1.ShowDialog()
                savesettingsController = False
                Me.Close()
            Else
                Dim currentuser As String = ""
                Dim currentcontext As String = ""
                currentuser = ReturnRegKeyValue("HKEY_CURRENT_USER", "Volatile Environment", "NWUSERNAME")
                If currentuser.StartsWith("Fail.") = True Or currentuser.StartsWith("Failure") = True Then
                    currentuser = ""
                End If
                If currentuser.Length <> 8 Then
                    currentcontext = "Student"
                Else
                    currentcontext = "Staff"
                End If
                If currentuser = "" Then
                    LoadSettings()
                    currentuser = currentusername
                    currentcontext = currentstaffswitch
                End If
                WebBrowser1.Navigate("http://comsan.uct.ac.za/Web/SAN Access/setusername.asp?str_currentuser=" & currentuser & "&str_currentcontext=" & currentcontext)
                MenuStrip1.Enabled = True

                Me.ToolStripStatusLabel1.Text = "Application Loaded"
            End If
        Catch ex As Exception
            Error_Handler(ex, "Application Load")
        End Try

    End Sub

    Private Sub HelpToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HelpToolStripMenuItem.Click
        Try
            Me.ToolStripStatusLabel1.Text = "Help displayed"
            '    HelpBox1.ShowDialog()
            Process.Start("http://comsan.uct.ac.za/Web/SAN Access/Help")
        Catch ex As Exception
            Error_Handler(ex, "Display Help Screen")
        End Try
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Try
            'ToolStripStatusLabel2.Text = lastnavigated
            'If (Long.Parse(Format(Now, "yyyymmddHHmmss")) - Long.Parse(lastnavigated)) > 300 Then
            '    WebBrowser1.Refresh()
            'End If

            If WebBrowser1.IsBusy = False Then
                If Panel2.Visible = False Then
                    Me.WebBrowser1.AllowNavigation = True
                End If
                Me.ToolStripProgressBar1.Visible = False
                If WebBrowser1.Url <> Nothing Then
                    If WebBrowser1.Url.ToString <> "about:blank" Then
                        Dim strurl As String = WebBrowser1.Url.ToString
                        strurl = strurl.Remove(0, strurl.LastIndexOf("/") + 1)
                        strurl = strurl.Substring(0, strurl.LastIndexOf("."))
                        If (strurl.IndexOf(".") > 0) Then
                            strurl = strurl.Substring(0, strurl.IndexOf("."))
                        End If
                        Me.ToolStripStatusLabel1.Text = "Page Loaded (" & Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(strurl.Replace("_", " ")) & ")"
                        If Me.ToolStripStatusLabel1.Text = "Page Loaded (My SAN File List)" = True Then
                            last_File_List_URL = WebBrowser1.Url.ToString
                            If My.Computer.FileSystem.FileExists(tempfile) = False Then
                                runUploadInitialiser()
                            End If
                        Else
                            Me.ToolStripStatusLabel2.Text = ""
                            Me.ToolStripProgressBar2.Visible = False
                            GroupBox1.Text = ""
                            CreateFolderButton.Enabled = False
                            CreateFolderButton.Text = ""
                            DeleteAllFilesButton.Enabled = False
                            DeleteAllFilesButton.Text = ""
                            DeleteAllFoldersButton.Enabled = False
                            DeleteAllFoldersButton.Text = ""
                            DeleteAllContentButton.Enabled = False
                            DeleteAllContentButton.Text = ""
                            DownloadAllFilesButton.Enabled = False
                            DownloadAllFilesButton.Text = ""
                            DownloadAllFoldersButton.Enabled = False
                            DownloadAllFoldersButton.Text = ""
                            DownloadAllContentButton.Enabled = False
                            DownloadAllContentButton.Text = ""
                            Label2.Visible = False
                            Label2.Enabled = False
                            If isBusy = False Then
                                If Me.ToolStripStatusLabel1.Text = "Page Loaded (File Download Requested)" = True Then
                                    isBusy = True
                                    SingleFileDownload(last_File_List_URL)
                                End If
                            End If
                            'currentusername = ""
                            'currentpassword = ""
                            If My.Computer.FileSystem.FileExists(tempfile) = True Then
                                My.Computer.FileSystem.DeleteFile(tempfile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently, FileIO.UICancelOption.DoNothing)
                            End If

                            End If

                    Else
                            Me.ToolStripStatusLabel1.Text = "Loading..."
                        End If
                    Else
                        Me.ToolStripStatusLabel1.Text = "Loading..."
                    End If
            Else
                Me.ToolStripStatusLabel1.Text = "Loading..."
                Me.ToolStripProgressBar1.Visible = True
                Me.ToolStripProgressBar1.PerformStep()
                If Me.ToolStripProgressBar1.Value = Me.ToolStripProgressBar1.Maximum Then
                    Me.ToolStripProgressBar1.Value = 0
                End If
                Me.WebBrowser1.AllowNavigation = False
            End If

        Catch ex As Exception
            Error_Handler(ex, "Broadcast Browser URL")
        End Try
    End Sub

    Private Sub LoginToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoginToolStripMenuItem.Click
        Try
            WebBrowser1.Navigate("http://comsan.uct.ac.za/Web/SAN Access/logout.php")
        Catch ex As Exception
            Error_Handler(ex, "Browser Login")
        End Try
    End Sub

    Private Sub CurrentSessionVariablesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CurrentSessionVariablesToolStripMenuItem.Click
        Try
            Dim url As String = "http://comsan.uct.ac.za/Web/SAN Access/getsessionvariables.php"

            If Not IsNothing(WebPageDisplay1) Then
                If Not WebPageDisplay1.IsDisposed Then
                    WebPageDisplay1.WindowState = FormWindowState.Normal  ' Optional
                    WebPageDisplay1.BringToFront()  '  Optional
                    WebPageDisplay1.WebBrowser1.Navigate(url)
                Else
                    WebPageDisplay1 = New WebPageDisplay()
                    WebPageDisplay1.WebBrowser1.Navigate(url)
                    WebPageDisplay1.Show()
                    WebPageDisplay1.WindowState = FormWindowState.Normal
                    WebPageDisplay1.BringToFront()
                    WebPageDisplay1.Refresh()
                End If
            Else
                WebPageDisplay1 = New WebPageDisplay()
                WebPageDisplay1.WebBrowser1.Navigate(url)
                WebPageDisplay1.Show()
                WebPageDisplay1.WindowState = FormWindowState.Normal
                WebPageDisplay1.BringToFront()
                WebPageDisplay1.Refresh()
            End If

        Catch ex As Exception
            Error_Handler(ex, "Display Current Session Variables")
        End Try
    End Sub


    Private Sub AboutSANAccessToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutSANAccessToolStripMenuItem.Click
        Try
            Me.ToolStripStatusLabel1.Text = "About displayed"
            AboutBox1.ShowDialog()
        Catch ex As Exception
            Error_Handler(ex, "Display About Screen")
        End Try
    End Sub

    Private Sub AutoUpdateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutoUpdateToolStripMenuItem.Click
        Try
            AutoUpdate = True
            Me.Close()
        Catch ex As Exception
            Error_Handler(ex, "AutoUpdate")
        End Try
    End Sub


    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try

            currentusername = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
            If currentusername.Length = 9 Then
                currentstaffswitch = "Students"
            Else
                currentstaffswitch = "Staff"
            End If

            If WebBrowser1.Url.ToString.ToLower.StartsWith("http://comsan.uct.ac.za/web/san access/my_san_file_list.php") Then
                If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                    currentfolder = currentstaffswitch & "\" & WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
                Else
                    currentfolder = currentstaffswitch & "\" & WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
                End If
            End If

            My.Computer.Network.DownloadFile("http://comsan.uct.ac.za/Web/SAN Access/getclearpass.php?number=" & currentusername, tempfile, "", "", False, 100000, True)
            If My.Computer.FileSystem.FileExists(tempfile) = True Then
                Dim reader As StreamReader = New StreamReader(tempfile)
                If reader.Peek <> -1 Then
                    currentpassword = reader.ReadLine()
                    currentpassword = currentpassword.Trim
                End If
                reader.Close()
                reader = Nothing
                Dim writer As StreamWriter = New StreamWriter(tempfile, False)
                writer.WriteLine()
                writer.Close()
                writer = Nothing
            End If

        Catch ex As Exception
            Error_Handler(ex, "Initialise Uploader Control")
            Label2.Text = "Failed to initialise uploader"
        End Try
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If e.Error Is Nothing Then
            GroupBox1.Text = "Upload your files"
            CreateFolderButton.Enabled = True
            CreateFolderButton.Text = "Create Folder"
            DeleteAllFilesButton.Enabled = True
            DeleteAllFilesButton.Text = "Delete All Files"
            DeleteAllFoldersButton.Enabled = True
            DeleteAllFoldersButton.Text = "Delete All Folders"
            DeleteAllContentButton.Enabled = True
            DeleteAllContentButton.Text = "Delete All Content"
            DownloadAllFilesButton.Enabled = True
            DownloadAllFilesButton.Text = "Download All Files"
            DownloadAllFoldersButton.Enabled = True
            DownloadAllFoldersButton.Text = "Download All Folders"
            DownloadAllContentButton.Enabled = True
            DownloadAllContentButton.Text = "Download All Content"

            Label2.Text = "Drag and drop the files or folders you wish to upload here"
        Else
            GroupBox1.Text = ""
            CreateFolderButton.Enabled = False
            CreateFolderButton.Text = ""
            DeleteAllFilesButton.Enabled = False
            DeleteAllFilesButton.Text = ""
            DeleteAllFoldersButton.Enabled = False
            DeleteAllFoldersButton.Text = ""
            DeleteAllContentButton.Enabled = False
            DeleteAllContentButton.Text = ""
            DownloadAllFilesButton.Enabled = False
            DownloadAllFilesButton.Text = ""
            DownloadAllFoldersButton.Enabled = False
            DownloadAllFoldersButton.Text = ""
            DownloadAllContentButton.Enabled = False
            DownloadAllContentButton.Text = ""
            Label2.Text = "Uploader component failed to initialize"
        End If
        controls_enabler(True)
        Timer1.Start()
    End Sub

    Private Sub WebBrowser1_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        Try
            If currentusername.Length > 0 Then
                If currentusername.Length = 9 Then
                    currentstaffswitch = "Students"
                Else
                    currentstaffswitch = "Staff"
                End If
            End If
            If WebBrowser1.Url.ToString.ToLower.StartsWith("http://comsan.uct.ac.za/web/san access/my_san_file_list.php") Then
                If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                    currentfolder = currentstaffswitch & "\" & WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
                Else
                    currentfolder = currentstaffswitch & "\" & WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
                End If
            End If
        Catch ex As Exception
            Error_Handler(ex, "Document Loaded")
        End Try
    End Sub

    Private Sub runUploadInitialiser()
        Try
            Timer1.Stop()
            controls_enabler(False)
            GroupBox1.Text = ""
            CreateFolderButton.Enabled = False
            CreateFolderButton.Text = ""
            DeleteAllFilesButton.Enabled = False
            DeleteAllFilesButton.Text = ""
            DeleteAllFoldersButton.Enabled = False
            DeleteAllFoldersButton.Text = ""
            DeleteAllContentButton.Enabled = False
            DeleteAllContentButton.Text = ""
            DownloadAllFilesButton.Enabled = False
            DownloadAllFilesButton.Text = ""
            DownloadAllFoldersButton.Enabled = False
            DownloadAllFoldersButton.Text = ""
            DownloadAllContentButton.Enabled = False
            DownloadAllContentButton.Text = ""
            Label2.Visible = True
            Label2.Text = "Initialising uploader..."
            GroupBox1.Refresh()
            BackgroundWorker1.RunWorkerAsync()
        Catch ex As Exception
            Error_Handler(ex, "Run Upload Initializer")
        End Try
    End Sub

    '**********************************************************************************
    '----------------------------------------------------------------------------------
    '**********************************************************************************
    'Uploading Files
    '**********************************************************************************
    '----------------------------------------------------------------------------------
    '**********************************************************************************

    Private Sub UploadLabel_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Label2.DragEnter
        Try
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                e.Effect = DragDropEffects.All
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Private Sub UploadLabel_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Label2.DragDrop
        Try
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Dim MyFiles() As String
                MyFiles = e.Data.GetData(DataFormats.FileDrop)
                Uploader(MyFiles)
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Private Sub Uploader(ByVal MyFiles() As String)
        Try
            controls_enabler(False, "upload")
            precountitems = 0
            precountsize = 0
            precountfiles = 0
            precountfolders = 0
            uploadedprecountitems = 0
            uploadedprecountfiles = 0
            uploadedprecountfolders = 0
            ToolStripProgressBar2.Value = 0
            ToolStripProgressBar2.Visible = True
            ToolStripStatusLabel2.Text = "Initializing Uploads..."
            BackgroundWorker2.RunWorkerAsync(MyFiles)
        Catch ex As Exception
            Error_Handler(ex, "FTP Uploader")
        End Try
    End Sub

    Private Sub RecursivePrecount(ByVal target As String)
        Try
            Dim finfo As FileInfo = New FileInfo(target)
            If finfo.Exists = True Then
                precountitems = precountitems + 1
                precountsize = precountsize + finfo.Length
                precountfiles = precountfiles + 1
            End If
            finfo = Nothing
            Dim dinfo As DirectoryInfo = New DirectoryInfo(target)
            If dinfo.Exists = True Then
                precountfolders = precountfolders + 1
                precountitems = precountitems + 1
                For Each ff As FileInfo In dinfo.GetFiles
                    precountitems = precountitems + 1
                    precountsize = precountsize + ff.Length
                    precountfiles = precountfiles + 1
                    ff = Nothing
                Next
                For Each dd As DirectoryInfo In dinfo.GetDirectories
                    RecursivePrecount(dd.FullName)
                    dd = Nothing
                Next
            End If
            dinfo = Nothing
        Catch ex As Exception
            Error_Handler(ex, "Recursive Precount")
        End Try
    End Sub

    Private Sub Precount(ByVal MyFiles() As String)
        If MyFiles.Length > 0 Then
            For i As Integer = 0 To MyFiles.Length - 1
                RecursivePrecount(MyFiles(i))
            Next
        End If
    End Sub

    Private Sub controls_enabler(ByVal enable As Boolean, Optional ByVal whichfunction As String = "")
        Try
            Select Case enable
                Case True
                    MenuStrip1.Enabled = True
                    WebBrowser1.AllowNavigation = True
                    Label2.Enabled = True
                    Panel2.Visible = False
                    Me.ControlBox = True
                    CreateFolderButton.Enabled = True
                    DeleteAllFilesButton.Enabled = True
                    DeleteAllContentButton.Enabled = True
                    DeleteAllFoldersButton.Enabled = True
                    DownloadAllFilesButton.Enabled = True
                    DownloadAllFoldersButton.Enabled = True
                    DownloadAllContentButton.Enabled = True
                    Select Case whichfunction
                        Case "upload"
                            CancelUpload.Enabled = False
                            CancelUpload.Visible = False
                    End Select
                Case False
                    MenuStrip1.Enabled = False
                    WebBrowser1.AllowNavigation = False
                    Label2.Enabled = False
                    Panel2.Visible = True
                    Me.ControlBox = False
                    CreateFolderButton.Enabled = False
                    DeleteAllFilesButton.Enabled = False
                    DeleteAllContentButton.Enabled = False
                    DeleteAllFoldersButton.Enabled = False
                    DownloadAllFilesButton.Enabled = False
                    DownloadAllFoldersButton.Enabled = False
                    DownloadAllContentButton.Enabled = False
                    Select Case whichfunction
                        Case "upload"
                            CancelUpload.Enabled = True
                            CancelUpload.Visible = True
                    End Select
            End Select
        Catch ex As Exception
            Error_Handler(ex, "Controls Enabler")
        End Try
    End Sub


    Private Sub BackgroundWorker2_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        Try
            Dim filelimitexceeded As Boolean = True

            Dim MyFiles() As String = e.Argument()
            Label2.Text = "Running precount function..."
            Precount(MyFiles)
            If BackgroundWorker2.CancellationPending = True Then
                e.Cancel = True
                Exit Sub
            End If
            
            Label2.Text = "Checking against file size limit..."
            My.Computer.Network.DownloadFile("http://comsan.uct.ac.za/Web/SAN Access/returnUsageStat.php?username=" & currentusername & "&filename=" & currentstaffswitch & "\" & currentusername, usagestatus, "", "", False, 100000, True)
            Dim lineread As String = ""
            Dim lineread2 As String = ""
            If My.Computer.FileSystem.FileExists(usagestatus) = True Then
                Dim reader As StreamReader = New StreamReader(usagestatus)
                If reader.Peek <> -1 Then
                    lineread = reader.ReadLine().Trim
                    lineread2 = reader.ReadLine().Trim
                End If
                reader.Close()
                reader = Nothing
                My.Computer.FileSystem.DeleteFile(usagestatus)
            End If
            'MsgBox(lineread)
            'MsgBox(lineread2)
            If (Long.Parse(lineread) + precountsize) <= Long.Parse(lineread2) Then
                filelimitexceeded = False
            End If
            If BackgroundWorker2.CancellationPending = True Then
                e.Cancel = True
                Exit Sub
            End If
            If filelimitexceeded = True Then
                MsgBox("Sorry, but the files that you are trying to upload to your SAN folder would cause you to exceed your size limit. It is suggested that you either upload less files or remove some existing files from your folder.", MsgBoxStyle.Information, "Size Limit Exceeded")
            Else

                Dim i As Integer
                If MyFiles.Length > 0 Then
                    Label2.Text = "Launching uploader component..."
                    Dim uploadpath As String
                    If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                        uploadpath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
                    Else
                        uploadpath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
                    End If
                    uploadpath = uploadpath.Remove(0, currentusername.Length)
                    uploadpath = ("ftp:////comsan.uct.ac.za/" & uploadpath).Replace("\", "/").Replace("//", "/")
                    currentuploadfolder = currentfolder
                    For i = 0 To MyFiles.Length - 1
                        If BackgroundWorker2.CancellationPending = True Then
                            e.Cancel = True
                            Exit Sub
                        End If
                        RecursiveUploader(e, MyFiles(i), uploadpath, currentuploadfolder)
                        If BackgroundWorker2.CancellationPending = True Then
                            e.Cancel = True
                            Exit Sub
                        End If
                    Next
                End If
            End If

        Catch ex As Exception
            Error_Handler(ex, "Upload File Process")
        End Try

    End Sub


    Private Sub RecursiveUploader(ByVal e As DoWorkEventArgs, ByVal TargetUpload As String, ByVal UploadPath As String, ByVal recursivecurrentuploadfolder As String)
        Try
            If BackgroundWorker2.CancellationPending = True Then
                e.Cancel = True
                Exit Sub
            End If
            Dim precountfileline, precountfolderline As String
            If uploadedprecountfolders <> 1 Then
                precountfolderline = " folders"
            Else
                precountfolderline = " folder"
            End If
            If uploadedprecountfiles <> 1 Then
                precountfileline = " files"
            Else
                precountfileline = " file"
            End If
            If precountfiles = 0 And precountfolders > 0 Then
                ToolStripStatusLabel2.Text = "Last Upload Count: " & uploadedprecountfolders & " (of " & precountfolders & ")" & precountfolderline
            End If
            If precountfolders = 0 And precountfiles > 0 Then
                ToolStripStatusLabel2.Text = "Last Upload Count: " & uploadedprecountfiles & " (of " & precountfiles & ")" & precountfileline
            End If
            If precountfiles > 0 And precountfolders > 0 Then
                ToolStripStatusLabel2.Text = "Last Upload Count: " & uploadedprecountfolders & " (of " & precountfolders & ")" & precountfolderline & " | " & uploadedprecountfiles & " (of " & precountfiles & ")" & precountfileline
            End If
            If precountfiles = 0 And precountfolders = 0 Then
                ToolStripStatusLabel2.Text = "Uploading File"
            End If


            If BackgroundWorker2.CancellationPending = True Then
                e.Cancel = True
                Exit Sub
            End If

            ' MsgBox("On entry:" & vbCrLf & "Upload Path: " & UploadPath & vbCrLf & "Currentuploadfolder: " & recursivecurrentuploadfolder)
            Dim performUpload As Boolean = False
            Dim finfo As FileInfo = New FileInfo(TargetUpload)
            If finfo.Exists = True Then
                Try
                    'ToolStripStatusLabel2.Text = "Uploading File"
                    Label2.Text = "Uploading " & finfo.FullName
                    My.Computer.Network.DownloadFile("http://comsan.uct.ac.za/Web/SAN Access/doesfileexist.php?filename=" & recursivecurrentuploadfolder.Replace("/", "\") & "\" & StripSpecialCharacters(finfo.Name), existstatus, "", "", False, 100000, True)
                    If My.Computer.FileSystem.FileExists(existstatus) = True Then
                        Dim reader As StreamReader = New StreamReader(existstatus)
                        Dim lineread As String = reader.ReadLine().Trim
                        ' MsgBox(lineread)
                        If reader.Peek <> -1 Then
                            If (lineread = "FALSE") Then
                                performUpload = True
                            Else
                                If MsgBox("The file you are trying to upload (" & StripSpecialCharacters(finfo.Name) & ") already exists. Do you wish to overwrite the existing file?", MsgBoxStyle.Information + MsgBoxStyle.YesNo, "File already exists") = MsgBoxResult.Yes Then
                                    performUpload = True
                                Else
                                    performUpload = False
                                End If
                            End If
                        End If
                        reader.Close()
                        reader = Nothing
                        My.Computer.FileSystem.DeleteFile(existstatus)
                    End If

                    If performUpload = True Then
                        UploadPath = (UploadPath & "/" & StripSpecialCharacters(finfo.Name))
                        Try
                            My.Computer.Network.UploadFile(finfo.FullName, New Uri(UploadPath), currentusername, currentpassword, True, 100000, FileIO.UICancelOption.ThrowException)
                            Label2.Text = "Uploaded " & StripSpecialCharacters(finfo.FullName)
                            uploadedprecountfiles = uploadedprecountfiles + 1

                            Label2.Text = "Checking MD5 Hash (" & StripSpecialCharacters(finfo.Name) & ")"
                            Dim readMD5 As String = ""
                            My.Computer.Network.DownloadFile("http://comsan.uct.ac.za/Web/SAN Access/getMD5Checksum.php?filename=" & (recursivecurrentuploadfolder).Replace("/", "\") & "\" & StripSpecialCharacters(finfo.Name), md5status, "", "", False, 100000, True)
                            If My.Computer.FileSystem.FileExists(md5status) = True Then
                                Dim reader As StreamReader = New StreamReader(md5status)
                                Dim lineread As String = reader.ReadLine().Trim
                                ' MsgBox(lineread)
                                If reader.Peek <> -1 Then
                                    readMD5 = lineread
                                End If
                                reader.Close()
                                reader = Nothing
                                My.Computer.FileSystem.DeleteFile(md5status)
                            End If

                            If Not readMD5 = GetMD5Checksum(finfo.FullName) Then
                                If MsgBox("Note: The file (" & StripSpecialCharacters(finfo.Name) & ") you just uploaded has failed a MD5 checksum validation and it is more than likely that this file corrupted during the upload process. Do you wish to REMOVE this corrupt file?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, "MD5 Checksum Failed") = MsgBoxResult.Yes Then
                                    Try
                                        Dim request As FtpWebRequest = WebRequest.Create(New Uri(UploadPath))
                                        request.Credentials = New NetworkCredential(currentusername, currentpassword)
                                        request.Method = WebRequestMethods.Ftp.DeleteFile
                                        Dim response As FtpWebResponse = request.GetResponse()
                                        response.Close()
                                        response = Nothing
                                        request = Nothing
                                        Label2.Text = "Corrupt Upload Removed: " & StripSpecialCharacters(finfo.FullName)
                                    Catch ex1 As Exception
                                        Error_Handler(ex1, "Removing Corrupt Upload")
                                    End Try
                                End If
                            End If



                        Catch ex As Exception
                            Label2.Text = "Cancelled " & StripSpecialCharacters(finfo.FullName)
                            Label2.Text = "Removing Incomplete " & New Uri(UploadPath).ToString
                            Try
                                Dim request As FtpWebRequest = WebRequest.Create(New Uri(UploadPath))
                                request.Credentials = New NetworkCredential(currentusername, currentpassword)
                                request.Method = WebRequestMethods.Ftp.DeleteFile
                                Dim response As FtpWebResponse = request.GetResponse()
                                response.Close()
                                response = Nothing
                                request = Nothing
                            Catch ex1 As Exception
                                Error_Handler(ex, "Removing Incomplete Upload")
                            End Try

                        End Try
                        If finfo.Length > 15728640 Then
                            WebBrowser1.Refresh()
                        End If
                    Else
                        Label2.Text = "Cancelled " & StripSpecialCharacters(finfo.FullName)
                    End If

                Catch ex As Exception
                    Error_Handler(ex, "FTP Uploader: File Upload Attempt")
                    Label2.Text = "File Upload Failed"
                End Try
                uploadedprecountitems = uploadedprecountitems + 1

            End If
            finfo = Nothing

            If BackgroundWorker2.CancellationPending = True Then
                e.Cancel = True
                Exit Sub
            End If

            Dim dinfo As DirectoryInfo = New DirectoryInfo(TargetUpload)
            If dinfo.Exists = True Then
                Try
                    'ToolStripStatusLabel2.Text = "Creating Directory"
                    Label2.Text = "Creating " & currentfolder & "\" & StripSpecialCharacters(dinfo.Name)
                    My.Computer.Network.DownloadFile("http://comsan.uct.ac.za/Web/SAN Access/doesfolderexist.php?filename=" & recursivecurrentuploadfolder.Replace("/", "\") & "\" & StripSpecialCharacters(dinfo.Name), existstatus, "", "", False, 100000, True)
                    If My.Computer.FileSystem.FileExists(existstatus) = True Then
                        Dim reader As StreamReader = New StreamReader(existstatus)
                        Dim lineread As String = reader.ReadLine().Trim
                        ' MsgBox(lineread)
                        If reader.Peek <> -1 Then
                            If (lineread = "FALSE") Then
                                performUpload = True
                            Else
                                performUpload = False
                            End If
                        End If
                        reader.Close()
                        reader = Nothing
                        My.Computer.FileSystem.DeleteFile(existstatus)
                    End If

                    UploadPath = (UploadPath & "/" & StripSpecialCharacters(dinfo.Name))
                    recursivecurrentuploadfolder = recursivecurrentuploadfolder & "/" & StripSpecialCharacters(dinfo.Name)
                    'MsgBox("UploadPath: " & UploadPath)
                    'MsgBox("recursivecurrentuploadfolder: " & recursivecurrentuploadfolder)
                    If performUpload = True Then
                        Dim request As FtpWebRequest = WebRequest.Create(New Uri(UploadPath))
                        request.Credentials = New NetworkCredential(currentusername, currentpassword)
                        request.Method = WebRequestMethods.Ftp.MakeDirectory
                        Dim response As FtpWebResponse = request.GetResponse()
                        response.Close()
                        response = Nothing
                        request = Nothing
                        Label2.Text = "Created " & recursivecurrentuploadfolder & "\" & StripSpecialCharacters(dinfo.Name)
                    Else
                        Label2.Text = "Ignored " & recursivecurrentuploadfolder & "\" & StripSpecialCharacters(dinfo.Name)
                    End If
                    uploadedprecountfolders = uploadedprecountfolders + 1
                    WebBrowser1.Refresh()
                    For Each ff As FileInfo In dinfo.GetFiles
                        If BackgroundWorker2.CancellationPending = True Then
                            e.Cancel = True
                            Exit For
                        End If
                        RecursiveUploader(e, ff.FullName, UploadPath, recursivecurrentuploadfolder)
                        ff = Nothing
                        If BackgroundWorker2.CancellationPending = True Then
                            e.Cancel = True
                            Exit For
                        End If
                    Next

                    For Each dd As DirectoryInfo In dinfo.GetDirectories
                        If BackgroundWorker2.CancellationPending = True Then
                            e.Cancel = True
                            Exit For
                        End If
                        RecursiveUploader(e, dd.FullName, UploadPath, recursivecurrentuploadfolder)
                        dd = Nothing
                        If BackgroundWorker2.CancellationPending = True Then
                            e.Cancel = True
                            Exit For
                        End If
                    Next
                Catch ex As Exception
                    Error_Handler(ex, "FTP Uploader: Folder Upload Attempt")
                    Label2.Text = "Folder Upload Failed"
                End Try
                uploadedprecountitems = uploadedprecountitems + 1

            End If
            dinfo = Nothing
            Dim percentage As Integer = CSng(uploadedprecountitems) / CSng(precountitems) * 100
            If precountitems > 0 Then
                ToolStripProgressBar2.Value = percentage
            Else
                ToolStripProgressBar2.Value = 100
            End If
        Catch ex As Exception
            Error_Handler(ex, "FTP Recursive Uploader")
        End Try
    End Sub


    Private Sub BackgroundWorker2_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        Try
            ToolStripProgressBar2.Visible = False
            Dim precountfileline, precountfolderline As String
            If uploadedprecountfolders <> 1 Then
                precountfolderline = " folders"
            Else
                precountfolderline = " folder"
            End If
            If uploadedprecountfiles <> 1 Then
                precountfileline = " files"
            Else
                precountfileline = " file"
            End If
            If precountfiles = 0 And precountfolders > 0 Then
                ToolStripStatusLabel2.Text = "Last Upload Count: " & uploadedprecountfolders & " (of " & precountfolders & ")" & precountfolderline
            End If
            If precountfolders = 0 And precountfiles > 0 Then
                ToolStripStatusLabel2.Text = "Last Upload Count: " & uploadedprecountfiles & " (of " & precountfiles & ")" & precountfileline
            End If
            If precountfiles > 0 And precountfolders > 0 Then
                ToolStripStatusLabel2.Text = "Last Upload Count: " & uploadedprecountfolders & " (of " & precountfolders & ")" & precountfolderline & " | " & uploadedprecountfiles & " (of " & precountfiles & ")" & precountfileline
            End If
            If precountfiles = 0 And precountfolders = 0 Then
                ToolStripStatusLabel2.Text = ""
            End If
            WebBrowser1.Refresh()
            Label2.Text = "Drag and drop the files or folders you wish to upload here"
            controls_enabler(True, "upload")
        Catch ex As Exception
            Error_Handler(ex, "Uploads Completed")
        End Try
        
    End Sub


    Private Function GetMD5Checksum(ByVal filePath As String) As String
        Dim result As String = ""
        Try
            Dim md5 As MD5CryptoServiceProvider = New MD5CryptoServiceProvider
            Dim fs As FileStream = New FileStream(filePath, FileMode.Open, _
            FileAccess.Read, FileShare.Read, 8192)
            fs = New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192)
            md5.ComputeHash(fs)
            fs.Close()
            Dim hash As Byte() = md5.Hash
            Dim sb As StringBuilder = New StringBuilder
            Dim hByte As Byte
            For Each hByte In hash
                sb.Append(String.Format("{0:X2}", hByte))
            Next
            result = sb.ToString()
        Catch ex As Exception
            Error_Handler(ex, "Generate MD5 Checksum")
        End Try
        Return result
    End Function 'GetMD5Checksum

    Private Function StripSpecialCharacters(ByVal filepath As String) As String
        Dim result As String = filepath
        Try
            Dim characters1() As String = {"`", "~", "!", "@", "#", "$", "^", "&", "*", "+", "=", "{", "}", "[", "]", "|", "'", ";", "\", "<", ">", "&", """", "'"}
            For Each str As String In characters1
                result = result.Replace(str, "_")
            Next
            Dim characters2() As String = {"À", "Á", "Â", "Ã", "Ä", "Å", "Æ"}
            For Each str As String In characters2
                result = result.Replace(str, "A")
            Next
            Dim characters3() As String = {"Ç"}
            For Each str As String In characters3
                result = result.Replace(str, "C")
            Next
            Dim characters4() As String = {"È", "É", "Ê", "Ë"}
            For Each str As String In characters4
                result = result.Replace(str, "E")
            Next
            Dim characters5() As String = {"à", "á", "â", "ã", "ä", "å", "æ"}
            For Each str As String In characters5
                result = result.Replace(str, "a")
            Next
            Dim characters6() As String = {"ò", "ó", "ô", "õ", "ö", "ø"}
            For Each str As String In characters6
                result = result.Replace(str, "o")
            Next
            Dim characters7() As String = {"è", "é", "ê", "ë", "ð"}
            For Each str As String In characters7
                result = result.Replace(str, "e")
            Next
            Dim characters8() As String = {"Ò", "Ó", "Ô", "Õ", "Ö", "Ø"}
            For Each str As String In characters8
                result = result.Replace(str, "O")
            Next
            Dim characters9() As String = {"Ù", "Ú", "Û", "Ü"}
            For Each str As String In characters9
                result = result.Replace(str, "U")
            Next
            Dim characters10() As String = {"Ì", "Í", "Î", "Ï"}
            For Each str As String In characters10
                result = result.Replace(str, "I")
            Next
            Dim characters11() As String = {"ì", "í", "î", "ï"}
            For Each str As String In characters11
                result = result.Replace(str, "i")
            Next
            Dim characters12() As String = {"ù", "ú", "û", "ü"}
            For Each str As String In characters12
                result = result.Replace(str, "u")
            Next
            Dim characters13() As String = {"ý", "þ", "ÿ"}
            For Each str As String In characters13
                result = result.Replace(str, "y")
            Next
            Dim characters14() As String = {"Ý", "Þ"}
            For Each str As String In characters14
                result = result.Replace(str, "Y")
            Next
            Dim characters15() As String = {"Ð"}
            For Each str As String In characters15
                result = result.Replace(str, "D")
            Next
            Dim characters16() As String = {"Ñ"}
            For Each str As String In characters16
                result = result.Replace(str, "N")
            Next
            Dim characters17() As String = {"ß"}
            For Each str As String In characters17
                result = result.Replace(str, "B")
            Next
            Dim characters18() As String = {"ç"}
            For Each str As String In characters18
                result = result.Replace(str, "c")
            Next
            Dim characters19() As String = {"ñ"}
            For Each str As String In characters19
                result = result.Replace(str, "n")
            Next
        Catch ex As Exception
            Error_Handler(ex, "Strip Special Characters")
        End Try
        Return result
    End Function

    Private Sub CancelUpload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelUpload.Click
        Try
            BackgroundWorker2.CancelAsync()
            CancelUpload.Enabled = False
        Catch ex As Exception
            Error_Handler(ex, "Push Cancel Upload Button")
        End Try
    End Sub


    Private Sub CreateFolderButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CreateFolderButton.Click
        Try
            ToolStripStatusLabel2.Text = "Creating Folder"
            controls_enabler(False)
            BackgroundWorker3.RunWorkerAsync()
        Catch ex As Exception
            Error_Handler(ex, "Create New Sub Folder")
        End Try
    End Sub

    Private Sub BackgroundWorker3_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker3.DoWork

        Try
            Dim performupload As Boolean = False
            Dim targetupload As String = InputBox("Please enter the name for the sub folder which you want to create:", "Create Sub Folder", "New Folder")
            If targetupload.Length > 0 Then
                My.Computer.Network.DownloadFile("http://comsan.uct.ac.za/Web/SAN Access/doesfolderexist.php?filename=" & currentfolder.Replace("/", "\") & "\" & StripSpecialCharacters(targetupload), existstatus, "", "", False, 100000, True)
                If My.Computer.FileSystem.FileExists(existstatus) = True Then
                    Dim reader As StreamReader = New StreamReader(existstatus)
                    Dim lineread As String = reader.ReadLine().Trim
                    ' MsgBox(lineread)
                    If reader.Peek <> -1 Then
                        If (lineread = "FALSE") Then
                            performupload = True
                        Else
                            performupload = False
                        End If
                    End If
                    reader.Close()
                    reader = Nothing
                    My.Computer.FileSystem.DeleteFile(existstatus)
                End If

                Dim UploadPath As String
                If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                    UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
                Else
                    UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
                End If
                UploadPath = UploadPath.Remove(0, currentusername.Length)
                UploadPath = ("ftp:////comsan.uct.ac.za/" & UploadPath & "/" & StripSpecialCharacters(targetupload)).Replace("\", "/").Replace("//", "/")


                'MsgBox("UploadPath: " & UploadPath)
                'MsgBox("recursivecurrentuploadfolder: " & recursivecurrentuploadfolder)
                If performupload = True Then
                    Dim request As FtpWebRequest = WebRequest.Create(New Uri(UploadPath))
                    request.Credentials = New NetworkCredential(currentusername, currentpassword)
                    request.Method = WebRequestMethods.Ftp.MakeDirectory
                    Dim response As FtpWebResponse = request.GetResponse()
                    response.Close()
                    response = Nothing
                    request = Nothing
                    WebBrowser1.Refresh()
                Else
                    MsgBox("The folder that you are trying to create already exists. Your request will be ignored.", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "Folder Exists")
                End If
            End If

        Catch ex As Exception
            Error_Handler(ex, "Creating New Folder")
        End Try

    End Sub

    Private Sub BackgroundWorker3_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker3.RunWorkerCompleted
        Try
            ToolStripStatusLabel2.Text = "Folder Creation Complete"
            controls_enabler(True)
        Catch ex As Exception
            Error_Handler(ex, "Create New Sub Folder")
        End Try
    End Sub


    Private Sub DeleteAllFilesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteAllFilesButton.Click
        Try
            ToolStripStatusLabel2.Text = "Deleting all Files"
            ToolStripProgressBar2.Visible = True
            ToolStripProgressBar2.Value = 0
            controls_enabler(False)
            BackgroundWorker4.RunWorkerAsync()
        Catch ex As Exception
            Error_Handler(ex, "Deleting all Files")
        End Try
    End Sub

    Private Sub BackgroundWorker4_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker4.DoWork

        Try
            Dim UploadPath As String
            If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
            Else
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
            End If

            If MsgBox("Are you sure you wish to delete all the files in """ & UploadPath & """?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, "Delete All Files") = MsgBoxResult.Yes Then
                UploadPath = UploadPath.Remove(0, currentusername.Length)
                UploadPath = ("ftp:////comsan.uct.ac.za/" & UploadPath).Replace("\", "/").Replace("//", "/")
                '    'MsgBox("UploadPath: " & UploadPath)
                '    'MsgBox("recursivecurrentuploadfolder: " & recursivecurrentuploadfolder)
                '    If performupload = True Then
                Dim request As FtpWebRequest = WebRequest.Create(New Uri(UploadPath))
                request.Credentials = New NetworkCredential(currentusername, currentpassword)
                request.Method = WebRequestMethods.Ftp.ListDirectory
                Dim filearray As ArrayList = New ArrayList
                Dim response As FtpWebResponse = request.GetResponse()
                Dim reader As StreamReader = New StreamReader(response.GetResponseStream)
                Dim lineread As String = ""
                While reader.Peek <> -1
                    lineread = reader.ReadLine
                    While lineread.IndexOf("/") <> -1
                        lineread = lineread.Remove(0, lineread.IndexOf("/") + 1)
                    End While
                    filearray.Add(lineread)
                End While
                reader.Close()
                response.Close()
                request = Nothing
                response = Nothing
                reader = Nothing
                request = WebRequest.Create(New Uri(UploadPath))
                request.Credentials = New NetworkCredential(currentusername, currentpassword)
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails
                response = request.GetResponse()
                reader = New StreamReader(response.GetResponseStream)
                Dim counter As Integer = 0
                While reader.Peek <> -1
                    lineread = reader.ReadLine
                    If lineread.ToLower.StartsWith("d") Then
                        filearray.Item(counter) = "_directory_ " & filearray.Item(counter)
                    End If
                    counter = counter + 1
                End While
                reader.Close()
                response.Close()
                response = Nothing
                request = Nothing
                reader = Nothing
                counter = 1
                Dim deletecounter As Integer = 0
                GroupBox1.Text = "File Deletion in Progress"
                Label2.Text = ""
                Dim filetodeletepath As String = ""
                For Each filename As String In filearray
                    Try
                        ToolStripStatusLabel2.Text = "Processing object " & counter & " (of " & filearray.Count & ")"
                        If filename.StartsWith("_directory_ ") = False Then
                            If UploadPath.EndsWith("/") Then
                                filetodeletepath = UploadPath & filename
                            Else
                                filetodeletepath = UploadPath & "/" & filename
                            End If
                            deletecounter = deletecounter + 1
                            Label2.Text = "Deleting " & filename
                            request = WebRequest.Create(New Uri(filetodeletepath))
                            request.Credentials = New NetworkCredential(currentusername, currentpassword)
                            request.Method = WebRequestMethods.Ftp.DeleteFile
                            response = request.GetResponse()
                            response.Close()
                            response = Nothing
                            request = Nothing
                        End If
                        Dim percentage As Integer = CSng(counter) / CSng(filearray.Count) * 100
                        If precountitems > 0 Then
                            ToolStripProgressBar2.Value = percentage
                        Else
                            ToolStripProgressBar2.Value = 100
                        End If
                        counter = counter + 1
                    Catch ex As Exception
                        Error_Handler(ex, "Deleting File: " & filetodeletepath)
                    End Try
                Next

                filearray.Clear()
                filearray = Nothing
                GroupBox1.Text = "Upload your files"
                Label2.Text = "Drag and drop the files or folders you wish to upload here"
                WebBrowser1.Refresh()
                ToolStripStatusLabel2.Text = "All files processed"
                If deletecounter <> 1 Then
                    MsgBox("Operation Complete. " & deletecounter & " files were removed from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Files Deleted")
                Else
                    MsgBox("Operation Complete. " & deletecounter & " file was removed from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Files Deleted")
                End If

            End If
        Catch ex As Exception
            Error_Handler(ex, "Deleting all Files")
        End Try
    End Sub

    Private Sub BackgroundWorker4_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker4.RunWorkerCompleted
        Try
            ToolStripStatusLabel2.Text = "File Deletion Complete"
            ToolStripProgressBar2.Visible = False
            controls_enabler(True)
        Catch ex As Exception
            Error_Handler(ex, "Deleting all Files")
        End Try
    End Sub


    Private Sub DeleteAllContentButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteAllContentButton.Click
        Try
            ToolStripStatusLabel2.Text = "Deleting all Content"
            controls_enabler(False)
            ToolStripProgressBar2.Visible = True
            ToolStripProgressBar2.Value = 0
            BackgroundWorker5.RunWorkerAsync()
        Catch ex As Exception
            Error_Handler(ex, "Deleting all Content")
        End Try
    End Sub

    Private Sub BackgroundWorker5_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker5.DoWork

        Try
            Dim UploadPath As String
            If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
            Else
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
            End If

            If MsgBox("Are you sure you wish to delete all the content in """ & UploadPath & """?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, "Delete All Content") = MsgBoxResult.Yes Then
                UploadPath = UploadPath.Remove(0, currentusername.Length)
                UploadPath = ("ftp:////comsan.uct.ac.za/" & UploadPath).Replace("\", "/").Replace("//", "/")
                '    'MsgBox("UploadPath: " & UploadPath)
                '    'MsgBox("recursivecurrentuploadfolder: " & recursivecurrentuploadfolder)
                '    If performupload = True Then
                Dim request As FtpWebRequest = WebRequest.Create(New Uri(UploadPath))
                request.Credentials = New NetworkCredential(currentusername, currentpassword)
                request.Method = WebRequestMethods.Ftp.ListDirectory
                Dim filearray As ArrayList = New ArrayList
                Dim response As FtpWebResponse = request.GetResponse()
                Dim reader As StreamReader = New StreamReader(response.GetResponseStream)
                Dim lineread As String = ""
                While reader.Peek <> -1
                    lineread = reader.ReadLine
                    While lineread.IndexOf("/") <> -1
                        lineread = lineread.Remove(0, lineread.IndexOf("/") + 1)
                    End While
                    filearray.Add(lineread)
                End While
                reader.Close()
                response.Close()
                request = Nothing
                response = Nothing
                reader = Nothing
                request = WebRequest.Create(New Uri(UploadPath))
                request.Credentials = New NetworkCredential(currentusername, currentpassword)
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails
                response = request.GetResponse()
                reader = New StreamReader(response.GetResponseStream)
                Dim counter As Integer = 0
                While reader.Peek <> -1
                    lineread = reader.ReadLine
                    If lineread.ToLower.StartsWith("d") Then
                        filearray.Item(counter) = "_directory_ " & filearray.Item(counter)
                    End If
                    counter = counter + 1
                End While
                reader.Close()
                response.Close()
                response = Nothing
                request = Nothing
                reader = Nothing
                counter = 1
                Dim deletecounter As Integer = 0
                GroupBox1.Text = "Content Deletion in Progress"
                Label2.Text = ""
                Dim filetodeletepath As String = ""
                For Each filename As String In filearray
                    Try
                        ToolStripStatusLabel2.Text = "Processing object " & counter & " (of " & filearray.Count & ")"
                        deletecounter = deletecounter + 1
                        Label2.Text = "Deleting " & filename.Replace("_directory_ ", "")
                        If filename.StartsWith("_directory_ ") = False Then
                            If UploadPath.EndsWith("/") Then
                                filetodeletepath = UploadPath & filename
                            Else
                                filetodeletepath = UploadPath & "/" & filename
                            End If
                            request = WebRequest.Create(New Uri(filetodeletepath))
                            request.Credentials = New NetworkCredential(currentusername, currentpassword)
                            request.Method = WebRequestMethods.Ftp.DeleteFile
                            response = request.GetResponse()
                            response.Close()
                            response = Nothing
                            request = Nothing
                        Else
                            My.Computer.Network.DownloadFile("http://comsan.uct.ac.za/Web/SAN Access/SAN_Access_FolderDelete.php?foldername=" & currentfolder.Replace("/", "\") & "\" & filename.Replace("_directory_ ", ""), existstatus, "", "", False, 100000, True)
                            If My.Computer.FileSystem.FileExists(existstatus) = True Then
                                My.Computer.FileSystem.DeleteFile(existstatus)
                            End If
                        End If
                        Dim percentage As Integer = CSng(counter) / CSng(filearray.Count) * 100
                        If precountitems > 0 Then
                            ToolStripProgressBar2.Value = percentage
                        Else
                            ToolStripProgressBar2.Value = 100
                        End If
                        counter = counter + 1
                    Catch ex As Exception
                        Error_Handler(ex, "Deleting Object: " & filetodeletepath)
                    End Try
                Next

                filearray.Clear()
                filearray = Nothing
                GroupBox1.Text = "Upload your files"
                Label2.Text = "Drag and drop the files or folders you wish to upload here"
                WebBrowser1.Refresh()
                ToolStripStatusLabel2.Text = "All files processed"
                If deletecounter <> 1 Then
                    MsgBox("Operation Complete. " & deletecounter & " objects were removed from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Content Deleted")
                Else
                    MsgBox("Operation Complete. " & deletecounter & " object was removed from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Content Deleted")
                End If

            End If
        Catch ex As Exception
            Error_Handler(ex, "Deleting all Content")
        End Try
    End Sub

    Private Sub BackgroundWorker5_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker5.RunWorkerCompleted
        Try
            ToolStripStatusLabel2.Text = "Content Deletion Complete"
            ToolStripProgressBar2.Visible = False
            controls_enabler(True)
        Catch ex As Exception
            Error_Handler(ex, "Deleting all Content")
        End Try
    End Sub


    Private Sub DeleteAllFoldersButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteAllFoldersButton.Click
        Try
            ToolStripStatusLabel2.Text = "Deleting all Folders"
            controls_enabler(False)
            ToolStripProgressBar2.Visible = True
            ToolStripProgressBar2.Value = 0
            BackgroundWorker6.RunWorkerAsync()
        Catch ex As Exception
            Error_Handler(ex, "Deleting all Content")
        End Try
    End Sub

    Private Sub BackgroundWorker6_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker6.DoWork

        Try
            Dim UploadPath As String
            If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
            Else
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
            End If

            If MsgBox("Are you sure you wish to delete all the folders in """ & UploadPath & """?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, "Delete All Folders") = MsgBoxResult.Yes Then
                UploadPath = UploadPath.Remove(0, currentusername.Length)
                UploadPath = ("ftp:////comsan.uct.ac.za/" & UploadPath).Replace("\", "/").Replace("//", "/")
                '    'MsgBox("UploadPath: " & UploadPath)
                '    'MsgBox("recursivecurrentuploadfolder: " & recursivecurrentuploadfolder)
                '    If performupload = True Then
                Dim request As FtpWebRequest = WebRequest.Create(New Uri(UploadPath))
                request.Credentials = New NetworkCredential(currentusername, currentpassword)
                request.Method = WebRequestMethods.Ftp.ListDirectory
                Dim filearray As ArrayList = New ArrayList
                Dim response As FtpWebResponse = request.GetResponse()
                Dim reader As StreamReader = New StreamReader(response.GetResponseStream)
                Dim lineread As String = ""
                While reader.Peek <> -1
                    lineread = reader.ReadLine
                    While lineread.IndexOf("/") <> -1
                        lineread = lineread.Remove(0, lineread.IndexOf("/") + 1)
                    End While
                    filearray.Add(lineread)
                End While
                reader.Close()
                response.Close()
                request = Nothing
                response = Nothing
                reader = Nothing
                request = WebRequest.Create(New Uri(UploadPath))
                request.Credentials = New NetworkCredential(currentusername, currentpassword)
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails
                response = request.GetResponse()
                reader = New StreamReader(response.GetResponseStream)
                Dim counter As Integer = 0
                While reader.Peek <> -1
                    lineread = reader.ReadLine
                    If lineread.ToLower.StartsWith("d") Then
                        filearray.Item(counter) = "_directory_ " & filearray.Item(counter)
                    End If
                    counter = counter + 1
                End While
                reader.Close()
                response.Close()
                response = Nothing
                request = Nothing
                reader = Nothing
                counter = 1
                Dim deletecounter As Integer = 0
                GroupBox1.Text = "Folders Deletion in Progress"
                Label2.Text = ""
                Dim filetodeletepath As String = ""
                For Each filename As String In filearray
                    Try
                        ToolStripStatusLabel2.Text = "Processing object " & counter & " (of " & filearray.Count & ")"
                        Label2.Text = "Deleting " & filename.Replace("_directory_ ", "")
                        If filename.StartsWith("_directory_ ") = True Then
                            My.Computer.Network.DownloadFile("http://comsan.uct.ac.za/Web/SAN Access/SAN_Access_FolderDelete.php?foldername=" & currentfolder.Replace("/", "\") & "\" & filename.Replace("_directory_ ", ""), existstatus, "", "", False, 100000, True)
                            If My.Computer.FileSystem.FileExists(existstatus) = True Then
                                My.Computer.FileSystem.DeleteFile(existstatus)
                            End If
                            deletecounter = deletecounter + 1
                        End If
                        Dim percentage As Integer = CSng(counter) / CSng(filearray.Count) * 100
                        If precountitems > 0 Then
                            ToolStripProgressBar2.Value = percentage
                        Else
                            ToolStripProgressBar2.Value = 100
                        End If
                        counter = counter + 1
                    Catch ex As Exception
                        Error_Handler(ex, "Deleting Folder: " & filetodeletepath)
                    End Try
                Next

                filearray.Clear()
                filearray = Nothing
                GroupBox1.Text = "Upload your files"
                Label2.Text = "Drag and drop the files or folders you wish to upload here"
                WebBrowser1.Refresh()
                ToolStripStatusLabel2.Text = "All folders processed"
                If deletecounter <> 1 Then
                    MsgBox("Operation Complete. " & deletecounter & " folders were removed from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Folders Deleted")
                Else
                    MsgBox("Operation Complete. " & deletecounter & " folder was removed from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Folders Deleted")
                End If

            End If
        Catch ex As Exception
            Error_Handler(ex, "Deleting all Folders")
        End Try
    End Sub

    Private Sub BackgroundWorker6_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker6.RunWorkerCompleted
        Try
            ToolStripStatusLabel2.Text = "Folders Deletion Complete"
            ToolStripProgressBar2.Visible = False
            controls_enabler(True)
        Catch ex As Exception
            Error_Handler(ex, "Deleting all Folders")
        End Try
    End Sub


    Private Sub DownloadAllFilesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloadAllFilesButton.Click
        Try
            Dim UploadPath As String
            If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
            Else
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
            End If
            FolderBrowserDialog1.Description = "Please select the folder you wish to download all the files in """ & UploadPath & """ to"
            If FolderBrowserDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                ToolStripStatusLabel2.Text = "Downloading all Files"
                ToolStripProgressBar2.Visible = True
                ToolStripProgressBar2.Value = 0
                controls_enabler(False)
                BackgroundWorker7.RunWorkerAsync(FolderBrowserDialog1.SelectedPath)
            End If
        Catch ex As Exception
            Error_Handler(ex, "Downloading all Files")
        End Try
    End Sub

    Private Sub BackgroundWorker7_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker7.DoWork
        Try
            Dim downloadtopath As String = e.Argument.ToString
            If downloadtopath.EndsWith("\") = True Then
                downloadtopath = downloadtopath.Remove(e.Argument.ToString.Length - 1, 1)
            End If
            Dim UploadPath As String
            If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
            Else
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
            End If

            UploadPath = UploadPath.Remove(0, currentusername.Length)
            UploadPath = ("ftp:////comsan.uct.ac.za/" & UploadPath).Replace("\", "/").Replace("//", "/")

            Dim request As FtpWebRequest = WebRequest.Create(New Uri(UploadPath))
            request.Credentials = New NetworkCredential(currentusername, currentpassword)
            request.Method = WebRequestMethods.Ftp.ListDirectory
            Dim filearray As ArrayList = New ArrayList
            Dim response As FtpWebResponse = request.GetResponse()
            Dim reader As StreamReader = New StreamReader(response.GetResponseStream)
            Dim lineread As String = ""
            While reader.Peek <> -1
                lineread = reader.ReadLine
                While lineread.IndexOf("/") <> -1
                    lineread = lineread.Remove(0, lineread.IndexOf("/") + 1)
                End While
                filearray.Add(lineread)
            End While
            reader.Close()
            response.Close()
            request = Nothing
            response = Nothing
            reader = Nothing
            request = WebRequest.Create(New Uri(UploadPath))
            request.Credentials = New NetworkCredential(currentusername, currentpassword)
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails
            response = request.GetResponse()
            reader = New StreamReader(response.GetResponseStream)
            Dim counter As Integer = 0
            While reader.Peek <> -1
                lineread = reader.ReadLine
                If lineread.ToLower.StartsWith("d") Then
                    filearray.Item(counter) = "_directory_ " & filearray.Item(counter)
                End If
                counter = counter + 1
            End While
            reader.Close()
            response.Close()
            response = Nothing
            request = Nothing
            reader = Nothing
            counter = 1
            Dim downloadcounter As Integer = 0
            GroupBox1.Text = "File Download in Progress"
            Label2.Text = ""
            Dim filetodownloadpath As String = ""
            For Each filename As String In filearray
                Try
                    ToolStripStatusLabel2.Text = "Downloading file " & counter & " (of " & filearray.Count & ")"
                    If filename.StartsWith("_directory_ ") = False Then
                        If UploadPath.EndsWith("/") Then
                            filetodownloadpath = UploadPath & filename
                        Else
                            filetodownloadpath = UploadPath & "/" & filename
                        End If
                        downloadcounter = downloadcounter + 1
                        Label2.Text = "Downloading " & filename
                        If My.Computer.FileSystem.FileExists(downloadtopath & "\" & filename) = True Then
                            If MsgBox(downloadtopath & "\" & filename & " arleady exists on the system. Do you wish to replace the existing file with this one?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, "File aready exists") = MsgBoxResult.Yes Then
                                My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                            End If
                        Else
                            My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                        End If

                    End If
                    Dim percentage As Integer = CSng(counter) / CSng(filearray.Count) * 100
                    If precountitems > 0 Then
                        ToolStripProgressBar2.Value = percentage
                    Else
                        ToolStripProgressBar2.Value = 100
                    End If
                    counter = counter + 1
                Catch ex As Exception
                    Error_Handler(ex, "Downloading File: " & filetodownloadpath)
                End Try
            Next

            filearray.Clear()
            filearray = Nothing
            GroupBox1.Text = "Upload your files"
            Label2.Text = "Drag and drop the files or folders you wish to upload here"
            'WebBrowser1.Refresh()
            ToolStripStatusLabel2.Text = "All files downloaded"
            If downloadcounter <> 1 Then
                MsgBox("Operation Complete. " & downloadcounter & " files were downloaded from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Files Downloaded")
            Else
                MsgBox("Operation Complete. " & downloadcounter & " file was downloaded from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Files Downloaded")
            End If


        Catch ex As Exception
            Error_Handler(ex, "Downloading all Files")
        End Try
    End Sub

    Private Sub BackgroundWorker7_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker7.RunWorkerCompleted
        Try
            ToolStripStatusLabel2.Text = "File Downloading Complete"
            ToolStripProgressBar2.Visible = False
            controls_enabler(True)
        Catch ex As Exception
            Error_Handler(ex, "Downloading all Files")
        End Try
    End Sub

    Private Sub DownloadAllFoldersButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloadAllFoldersButton.Click
        Try
            Dim UploadPath As String
            If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
            Else
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
            End If
            FolderBrowserDialog1.Description = "Please select the folder you wish to download all the folders in """ & UploadPath & """ to"
            If FolderBrowserDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then

                downloadedprecountfiles = 0
                downloadedprecountfolders = 0
                downloadedprecountitems = 0

                ToolStripStatusLabel2.Text = "Downloading all Folders"
                ToolStripProgressBar2.Visible = True
                ToolStripProgressBar2.Value = 0
                controls_enabler(False)
                BackgroundWorker8.RunWorkerAsync(FolderBrowserDialog1.SelectedPath)
            End If
        Catch ex As Exception
            Error_Handler(ex, "Downloading all Folders")
        End Try
    End Sub

    Private Sub BackgroundWorker8_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker8.DoWork
        Try
            Dim downloadtopath As String = e.Argument.ToString
            If downloadtopath.EndsWith("\") = True Then
                downloadtopath = downloadtopath.Remove(e.Argument.ToString.Length - 1, 1)
            End If
            Dim UploadPath As String
            If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
            Else
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
            End If

            UploadPath = UploadPath.Remove(0, currentusername.Length)
            UploadPath = ("ftp:////comsan.uct.ac.za/" & UploadPath).Replace("\", "/").Replace("//", "/")

            Dim request As FtpWebRequest = WebRequest.Create(New Uri(UploadPath))
            request.Credentials = New NetworkCredential(currentusername, currentpassword)
            request.Method = WebRequestMethods.Ftp.ListDirectory
            Dim filearray As ArrayList = New ArrayList
            Dim response As FtpWebResponse = request.GetResponse()
            Dim reader As StreamReader = New StreamReader(response.GetResponseStream)
            Dim lineread As String = ""
            While reader.Peek <> -1
                lineread = reader.ReadLine
                While lineread.IndexOf("/") <> -1
                    lineread = lineread.Remove(0, lineread.IndexOf("/") + 1)
                End While
                filearray.Add(lineread)
            End While
            reader.Close()
            response.Close()
            request = Nothing
            response = Nothing
            reader = Nothing
            request = WebRequest.Create(New Uri(UploadPath))
            request.Credentials = New NetworkCredential(currentusername, currentpassword)
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails
            response = request.GetResponse()
            reader = New StreamReader(response.GetResponseStream)
            Dim counter As Integer = 0
            While reader.Peek <> -1
                lineread = reader.ReadLine
                If lineread.ToLower.StartsWith("d") Then
                    filearray.Item(counter) = "_directory_ " & filearray.Item(counter)
                End If
                counter = counter + 1
            End While
            reader.Close()
            response.Close()
            response = Nothing
            request = Nothing
            reader = Nothing
            counter = 1

            GroupBox1.Text = "Folder Download in Progress"
            Label2.Text = ""
            Dim filetodownloadpath As String = ""
            For Each filename As String In filearray
                Try
                    ToolStripStatusLabel2.Text = "Downloading file " & counter & " (of " & filearray.Count & ")"
                    If filename.StartsWith("_directory_ ") = True Then
                        If UploadPath.EndsWith("/") Then
                            filetodownloadpath = UploadPath & filename
                        Else
                            filetodownloadpath = UploadPath & "/" & filename
                        End If
                        'downloadcounter = downloadcounter + 1
                        Label2.Text = "Downloading " & filename
                        recursivedownloader(filetodownloadpath.Replace("_directory_ ", ""), downloadtopath & "\" & filename.Replace("_directory_ ", ""))

                        'If My.Computer.FileSystem.FileExists(downloadtopath & "\" & filename) = True Then
                        '    If MsgBox(downloadtopath & "\" & filename & " arleady exists on the system. Do you wish to replace the existing file with this one?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, "File aready exists") = MsgBoxResult.Yes Then
                        '        My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                        '    End If
                        'Else
                        '    My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                        'End If

                    End If
                    Dim percentage As Integer = CSng(counter) / CSng(filearray.Count) * 100
                    If precountitems > 0 Then
                        ToolStripProgressBar2.Value = percentage
                    Else
                        ToolStripProgressBar2.Value = 100
                    End If
                    counter = counter + 1
                Catch ex As Exception
                    Error_Handler(ex, "Downloading File: " & filetodownloadpath)
                End Try
            Next

            filearray.Clear()
            filearray = Nothing
            GroupBox1.Text = "Upload your files"
            Label2.Text = "Drag and drop the files or folders you wish to upload here"
            'WebBrowser1.Refresh()
            'ToolStripStatusLabel2.Text = "All files downloaded"
            'If downloadcounter <> 1 Then
            '    MsgBox("Operation Complete. " & downloadcounter & " files were downloaded from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Files Downloaded")
            'Else
            '    MsgBox("Operation Complete. " & downloadcounter & " file was downloaded from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Files Downloaded")
            'End If


        Catch ex As Exception
            Error_Handler(ex, "Downloading all Files")
        End Try
    End Sub

    Private Sub BackgroundWorker8_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker8.RunWorkerCompleted
        Try
            ToolStripStatusLabel2.Text = "Folder Downloading Complete"
            ToolStripProgressBar2.Visible = False
            controls_enabler(True)
        Catch ex As Exception
            Error_Handler(ex, "Downloading all Folders")
        End Try
    End Sub


    Private Sub DownloadAllContentButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DownloadAllContentButton.Click
        Try
            Dim UploadPath As String
            If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
            Else
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
            End If
            FolderBrowserDialog1.Description = "Please select the folder you wish to download all the content in """ & UploadPath & """ to"
            If FolderBrowserDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                ToolStripStatusLabel2.Text = "Downloading all Content"
                ToolStripProgressBar2.Visible = True
                ToolStripProgressBar2.Value = 0
                controls_enabler(False)
                BackgroundWorker9.RunWorkerAsync(FolderBrowserDialog1.SelectedPath)
            End If
        Catch ex As Exception
            Error_Handler(ex, "Downloading all Content")
        End Try
    End Sub

    Private Sub BackgroundWorker9_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker9.DoWork
        Try
            Dim downloadtopath As String = e.Argument.ToString
            If downloadtopath.EndsWith("\") = True Then
                downloadtopath = downloadtopath.Remove(e.Argument.ToString.Length - 1, 1)
            End If
            Dim UploadPath As String
            If WebBrowser1.Url.ToString.LastIndexOf("&") <> -1 Then
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1).Substring(0, WebBrowser1.Url.ToString.LastIndexOf("&"))
            Else
                UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
            End If

            UploadPath = UploadPath.Remove(0, currentusername.Length)
            UploadPath = ("ftp:////comsan.uct.ac.za/" & UploadPath).Replace("\", "/").Replace("//", "/")

            Dim request As FtpWebRequest = WebRequest.Create(New Uri(UploadPath))
            request.Credentials = New NetworkCredential(currentusername, currentpassword)
            request.Method = WebRequestMethods.Ftp.ListDirectory
            Dim filearray As ArrayList = New ArrayList
            Dim response As FtpWebResponse = request.GetResponse()
            Dim reader As StreamReader = New StreamReader(response.GetResponseStream)
            Dim lineread As String = ""
            While reader.Peek <> -1
                lineread = reader.ReadLine
                While lineread.IndexOf("/") <> -1
                    lineread = lineread.Remove(0, lineread.IndexOf("/") + 1)
                End While
                filearray.Add(lineread)
            End While
            reader.Close()
            response.Close()
            request = Nothing
            response = Nothing
            reader = Nothing
            request = WebRequest.Create(New Uri(UploadPath))
            request.Credentials = New NetworkCredential(currentusername, currentpassword)
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails
            response = request.GetResponse()
            reader = New StreamReader(response.GetResponseStream)
            Dim counter As Integer = 0
            While reader.Peek <> -1
                lineread = reader.ReadLine
                If lineread.ToLower.StartsWith("d") Then
                    filearray.Item(counter) = "_directory_ " & filearray.Item(counter)
                End If
                counter = counter + 1
            End While
            reader.Close()
            response.Close()
            response = Nothing
            request = Nothing
            reader = Nothing
            counter = 1
            Dim downloadcounter As Integer = 0
            GroupBox1.Text = "Content Download in Progress"
            Label2.Text = ""
            Dim filetodownloadpath As String = ""
            For Each filename As String In filearray
                Try
                    ToolStripStatusLabel2.Text = "Downloading file " & counter & " (of " & filearray.Count & ")"
                    If filename.StartsWith("_directory_ ") = False Then
                        If UploadPath.EndsWith("/") Then
                            filetodownloadpath = UploadPath & filename
                        Else
                            filetodownloadpath = UploadPath & "/" & filename
                        End If
                        downloadcounter = downloadcounter + 1
                        Label2.Text = "Downloading " & filename
                        If My.Computer.FileSystem.FileExists(downloadtopath & "\" & filename) = True Then
                            If MsgBox(downloadtopath & "\" & filename & " arleady exists on the system. Do you wish to replace the existing file with this one?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, "File aready exists") = MsgBoxResult.Yes Then
                                My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                            End If
                        Else
                            My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                        End If
                    Else
                        If UploadPath.EndsWith("/") Then
                            filetodownloadpath = UploadPath & filename
                        Else
                            filetodownloadpath = UploadPath & "/" & filename
                        End If
                        'downloadcounter = downloadcounter + 1
                        Label2.Text = "Downloading " & filename
                        recursivedownloader(filetodownloadpath.Replace("_directory_ ", ""), downloadtopath & "\" & filename.Replace("_directory_ ", ""))

                        'If My.Computer.FileSystem.FileExists(downloadtopath & "\" & filename) = True Then
                        '    If MsgBox(downloadtopath & "\" & filename & " arleady exists on the system. Do you wish to replace the existing file with this one?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, "File aready exists") = MsgBoxResult.Yes Then
                        '        My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                        '    End If
                        'Else
                        '    My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                        'End If
                    End If
                    Dim percentage As Integer = CSng(counter) / CSng(filearray.Count) * 100
                    If precountitems > 0 Then
                        ToolStripProgressBar2.Value = percentage
                    Else
                        ToolStripProgressBar2.Value = 100
                    End If
                    counter = counter + 1
                Catch ex As Exception
                    Error_Handler(ex, "Downloading File: " & filetodownloadpath)
                End Try
            Next

            filearray.Clear()
            filearray = Nothing
            GroupBox1.Text = "Upload your files"
            Label2.Text = "Drag and drop the files or folders you wish to upload here"
            'WebBrowser1.Refresh()
            ToolStripStatusLabel2.Text = "All Content downloaded"
            If downloadcounter <> 1 Then
                MsgBox("Operation Complete. " & downloadcounter & " files were downloaded from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Files Downloaded")
            Else
                MsgBox("Operation Complete. " & downloadcounter & " file was downloaded from the current working folder", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "All Files Downloaded")
            End If


        Catch ex As Exception
            Error_Handler(ex, "Downloading all Content")
        End Try
    End Sub

    Private Sub BackgroundWorker9_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker9.RunWorkerCompleted
        Try
            ToolStripStatusLabel2.Text = "Content Downloading Complete"
            ToolStripProgressBar2.Visible = False
            controls_enabler(True)
        Catch ex As Exception
            Error_Handler(ex, "Downloading all Content")
        End Try
    End Sub

    Private Sub recursivedownloader(ByVal inputtodownloadpath As String, ByVal downloadtopath As String)
        Try
            If My.Computer.FileSystem.DirectoryExists(downloadtopath) = False Then
                My.Computer.FileSystem.CreateDirectory(downloadtopath)
            End If
            Dim request As FtpWebRequest = WebRequest.Create(New Uri(inputtodownloadpath))
            request.Credentials = New NetworkCredential(currentusername, currentpassword)
            request.Method = WebRequestMethods.Ftp.ListDirectory
            Dim filearray As ArrayList = New ArrayList
            Dim response As FtpWebResponse = request.GetResponse()
            Dim reader As StreamReader = New StreamReader(response.GetResponseStream)
            Dim lineread As String = ""
            While reader.Peek <> -1
                lineread = reader.ReadLine
                While lineread.IndexOf("/") <> -1
                    lineread = lineread.Remove(0, lineread.IndexOf("/") + 1)
                End While
                filearray.Add(lineread)
            End While
            reader.Close()
            response.Close()
            request = Nothing
            response = Nothing
            reader = Nothing
            request = WebRequest.Create(New Uri(inputtodownloadpath))
            request.Credentials = New NetworkCredential(currentusername, currentpassword)
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails
            response = request.GetResponse()
            reader = New StreamReader(response.GetResponseStream)
            Dim counter As Integer = 0
            While reader.Peek <> -1
                lineread = reader.ReadLine
                If lineread.ToLower.StartsWith("d") Then
                    filearray.Item(counter) = "_directory_ " & filearray.Item(counter)
                End If
                counter = counter + 1
            End While
            reader.Close()
            response.Close()
            response = Nothing
            request = Nothing
            reader = Nothing
            counter = 1

            GroupBox1.Text = "Folder Download in Progress"
            Label2.Text = ""
            Dim filetodownloadpath As String = ""
            For Each filename As String In filearray
                Try
                    ToolStripStatusLabel2.Text = "Downloading file " & counter & " (of " & filearray.Count & ")"
                    If filename.StartsWith("_directory_ ") = True Then
                        If inputtodownloadpath.EndsWith("/") Then
                            filetodownloadpath = inputtodownloadpath & filename
                        Else
                            filetodownloadpath = inputtodownloadpath & "/" & filename
                        End If
                        'downloadcounter = downloadcounter + 1
                        Label2.Text = "Downloading " & filename
                        recursivedownloader(filetodownloadpath.Replace("_directory_ ", ""), downloadtopath & "\" & filename.Replace("_directory_ ", ""))

                        'If My.Computer.FileSystem.FileExists(downloadtopath & "\" & filename) = True Then
                        '    If MsgBox(downloadtopath & "\" & filename & " arleady exists on the system. Do you wish to replace the existing file with this one?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, "File aready exists") = MsgBoxResult.Yes Then
                        '        My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                        '    End If
                        'Else
                        '    My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                        'End If
                    Else

                        If inputtodownloadpath.EndsWith("/") Then
                            filetodownloadpath = inputtodownloadpath & filename
                        Else
                            filetodownloadpath = inputtodownloadpath & "/" & filename
                        End If
                        'downloadcounter = downloadcounter + 1
                        Label2.Text = "Downloading " & filename
                        If My.Computer.FileSystem.FileExists(downloadtopath & "\" & filename) = True Then
                            If MsgBox(downloadtopath & "\" & filename & " arleady exists on the system. Do you wish to replace the existing file with this one?", MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, "File aready exists") = MsgBoxResult.Yes Then
                                My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                            End If
                        Else
                            My.Computer.Network.DownloadFile(filetodownloadpath, downloadtopath & "\" & filename, currentusername, currentpassword, True, 100000, True)
                        End If


                    End If
                    Dim percentage As Integer = CSng(counter) / CSng(filearray.Count) * 100
                    If precountitems > 0 Then
                        ToolStripProgressBar2.Value = percentage
                    Else
                        ToolStripProgressBar2.Value = 100
                    End If
                    counter = counter + 1
                Catch ex As Exception
                    Error_Handler(ex, "Downloading File: " & filetodownloadpath)
                End Try
            Next

            filearray.Clear()
            filearray = Nothing
        Catch ex As Exception
            Error_Handler(ex, "Recursive Downloader:" & inputtodownloadpath & " - " & downloadtopath)
        End Try

    End Sub

    Private Sub SingleFileDownload(ByVal lastURL As String)
        Try
            If currentusername.IndexOf("\") <> -1 Then
                currentusername = currentusername.Substring(0, currentusername.IndexOf("\"))
            End If
            Dim UploadPath As String
            UploadPath = WebBrowser1.Url.ToString.Remove(0, WebBrowser1.Url.ToString.IndexOf("=") + 1)
            UploadPath = UploadPath.Substring(0, UploadPath.LastIndexOf("&"))
            UploadPath = UploadPath.Remove(0, currentusername.Length)
            UploadPath = ("ftp:////comsan.uct.ac.za/" & UploadPath).Replace("\", "/").Replace("//", "/")
            SaveFileDialog1.FileName = UploadPath.Remove(0, UploadPath.LastIndexOf("/") + 1)
            If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim downloadtopath As String = SaveFileDialog1.FileName
                Try
                    My.Computer.Network.DownloadFile(UploadPath, downloadtopath, currentusername, currentpassword, True, 100000, True)
                Catch ex As Exception
                    Error_Handler(ex, "Single File Download: " & UploadPath)
                End Try
            End If
            WebBrowser1.Navigate(lastURL)
            isBusy = False

        Catch ex As Exception
            Error_Handler(ex, "Single File Download")
        End Try
    End Sub

    Private Sub WebBrowser1_Navigated(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserNavigatedEventArgs) Handles WebBrowser1.Navigated
        lastnavigated = Format(Now, "yyyymmddHHmmss")
    End Sub

    Private Sub FTPViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Process.Start("explorer ftp://comsan.uct.ac.za")
        Catch ex As Exception
            Error_Handler(ex, "Open FTP View")
        End Try
    End Sub
End Class
