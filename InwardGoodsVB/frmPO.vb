Imports System.Data.SqlClient
Imports System.DirectoryServices
Imports Lawson.M3.MvxSock
Imports PocketSOAP
Imports InwardGoodsVB.wsStockOperations
Imports System.ServiceModel
Imports System.Diagnostics


Public Class frmPO
    Private _poNumber As String
    Private _lineloop As Integer
    Private _igType As String
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
    Dim supPackID As String
    Dim attributeValue As String
    Dim lotControl As Integer = 0
    Public attributeTable As DataTable = New DataTable("Attribute Table")
    Dim strWrk, strWrk1, strWrk2 As String
    Dim runNumber As Integer
    Private frmAttEnt As frmAttributeEntry
    Private _dt As DataTable
    Dim duplicatePacks As Integer
    Dim i As Integer
    Dim j As Integer
    Dim itemQty As Integer
    Dim packQty As Integer
    Dim qtyTotal As Integer
    Dim RUN_NUMBER As String
    Dim img As Image
    Dim frmProg As frmProgress
    Private frmPurchaseOrder As frmPO
    Dim delete As String
    Dim dsLabels As New DataSet
    Dim currentDate As DateTime = DateTime.Now
    Dim intday As String = currentDate.Day
    Dim intYear As String = currentDate.Year
    Dim pb As New ProgressBar
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
    Dim rowNumber As Integer
    Dim wrkCAWE As Decimal
    Dim packsRow As Integer
    Dim totPackQty As Integer
    Dim totItemQty As Integer
    Dim loopcount As Integer
    Dim perpack As Decimal
    Dim shortpack As Integer

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
    Public Property igType() As String
        Get
            Return _igType
        End Get
        Set(value As String)
            _igType = value
        End Set
    End Property
    Public Sub New(ByVal po As String, ByVal lineloop As Integer, ByVal igType As String)
        InitializeComponent()
        Me.poNumber = po
        Me.lineloop = lineloop
        Me.igType = igType
    End Sub

    Private Sub frmPO_Load(sender As Object, e As EventArgs) Handles Me.Load

        lblTitle.Text = "Inward Good Processing for " & igType & " " & Me.poNumber
        connectionString = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"

        If igType = "PO" Then
            sql = "SELECT * FROM MPHEAD where IAPUNO = '" & Trim(Me.poNumber) & "'"
        ElseIf igType = "DO" Then
            sql = "SELECT * FROM MGHEAD where MGTRNR = '" & Trim(Me.poNumber) & "'"
        End If

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

        If igType = "PO" Then
            lblOne.Text = "Purchase Order Number"
            lblTwo.Text = "Creditor ID"
            lblStatus.Visible = True
            lblPO.Text = ds.Tables(0).Rows(0)("IAPUNO")
            lblCreditorID.Text = ds.Tables(0).Rows(0)("IASUNO")
            lblStatus.Text = ds.Tables(0).Rows(0)("IAPUSL")
            lblWarehouseID.Text = ds.Tables(0).Rows(0)("IAWHLO")
        ElseIf igType = "DO" Then
            lblOne.Text = "Distribution Order Number"
            lblTwo.Text = "To Warehouse"
            lblWarehouseID.Text = ds.Tables(0).Rows(0)("MGWHLO")
            lblPO.Text = ds.Tables(0).Rows(0)("MGTRNR")
            lblCreditorID.Text = ds.Tables(0).Rows(0)("MGTWLO")
            lblStatus.Visible = False
            lblstatustext.visible = False
        End If

        FillItemGrid()

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
            If attributeValue = "" Then
                Select Case Trim(ds2.Tables("Product Table PO").Rows(i)("MMITTY"))
                    Case "G10"
                        attributeValue = "EXPIRY"
                    Case "G18"
                        attributeValue = "BATCH"
                    Case Else
                        attributeValue = "STANDARD"
                End Select
            End If

            If attributeValue = "MIXEDPACK" Then
                dgvItems.Rows(i).Cells.Item("Qty Received").Value = 1
                'dgvItems.Rows(i).Cells("Qty Received").ReadOnly = True
            End If
        Next

        FillSavedColumn()

        ' sets the width of the datagridview to the total width of all the columns meaning no grey space on sides
        Dim dgvWidth As Integer = 43
        For i = 0 To dgvItems.Columns.Count - 1
            If dgvItems.Columns(i).Visible = True Then
                dgvWidth = dgvWidth + dgvItems.Columns(i).Width
            End If
        Next
        dgvItems.Width = dgvWidth

        FillRunDataGrid()

    End Sub

    Private Sub tsbNewRun_Click(sender As Object, e As EventArgs) Handles tsbNewRun.Click

        For countRows = 0 To dgvItems.Rows.Count - 1
            If IsDBNull(dgvItems.Rows(countRows).Cells("Packs Received").Value) Then
                dgvItems.Rows(countRows).Cells("Packs Received").Value = 0
            End If
            totPackQty = totPackQty + dgvItems.Rows(countRows).Cells("Packs Received").Value

            If IsDBNull(dgvItems.Rows(countRows).Cells("Qty Received").Value) Then
                dgvItems.Rows(countRows).Cells("Qty Received").Value = 0
            End If
            totItemQty = totItemQty + dgvItems.Rows(countRows).Cells("Qty Received").Value
        Next

        ' dgvRun.DataSource = ds2.Tables("Runs")
        runNumber = ds2.Tables("Runs").Rows.Count + 1

        If Not totPackQty = 0 Then
            frmAttEnt = New frmAttributeEntry(Me.attributeTable, supPackID, False, ds2, runNumber)
            If attributeValue.Contains("IX") Then
                If frmAttEnt.IsDisposed = False Then
                    frmAttEnt.Show()
                    frmAttEnt.MdiParent = frmMain
                    Me.Close()
                End If
            End If
        Else
            MessageBox.Show("You have not entered any packs, enter packs to be received.")
        End If
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

    Private Sub dgvItems_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvItems.CellValidating
        If e.ColumnIndex = 17 Then
            If attributeValue.Contains("IX") Then
                If Not e.FormattedValue = "" Then
                    If IsNumeric(e.FormattedValue) Then
                        Dim rowPackQty, rowReceivedQty, rowSaved, rowQtytoRec, rowPacksRec As Integer
                        rowPackQty = dgvItems.Rows(e.RowIndex).Cells("IBORQA").Value
                        rowReceivedQty = dgvItems.Rows(e.RowIndex).Cells("IBRVQA").Value
                        rowSaved = dgvItems.Rows(e.RowIndex).Cells("Saved").Value
                        rowQtytoRec = rowPackQty - rowReceivedQty - rowSaved
                        rowPacksRec = e.FormattedValue
                        If rowPacksRec > rowQtytoRec Then
                            If Not IsNothing(dgvItems.EditingControl.Text) Then
                                MessageBox.Show("Quanity of packs entered to receive is greater than the amount left to receive, please re-enter")
                                dgvItems.EditingControl.Text = ""
                                e.Cancel = True
                            End If
                        Else
                            tsbNewRun.Enabled = True
                        End If

                    Else
                        MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        dgvItems.EditingControl.Text = ""
                        e.Cancel = True
                    End If
                Else
                    If Not e.FormattedValue = "" Then
                        MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    End If
                End If
            End If
        End If

        If e.ColumnIndex = 18 Then
            If Not attributeValue.Contains("IX") Then
                If Not e.FormattedValue = "" Then
                    If IsNumeric(e.FormattedValue) Then
                        Dim rowPackQty, rowReceivedQty, rowSaved, rowQtytoRec, rowPacksRec As Integer
                        rowPackQty = dgvItems.Rows(e.RowIndex).Cells("IBORQA").Value
                        rowReceivedQty = dgvItems.Rows(e.RowIndex).Cells("IBRVQA").Value
                        rowSaved = dgvItems.Rows(e.RowIndex).Cells("Saved").Value
                        rowQtytoRec = rowPackQty - rowReceivedQty - rowSaved
                        rowPacksRec = e.FormattedValue
                        If rowPacksRec > rowQtytoRec Then
                            If Not IsNothing(dgvItems.EditingControl.Text) Then
                                MessageBox.Show("Quanity of items entered to receive is greater than the amount left to receive, please re-enter")
                                dgvItems.EditingControl.Text = ""
                                e.Cancel = True
                            End If
                        Else
                            tsbNewRun.Enabled = True
                        End If

                    Else
                        MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        dgvItems.EditingControl.Text = ""
                        e.Cancel = True
                    End If
                Else
                    If Not e.FormattedValue = "" Then
                        MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    End If
                End If
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
        sql = "SELECT * FROM Attributes WHERE RUN_NUMBER = '" & poNumber & runNumber & "';"
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
        sql = "SELECT * FROM Attributes WHERE PACKTYPE IS NOT NULL AND RUN_NUMBER = '" & poNumber & runNumber & "';"
        connection = New SqlConnection(connectionString)
        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds.Tables.Add("Packs"))
            adapter.Dispose()
            command.Dispose()
            connection.Close()


            Dim runs As Integer = ds.Tables("Packs").Rows.Count
            Dim progressText As String
            frmProgress.ProgressSetup(runs * 2)

            For i = 0 To ds.Tables("Packs").Rows.Count - 1

                progressText = "Pack number " & i + 1 & " of " & runs & " packs processing"

                frmProgress.IncProg(progressText)

                '______________________________________________________________________________________________________________________________________
                ' connect to M3 via the PPS001MI to receipt the current pack

                Dim Server As String = "M3BE"
                Dim Port As String = "16205"
                Dim UserID As String = "DTAMIGR"
                Dim PWD As String = "Q190E87AG"
                Dim APIName As String = "PPS001MI"
                Dim APIOpr As String = "Receipt"
                Dim sid As New SERVER_ID

                rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
                If rc <> 0 Then
                    MvxSock.ShowLastError(sid, "Error Occured: ")
                End If

                todayDate = Date.Today().ToString("yyyyMMdd")
                respUser = UCase(modFunctions.GetActiveDirUserDetails(Environment.UserName))
                rowNumber = ds.Tables("Packs").Rows(i)("ROWNUMBER")
                attributeValue = ds.Tables("Packs").Rows(i)("PACKTYPE")

                If attributeValue = "MIXEDPACK" Or attributeValue.ToString.Contains("FIXED") Then
                    isLotControl = True
                    catchWeight = ds.Tables("Packs").Rows(i)("CATCHWEIGHT")
                    wrkCAWE = catchWeight
                    tally = ds.Tables("Packs").Rows(i)("Tally")
                End If

                purOrderLine = Trim(ds2.Tables(0).Rows(rowNumber)("IBPNLI"))
                itemNumber = Trim(ds2.Tables(0).Rows(rowNumber)("IBITNO"))
                PNLS = ds2.Tables(0).Rows(rowNumber)("IBPNLS")
                Dim pack = Trim(ds2.Tables(0).Rows(rowNumber)("IBPUUN"))

                If Trim(ds2.Tables(0).Rows(rowNumber)("IBPUUN")) = "PK" Then
                    isPack = True
                Else
                    isPack = False
                End If

                itemQty = ds.Tables("Packs").Rows(i)("ITEMQTY")

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
                    MvxSock.SetField(sid, "CAWE", PNLS)
                    MvxSock.SetField(sid, "RVQA", itemQty)
                End If

                MvxSock.SetTrimFields(sid, 0)  ' Do not trim trailing spaces
                rc = MvxSock.Access(sid, APIOpr)
                If rc <> 0 Then
                    MvxSock.ShowLastError(sid, "Error Occurred: ")
                    GoTo abnormalExit
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
                    GoTo abnormalExit
                End If

                While MvxSock.More(sid)
                    putAway = MvxSock.GetField(sid, "REPN")
                    If isLotControl Then
                        lotNO = MvxSock.GetField(sid, "BANO")
                    End If
                    MvxSock.Access(sid, Nothing)
                End While

                '____________________________________________________________________________________________________________
                'connect to M3 using the MMS235MI to get the Attribute Type Number - not needed for std

                If isLotControl Then
                    Server = "M3BE"
                    Port = "16205"
                    UserID = "DTAMIGR"
                    PWD = "Q190E87AG"
                    APIName = "MMS235MI"
                    APIOpr = "GetLotItm"

                    ' Connect API here
                    rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
                    If rc <> 0 Then
                        MvxSock.ShowLastError(sid, "Error Occurred: ")
                        GoTo abnormalExit
                    End If
                    MvxSock.SetField(sid, "BANO", lotNO)  'Lot Number
                    MvxSock.SetField(sid, "ITNO", itemNumber) 'Item Number

                    ' set the transaction name and call it
                    MvxSock.SetTrimFields(sid, 0) ' Do not trim trailing spaces
                    rc = MvxSock.Access(sid, APIOpr)
                    If rc <> 0 Then
                        MvxSock.ShowLastError(sid, "Error Occurred: ")
                        GoTo AbnormalExit
                    End If

                    strATNR = MvxSock.GetField(sid, "ATNR")

                    '__________________________________________________________________________________________________________________
                    'connect to M3 using the ATS101MI to set the attributes for each item. - not needed for std

                    Server = "M3BE"
                    Port = "16205"
                    UserID = "DTAMIGR"
                    PWD = "Q190E87AG"
                    APIName = "ATS101MI"
                    APIOpr = "SetAttrValue"

                    rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)

                    ' connect api here
                    attQty = ds.Tables("run").Rows(j)("attribute_qty")
                    attVal = Trim((ds.Tables("run").Rows(j)("attribute_value")))
                    MvxSock.SetField(sid, "CONO", "100")       '
                    MvxSock.SetField(sid, "ATNR", strATNR) 'attribute number
                    MvxSock.SetField(sid, "ATID", attVal) 'attribute id
                    MvxSock.SetField(sid, "ATVA", attQty) 'value for attribute

                    If attVal = "SUPPPACK" Then
                        strSuppPackID = attQty          ''?????????????????????????????
                    End If


                    ' set the transaction name and call it
                    MvxSock.SetTrimFields(sid, 0) ' do not trim trailing spaces
                    rc = MvxSock.Access(sid, APIOpr)
                    If rc <> 0 Then
                        MvxSock.ShowLastError(sid, "error occurred: ")
                        GoTo abnormalexit
                    End If

                    j = j + 1
                    attVal = (ds.Tables("Run").Rows(j)("ATTRIBUTE_VALUE")).ToString.Length
                    attVal = Decimal.Parse(attVal)

                    Try '

                        Do Until Decimal.Parse((Trim(ds.Tables("Run").Rows(j)("ATTRIBUTE_VALUE"))).ToString.Length) > 5

                            attQty = ds.Tables("Run").Rows(j)("ATTRIBUTE_QTY")
                            attVal = Trim((ds.Tables("run").Rows(j)("ATTRIBUTE_VALUE")))
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
                                GoTo AbnormalExit
                            End If
                            j = j + 1
                            If j > ds.Tables("Run").Rows.Count - 1 Then
                                Exit Do
                            End If
                        Loop

                    Catch ex As Exception
                        'MessageBox.Show(ex.ToString)
                    End Try


                    '________________________________________________________________________________________________________________________________
                    ' Connect to M3 using the CUSEXTMI to set the CatchWeight and TallyString - not needed for fixed and std
                    If attributeValue.Contains("MIXED") Then

                        Server = "M3BE"
                        Port = "16205"
                        UserID = "DTAMIGR"
                        PWD = "Q190E87AG"
                        APIName = "CUSEXTMI"
                        APIOpr = "AddFieldValue"
                        Server = "M3BE"
                        ' Connect API here
                        rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
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
                            GoTo AbnormalExit
                        End If
                        '_________________________________________________________________________________________________________________________________
                        ' Connect to M3 using a webservice to set the catchweight of the pack

                        ' Create a client with basic http credentials
                        Dim client As New StockOperationsClient
                        Dim binding As New System.ServiceModel.BasicHttpBinding
                        binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic
                        binding.MaxReceivedMessageSize = 25 * 1024 * 1024
                        client.Endpoint.Binding = binding
                        client.ClientCredentials.UserName.UserName = "DTAMIGR"
                        client.ClientCredentials.UserName.Password = "Q190E87AG"

                        ' MessageBox.Show(client.Endpoint.Address.ToString)
                        ' MessageBox.Show(client.Endpoint.Name.ToString)

                        'create lws header
                        Dim header = New lws
                        header.user = "DTAMIGR"
                        header.password = "Q190E87AG"
                        Try
                            'Create a requests item
                            Dim catchweightItem = New ct_02
                            catchweightItem.ItemNumber = itemNumber
                            catchweightItem.Warehouse = ds.Tables(0).Rows(0)("IAWHLO")
                            catchweightItem.Location = "RECEIVING"
                            catchweightItem.LotNumber = Trim(lotNO)
                            catchweightItem.ReceivingNumber = Trim(putAway)
                            catchweightItem.CatchWeight = wrkCAWE

                            Dim collection = New CatchweightType
                            collection.MMS360 = catchweightItem
                            client.Catchweight(header, collection)

                        Catch ex As Exception
                            MessageBox.Show(ex.ToString)
                            GoTo abnormalExit
                        End Try
                    End If
                End If

                If Not ds.Tables("Packs").Columns.Contains("perPack") Then
                    ds.Tables("Packs").Columns.Add("perPack")
                    ds.Tables("Packs").Columns.Add("shortPack")
                    ds.Tables("Packs").Columns.Add("putAway")
                    ds.Tables("Packs").Columns.Add("lotNO")
                End If

                PrintLabels(rowNumber)
                SaveRunPackRows()
                frmProgress.IncProg(progressText)
                Dim attributeItemNO = ds.Tables("Packs").Rows(packsRow - 1)("ATTRIBUTE_ITEM")
                deleteRunLine(poNumber, runNumber, attributeItemNO)

            Next

            deleteRun(poNumber, runNumber)
            FillRunDataGrid()
            FillItemGrid()
            frmProgress.IncProg(progressText)
