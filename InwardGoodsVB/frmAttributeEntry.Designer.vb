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
        Me.lblEnter = New System.Windows.Forms.Label()
        Me.lblProduct = New System.Windows.Forms.Label()
        Me.lblPacks = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.dgvAttributeEntry, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvAttributeEntry
        '
        Me.dgvAttributeEntry.AllowUserToAddRows = False
        Me.dgvAttributeEntry.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvAttributeEntry.Location = New System.Drawing.Point(24, 146)
        Me.dgvAttributeEntry.Name = "dgvAttributeEntry"
        Me.dgvAttributeEntry.Size = New System.Drawing.Size(257, 503)
        Me.dgvAttributeEntry.TabIndex = 0
        '
        'btnConfirm
        '
        Me.btnConfirm.Location = New System.Drawing.Point(24, 115)
        Me.btnConfirm.Name = "btnConfirm"
        Me.btnConfirm.Size = New System.Drawing.Size(78, 25)
        Me.btnConfirm.TabIndex = 1
        Me.btnConfirm.Text = "Confirm Entry"
        Me.btnConfirm.UseVisualStyleBackColor = True
        '
        'cboSuppPacks
        '
        Me.cboSuppPacks.FormattingEnabled = True
        Me.cboSuppPacks.Location = New System.Drawing.Point(156, 88)
        Me.cboSuppPacks.Name = "cboSuppPacks"
        Me.cboSuppPacks.Size = New System.Drawing.Size(125, 21)
        Me.cboSuppPacks.TabIndex = 2
        Me.cboSuppPacks.Visible = False
        '
        'lblSuppPackNo
        '
        Me.lblSuppPackNo.AutoSize = True
        Me.lblSuppPackNo.Location = New System.Drawing.Point(21, 91)
        Me.lblSuppPackNo.Name = "lblSuppPackNo"
        Me.lblSuppPackNo.Size = New System.Drawing.Size(116, 13)
        Me.lblSuppPackNo.TabIndex = 3
        Me.lblSuppPackNo.Text = "Supplier Pack Number "
        Me.lblSuppPackNo.Visible = False
        '
        'lblEnter
        '
        Me.lblEnter.AutoSize = True
        Me.lblEnter.Location = New System.Drawing.Point(21, 91)
        Me.lblEnter.Name = "lblEnter"
        Me.lblEnter.Size = New System.Drawing.Size(122, 13)
        Me.lblEnter.TabIndex = 4
        Me.lblEnter.Text = "Enter details of the pack"
        Me.lblEnter.Visible = False
        '
        'lblProduct
        '
        Me.lblProduct.AutoSize = True
        Me.lblProduct.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProduct.Location = New System.Drawing.Point(62, 20)
        Me.lblProduct.Name = "lblProduct"
        Me.lblProduct.Size = New System.Drawing.Size(178, 24)
        Me.lblProduct.TabIndex = 5
        Me.lblProduct.Text = "XXXXXXXXXXXX"
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
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(437, 352)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(39, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Label1"
        '
        'frmAttributeEntry
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(865, 672)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblPacks)
        Me.Controls.Add(Me.lblProduct)
        Me.Controls.Add(Me.lblEnter)
        Me.Controls.Add(Me.lblSuppPackNo)
        Me.Controls.Add(Me.cboSuppPacks)
        Me.Controls.Add(Me.btnConfirm)
        Me.Controls.Add(Me.dgvAttributeEntry)
        Me.Name = "frmAttributeEntry"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Attribute Entry"
        CType(Me.dgvAttributeEntry, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dgvAttributeEntry As System.Windows.Forms.DataGridView
    Friend WithEvents btnConfirm As System.Windows.Forms.Button
    Friend WithEvents cboSuppPacks As System.Windows.Forms.ComboBox
    Friend WithEvents lblSuppPackNo As System.Windows.Forms.Label
    Friend WithEvents lblEnter As System.Windows.Forms.Label
    Friend WithEvents lblProduct As System.Windows.Forms.Label
    Friend WithEvents lblPacks As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
