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
    Private frmLneRec As frmLineReceiving
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
    Dim lineNumber As String
    Dim duplicatePacks As Integer
    Dim itemQty As Integer
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
    Public Shared lineloop As Integer
    Dim clickcmd As Integer




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

    Public Sub New(ByVal dt As DataTable, ByVal supPackID As String, ByVal isThisAnEdit As Boolean, ByVal dsLineItems As DataSet, ByVal RUN_NUMBER As String)
        InitializeComponent()

        If Not isThisAnEdit Then
            delDockNO = InputBox("Please enter the Delivery Docket number", "Delivery Docket Number", MessageBoxButtons.OKCancel)
        Else
            btnConfirm.Enabled = False
        End If

        Me.dt = dt
        Me.isThisAnEdit = isThisAnEdit
        Me.dsLineItems = dsLineItems
        Me.RUN_NUMBER = RUN_NUMBER
        Me.supPackID = supPackID
        packQty = dsLineItems.Tables(0).Rows(lineloop)("Packs Received")

        LoadAttributeTable()

        If Not packQty = 0 Then
            lblPacks.Text = "Pack number " & packLoop + 1 & " of " & packQty & "Line = " & lineloop
        End If

    End Sub


    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) Handles btnConfirm.Click
        Dim clickcmd As Integer
        clickcmd = clickcmd + 1
        Label1.Text = clickcmd

        Dim catchWeight As Decimal
        Dim tally As String = ""
        Dim attID As String
        Dim attQty As String


        If addCatchTally = 0 Then
            dsLineItems.Tables("Attribute Table").Columns.Add("CatchWeight")
            dsLineItems.Tables("Attribute Table").Columns.Add("Tally")
            addCatchTally = 1
        End If

        Dim dbConnString As String = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"

        If Me.isThisAnEdit Then
            For i = 0 To dsAttributeValuesbySuppPack.Tables(cboSuppPacks.SelectedValue.ToString).Rows.Count - 1
                sql = "UPDATE Attributes SET ATTRIBUTE_QTY = '" & dsAttributeValuesbySuppPack.Tables(cboSuppPacks.SelectedValue.ToString).Rows(i)("ATTRIBUTE_QTY") & "' WHERE (((ID)=" & dsAttributeValuesbySuppPack.Tables(cboSuppPacks.SelectedValue.ToString).Rows(i)("ID") & "));"
                connection = New SqlConnection(dbConnString)
                Try
                    command = New SqlCommand(sql, connection)
                    connection.Open()
                    command.ExecuteNonQuery()
                Catch ex As Exception
                    MessageBox.Show("here he is")
                End Try
                Me.Close()
            Next
        Else

            For i = 0 To dsLineItems.Tables("Attribute Table").Rows.Count - 1
                attID = Trim(dsLineItems.Tables("Attribute Table").Rows(i)("AEATID"))
                attQty = dsLineItems.Tables("Attribute Table").Rows(i)("AEQTY")

                If Len(attID) < "5" Then
                    If catchWeight > 0 And attQty > 0 Then
                        tally = tally & ", " & attID & "x" & Trim(attQty)
                    ElseIf attQty > 0 Then
                        tally = attID & "x" & Trim(attQty)
                    End If
                    catchWeight = catchWeight + (Decimal.Parse(attID) * Decimal.Parse(attQty))
                End If

            Next

            dsLineItems.Tables("Attribute Table").Rows(0)("CatchWeight") = catchWeight
            dsLineItems.Tables("Attribute Table").Rows(0)("Tally") = tally


            Using bcp As New SqlClient.SqlBulkCopy(dbConnString)
                bcp.DestinationTableName = "Attributes"
                bcp.BatchSize = 100
                bcp.WriteToServer(dsLineItems.Tables("Attribute Table"))
            End Using

            If packLoop < packQty - 1 Then
                duplicatePack = MessageBox.Show("Does the next pack have the same Tally Information as this pack?", "Duplicate Pack?", MessageBoxButtons.YesNo)
                If duplicatePack = 6 Then
                    supPackID = InputBox(lineloop & " Please enter the Supplier Pack Number for the next pack of " & ProductName, "Supplier Pack Number")
                    dsLineItems.Tables("Attribute Table").Rows(0)("AEQTY") = supPackID
                    dgvAttributeEntry.Refresh()
                Else
                    MessageBox.Show("TaDa")
                End If
                packLoop = packLoop + 1
            Else
                packLoop = 0
                If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                    lineloop = lineloop + 1
                    LoadAttributeTable()
                Else
                    If lineloop = dsLineItems.Tables(0).Rows.Count - 1 Then
                        MessageBox.Show("All packs entered")
                        lineloop = 0
                        Me.Close()
                        frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1)
                        frmPurchaseOrder.Show()
                        frmPurchaseOrder.MdiParent = frmMain
                    End If
                    End If
            End If
            dsLine.Tables("QtySaved").Clear()
        End If
    End Sub

    Public Function RunSQL(ByVal conString As String, ByVal sql As String, tableName As String)
        connection = New SqlConnection(conString)
        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(dsLineItems.Tables.Add(tableName))
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
            dgvAttributeEntry.Columns(3).HeaderText = "Qt"
            dgvAttributeEntry.Columns(1).HeaderText = "Value"
            btnConfirm.Enabled = True
        End If
    End Sub

   
    Public Function LoadAttributeTable()
        If Not isThisAnEdit Then
            'the below code deals with a run which is being created.

            'Fill variables with data from the relevant dataset
            subLine = dsLineItems.Tables(0).Rows(lineloop)("IBPNLS")
            LINE_ID = Trim(dsLineItems.Tables(0).Rows(lineloop)("IBPUNO")) & dsLineItems.Tables(0).Rows(lineloop)("IBPNLI") & dsLineItems.Tables(0).Rows(lineloop)("IBPNLS")
            po = Trim(dsLineItems.Tables(0).Rows(lineloop)("IBPUNO"))
            lineNumber = dsLineItems.Tables(0).Rows(lineloop)("IBPNLI")
            ProductName = dsLineItems.Tables(0).Rows(lineloop)("MMFUDS")
            itemQty = dsLineItems.Tables(0).Rows(lineloop)("Qty Received")
            qtyTotal = dsLineItems.Tables(0).Rows(lineloop)("IBRVQA")
            packQty = dsLineItems.Tables(0).Rows(lineloop)("Packs Received")
            lblProduct.Text = ProductName

            If packLoop = 0 And lineloop = 0 Then
                dsLine.Tables.Add("qtySaved")
            End If

            ' query the M3 database for the product details and fill the dataset, for the purposes of getting the attribute value and handling it accordingly.
            connectionString = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
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
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try


            ' check that the packqty for the current row isn't 0, if it is don't run the routine, and go to the next row
            If packQty <> 0 Then

                If attributeValue.Contains("MIXED") Then
                    supPackID = InputBox(lineloop & packLoop & " Please enter the Supplier Pack Number for the first pack of " & ProductName, "Supplier Pack Number")
                    lblPacks.Text = "Pack number " & lineloop + 1 & " of " & packQty
                    lotcontrol = 1
                ElseIf attributeValue.Contains("FIXED") Then
                    supPackID = "BULK"
                    lotcontrol = 2
                End If

                If supPackID <> "" Then
                    ' query the m3 database to select the attribute model for the purpose of building the attribute table
                    connectionString = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
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


                    'Query the M3 database and fill thetable in the dataset, if the table already exists then clear it and add the new data
                    connectionString = "Data Source=m3db;Initial Catalog=M3FDBTST;Persist Security Info=True;User ID=Query;Password=Query"
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


                    ' if this is the first run of this function then perform the below actions
                    If Not dsLineItems.Tables.Contains("Attribute Table") Then
                        'create attribute table for filling
                        dsLineItems.Tables.Add(attributeTable)

                        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                        sql = "SELECT * FROM LineReceiving WHERE PO_ID= '" & po & lineNumber & subLine & "';"
                        RunSQL(connectionString, sql, "Run List")

                        max = dsLineItems.Tables("Run List").Rows.Count - 1
                        If max = -1 Then
                            runNumber = 0
                        Else
                            runNumber = dsLineItems.Tables("Run List").Rows.Count + 1
                        End If

                        'write into the Gunnersen database the details of the line
                        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
                        ' There is dummy data in the below sql string
                        sql = "INSERT INTO dbo.LineReceiving(ID, RUN_NO, PO_ID, RUN_DATETIME, PO, DELDOCKNO) Values ('" & po & lineNumber & subLine & runNumber & "','" & runNumber & "','" & po & lineNumber & subLine & "','" & DateTime.Now & "','" & po & "','" & delDockNO & "');"
                        connection = New SqlConnection(connectionString)
                        Try
                            command = New SqlCommand(sql, connection)
                            connection.Open()
                            command.ExecuteNonQuery()
                        Catch ex As Exception
                            MessageBox.Show(ex.ToString)
                        End Try
                    End If

                    runNumberKey = po & runNumber.ToString

                    ' load the attributes table in from the dataset and add some relevant columns and thier associated values for diaplay
                    dgvAttributeEntry.DataSource = dsLineItems.Tables("Attribute Table")
                    dsLineItems.Tables("Attribute Table").Rows(0)("AEQTY") = supPackID
                    If creation = 0 Then
                        dsLineItems.Tables("Attribute Table").Columns.Add("LINE_ID")
                        dsLineItems.Tables("Attribute Table").Columns.Add("Run Number")
                        dsLineItems.Tables("Attribute Table").Columns.Add("ROWNUMBER")
                    End If
                    For i = 0 To dsLineItems.Tables("Attribute Table").Rows.Count - 1
                        dsLineItems.Tables("Attribute Table").Rows(i)("LINE_ID") = LINE_ID
                        dsLineItems.Tables("Attribute Table").Rows(i)("Run Number") = runNumberKey
                        dsLineItems.Tables("Attribute Table").Rows(i)("ROWNUMBER") = subLine
                    Next

                    dgvAttributeEntry.Columns(0).Visible = False
                    dgvAttributeEntry.Columns(2).Visible = False
                    dgvAttributeEntry.Columns(4).Visible = False
                    dgvAttributeEntry.Columns(5).Visible = False
                    dgvAttributeEntry.Columns(3).HeaderText = "Qty"
                    dgvAttributeEntry.Columns(1).HeaderText = "Value"
                    dgvAttributeEntry.Columns(0).Width = 114
                    dgvAttributeEntry.Columns(1).Width = 114

                Else
                    ' if the user doesn't enter a value for the supplier pack id or clicks cancel dela with it here
                    MessageBox.Show("You Clicked cancel")
                    Me.Hide()
                    frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1)
                    frmPurchaseOrder.Show()
                    frmPurchaseOrder.MdiParent = frmMain
                End If
                creation = 1
            Else
                'if the pack qty is zero then go to the next line as long as this isn't the last line
                If lineloop < dsLineItems.Tables(0).Rows.Count - 1 Then
                    lineloop = lineloop + 1
                    LoadAttributeTable()
                Else
                    MessageBox.Show("All packs entered")
                    lineloop = 0
                    Me.Close()
                    frmPurchaseOrder = New frmPO(po, Convert.ToInt32(subLine) + 1)
                    frmPurchaseOrder.Show()
                    frmPurchaseOrder.MdiParent = frmMain
                End If
            End If
        Else
            'the below code deals with a run which is being edited

            ProductName = dsLineItems.Tables(0).Rows(0)("MMFUDS")
            lblProduct.Text = ProductName

            'query the GUnnersen database for the relevant run and fill its details in the dataset
            Dim dbConnString As String = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=sa;Password=NewChair4JAC;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
            sql = "SELECT * FROM Attributes WHERE RUN_NUMBER ='" & RUN_NUMBER & "';"
            connection = New SqlConnection(dbConnString)
            Try
                connection.Open()
                command = New SqlCommand(sql, connection)
                adapter.SelectCommand = command
                adapter.Fill(dsAttributeValuesbySuppPack.Tables.Add("Attributes"))
                adapter.Dispose()
                command.Dispose()
                connection.Close()
            Catch ex As Exception
                MessageBox.Show(ex.ToString)
            End Try


            'The below code loops through the dataset looking for the attribute header, which contains the Supplier Pack number, once it finds that it creates a table by that name and adds
            'all of the lines of attributes until the next supplier pack number in to a datatable

            For i = 0 To dsAttributeValuesbySuppPack.Tables("Attributes").Rows.Count - 1
                Dim temp = dsAttributeValuesbySuppPack.Tables("Attributes").Rows(i)("ATTRIBUTE_VALUE")
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

            'make visible some controls which are only relevant to edits
            btnConfirm.Text = "Confirm Update"
            cboSuppPacks.Visible = True
            lblSuppPackNo.Visible = True
            lblEnter.Visible = False

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
            If Convert.ToInt32(cboSuppPacks.SelectedIndex) <> 0 Then
                dgvAttributeEntry.DataSource = dsAttributeValuesbySuppPack.Tables(cboSuppPacks.SelectedValue.ToString)
                dgvAttributeEntry.Columns("ID").Visible = False
                dgvAttributeEntry.Columns("ATTRIBUTE_ITEM").Visible = False
                dgvAttributeEntry.Columns("LINE_ID").Visible = False
                dgvAttributeEntry.Columns("RUN_NUMBER").Visible = False
                dgvAttributeEntry.Columns("ROWNUMBER").Visible = False
            End If
        End If
    End Function
End Class