Public Class frmAddRun
    Dim _dt As DataTable
    Private frmPurchaseOrder As frmPO
    Public Property dt As DataTable
        Get
            Return _dt
        End Get
        Set(value As DataTable)
            _dt = value
        End Set
    End Property

    Public Sub New(ByVal dt As DataTable)
        InitializeComponent()
        Me.dt = dt
        dt.Columns.Add("Qty")
        dgvItems.DataSource = dt
        With dgvItems
            .Columns("IBITNO").ReadOnly = True
            .Columns("MMFUDS").ReadOnly = True
            .Columns("IBPUNO").Visible = False
            .Columns("IBWHLO").Visible = False
            .Columns("IBFACI").Visible = False
            .Columns("IBSITE").Visible = False
            .Columns("IBPUPR").Visible = False
            .Columns("MMUNMS").Visible = False
            .Columns("MMALUC").Visible = False
            .Columns("MMSTUN").Visible = False
            .Columns("IBPNLS").Visible = False
            .Columns("IBPPUN").Visible = False
            .Columns("IBRVQA").Visible = False
            .Columns("Saved").Visible = False
            .Columns("IBITNO").HeaderCell.Value = "Product ID"
            .Columns("MMFUDS").HeaderCell.Value = "Descripton"
            .Columns("IBORQA").Visible = False
            .Columns("IBPNLI").Visible = False
            .Columns("IBPUUN").Visible = False
            .Columns("IBPNLS").Width = 80
            .Columns("IBITNO").Width = 135
            .Columns("MMFUDS").Width = 210
            .Columns("IBORQA").Width = 80
            .Columns("IBPNLI").Width = 40
            .Columns("IBPUUN").Width = 50
        End With
    End Sub

    Private Sub cmdNext_Click(sender As Object, e As EventArgs) Handles cmdNext.Click
        Dim totalRowsToBeAdded As Integer
        For i = 0 To dgvItems.Rows.Count - 1
            totalRowsToBeAdded = totalRowsToBeAdded + dgvItems.Rows(i).Cells("Qty").Value
        Next

        Dim goAhead = MessageBox.Show("Do you want to create a new Delivery docket with a total of " & totalRowsToBeAdded & " Items?", "Create New Run", MessageBoxButtons.YesNo)

        If goAhead = 6 Then
        Else

        End If



        'frmPurchaseOrder = New frmPO("1000009", dgvItems.DataSource)
        'frmPurchaseOrder.Show()
        'frmPurchaseOrder.MdiParent = frmMain
        'Me.Hide()

    End Sub
End Class