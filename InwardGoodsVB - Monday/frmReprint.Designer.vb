<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmReprint
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
        Me.lstReprint = New System.Windows.Forms.ListBox()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lstReprint
        '
        Me.lstReprint.FormattingEnabled = True
        Me.lstReprint.Location = New System.Drawing.Point(63, 67)
        Me.lstReprint.Name = "lstReprint"
        Me.lstReprint.Size = New System.Drawing.Size(338, 342)
        Me.lstReprint.TabIndex = 0
        '
        'btnPrint
        '
        Me.btnPrint.Location = New System.Drawing.Point(314, 428)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(75, 23)
        Me.btnPrint.TabIndex = 1
        Me.btnPrint.Text = "Print"
        Me.btnPrint.UseVisualStyleBackColor = True
        '
        'frmReprint
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(493, 469)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.lstReprint)
        Me.Name = "frmReprint"
        Me.Text = "frmReprint"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lstReprint As System.Windows.Forms.ListBox
    Friend WithEvents btnPrint As System.Windows.Forms.Button
End Class
