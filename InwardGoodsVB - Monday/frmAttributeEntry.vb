Imports System.Data.SqlClient
Imports Lawson.M3.MvxSock

Public Class frmAttributeEntry
    Private _dt As DataTable
    Private _isThisAnEdit As String
    Private _supPackID As String
    Private _dsLineItems As DataSet
    Private _RUN_NUMBER As String
    Private _lineloop As Integer
    Dim duplicatePack As Integer
    Dim ID As String
    Dim connection As SqlConnection
    Dim command As SqlCommand
    Dim sql As String
    Private frmPurchaseOrder As frmPO
    Dim connectionString As String
    Dim dsLine As New DataSet
    Dim dsAttributeValuesbySuppPack As New DataSet
    Dim adapter As New SqlDataAdapter
    Dim tableNo As Integer
    Dim lwrLmt As Integer
    Dim upper As Integer
    Dim tblNmeOne As Object
    Dim tblNameTwo As Object
    Dim tableName As Object
    Dim outup As Integer
    Dim outlow As Integer
    Dim subLine As String
    Dim qtyTotal As Integer
    Dim LINE_ID As String
    Dim po As String
    Dim co As String
    Dim distro As String
    Dim lineNumber As String
    Dim duplicatePacks As Integer
    Dim itemQty As Integer
    Dim labQty As Integer
    Dim appCharge As Decimal
    Dim packQty As Integer
    Dim attributeValue As String
    Dim lotcontrol As Integer
    Public attributeTable As DataTable = New DataTable("Attribute Table")
    Dim strWrk, strWrk1, strWrk2 As String
    Dim runNumber As Integer
    Dim frmAttEnt As frmAttributeEntry
    Dim runNumberKey As String
    Dim packLoop As Integer
    Dim max As Integer
    Dim delDockNO
    Dim rowNumber As Integer
    Dim addCatchTally As Integer = 0
    Dim creation = 0
    Dim ProductName As String
    Dim itemNumber As String
    Public Shared lineloop As Integer
    Dim clickcmd As Integer
    Dim dtTempAttribute As New DataTable
    Dim remPack As Integer
    Dim totPacks As Integer
    Dim dtEditStdPack As New DataTable
    Dim catchWeight As Decimal
    Dim tally As String = ""
    Dim attID As String
    Dim attQty As String
    Dim athRowNo As Integer
    Dim exitform As Boolean = False
    Dim dd As New frmInput
    Dim cleanupExit As Boolean = False
    Dim batchNo As String
    Private _igType As String
    Dim M3Svr As M3Point.M3Point = New M3Point.M3Point
    Dim warehouse As String
    Dim productDesc As String
    Dim typeFlag As Object


    Public Property dt As DataTable
        Get
            Return _dt
        End Get
        Set(value As DataTable)
            _dt = value
        End Set
    End Property
    Public Property isThisAnEdit As Boolean
        Get
            Return _isThisAnEdit
        End Get
        Set(value As Boolean)
            _isThisAnEdit = value
        End Set
    End Property
    Public Property supPackID As String
        Get
            Return _supPackID
        End Get
        Set(value As String)
            _supPackID = value
        End Set
    End Property
    Public Property dsLineItems As DataSet
        Get
            Return _dsLineItems
        End Get
        Set(value As DataSet)
            _dsLineItems = value
        End Set
    End Property
    Public Property RUN_NUMBER As String
        Get
            Return _RUN_NUMBER
        End Get
        Set(value As String)
            _RUN_NUMBER = value
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


    Public Sub New(ByVal dt As DataTable, ByVal supPackID As String, ByVal isThisAnEdit As Boolean, ByVal dsLineItems As DataSet, ByVal RUN_NUMBER As String, ByVal igType As String)
        InitializeComponent()
        lblPacks.Location = New Point(Me.Width / 2 - lblPacks.Width, lblPacks.Location.Y)

        If Not isThisAnEdit Then
            dd.ShowDialog("Delivery Docket Number", "Please enter the Delivery Docket number", "", delDockNO, False, False)
            If Not delDockNO = "" Then
                packQty = dsLineItems.Tables(0).Rows(lineloop)("Packs Received")
                remPack = packQty - 1
                If Not packQty = 0 Then
                    lblPacks.Text = "Pack number " & packLoop + 1 & " of " & packQty
                End If
            Else
                Me.Close()
            End If
        Else
            btnConfirm.Enabled = False
            lblPacks.Visible = False
            btnCancel.Visible = True
            delDockNO = "edit"
        End If

        If Not delDockNO = "" Then
            Me.dt = dt
            Me.isThisAnEdit = isThisAnEdit
            Me.dsLineItems = dsLineItems
            Me.RUN_NUMBER = RUN_NUMBER
            Me.supPackID = supPackID
            Me.igType = igType
            LoadAttributeTable()
        End If
    End Sub


    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) Handles btnConfirm.Click
        Dim dbConnString As String = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"

        If Me.isThisAnEdit Then

            If dsAttributeValuesbySuppPack.Tables.Contains("TempSTD") Then
                'For i = 0 To dsAttributeValuesbySuppPack.Tables("TempSTD").Rows.Count - 1
                For i = 0 To dgvAttributeEntry.Rows.Count - 1
                    sql = "UPDATE Attributes SET PACKQTY = '" & dgvAttributeEntry.Rows(i).Cells("PackQty").Value & "', ITEMQTY = '" & dgvAttributeEntry.Rows(i).Cells("ItemQty").Value & "' WHERE (((LINE_ID)='" & dsAttributeValuesbySuppPack.Tables("Attributes").Rows(i)("LINE_ID") & "'));"
                    connection = New SqlConnection(dbConnString)
                    Try
                        command = New SqlCommand(sql, connection)
                        connection.Open()
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        'MessageBox.Show("here he is")
                    End Try
                Next
                Me.Close()
            Else
                For i = 0 To dsAttributeValuesbySuppPack.Tables(cboSuppPacks.SelectedValue.ToString).Rows.Count - 1
                    CalcCatchweight()
                    Dim test = catchWeight
                    Dim test2 = tally

                    If i = 0 Then
                        sql = "UPDATE Attributes SET CATCHWEIGHT = '" & catchWeight & "', TALLY = '" & tally & "' WHERE (((ID)=" & (dsAttributeValuesbySuppPack.Tables(cboSuppPacks.SelectedValue.ToString).Rows(i)("ID")) - 1 & "));"
                        connection = New SqlConnection(dbConnString)
                        Try
                            command = New SqlCommand(sql, connection)
                            connection.Open()
                            command.ExecuteNonQuery()
                        Catch ex As Exception
                            'MessageBox.Show("here he is")
                        End Try
                    End If

                    sql = "UPDATE Attributes SET ATTRIBUTE_QTY = '" & dsAttributeValuesbySuppPack.Tables(cboSuppPacks.SelectedValue.ToString).Rows(i)("ATTRIBUTE_QTY") & "' WHERE (((ID)=" & dsAttributeValuesbySuppPack.Tables(cboSuppPacks.SelectedValue.ToString).Rows(i)("ID") & "));"
                    connection = New SqlConnection(dbConnString)
                    Try
                        command = New SqlCommand(sql, connection)
                        connection.Open()
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        'MessageBox.Show("here he is")
                    End Try
                Next
                Me.Close()
            End If
        Else

            If Not dsLineItems.Tables("Attribute Table").Columns.Contains("CatchWeight") Then
                dsLineItems.Tables("Attribute Table").Columns.Add("CatchWeight")
                dsLineItems.Tables("Attribute Table").Columns.Add("Tally")
                dsLineItems.Tables("Attribute Table").Columns.Add("PackQty")
                dsLineItems.Tables("Attribute Table").Columns.Add("ItemQty")
                dsLineItems.Tables("Attribute Table").Columns.Add("PackType")

                addCatchTally = 1
            End If

            CalcCatchWeight()

            dsLineItems.Tables("Attribute Table").Rows(0)("CatchWeight") = catchWeight
            dsLineItems.Tables("Attribute Table").Rows(0)("Tally") = tally
            dsLineItems.Tables("Attribute Table").Rows(0)("PackQty") = 1
            dsLineItems.Tables("Attribute Table").Rows(0)("ItemQty") = 1
            dsLineItems.Tables("Attribute Table").Rows(0)("PackType") = attributeValue
            If IsDBNull(attributeValue) Then
                attributeValue = "STANDARD"
            End If
            dgvAttributeEntry.Columns("CatchWeight").Visible = False
            dgvAttributeEntry.Columns("Tally").Visible = False
            dgvAttributeEntry.Columns("PackQty").Visible = False
            dgvAttributeEntry.Columns("ItemQty").Visible = False
            dgvAttributeEntry.Columns("PackType").Visible = False

            Using bcp As New SqlClient.SqlBulkCopy(dbConnString)
                bcp.DestinationTableName = "Attributes"
                bcp.BatchSize = 100
                bcp.WriteToServer(dsLineItems.Tables("Attribute Table"))
            End Using

            If packLoop < packQty - 1 Then
                If attributeValue.Contains("MIXED") Then

                    duplicatePack = MessageBox.Show("Does the next pack have the same Tally Information as this pack?", "Duplicate Pack?", MessageBoxButtons.YesNo)
                    If duplicatePack = 6 Then
                        dd.ShowDialog("Supplier Pack Number", "Please enter the Supplier Pack Number for the next pack of " & System.Environment.NewLine & ProductName, "", supPackID, False, False)
                        If Not supPackID = "" Then
                            dsLineItems.Tables("Attribute Table").Rows(0)("AEQTY") = supPackID
                            dgvAttributeEntry.Refresh()
                        Else
                            MessageBox.Show("You have pressed cancel, nothing has been saved.")
                            cleanupExit = True
                            GoTo exitRoutine
                        End If
                    Else
                        dd.ShowDialog("Supplier Pack Number", "Please enter the Supplier Pack Number for the next pack of" & System.Environment.NewLine & ProductName, "", supPackID, False, False)
                        If not supPackID = "" then
                            dsLineItems.Tables("Attribute Table").Rows(0)("AEQTY") = supPackID
                            For i = 1 To dsLineItems.Tables("Attribute Table").Rows.Count - 1
                                dsLineItems.Tables("Attribute Table").Rows(i)("AEQTY") = dsLineItems.Tables("Attribute Original" & lineloop).Rows(i)("AEQTY")
                            Next
                            dgvAttributeEntry.Refresh()
                        Else
                            MessageBox.Show("You have pressed cancel, nothing has been saved.")
                            cleanupExit = True
                            GoTo exitRoutine
                        End If
                    End If
                    packLoop = packLoop + 1
                ElseIf attributeValue.Contains("FIXED") Then
                    packLoop = packLoop + 1
                    If Not remPack = 1 Then
                        HowManyPacks()
                    End If
                End If
            Else
                packLoop = 0
                If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                    lineloop = lineloop + 1
                    LoadAttributeTable()
                Else
                    If lineloop = dsLineItems.Tables(0).Rows.Count - 1 Then
                        MessageBox.Show("All packs entered", "Packs Entered", MessageBoxButtons.OK, MessageBoxIcon.Information)
