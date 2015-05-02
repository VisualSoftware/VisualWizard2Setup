Imports System.Reflection
Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports Microsoft.VisualBasic

Public Class CustomUI

    Private Sub CustomUI_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        If IO.File.Exists("Code.txt") Then
            IO.File.Delete("Code.txt")
        Else

        End If
        IO.File.WriteAllText("Code.txt", TextBox4.Text)
        ' Read code from file  
        Dim input = My.Computer.FileSystem.ReadAllText("Code.txt")

        ' Create "code" literal to pass to the compiler.  
        '  
        ' Notice the <% = input % > where the code read from the text file (Code.txt)   
        ' is inserted into the code fragment.  
        Dim code = <code>  
                       Imports System  
                       Imports System.Windows.Forms  
  
                       Public Class TempClass  
                           Public Sub UpdateText(ByVal txtOutput As TextBox)  
                               <%= input %>  
                           End Sub  
                       End Class  
                   </code>

        ' Create the VB.NET compiler.  
        Dim vbProv = New VBCodeProvider()
        ' Create parameters to pass to the compiler.  
        Dim vbParams = New CompilerParameters()
        ' Add referenced assemblies.  
        vbParams.ReferencedAssemblies.Add("mscorlib.dll")
        vbParams.ReferencedAssemblies.Add("System.dll")
        vbParams.ReferencedAssemblies.Add("System.Windows.Forms.dll")
        vbParams.GenerateExecutable = False
        ' Ensure we generate an assembly in memory and not as a physical file.  
        vbParams.GenerateInMemory = True

        ' Compile the code and get the compiler results (contains errors, etc.)  
        Dim compResults = vbProv.CompileAssemblyFromSource(vbParams, code.Value)

        ' Check for compile errors  
        If compResults.Errors.Count > 0 Then

            ' Show each error.  
            For Each er In compResults.Errors
                MessageBox.Show(er.ToString())
            Next

        Else
            Dim something
            something = "{Me." & TextBox1.Text & "}"
            ' Create instance of the temporary compiled class.  
            Dim obj As Object = compResults.CompiledAssembly.CreateInstance("TempClass")
            ' An array of object that represent the arguments to be passed to our method (UpdateText).  
            Dim args() As Object = {Me.TextBox1}
            ' Execute the method by passing the method name and arguments.  
            Dim t As Type = obj.GetType().InvokeMember("UpdateText", BindingFlags.InvokeMethod, Nothing, obj, args)

        End If

    End Sub
End Class