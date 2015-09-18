Imports System.Windows.Forms

Public Class frmInput
    Dim fAtt As frmAttributeEntry

    Protected m_BlankValid As Boolean = True
    Protected m_ReturnText As String = ""
    Protected m_isDate As Boolean = False
    Dim isDate As Boolean

    Public Overloads Function ShowDialog(ByVal TitleText As String, ByVal PromptText As String, ByVal DefaulText As String, ByRef EnteredText As String, ByVal BlankValid As Boolean, ByVal isDate As Boolean)
        m_BlankValid = BlankValid
        m_isDate = isDate

        If isDate Then
            txtInput.Visible = False
            dtpDate.Visible = True
            Me.ActiveControl = Me.dtpDate
        Else
            dtpDate.Visible = False
            txtInput.Visible = True
            Me.ActiveControl = Me.txtInput
        End If

        Me.lblPrompt.Text = PromptText
        Me.Text = TitleText
        Me.txtInput.Text = DefaulText
        Me.ShowDialog()
        EnteredText = m_ReturnText
        Return Me.DialogResult
    End Function

    Private Sub txtInput_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtInput.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Return) Then
            Confirm()
        End If
    End Sub

    Private Sub txtInput_TextChanged(sender As Object, e As EventArgs) Handles txtInput.TextChanged
        If Me.txtInput.Text = "" Then
            Me.btnConfirm.Enabled = m_BlankValid
        Else
            Me.btnConfirm.Enabled = True
        End If
    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) Handles btnConfirm.Click
        Confirm()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        m_ReturnText = ""
        Me.Close()

    End Sub

    Private Sub dtpDate_TextChanged(sender As Object, e As EventArgs) Handles dtpDate.TextChanged
        If Me.dtpDate.Text = Today Then
            Me.btnConfirm.Enabled = m_BlankValid
        Else
            Me.btnConfirm.Enabled = True
        End If
    End Sub

    Private Function Confirm()
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        If Me.dtpDate.Visible = True Then
            Dim batchdate As Date = dtpDate.Text
            m_ReturnText = batchdate.ToShortDateString
        Else
            m_ReturnText = Me.txtInput.Text
        End If

        Me.Close()
    End Function
End Class