Imports System.Data.SqlClient

Public Class frmPost2M3
    Dim connectionString As String
    Dim sql As String
    Dim connection As SqlConnection
    Dim adapter As New SqlDataAdapter
    Dim command As SqlCommand
    Dim ds3 As New DataSet
    Dim noOfLines As String
    Dim dragbounds As System.Drawing.Rectangle
    Dim dragMethod As String

    Private Sub frmPosttoM3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
        sql = "SELECT DISTINCT PO FROM LineReceiving"

        connection = New SqlConnection(connectionString)

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds3.Tables.Add)
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        cboPO.DataSource = ds3.Tables(0)
        cboPO.ValueMember = "PO"
        cboPO.DisplayMember = "PO"


    End Sub
End Class