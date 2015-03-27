Imports System.Data.SqlClient
Public Class frmAttributeEdit
    Dim connectionString As String
    Dim sql As String
    Dim connection As SqlConnection
    Dim command As SqlCommand
    Dim adapter As New SqlDataAdapter
    Dim dsAttributeRun As New DataSet
    Private frmAttEnt As frmAttributeEntry
    Dim _PO_ID As String

    'define public propertys to be passed by parent form
    Public Property PO_ID() As String
        Get
            Return _PO_ID
        End Get
        Set(value As String)
            _PO_ID = value
        End Set
    End Property

    Public Sub New(ByVal PO_ID As String)
        InitializeComponent()
        Me.PO_ID = PO_ID

        'select data from gunnersen to build a list of the runs associated with each line item
        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
        sql = "SELECT * FROM LineReceiving WHERE PO_ID= '" & Me.PO_ID & "';"
        RunSQL(connectionString, sql, "Run List")

        With dgvAttributeRuns
            .DataSource = dsAttributeRun.Tables("Run List")
            .Columns("ID").Visible = False
            .Columns("PACK_QTY_TOTAL").Visible = False
            .Columns("PACK_QTY_RECEIVED").Visible = False
            .Columns("PO_ID").Visible = False
            .Columns("RUN_NO").Width = 70
            .Columns("RUN_DATETIME").Width = 140
            .Columns("RUN_NO").HeaderText = "Run Number"
            .Columns("RUN_DATETIME").HeaderText = "Run Created"
        End With

        Dim dgvWidth As Integer = 43

        For i = 0 To dgvAttributeRuns.Columns.Count - 1
            If dgvAttributeRuns.Columns(i).Visible = True Then
                dgvWidth = dgvWidth + dgvAttributeRuns.Columns(i).Width
            End If
        Next
        dgvAttributeRuns.Width = dgvWidth
    End Sub


    Public Function RunSQL(ByVal conString As String, ByVal sql As String, tableName As String)
        connection = New SqlConnection(conString)
        Try
            connection.Open()
            Command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(dsAttributeRun.Tables.Add(tableName))
            adapter.Dispose()
            Command.Dispose()
            connection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Function

    Private Sub dgvAttributeRuns_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvAttributeRuns.CellContentClick

    End Sub


    Private Sub dgvAttributeRuns_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvAttributeRuns.CellDoubleClick

        'select attribute table for the run and send it to the attribute entry screen for editing
        If e.RowIndex <> "-1" Then
            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
            sql = "SELECT * FROM Attributes WHERE LINE_ID= '" & Me.PO_ID & dsAttributeRun.Tables("Run List").Rows(e.RowIndex)("RUN_NO") & "';"
            RunSQL(connectionString, sql, "Attribute Table")

            'frmAttEnt = New frmAttributeEntry(dsAttributeRun.Tables("Attribute Table"), Me.PO_ID & dsAttributeRun.Tables("Run List").Rows(e.RowIndex)("RUN_NO"), "edit012345", True, 3, 1, 1, 1, 1, 1)
            frmAttEnt.Show()
            frmAttEnt.MdiParent = frmMain
            Me.Visible = False
        End If

    End Sub
End Class