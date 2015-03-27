<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLineReceiving
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
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblPO = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblProductDesc = New System.Windows.Forms.Label()
        Me.lblSupProductID = New System.Windows.Forms.Label()
        Me.lblProductID = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblQtyRec = New System.Windows.Forms.Label()
        Me.lblQtyTotal = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblQtyUOM = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblQtyOut = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.lblPriceUOM = New System.Windows.Forms.Label()
        Me.btnConfirm = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.lblPacksRec = New System.Windows.Forms.Label()
        Me.txtPacksQty = New System.Windows.Forms.TextBox()
        Me.txtPacksRec = New System.Windows.Forms.TextBox()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.btnEdit = New System.Windows.Forms.Button()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.lblQtySaved = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(23, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(192, 31)
        Me.lblTitle.TabIndex = 1
        Me.lblTitle.Text = "Line Receiving"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(26, 105)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(72, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Product ID:"
        '
        'lblPO
        '
        Me.lblPO.AutoSize = True
        Me.lblPO.Location = New System.Drawing.Point(185, 60)
        Me.lblPO.Name = "lblPO"
        Me.lblPO.Size = New System.Drawing.Size(42, 13)
        Me.lblPO.TabIndex = 3
        Me.lblPO.Text = "XXXXX"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(25, 60)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(99, 13)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Purchase Order:"
        '
        'lblProductDesc
        '
        Me.lblProductDesc.AutoSize = True
        Me.lblProductDesc.Location = New System.Drawing.Point(185, 151)
        Me.lblProductDesc.Name = "lblProductDesc"
        Me.lblProductDesc.Size = New System.Drawing.Size(49, 13)
        Me.lblProductDesc.TabIndex = 8
        Me.lblProductDesc.Text = "XXXXXX"
        '
        'lblSupProductID
        '
        Me.lblSupProductID.AutoSize = True
        Me.lblSupProductID.Location = New System.Drawing.Point(185, 127)
        Me.lblSupProductID.Name = "lblSupProductID"
        Me.lblSupProductID.Size = New System.Drawing.Size(49, 13)
        Me.lblSupProductID.TabIndex = 9
        Me.lblSupProductID.Text = "XXXXXX"
        '
        'lblProductID
        '
        Me.lblProductID.AutoSize = True
        Me.lblProductID.Location = New System.Drawing.Point(185, 105)
        Me.lblProductID.Name = "lblProductID"
        Me.lblProductID.Size = New System.Drawing.Size(49, 13)
        Me.lblProductID.TabIndex = 10
        Me.lblProductID.Text = "XXXXXX"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(26, 151)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(123, 13)
        Me.Label8.TabIndex = 11
        Me.Label8.Text = "Product Description:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(26, 127)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(122, 13)
        Me.Label9.TabIndex = 12
        Me.Label9.Text = "Supplier Product ID:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(26, 215)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(88, 13)
        Me.Label2.TabIndex = 17
        Me.Label2.Text = "Qty Received:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(26, 191)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(59, 13)
        Me.Label3.TabIndex = 16
        Me.Label3.Text = "Qty Total"
        '
        'lblQtyRec
        '
        Me.lblQtyRec.AutoSize = True
        Me.lblQtyRec.Location = New System.Drawing.Point(184, 215)
        Me.lblQtyRec.Name = "lblQtyRec"
        Me.lblQtyRec.Size = New System.Drawing.Size(49, 13)
        Me.lblQtyRec.TabIndex = 15
        Me.lblQtyRec.Text = "XXXXXX"
        '
        'lblQtyTotal
        '
        Me.lblQtyTotal.AutoSize = True
        Me.lblQtyTotal.Location = New System.Drawing.Point(185, 191)
        Me.lblQtyTotal.Name = "lblQtyTotal"
        Me.lblQtyTotal.Size = New System.Drawing.Size(49, 13)
        Me.lblQtyTotal.TabIndex = 14
        Me.lblQtyTotal.Text = "XXXXXX"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(26, 312)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(124, 13)
        Me.Label7.TabIndex = 19
        Me.Label7.Text = "Qty Unit of Measure:"
        '
        'lblQtyUOM
        '
        Me.lblQtyUOM.AutoSize = True
        Me.lblQtyUOM.Location = New System.Drawing.Point(185, 312)
        Me.lblQtyUOM.Name = "lblQtyUOM"
        Me.lblQtyUOM.Size = New System.Drawing.Size(49, 13)
        Me.lblQtyUOM.TabIndex = 18
        Me.lblQtyUOM.Text = "XXXXXX"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(26, 241)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(102, 13)
        Me.Label5.TabIndex = 20
        Me.Label5.Text = "Qty Outstanding:"
        '
        'lblQtyOut
        '
        Me.lblQtyOut.AutoSize = True
        Me.lblQtyOut.Location = New System.Drawing.Point(185, 241)
        Me.lblQtyOut.Name = "lblQtyOut"
        Me.lblQtyOut.Size = New System.Drawing.Size(49, 13)
        Me.lblQtyOut.TabIndex = 21
        Me.lblQtyOut.Text = "XXXXXX"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(26, 336)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(144, 13)
        Me.Label6.TabIndex = 23
        Me.Label6.Text = "Pricing Unit of Measure:"
        '
        'lblPriceUOM
        '
        Me.lblPriceUOM.AutoSize = True
        Me.lblPriceUOM.Location = New System.Drawing.Point(185, 336)
        Me.lblPriceUOM.Name = "lblPriceUOM"
        Me.lblPriceUOM.Size = New System.Drawing.Size(49, 13)
        Me.lblPriceUOM.TabIndex = 22
        Me.lblPriceUOM.Text = "XXXXXX"
        '
        'btnConfirm
        '
        Me.btnConfirm.Location = New System.Drawing.Point(429, 241)
        Me.btnConfirm.Name = "btnConfirm"
        Me.btnConfirm.Size = New System.Drawing.Size(328, 40)
        Me.btnConfirm.TabIndex = 29
        Me.btnConfirm.Text = "Confirm"
        Me.btnConfirm.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(426, 113)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(77, 13)
        Me.Label10.TabIndex = 28
        Me.Label10.Text = "Recived Qty"
        '
        'lblPacksRec
        '
        Me.lblPacksRec.AutoSize = True
        Me.lblPacksRec.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPacksRec.Location = New System.Drawing.Point(426, 73)
        Me.lblPacksRec.Name = "lblPacksRec"
        Me.lblPacksRec.Size = New System.Drawing.Size(135, 13)
        Me.lblPacksRec.TabIndex = 27
        Me.lblPacksRec.Text = "No of Packs Received"
        '
        'txtPacksQty
        '
        Me.txtPacksQty.Location = New System.Drawing.Point(589, 113)
        Me.txtPacksQty.Name = "txtPacksQty"
        Me.txtPacksQty.Size = New System.Drawing.Size(168, 20)
        Me.txtPacksQty.TabIndex = 26
        '
        'txtPacksRec
        '
        Me.txtPacksRec.Location = New System.Drawing.Point(589, 73)
        Me.txtPacksRec.Name = "txtPacksRec"
        Me.txtPacksRec.Size = New System.Drawing.Size(168, 20)
        Me.txtPacksRec.TabIndex = 25
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(92, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'btnEdit
        '
        Me.btnEdit.Location = New System.Drawing.Point(429, 303)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(328, 43)
        Me.btnEdit.TabIndex = 30
        Me.btnEdit.Text = "Edit"
        Me.btnEdit.UseVisualStyleBackColor = True
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(26, 266)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(154, 13)
        Me.Label11.TabIndex = 32
        Me.Label11.Text = "Qty Saved for Receipting:"
        '
        'lblQtySaved
        '
        Me.lblQtySaved.AutoSize = True
        Me.lblQtySaved.Location = New System.Drawing.Point(185, 266)
        Me.lblQtySaved.Name = "lblQtySaved"
        Me.lblQtySaved.Size = New System.Drawing.Size(49, 13)
        Me.lblQtySaved.TabIndex = 31
        Me.lblQtySaved.Text = "XXXXXX"
        '
        'frmLineReceiving
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(783, 375)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.lblQtySaved)
        Me.Controls.Add(Me.btnEdit)
        Me.Controls.Add(Me.btnConfirm)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.lblPacksRec)
        Me.Controls.Add(Me.txtPacksQty)
        Me.Controls.Add(Me.txtPacksRec)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.lblPriceUOM)
        Me.Controls.Add(Me.lblQtyOut)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.lblQtyUOM)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lblQtyRec)
        Me.Controls.Add(Me.lblQtyTotal)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.lblProductID)
        Me.Controls.Add(Me.lblSupProductID)
        Me.Controls.Add(Me.lblProductDesc)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblPO)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTitle)
        Me.Name = "frmLineReceiving"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Line Receiving"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblPO As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblProductDesc As System.Windows.Forms.Label
    Friend WithEvents lblSupProductID As System.Windows.Forms.Label
    Friend WithEvents lblProductID As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblQtyRec As System.Windows.Forms.Label
    Friend WithEvents lblQtyTotal As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblQtyUOM As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblQtyOut As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lblPriceUOM As System.Windows.Forms.Label
    Friend WithEvents btnConfirm As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents lblPacksRec As System.Windows.Forms.Label
    Friend WithEvents txtPacksQty As System.Windows.Forms.TextBox
    Friend WithEvents txtPacksRec As System.Windows.Forms.TextBox
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnEdit As System.Windows.Forms.Button
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents lblQtySaved As System.Windows.Forms.Label
End Class
