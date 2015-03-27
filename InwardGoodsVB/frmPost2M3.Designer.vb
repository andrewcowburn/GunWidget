<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPost2M3
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
        Me.btnPost = New System.Windows.Forms.Button()
        Me.cboPO = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'btnPost
        '
        Me.btnPost.Location = New System.Drawing.Point(388, 349)
        Me.btnPost.Name = "btnPost"
        Me.btnPost.Size = New System.Drawing.Size(75, 23)
        Me.btnPost.TabIndex = 2
        Me.btnPost.Text = "Button1"
        Me.btnPost.UseVisualStyleBackColor = True
        '
        'cboPO
        '
        Me.cboPO.FormattingEnabled = True
        Me.cboPO.Location = New System.Drawing.Point(130, 41)
        Me.cboPO.Name = "cboPO"
        Me.cboPO.Size = New System.Drawing.Size(281, 21)
        Me.cboPO.TabIndex = 3
        '
        'frmPost2M3
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(605, 462)
        Me.Controls.Add(Me.cboPO)
        Me.Controls.Add(Me.btnPost)
        Me.Name = "frmPost2M3"
        Me.Text = "frmPost2M3"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnPost As System.Windows.Forms.Button
    Friend WithEvents cboPO As System.Windows.Forms.ComboBox
End Class
