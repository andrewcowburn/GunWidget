﻿Imports System.Data.SqlClient

Public Class frmStartPO
    Private frmPurchase As frmPO

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
        connectionString = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
        sql = "SELECT * FROM MPHEAD where IAPUNO = '" & Trim(txtPONumber.Text) & "'"
        connection = New SqlConnection(connectionString)

        Try
            ' Open the database, fill recordset and close
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds)
            adapter.Dispose()
            command.Dispose()
            connection.Close()

            'test if the Purchase Order Number exists, if it does run the PO form otherwise look for an RO number or container number
            If ds.Tables(0).Rows.Count <> 0 Then
                Me.Hide()
                frmPurchase = New frmPO(txtPONumber.Text, 0, "PO")
                frmPurchase.Show()
                frmPurchase.MdiParent = frmMain
                Me.Close()
            Else
                'sql to retrieve Requisition orders from M3
                sql = "SELECT * FROM MGHEAD where MGTRNR = '" & Trim(txtPONumber.Text) & "'"

                ' open the database, fill recordset and close
                connection = New SqlConnection(connectionString)
                connection.Open()
                command = New SqlCommand(sql, connection)
                adapter.SelectCommand = command
                adapter.Fill(ds2)
                adapter.Dispose()
                command.Dispose()
                connection.Close()

                'test if the sql query returned any records, if it returned none then the PO, Do or Cont number entered isn't a valid number
                If ds2.Tables(0).Rows.Count <> 0 Then
                    Me.Hide()
                    frmPurchase = New frmPO(txtPONumber.Text, 0, "DO")
                    frmPurchase.Show()
                    frmPurchase.MdiParent = frmMain
                    Me.Close()
                Else
                    If Not txtPONumber.Text = "" Then
                        MessageBox.Show("The number you have entered is not a valid PO, DO or Container Number, please check the number and try again", "Incorrect Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        txtPONumber.Focus()
                    End If
                End If

            End If

        Catch ex As Exception
            MessageBox.Show("Connection to the M3 database is unavailable. Please contact Help Desk.", "Connection Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Error)
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