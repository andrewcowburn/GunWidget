<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAttributeEntry
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
        Me.dgvAttributeEntry = New System.Windows.Forms.DataGridView()
        Me.btnConfirm = New System.Windows.Forms.Button()
        Me.cboSuppPacks = New System.Windows.Forms.ComboBox()
        Me.lblSuppPackNo = New System.Windows.Forms.Label()
        Me.lblProduct = New System.Windows.Forms.Label()
        Me.lblPacks = New System.Windows.Forms.Label()
        Me.lblEnter = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        CType(Me.dgvAttributeEntry, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvAttributeEntry
        '
        Me.dgvAttributeEntry.AllowUserToAddRows = False
        Me.dgvAttributeEntry.AllowUserToDeleteRows = False
        Me.dgvAttributeEntry.AllowUserToResizeColumns = False
        Me.dgvAttributeEntry.AllowUserToResizeRows = False
        Me.dgvAttributeEntry.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvAttributeEntry.Location = New System.Drawing.Point(31, 115)
        Me.dgvAttributeEntry.Name = "dgvAttributeEntry"
        Me.dgvAttributeEntry.Size = New System.Drawing.Size(257, 503)
        Me.dgvAttributeEntry.TabIndex = 0
        '
        'btnConfirm
        '
        Me.btnConfirm.Location = New System.Drawing.Point(210, 624)
        Me.btnConfirm.Name = "btnConfirm"
        Me.btnConfirm.Size = New System.Drawing.Size(78, 25)
        Me.btnConfirm.TabIndex = 1
        Me.btnConfirm.Text = "Confirm Entry"
        Me.btnConfirm.UseVisualStyleBackColor = True
        '
        'cboSuppPacks
        '
        Me.cboSuppPacks.FormattingEnabled = True
        Me.cboSuppPacks.Location = New System.Drawing.Point(163, 83)
        Me.cboSuppPacks.Name = "cboSuppPacks"
        Me.cboSuppPacks.Size = New System.Drawing.Size(125, 21)
        Me.cboSuppPacks.TabIndex = 2
        Me.cboSuppPacks.Visible = False
        '
        'lblSuppPackNo
        '
        Me.lblSuppPackNo.AutoSize = True
        Me.lblSuppPackNo.Location = New System.Drawing.Point(34, 86)
        Me.lblSuppPackNo.Name = "lblSuppPackNo"
        Me.lblSuppPackNo.Size = New System.Drawing.Size(116, 13)
        Me.lblSuppPackNo.TabIndex = 3
        Me.lblSuppPackNo.Text = "Supplier Pack Number "
        Me.lblSuppPackNo.Visible = False
        '
        'lblProduct
        '
        Me.lblProduct.AutoSize = True
        Me.lblProduct.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProduct.Location = New System.Drawing.Point(12, 21)
        Me.lblProduct.Name = "lblProduct"
        Me.lblProduct.Size = New System.Drawing.Size(276, 24)
        Me.lblProduct.TabIndex = 5
        Me.lblProduct.Text = "XXXXXXXXXXXXXXXXXXX"
        Me.lblProduct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPacks
        '
        Me.lblPacks.AutoSize = True
        Me.lblPacks.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPacks.Location = New System.Drawing.Point(113, 54)
        Me.lblPacks.Name = "lblPacks"
        Me.lblPacks.Size = New System.Drawing.Size(79, 20)
        Me.lblPacks.TabIndex = 6
        Me.lblPacks.Text = "xxxxxxxxxx"
        '
        'lblEnter
        '
        Me.lblEnter.AutoSize = True
        Me.lblEnter.Location = New System.Drawing.Point(30, 86)
        Me.lblEnter.Name = "lblEnter"
        Me.lblEnter.Size = New System.Drawing.Size(122, 13)
        Me.lblEnter.TabIndex = 4
        Me.lblEnter.Text = "Enter details of the pack"
        Me.lblEnter.Visible = False
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(126, 624)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(78, 25)
        Me.btnCancel.TabIndex = 7
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        Me.btnCancel.Visible = False
        '
        'frmAttributeEntry
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(320, 658)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.lblPacks)
        Me.Controls.Add(Me.lblProduct)
        Me.Controls.Add(Me.lblEnter)
        Me.Controls.Add(Me.lblSuppPackNo)
        Me.Controls.Add(Me.cboSuppPacks)
        Me.Controls.Add(Me.btnConfirm)
        Me.Controls.Add(Me.dgvAttributeEntry)
        Me.Name = "frmAttributeEntry"
        Me.Text = "Attribute Entry"
        CType(Me.dgvAttributeEntry, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dgvAttributeEntry As System.Windows.Forms.DataGridView
    Friend WithEvents btnConfirm As System.Windows.Forms.Button
    Friend WithEvents cboSuppPacks As System.Windows.Forms.ComboBox
    Friend WithEvents lblSuppPackNo As System.Windows.Forms.Label
    Friend WithEvents lblProduct As System.Windows.Forms.Label
    Friend WithEvents lblPacks As System.Windows.Forms.Label
    Friend WithEvents lblEnter As System.Windows.Forms.Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