exitRoutine:
                        lineloop = 0
                        If cleanupExit Then
                            deleteRun(po, RUN_NUMBER)
                        End If
                        Me.Close()
                        If igType = "CO" Then
                            frmPurchaseOrder = New frmPO(co, Convert.ToInt32(subLine) + 1, igType)
                        Else
                            frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1, igType)
                        End If

                        frmPurchaseOrder.Show()
                        frmPurchaseOrder.MdiParent = frmMain
                    End If
                End If
            End If
            dsLine.Tables("QtySaved").Clear()
        End If
        lblPacks.Text = "Pack number " & packLoop + 1 & " of " & packQty
    End Sub

    Public Function HowManyPacks()
        Try
tryagain:
            dd.ShowDialog("Duplicate Packs", "How many of the " & remPack & " remaining packs of " & System.Environment.NewLine & ProductName & System.Environment.NewLine & " are the same as this ?", "", totPacks, False, False)
            'totPacks = InputBox("How many of the " & remPack & " remaining packs of " & ProductName & " are the same as this ?")
            Dim dbConnString As String = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"

            If totPacks > packQty - 1 Then
                MessageBox.Show("A number greater than the number of packs being received has been entered, re-enter a number " & remPack & " or less.")
                GoTo tryagain
            End If
            For i = 0 To totPacks - 1
                Using bcp As New SqlClient.SqlBulkCopy(dbConnString)
                    bcp.DestinationTableName = "Attributes"
                    bcp.BatchSize = 100
                    bcp.WriteToServer(dsLineItems.Tables("Attribute Table"))
                End Using
                remPack = remPack - 1
                packLoop = packLoop + 1
            Next
        Catch ex As Exception
            MessageBox.Show("Incorrect value entered! Please re-enter")
            GoTo tryagain
        End Try

        If remPack = 0 Then
            If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                packLoop = 0
                lineloop = lineloop + 1
                LoadAttributeTable()
            Else
                If lineloop = dsLineItems.Tables(0).Rows.Count - 1 Then
                    MessageBox.Show("All packs entered", "Packs Entered", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    lineloop = 0
                    Me.Close()
                    If igType = "CO" Then
                        frmPurchaseOrder = New frmPO(co, Convert.ToInt32(subLine) + 1, igType)
                    Else
                        frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1, igType)
                    End If
                    frmPurchaseOrder.Show()
                    frmPurchaseOrder.MdiParent = frmMain
                End If
            End If
        End If
    End Function

    Public Function RunSQL(ByVal conString As String, ByVal sql As String, tableName As String)
        connection = New SqlConnection(conString)
        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            If Not dsLineItems.Tables.Contains(tableName) Then
                adapter.Fill(dsLineItems.Tables.Add(tableName))
            Else
                adapter.Fill(dsLineItems.Tables(tableName))
            End If
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Function

    Private Sub cboSuppPacks_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSuppPacks.SelectedIndexChanged
        dgvAttributeEntry.DataSource = dsAttributeValuesbySuppPack.Tables(cboSuppPacks.SelectedValue.ToString)

        Dim selectedRun = Convert.ToInt32(cboSuppPacks.SelectedIndex)
        If Not selectedRun = 0 Then
            rowNumber = dsAttributeValuesbySuppPack.Tables(cboSuppPacks.SelectedValue.ToString).Rows(0)("ROWNUMBER")
            lblProduct.Text = dsLineItems.Tables(0).Rows(rowNumber)("MMFUDS")
            dgvAttributeEntry.Columns("ID").Visible = False
            dgvAttributeEntry.Columns("ATTRIBUTE_ITEM").Visible = False
            dgvAttributeEntry.Columns("LINE_ID").Visible = False
            dgvAttributeEntry.Columns("RUN_NUMBER").Visible = False
            dgvAttributeEntry.Columns("ROWNUMBER").Visible = False
            dgvAttributeEntry.Columns(3).HeaderText = "Qty"
            dgvAttributeEntry.Columns(1).HeaderText = "Value"
            dgvAttributeEntry.Columns(1).ReadOnly = True
            btnConfirm.Enabled = True
        Else
            btnConfirm.Enabled = False
        End If
    End Sub

   
    Public Function LoadAttributeTable()
        Dim dtAttributeOriginal As New DataTable

        ' query the M3 database for the product details and fill the dataset, for the purposes of getting the attribute value and handling it accordingly.
        connectionString = M3Svr.ConnString(frmMain.grid)
        sql = "SELECT * FROM MITMAS WHERE MMITNO = '" & dsLineItems.Tables(0).Rows(lineloop)("IBITNO") & "';"
        connection = New SqlConnection(connectionString)
        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            If dsLineItems.Tables.Contains("Product Table") Then
                adapter.Fill(dsLineItems.Tables("Product Table"))
            Else
                adapter.Fill(dsLineItems.Tables.Add("Product Table"))
            End If
            adapter.Dispose()
            command.Dispose()
            connection.Close()
            attributeValue = Trim(dsLineItems.Tables("Product Table").Rows(lineloop)("MMATMO"))
            If attributeValue = "" Then
                Select Case Trim(dsLineItems.Tables("Product Table").Rows(lineloop)("MMITTY"))
                    Case "G10"
                        attributeValue = "EXPIRY"
                    Case "G18"
                        attributeValue = "BATCH"
                    Case Else
                        attributeValue = "STANDARD"
                End Select
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try


        If Not isThisAnEdit Then
            'the below code deals with a run which is being created.

            'Fill variables with data from the relevant dataset

            LINE_ID = Trim(dsLineItems.Tables(0).Rows(lineloop)("IBPUNO")) & dsLineItems.Tables(0).Rows(lineloop)("IBPNLI") & dsLineItems.Tables(0).Rows(lineloop)("IBPNLS")
            If igType = "PO" Then
                po = Trim(dsLineItems.Tables(0).Rows(lineloop)("IBPUNO"))
                lineNumber = dsLineItems.Tables(0).Rows(lineloop)("IBPNLI")
                subLine = dsLineItems.Tables(0).Rows(lineloop)("IBPNLS")
                warehouse = dsLineItems.Tables(0).Rows(lineloop)("IBWHLO")

            ElseIf igType = "CO" Then
                po = Trim(dsLineItems.Tables(0).Rows(lineloop)("ICPUNO"))
                co = Trim(dsLineItems.Tables(0).Rows(lineloop)("IBPUNO"))
                lineNumber = dsLineItems.Tables(0).Rows(lineloop)("ICPNLI")
                subLine = dsLineItems.Tables(0).Rows(lineloop)("IBPNLS")
                qtyTotal = dsLineItems.Tables(0).Rows(lineloop)("ICRPQA")
                warehouse = dsLineItems.Tables(0).Rows(lineloop)("IBWHLO")
            ElseIf igType = "DO" Then
                distro = Trim(dsLineItems.Tables(0).Rows(lineloop)("IBPUNO"))
                po = Trim(dsLineItems.Tables(0).Rows(lineloop)("IBPUNO"))
                lineNumber = dsLineItems.Tables(0).Rows(lineloop)("IBPNLI")
                subLine = dsLineItems.Tables(0).Rows(lineloop)("IBPNLS")
                warehouse = dsLineItems.Tables(0).Rows(lineloop)("IBWHLO")
            End If

            typeFlag = dsLineItems.Tables(0).Rows(lineloop)("MMEVGR")
            productDesc = dsLineItems.Tables(0).Rows(lineloop)("MMITDS")
            itemNumber = dsLineItems.Tables(0).Rows(lineloop)("IBITNO")
            ProductName = dsLineItems.Tables(0).Rows(lineloop)("MMFUDS")
            itemQty = dsLineItems.Tables(0).Rows(lineloop)("Qty Received")
            If dsLineItems.Tables(0).Columns.Contains("LabelQty") Then
                labQty = dsLineItems.Tables(0).Rows(lineloop)("LabelQty")
                appCharge = dsLineItems.Tables(0).Rows(lineloop)("AppCharge")
            End If
            packQty = dsLineItems.Tables(0).Rows(lineloop)("Packs Received")
            lblProduct.Text = ProductName
            remPack = packQty - 1

            If packLoop = 0 And lineloop = 0 Then
                dsLine.Tables.Add("qtySaved")
            End If

            If igType = "DO" Then


                If lineloop = 0 Then
                    'write into the Gunnersen database the details of the run
                    connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                    If igType = "PO" Then
                        sql = "SELECT * FROM LineReceiving WHERE PO = '" & po & lineNumber & subLine & "';"
                    ElseIf igType = "DO" Then
                        sql = "SELECT * FROM LineReceiving WHERE PO = '" & distro & lineNumber & subLine & "';"
                    ElseIf igType = "CO" Then
                        sql = "SELECT * FROM LineReceiving WHERE PO = '" & co & lineNumber & subLine & "';"
                    End If

                    RunSQL(connectionString, sql, "Run List")

                    max = dsLineItems.Tables("Run List").Rows.Count - 1
                    If max = -1 Then
                        runNumber = 0
                    Else
                        runNumber = dsLineItems.Tables("Run List").Rows.Count + 1
                    End If

                    connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                    sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & distro & RUN_NUMBER & "','" & RUN_NUMBER & "','" & distro & lineNumber & subLine & "','" & DateTime.Now & "','" & distro & "','" & delDockNO & "');"
                    connection = New SqlConnection(connectionString)
                    Try
                        command = New SqlCommand(sql, connection)
                        connection.Open()
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        MessageBox.Show(ex.ToString)
                    End Try
                End If

                If itemQty > 0 Then

                    'write into the Gunnersen database the details of the line
                    connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                    sql = "INSERT INTO dbo.Attributes(ATTRIBUTE_VALUE, LINE_ID, RUN_NUMBER, ROWNUMBER, PACKQTY, ITEMQTY, PACKTYPE, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag) Values ('" & "STD_PACK" & "', '" & distro & lineNumber & subLine & "','" & distro & RUN_NUMBER & "','" & lineloop & "','" & packQty & "','" & itemQty & "','" & attributeValue & "','" & labQty & "','" & appCharge & "', '" & warehouse & "', '" & itemNumber & "', '" & productDesc & "', '" & typeFlag & "');"

                    connection = New SqlConnection(connectionString)
                    Try
                        command = New SqlCommand(sql, connection)
                        connection.Open()
                        command.ExecuteNonQuery()
                    Catch ex As Exception
                        MessageBox.Show(ex.ToString)
                    End Try

                    If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                        lineloop = lineloop + 1
                        LoadAttributeTable()
                    Else
                        MessageBox.Show("All packs entered", "Packs Entered", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        lineloop = 0
                        Me.Close()
                        If igType = "CO" Then
                            frmPurchaseOrder = New frmPO(co, Convert.ToInt32(subLine) + 1, igType)
                        Else
                            frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1, igType)
                        End If
                        frmPurchaseOrder.Show()
                        frmPurchaseOrder.MdiParent = frmMain
                    End If
                Else
                    If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                        lineloop = lineloop + 1
                        LoadAttributeTable()
                    Else
                        MessageBox.Show("All packs entered", "Packs Entered", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        lineloop = 0
                        Me.Close()
                        If igType = "CO" Then
                            frmPurchaseOrder = New frmPO(co, Convert.ToInt32(subLine) + 1, igType)
                        Else
                            frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1, igType)
                        End If
                        frmPurchaseOrder.Show()
                        frmPurchaseOrder.MdiParent = frmMain
                    End If
                End If
            Else




                Select Case attributeValue
                    Case "BATCH"
                        If lineloop = 0 Then

                            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                            If igType = "PO" Then
                                sql = "SELECT * FROM LineReceiving WHERE PO = '" & po & lineNumber & subLine & "';"
                            ElseIf igType = "DO" Then
                                sql = "SELECT * FROM LineReceiving WHERE PO = '" & distro & lineNumber & subLine & "';"
                            ElseIf igType = "CO" Then
                                sql = "SELECT * FROM LineReceiving WHERE PO = '" & co & lineNumber & subLine & "';"
                            End If

                            RunSQL(connectionString, sql, "Run List")

                            max = dsLineItems.Tables("Run List").Rows.Count - 1
                            If max = -1 Then
                                runNumber = 0
                            Else
                                runNumber = dsLineItems.Tables("Run List").Rows.Count + 1
                            End If

                            'write into the Gunnersen database the details of the run
                            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                            If igType = "PO" Then
                                sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & po & RUN_NUMBER & "','" & RUN_NUMBER & "','" & po & lineNumber & subLine & "','" & DateTime.Now & "','" & po & "','" & delDockNO & "');"
                            ElseIf igType = "CO" Then
                                sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & co & RUN_NUMBER & "','" & RUN_NUMBER & "','" & co & lineNumber & subLine & "','" & DateTime.Now & "','" & co & "','" & delDockNO & "');"
                            ElseIf igType = "DO" Then
                                sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & distro & RUN_NUMBER & "','" & RUN_NUMBER & "','" & distro & lineNumber & subLine & "','" & DateTime.Now & "','" & distro & "','" & delDockNO & "');"
                            End If
                            connection = New SqlConnection(connectionString)
                            Try
                                command = New SqlCommand(sql, connection)
                                connection.Open()
                                command.ExecuteNonQuery()
                            Catch ex As Exception
                                MessageBox.Show(ex.ToString)
                            End Try
                        End If

                        If itemQty > 0 Then
                            For i = 0 To packQty - 1
                                dd.ShowDialog("Batch Number", "Please enter the batch number for " & System.Environment.NewLine & ProductName, "", batchNo, False, False)
                                batchNo = batchNo.ToUpper
                                If Not batchNo = "" Then
                                    lblPacks.Text = "Pack number " & lineloop & " of " & packQty
                                    lotcontrol = 1
                                Else
                                    MessageBox.Show("You have pressed cancel, nothing has been saved.", "Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    cleanupExit = True
                                    GoTo ExitRoutine
                                End If
                                'write into the Gunnersen database the details of the line
                                connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                                If igType = "PO" Then
                                    sql = "INSERT INTO dbo.Attributes(ATTRIBUTE_VALUE, LINE_ID, RUN_NUMBER, ROWNUMBER, PACKQTY, ITEMQTY, PACKTYPE, TALLY, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag) Values ('" & "STD_PACK" & "', '" & po & lineNumber & subLine & "','" & po & RUN_NUMBER & "','" & lineloop & "','" & packQty & "','" & itemQty & "','" & attributeValue & "','" & batchNo & "','" & labQty & "','" & appCharge & "', '" & warehouse & "', '" & itemNumber & "', '" & productDesc & "', '" & typeFlag & "');"
                                ElseIf igType = "CO" Then
                                    sql = "INSERT INTO dbo.Attributes(ATTRIBUTE_VALUE, LINE_ID, RUN_NUMBER, ROWNUMBER, PACKQTY, ITEMQTY, PACKTYPE, TALLY, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag) Values ('" & "STD_PACK" & "', '" & co & lineNumber & subLine & "','" & co & RUN_NUMBER & "','" & lineloop & "','" & packQty & "','" & itemQty & "','" & attributeValue & "','" & batchNo & "','" & labQty & "','" & appCharge & "', '" & warehouse & "', '" & itemNumber & "', '" & productDesc & "', '" & typeFlag & "');"
                                ElseIf igType = "DO" Then
                                    sql = "INSERT INTO dbo.Attributes(ATTRIBUTE_VALUE, LINE_ID, RUN_NUMBER, ROWNUMBER, PACKQTY, ITEMQTY, PACKTYPE, TALLY, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag) Values ('" & "STD_PACK" & "', '" & distro & lineNumber & subLine & "','" & distro & RUN_NUMBER & "','" & lineloop & "','" & packQty & "','" & itemQty & "','" & attributeValue & "','" & batchNo & "','" & labQty & "','" & appCharge & "', '" & warehouse & "', '" & itemNumber & "', '" & productDesc & "', '" & typeFlag & "');"
                                End If

                                connection = New SqlConnection(connectionString)
                                Try
                                    command = New SqlCommand(sql, connection)
                                    connection.Open()
                                    command.ExecuteNonQuery()
                                Catch ex As Exception
                                    MessageBox.Show(ex.ToString)
                                End Try

                                If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                                    lineloop = lineloop + 1
                                    LoadAttributeTable()
                                Else
                                    MessageBox.Show("All items entered", "Items Entered", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                    lineloop = 0
                                    Me.Close()

                                    If igType = "CO" Then
                                        frmPurchaseOrder = New frmPO(co, Convert.ToInt32(subLine) + 1, igType)
                                    Else
                                        frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1, igType)
                                    End If

                                    frmPurchaseOrder.Show()
                                    frmPurchaseOrder.MdiParent = frmMain
                                End If
                            Next

                        Else
                            If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                                lineloop = lineloop + 1
                                LoadAttributeTable()
                            Else
                                MessageBox.Show("All items entered", "Items Entered", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                lineloop = 0
                                Me.Close()

                                If igType = "CO" Then
                                    frmPurchaseOrder = New frmPO(co, Convert.ToInt32(subLine) + 1, igType)
                                Else
                                    frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1, igType)
                                End If


                                frmPurchaseOrder.Show()
                                frmPurchaseOrder.MdiParent = frmMain
                            End If
                        End If

                    Case "EXPIRY"
                        If lineloop = 0 Then

                            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                            If igType = "PO" Then
                                sql = "SELECT * FROM LineReceiving WHERE PO = '" & po & lineNumber & subLine & "';"
                            ElseIf igType = "DO" Then
                                sql = "SELECT * FROM LineReceiving WHERE PO = '" & distro & lineNumber & subLine & "';"
                            ElseIf igType = "CO" Then
                                sql = "SELECT * FROM LineReceiving WHERE PO = '" & co & lineNumber & subLine & "';"
                            End If

                            RunSQL(connectionString, sql, "Run List")

                            max = dsLineItems.Tables("Run List").Rows.Count - 1
                            If max = -1 Then
                                runNumber = 0
                            Else
                                runNumber = dsLineItems.Tables("Run List").Rows.Count + 1
                            End If

                            'write into the Gunnersen database the details of the run
                            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"

                            If igType = "PO" Then
                                sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & po & RUN_NUMBER & "','" & RUN_NUMBER & "','" & po & lineNumber & subLine & "','" & DateTime.Now & "','" & po & "','" & delDockNO & "');"
                            ElseIf igType = "CO" Then
                                sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & co & RUN_NUMBER & "','" & RUN_NUMBER & "','" & co & lineNumber & subLine & "','" & DateTime.Now & "','" & co & "','" & delDockNO & "');"
                            ElseIf igType = "DO" Then
                                sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & distro & RUN_NUMBER & "','" & RUN_NUMBER & "','" & distro & lineNumber & subLine & "','" & DateTime.Now & "','" & distro & "','" & delDockNO & "');"
                            End If

                            connection = New SqlConnection(connectionString)
                            Try
                                command = New SqlCommand(sql, connection)
                                connection.Open()
                                command.ExecuteNonQuery()
                            Catch ex As Exception
                                MessageBox.Show(ex.ToString)
                            End Try
                        End If

                        If itemQty > 0 Then

                            dd.ShowDialog("Batch Number", "Please enter the manufactured date for " & System.Environment.NewLine & ProductName, batchNo, batchNo, False, True)
                            If Not batchNo = "" Then
                                lblPacks.Text = "Pack number " & lineloop & " of " & packQty
                                lotcontrol = 1
                            Else
                                MessageBox.Show("You have pressed cancel, nothing has been saved.")
                                cleanupExit = True
                                GoTo ExitRoutine
                            End If

                            batchNo = Convert.ToDateTime(batchNo).ToString("yyyMMdd")

                            'write into the Gunnersen database the details of the line
                            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                            If igType = "PO" Then
                                sql = "INSERT INTO dbo.Attributes(ATTRIBUTE_VALUE, LINE_ID, RUN_NUMBER, ROWNUMBER, PACKQTY, ITEMQTY, PACKTYPE, TALLY, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag) Values ('" & "STD_PACK" & "', '" & po & lineNumber & subLine & "','" & po & RUN_NUMBER & "','" & lineloop & "','" & packQty & "','" & itemQty & "','" & attributeValue & "','" & batchNo & "','" & labQty & "','" & appCharge & "', '" & warehouse & "', '" & itemNumber & "', '" & productDesc & "', '" & typeFlag & "');"
                            ElseIf igType = "CO" Then
                                sql = "INSERT INTO dbo.Attributes(ATTRIBUTE_VALUE, LINE_ID, RUN_NUMBER, ROWNUMBER, PACKQTY, ITEMQTY, PACKTYPE, TALLY, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag) Values ('" & "STD_PACK" & "', '" & co & lineNumber & subLine & "','" & co & RUN_NUMBER & "','" & lineloop & "','" & packQty & "','" & itemQty & "','" & attributeValue & "','" & batchNo & "','" & labQty & "','" & appCharge & "', '" & warehouse & "', '" & itemNumber & "', '" & productDesc & "', '" & typeFlag & "');"
                            ElseIf igType = "DO" Then
                                sql = "INSERT INTO dbo.Attributes(ATTRIBUTE_VALUE, LINE_ID, RUN_NUMBER, ROWNUMBER, PACKQTY, ITEMQTY, PACKTYPE, TALLY, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag) Values ('" & "STD_PACK" & "', '" & distro & lineNumber & subLine & "','" & distro & RUN_NUMBER & "','" & lineloop & "','" & packQty & "','" & itemQty & "','" & attributeValue & "','" & batchNo & "','" & labQty & "','" & appCharge & "', '" & warehouse & "', '" & itemNumber & "', '" & productDesc & "', '" & typeFlag & "');"
                            End If

                            connection = New SqlConnection(connectionString)
                            Try
                                command = New SqlCommand(sql, connection)
                                connection.Open()
                                command.ExecuteNonQuery()
                            Catch ex As Exception
                                MessageBox.Show(ex.ToString)
                            End Try

                            If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                                lineloop = lineloop + 1
                                LoadAttributeTable()
                            Else
                                MessageBox.Show("All items entered", "Items Entered", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                lineloop = 0
                                Me.Close()

                                If igType = "CO" Then
                                    frmPurchaseOrder = New frmPO(co, Convert.ToInt32(subLine) + 1, igType)
                                Else
                                    frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1, igType)
                                End If

                                frmPurchaseOrder.Show()
                                frmPurchaseOrder.MdiParent = frmMain
                            End If
                        Else
                            If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                                lineloop = lineloop + 1
                                LoadAttributeTable()
                            Else
                                MessageBox.Show("All items entered", "Items Entered", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                lineloop = 0
                                Me.Close()

                                If igType = "CO" Then
                                    frmPurchaseOrder = New frmPO(co, Convert.ToInt32(subLine) + 1, igType)
                                Else
                                    frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1, igType)
                                End If


                                frmPurchaseOrder.Show()
                                frmPurchaseOrder.MdiParent = frmMain
                            End If
                        End If

                    Case "STANDARD"

                        If lineloop = 0 Then

                            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                            If igType = "PO" Then
                                sql = "SELECT * FROM LineReceiving WHERE PO = '" & po & lineNumber & subLine & "';"
                            ElseIf igType = "DO" Then
                                sql = "SELECT * FROM LineReceiving WHERE PO = '" & distro & lineNumber & subLine & "';"
                            ElseIf igType = "CO" Then
                                sql = "SELECT * FROM LineReceiving WHERE PO = '" & co & lineNumber & subLine & "';"
                            End If

                            RunSQL(connectionString, sql, "Run List")

                            max = dsLineItems.Tables("Run List").Rows.Count - 1
                            If max = -1 Then
                                runNumber = 0
                            Else
                                runNumber = dsLineItems.Tables("Run List").Rows.Count + 1
                            End If

                            'write into the Gunnersen database the details of the run
                            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                            If igType = "PO" Then
                                sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & po & RUN_NUMBER & "','" & RUN_NUMBER & "','" & po & lineNumber & subLine & "','" & DateTime.Now & "','" & po & "','" & delDockNO & "');"
                            ElseIf igType = "CO" Then
                                sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & co & RUN_NUMBER & "','" & RUN_NUMBER & "','" & co & lineNumber & subLine & "','" & DateTime.Now & "','" & co & "','" & delDockNO & "');"
                            ElseIf igType = "DO" Then
                                sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & distro & RUN_NUMBER & "','" & RUN_NUMBER & "','" & distro & lineNumber & subLine & "','" & DateTime.Now & "','" & distro & "','" & delDockNO & "');"
                            End If

                            connection = New SqlConnection(connectionString)
                            Try
                                command = New SqlCommand(sql, connection)
                                connection.Open()
                                command.ExecuteNonQuery()
                            Catch ex As Exception
                                MessageBox.Show(ex.ToString)
                            End Try
                        else

                        End If

                        If itemQty > 0 Then

                            'write into the Gunnersen database the details of the line
                            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                            If igType = "PO" Then
                                sql = "INSERT INTO dbo.Attributes(ATTRIBUTE_VALUE, LINE_ID, RUN_NUMBER, ROWNUMBER, PACKQTY, ITEMQTY, PACKTYPE, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag) Values ('" & "STD_PACK" & "', '" & po & lineNumber & subLine & "','" & po & RUN_NUMBER & "','" & lineloop & "','" & packQty & "','" & itemQty & "','" & attributeValue & "','" & labQty & "','" & appCharge & "', '" & warehouse & "', '" & itemNumber & "', '" & productDesc & "', '" & typeFlag & "');"
                            ElseIf igType = "CO" Then
                                sql = "INSERT INTO dbo.Attributes(ATTRIBUTE_VALUE, LINE_ID, RUN_NUMBER, ROWNUMBER, PACKQTY, ITEMQTY, PACKTYPE, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag) Values ('" & "STD_PACK" & "', '" & co & lineNumber & subLine & "','" & co & RUN_NUMBER & "','" & lineloop & "','" & packQty & "','" & itemQty & "','" & attributeValue & "','" & labQty & "','" & appCharge & "', '" & warehouse & "', '" & itemNumber & "', '" & productDesc & "', '" & typeFlag & "');"
                            ElseIf igType = "DO" Then
                                sql = "INSERT INTO dbo.Attributes(ATTRIBUTE_VALUE, LINE_ID, RUN_NUMBER, ROWNUMBER, PACKQTY, ITEMQTY, PACKTYPE, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag) Values ('" & "STD_PACK" & "', '" & distro & lineNumber & subLine & "','" & distro & RUN_NUMBER & "','" & lineloop & "','" & packQty & "','" & itemQty & "','" & attributeValue & "','" & labQty & "','" & appCharge & "', '" & warehouse & "', '" & itemNumber & "', '" & productDesc & "', '" & typeFlag & "');"
                            End If

                            connection = New SqlConnection(connectionString)
                            Try
                                command = New SqlCommand(sql, connection)
                                connection.Open()
                                command.ExecuteNonQuery()
                            Catch ex As Exception
                                MessageBox.Show(ex.ToString)
                            End Try

                            If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                                lineloop = lineloop + 1
                                LoadAttributeTable()
                            Else
                                MessageBox.Show("All packs entered", "Packs Entered", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                lineloop = 0
                                Me.Close()
                                If igType = "CO" Then
                                    frmPurchaseOrder = New frmPO(co, Convert.ToInt32(subLine) + 1, igType)
                                Else
                                    frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1, igType)
                                End If
                                frmPurchaseOrder.Show()
                                frmPurchaseOrder.MdiParent = frmMain
                            End If
                        Else
                            If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                                lineloop = lineloop + 1
                                LoadAttributeTable()
                            Else
                                MessageBox.Show("All packs entered", "Packs Entered", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                lineloop = 0
                                Me.Close()
                                If igType = "CO" Then
                                    frmPurchaseOrder = New frmPO(co, Convert.ToInt32(subLine) + 1, igType)
                                Else
                                    frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1, igType)
                                End If
                                frmPurchaseOrder.Show()
                                frmPurchaseOrder.MdiParent = frmMain
                            End If
                        End If


                    Case Else ' put case else here
                        ' check that the packqty for the current row isn't 0, if it is run the routine, and go to the next row
                        If packQty <> 0 Then
                            If attributeValue.Contains("MIXED") Then
                                dd.ShowDialog("Supplier Pack Number", "Please enter the Supplier Pack Number for the first pack of" & System.Environment.NewLine & ProductName, "", supPackID, False, False)
                                If Not supPackID = "" Then
                                    lblPacks.Text = "Pack number " & 1 & " of " & packQty
                                    lotcontrol = 1
                                Else
                                    MessageBox.Show("You have pressed cancel, nothing has been saved.")
                                    GoTo ExitRoutine
                                End If
                            ElseIf attributeValue.Contains("FIXED") Then
                                supPackID = "BULK"
                                lotcontrol = 2
                            End If


                            ' query the m3 database to select the attribute model for the purpose of building the attribute table
                            connectionString = M3Svr.ConnString(frmMain.grid)
                            sql = "SELECT AEATMO, AEATID, AEANSQ, AECOBT FROM MAMOLI WHERE AEATMO = '" & attributeValue & "';"
                            connection = New SqlConnection(connectionString)
                            Try
                                connection.Open()
                                command = New SqlCommand(sql, connection)
                                adapter.SelectCommand = command
                                If Not dsLineItems.Tables.Contains("Attribute Model") Then
                                    adapter.Fill(dsLineItems.Tables.Add("Attribute Model"))
                                Else
                                    dsLineItems.Tables("Attribute Model").Clear()
                                    adapter.Fill(dsLineItems.Tables("Attribute Model"))
                                End If
                                adapter.Dispose()
                                command.Dispose()
                                connection.Close()
                            Catch ex As Exception
                                MsgBox(ex.ToString)
                            End Try

                            ' throw an error if for some reason the attribute value is not returned.
                            If dsLineItems.Tables("Attribute Model").Rows.Count = 0 Then
                                MessageBox.Show("Invalid Attribute Model - Contact Help Desk")
                            End If

                            'build the temporary attribute table for filling from the M3 database
                            If attributeTable.Columns.Count = 0 Then
                                With attributeTable
                                    .Columns.Add("AEANSQ", Type.GetType("System.Double"))
                                    .Columns.Add("AEATID", Type.GetType("System.String"))
                                    .Columns.Add("AEATMO", Type.GetType("System.String"))
                                    .Columns.Add("AEQTY", Type.GetType("System.String"))
                                    .PrimaryKey = New DataColumn() {attributeTable.Columns("AEANSQ")}
                                End With
                            End If


                            'Query the M3 database and fill the table in the dataset, if the table already exists then clear it and add the new data
                            connectionString = M3Svr.ConnString(frmMain.grid)
                            sql = "SELECT AEANSQ, AEATID, AEATMO FROM MAMOLI where AEATMO = '" & attributeValue & "' ORDER BY AEANSQ"
                            connection = New SqlConnection(connectionString)
                            Try
                                connection.Open()
                                command = New SqlCommand(sql, connection)
                                adapter.SelectCommand = command
                                If Not dsLineItems.Tables.Contains("LineAttributes") Then
                                    adapter.Fill(dsLineItems.Tables.Add("LineAttributes"))
                                Else
                                    dsLineItems.Tables("LineAttributes").Clear()
                                    attributeTable.Clear()
                                    adapter.Fill(dsLineItems.Tables("LineAttributes"))
                                End If
                                adapter.Dispose()
                                command.Dispose()
                                connection.Close()
                            Catch ex As Exception
                                MsgBox(ex.ToString)
                            End Try

                            'iterate through the Attributes for that pack and set the qty to zero ready for updating to M3
                            For i As Integer = 0 To dsLineItems.Tables("LineAttributes").Rows.Count - 1
                                attributeTable.Rows.Add(dsLineItems.Tables("LineAttributes").Rows(i)("AEANSQ"), dsLineItems.Tables("LineAttributes").Rows(i)("AEATID"), dsLineItems.Tables("LineAttributes").Rows(i)("AEATMO"), 0)
                            Next


                            'connect to m3 using the ATS101MI to get the relevant attribute for the lineitem and fill them into the temp attributes table
                            Dim Server = "M3BE"
                            Dim Port = M3Svr.Port(frmMain.grid)
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
                            MvxSock.SetField(sid, "RIDN", po)
                            MvxSock.SetField(sid, "RIDL", lineNumber)
                            MvxSock.SetField(sid, "RIDX", subLine)

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

                            If Not dsLineItems.Tables.Contains("Attribute Table") Then
                                'create attribute table for filling
                                dsLineItems.Tables.Add(attributeTable)
                            End If

                            If lineloop = 0 Then
                                connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                                If igType = "PO" Then
                                    sql = "SELECT * FROM LineReceiving WHERE PO = '" & po & lineNumber & subLine & "';"
                                ElseIf igType = "DO" Then
                                    sql = "SELECT * FROM LineReceiving WHERE PO = '" & distro & lineNumber & subLine & "';"
                                ElseIf igType = "CO" Then
                                    sql = "SELECT * FROM LineReceiving WHERE PO = '" & co & lineNumber & subLine & "';"
                                End If

                                RunSQL(connectionString, sql, "Run List")

                                max = dsLineItems.Tables("Run List").Rows.Count - 1
                                If max = -1 Then
                                    runNumber = 0
                                Else
                                    runNumber = dsLineItems.Tables("Run List").Rows.Count + 1
                                End If

                                'write into the Gunnersen database the details of the line
                                connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                                ' There is dummy data in the below sql string
                                If igType = "PO" Then
                                    sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & po & lineNumber & subLine & RUN_NUMBER & "','" & RUN_NUMBER & "','" & po & lineNumber & subLine & "','" & DateTime.Now & "','" & po & "','" & delDockNO & "');"
                                ElseIf igType = "CO" Then
                                    sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & co & lineNumber & subLine & RUN_NUMBER & "','" & RUN_NUMBER & "','" & co & lineNumber & subLine & "','" & DateTime.Now & "','" & co & "','" & delDockNO & "');"
                                ElseIf igType = "DO" Then
                                    sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & distro & lineNumber & subLine & RUN_NUMBER & "','" & RUN_NUMBER & "','" & distro & lineNumber & subLine & "','" & DateTime.Now & "','" & distro & "','" & delDockNO & "');"
                                End If

                                connection = New SqlConnection(connectionString)
                                Try
                                    command = New SqlCommand(sql, connection)
                                    connection.Open()
                                    command.ExecuteNonQuery()
                                Catch ex As Exception
                                    MessageBox.Show(ex.ToString)
                                End Try

                            End If

                            ' if this is the first run of this function then perform the below actions

                            'get the list of previous runs saved for this purchase order

                            ' load the attributes table in from the dataset and add some relevant columns and thier associated values for display
                            dgvAttributeEntry.DataSource = dsLineItems.Tables("Attribute Table")
                            dsLineItems.Tables("Attribute Table").Rows(0)("AEQTY") = supPackID
                            If creation = 0 Then
                                dsLineItems.Tables("Attribute Table").Columns.Add("LINE_ID")
                                dsLineItems.Tables("Attribute Table").Columns.Add("Run Number")
                                dsLineItems.Tables("Attribute Table").Columns.Add("ROWNUMBER")
                            End If

                            Dim athRowNo As Integer

                            If igType = "PO" Or igType = "DO" Then
                                Do Until dsLineItems.Tables(0).Rows(athRowNo)("IBPNLI") = lineNumber
                                    athRowNo = athRowNo + 1
                                Loop
                            ElseIf igType = "CO" Then
                                Do Until dsLineItems.Tables(0).Rows(athRowNo)("ICPNLI") = lineNumber
                                    athRowNo = athRowNo + 1
                                Loop
                            End If



                            For i = 0 To dsLineItems.Tables("Attribute Table").Rows.Count - 1
                                dsLineItems.Tables("Attribute Table").Rows(i)("LINE_ID") = LINE_ID

                                If igType = "PO" Then
                                    dsLineItems.Tables("Attribute Table").Rows(i)("Run Number") = po & RUN_NUMBER
                                ElseIf igType = "CO" Then
                                    dsLineItems.Tables("Attribute Table").Rows(i)("Run Number") = co & RUN_NUMBER
                                ElseIf igType = "DO" Then
                                    dsLineItems.Tables("Attribute Table").Rows(i)("Run Number") = distro & RUN_NUMBER
                                End If
                                dsLineItems.Tables("Attribute Table").Rows(i)("ROWNUMBER") = athRowNo
                            Next

                            dgvAttributeEntry.Columns(0).Visible = False
                            dgvAttributeEntry.Columns(2).Visible = False
                            dgvAttributeEntry.Columns(4).Visible = False
                            dgvAttributeEntry.Columns(5).Visible = False
                            dgvAttributeEntry.Columns(6).Visible = False
                            dgvAttributeEntry.Columns(3).HeaderText = "Qty"
                            dgvAttributeEntry.Columns(1).HeaderText = "Value"
                            dgvAttributeEntry.Columns(0).Width = 114
                            dgvAttributeEntry.Columns(1).Width = 114
                            creation = 1

                        Else
                            'if the pack qty is zero then go to the next line as long as this isn't the last line
                            If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                                lineloop = lineloop + 1
                                LoadAttributeTable()
                            Else
                                MessageBox.Show("All packs entered", "Packs Entered", MessageBoxButtons.OK, MessageBoxIcon.Information)
ExitRoutine:
                                lineloop = 0
                                If cleanupExit Then
                                    deleteRun(po, RUN_NUMBER)
                                End If
                                Me.Close()
                                If igType = "CO" Then
                                    frmPurchaseOrder = New frmPO(co, Convert.ToInt32(subLine) + 1, igType)
                                Else
                                    frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1, igType)
                                End If
                                frmPurchaseOrder.Show()
                                frmPurchaseOrder.MdiParent = frmMain
                            End If
                        End If

                        If Not supPackID = "" Then

                            If Not dsLineItems.Tables.Contains("Attribute Original" & lineloop) Then
                                dtAttributeOriginal = dsLineItems.Tables("Attribute Table").Copy
                                dtAttributeOriginal.TableName = "Attribute Original" & lineloop
                                dsLineItems.Tables.Add(dtAttributeOriginal)
                            End If
                        End If
                End Select
            End If
        Else
            'the below code deals with a run which is being edited

            ProductName = dsLineItems.Tables(0).Rows(0)("MMFUDS")
            lblProduct.Text = ProductName
            Dim test = attributeValue

            'query the Gunnersen database for the relevant run and fill its details in the dataset
            Dim dbConnString As String = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
            sql = "SELECT * FROM Attributes WHERE RUN_NUMBER ='" & RUN_NUMBER & "';"
            connection = New SqlConnection(dbConnString)
            Try
                connection.Open()
                command = New SqlCommand(sql, connection)
                adapter.SelectCommand = command
                If dsAttributeValuesbySuppPack.Tables.Contains("Attributes") Then
                    adapter.Fill(dsAttributeValuesbySuppPack.Tables("Attributes"))
                Else
                    adapter.Fill(dsAttributeValuesbySuppPack.Tables.Add("Attributes"))
                End If

                adapter.Dispose()
                command.Dispose()
                connection.Close()
            Catch ex As Exception
                MessageBox.Show(ex.ToString)
            End Try


            'The below code loops through the dataset looking for the attribute header, which contains the Supplier Pack number, once it finds that it creates a table by that name and adds
            'all of the lines of attributes until the next supplier pack number in to a datatable
            Dim temp As String

            For i = 0 To dsAttributeValuesbySuppPack.Tables("Attributes").Rows.Count - 1
                temp = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(i)("ATTRIBUTE_VALUE")
                tableName = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(i)("ATTRIBUTE_QTY")
                If Trim(temp) = "SUPPPACK" Then
                    If lwrLmt = 0 Then
                        lwrLmt = i + 1
                        tblNmeOne = tableName
                    End If

                    If lwrLmt < i Then

                        upper = i - 1
                        tblNameTwo = tableName
                        For j = lwrLmt To upper
                            Dim tempID = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("ID")
                            Dim tempAttValue = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("ATTRIBUTE_VALUE")
                            Dim tempAttItem = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("ATTRIBUTE_ITEM")
                            Dim tempAttQty = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("ATTRIBUTE_QTY")
                            Dim tempLineID = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("LINE_ID")
                            Dim tempRunNO = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("RUN_NUMBER")
                            Dim tempRowNO = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("ROWNUMBER")
                            dsAttributeValuesbySuppPack.Tables(tblNmeOne).Rows.Add(tempID, tempAttValue, tempAttItem, tempAttQty, tempLineID, tempRunNO, tempRowNO)
                        Next
                        lwrLmt = upper + 2
                        tblNmeOne = tblNameTwo
                    End If

                    tableNo = tableNo + 1

                    If dsAttributeValuesbySuppPack.Tables.Contains(tableName) Then
                        tableName = tableName & tableNo
                        tblNmeOne = tblNameTwo & tableNo
                    End If

                    With dsAttributeValuesbySuppPack
                        .Tables.Add(tableName)
                        .Tables(tableName).Columns.Add("ID")
                        .Tables(tableName).Columns.Add("ATTRIBUTE_VALUE")
                        .Tables(tableName).Columns.Add("ATTRIBUTE_ITEM")
                        .Tables(tableName).Columns.Add("ATTRIBUTE_QTY")
                        .Tables(tableName).Columns.Add("LINE_ID")
                        .Tables(tableName).Columns.Add("RUN_NUMBER")
                        .Tables(tableName).Columns.Add("ROWNUMBER")
                    End With

                ElseIf temp.ToString.Contains("STD") Then

                    If Not dsAttributeValuesbySuppPack.Tables.Contains("tempSTD") Then
                        dsAttributeValuesbySuppPack.Tables.Add("tempSTD")
                        dsAttributeValuesbySuppPack.Tables.Add("Product Details")
                        dsAttributeValuesbySuppPack.Tables("tempSTD").Columns.Add("ROWNUMBER")
                        dsAttributeValuesbySuppPack.Tables("tempSTD").Columns.Add("PRODUCT")
                        dsAttributeValuesbySuppPack.Tables("TempSTD").Columns.Add("PACKQTY")
                        dsAttributeValuesbySuppPack.Tables("TempSTD").Columns.Add("ItemQty")
                    End If

                    po = dsLineItems.Tables(0).Rows(0)("IBPUNO")
                    connectionString = M3Svr.ConnString(frmMain.grid)
                    sql = "SELECT MITMAS.MMFUDS, MPLINE.IBPNLI, MPLINE.IBPNLS" & _
                        " FROM MPLINE, MITMAS" & _
                        " WHERE (((MPLINE.IBITNO)=[MITMAS].[MMITNO]) AND ((MPLINE.IBPUNO)='" & po & "'));"

                    connection = New SqlConnection(connectionString)

                    Try
                        connection.Open()
                        command = New SqlCommand(sql, connection)
                        adapter.SelectCommand = command
                        adapter.Fill(dsAttributeValuesbySuppPack.Tables("Product Details"))
                        adapter.Dispose()
                        command.Dispose()
                        connection.Close()

                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try

                    'For k = 0 To dsAttributeValuesbySuppPack.Tables("Attributes").Rows.Count - 1
                    Dim rownumber As String = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(i)("ROWNUMBER")
                    Dim product As String = dsAttributeValuesbySuppPack.Tables("Product Details").Rows(i)("MMFUDS")
                    Dim packQty As Integer = dsAttributeValuesbySuppPack.Tables("Attributes")(i)("PACKQTY")
                    Dim itemQty As Integer = dsAttributeValuesbySuppPack.Tables("Attributes")(i)("ITEMQTY")
                    dsAttributeValuesbySuppPack.Tables("TempSTD").Rows.Add(rownumber, product, packQty, itemQty)
                    'Next
                End If

                outup = i
                outlow = upper + 2
            Next

            Dim tblNmeTwo = tableName
            For j = outlow To outup
                Dim tempID = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("ID")
                Dim tempAttValue = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("ATTRIBUTE_VALUE")
                Dim tempAttItem = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("ATTRIBUTE_ITEM")
                Dim tempAttQty = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("ATTRIBUTE_QTY")
                Dim tempLineID = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("LINE_ID")
                Dim tempRunNO = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("RUN_NUMBER")
                Dim tempRowNO = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(j)("ROWNUMBER")
                dsAttributeValuesbySuppPack.Tables(tblNmeOne).Rows.Add(tempID, tempAttValue, tempAttItem, tempAttQty, tempLineID, tempRunNO, tempRowNO)
            Next

            'fill a datatable with the list of all of the supplier pack IDs relevant to this run
            With dsAttributeValuesbySuppPack
                .Tables.Add("SuppPack_List")
                .Tables("SuppPack_List").Columns.Add("SuppPackID")
            End With

            dsAttributeValuesbySuppPack.Tables("SuppPack_List").Rows.Add("")

            For i = 1 To dsAttributeValuesbySuppPack.Tables.Count - 2
                dsAttributeValuesbySuppPack.Tables("SuppPack_List").Rows.Add(dsAttributeValuesbySuppPack.Tables(i).TableName)
            Next

            cboSuppPacks.DataSource = dsAttributeValuesbySuppPack.Tables("SuppPack_List")
            cboSuppPacks.ValueMember = "SuppPackID"
            cboSuppPacks.DisplayMember = "SuppPackID"

            'once the datagrid has some values, remove most of the columns
            If Not temp.ToString.Contains("STD") Then
                If Convert.ToInt32(cboSuppPacks.SelectedIndex) <> 0 Then
                    dgvAttributeEntry.DataSource = dsAttributeValuesbySuppPack.Tables(cboSuppPacks.SelectedValue.ToString)
                    dgvAttributeEntry.Columns("ID").Visible = False
                    dgvAttributeEntry.Columns("ATTRIBUTE_ITEM").Visible = False
                    dgvAttributeEntry.Columns("LINE_ID").Visible = False
                    dgvAttributeEntry.Columns("RUN_NUMBER").Visible = False
                    dgvAttributeEntry.Columns("ROWNUMBER").Visible = False
                End If
                'make visible some controls which are only relevant to edits
                cboSuppPacks.Visible = True
                lblSuppPackNo.Visible = True
                lblEnter.Visible = False
            Else
                dgvAttributeEntry.DataSource = dsAttributeValuesbySuppPack.Tables("TempSTD")
                dgvAttributeEntry.Columns("Product").Width = 150
                dgvAttributeEntry.Width = 510
                btnConfirm.Enabled = True
            End If

            btnConfirm.Text = "Confirm Update"
        End If
    End Function

    Public Function CalcCatchweight()
        tally = ""
        catchWeight = 0

        If isThisAnEdit Then
            For i = 0 To dgvAttributeEntry.Rows.Count - 1
                attID = Trim(dgvAttributeEntry.Rows(i).Cells(1).Value)
                attQty = dgvAttributeEntry.Rows(i).Cells(3).Value
                If attributeValue.Contains("MIXED") Then
                    If Len(attID) < "5" Then
                        If catchWeight > 0 And attQty > 0 Then
                            tally = tally & ", " & attID & "x" & Trim(attQty)
                        ElseIf attQty > 0 Then
                            tally = attID & "x" & Trim(attQty)
                        End If
                        catchWeight = catchWeight + (Decimal.Parse(attID) * Decimal.Parse(attQty))
                    End If
                ElseIf attributeValue.Contains("FIXED") Then
                    If Len(attID) > 5 Then
                        catchWeight = Decimal.Parse(attQty)
                    End If
                End If

            Next
        Else
            For i = 0 To dsLineItems.Tables("Attribute Table").Rows.Count - 1
                attID = Trim(dsLineItems.Tables("Attribute Table").Rows(i)("AEATID"))
                attQty = dsLineItems.Tables("Attribute Table").Rows(i)("AEQTY")

                If attributeValue.Contains("MIXED") Then
                    If Len(attID) < "5" Then
                        If catchWeight > 0 And attQty > 0 Then
                            tally = tally & ", " & attID & "x" & Trim(attQty)
                        ElseIf attQty > 0 Then
                            tally = attID & "x" & Trim(attQty)
                        End If
                        catchWeight = catchWeight + (Decimal.Parse(attID) * Decimal.Parse(attQty))
                    End If
                ElseIf attributeValue.Contains("FIXED") Then
                    If attQty <> "BULK" Then
                        catchWeight = Decimal.Parse(attQty)
                    End If
                End If
            Next
        End If


    End Function

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub dgvAttributeEntry_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvAttributeEntry.CellValidating
        If e.ColumnIndex = 3 And e.RowIndex <> 0 Then
            If Not e.FormattedValue = "" Then
                If Not IsNumeric(e.FormattedValue) Then
                    MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    dgvAttributeEntry.EditingControl.Text = dgvAttributeEntry.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
                    e.Cancel = True
                End If
            End If
        End If
    End Sub

    Private Sub deleteRun(ByVal PurchNo As String, ByVal runNo As String)
        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"

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
                  " WHERE RUN_NO='" & runNo & "' AND PO='" & PurchNo & "';"
            connection = New SqlConnection(connectionString)

            command = New SqlCommand(sql, connection)
            connection.Open()
            command.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try


    End Sub

End Class
