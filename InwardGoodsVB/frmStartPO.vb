Imports System.Data.SqlClient

Public Class frmStartPO
    Private frmPurchase As frmPO
    Dim M3Svr As M3Point.M3Point = New M3Point.M3Point

    'variable used for passing purchase order number to the PO form

    Private Sub frmStartPO_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            Me.Hide()
        End If

    End Sub

    Private Sub GetPO()
        'variables used to retriving data from Purchase orders out of M3
        Dim connectionString As String
        Dim connection As SqlConnection
        Dim command As SqlCommand
        Dim adapter As New SqlDataAdapter
        Dim ds As New DataSet
        Dim ds2 As New DataSet
        Dim sql As String

        'sql to retrieve purchase orders from M3 


        connectionString = M3Svr.ConnString(frmMain.grid)
        sql = "SELECT IAPUST, IAPUSL, IDCSCD, IADIVI " & _
              "FROM MPHEAD, CIDMAS " & _
              "WHERE IASUNO = IDSUNO AND IAPUNO = '" & Trim(txtPONumber.Text) & "'"

        connection = New SqlConnection(connectionString)

        Try
            'Open the database, fill recordset and close
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds)
            adapter.Dispose()
            command.Dispose()
            connection.Close()

            'test if the Purchase Order Number exists, if it does run the PO form otherwise look for an RO number or container number
            If ds.Tables(0).Rows.Count <> 0 Then
                If ds.Tables(0).Rows(0)("IADIVI") = "100" Then
                    If Trim(ds.Tables(0).Rows(0)("IDCSCD")) = "AU" Then
                        If ds.Tables(0).Rows(0)("IAPUST") >= "20" Then
                            If ds.Tables(0).Rows(0)("IAPUSL") < "50" Then
                                Me.Hide()
                                frmPurchase = New frmPO(txtPONumber.Text, 0, "PO")
                                frmPurchase.MdiParent = Me.ParentForm
                                frmPurchase.Show()
                                Me.Close()
                            Else
                                MessageBox.Show("The number you have entered has been fully receipted", "Fully Receipted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                txtPONumber.Focus()
                                txtPONumber.Text = ""
                            End If
                        Else
                            MessageBox.Show("This po has not been printed, contact help desk", "Not Printed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            txtPONumber.Focus()
                            txtPONumber.Text = ""
                        End If
                    Else
                        MessageBox.Show("Imports PO, must be receipted via Container Number", "Imports PO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        txtPONumber.Focus()
                        txtPONumber.Text = ""
                    End If
                ElseIf ds.Tables(0).Rows(0)("IADIVI") = "200" Then
                    If Trim(ds.Tables(0).Rows(0)("IDCSCD")) = "NZ" Then
                        If ds.Tables(0).Rows(0)("IAPUST") >= "20" Then
                            If ds.Tables(0).Rows(0)("IAPUSL") < "50" Then
                                Me.Hide()
                                frmPurchase = New frmPO(txtPONumber.Text, 0, "PO")
                                frmPurchase.MdiParent = Me.ParentForm
                                frmPurchase.Show()
                                Me.Close()
                            Else
                                MessageBox.Show("The number you have entered has been fully receipted", "Fully Receipted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                txtPONumber.Focus()
                                txtPONumber.Text = ""
                            End If
                        Else
                            MessageBox.Show("This po has not been printed, contact help desk", "Not Printed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            txtPONumber.Focus()
                            txtPONumber.Text = ""
                        End If
                    Else
                        MessageBox.Show("Imports PO, must be receipted via Container Number", "Imports PO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        txtPONumber.Focus()
                        txtPONumber.Text = ""
                    End If
                End If
            Else
                'sql to retrieve Requisition orders from M3
                sql = "SELECT * FROM MGHEAD where MGTRNR = '" & Trim(txtPONumber.Text) & "'"

                ' open the database, fill recordset and close
                ds.Tables.Clear()
                connection = New SqlConnection(connectionString)
                connection.Open()
                command = New SqlCommand(sql, connection)
                adapter.SelectCommand = command
                adapter.Fill(ds)
                adapter.Dispose()
                command.Dispose()
                connection.Close()

                If ds.Tables(0).Rows.Count <> 0 Then
                    If Convert.ToInt32(ds.Tables(0).Rows(0)("MGTRSL")) >= 40 Then
                        If Convert.ToInt32(ds.Tables(0).Rows(0)("MGTRSL")) < 99 Then
                            Me.Hide()
                            frmPurchase = New frmPO(txtPONumber.Text, 0, "DO")
                            frmPurchase.MdiParent = Me.ParentForm
                            frmPurchase.Show()
                            Me.Close()
                        Else
                            MessageBox.Show("The number you have entered has been fully receipted", "Fully Receipted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            txtPONumber.Text = ""
                            txtPONumber.Focus()
                        End If
                    Else
                        MessageBox.Show("This number has not been advised for shipment, contact help desk", "Not advised for shipment", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        txtPONumber.Focus()
                        txtPONumber.Text = ""
                    End If
                Else
                    sql = "SELECT * FROM PPTRNS where IPPACN = '" & Trim(txtPONumber.Text) & "' ORDER BY IPSUDO DESC;"

                    ' open the database, fill recordset and close
                    ds.Tables.Clear()
                    connection = New SqlConnection(connectionString)
                    connection.Open()
                    command = New SqlCommand(sql, connection)
                    adapter.SelectCommand = command
                    adapter.Fill(ds)
                    adapter.Dispose()
                    command.Dispose()
                    connection.Close()

                    'test if the sql query returned any records, if it returned none then the PO, Do or Cont number entered isn't a valid number
                    If ds.Tables(0).Rows.Count <> 0 Then
                        If ds.Tables(0).Rows(0)("IPPUSL") < "50" Then
                            If ds.Tables(0).Rows(0)("IPPUST") >= "45" Then
                                Me.Hide()
                                frmPurchase = New frmPO(txtPONumber.Text, 0, "CO")
                                frmPurchase.MdiParent = Me.ParentForm
                                frmPurchase.Show()
                                Me.Close()
                            Else
                                MessageBox.Show("This number has not been advised for shipment, contact help desk", "Not advised for shipment", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                txtPONumber.Focus()
                                txtPONumber.Text = ""
                            End If
                        ElseIf ds.Tables(0).Rows(0)("IPPUSL") = "50" Then
                            MessageBox.Show("The number you have entered has been fully receipted.", "Fully Receipted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            txtPONumber.Focus()
                            txtPONumber.Text = ""
                        Else
                            MessageBox.Show("The number you have entered is not a valid PO, DO or Container Number, please check the number and try again", "Not advised for shipment", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            txtPONumber.Focus()
                            txtPONumber.Text = ""
                        End If
                    Else
                        If Not txtPONumber.Text = "" Then
                            MessageBox.Show("The number you have entered is not a valid PO, DO or Container Number, please check the number and try again", "Incorrect Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            txtPONumber.Focus()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub


    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        'run the GetPO routine when the submit button is clicked
        GetPO()
    End Sub


    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        'Close the form when the cancel button is clicked
        Me.Close()
    End Sub

    Private Sub txtPONumber_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtPONumber.KeyPress
        'run the GetPO routine when the use hits 'enter' when in the input textbox
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Return) Then
            GetPO()
        End If
    End Sub
End Class