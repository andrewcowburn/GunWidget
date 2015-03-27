Imports System.Data.SqlClient
Imports System.DirectoryServices
Imports Lawson.M3.MvxSock
Imports PocketSOAP



Public Class frmPO
    Private _poNumber As String
    Private _lineloop As Integer
    Dim connectionString As String
    Dim connection As SqlConnection
    Dim command As SqlCommand
    Dim adapter As New SqlDataAdapter
    Dim ds As New DataSet
    Dim ds2 As New DataSet
    Dim dsline As New DataSet
    Dim dsBlank As New DataSet
    Dim dtBlank As New DataTable
    Dim sql As String
    Dim QtySaved As Integer
    Dim LineNo As String
    Dim subLineNo As String
    Private frmLneRec As frmLineReceiving
    Dim supPackID As String
    Dim attributeValue As String
    Dim lotControl As Integer = 0
    Public attributeTable As DataTable = New DataTable("Attribute Table")
    Dim strWrk, strWrk1, strWrk2 As String
    Dim runNumber As Integer
    Private frmAttEnt As frmAttributeEntry
    Private frmAddRn As frmAddRun
    Private _dt As DataTable
    Dim duplicatePacks As Integer
    Dim i As Integer
    Dim itemQty As Integer
    Dim packQty As Integer
    Dim qtyTotal As Integer
    Dim RUN_NUMBER As String

    Public Property poNumber() As String
        Get
            Return _poNumber
        End Get
        Set(value As String)
            _poNumber = value
        End Set
    End Property
    Public Property lineloop() As Integer
        Get
            Return _lineloop
        End Get
        Set(value As Integer)
            _lineloop = value
        End Set
    End Property

    Public Sub New(ByVal po As String, ByVal lineloop As Integer)
        InitializeComponent()
        Me.poNumber = po
        Me.lineloop = lineloop
    End Sub

    Private Sub frmPO_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblTitle.Text = "Inward Good Processing for PO " & Me.poNumber

        connectionString = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
        sql = "SELECT * FROM MPHEAD where IAPUNO = '" & Trim(Me.poNumber) & "'"

        connection = New SqlConnection(connectionString)

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds)
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MsgBox(ex)
        End Try

        lblPO.Text = ds.Tables(0).Rows(0)("IAPUNO")
        lblCreditorID.Text = ds.Tables(0).Rows(0)("IASUNO")
        lblStatus.Text = ds.Tables(0).Rows(0)("IAPUSL")
        lblWarehouseID.Text = ds.Tables(0).Rows(0)("IAWHLO")

        sql = "SELECT MPLINE.IBPUNO, MPLINE.IBPNLI, MPLINE.IBPNLS, MPLINE.IBITNO, MITMAS.MMFUDS, MPLINE.IBWHLO, MPLINE.IBFACI, MPLINE.IBSITE, MPLINE.IBORQA, MPLINE.IBPUPR, MPLINE.IBPPUN, MPLINE.IBPUUN, MPLINE.IBRVQA, MITMAS.MMUNMS, MITMAS.MMALUC, MITMAS.MMSTUN " & _
                "FROM MPLINE LEFT JOIN MITMAS ON MPLINE.IBITNO = MITMAS.MMITNO " & _
                "WHERE MPLINE.IBPUNO='" & Me.poNumber & "';"

        connection = New SqlConnection(connectionString)

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds2.Tables.Add)
            adapter.Dispose()
            command.Dispose()
            connection.Close()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        ds2.Tables(0).Columns.Add("Saved")
        ds2.Tables(0).Columns.Add("Packs Received")
        ds2.Tables(0).Columns.Add("Qty Received")
        ds2.Tables.Add("Saved")


        For i = 0 To ds2.Tables(0).Rows.Count - 1
            LineNo = ds2.Tables(0).Rows(i)("IBPNLI").ToString
            subLineNo = ds2.Tables(0).Rows(i)("IBPNLS").ToString

            'Open the Gunnersen database and chek the number of packs already saved, if this number is equal to the total number for that line then diable any further entries. 
            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
            sql = "SELECT ATTRIBUTE_VALUE, LINE_ID FROM Attributes WHERE ATTRIBUTE_VALUE= 'SUPPPACK' AND LINE_ID='" & poNumber & LineNo & subLineNo & "';"

            connection = New SqlConnection(connectionString)

            Try
                connection.Open()
                command = New SqlCommand(sql, connection)
                adapter.SelectCommand = command
                adapter.Fill(ds2.Tables("Saved"))
                adapter.Dispose()
                command.Dispose()
                connection.Close()

            Catch ex As Exception
                MsgBox(ex)
            End Try

            QtySaved = ds2.Tables("Saved").Rows.Count
            ds2.Tables(0).Rows(i)("Saved") = QtySaved
            ds2.Tables("Saved").Clear()
        Next

        Try
            dgvItems.DataSource = ds2.Tables(0)
            With dgvItems
                .Columns("IBPUNO").Visible = False
                '.Columns("MMITDS").Visible = False
                .Columns("IBWHLO").Visible = False
                .Columns("IBFACI").Visible = False
                .Columns("IBSITE").Visible = False
                .Columns("IBPUPR").Visible = False
                .Columns("MMUNMS").Visible = False
                .Columns("MMALUC").Visible = False
                .Columns("MMSTUN").Visible = False
                .Columns("IBPUUN").Visible = False
                .Columns("IBPPUN").Visible = False
                .Columns("IBRVQA").DefaultCellStyle.Format = "N0"
                .Columns("IBORQA").DefaultCellStyle.Format = "N0"
                .Columns("IBPNLS").HeaderCell.Value = "Sub Line"
                .Columns("IBITNO").HeaderCell.Value = "Product ID"
                .Columns("MMFUDS").HeaderCell.Value = "Descripton"
                .Columns("IBORQA").HeaderCell.Value = "Order Qty"
                .Columns("IBPNLI").HeaderCell.Value = "Line"
                .Columns("IBRVQA").HeaderCell.Value = "Qty Receipted"
                .Columns("IBPNLS").Width = 80
                .Columns("IBITNO").Width = 135
                .Columns("MMFUDS").Width = 210
                .Columns("IBORQA").Width = 80
                .Columns("IBPNLI").Width = 40
                .Columns("IBPUUN").Width = 50
            End With

            For i = 0 To dgvItems.Columns.Count - 3
                dgvItems.Columns(i).DisplayIndex = i + 2
                dgvItems.Columns(i).ReadOnly = True
            Next

            dgvItems.Columns("Packs Received").DisplayIndex = 0
            dgvItems.Columns("Qty Received").DisplayIndex = 1

            'Check attribute value, if it's "MIXEDPACK" then set the itemQty cell to 1 and readonly
            connectionString = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"

            For i = 0 To dgvItems.Rows.Count - 1
                sql = "SELECT * FROM MITMAS WHERE MMITNO = '" & ds2.Tables(0).Rows(i)("IBITNO") & "';"
                connection = New SqlConnection(connectionString)

                Try
                    connection.Open()
                    command = New SqlCommand(sql, connection)
                    adapter.SelectCommand = command
                    If i = 0 Then
                        adapter.Fill(ds2.Tables.Add("Product Table PO"))
                    Else
                        adapter.Fill(ds2.Tables("Product Table PO"))
                    End If
                    adapter.Dispose()
                    command.Dispose()
                    connection.Close()
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try


                attributeValue = Trim(ds2.Tables("Product Table PO").Rows(i)("MMATMO"))
                If attributeValue = "MIXEDPACK" Then
                    dgvItems.Rows(i).Cells.Item("Qty Received").Value = 1
                    'dgvItems.Rows(i).Cells("Qty Received").ReadOnly = True
                End If
            Next


        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try

        ' sets the width of the datagridview to the total width of all the columns meaning no grey space on sides
        Dim dgvWidth As Integer = 43
        For i = 0 To dgvItems.Columns.Count - 1
            If dgvItems.Columns(i).Visible = True Then
                dgvWidth = dgvWidth + dgvItems.Columns(i).Width
            End If
        Next
        dgvItems.Width = dgvWidth

        'Open the Gunnersen database and check for runs already completed and fill the datagrid with run if they exist. 
        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
        sql = "SELECT DELDOCKNO, RUN_NO, RUN_DATETIME FROM LineReceiving WHERE PO = '" & poNumber & "';"

        connection = New SqlConnection(connectionString)

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds2.Tables.Add("Runs"))
            adapter.Dispose()
            command.Dispose()
            connection.Close()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        For i = 0 To dgvRun.Columns.Count - 1
            dgvRun.Columns(i).DisplayIndex = i + 2
        Next

        dgvRun.DataSource = ds2.Tables("Runs")
        Dim btn As New DataGridViewButtonColumn()
        dgvRun.Columns.Add(btn)
        btn.HeaderText = ""
        btn.Text = "..."
        btn.Name = "btn"
        dgvRun.Columns("btn").DisplayIndex = 0
        dgvRun.Columns("btn").Width = 20

        Dim post As New DataGridViewButtonColumn()
        dgvRun.Columns.Add(post)
        post.HeaderText = ""
        post.Text = "M3"
        post.Name = "post"
        dgvRun.Columns("post").DisplayIndex = 1
        dgvRun.Columns("post").Width = 20


    End Sub

    Private Sub tsbNewRun_Click(sender As Object, e As EventArgs) Handles tsbNewRun.Click
        frmAttEnt = New frmAttributeEntry(Me.attributeTable, supPackID, False, ds2, "")
        frmAttEnt.Show()
        frmAttEnt.MdiParent = frmMain
        Me.Close()
    End Sub

    Public Function RunSQL(ByVal conString As String, ByVal sql As String, tableName As String)
        connection = New SqlConnection(conString)
        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds.Tables.Add(tableName))
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Function

    Private Sub dgvItems_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvItems.CellDoubleClick
        MessageBox.Show(e.ColumnIndex.ToString)
    End Sub

    Private Sub dgvItems_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvItems.CellValueChanged
        If e.ColumnIndex = 17 Then
            If Not dgvItems.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = "" Then
                If IsNumeric(dgvItems.Rows(e.RowIndex).Cells(17).Value) Then
                    Dim rowPackQty, rowReceivedQty, rowSaved, rowQtytoRec, rowPacksRec As Integer
                    rowPackQty = dgvItems.Rows(e.RowIndex).Cells("IBORQA").Value
                    rowReceivedQty = dgvItems.Rows(e.RowIndex).Cells("IBRVQA").Value
                    rowSaved = dgvItems.Rows(e.RowIndex).Cells("Saved").Value
                    rowQtytoRec = rowPackQty - rowReceivedQty - rowSaved
                    rowPacksRec = dgvItems.Rows(e.RowIndex).Cells("Packs Received").Value
                    If rowPacksRec > rowQtytoRec Then
                        MessageBox.Show("Quanity of packs entered to receive is greater than the amount left to receive.")
                    End If
                Else
                    dgvItems.Rows(e.RowIndex).Cells(17).Value = ""
                End If
            Else
                MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        End If

    End Sub

    Private Sub dgvItems_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles dgvItems.KeyPress
        Dim Characters As String = ChrW(Keys.Tab)
        If InStr(Characters, e.KeyChar) = 1 Then
            If Me.dgvItems.CurrentCell.ColumnIndex = 1 Then
                'Me.dgvItems.CurrentCell.ColumnIndex = Me.dgvItems.CurrentCell.ColumnIndex + 1
                If Me.dgvItems.CurrentRow.Index = Me.dgvItems.Rows.Count - 1 Then
                    Me.dgvItems.CurrentCell = Me.dgvItems.Rows(0).Cells(17)
                Else
                    Me.dgvItems.CurrentCell = Me.dgvItems.Rows(Me.dgvItems.CurrentRow.Index + 1).Cells(17)
                End If
            Else
            End If
        End If
    End Sub

    Private Function PostToM3(ByVal runNumber As String)
        Me.Cursor = Cursors.WaitCursor
        ' load relevant run into dataset

        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
        sql = "SELECT * FROM Attributes WHERE RUN_NUMBER = '" & runNumber & "';"
        connection = New SqlConnection(connectionString)
        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds.Tables.Add("Run"))
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try

        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
        sql = "SELECT * FROM Attributes WHERE ATTRIBUTE_VALUE = 'SUPPPACK' AND RUN_NUMBER = '" & runNumber & "';"
        connection = New SqlConnection(connectionString)
        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds.Tables.Add("Packs"))
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try

        For i = 0 To ds.Tables("packs").Rows.Count - 1


            Dim Server As String = "M3BE"
            Dim Port As String = "16205"
            Dim UserID As String = "DTAMIGR"
            Dim PWD As String = "Q190E87AG"
            Dim APIName As String = "PPS001MI"
            Dim APIOpr As String = "Receipt"
            Dim sid As New SERVER_ID

            Dim rc
            Dim todayDate As String
            Dim respUser As String
            Dim PNLS As String
            Dim catchWeight As String
            Dim isPack As Boolean
            Dim putAway As String
            Dim lotNO As String
            Dim isLotControl As Boolean
            Dim purOrderLine As String
            Dim itemNumber As String
            Dim strATNR As String
            Dim strSuppPackID As String
            Dim attVal As String
            Dim attQty As String
            Dim tally As String
            Dim strSevicename

            Dim env
            Dim strUsername, strPassword As String
            Dim strServiceRoot, strNamespaceBase, strServiceName, strMethod, strProgram As String
            Dim prm1
            Dim http


            rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
            If rc <> 0 Then
                MvxSock.ShowLastError(sid, "Error Occured: ")
            End If

            todayDate = Date.Today().ToString("yyyyMMdd")
            respUser = UCase(modFunctions.GetActiveDirUserDetails(Environment.UserName))

            PNLS = ds.Tables("Packs").Rows(i)("ROWNUMBER")
            If PNLS Is Nothing Then
                PNLS = 0
            End If

            If attributeValue = "MIXEDPACK" Then
                isLotControl = True
            End If

            purOrderLine = Trim(ds2.Tables(0).Rows(PNLS)("IBPNLI"))
            itemNumber = ds2.Tables(0).Rows(PNLS)("IBITNO")

            catchWeight = ds.Tables("Packs").Rows(i)("CATCHWEIGHT")
            Dim wrkCAWE = catchWeight
            tally = ds.Tables("Packs").Rows(i)("Tally")
            attributeValue = Trim(ds2.Tables("Product Table PO").Rows(PNLS)("MMATMO"))
            Dim pack = Trim(ds2.Tables(0).Rows(PNLS)("IBPUUN"))
            If Trim(ds2.Tables(0).Rows(PNLS)("IBPUUN")) = "PK" Then
                isPack = True
            Else
                isPack = False
            End If


            MvxSock.SetField(sid, "CONO", "100")
            MvxSock.SetField(sid, "TRDT", todayDate)
            MvxSock.SetField(sid, "RESP", respUser)
            MvxSock.SetField(sid, "PUNO", poNumber)
            MvxSock.SetField(sid, "PNLI", purOrderLine)
            MvxSock.SetField(sid, "PNLS", PNLS)

            If isPack Then
                If isLotControl Then
                    MvxSock.SetField(sid, "CAWE", PNLS)
                    MvxSock.SetField(sid, "RVQA", 1)
                End If
            Else
                'HANDLE THINGS THAT AREN'T PACKS HERE
            End If

            MvxSock.SetTrimFields(sid, 0)  ' Do not trim trailing spaces
            rc = MvxSock.Access(sid, APIOpr)
            If rc <> 0 Then
                MvxSock.ShowLastError(sid, "Error Occurred: ")
            End If

            '__________________________________________________________________________________________________________________________

            'Connect to M3 using the Stock Transaction History Interface to get the putaway and lot numbers
            Server = "M3BE"
            Port = "16205"
            UserID = "DTAMIGR"
            PWD = "Q190E87AG"
            APIName = "MWS070MI"
            APIOpr = "LstTransByOrder"

            ' Connect API here
            rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
            If rc <> 0 Then
                MvxSock.ShowLastError(sid, "Error Occurred: ")
            End If

            MvxSock.SetField(sid, "TTYP", "20")       '
            MvxSock.SetField(sid, "RIDN", poNumber) 'Purchase Order Number
            MvxSock.SetField(sid, "RIDL", purOrderLine) 'Purchase Order Line

            MvxSock.SetTrimFields(sid, 0) ' Do not trim trailing spaces
            rc = MvxSock.Access(sid, APIOpr)
            If rc <> 0 Then
                MvxSock.ShowLastError(sid, "Error Occurred: ")
                'GoTo abnormalExit 
            End If

            While MvxSock.More(sid)
                putAway = MvxSock.GetField(sid, "REPN")
                If isLotControl Then
                    lotNO = MvxSock.GetField(sid, "BANO")
                End If
                MvxSock.Access(sid, Nothing)
            End While

            '_____________________________________________________________________________________________________________
            'connect to M3 using the MMS235MI to get the Attribute Type Number

            If isLotControl Then
                Server = "M3BE"
                Port = "16205"
                UserID = "DTAMIGR"
                PWD = "Q190E87AG"
                APIName = "MMS235MI"
                APIOpr = "GetLotItm"

                ' Connect API here
                rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, "")
                If rc <> 0 Then
                    MvxSock.ShowLastError(sid, "Error Occurred: ")
                    'GoTo AbnormalExit
                End If
                MvxSock.SetField(sid, "BANO", lotNO)  'Lot Number
                MvxSock.SetField(sid, "ITNO", itemNumber) 'Item Number

                ' set the transaction name and call it
                MvxSock.SetTrimFields(sid, 0) ' Do not trim trailing spaces
                rc = MvxSock.Access(sid, APIOpr)
                If rc <> 0 Then
                    MvxSock.ShowLastError(sid, "Error Occurred: ")
                    'GoTo AbnormalExitmms235
                End If

                strATNR = MvxSock.GetField(sid, "ATNR")

                'On Error GoTo loopErrorHandler

                '__________________________________________________________________________________________________________________
                'connect to M3 using the ATS101MI to set the attributes for each item.

                Server = "M3BE"
                Port = "16205"
                UserID = "DTAMIGR"
                PWD = "Q190E87AG"
                APIName = "ATS101MI"
                APIOpr = "SetAttrValue"

                ' Connect API here

                rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, "")
                If rc <> 0 Then
                    MvxSock.ShowLastError(sid, "Error Occurred: ")
                    'GoTo AbnormalExit
                End If
                catchWeight = 0



                For j = 0 To 15 'ds.Tables("run").Rows.Count - 1

                    attVal = ds.Tables("run").Rows(j)("ATTRIBUTE_VALUE")
                    attQty = ds.Tables("Run").Rows(j)("ATTRIBUTE_QTY")

                    MvxSock.SetField(sid, "CONO", "100")       '
                    MvxSock.SetField(sid, "ATNR", strATNR) 'Attribute Number
                    MvxSock.SetField(sid, "ATID", attVal) 'Attribute ID
                    MvxSock.SetField(sid, "ATVA", attQty) 'Value for Attribute

                    If attVal = "SUPPPACK" Then
                        strSuppPackID = attQty          ''?????????????????????????????
                    End If

                    'If lotControl > 1 And LoopVal > 1 Then
                    'strSuppPackID = "BULK"                     ' deal with this when doing standard packs
                    'End If


                    ' set the transaction name and call it
                    MvxSock.SetTrimFields(sid, 0) ' Do not trim trailing spaces
                    rc = MvxSock.Access(sid, APIOpr)
                    If rc <> 0 Then
                        MvxSock.ShowLastError(sid, "Error Occurred: ")
                        'GoTo AbnormalExit
                    End If
                Next

                '________________________________________________________________________________________________________________________________
                ' Connect to M3 using the CUSEXTMI to set the CatchWeight and TallyString

                Server = "M3BE"
                Port = "16205"
                UserID = "DTAMIGR"
                PWD = "Q190E87AG"
                APIName = "CUSEXTMI"
                APIOpr = "AddFieldValue"
                Server = "M3BE"
                ' Connect API here
                rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, "")
                If rc <> 0 Then
                    MvxSock.ShowLastError(sid, "Error Occurred: ")
                    'GoTo AbnormalExit
                End If

                MvxSock.SetField(sid, "FILE", "AAAA")       '
                MvxSock.SetField(sid, "PK01", strATNR) 'Attribute Number
                MvxSock.SetField(sid, "A121", tally) 'Attribute ID
                MvxSock.SetField(sid, "N096", wrkCAWE) 'Value for Attribute


                ' set the transaction name and call it
                MvxSock.SetTrimFields(sid, 0) ' Do not trim trailing spaces
                rc = MvxSock.Access(sid, APIOpr)
                If rc <> 0 Then
                    MvxSock.ShowLastError(sid, "Error Occurred: ")
                    'GoTo AbnormalExit
                End If
                '_________________________________________________________________________________________________________________________________

                'Me.Cursor = Cursors.Arrow

                'env = CreateObject("pocketSOAP.Envelope.11")
                'env.EncodingStyle = ""

                'strUsername = "DTAMIGR"
                'strPassword = "Q190E87AG"
                'strServiceRoot = "http://m3be.gunnersens.com.au:21007/mws-ws/services"
                'strNamespaceBase = "http://schemas.lawson.com"
                'strServiceName = "StockOperations"
                'strMethod = "Catchweight"
                'strProgram = "MMS360"

                'http = CreateObject("pocketSOAP.HTTPTransport")
                'http.Authentication(strUsername, strPassword)
                'http.SOAPAction = strMethod

                '' set the method to be called

                '' env.SetMethod(strMethod, strNamespaceBase & "/" & strServiceName & "/" & strMethod)
                'env.MethodName = strMethod
                'env.URI = strServiceRoot & "/" & strServiceName

                '' set the name of the program here

                'prm1 = env.Parameters.Create(strProgram, "", env.URI)
                '' set the input values here

                'prm1.Nodes.Create("ItemNumber", "" & itemNumber, env.URI)
                'prm1.Nodes.Create("Warehouse", "" & ds.Tables(0).Rows(0)("IAWHLO"), env.URI)
                'prm1.Nodes.Create("LotNumber", "" & Trim(lotNO), env.URI)
                'prm1.Nodes.Create("Location", "RECEIVING", env.URI)
                'prm1.Nodes.Create("ReceivingNumber", "" & Trim(putAway), env.URI)
                'prm1.Nodes.Create("CatchWeight", "" & wrkCAWE, env.URI)

                '' set the service name

                'Dim test = env.serialize.ToString

                'MessageBox.Show(http.send(strServiceRoot & "/" & strServiceName, env.serialize).ToString)


                '' parse the envelope
                'env.Parse(http)
            End If
        Next
    End Function


    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub
    Private Sub dgvRun_CelltClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvRun.CellContentClick
        If e.ColumnIndex = 0 Then
            frmAttEnt = New frmAttributeEntry(Me.attributeTable, supPackID, True, ds2, "")
            frmAttEnt.Show()
            frmAttEnt.MdiParent = frmMain
            Me.Close()
        End If
    End Sub
End Class