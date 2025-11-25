Imports System
Imports System.Windows.Forms
Imports GameFramework.Core

Namespace GameFramework
    Public Module Program
        <STAThread()>
        Public Sub Main()
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            
            Try
                Using game As New SampleGame()
                    game.Run()
                End Using
            Catch ex As Exception
                MessageBox.Show($"Game crashed: {ex.Message}", "Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Module
End Namespace