abnormalExit:
            Me.Cursor = Cursors.Arrow
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Function

    Private Sub dgvRun_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvRun.CellClick
        If e.RowIndex >= 0 Then
            If e.ColumnIndex = 0 Then
                frmAttEnt = New frmAttributeEntry(Me.attributeTable, supPackID, True, ds2, poNumber.ToString & dgvRun.Rows(e.RowIndex).Cells("RUN_NO").Value.ToString)
                frmAttEnt.Show()
                frmAttEnt.MdiParent = frmMain
            ElseIf e.ColumnIndex = 1 Then
                PostToM3(dgvRun.Rows(e.RowIndex).Cells("RUN_NO").Value.ToString)
            ElseIf e.ColumnIndex = 2 Then
                delete = MessageBox.Show("You are about to delete this run, this cannot be undone, do you wish to proceed?", "Delete Run?", MessageBoxButtons.YesNo)
                If delete = 6 Then
                    deleteRun(poNumber, dgvRun.Rows(e.RowIndex).Cells("RUN_NO").Value.ToString)
                    FillRunDataGrid()
                    FillItemGrid()
                    FillSavedColumn()
                End If
            End If
        End If
    End Sub

    Private Sub dgvRun_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvRun.CellPainting
        If e.ColumnIndex = 0 AndAlso e.RowIndex >= 0 Then
            e.Paint(e.CellBounds, DataGridViewPaintParts.All)
            img = Image.FromFile("C:\Inward Goods\InwardGoodsVB\InwardGoodsVB\bin\Debug\edit.png")
            e.Graphics.DrawImage(img, e.CellBounds.Left + 10, e.CellBounds.Top + 3, 15, 15)
            e.Handled = True
        End If

        If e.ColumnIndex = 1 AndAlso e.RowIndex >= 0 Then
            e.Paint(e.CellBounds, DataGridViewPaintParts.All)
            img = Image.FromFile("C:\Inward Goods\InwardGoodsVB\InwardGoodsVB\bin\Debug\forward.png")
            e.Graphics.DrawImage(img, e.CellBounds.Left + 10, e.CellBounds.Top + 3, 15, 15)
            e.Handled = True
        End If

        If e.ColumnIndex = 2 AndAlso e.RowIndex >= 0 Then
            e.Paint(e.CellBounds, DataGridViewPaintParts.All)
            img = Image.FromFile("C:\Inward Goods\InwardGoodsVB\InwardGoodsVB\bin\Debug\delete.png")
            e.Graphics.DrawImage(img, e.CellBounds.Left + 10, e.CellBounds.Top + 3, 15, 15)
            e.Handled = True
        End If
    End Sub

    Private Sub deleteRun(ByVal PurchNo As String, ByVal runNo As String)
        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"

        sql = "DELETE " & _
             " FROM Attributes" & _
             " WHERE RUN_NUMBER='" & PurchNo & runNo & "';"
        connection = New SqlConnection(connectionString)
        Try
            command = New SqlCommand(sql, connection)
            connection.Open()
            command.ExecuteNonQuery()

            sql = "DELETE " & _
                  " FROM LineReceiving" & _
                  " WHERE RUN_NO='" & runNo & "';"
            connection = New SqlConnection(connectionString)

            command = New SqlCommand(sql, connection)
            connection.Open()
            command.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Public Function FillRunDataGrid()
        'Open the Gunnersen database and check for runs already completed and fill the datagrid with run if they exist. 
        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
        sql = "SELECT DELDOCKNO, RUN_NO, RUN_DATETIME FROM LineReceiving WHERE PO = '" & poNumber & "';"

        connection = New SqlConnection(connectionString)

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            If Not ds2.Tables.Contains("Runs") Then
                adapter.Fill(ds2.Tables.Add("Runs"))
            Else
                ds2.Tables("Runs").Clear()
                adapter.Fill(ds2.Tables("Runs"))
            End If
            adapter.Dispose()
            command.Dispose()
            connection.Close()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        dgvRun.DataSource = ds2.Tables("Runs")
        'runNumber = ds2.Tables("Runs").Rows.Count

        If Not dgvRun.Columns.Contains("btn") Then
            Dim btn As New DataGridViewButtonColumn()
            dgvRun.Columns.Add(btn)
            btn.HeaderText = "Edit"
            btn.Text = "..."
            btn.Name = "btn"
            dgvRun.Columns("btn").DisplayIndex = 0
            dgvRun.Columns("btn").Width = 40
            'ttbuttons.Show("Edit this run", btn)

            Dim post As New DataGridViewButtonColumn()
            dgvRun.Columns.Add(post)
            post.HeaderText = "Post"
            post.Text = "M3"
            post.Name = "post"
            dgvRun.Columns("post").DisplayIndex = 1
            dgvRun.Columns("post").Width = 40
            'ttbuttons.Show("Post this run to M3", post)

            Dim del As New DataGridViewButtonColumn()
            dgvRun.Columns.Add(del)
            del.HeaderText = "Delete"
            del.Text = "Delete"
            del.Name = "Delete"
            dgvRun.Columns("Delete").DisplayIndex = 2
            dgvRun.Columns("Delete").Width = 40
            'ttbuttons.Show("Delete this run", del)

            dgvRun.Columns("RUN_NO").DisplayIndex = 3
            dgvRun.Columns("RUN_DATETIME").DisplayIndex = 5
            dgvRun.Columns("DELDOCKNO").DisplayIndex = 4
            dgvRun.Columns("RUN_NO").Visible = False
            dgvRun.Columns("RUN_DATETIME").Width = 150
            dgvRun.Columns("DELDOCKNO").Width = 150
            dgvRun.Columns("RUN_DATETIME").HeaderText = "Run Date & Time"
            dgvRun.Columns("DELDOCKNO").HeaderText = "Delivery Docket No"

            For i = 0 To dgvRun.Columns.Count - 1
                dgvRun.Columns(i).SortMode = DataGridViewColumnSortMode.NotSortable
            Next

        End If
    End Function

    Public Function FillItemGrid()
        connectionString = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
        If igType = "PO" Then
            sql = "SELECT MPLINE.IBPUNO, MPLINE.IBPNLI, MPLINE.IBPNLS, MPLINE.IBITNO, MITMAS.MMFUDS, MPLINE.IBWHLO, MPLINE.IBFACI, MPLINE.IBSITE, MPLINE.IBORQA, MPLINE.IBPUPR, MPLINE.IBPPUN, MPLINE.IBPUUN, MPLINE.IBRVQA, MITMAS.MMUNMS, MITMAS.MMALUC, MITMAS.MMSTUN " & _
            "FROM MPLINE LEFT JOIN MITMAS ON MPLINE.IBITNO = MITMAS.MMITNO " & _
            "WHERE MPLINE.IBPUNO='" & Me.poNumber & "';"
        ElseIf igType = "DO" Then
            sql = "SELECT MGLINE.MRPONR, MGLINE.MRPOSX, MGLINE.MRTRNR, MGLINE.MRITNO, MITMAS.MMFUDS, MGLINE.MRACQT " & _
                    "FROM MGLINE INNER JOIN MITMAS ON MGLINE.MRITNO = MITMAS.MMITNO " & _
                    "WHERE MGLINE.MRTRNR ='" & Me.poNumber & "';"
        End If
        connection = New SqlConnection(connectionString)

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            If Not ds2.Tables.Contains("Table1") Then
                adapter.Fill(ds2.Tables.Add)
            Else
                ds2.Tables(0).Clear()
                adapter.Fill(ds2.Tables(0))
            End If

            adapter.Dispose()
            command.Dispose()
            connection.Close()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        If Not ds2.Tables(0).Columns.Contains("Saved") Then
            ds2.Tables(0).Columns.Add("Saved")
            ds2.Tables(0).Columns.Add("Packs Received")
            ds2.Tables(0).Columns.Add("Qty Received")
            ds2.Tables.Add("Saved")
        End If

        Try

            If igType = "DO" Then
                ds2.Tables(0).Columns("MRITNO").ColumnName = "IBITNO"
                ds2.Tables(0).Columns("MRPOSX").ColumnName = "IBPNLS"
                ds2.Tables(0).Columns("MRPONR").ColumnName = "IBPNLI"
            End If

            dgvItems.DataSource = ds2.Tables(0)
            If igType = "PO" Then
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
            Else
            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try

        For i = 0 To dgvItems.Columns.Count - 3
            dgvItems.Columns(i).DisplayIndex = i + 2
            dgvItems.Columns(i).ReadOnly = True
        Next

        dgvItems.Columns("Packs Received").DisplayIndex = 0
        dgvItems.Columns("Qty Received").DisplayIndex = 1

        For i = 0 To dgvItems.Columns.Count - 1
            dgvItems.Columns(i).SortMode = DataGridViewColumnSortMode.NotSortable
        Next

    End Function

    Public Function FillSavedColumn()
        For count = 0 To ds2.Tables(0).Rows.Count - 1
            LineNo = ds2.Tables(0).Rows(count)("IBPNLI").ToString
            subLineNo = ds2.Tables(0).Rows(count)("IBPNLS").ToString

            'Open the Gunnersen database and chek the number of packs already saved, if this number is equal to the total number for that line then diable any further entries. 
            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"

            If attributeValue.Contains("IX") Then
                sql = "SELECT ATTRIBUTE_VALUE, LINE_ID, PACKQTY FROM Attributes WHERE ATTRIBUTE_VALUE= 'SUPPPACK' AND LINE_ID='" & poNumber & LineNo & subLineNo & "';"
            Else
                sql = "SELECT SUM(ITEMQTY) AS 'PACKQTY' FROM Attributes WHERE LINE_ID='" & poNumber & LineNo & subLineNo & "';"
            End If
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

            If attributeValue.Contains("IX") Then
                QtySaved = ds2.Tables("Saved").Rows.Count
            ElseIf Not IsDBNull(ds2.Tables("Saved").Rows(0)("PACKQTY")) Then
                QtySaved = ds2.Tables("Saved").Rows(0)("PACKQTY")
            Else
                QtySaved = 0
            End If

            ds2.Tables(0).Rows(count)("Saved") = QtySaved
            ds2.Tables("Saved").Clear()
        Next
    End Function

    Public Function PrintLabels(ByVal line As Integer)
        Dim warehouseID As String
        Dim branch As String
        Dim ProductID As String
        Dim Desc As String
        Dim typeFlag As String
        Dim printerid As String
        Dim printerport As String
        Dim login As String
        Dim pw As String
        Dim barcode As String
        Dim bin1 As String
        Dim bin2 As String

        Dim startCount As Integer
        Dim labelcount As Integer
        Dim loopval As Integer
        Dim intkeep As Integer
        Dim intsay As Integer
        Dim intcount As Integer
        Dim strHold As String
        Dim desc2 As String
        Dim desc3 As String


        If Len(intday.ToString) = 1 Then
            intday = "0" & intday
        End If
        Dim intMonth As String = currentDate.Month
        If Len(intMonth.ToString) = 1 Then
            intMonth = "0" & intMonth
        End If

        Dim strRecDate = intday & intMonth & intYear
        warehouseID = ds2.Tables(0).Rows(line)("IBFACI")
        branch = warehouseID
        If Strings.Left(warehouseID, 1) = "9" Then
            warehouseID = "900"
        End If
        ProductID = Trim(ds2.Tables("Product Table PO").Rows(line)("MMITNO"))
        Desc = ds2.Tables("Product Table PO").Rows(line)("MMFUDS")

        If IsNothing(Trim(ds2.Tables("Product Table PO").Rows(line)("MMITNO"))) = True Then
            typeFlag = ""
        Else
            typeFlag = ds2.Tables("Product Table PO").Rows(line)("MMEVGR")
        End If

        Dim FileNo = FreeFile()
        Dim File2 = FreeFile()
        ChDir("C:\Test")

        attributeValue = ds.Tables("Packs").Rows(packsRow)("PACKTYPE")

        If attributeValue.ToString.Contains("FIXED") Or attributeValue.ToString.Contains("MIXED") Then
            lotControl = True
            loopcount = 1
            tally = ds.Tables("Packs").Rows(packsRow)("TALLY")
            wrkCAWE = ds.Tables("Packs").Rows(packsRow)("CATCHWEIGHT")
            perpack = wrkCAWE

            ds.Tables("Packs").Rows(packsRow)("perPack") = perpack
            ds.Tables("Packs").Rows(packsRow)("shortPack") = shortpack
            ds.Tables("Packs").Rows(packsRow)("lotNo") = lotNO
            ds.Tables("Packs").Rows(packsRow)("putAway") = putAway

        Else
            lotControl = False
            loopcount = ds.Tables("Packs").Rows(packsRow)("PACKQTY")
            itemQty = ds.Tables("Packs").Rows(packsRow)("ITEMQTY")

            perpack = itemQty / loopcount
            If Not itemQty Mod loopcount = 0 Then
                perpack = Math.Ceiling(perpack)
                shortpack = itemQty - ((loopcount - 1) * perpack)
            End If

            ds.Tables("Packs").Rows(packsRow)("perPack") = perpack
            ds.Tables("Packs").Rows(packsRow)("shortPack") = shortpack
            ds.Tables("Packs").Rows(packsRow)("lotNo") = lotNO
            ds.Tables("Packs").Rows(packsRow)("putAway") = putAway
        End If

        connectionString = "Data Source=SQL01;Initial Catalog=DSA;Persist Security Info=True;User ID=timms;Password=timms123"
        sql = "SELECT *" & _
              " FROM LBLPrinter_DIM" & _
              " WHERE SysName = '" & Environ("computername") & "';"

        connection = New SqlConnection(connectionString)

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds2.Tables.Add("LblPrint"))
            adapter.Dispose()
            command.Dispose()
            connection.Close()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        printerport = Trim((ds2.Tables("LblPrint").Rows(0)("Printer_Port")))
        printerid = Trim(ds2.Tables("LblPrint").Rows(0)("Printer_IP"))
        login = Trim(ds2.Tables("LblPrint").Rows(0)("Login"))
        pw = Trim(ds2.Tables("LblPrint").Rows(0)("Password"))

        ds2.Tables.Remove("LBLPrint")

        barcode = GetBarcode(ProductID)

        connectionString = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
        sql = "SELECT *" & _
              " FROM MITBAL" & _
              " WHERE MBITNO = '" & ProductID & "' AND MBWHLO = '" & warehouseID & "';"

        connection = New SqlConnection(connectionString)

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds2.Tables.Add("MITBAL"))
            adapter.Dispose()
            command.Dispose()
            connection.Close()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        If ds2.Tables("MITBAL").Rows.Count = 0 Then
            bin1 = ""
            bin2 = ""
            MessageBox.Show("Product not in this warehouse, please contact Help Desk", "Product not in warehouse", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            If ds2.Tables("MITBAL").Rows(0)("MBDPLO") Is Nothing Then
                bin1 = ""
                MessageBox.Show("Product has no default location in this warehouse, Contact Help Desk", "No Default Location", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Else
                bin1 = ds2.Tables("MITBAL").Rows(0)("MBDPLO")
            End If

            If ds2.Tables("MITBAL").Rows(0)("MBWHSL") Is Nothing Then
                bin2 = ""
                MessageBox.Show("Product has no stock location in this warehouse, Contact Help Desk", "No Stock Location", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Else
                bin2 = ds2.Tables("MITBAL").Rows(0)("MBWHSL")
            End If
        End If
        ds2.Tables.Remove("MITBAL")

        FileOpen(FileNo, "ftp-cmd.txt", OpenMode.Output)
        Print(FileNo, "open " & printerid & vbCrLf)
        Print(FileNo, login & vbCrLf)
        Print(FileNo, pw & vbCrLf)
        Print(FileNo, "cd dest/" & printerport & vbCrLf)
        Print(FileNo, "send C:\Test\labelprint.txt" & vbCrLf)
        Print(FileNo, "bye")
        FileClose(FileNo)
        FileOpen(File2, "labelprint.txt", OpenMode.Output)
        Do While loopcount > 0

            If loopcount = 1 And attributeValue.Contains("STANDARD") Then
                perpack = shortpack
            End If

            labelcount = (loopcount + startCount)
            Print(File2, Chr(27) & "A" & vbCrLf)
            Print(File2, Chr(27) & "#E3" & vbCrLf)
            Print(File2, Chr(27) & "A124000776" & vbCrLf)
            Print(File2, Chr(27) & "%1" & vbCrLf)
            Print(File2, Chr(27) & "V2280" & Chr(27) & "H45" & Chr(27) & "CC2" & Chr(27) & "PY050" & vbCrLf)
            Print(File2, Chr(27) & "V2300" & Chr(27) & "H350" & Chr(27) & "L0202" & Chr(27) & "XMGUNNERSEN" & vbCrLf)

            typeFlag = "1" 'fixed check database

            Select Case typeFlag
                Case "0"
                    ' Don't know
                Case "1"
                    Print(File2, Chr(27) & "V2270" & Chr(27) & "H390" & Chr(27) & "C22" & Chr(27) & "PY051" & vbCrLf) ' FSC 100%
                Case "2"
                    Print(File2, Chr(27) & "V2270" & Chr(27) & "H390" & Chr(27) & "C22" & Chr(27) & "PY052" & vbCrLf) ' FSC Mix
                Case "3"
                    Print(File2, Chr(27) & "V2270" & Chr(27) & "H390" & Chr(27) & "C22" & Chr(27) & "PY052" & vbCrLf) ' FSC Mix
                Case "4"
                    Print(File2, Chr(27) & "V2270" & Chr(27) & "H390" & Chr(27) & "C22" & Chr(27) & "PY054" & vbCrLf) ' AFS/PEFC
                Case "5"
                    Print(File2, Chr(27) & "V2270" & Chr(27) & "H390" & Chr(27) & "C22" & Chr(27) & "PY054" & vbCrLf) ' AFS/PEFC
                Case "6"
                    Print(File2, Chr(27) & "V2270" & Chr(27) & "H390" & Chr(27) & "C22" & Chr(27) & "PY052" & vbCrLf) ' FSC Contr Wood
                Case "7"
                    Print(File2, Chr(27) & "V2270" & Chr(27) & "H390" & Chr(27) & "C22" & Chr(27) & "PY052" & vbCrLf) ' FSC Recycled
                Case Else
                    ' not specified
            End Select

            Print(File2, Chr(27) & "V1250" & Chr(27) & "H600" & Chr(27) & "L0102" & Chr(27) & "XMLocations - Prime:" & vbCrLf)
            Print(File2, Chr(27) & "V1050" & Chr(27) & "H700" & Chr(27) & "L0102" & Chr(27) & "XMPick:" & vbCrLf)
            Print(File2, Chr(27) & "V500" & Chr(27) & "H250" & Chr(27) & "L0102" & Chr(27) & "XMQty Per Pack:" & vbCrLf)
            Print(File2, Chr(27) & "&" & vbCrLf)
            Print(File2, Chr(27) & "Z" & vbCrLf)
            Print(File2, Chr(27) & "A" & vbCrLf)
            Print(File2, Chr(27) & "%1" & vbCrLf)
            Print(File2, Chr(27) & "/" & vbCrLf)
            Print(File2, Chr(27) & "V400" & Chr(27) & "H50" & Chr(27) & "L0102" & Chr(27) & "XM" & warehouseID & "-" & strRecDate & vbCrLf)

            If Len(Desc) > 30 Then
                intkeep = 0
                intcount = 0
                strHold = Desc
                Do While intkeep < 30
                    intSay = InStr(1, strHold, " ")
                    intkeep = intkeep + intSay
                    If intkeep > 30 And intcount = 0 Then
                        desc2 = Strings.Left(Desc, 30)
                        desc3 = Strings.Mid(Desc, 31, Len(Desc) - intkeep)
                        Exit Do
                    ElseIf intkeep > 30 Then
                        intkeep = intkeep - intSay
                        desc2 = Strings.Left(Desc, intkeep)
                        desc3 = Strings.Mid(Desc, intkeep + 1, Len(Desc) - intkeep)
                        Exit Do
                    ElseIf Len(strHold) > intkeep - 30 Then
                        intkeep = intkeep - intsay
                        desc2 = Strings.Left(Desc, intkeep)
                        desc3 = Strings.Mid(Desc, intkeep + 1, Len(Desc) - intkeep)
                        Exit Do
                    End If
                    intcount = intcount + 1
                    strHold = Mid(strHold, intSay + 1)
                Loop

            End If

            If lotControl Then
                Print(File2, Chr(27) & "V1900" & Chr(27) & "H210" & Chr(27) & "L0102" & Chr(27) & "XM" & ProductID & vbCrLf)
                Print(File2, Chr(27) & "V1900" & Chr(27) & "H50" & Chr(27) & "L0104" & Chr(27) & "XM" & Desc & vbCrLf)
                Print(File2, Chr(27) & "V1900" & Chr(27) & "H150" & Chr(27) & "L0202" & Chr(27) & "XS" & tally & vbCrLf) ' fixed value here
                If loopval <= 1 Then
                    Print(File2, Chr(27) & "V1900" & Chr(27) & "H550" & Chr(27) & "L0202" & Chr(27) & "XSSupplier Pack ID: " & strSuppPackID & vbCrLf) 'fixed value
                End If
            Else
                Print(File2, Chr(27) & "V1900" & Chr(27) & "H300" & Chr(27) & "L0204" & Chr(27) & "XM" & ProductID & vbCrLf)
                If Len(Desc) > 30 Then
                    Print(File2, Chr(27) & "V1900" & Chr(27) & "H50" & Chr(27) & "L0205" & Chr(27) & "XM" & desc2 & vbCrLf)
                    Print(File2, Chr(27) & "V1900" & Chr(27) & "H150" & Chr(27) & "L0205" & Chr(27) & "XM" & desc3 & vbCrLf)
                Else
                    Print(File2, Chr(27) & "V1900" & Chr(27) & "H50" & Chr(27) & "L0205" & Chr(27) & "XM" & Desc & vbCrLf)
                End If
            End If

            Print(File2, Chr(27) & "V750" & Chr(27) & "H580" & Chr(27) & "L0203" & Chr(27) & "XM" & bin1 & vbCrLf)
            Print(File2, Chr(27) & "V750" & Chr(27) & "H680" & Chr(27) & "L0203" & Chr(27) & "XM" & bin2 & vbCrLf)
            ' Print(File2, Chr(27) & "V1050" & Chr(27) & "H400" & Chr(27) & "L0203" & Chr(27) & "XM" & bin3 & vbcrlf) ' bin 3 is facings
            Print(File2, Chr(27) & "V200" & Chr(27) & "H250" & Chr(27) & "L0203" & Chr(27) & "XM" & perpack & vbCrLf)
            Print(File2, Chr(27) & "V1900" & Chr(27) & "H630" & Chr(27) & "L0202" & Chr(27) & "XSPutAway Number: " & putAway & vbCrLf)
            Print(File2, Chr(27) & "V1900" & Chr(27) & "H660" & Chr(27) & "BT101030103" & Chr(27) & "BW03100*" & putAway & "*" & vbCrLf)
            Print(File2, Chr(27) & "V400" & Chr(27) & "H370" & Chr(27) & "L0202" & Chr(27) & "XSProduct ID" & vbCrLf)
            Print(File2, Chr(27) & "V400" & Chr(27) & "H400" & Chr(27) & "BQ3015,1" & barcode & vbCrLf)
            Print(File2, Chr(27) & "V400" & Chr(27) & "H740" & Chr(27) & "XU" & barcode & vbCrLf)
            If lotControl Then
                Print(File2, Chr(27) & "V1000" & Chr(27) & "H320" & Chr(27) & "L0202" & Chr(27) & "XSPack ID" & vbCrLf)
                Print(File2, Chr(27) & "V1000" & Chr(27) & "H350" & Chr(27) & "BQ3009,1" & lotNO & vbCrLf)
                Print(File2, Chr(27) & "V1950" & Chr(27) & "H330" & Chr(27) & "L0404" & Chr(27) & "XB1" & lotNO & vbCrLf)
            End If
            Print(File2, Chr(27) & "Q1" & vbCrLf)
            Print(File2, Chr(27) & "Z" & vbCrLf)
            loopcount = loopcount - 1
        Loop
        packsRow = packsRow + 1
        FileClose(File2)

        Dim strTemp As String = Environment.GetEnvironmentVariable("temp")
        Dim strFtpFile As String = strTemp & "\ftp-cmd.txt"
        Dim WINDIR As String = Environment.GetEnvironmentVariable("windir")
        Dim exec As String = WINDIR & "\system32\ftp.exe"
        Dim strArgs As String = " -s:" & strFtpFile

        Dim startFTP = New ProcessStartInfo
        startFTP.WindowStyle = ProcessWindowStyle.Hidden
        startFTP.FileName = exec
        startFTP.Arguments = strArgs
        'Process.Start(startFTP)

    End Function

    Private Function GetBarcode(ByVal ProdID As String)
        Dim barcode As String

        If Len(intday.ToString) = 1 Then
            intday = "0" & intday
        End If
        Dim intMonth As String = currentDate.Month
        If Len(intMonth.ToString) = 1 Then
            intMonth = "0" & intMonth
        End If

        Dim todaySTR = intYear & intMonth & intday

        connectionString = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
        sql = "SELECT *" & _
              " FROM MITPOP" & _
              " WHERE MPITNO = '" & ProdID & "' AND MPALWT = 2 AND MPALWQ = 'EA13' AND MPVFDT < " & todaySTR & ";"

        connection = New SqlConnection(connectionString)

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds2.Tables.Add("MITPOP"))
            adapter.Dispose()
            command.Dispose()
            connection.Close()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        If Not ds2.Tables("MITPOP").Rows.Count = 0 Then
            barcode = ds2.Tables("MITPOP").Rows(0)("MPPOPN")
        Else
            barcode = "1234567"
            MessageBox.Show("No barcode available for this product")
        End If

        ds2.Tables.Remove("MITPOP")
        Return barcode


    End Function

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyData = Keys.Control + Keys.N Then
            tsbNewRun_Click(sender, e)
        End If
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub


    Private Function SaveRunPackRows()

        Dim dbConnString As String = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
        Try
            Using bcp As New SqlClient.SqlBulkCopy(dbConnString)
                bcp.DestinationTableName = "LabelPrinting"
                bcp.BatchSize = 100
                bcp.WriteToServer(ds.Tables("Packs"))
            End Using
        Catch ex As Exception
            sql = "UPDATE LabelPrinting SET perPack = '" & perpack & "', ShortPack = '" & shortpack & "', putaway = '" & putAway & "', lotNo = '" & lotNO & "'" & " WHERE (((ID)='" & ds.Tables("Packs").Rows(packsRow - 1)("ID") & "'));"
            connection = New SqlConnection(dbConnString)
            command = New SqlCommand(sql, connection)
            connection.Open()
            command.ExecuteNonQuery()
        End Try
    End Function

    Private Sub deleteRunLine(ByVal PurchNo As String, ByVal runNo As String, ByVal AttItemNo As String)
        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
        Try
            sql = "DELETE " & _
                  " FROM Attributes" & _
                  " WHERE ATTRIBUTE_ITEM='" & AttItemNo & "';"
            connection = New SqlConnection(connectionString)

            command = New SqlCommand(sql, connection)
            connection.Open()
            command.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class