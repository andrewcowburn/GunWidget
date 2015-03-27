Imports System.Data.SqlClient

Public Class frmStartPO
    Private frmPurchase As frmPO 'variable used for passing purchase order number to the PO form

    Private Sub txtPONumber_LostFocus(sender As Object, e As EventArgs) Handles txtPONumber.LostFocus
        'variables used to retriving data from Purchase orders out of M3
        Dim connectionString As String
        Dim connection As SqlConnection
        Dim command As SqlCommand
        Dim adapter As New SqlDataAdapter
        Dim ds As New DataSet
        Dim ds2 As New DataSet
        Dim sql As String
        Dim blank As DataTable

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
                frmPurchase = New frmPO(txtPONumber.Text, 0)
                frmPurchase.Show()
                frmPurchase.MdiParent = frmMain
                Me.Close()
            Else
                If Not txtPONumber.Text = "" Then
                    MessageBox.Show("The number you have entered is not a valid PO, DO or Container Number, please check the number and try again", "Incorrect Entry", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    txtPONumber.Focus()
                End If

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
            End If

        Catch ex As Exception
            MsgBox("Can not open connection ! ")
        End Try


    End Sub

End Class