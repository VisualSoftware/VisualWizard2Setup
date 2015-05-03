'============================================================================
'
'    VisualWizard2Setup
'    Copyright (C) 2012 - 2015 Visual Software Corporation
'
'    Author: ASV93
'    File: UpdateReminder.vb
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

Public Class UpdateReminder

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            Button1.Enabled = False
            ComboBox1.Enabled = True
        Else
            Button1.Enabled = True
            ComboBox1.Enabled = False
        End If
    End Sub

    Private Sub UpdateReminder_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If IO.File.Exists(My.Application.Info.DirectoryPath & "\RML.dat") = True Then
            IO.File.Delete(My.Application.Info.DirectoryPath & "\RML.dat")
        Else

        End If
        If CheckBox1.Checked = True Then
            'REMIND ME LATER (RML)
            If ComboBox1.SelectedIndex = 0 Then
                '1 DAY
                IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\RML.dat", Now.AddDays(1))
            ElseIf ComboBox1.SelectedIndex = 1 Then
                '2 DAYS
                IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\RML.dat", Now.AddDays(2))
            ElseIf ComboBox1.SelectedIndex = 2 Then
                '5 DAYS
                IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\RML.dat", Now.AddDays(5))
            ElseIf ComboBox1.SelectedIndex = 3 Then
                '7 DAYS
                IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\RML.dat", Now.AddDays(7))
            ElseIf ComboBox1.SelectedIndex = 4 Then
                '15 DAYS
                IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\RML.dat", Now.AddDays(15))
            ElseIf ComboBox1.SelectedIndex = 5 Then
                '30 DAYS
                IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\RML.dat", Now.AddDays(30))
            End If
        Else

        End If
        End
    End Sub
End Class