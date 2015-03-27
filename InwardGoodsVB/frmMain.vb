Public Class frmMain
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        End
    End Sub

    Private Sub NewRecieptToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewRecieptToolStripMenuItem.Click
        Dim frmStrtPO As New frmStartPO
        frmStrtPO.Show()
        frmStrtPO.MdiParent = Me
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tslUser.Text = modFunctions.GetActiveDirUserDetails(Environment.UserName)
    End Sub

    Private Sub ProposeRecipetToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ProposeRecipetToolStripMenuItem.Click
        frmPost2M3.Show()
    End Sub

    Private Sub TestToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TestToolStripMenuItem.Click
        frmTestWS.Show()
    End Sub
End Class