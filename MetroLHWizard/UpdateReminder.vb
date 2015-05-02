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