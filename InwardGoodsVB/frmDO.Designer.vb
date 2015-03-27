<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDO
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblDO = New System.Windows.Forms.Label()
        Me.lblWarehouseTo = New System.Windows.Forms.Label()
        Me.lblDDNo = New System.Windows.Forms.Label()
        Me.lblWarehouseID = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblDO
        '
        Me.lblDO.AutoSize = True
        Me.lblDO.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDO.Location = New System.Drawing.Point(214, 77)
        Me.lblDO.Name = "lblDO"
        Me.lblDO.Size = New System.Drawing.Size(49, 13)
        Me.lblDO.TabIndex = 23
        Me.lblDO.Text = "XXXXXX"
        '
        'lblWarehouseTo
        '
        Me.lblWarehouseTo.AutoSize = True
        Me.lblWarehouseTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWarehouseTo.Location = New System.Drawing.Point(214, 104)
        Me.lblWarehouseTo.Name = "lblWarehouseTo"
        Me.lblWarehouseTo.Size = New System.Drawing.Size(49, 13)
        Me.lblWarehouseTo.TabIndex = 22
        Me.lblWarehouseTo.Text = "XXXXXX"
        '
        'lblDDNo
        '
        Me.lblDDNo.AutoSize = True
        Me.lblDDNo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDDNo.Location = New System.Drawing.Point(214, 157)
        Me.lblDDNo.Name = "lblDDNo"
        Me.lblDDNo.Size = New System.Drawing.Size(49, 13)
        Me.lblDDNo.TabIndex = 21
        Me.lblDDNo.Text = "XXXXXX"
        '
        'lblWarehouseID
        '
        Me.lblWarehouseID.AutoSize = True
        Me.lblWarehouseID.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWarehouseID.Location = New System.Drawing.Point(214, 130)
        Me.lblWarehouseID.Name = "lblWarehouseID"
        Me.lblWarehouseID.Size = New System.Drawing.Size(49, 13)
        Me.lblWarehouseID.TabIndex = 19
        Me.lblWarehouseID.Text = "XXXXXX"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(15, 104)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(94, 13)
        Me.Label6.TabIndex = 18
        Me.Label6.Text = "To Warehouse:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(15, 130)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(92, 13)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "Warehouse ID:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(15, 157)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(181, 13)
        Me.Label3.TabIndex = 15
        Me.Label3.Text = "Delivery Docket Number or ID:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(15, 77)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(153, 13)
        Me.Label1.TabIndex = 14
        Me.Label1.Text = "Distibution Order Number:"
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(12, 18)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(531, 31)
        Me.lblTitle.TabIndex = 13
        Me.lblTitle.Text = "Inward Good Processing for DO XXXXXXX"
        '
        'frmDO
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(781, 495)
        Me.Controls.Add(Me.lblDO)
        Me.Controls.Add(Me.lblWarehouseTo)
        Me.Controls.Add(Me.lblDDNo)
        Me.Controls.Add(Me.lblWarehouseID)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTitle)
        Me.Name = "frmDO"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmDO"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblDO As System.Windows.Forms.Label
    Friend WithEvents lblWarehouseTo As System.Windows.Forms.Label
    Friend WithEvents lblDDNo As System.Windows.Forms.Label
    Friend WithEvents lblWarehouseID As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
End Class
