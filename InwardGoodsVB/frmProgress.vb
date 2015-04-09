Public Class frmProgress
    Private _max As String
    Private _text As Integer

    Private Sub frmProgress_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        prgProgress.Minimum = 0
    End Sub

    Public Sub ProgressSetup(ByRef Maximum As Integer)
        prgProgress.Maximum = Maximum
        prgProgress.Value = 0
        Me.Show()
    End Sub

    Public Sub IncProg(ByRef text As String)

        prgProgress.Value = prgProgress.Value + 1
        lblStatus.Text = text

        Dim y = Me.lblStatus.Location.Y
        Dim x = ((Me.Width / 2) - (lblStatus.Width / 2)) - 10
        lblStatus.Location = New Point(x, y)

        Application.DoEvents()

        If prgProgress.Value = prgProgress.Maximum Then
            'MessageBox.Show("I am about to close")
            Me.Close()
        End If

    End Sub
End Class