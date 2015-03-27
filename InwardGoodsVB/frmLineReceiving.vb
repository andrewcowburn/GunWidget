Imports System.Data.SqlClient
Imports Lawson.M3.MvxSock

Public Class frmLineReceiving
    Private _lineNumber As String
    Private _po As String
    Private _subLine As String
    Private _user As String
    Dim sql As String
    Dim connectionstring As String
    Dim connection As SqlConnection
    Dim command As SqlCommand
    Dim adapter As New SqlDataAdapter
    Dim dsLine As New DataSet
    Dim attributeValue As String
    Dim lotControl As Integer = 0
    Public attributeTable As DataTable = New DataTable("Attribute Table")
    Dim strWrk, strWrk1, strWrk2 As String
    Private frmAttEnt As frmAttributeEntry
    Private frmEdtAtt As frmAttributeEdit
    Dim runNumber As Integer
    Dim supPackID As String


    Public Property po() As String
        Get
            Return _po
        End Get
        Set(value As String)
            _po = value
        End Set
    End Property

    Public Property subLine() As String
        Get
            Return _subLine
        End Get
        Set(value As String)
            _subLine = value
        End Set
    End Property

    Public Property lineNumber() As String
        Get
            Return _lineNumber
        End Get
        Set(value As String)
            _lineNumber = value
        End Set
    End Property

    Public Sub New(ByVal po As String, ByVal line As String, ByVal subLine As String)
        'Pass purchase order number, line number and sub line number from the Purchase Order form
        InitializeComponent()
        Me.lineNumber = line
        Me.subLine = subLine
        Me.po = po
        lblPO.Text = Me.po


        'Open M3 database and pass values for the Purchase Order Line to the form and set the relevant labels controls to these values.
        connectionstring = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
        sql = "SELECT MPLINE.IBPUNO, MPLINE.IBPNLI, MPLINE.IBPNLS, MPLINE.IBITNO, MITMAS.MMFUDS, MITMAS.MMITDS, MPLINE.IBWHLO, MPLINE.IBFACI, MPLINE.IBSITE, MPLINE.IBORQA, MPLINE.IBPUPR, MPLINE.IBPPUN, MPLINE.IBPUUN, MPLINE.IBRVQA, MITMAS.MMUNMS, MITMAS.MMALUC, MITMAS.MMSTUN " & _
                "FROM MPLINE LEFT JOIN MITMAS ON MPLINE.IBITNO = MITMAS.MMITNO " & _
                "WHERE (((MPLINE.IBPUNO)='" & Me.po & "') AND ((MPLINE.IBPNLI)=" & Me.lineNumber & ") AND ((MPLINE.IBPNLS)=" & Me.subLine & "));"
        RunSQL(connectionstring, sql, "POLine")

        'Set relevant labels with the data which has been pulled from M3
        lblProductID.Text = dsLine.Tables("POLine").Rows(0)("IBITNO")
        lblSupProductID.Text = dsLine.Tables("POLine").Rows(0)("IBSITE")
        lblProductDesc.Text = dsLine.Tables("POLine").Rows(0)("MMFUDS")
        lblQtyRec.Text = Math.Round(dsLine.Tables("POLine").Rows(0)("IBORQA"))
        lblQtyTotal.Text = Math.Round(dsLine.Tables("POLine").Rows(0)("IBRVQA"))
        lblQtyOut.Text = Math.Round((dsLine.Tables("POLine").Rows(0)("IBORQA")) - (dsLine.Tables(0).Rows(0)("IBRVQA")))
        lblQtyUOM.Text = dsLine.Tables("POLine").Rows(0)("IBPUUN")
        lblPriceUOM.Text = dsLine.Tables("POLine").Rows(0)("IBPPUN")


        'Open the Gunnersen database and chek the number of packs already saved, if this number is equal to the total number for that line then diable any further entries. 
        connectionstring = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
        sql = "SELECT ATTRIBUTE_VALUE, PO_ID FROM Attributes INNER JOIN LineReceiving ON Attributes.LINE_ID = LineReceiving.ID WHERE (((Attributes.ATTRIBUTE_VALUE)= 'SUPPPACK') AND ((LineReceiving.PO_ID)=" & Me.po & Me.lineNumber & Me.subLine & "));"
        RunSQL(connectionstring, sql, "QtySaved")

        lblQtySaved.Text = dsLine.Tables("QtySaved").Rows.Count
        If dsLine.Tables("QtySaved").Rows.Count >= Math.Round(dsLine.Tables("POLine").Rows(0)("IBRVQA")) Then
            btnConfirm.Enabled = False
            MessageBox.Show("Total number of lines saved reached, only editing allowed")
        End If
    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) Handles btnConfirm.Click
        supPackID = InputBox("Please enter the Supplier Pack Number.", "Supplier Pack Number")
        If supPackID <> "" Then
            connectionstring = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
            sql = "SELECT * FROM MITMAS WHERE MMITNO = '" & dsLine.Tables(0).Rows(0)("IBITNO") & "';"
            connection = New SqlConnection(connectionstring)
            RunSQL(connectionstring, sql, "Product Table")

            attributeValue = Trim(dsLine.Tables("Product Table").Rows(0)("MMATMO"))


            If Not attributeValue Is Nothing Or Trim(attributeValue) = "" Then
                If attributeValue = "MIXEDPACK" Then
                    lotControl = 1
                    Me.txtPacksQty.Text = 1
                    Me.txtPacksRec.Text = 1
                Else
                    lotControl = 2
                End If

                connectionstring = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
                sql = "SELECT AEATMO, AEATID, AEANSQ, AECOBT FROM MAMOLI WHERE AEATMO = '" & attributeValue & "';"
                RunSQL(connectionstring, sql, "Attribute Model")

                If dsLine.Tables("Attribute Model").Rows.Count = 0 Then
                    MessageBox.Show("Invalid Attribute Model - Contact Help Desk")
                End If

                If attributeTable.Columns.Count = 0 Then
                    With attributeTable
                        .Columns.Add("AEANSQ", Type.GetType("System.Double"))
                        .Columns.Add("AEATID", Type.GetType("System.String"))
                        .Columns.Add("AEATMO", Type.GetType("System.String"))
                        .Columns.Add("AEQTY", Type.GetType("System.String"))
                        .PrimaryKey = New DataColumn() {attributeTable.Columns("AEANSQ")}
                    End With
                End If

                connectionstring = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
                sql = "SELECT AEANSQ, AEATID, AEATMO FROM MAMOLI where AEATMO = '" & attributeValue & "' ORDER BY AEANSQ"
                RunSQL(connectionstring, sql, "LineAttributes")

                'iterate through the Attributes for that pack and set the qty to zero ready for updating to M3
                For i As Integer = 0 To dsLine.Tables("LineAttributes").Rows.Count - 1
                    attributeTable.Rows.Add(dsLine.Tables("LineAttributes").Rows(i)("AEANSQ"), dsLine.Tables("LineAttributes").Rows(i)("AEATID"), dsLine.Tables("LineAttributes").Rows(i)("AEATMO"), 0)
                Next

            End If

            Dim Server = "M3BE"
            Dim Port = 16205
            Dim UserID = "DTAMIGR"
            Dim PWD = "Q190E87AG"
            Dim APIName = "ATS101MI"
            Dim APIOpr = "LstAttrByRef"
            Dim sid As New SERVER_ID
            Dim rc

            rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, "")

            If rc <> 0 Then
                MvxSock.ShowLastError(sid, "Error Occured: ")
            End If

            MvxSock.SetField(sid, "ORCA", "251")
            MvxSock.SetField(sid, "RIDN", Me.po)
            MvxSock.SetField(sid, "RIDL", Me.lineNumber)
            MvxSock.SetField(sid, "RIDX", Me.subLine)

            rc = MvxSock.Access(sid, APIOpr)
            If rc <> 0 Then
                MvxSock.ShowLastError(sid, "Error Occured: ")
            End If

            strWrk = Trim(MvxSock.GetField(sid, "ATNR"))
            strWrk1 = Trim(MvxSock.GetField(sid, "ATID"))
            strWrk2 = Trim(MvxSock.GetField(sid, "ATVL"))

            For i As Integer = 0 To attributeTable.Rows.Count - 1
                If Trim(attributeTable.Rows(i).Field(Of String)(1)) = Trim(strWrk1) Then
                    attributeTable.Rows(i)(2) = Trim(strWrk)
                    attributeTable.Rows(i)(3) = Trim(strWrk2)
                End If
            Next

            While MvxSock.More(sid)
                MvxSock.Access(sid, Nothing)
                strWrk = Trim(MvxSock.GetField(sid, "ATNR"))
                strWrk1 = Trim(MvxSock.GetField(sid, "ATID"))
                strWrk2 = Trim(MvxSock.GetField(sid, "ATVL"))

                For i As Integer = 0 To attributeTable.Rows.Count - 1
                    If Trim(attributeTable.Rows(i).Field(Of String)(1)) = Trim(strWrk1) Then
                        attributeTable.Rows(i)(2) = Trim(strWrk)
                        attributeTable.Rows(i)(3) = Trim(strWrk2)
                    End If
                Next

            End While

            dsLine.Tables.Add(attributeTable)

            connectionstring = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
            sql = "SELECT * FROM LineReceiving WHERE PO_ID= '" & Me.po & lineNumber & subLine & "';"
            RunSQL(connectionstring, sql, "Run List")

            Dim max As Integer = dsLine.Tables("Run List").Rows.Count - 1
            If max = -1 Then
                runNumber = 0
            Else
                runNumber = dsLine.Tables("Run List").Rows.Count + 1
            End If
            connectionstring = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
            sql = "INSERT INTO dbo.LineReceiving(ID, PACK_QTY_TOTAL, PACK_QTY_RECEIVED, RUN_NO, PO_ID, RUN_DATETIME, PO, ROWNUMBER) Values ('" & po & lineNumber & subLine & runNumber & "','" & Me.txtPacksQty.Text & "','" & Me.txtPacksRec.Text & "','" & runNumber & "','" & po & lineNumber & subLine & "','" & DateTime.Now & po & "'," & Convert.ToInt32(subLine) & ")"
            connection = New SqlConnection(connectionstring)
            Try
                command = New SqlCommand(sql, connection)
                connection.Open()
                command.ExecuteNonQuery()
            Catch ex As Exception
                MessageBox.Show(ex.ToString)
            End Try

            'frmAttEnt = New frmAttributeEntry(Me.attributeTable, supPackID, False, ds2)
            frmAttEnt.Show()
            frmAttEnt.MdiParent = frmMain
            Me.Close()
        End If
    End Sub


    Public Function RunSQL(ByVal conString As String, ByVal sql As String, tableName As String)
        connection = New SqlConnection(conString)
        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(dsLine.Tables.Add(tableName))
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Function

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        frmEdtAtt = New frmAttributeEdit(po & lineNumber & subLine)
        frmEdtAtt.Show()
        frmEdtAtt.MdiParent = frmMain
    End Sub
End Class