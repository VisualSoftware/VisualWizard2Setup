'============================================================================
'
'    VisualWizard2Setup
'    Copyright (C) 2012 - 2015 Visual Software Corporation
'
'    Author: ASV93
'    File: Form1.vb
'
'    This program is free software; you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation; either version 2 of the License, or
'    (at your option) any later version.
'
'    This program is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License along
'    with this program; if not, write to the Free Software Foundation, Inc.,
'    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
'
'============================================================================

Imports System.Net
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CompilerServices
Imports SimpleSerials
Imports ICSharpCode.SharpZipLib
Imports System.Xml


Public Class Form1

    Dim wizappname As String
    Dim wizappver As String
    Dim wizappfile As String
    Dim runwhenfinish As Integer
    Dim wizappexe As String
    Dim showlicense As Integer
    Dim showpkey As Integer
    Dim appid As String
    Friend disableid As String
    Dim wizzip As New ICSharpCode.SharpZipLib.Zip.FastZip
    Dim nteval As String
    Dim uicolor As String
    Dim bgimg As String
    Dim online As Integer
    Dim setupserver As String
    Dim wizdownloader As New WebClient()
    Dim globalsize As String
    Dim globalsize2 As String
    Dim saltkey As String
    Dim dlspeed As String
    Dim devname As String
    Dim appurl As String
    Dim extupd As String
    Dim defeula As String
    Dim remainingtime As String = "Calculating..."
    Dim portableapp As String
    Dim VSTools As VSSharedSource = New VSSharedSource
    Dim setuptype As String
    Dim uselocalbranch As String
    Dim extupdsrv As String
    Dim lastupdateurl As String
    Dim PBDLVal As String
    Dim MaxPBVal As String
    Dim SilentCFU As String
    Friend SetupMD5 As String
    Dim CommandLineArgs As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        TabControl1.SelectedIndex = "1"
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        End
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'XML LOAD
        If IO.File.Exists(My.Application.Info.DirectoryPath & "\Settings.dat") = True Then
            TextBox4.Text = IO.File.ReadAllText(My.Application.Info.DirectoryPath & "\Settings.dat")
            Dim dec As Encriptador
            dec = New Encriptador()
            TextBox4.Text = dec.DesEncriptarCadena(TextBox4.Text)
            Dim doc As New XmlDocument
            doc.LoadXml(TextBox4.Text)
            Dim nodes As XmlNodeList = doc.SelectNodes("VISUALWIZARD2/SetupConfig")
            For Each node As XmlNode In nodes
                'FILE VERSION 1.0 ################################
                'BASIC
                wizappname = node.SelectSingleNode("AppName").InnerText
                wizappfile = node.SelectSingleNode("PackageName").InnerText
                wizappexe = node.SelectSingleNode("ExecutableName").InnerText
                uicolor = node.SelectSingleNode("UIColor").InnerText
                runwhenfinish = node.SelectSingleNode("RunAppWhenFinish").InnerText
                portableapp = node.SelectSingleNode("PortableApp").InnerText
                'ADVANCED
                showlicense = node.SelectSingleNode("ShowEULA").InnerText
                If node.SelectSingleNode("EULAFile").InnerText = "" Then
                    defeula = "1"
                Else
                    TextBox2.Text = node.SelectSingleNode("EULAFile").InnerText
                End If
                showpkey = node.SelectSingleNode("ShowPKEY").InnerText
                appid = node.SelectSingleNode("KeyVar1").InnerText
                saltkey = node.SelectSingleNode("KeyVar2").InnerText
                online = node.SelectSingleNode("OnlineSetup").InnerText
                setupserver = node.SelectSingleNode("SetupURL").InnerText
                'FILE VERSION 1.1 ################################
                'BASIC
                wizappver = node.SelectSingleNode("AppVersion").InnerText
                devname = node.SelectSingleNode("DevName").InnerText
                appurl = node.SelectSingleNode("AppWebsite").InnerText
                extupd = node.SelectSingleNode("EnableUpdate").InnerText
                extupdsrv = node.SelectSingleNode("UpdateURL").InnerText
            Next
        Else
            MessageBox.Show("Error: Settings.dat does not exist. Setup will now exit", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        End If
        Label13.Text = "Visual Wizard 2" & VSTools.GetCopyrightDate() & " Visual Software"
        'VARS
        bgimg = "default"
        If devname = "" Then
            Label26.Text = ""
        ElseIf devname = "Visual Software" Then
            Label26.Visible = False
        Else
            Label26.Text = "© " & Now.Year & " " & devname
            If showlicense = "1" Then
                If defeula = "1" Then
                    TextBox2.Text = TextBox2.Text.Replace("Visual Software", devname)
                End If
            End If
        End If
        disableid = getMD5Hash(wizappname) & ".txt"
        VSTools.appname = getMD5Hash(wizappname)
        If runwhenfinish = 1 Then
            CheckBox1.Enabled = True
        Else
            CheckBox1.Enabled = False
            CheckBox1.Checked = False
        End If
        'STRINGS
        Me.Text = wizappname & " - Setup"
        If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappexe) = True Then
            'REPAIR
            Label5.Text = "Repair " & wizappname
            Label28.Enabled = True
            If extupd = "1" Then
                Label27.Enabled = True
            Else
                Label27.Enabled = False
            End If
        Else
            'INSTALL
            Label5.Text = "Install " & wizappname & " " & wizappver
            Label28.Enabled = False
            Label27.Enabled = False
        End If
        Label27.Text = "Update " & wizappname
        Label28.Text = "Uninstall " & wizappname
        Label7.Text = "Specify the directory where " & wizappname & " will be installed:"
        Label10.Text = "Please wait while Visual Wizard 2 installs " & wizappname
        Label12.Text = wizappname & " has been successfully installed. You can now use the application. Click on the Finish button to exit from Visual Wizard 2."
        CheckBox1.Text = "Run " & wizappname
        Label16.Text = "Enter the Product Key you received when buying " & wizappname & ":"
        'UI
        If uicolor = "1" Then
            'blue
            nextnormal.Image = PictureBox3.Image
            nexthot.Image = PictureBox1.Image
            Label1.BackColor = Label17.BackColor
            PictureBox5.Image = PictureBox8.Image
            PictureBox6.Image = PictureBox7.Image
        ElseIf uicolor = "2" Then
            'red/orange
            nextnormal.Image = My.Forms.CustomUI.PictureBox3.Image
            nexthot.Image = My.Forms.CustomUI.PictureBox1.Image
            Label1.BackColor = My.Forms.CustomUI.Label17.BackColor
            PictureBox5.Image = My.Forms.CustomUI.PictureBox9.Image
            PictureBox6.Image = My.Forms.CustomUI.PictureBox10.Image
        ElseIf uicolor = "3" Then
            'yellow
            nextnormal.Image = My.Forms.CustomUI.PictureBox6.Image
            nexthot.Image = My.Forms.CustomUI.PictureBox5.Image
            Label1.BackColor = My.Forms.CustomUI.Label1.BackColor
            PictureBox5.Image = My.Forms.CustomUI.PictureBox12.Image
            PictureBox6.Image = My.Forms.CustomUI.PictureBox11.Image
        ElseIf uicolor = "4" Then
            'purple
            nextnormal.Image = My.Forms.CustomUI.PictureBox8.Image
            nexthot.Image = My.Forms.CustomUI.PictureBox7.Image
            Label1.BackColor = My.Forms.CustomUI.Label2.BackColor
            PictureBox5.Image = My.Forms.CustomUI.PictureBox14.Image
            PictureBox6.Image = My.Forms.CustomUI.PictureBox13.Image
        Else
            'green (default)
        End If
        PictureBox10.BackColor = Label1.BackColor
        If bgimg = "bg1" Then
            PictureBox2.BackgroundImage = My.Forms.CustomUI.PictureBox2.BackgroundImage
        ElseIf bgimg = "bg2" Then
            PictureBox2.BackgroundImage = My.Forms.CustomUI.PictureBox4.BackgroundImage
        Else
            'default
        End If
        If portableapp = 1 Then
            PictureBox9.Visible = True
            Label25.Visible = True
        Else

        End If
        picturebox4.Image = PictureBox5.Image
        Label20.Image = PictureBox5.Image
        Label21.Image = PictureBox5.Image
        Label22.Image = PictureBox5.Image
        Label23.Image = PictureBox5.Image
        Label24.Image = PictureBox5.Image
        Label6.Image = PictureBox5.Image
        picturebox4.BackColor = Label1.BackColor
        Label20.BackColor = Label1.BackColor
        Label21.BackColor = Label1.BackColor
        Label22.BackColor = Label1.BackColor
        Label23.BackColor = Label1.BackColor
        Label24.BackColor = Label1.BackColor
        Label6.BackColor = Label1.BackColor
        Label5.Image = nextnormal.Image
        Label27.Image = nextnormal.Image
        Label28.Image = nextnormal.Image
        Label2.Font = Label1.Font
        Label8.Font = Label1.Font
        Label11.Font = Label1.Font
        Label14.Font = Label1.Font
        Label15.Font = Label1.Font
        Label29.Font = Label1.Font
        Label2.BackColor = Label1.BackColor
        Label8.BackColor = Label1.BackColor
        Label11.BackColor = Label1.BackColor
        Label14.BackColor = Label1.BackColor
        Label15.BackColor = Label1.BackColor
        Label29.BackColor = Label1.BackColor
        If IO.File.Exists(wizappfile) = True Then
            online = 0
        Else
            If online = 1 Then

            Else
                Label5.Enabled = False
            End If
        End If
        TabPage1.BackgroundImage = PictureBox2.BackgroundImage
        TabPage2.BackgroundImage = PictureBox2.BackgroundImage
        TabPage3.BackgroundImage = PictureBox2.BackgroundImage
        TabPage4.BackgroundImage = PictureBox2.BackgroundImage
        TabPage5.BackgroundImage = PictureBox2.BackgroundImage
        TabPage6.BackgroundImage = PictureBox2.BackgroundImage
        If portableapp = 1 Then
            TextBox1.Text = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\"
        Else
            TextBox1.Text = My.Computer.FileSystem.SpecialDirectories.ProgramFiles & "\"
        End If
        If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappfile & ".upd") = True Then
            IO.File.Delete(My.Application.Info.DirectoryPath & "\" & wizappfile & ".upd")
        End If
        'CHECK MD5 OF SETUP FILES
        If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappfile) = True Then
            SetupMD5 = MD5CalcFile(My.Application.Info.DirectoryPath & "\" & wizappfile)
        Else

        End If
        VSTools.appname2 = SetupMD5
        VSTools.appname = disableid
        'LOAD FINISHED, CHECK ARGS
        Dim Allargs As String
        If My.Application.CommandLineArgs.Count = 0 Then
            'No args
        Else
            'Args
            For i As Integer = 0 To CommandLineArgs.Count - 1
                Allargs = Allargs & " " & CommandLineArgs(i)
            Next
            If Allargs.Contains("-autoupdate") Then
                'CHECK FOR UPDATES BUT BEFORE CHECK IF RML IS ON
                SilentCFU = "1"
                If IO.File.Exists(My.Application.Info.DirectoryPath & "\RML.dat") = True Then
                    Dim RMLTime As String = IO.File.ReadAllText(My.Application.Info.DirectoryPath & "\RML.dat")
                    If Now.Date > RMLTime Then
                        'CHECK FOR UPDATES
                        Label27_Click(sender, e)
                        If TextBox5.Text = "Error" Then
                            'NO UPDATES
                            End
                        Else
                            If TextBox5.Text > TextBox6.Text Then
                                'UPDATES
                                My.Forms.UpdateReminder.Label1.Text = My.Forms.UpdateReminder.Label1.Text.Replace("%APPNAME%", wizappname)
                                My.Forms.UpdateReminder.Label1.Text = My.Forms.UpdateReminder.Label1.Text.Replace("%BRANCHNAME%", ListView1.Items(ComboBox1.SelectedIndex).SubItems(0).Text)
                                My.Forms.UpdateReminder.Text = My.Forms.UpdateReminder.Text & " (" & TextBox5.Text & ")"
                                UpdateReminder.ShowDialog()
                            Else
                                'NO UPDATES
                                End
                            End If
                        End If
                    Else
                        'IGNORE
                        End
                    End If
                Else
                    'CHECK FOR UPDATES
                    Label27_Click(sender, e)
                    If TextBox5.Text = "Error" Then
                        'NO UPDATES
                        End
                    Else
                        If TextBox5.Text > TextBox6.Text Then
                            'UPDATES
                            My.Forms.UpdateReminder.Label1.Text = My.Forms.UpdateReminder.Label1.Text.Replace("%APPNAME%", wizappname)
                            My.Forms.UpdateReminder.Label1.Text = My.Forms.UpdateReminder.Label1.Text.Replace("%BRANCHNAME%", ListView1.Items(ComboBox1.SelectedIndex).SubItems(0).Text)
                            My.Forms.UpdateReminder.Text = My.Forms.UpdateReminder.Text & " (" & TextBox5.Text & ")"
                            UpdateReminder.ShowDialog()
                        Else
                            'NO UPDATES
                            End
                        End If
                    End If
                End If
            Else
                'UNRECOGNIZED ARG
            End If
        End If
        GetAppCert()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If showpkey = 1 Then
            TabControl1.SelectedIndex = "5"
        Else
            If showlicense = 1 Then
                TabControl1.SelectedIndex = "4"
            Else
                TabControl1.SelectedIndex = "0"
            End If
        End If
    End Sub

    Private Sub Label5_MouseHover(sender As Object, e As EventArgs) Handles Label5.MouseHover
        Label5.Image = nexthot.Image
        Label5.Font = Label4.Font
    End Sub

    Private Sub Label5_MouseLeave(sender As Object, e As EventArgs) Handles Label5.MouseLeave
        Label5.Image = nextnormal.Image
        Label5.Font = Label3.Font
    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click
        setuptype = "Install"
        If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappexe) = True Then
            CheckBox2.Checked = False
            If showpkey = "1" Then
                If IO.File.Exists(My.Application.Info.DirectoryPath & "\PKey.dat") Then
                    TextBox3.Text = IO.File.ReadAllText(My.Application.Info.DirectoryPath & "\PKey.dat")
                    TabControl1.SelectedIndex = "5"
                Else
                    TabControl1.SelectedIndex = "5"
                End If
            Else
                Button4_Click(sender, e)
            End If
        Else
            If showlicense = "1" Then
                TabControl1.SelectedIndex = "4"
            Else
                If showpkey = "1" Then
                    TabControl1.SelectedIndex = "5"
                Else
                    TabControl1.SelectedIndex = "1"
                End If
            End If
        End If
    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs)
        End
    End Sub

    Private Sub Label27_MouseHover(sender As Object, e As EventArgs) Handles Label27.MouseHover
        Label27.Image = nexthot.Image
        Label27.Font = Label4.Font
    End Sub

    Private Sub Label27_MouseLeave(sender As Object, e As EventArgs) Handles Label27.MouseLeave
        Label27.Image = nextnormal.Image
        Label27.Font = Label3.Font
    End Sub

    Private Sub Label28_MouseHover(sender As Object, e As EventArgs) Handles Label28.MouseHover
        Label28.Image = nexthot.Image
        Label28.Font = Label4.Font
    End Sub

    Private Sub Label28_MouseLeave(sender As Object, e As EventArgs) Handles Label28.MouseLeave
        Label28.Image = nextnormal.Image
        Label28.Font = Label3.Font
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim newconfirm As Object
        newconfirm = MessageBox.Show("Are you sure you want to cancel the installation?", "Cancel Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If newconfirm = vbYes Then
            Me.Dispose()
            Me.Close()
        Else

        End If
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        FolderBrowserDialog1.ShowDialog()
        If FolderBrowserDialog1.SelectedPath = "" Then

        Else
            TextBox1.Text = FolderBrowserDialog1.SelectedPath & "\"
        End If
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text = "" Then
            Button4.Enabled = False
        Else
            Button4.Enabled = True
        End If
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        Button5_Click(sender, e)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        TabControl1.SelectedIndex = "2"
        If showpkey = 1 Then
            Dim dec As Encriptador
            dec = New Encriptador()
            Dim plainText As String = TextBox3.Text
            Dim password As String = saltkey
            Dim wrapper As New VSSharedSource.Simple3Des(password)
            Dim cipherText As String = wrapper.DecryptData(plainText)
            If SimpleSerials.Serial.ValidateSerial(cipherText, dec.DesEncriptarCadena(appid), saltkey) Then

            Else
                MessageBox.Show("Error, Package decryption failed", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End
            End If
        Else

        End If
        If IO.Directory.Exists(TextBox1.Text & devname & "\" & wizappname) = True Then
            If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappexe) = True Then

            Else
                If IO.File.Exists(TextBox1.Text & devname & "\" & wizappname & "\" & wizappfile) = True Then
                    IO.File.Delete(TextBox1.Text & devname & "\" & wizappname & "\" & wizappfile)
                Else

                End If
            End If
        Else
            IO.Directory.CreateDirectory(TextBox1.Text & devname & "\" & wizappname)
        End If
        If online = 1 Then
            Dim dlspeed As String
            dlspeed = "0"
            Label9.Text = "Downloading " & wizappname & "..."
            Timer2.Enabled = True
            Label19.Visible = True
        Else

        End If
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            If setuptype = "Install" Then
                If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappexe) = True Then
                    'REINSTALL
                    If online = 1 Then
                        'GET SIZE
                        Dim Request As System.Net.WebRequest
                        Dim Response As System.Net.WebResponse
                        Dim FileSize As Integer
                        Request = Net.WebRequest.Create(setupserver)
                        Request.Method = Net.WebRequestMethods.Http.Head
                        Response = Request.GetResponse
                        FileSize = Response.ContentLength
                        ProgressBar1.Maximum = FileSize
                        'DOWNLOAD
                        wizdownloader.DownloadFile(setupserver, My.Application.Info.DirectoryPath & "\" & wizappfile)
                    Else

                    End If
                Else
                    'NORMAL INSTALLATION
                    If online = 1 Then
                        'GET SIZE
                        Dim Request As System.Net.WebRequest
                        Dim Response As System.Net.WebResponse
                        Dim FileSize As Integer
                        Request = Net.WebRequest.Create(setupserver)
                        Request.Method = Net.WebRequestMethods.Http.Head
                        Response = Request.GetResponse
                        FileSize = Response.ContentLength
                        ProgressBar1.Maximum = FileSize
                        'DOWNLOAD
                        wizdownloader.DownloadFile(setupserver, TextBox1.Text & devname & "\" & wizappname & "\" & wizappfile)
                    Else
                        'GET SIZE OF LOCAL FILE
                        Dim flsz As String
                        Dim fFile As New FileInfo(My.Application.Info.DirectoryPath & "\" & wizappfile)
                        Dim fSize As Integer = fFile.Length
                        ProgressBar1.Maximum = Val(fSize)
                        ProgressBar1.Value = Val(fSize) / 2
                        IO.File.Copy(My.Computer.FileSystem.CurrentDirectory & "\" & wizappfile, TextBox1.Text & devname & "\" & wizappname & "\" & wizappfile)
                    End If
                End If
            Else
                'UPDATE
                'GET SIZE
                Dim Request As System.Net.WebRequest
                Dim Response As System.Net.WebResponse
                Dim FileSize As Integer
                Request = Net.WebRequest.Create(lastupdateurl & "/" & wizappfile)
                Request.Method = Net.WebRequestMethods.Http.Head
                Response = Request.GetResponse
                FileSize = Response.ContentLength
                ProgressBar1.Maximum = FileSize
                'DOWNLOAD
                wizdownloader.DownloadFile(lastupdateurl & "/" & wizappfile, My.Application.Info.DirectoryPath & "\" & wizappfile & ".upd")
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        End Try
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Label9.Text = "Installing..."
        Timer2.Enabled = False
        PBDLVal = ProgressBar1.Value
        MaxPBVal = ProgressBar1.Maximum
        Timer3.Enabled = True
        Label19.Visible = False
        BackgroundWorker2.RunWorkerAsync()
    End Sub

    Private Sub BackgroundWorker2_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        Try
            If setuptype = "Install" Then
                If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappexe) = True Then
                    If showpkey = 1 Then
                        Dim dec As Encriptador
                        dec = New Encriptador()
                        Dim tmppwd As String
                        tmppwd = dec.DesEncriptarCadena(appid) & "vw2"
                        wizzip.Password = tmppwd
                        wizzip.ExtractZip(My.Application.Info.DirectoryPath & "\" & wizappfile, My.Application.Info.DirectoryPath & "\", "")
                    Else
                        wizzip.ExtractZip(My.Application.Info.DirectoryPath & "\" & wizappfile, My.Application.Info.DirectoryPath & "\", "")
                    End If
                Else
                    If showpkey = 1 Then
                        Dim dec As Encriptador
                        dec = New Encriptador()
                        Dim tmppwd As String
                        tmppwd = dec.DesEncriptarCadena(appid) & "vw2"
                        wizzip.Password = tmppwd
                        wizzip.ExtractZip(TextBox1.Text & devname & "\" & wizappname & "\" & wizappfile, TextBox1.Text & devname & "\" & wizappname & "\", "")
                    Else
                        wizzip.ExtractZip(TextBox1.Text & devname & "\" & wizappname & "\" & wizappfile, TextBox1.Text & devname & "\" & wizappname & "\", "")
                    End If
                    'COPY SETUP FILES
                    If IO.File.Exists(TextBox1.Text & devname & "\" & wizappname & "\PKey.dat") = True Then
                        IO.File.Delete(TextBox1.Text & devname & "\" & wizappname & "\PKey.dat")
                    Else

                    End If
                    IO.File.WriteAllText(TextBox1.Text & devname & "\" & wizappname & "\PKey.dat", TextBox3.Text)

                    If IO.File.Exists(My.Application.Info.DirectoryPath & "\Setup.dat") = True Then
                        IO.File.Copy(My.Application.Info.DirectoryPath & "\Setup.dat", TextBox1.Text & devname & "\" & wizappname & "\Setup.exe")
                    Else
                        IO.File.Copy(My.Application.Info.DirectoryPath & "\Setup.exe", TextBox1.Text & devname & "\" & wizappname & "\Setup.exe")
                    End If
                    IO.File.Copy(My.Application.Info.DirectoryPath & "\Settings.dat", TextBox1.Text & devname & "\" & wizappname & "\Settings.dat")
                    IO.File.Copy(My.Application.Info.DirectoryPath & "\ICSharpCode.SharpZipLib.dll", TextBox1.Text & devname & "\" & wizappname & "\ICSharpCode.SharpZipLib.dll")
                    IO.File.Copy(My.Application.Info.DirectoryPath & "\SimpleSerials.dll", TextBox1.Text & devname & "\" & wizappname & "\SimpleSerials.dll")

                End If
            Else
                'UPDATE
                If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappfile) = True Then
                    IO.File.Delete(My.Application.Info.DirectoryPath & "\" & wizappfile)
                End If
                My.Computer.FileSystem.RenameFile(My.Application.Info.DirectoryPath & "\" & wizappfile & ".upd", wizappfile)
                If showpkey = 1 Then
                    Dim dec As Encriptador
                    dec = New Encriptador()
                    Dim tmppwd As String
                    tmppwd = dec.DesEncriptarCadena(appid) & "vw2"
                    wizzip.Password = tmppwd
                    wizzip.ExtractZip(My.Application.Info.DirectoryPath & "\" & wizappfile, My.Application.Info.DirectoryPath & "\", "")
                Else
                    wizzip.ExtractZip(My.Application.Info.DirectoryPath & "\" & wizappfile, My.Application.Info.DirectoryPath & "\", "")
                End If
                CheckBox2.Checked = False
            End If

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        End Try
    End Sub

    Private Sub BackgroundWorker2_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        Timer3.Enabled = False
        Label9.Text = "Installation Completed"
        ProgressBar1.Value = ProgressBar1.Maximum
        TabControl1.SelectedIndex = "3"
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If CheckBox4.Checked = True Then
            If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappexe) = False Then
                If IO.File.Exists(TextBox1.Text & devname & "\" & wizappname & "\" & wizappfile) = True Then
                    IO.File.Delete(TextBox1.Text & devname & "\" & wizappname & "\" & wizappfile)
                Else

                End If
            Else
                If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappfile) = True Then
                    IO.File.Delete(My.Application.Info.DirectoryPath & "\" & wizappfile)
                Else

                End If
            End If
        Else
        End If
        If CheckBox1.Checked = True Then
            If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappexe) = True Then
                Shell(My.Application.Info.DirectoryPath & "\" & wizappexe, AppWinStyle.NormalFocus)
            Else
                Shell(TextBox1.Text & devname & "\" & wizappname & "\" & wizappexe, AppWinStyle.NormalFocus)
            End If
        Else

        End If
        If CheckBox2.Checked = True Then
            If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappexe) = True Then
                Shortcut.Create_Shortcut(My.Application.Info.DirectoryPath & "\" & wizappexe, My.Computer.FileSystem.SpecialDirectories.Desktop, wizappname, My.Application.Info.DirectoryPath & "\", "", 0, 0)
                Shortcut.Create_Shortcut(My.Application.Info.DirectoryPath & "\Setup.exe", My.Computer.FileSystem.SpecialDirectories.Desktop, "Configure " & wizappname, My.Application.Info.DirectoryPath & "\", "", 0, 0)
            Else
                Shortcut.Create_Shortcut(TextBox1.Text & devname & "\" & wizappname & "\" & wizappexe, My.Computer.FileSystem.SpecialDirectories.Desktop, wizappname, TextBox1.Text & devname & "\" & wizappname, "", 0, 0)
                Shortcut.Create_Shortcut(TextBox1.Text & devname & "\" & wizappname & "\Setup.exe", My.Computer.FileSystem.SpecialDirectories.Desktop, "Configure " & wizappname, TextBox1.Text & devname & "\" & wizappname, "", 0, 0)
            End If
        Else

        End If
        End
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        TabControl1.SelectedIndex = "0"
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Button5_Click(sender, e)
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked = True Then
            Button8.Enabled = True
        Else
            Button8.Enabled = False
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        If showpkey = 1 Then
            TabControl1.SelectedIndex = "5"
        Else
            TabControl1.SelectedIndex = "1"
        End If
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Button5_Click(sender, e)
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        If setuptype = "Install" Then
            If showlicense = "1" Then
                TabControl1.SelectedIndex = "4"
            Else
                TabControl1.SelectedIndex = "0"
            End If
        Else
            TabControl1.SelectedIndex = "0"
        End If
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        If IO.File.Exists("SimpleSerials.dll") = True Then
            Dim dec As Encriptador
            dec = New Encriptador()
            Dim plainText As String = TextBox3.Text
            Dim password As String = saltkey
            Dim wrapper As New VSSharedSource.Simple3Des(password)
            Dim cipherText As String = wrapper.DecryptData(plainText)
            If SimpleSerials.Serial.ValidateSerial(cipherText, dec.DesEncriptarCadena(appid), saltkey) Then
                If setuptype = "Update" Then
                    'Update
                    TabControl1.SelectedIndex = "6"
                    If IO.File.Exists(My.Application.Info.DirectoryPath & "\PKey.dat") = True Then
                        IO.File.Delete(My.Application.Info.DirectoryPath & "\PKey.dat")
                    Else

                    End If
                    IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\PKey.dat", TextBox3.Text)
                Else
                    If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappexe) = True Then
                        'Repair
                        Button4_Click(sender, e)
                        If IO.File.Exists(My.Application.Info.DirectoryPath & "\PKey.dat") = True Then
                            IO.File.Delete(My.Application.Info.DirectoryPath & "\PKey.dat")
                        Else

                        End If
                        IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\PKey.dat", TextBox3.Text)
                    Else
                        'Installation
                        TabControl1.SelectedIndex = "1"
                    End If
                End If
            Else
                Label18.Visible = True
                Button11.Enabled = False
            End If
        Else
            MessageBox.Show("One or more installation files are missing or corrupt, Setup cannot continue", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        End If
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        If TextBox3.Text = "" Then
            Button11.Enabled = False
        Else
            Button11.Enabled = True
        End If
        Label18.Visible = False
    End Sub

    Private Sub Label13_Click(sender As Object, e As EventArgs) Handles Label13.Click
        Label13.Text = "Dev: ASV93 // STT: arseny92 & evil_pro"
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        wizzip.ExtractZip("metro.pkg", "decompressed", "")
    End Sub

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        wizzip.Password = "yes"
        wizzip.CreateZip("metro.pkg", "MetroApp", True, "")
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If BackgroundWorker3.IsBusy = True Then

        Else
            BackgroundWorker3.RunWorkerAsync()
        End If
    End Sub

    Private Sub BackgroundWorker3_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker3.DoWork

    End Sub

    Function getMD5Hash(ByVal strToHash As String) As String
        Dim md5Obj As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(strToHash)

        bytesToHash = md5Obj.ComputeHash(bytesToHash)

        Dim strResult As String = ""

        For Each b As Byte In bytesToHash
            strResult += b.ToString("x2")
        Next

        Return strResult
    End Function

    Public Function MD5CalcFile(ByVal filepath As String) As String

        ' open file (as read-only)
        Using reader As New System.IO.FileStream(filepath, IO.FileMode.Open, IO.FileAccess.Read)
            Using md5 As New System.Security.Cryptography.MD5CryptoServiceProvider

                ' hash contents of this stream
                Dim hash() As Byte = md5.ComputeHash(reader)

                ' return formatted hash
                Return ByteArrayToString(hash)

            End Using
        End Using

    End Function

    ' utility function to convert a byte array into a hex string
    Private Function ByteArrayToString(ByVal arrInput() As Byte) As String

        Dim sb As New System.Text.StringBuilder(arrInput.Length * 2)

        For i As Integer = 0 To arrInput.Length - 1
            sb.Append(arrInput(i).ToString("X2"))
        Next

        Return sb.ToString().ToLower

    End Function

    Public Function Genera_Pass(Longitud As Byte, Optional MiCadena As String = "", Optional Codigo_ansii As Boolean = False, _
Optional Numeros As Boolean = False, Optional Minusculas As Boolean = False, Optional Mayusculas As _
Boolean = False, Optional Especiales As Boolean = False, Optional bRepetir As Boolean = True)
        On Error GoTo eti


        Const sNum = "1234567890"
        Const sMin = "abcdefghijklmnopqrstuvwxyz"
        Const sMay = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Const sEsp = "[]{}!¡¿?#$%&/()"


        Dim sIn As String
        Dim sFi As String
        Dim i As Integer
        Dim iRan As Integer
        Dim iCadFin As Integer


        sFi = ""


        If MiCadena = "" Then


            If Codigo_ansii = True Then
                For i = 33 To 126
                    sFi = sFi & Chr(i)
                Next
            Else
                If Numeros = True Then sFi = sFi & sNum
                If Minusculas = True Then sFi = sFi & sMin
                If Mayusculas = True Then sFi = sFi & sMay
                If Especiales = True Then sFi = sFi & sEsp
            End If
        Else
            sFi = MiCadena
        End If


        iCadFin = Len(sFi)


        If bRepetir = False Then
            If Longitud > iCadFin Then Longitud = iCadFin
        End If


        If iCadFin = 0 Or Longitud < 1 Then
            Genera_Pass = "Error" : Exit Function
        End If


        For i = 1 To Longitud
            Randomize()
            Do
                iRan = (Rnd(iCadFin) * iCadFin)

            Loop While iRan = 0

            sIn = sIn & Mid(sFi, iRan, 1)
            If bRepetir = False Then
                sFi = Replace(sFi, Mid(sFi, iRan, 1), "")
                iCadFin = iCadFin - 1
            End If
        Next


        Genera_Pass = sIn


        Exit Function
eti:
        MsgBox(Err.Number & " " & Err.Description, vbExclamation, "Error")
    End Function

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If TabControl1.SelectedIndex = 0 Then
            End
        Else
            Dim newconfirm As Object
            newconfirm = MessageBox.Show("Are you sure you want to cancel the installation?", "Cancel Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If newconfirm = vbYes Then
                End
            Else
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        If setuptype = "Install" Then
            If IO.File.Exists(TextBox1.Text & devname & "\" & wizappname & "\" & wizappfile) Then
                Try
                    Dim globalsizedisplayed As String
                    Dim globalsize2displayed As String
                    Dim dlspeeddisplayed As String
                    Dim flsz As String
                    Dim fFile As New FileInfo(TextBox1.Text & devname & "\" & wizappname & "\" & wizappfile)
                    Dim fSize As Integer = fFile.Length
                    Dim dlspeed2 As String
                    flsz = fSize
                    ProgressBar1.Value = Val(flsz) / 2
                    NumericUpDown1.Value = Val(flsz) / 1048576
                    NumericUpDown2.Value = Val(ProgressBar1.Maximum) / 1048576
                    globalsize = NumericUpDown1.Value
                    If globalsize = globalsize2 Then

                    Else
                        dlspeed2 = Val(globalsize - globalsize2)
                        remainingtime = Val(Val(Val(NumericUpDown2.Value - NumericUpDown1.Value) / dlspeed2)) / 60
                        dlspeed = Val(globalsize - globalsize2) * 1024
                    End If
                    NumericUpDown3.Value = Format(remainingtime, "Fixed")
                    globalsize2 = globalsize
                    globalsizedisplayed = Format(globalsize, "Fixed")
                    globalsize2displayed = Format(NumericUpDown2.Value, "Fixed")
                    dlspeeddisplayed = Format(dlspeed, "Fixed")
                    Label19.Text = globalsizedisplayed & " MB / " & globalsize2displayed & " MB @ " & dlspeeddisplayed & " KB/s. " & FormatNumber(NumericUpDown3.Value, 0) & " minute(s) left."
                Catch ex As Exception
                End Try
            ElseIf IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappfile) Then
                Try
                    Dim globalsizedisplayed As String
                    Dim globalsize2displayed As String
                    Dim dlspeeddisplayed As String
                    Dim flsz As String
                    Dim fFile As New FileInfo(My.Application.Info.DirectoryPath & "\" & wizappfile)
                    Dim fSize As Integer = fFile.Length
                    Dim dlspeed2 As String
                    flsz = fSize
                    ProgressBar1.Value = Val(flsz) / 2
                    NumericUpDown1.Value = Val(flsz) / 1048576
                    NumericUpDown2.Value = Val(ProgressBar1.Maximum) / 1048576
                    globalsize = NumericUpDown1.Value
                    If globalsize = globalsize2 Then

                    Else
                        dlspeed2 = Val(globalsize - globalsize2)
                        remainingtime = Val(Val(Val(NumericUpDown2.Value - NumericUpDown1.Value) / dlspeed2)) / 60
                        dlspeed = Val(globalsize - globalsize2) * 1024
                    End If
                    NumericUpDown3.Value = Format(remainingtime, "Fixed")
                    globalsize2 = globalsize
                    globalsizedisplayed = Format(globalsize, "Fixed")
                    globalsize2displayed = Format(NumericUpDown2.Value, "Fixed")
                    dlspeeddisplayed = Format(dlspeed, "Fixed")
                    Label19.Text = globalsizedisplayed & " MB / " & globalsize2displayed & " MB @ " & dlspeeddisplayed & " KB/s. " & FormatNumber(NumericUpDown3.Value, 0) & " minute(s) left."
                Catch ex As Exception
                End Try
            End If

        Else
            'UPDATE
            If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappfile & ".upd") Then
                Try
                    Dim globalsizedisplayed As String
                    Dim globalsize2displayed As String
                    Dim dlspeeddisplayed As String
                    Dim flsz As String
                    Dim fFile As New FileInfo(My.Application.Info.DirectoryPath & "\" & wizappfile & ".upd")
                    Dim fSize As Integer = fFile.Length
                    Dim dlspeed2 As String
                    flsz = fSize
                    ProgressBar1.Value = Val(flsz) / 2
                    NumericUpDown1.Value = Val(flsz) / 1048576
                    NumericUpDown2.Value = Val(ProgressBar1.Maximum) / 1048576
                    globalsize = NumericUpDown1.Value
                    If globalsize = globalsize2 Then

                    Else
                        dlspeed2 = Val(globalsize - globalsize2)
                        remainingtime = Val(Val(Val(NumericUpDown2.Value - NumericUpDown1.Value) / dlspeed2)) / 60
                        dlspeed = Val(globalsize - globalsize2) * 1024
                    End If
                    NumericUpDown3.Value = Format(remainingtime, "Fixed")
                    globalsize2 = globalsize
                    globalsizedisplayed = Format(globalsize, "Fixed")
                    globalsize2displayed = Format(NumericUpDown2.Value, "Fixed")
                    dlspeeddisplayed = Format(dlspeed, "Fixed")
                    Label19.Text = globalsizedisplayed & " MB / " & globalsize2displayed & " MB @ " & dlspeeddisplayed & " KB/s. " & FormatNumber(NumericUpDown3.Value, 0) & " minute(s) left."
                Catch ex As Exception
                End Try
            Else
            End If

        End If
    End Sub

    Private Sub PictureBox4_MouseHover(sender As Object, e As EventArgs) Handles picturebox4.MouseHover
        picturebox4.Image = PictureBox6.Image
    End Sub

    Private Sub PictureBox4_MouseLeave(sender As Object, e As EventArgs) Handles picturebox4.MouseLeave
        picturebox4.Image = PictureBox5.Image
    End Sub

    Private Sub Label20_Click(sender As Object, e As EventArgs) Handles Label20.Click
        Button3_Click(sender, e)
    End Sub

    Private Sub Label20_MouseHover(sender As Object, e As EventArgs) Handles Label20.MouseHover
        Label20.Image = PictureBox6.Image
    End Sub

    Private Sub Label20_MouseLeave(sender As Object, e As EventArgs) Handles Label20.MouseLeave
        Label20.Image = PictureBox5.Image
    End Sub

    Private Sub Label23_Click(sender As Object, e As EventArgs) Handles Label23.Click
        Button9_Click(sender, e)
    End Sub

    Private Sub Label23_MouseHover(sender As Object, e As EventArgs) Handles Label23.MouseHover
        Label23.Image = PictureBox6.Image
    End Sub

    Private Sub Label23_MouseLeave(sender As Object, e As EventArgs) Handles Label23.MouseLeave
        Label23.Image = PictureBox5.Image
    End Sub

    Private Sub Label24_Click(sender As Object, e As EventArgs) Handles Label24.Click
        Button12_Click(sender, e)
    End Sub

    Private Sub Label24_MouseHover(sender As Object, e As EventArgs) Handles Label24.MouseHover
        Label24.Image = PictureBox6.Image
    End Sub

    Private Sub Label24_MouseLeave(sender As Object, e As EventArgs) Handles Label24.MouseLeave
        Label24.Image = PictureBox5.Image
    End Sub

    Private Sub Label6_MouseHover(sender As Object, e As EventArgs) Handles Label24.MouseHover, Label6.MouseHover
        Label6.Image = PictureBox6.Image
    End Sub

    Private Sub Label6_MouseLeave(sender As Object, e As EventArgs) Handles Label24.MouseLeave, Label6.MouseLeave
        Label6.Image = PictureBox5.Image
    End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        CustomUI.Show()

    End Sub

    Private Sub OpenVisualSoftwareWebSiteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenVisualSoftwareWebSiteToolStripMenuItem.Click
        Process.Start("http://visualsoftware.wordpress.com")
    End Sub

    Private Sub VisualSoftCorpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VisualSoftCorpToolStripMenuItem.Click
        Process.Start("https://www.twitter.com/VisualSoftCorp")
    End Sub

    Private Sub DonateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DonateToolStripMenuItem.Click
        VSTools.OpenDonationPage()
    End Sub

    Private Sub Label26_Click(sender As Object, e As EventArgs) Handles Label26.Click
        If appurl.Count > 5 Then
            Process.Start(appurl)
        Else

        End If
    End Sub

    Friend Sub Label27_Click(sender As Object, e As EventArgs) Handles Label27.Click
        If My.Computer.Network.IsAvailable = False Then
            If SilentCFU = "1" Then

            Else
                MessageBox.Show("Error: No network connection is available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            setuptype = "Update"
            'LOAD LOCAL VERSION
            Dim FileProperties As FileVersionInfo = FileVersionInfo.GetVersionInfo(My.Application.Info.DirectoryPath & "\" & wizappexe)
            TextBox6.Text = FileProperties.FileVersion
            If IO.File.Exists(My.Application.Info.DirectoryPath & "\UpdateChannels.xml") = True Then
                IO.File.Delete(My.Application.Info.DirectoryPath & "\UpdateChannels.xml")
            Else

            End If
            If extupdsrv = "" Then
                uselocalbranch = 1
            Else
                uselocalbranch = 0
                GetLatestBranchFile()
            End If
            'LOAD UPDATE BRANCHES
            Dim xmlbranch As String
            Dim doc As New XmlDocument
            Dim nodes2 As XmlNodeList
            If uselocalbranch = 1 Then
                xmlbranch = My.Application.Info.DirectoryPath & "\Settings.dat"
                doc.LoadXml(TextBox4.Text)
                nodes2 = doc.SelectNodes("VISUALWIZARD2/UpdateConfig/UpdateChannel")
            Else
                xmlbranch = My.Application.Info.DirectoryPath & "\UpdateChannels.xml"
                doc.Load(xmlbranch)
                nodes2 = doc.SelectNodes("VISUALWIZARD2-UPDATECONFIG/UpdateChannel")
            End If
            For Each node1 As XmlNode In nodes2
                Dim UCName As String = node1.SelectSingleNode("Name").InnerText
                Dim UCPWD As String = node1.SelectSingleNode("Password").InnerText
                Dim UCURL As String = node1.SelectSingleNode("URL").InnerText
                ListView1.Items.Add(New ListViewItem(New String() {UCName, UCPWD, UCURL}))
                ComboBox1.Items.Add(UCName)
            Next
            ComboBox1.SelectedIndex = 0
            'END
            If showpkey = "1" Then
                If IO.File.Exists(My.Application.Info.DirectoryPath & "\PKey.dat") Then
                    TextBox3.Text = IO.File.ReadAllText(My.Application.Info.DirectoryPath & "\PKey.dat")
                    TabControl1.SelectedIndex = "5"
                Else
                    TabControl1.SelectedIndex = "5"
                End If
            Else
                TabControl1.SelectedIndex = "6"
            End If
        End If
    End Sub

    Private Sub GetLatestBranchFile()
        Try
            My.Computer.Network.DownloadFile(extupdsrv, My.Application.Info.DirectoryPath & "\UpdateChannels.xml")
        Catch ex As Exception
            Label33.Visible = True
            PictureBox11.Visible = True
            uselocalbranch = "1"
        End Try
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If IO.File.Exists(My.Application.Info.DirectoryPath & "\SrvVer.txt") = True Then
            IO.File.Delete(My.Application.Info.DirectoryPath & "\SrvVer.txt")
        Else

        End If
        If ListView1.Items(ComboBox1.SelectedIndex).SubItems(1).Text = "Yes" Then
            Dim ChanPassword As String
            If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & ListView1.Items(ComboBox1.SelectedIndex).SubItems(0).Text & ".dat") = True Then
                ChanPassword = IO.File.ReadAllText(My.Application.Info.DirectoryPath & "\" & ListView1.Items(ComboBox1.SelectedIndex).SubItems(0).Text & ".dat")
            Else
                ChanPassword = InputBox("This Update channel is password protected, please insert the password to unlock it.", "Unlock Update Channel")
            End If
            If ChanPassword = "" Then
                TextBox5.Text = "Error"
            Else
                Try
                    Dim plainText As String = ListView1.Items(ComboBox1.SelectedIndex).SubItems(2).Text
                    Dim password As String = ChanPassword
                    Dim wrapper As New VSSharedSource.Simple3Des(password)
                    Dim cipherText As String = wrapper.DecryptData(plainText)
                    My.Computer.Network.DownloadFile(cipherText & "/Version.txt", My.Application.Info.DirectoryPath & "\SrvVer.txt")
                    TextBox5.Text = IO.File.ReadAllText(My.Application.Info.DirectoryPath & "\SrvVer.txt")
                    lastupdateurl = cipherText
                    IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\" & ListView1.Items(ComboBox1.SelectedIndex).SubItems(0).Text & ".dat", ChanPassword)
                Catch ex As Exception
                    If ex.Message = "Bad Data. " Then
                        MessageBox.Show("Error: Invalid Password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Else
                        MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                    TextBox5.Text = "Error"
                    If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & ListView1.Items(ComboBox1.SelectedIndex).SubItems(0).Text & ".dat") = True Then
                        IO.File.Delete(My.Application.Info.DirectoryPath & "\" & ListView1.Items(ComboBox1.SelectedIndex).SubItems(0).Text & ".dat")
                    Else

                    End If
                End Try
            End If

        Else
            If IO.File.Exists(My.Application.Info.DirectoryPath & "\SrvVer.txt") = True Then
                IO.File.Delete(My.Application.Info.DirectoryPath & "\SrvVer.txt")
            Else

            End If
            Try
                My.Computer.Network.DownloadFile(ListView1.Items(ComboBox1.SelectedIndex).SubItems(2).Text & "/Version.txt", My.Application.Info.DirectoryPath & "\SrvVer.txt")
                TextBox5.Text = IO.File.ReadAllText(My.Application.Info.DirectoryPath & "\SrvVer.txt")
                lastupdateurl = ListView1.Items(ComboBox1.SelectedIndex).SubItems(2).Text
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                TextBox5.Text = "Error"
            End Try
        End If
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        Button5_Click(sender, e)
    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        ComboBox1.Items.Clear()
        ListView1.Items.Clear()
        If showpkey = 1 Then
            TabControl1.SelectedIndex = "5"
        Else
            TabControl1.SelectedIndex = "0"
        End If
    End Sub

    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged
        If TextBox5.Text = "Error" Then
            Button17.Enabled = False
        Else
            If TextBox5.Text > TextBox6.Text Then
                Button17.Enabled = True
            Else
                Button17.Enabled = False
            End If
        End If
    End Sub

    Private Sub Label6_Click_1(sender As Object, e As EventArgs) Handles Label6.Click
        Button18_Click(sender, e)
    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        TabControl1.SelectedIndex = "2"
        If showpkey = 1 Then
            Dim dec As Encriptador
            dec = New Encriptador()
            Dim plainText As String = TextBox3.Text
            Dim password As String = saltkey
            Dim wrapper As New VSSharedSource.Simple3Des(password)
            Dim cipherText As String = wrapper.DecryptData(plainText)
            If SimpleSerials.Serial.ValidateSerial(cipherText, dec.DesEncriptarCadena(appid), saltkey) Then

            Else
                MessageBox.Show("Error, Package decryption failed", "Setup", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End
            End If
        Else

        End If
        'KILL APP
        TextBox7.Text = TextBox7.Text.Replace("%APPNAME%", wizappname)
        TextBox7.Text = TextBox7.Text.Replace("%APPEXE%", wizappexe)
        If IO.File.Exists(My.Application.Info.DirectoryPath & "\Update.bat") Then

        Else
            IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\Update.bat", TextBox7.Text)
        End If
        Shell(My.Application.Info.DirectoryPath & "\Update.bat", AppWinStyle.MinimizedNoFocus)
        'END
        Dim dlspeed As String
        dlspeed = "0"
        Label9.Text = "Downloading update of " & wizappname & "..."
        Timer2.Enabled = True
        Label19.Visible = True
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub Label28_Click(sender As Object, e As EventArgs) Handles Label28.Click
        Dim newconfirm As Object
        newconfirm = MessageBox.Show("Are you sure you want to uninstall " & wizappname & "?" & vbCrLf & "Warning: All files from this folder will be permantently removed.", "Uninstall", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If newconfirm = vbYes Then
            Label8.Text = "             Uninstalling"
            Label10.Text = "Please wait while Visual Wizard 2 uninstalls " & wizappname
            Label9.Text = "Removing all files..."
            TabControl1.SelectedIndex = "2"
            ProgressBar1.Value = "0"
            BackgroundWorker4.RunWorkerAsync()
        Else
            End
        End If
    End Sub

    Private Sub BackgroundWorker4_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker4.DoWork
        Try
            Dim remove1 As String
            Dim remove2 As String
            remove1 = My.Computer.FileSystem.SpecialDirectories.Desktop & "\" & wizappname & ".lnk"
            remove2 = My.Computer.FileSystem.SpecialDirectories.Desktop & "\Configure " & wizappname & ".lnk"
            If IO.File.Exists(remove1) = True Then
                IO.File.Delete(remove1)
            Else

            End If
            If IO.File.Exists(remove2) = True Then
                IO.File.Delete(remove2)
            Else

            End If
            Dim myFile As String
            Dim mydir As String = My.Application.Info.DirectoryPath
            For Each myFile In Directory.GetFiles(mydir, "*.*")
                File.Delete(myFile)
            Next

        Catch ex As Exception
        End Try
    End Sub

    Private Sub BackgroundWorker4_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker4.RunWorkerCompleted
        ProgressBar1.Value = "100"
        TextBox8.Text = TextBox8.Text.Replace("%APPEXE%", wizappexe)
        MessageBox.Show(wizappname & " has been successfully uninstalled.", "Uninstall", MessageBoxButtons.OK, MessageBoxIcon.Information)
        IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\Uninstall.bat", TextBox8.Text)
        Shell(My.Application.Info.DirectoryPath & "\Uninstall.bat", AppWinStyle.MinimizedNoFocus)
        End
    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        'Calculate Folder Size
        Dim DirSize As String
        If setuptype = "Install" Then
            If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & wizappexe) = True Then
                'REPAIR
                DirSize = Val(GetFolderSize(My.Application.Info.DirectoryPath, True)) / 2
                Dim FinalPBVal As String
                FinalPBVal = Val(PBDLVal) + Val(DirSize)
                If Val(FinalPBVal) > Val(MaxPBVal) Then
                    ProgressBar1.Value = MaxPBVal
                Else
                    ProgressBar1.Value = FinalPBVal
                End If
            Else
                'INSTALL
                DirSize = Val(GetFolderSize(TextBox1.Text & devname & "\" & wizappname, True)) / 2
                Dim FinalPBVal As String
                FinalPBVal = Val(PBDLVal) + Val(DirSize)
                If Val(FinalPBVal) > Val(MaxPBVal) Then
                    ProgressBar1.Value = MaxPBVal
                Else
                    ProgressBar1.Value = FinalPBVal
                End If
            End If
        Else
            'UPDATE
            DirSize = Val(GetFolderSize(My.Application.Info.DirectoryPath, True)) / 2
            Dim FinalPBVal As String
            FinalPBVal = Val(PBDLVal) + Val(DirSize)
            If Val(FinalPBVal) > Val(MaxPBVal) Then
                ProgressBar1.Value = MaxPBVal
            Else
                ProgressBar1.Value = FinalPBVal
            End If
        End If

    End Sub

    Function GetFolderSize(ByVal DirPath As String, _
   Optional IncludeSubFolders As Boolean = True) As Long

        Dim lngDirSize As Long
        Dim objFileInfo As FileInfo
        Dim objDir As DirectoryInfo = New DirectoryInfo(DirPath)
        Dim objSubFolder As DirectoryInfo

        Try

            'add length of each file
            For Each objFileInfo In objDir.GetFiles()
                lngDirSize += objFileInfo.Length
            Next
            If IncludeSubFolders Then
                For Each objSubFolder In objDir.GetDirectories()
                    lngDirSize += GetFolderSize(objSubFolder.FullName)
                Next
            End If

        Catch Ex As Exception


        End Try

        Return lngDirSize
    End Function

    Private Sub DebugTimer_Tick(sender As Object, e As EventArgs) Handles DebugTimer.Tick
        Me.Text = "PRG VAL: " & ProgressBar1.Value & " | PRG MAXVAL: " & ProgressBar1.Maximum & " | DIRSIZE: " & Val(GetFolderSize(My.Application.Info.DirectoryPath)) / 2
    End Sub

    Private Sub GetAppCert()
        Try
            If IO.File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\VW2" & ".md5") = True Then
                IO.File.Delete(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\VW2" & ".md5")
            Else

            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub PictureBox10_Click(sender As Object, e As EventArgs) Handles PictureBox10.Click

    End Sub
End Class

Public Class Shortcut
    Public Shared Function Create_Shortcut(ByVal Target_Path As String, ByVal Shortcut_Path As String, ByVal Shortcut_Name As String, _
    ByVal Working_Directory As String, ByVal Arguments As String, ByVal Window_Style As Integer, ByVal Icon_Num As Integer) As Object
        Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("WScript.Shell", ""))
        Dim objectValue2 As Object = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(objectValue, Nothing, _
        "CreateShortcut", New Object() {Shortcut_Path & "\" & Shortcut_Name & ".lnk"}, Nothing, Nothing, Nothing))
        NewLateBinding.LateSet(objectValue2, Nothing, "TargetPath", New Object() {Target_Path}, Nothing, Nothing)
        NewLateBinding.LateSet(objectValue2, Nothing, "WorkingDirectory", New Object() {Working_Directory}, Nothing, Nothing)
        NewLateBinding.LateSet(objectValue2, Nothing, "Arguments", New Object() {Arguments}, Nothing, Nothing)
        NewLateBinding.LateSet(objectValue2, Nothing, "WindowStyle", New Object() {Window_Style}, Nothing, Nothing)
        NewLateBinding.LateSet(objectValue2, Nothing, "IconLocation", New Object() {Target_Path & "," _
        & Conversions.ToString(Icon_Num)}, Nothing, Nothing)
        NewLateBinding.LateCall(objectValue2, Nothing, "Save", New Object() {}, Nothing, Nothing, Nothing, True)
        Return True
    End Function
End Class

Public Class Encriptador
    Private patron_busqueda As String = "7wW$va8çpoLár}Ek¹iBN(unY%#½t<¼íKR4>óGeS¨Az;QD{6=¿/*yf1³cZ!?+@²gÑª9€®3'\ x´0P-MJ^j`ñ5»2_·©]X.OmT~ºU«H)|CI,Vhsbé¾§úF¡:l&¬[÷Çqd"
    Private Patron_encripta As String = "zOº[a:áÑGÇ÷45 Uv½n§tçc´¼{W>kfmb6.HZgR/#JN-íhyX\`^?,9%_F'¬©¿*~E$YSé!A<&«L;¨21Dexw²sªB}=@¹®ñIipj7ó]dl0¾8MrCT€)3Pq(ú¡+o»|³Q·KuV"
    'WARNING: These values are used in VW2 Setup and VW2 Manager, If you want to change them you must change them in both apps.

    Private Function EncriptarCaracter(ByVal caracter As String, _
    ByVal variable As Integer, _
    ByVal a_indice As Integer) As String

        Dim caracterEncriptado As String, indice As Integer

        If patron_busqueda.IndexOf(caracter) <> -1 Then
            indice = (patron_busqueda.IndexOf(caracter) + variable + a_indice) Mod patron_busqueda.Length
            Return Patron_encripta.Substring(indice, 1)
        End If

        Return caracter


    End Function

    Public Function DesEncriptarCadena(ByVal cadena As String) As String

        Dim idx As Integer
        Dim result As String

        For idx = 0 To cadena.Length - 1
            result += DesEncriptarCaracter(cadena.Substring(idx, 1), cadena.Length, idx)
        Next
        Return result
    End Function

    Private Function DesEncriptarCaracter(ByVal caracter As String, _
    ByVal variable As Integer, _
    ByVal a_indice As Integer) As String

        Dim indice As Integer

        If Patron_encripta.IndexOf(caracter) <> -1 Then
            If (Patron_encripta.IndexOf(caracter) - variable - a_indice) > 0 Then
                indice = (Patron_encripta.IndexOf(caracter) - variable - a_indice) Mod Patron_encripta.Length
            Else
                'La línea está cortada por falta de espacio
                indice = (patron_busqueda.Length) + ((Patron_encripta.IndexOf(caracter) - variable - a_indice) Mod Patron_encripta.Length)
            End If
            indice = indice Mod Patron_encripta.Length
            Return patron_busqueda.Substring(indice, 1)
        Else
            Return caracter
        End If

    End Function
End Class
