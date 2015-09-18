Public Class frmMain
    Public grid = "PRD"
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

    Private Sub ClosePurchaseOrderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClosePurchaseOrderToolStripMenuItem.Click
        If ActiveMdiChild IsNot Nothing Then
            ActiveMdiChild.Close()
        End If
    End Sub

    Private Sub frmMain_MdiChildActivate(sender As Object, e As EventArgs) Handles Me.MdiChildActivate
        If ActiveMdiChild IsNot Nothing Then
            If Me.ActiveMdiChild.Name = "frmPO" Then
                ClosePurchaseOrderToolStripMenuItem.Enabled = True
            Else
                ClosePurchaseOrderToolStripMenuItem.Enabled = False
            End If
        Else
            ClosePurchaseOrderToolStripMenuItem.Enabled = False
        End If
    End Sub

    Private Sub RePrintLabelsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RePrintLabelsToolStripMenuItem.Click
        Dim frmRep As New frmReprint
        frmRep.Show()
        frmRep.MdiParent = Me
    End Sub
End Class