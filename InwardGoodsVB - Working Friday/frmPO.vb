Imports System.Data.SqlClient
Imports Lawson.M3.MvxSock
Imports InwardGoodsVB.wsStockOperations
Imports InwardGoodsVB.wsGLUtlities
Imports System.ServiceModel
Imports System.Diagnostics
Imports System.IO
Imports System.Threading


Public Class frmPO
    Dim arrExtStock
    Dim arrNewStock
    Private _poNumber As String
    Private _lineloop As Integer
    Private _igType As String
    Dim connectionString As String
    Dim connection As SqlConnection
    Dim command As SqlCommand
    Dim adapter As New SqlDataAdapter
    Dim ds As New DataSet
    Dim ds2 As New DataSet
    Dim sql As String
    Dim QtySaved As Integer
    Dim LineNo As String
    Dim subLineNo As String
    Dim supPackID As String
    Dim attributeValue As String
    Dim lotControl As Boolean = False
    Public attributeTable As DataTable = New DataTable("Attribute Table")
    Dim runNumber As Integer
    Private frmAttEnt As frmAttributeEntry
    Dim i As Integer
    Dim j As Integer
    Dim itemQty As Integer
    Dim img As Image
    Dim delete As String
    Dim currentDate As DateTime = DateTime.Now
    Dim intday As String = currentDate.Day
    Dim intYear As String = currentDate.Year
    Dim rc
    Dim todayDate As String
    Dim respUser As String
    Dim PNLS As String
    Dim catchWeight As String
    Dim isPack As Boolean
    Dim putAway As String
    Dim lotNO As String
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
    Dim attributeItemNO As String
    Dim Desc As String
    Dim Server As String
    Dim Port As String
    Dim UserID As String
    Dim PWD As String
    Dim APIName As String
    Dim APIOpr As String
    Dim sid As New SERVER_ID
    Dim progressText As String
    Dim M3Svr As M3Point.M3Point = New M3Point.M3Point
    Dim SUDO As String
    Dim SUNO As String
    Dim DNDT As String


    'dim global constructors for passing values to the form from parent form 
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
        Me.Text = igType & " " & Me.poNumber 'set the title of the form to the current PO number being edited/created

        connectionString = M3Svr.ConnString(frmMain.grid)

        'relevant sql statment, determined by whether the number entered was PO, DO or Container
        If igType = "PO" Then
            sql = "SELECT * FROM MPHEAD where IAPUNO = '" & Trim(Me.poNumber) & "'"
        ElseIf igType = "DO" Then
            sql = "SELECT * FROM MGHEAD where MGTRNR = '" & Trim(Me.poNumber) & "'"
            txtFreightCharge.Visible = True
        ElseIf igType = "CO" Then
            sql = "SELECT *, ICWHLO AS IAWHLO " & _
                  "FROM MPLIND " & _
                  "WHERE ICPACN = '" & Trim(Me.poNumber) & "' AND ICPUOS = 40 AND ICSUDO IN (SELECT TOP 1 ICSUDO " & _
                                                                                        "FROM MPLIND " & _
                                                                                        "WHERE ICPACN = '" & Trim(Me.poNumber) & "' " & _
                                                                                        "ORDER BY ICSUDO DESC);"
        End If

        'run the above sql
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

        'set textboxes to relevant values based on the type of order being recepited
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
            lblStatusText.Visible = False
        ElseIf igType = "CO" Then
            lblOne.Text = "Container Number"
            lblTwo.Text = "To Warehouse"
            lblWarehouseID.Text = ds.Tables(0).Rows(0)("IAWHLO")
            lblPO.Text = ds.Tables(0).Rows(0)("ICPACN")
            lblStatus.Visible = False
            lblStatusText.Visible = False
            lblCreditorID.Text = ds.Tables(0).Rows(0)("ICWHLO")
            SUDO = ds.Tables(0).Rows(0)("ICSUDO")
            SUNO = ds.Tables(0).Rows(0)("ICSUNO")
            DNDT = ds.Tables(0).Rows(0)("ICTRDT")
        End If

        'fill the item grid
        FillItemGrid()

        'Check attribute value, if it's "MIXEDPACK" then set the itemQty cell to 1 and readonly
        connectionString = M3Svr.ConnString(frmMain.grid)


        'retrieve the line details of the products which form the order being receipted
        For i = 0 To dgvItems.Rows.Count - 1

            sql = "SELECT * FROM MITMAS WHERE MMITNO = '" & ds2.Tables(0).Rows(i)("IBITNO") & "';"


            connection = New SqlConnection(connectionString)

            'run the above sql
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

            'determine the type of product being receipted, if the MMATMO field in M3 database is "" then look in the MMITTY field for G number
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
        Next

        'fill the saved column with values if there is any runs previously saved for this order, if thier isn't fill it with zeros
        FillSavedColumn()

        ' sets the width of the datagridview to the total width of all the columns meaning no grey space on sides
        Dim dgvWidth As Integer = 43
        For i = 0 To dgvItems.Columns.Count - 1
            If dgvItems.Columns(i).Visible = True Then
                dgvWidth = dgvWidth + dgvItems.Columns(i).Width
            End If
        Next
        dgvItems.Width = dgvWidth

        ' fill the rundatagrid with runs if any exist
        FillRunDataGrid()



    End Sub

    Private Sub tsbNewRun_Click(sender As Object, e As EventArgs) Handles tsbNewRun.Click
        totItemQty = 0

        Me.Hide()

        'fill the received cells with zeros if there is no values in them to prevent dbnull errors
        For countRows = 0 To dgvItems.Rows.Count - 1
            If IsDBNull(dgvItems.Rows(countRows).Cells("Packs Received").Value) Then
                dgvItems.Rows(countRows).Cells("Packs Received").Value = 0
            End If
            totPackQty = totPackQty + dgvItems.Rows(countRows).Cells("Packs Received").Value

            If IsDBNull(dgvItems.Rows(countRows).Cells("Qty Received").Value) Then
                dgvItems.Rows(countRows).Cells("Qty Received").Value = 0
            End If
            totItemQty = totItemQty + dgvItems.Rows(countRows).Cells("Qty Received").Value
            If dgvItems.Columns.Contains("LabelQty") Then
                If IsDBNull(dgvItems.Rows(countRows).Cells("LabelQty").Value) Then
                    dgvItems.Rows(countRows).Cells("LabelQty").Value = 0
                End If
            End If
        Next

        'Check that the the freight charge has been input, only if the receipt type is a DO
        If IsNumeric(txtFreightCharge.Text) Or Not igType = "DO" Then


            ' apportion delivery charge across lines (weighted average based on cost) for Distribution Orders
            If igType = "DO" Then
                AppDelCharge()
            End If

            'set the run number 
            runNumber = ds2.Tables("Runs").Rows.Count + 1

            'check that there has actually been some data entered into the cells which are relevent to the prudct, and if there hasn't prompt the user
            If attributeValue.Contains("IX") Then 'if the item is a mixed or fixed pack product handle it's data entry
                If Not totPackQty = 0 Then
                    frmAttEnt = New frmAttributeEntry(Me.attributeTable, supPackID, False, ds2, runNumber, igType)
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
            ElseIf attributeValue = "BATCH" Or attributeValue = "EXPIRY" Then 'if the item is a Batch or Expiry product handle it's data entry 
                'For countRows = 0 To dgvItems.Rows.Count - 1
                '    dgvItems.Rows(countRows).Cells("Packs Received").Value = 1
                'Next
                If Not totItemQty = 0 Then
                    frmAttEnt = New frmAttributeEntry(Me.attributeTable, supPackID, False, ds2, runNumber, igType)
                    If frmAttEnt.IsDisposed = False Then
                        frmAttEnt.Show()
                        frmAttEnt.MdiParent = frmMain
                        Me.Close()
                    End If
                Else
                    MessageBox.Show("You have not entered any items, enter items to be received.")
                End If
            ElseIf attributeValue = "STANDARD" Then ' if the item is a standard product handle the data entry (i.e there is o data to enter)
                If Not totItemQty = 0 And Not totPackQty = 0 Then
                    frmAttEnt = New frmAttributeEntry(Me.attributeTable, supPackID, False, ds2, runNumber, igType)
                    If frmAttEnt.IsDisposed = False Then
                        frmAttEnt.Show()
                        frmAttEnt.MdiParent = frmMain
                        Me.Close()
                    End If
                Else
                    MessageBox.Show("You have not entered any items or packs, enter items and packs to be received.")
                End If
            End If
        Else
            MessageBox.Show("You have not entered a delivery charge, enter a delivery charge to proceed")
        End If



    End Sub

    Public Function RunSQL(ByVal conString As String, ByVal sql As String, tableName As String)
        'function to runs SQL, doesn't get used much, should have used more
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

    Private Sub dgvItems_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvItems.CellContentClick
        'Handle the event when the label print check box is clicked on a DO receipt, this puts a binary value in the datgrid which is copied
        ' to the dataset for retreival when the run is posted and labels are printed or not.
        If e.ColumnIndex = 0 Then
            If dgvItems.Rows(e.RowIndex).Cells("chk").EditedFormattedValue Then
                dgvItems.Rows(e.RowIndex).Cells("LabelQty").Value = 1
            Else
                dgvItems.Rows(e.RowIndex).Cells("LabelQty").Value = 0
            End If
        End If

    End Sub

    Private Sub dgvItems_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvItems.CellValidating
        'routine to validate the cells in the items grid, this checks that the number of packs or items the user is inputting isn't more than the number
        'which has been entered on the PO, and inform the user that they are sure they want to over receipt the line.
        Select Case igType
            Case "PO"
                If Not dgvItems.EditingControl Is Nothing Then
                    If e.ColumnIndex = dgvItems.Columns("Packs Received").Index Then
                        If attributeValue.Contains("IX") Then
                            If Not e.FormattedValue = "" Then
                                If IsNumeric(e.FormattedValue) Then
                                    Dim rowPackQty, rowReceivedQty, rowSaved, rowQtytoRec, rowPacksRec As Integer
                                    rowPackQty = dgvItems.Rows(e.RowIndex).Cells("IBORQA").Value
                                    rowReceivedQty = dgvItems.Rows(e.RowIndex).Cells("IBRVQA").Value
                                    rowSaved = dgvItems.Rows(e.RowIndex).Cells("Saved").Value
                                    rowQtytoRec = rowPackQty - rowReceivedQty - rowSaved
                                    rowPacksRec = e.FormattedValue
                                    If rowPacksRec > rowQtytoRec Then ' allow for over receipting of PO, but inform the user they are doing it first
                                        If Not IsNothing(dgvItems.EditingControl.Text) Then
                                            Dim okToContinue = MessageBox.Show("Quanity of packs entered to receive is greater than the amount left to receive, is this correct?", "Gunnersen Inward Goods", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                            If okToContinue = 6 Then
                                                tsbNewRun.Enabled = True
                                                btnCreateNewDD.Enabled = True
                                            Else
                                                tsbNewRun.Enabled = False
                                                btnCreateNewDD.Enabled = False
                                            End If
                                        End If
                                    Else
                                        tsbNewRun.Enabled = True
                                        btnCreateNewDD.Enabled = True
                                    End If
                                Else
                                    MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter " & _
                                                    "a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    dgvItems.EditingControl.Text = ""
                                    e.Cancel = True
                                    tsbNewRun.Enabled = False
                                    btnCreateNewDD.Enabled = False
                                End If
                            Else
                                If Not e.FormattedValue = "" Then
                                    MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter " & _
                                                    "a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                End If
                            End If
                        End If
                    End If

                    If e.ColumnIndex = dgvItems.Columns("Qty Received").Index Then
                        If Not attributeValue.Contains("IX") Then
                            If Not e.FormattedValue = "" Then
                                If IsNumeric(e.FormattedValue) Then
                                    Dim rowPackQty, rowReceivedQty, rowSaved, rowQtytoRec, rowPacksRec As Integer
                                    rowPackQty = dgvItems.Rows(e.RowIndex).Cells("IBORQA").Value
                                    rowReceivedQty = dgvItems.Rows(e.RowIndex).Cells("IBRVQA").Value
                                    rowSaved = dgvItems.Rows(e.RowIndex).Cells("Saved").Value
                                    rowQtytoRec = rowPackQty - rowReceivedQty - rowSaved
                                    rowPacksRec = e.FormattedValue
                                    If rowPacksRec > rowQtytoRec Then ' allow for over receipting of PO, but inform the user they are doing it first
                                        If Not IsNothing(dgvItems.EditingControl.Text) Then
                                            If Not IsNothing(dgvItems.EditingControl.Text) Then
                                                Dim okToContinue = MessageBox.Show("Quanity of items entered to receive is greater than the amount left to receive, is this correct?", "Gunnersen Inward Goods", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                                If okToContinue = 6 Then
                                                    tsbNewRun.Enabled = True
                                                    btnCreateNewDD.Enabled = True
                                                Else
                                                    tsbNewRun.Enabled = False
                                                    btnCreateNewDD.Enabled = False
                                                End If
                                            End If
                                        End If
                                    Else
                                        tsbNewRun.Enabled = True
                                        btnCreateNewDD.Enabled = True
                                    End If

                                Else
                                    MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter " & _
                                                    "a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    dgvItems.EditingControl.Text = ""
                                    e.Cancel = True
                                    tsbNewRun.Enabled = False
                                    btnCreateNewDD.Enabled = False
                                End If
                            Else
                                If Not e.FormattedValue = "" Then
                                    MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter " & _
                                                    "a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                End If
                            End If
                        End If
                    End If
                End If

            Case "CO"
                ' Containers cannot be over receipted, this is a limitation of the API provided by Infor, it may be changed in future updates to the ZPP360 API
                If Not dgvItems.EditingControl Is Nothing Then
                    If e.ColumnIndex = dgvItems.Columns("Packs Received").Index Then
                        If Not e.FormattedValue = "" Then
                            If Trim(dgvItems.Rows(e.RowIndex).Cells("IBRVQA").Value) <> dgvItems.Rows(e.RowIndex).Cells("ICRPQA").Value Then
                                If IsNumeric(e.FormattedValue) Then
                                    tsbNewRun.Enabled = True
                                    btnCreateNewDD.Enabled = True
                                    dgvItems.Rows(e.RowIndex).Cells("Qty Received").Value = ds2.Tables(0).Rows(e.RowIndex)("ICRPQA")
                                Else
                                    MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter " & _
                                                    "a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    dgvItems.EditingControl.Text = ""
                                    e.Cancel = True
                                    tsbNewRun.Enabled = False
                                    btnCreateNewDD.Enabled = False
                                End If
                            Else
                                MessageBox.Show("This item has been fully receipted", "Fully " & _
                                                        "Receipted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                dgvItems.EditingControl.Text = ""
                                e.Cancel = True
                                tsbNewRun.Enabled = False
                                btnCreateNewDD.Enabled = False
                            End If
                        Else
                            If Not e.FormattedValue = "" Then
                                MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter " & _
                                                "a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            End If
                        End If
                    End If
                End If

            Case "DO"
                If Not dgvItems.EditingControl Is Nothing Then
                    If e.ColumnIndex = dgvItems.Columns("Packs Received").Index Then
                        If attributeValue.Contains("IX") Then
                            If Not e.FormattedValue = "" Then
                                If IsNumeric(e.FormattedValue) Then
                                    Dim rowPackQty, rowReceivedQty, rowSaved, rowQtytoRec, rowPacksRec As Integer
                                    rowPackQty = dgvItems.Rows(e.RowIndex).Cells("MRTRQT").Value
                                    rowReceivedQty = dgvItems.Rows(e.RowIndex).Cells("MRRPQT").Value
                                    rowSaved = dgvItems.Rows(e.RowIndex).Cells("Saved").Value
                                    rowQtytoRec = rowPackQty - rowReceivedQty - rowSaved
                                    rowPacksRec = e.FormattedValue
                                    If rowPacksRec > rowQtytoRec Then ' allow for over receipting of DO, but inform the user they are doing it first
                                        If Not IsNothing(dgvItems.EditingControl.Text) Then
                                            If Not IsNothing(dgvItems.EditingControl.Text) Then
                                                Dim okToContinue = MessageBox.Show("Quanity of packs entered to receive is greater than the amount left to receive, is this correct?", "Gunnersen Inward Goods", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                                If okToContinue = 6 Then
                                                    tsbNewRun.Enabled = True
                                                    btnCreateNewDD.Enabled = True
                                                Else
                                                    tsbNewRun.Enabled = False
                                                    btnCreateNewDD.Enabled = False
                                                End If
                                            End If
                                        End If
                                    Else
                                        tsbNewRun.Enabled = True
                                        btnCreateNewDD.Enabled = True
                                    End If
                                Else
                                    MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter " & _
                                                    "a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    dgvItems.EditingControl.Text = ""
                                    e.Cancel = True
                                    tsbNewRun.Enabled = False
                                    btnCreateNewDD.Enabled = False
                                End If
                            Else
                                If Not e.FormattedValue = "" Then
                                    MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter " & _
                                                    "a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                End If
                            End If
                        End If
                    End If

                    If e.ColumnIndex = dgvItems.Columns("Qty Received").Index Then
                        If Not attributeValue.Contains("IX") Then
                            If Not e.FormattedValue = "" Then
                                If IsNumeric(e.FormattedValue) Then
                                    Dim rowPackQty, rowReceivedQty, rowSaved, rowQtytoRec, rowPacksRec As Integer
                                    rowPackQty = dgvItems.Rows(e.RowIndex).Cells("MRTRQT").Value
                                    rowReceivedQty = dgvItems.Rows(e.RowIndex).Cells("MRRPQT").Value
                                    rowSaved = dgvItems.Rows(e.RowIndex).Cells("Saved").Value
                                    rowQtytoRec = rowPackQty - rowReceivedQty - rowSaved
                                    rowPacksRec = e.FormattedValue
                                    If rowPacksRec > rowQtytoRec Then ' allow for over receipting of Do, but inform the user they are doing it first
                                        If Not IsNothing(dgvItems.EditingControl.Text) Then
                                            If Not IsNothing(dgvItems.EditingControl.Text) Then
                                                Dim okToContinue = MessageBox.Show("Quanity of packs entered to receive is greater than the amount left to receive, is this correct?", "Gunnersen Inward Goods", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                                If okToContinue = 6 Then
                                                    tsbNewRun.Enabled = True
                                                    btnCreateNewDD.Enabled = True
                                                Else
                                                    tsbNewRun.Enabled = False
                                                    btnCreateNewDD.Enabled = False
                                                End If
                                            End If
                                        End If
                                    Else
                                        tsbNewRun.Enabled = True
                                        btnCreateNewDD.Enabled = True
                                    End If

                                Else
                                    MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter " & _
                                                    "a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    dgvItems.EditingControl.Text = ""
                                    e.Cancel = True
                                    tsbNewRun.Enabled = False
                                    btnCreateNewDD.Enabled = False
                                End If
                            Else
                                If Not e.FormattedValue = "" Then
                                    MessageBox.Show("You have entered a non numeric value into the quanity field, please correct.", "Please enter " & _
                                                    "a number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                End If
                            End If
                        End If
                    End If
                End If
        End Select
    End Sub

    ' buggy code to go to the next row once a column has been reached ala Excel, still working on it, not implemented yet.
    Private Sub dgvItems_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles dgvItems.KeyPress
        Dim Characters As String = ChrW(Keys.Tab)
        If InStr(Characters, e.KeyChar) = 1 Then
            If Me.dgvItems.CurrentCell.ColumnIndex = 1 Then
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
        Dim success As Boolean

        ' load relevant run into "Run", this is attributes tables with all records.
        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect " & _
                            "Timeout=15;Encrypt=False;TrustServerCertificate=False"
        sql = "SELECT * FROM Attributes WHERE RUN_NUMBER = '" & poNumber & runNumber & "';"
        connection = New SqlConnection(connectionString)
        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            If Not ds.Tables.Contains("Run") Then
                adapter.Fill(ds.Tables.Add("Run"))
            Else
                ds.Tables("Run").Clear()
                adapter.Fill(ds.Tables("Run"))
            End If
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try

        'load relevant data into the "packs", table this is attributes table with only the header recordss
        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect " & _
                            "Timeout=15;Encrypt=False;TrustServerCertificate=False"
        sql = "SELECT * FROM Attributes WHERE PACKTYPE IS NOT NULL AND RUN_NUMBER = '" & poNumber & runNumber & "';"
        connection = New SqlConnection(connectionString)
        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            If Not ds.Tables.Contains("Packs") Then
                adapter.Fill(ds.Tables.Add("Packs"))
            Else
                ds.Tables("Packs").Clear()
                adapter.Fill(ds.Tables("Packs"))
            End If
            adapter.Dispose()
            command.Dispose()
            connection.Close()

            'get the total number of packs from the "packs"" table by counting the number of records.
            Dim runs As Integer = ds.Tables("Packs").Compute("Count(PackQty)", "")

            frmProgress.ProgressSetup(runs * 2) 'setup the progressbar

            For i = 0 To ds.Tables("Packs").Rows.Count - 1
                Me.Cursor = Cursors.WaitCursor

                progressText = i + 1 & " of " & runs & " products processing" 'inccrement the progressbar
                frmProgress.IncProg(progressText)



                Dim chkStat As String
                Dim stat As String

                'check that the status of the PO, DO or Container is not already at the stage were trying to push it to.
                If igType = "CO" Then
                    chkStat = "IBPUSL"
                    stat = "50"
                ElseIf igType = "PO" Then
                    chkStat = "IBPUSL"
                    stat = "50"
                ElseIf igType = "DO" Then
                    chkStat = "MRTRSH"
                    stat = "99"
                End If

                packsRow = ds.Tables("Packs").Rows(i)("ROWNUMBER")

                If ds2.Tables(0).Rows(packsRow)(chkStat) < stat Then
                    rowNumber = ds.Tables("Packs").Rows(i)("ROWNUMBER")
                    itemNumber = Trim(ds2.Tables(0).Rows(rowNumber)("IBITNO"))
                    attributeValue = ds.Tables("Packs").Rows(i)("PACKTYPE")
                    Dim PONo As String


                    'run the required M3 API calls to post the receipt to M3.
                    If igType = "DO" Then
                        arrExtStock = GetAvgCostandStock(itemNumber, lblCreditorID.Text)
                        success = MHS850()
                        If success Then
                            arrNewStock = GetAvgCostandStock(itemNumber, lblCreditorID.Text)
                            WSAverageCostAdjust(ds.Tables("Packs").Rows(i)("AppCharge"))
                            FinishUpAndPrintLabels()
                        End If
                    Else

                        If attributeValue.Contains("IX") Then
                            lotControl = True
                            tally = ds.Tables("Packs").Rows(packsRow)("TALLY")
                            wrkCAWE = ds.Tables("Packs").Rows(packsRow)("CATCHWEIGHT")
                        Else
                            lotControl = False
                        End If

                        If igType = "CO" Then
                            PONo = Trim(ds2.Tables(0).Rows(rowNumber)("ICPUNO"))
                            success = PPS200MI(PONo)
                            If success Then
                                success = ZPP363MI()
                            Else
                                Me.Cursor = Cursors.Arrow
                                frmProgress.Close()
                            End If
                        Else
                            success = PPS001MI()
                        End If

                        If success Then
                            success = MWS070MI()
                            If success Then
                                If lotControl Then ' the below only runs for lotcontrolled packs (MIXED or FIXED)
                                    success = MMS235MI()
                                    If success Then
                                        success = ATS101MI()
                                        If success Then
                                            If attributeValue.Contains("MIXED") Then
                                                success = CUSTEXTMI()
                                                If success Then
                                                    success = WSCATCHWEIGHT()
                                                    If success Then
                                                        PPS320()
                                                        FinishUpAndPrintLabels()
                                                    Else
                                                        Me.Cursor = Cursors.Arrow
                                                        frmProgress.Close()
                                                    End If
                                                Else
                                                    Me.Cursor = Cursors.Arrow
                                                    frmProgress.Close()
                                                End If
                                            Else
                                                FinishUpAndPrintLabels()
                                            End If
                                        Else
                                            Me.Cursor = Cursors.Arrow
                                            frmProgress.Close()
                                        End If
                                    Else
                                        Me.Cursor = Cursors.Arrow
                                        frmProgress.Close()
                                    End If
                                Else
                                    FinishUpAndPrintLabels()
                                End If
                            Else
                                Me.Cursor = Cursors.Arrow
                                frmProgress.Close()
                            End If
                        Else
                            Me.Cursor = Cursors.Arrow
                            frmProgress.Close()
                        End If
                    End If

                Else
                    MessageBox.Show(itemNumber & " has been fully receipted, no M3 post has been performed for this item", "Item Fully Receipted")
                End If
            Next

            'now that all of the lines have been deleted delete the run
            deleteRun(poNumber, runNumber)

            'update the rundatagrid
            FillRunDataGrid()

            'update the itemgrid
            FillItemGrid()

            'update the saved column
            FillSavedColumn()

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Function

    Private Sub FinishUpAndPrintLabels()
        'add columns to the dataset table which contains the details of each pack being posted for the purpose of saving the data
        If Not ds.Tables("Packs").Columns.Contains("perPack") Then
            ds.Tables("Packs").Columns.Add("perPack")
            ds.Tables("Packs").Columns.Add("shortPack")
            ds.Tables("Packs").Columns.Add("putAway")
            ds.Tables("Packs").Columns.Add("lotNO")
        End If
        Dim qtyToPrint

        If igType = "DO" Then
            qtyToPrint = Convert.ToInt32(ds.Tables("RUN").Rows(rowNumber)("LABELQTY"))
        End If

        'print the labels
        If ds2.Tables(0).Columns.Contains("LabelQty") Then
            For z = 0 To qtyToPrint - 1
                PrintLabels(rowNumber)
                packsRow = packsRow - 1
                System.Threading.Thread.Sleep(1500)
            Next
            packsRow = packsRow + 1
        Else
            PrintLabels(rowNumber)
        End If


        'save the data into the Gunnersen.labelprinting SQL table on the M3 server
        SaveRunPackRows()

        'increment the progressbar
        frmProgress.IncProg(progressText)
        Me.Cursor = Cursors.Arrow

        'set the attribute Number for the pack just received for the purpose of deleting that line form the attributes SQL table now 
        'that the line has been saved into the labelprinting table
        If attributeValue.Contains("IX") Then
            If igType = "DO" Then
                attributeItemNO = ""
            Else
                attributeItemNO = ds.Tables("Packs").Rows(packsRow - 1)("ATTRIBUTE_ITEM")
            End If

        Else
            attributeItemNO = ""
        End If
        deleteRunLine(poNumber, runNumber, attributeItemNO)
    End Sub

    Private Function WSCATCHWEIGHT()

        ' create the webservices clinet and set values.
        Dim client As New StockOperationsClient
        Dim binding As New System.ServiceModel.BasicHttpBinding
        binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic
        binding.MaxReceivedMessageSize = 25 * 1024 * 1024
        client.Endpoint.Binding = binding
        client.ClientCredentials.UserName.UserName = "DTAMIGR"
        client.ClientCredentials.UserName.Password = "Q190E87AG"

        'create lws header
        Dim header = New InwardGoodsVB.wsStockOperations.lws
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

            'create collection of items(even though there is only one item)
            Dim collection = New CatchweightType
            collection.MMS360 = catchweightItem

            'put data to M3 webservice
            client.Catchweight(header, collection)

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
            frmProgress.Close()

        End Try

        If rc = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function CUSTEXTMI()
        'set M3 API parameters
        Server = "M3BE"
        Port = M3Svr.Port(frmMain.grid)
        UserID = "DTAMIGR"
        PWD = "Q190E87AG"
        APIName = "CUSEXTMI"
        APIOpr = "AddFieldValue"
        Server = "M3BE"

        ' Connect API
        rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "CUSTEXTMI - Error Occurred: ")
        End If


        MvxSock.SetField(sid, "FILE", "AAAA")
        MvxSock.SetField(sid, "PK01", strATNR) 'Attribute Number
        MvxSock.SetField(sid, "A121", tally) 'Attribute ID
        MvxSock.SetField(sid, "N096", wrkCAWE) 'Value for Attribute

        ' set the transaction name and call it
        MvxSock.SetTrimFields(sid, 0) ' Do not trim trailing spaces
        rc = MvxSock.Access(sid, APIOpr)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "CUSTEXTMI - Error Occurred: ")
        End If

        'return value so the parent function will proceed if the API call is a success
        If rc = 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function ATS101MI()
        'set M3 API parameters
        Server = "M3BE"
        Port = M3Svr.Port(frmMain.grid)
        UserID = "DTAMIGR"
        PWD = "Q190E87AG"
        APIName = "ATS101MI"
        APIOpr = "SetAttrValue"

        ' connect api
        rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)

        If IsDBNull(ds.Tables("run").Rows(j)("attribute_qty")) Then
            j = j + 1
        End If

        attQty = ds.Tables("run").Rows(j)("attribute_qty")
        attVal = Trim((ds.Tables("run").Rows(j)("attribute_value")))

        MvxSock.SetField(sid, "CONO", "100")       '
        MvxSock.SetField(sid, "ATNR", strATNR) 'attribute number
        MvxSock.SetField(sid, "ATID", attVal) 'attribute id
        MvxSock.SetField(sid, "ATVA", attQty) 'value for attribute

        If attVal = "SUPPPACK" Then
            strSuppPackID = attQty
        End If

        ' set the transaction name and call it
        MvxSock.SetTrimFields(sid, 0) ' do not trim trailing spaces
        rc = MvxSock.Access(sid, APIOpr)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "ATS101MI - error occurred: ")
            frmProgress.Close()
        End If

        j = j + 1
        attVal = (ds.Tables("Run").Rows(j)("ATTRIBUTE_VALUE")).ToString.Length
        attVal = Decimal.Parse(attVal)

        Try '
            'loop through the packs if there is multiple packs of the one line
            Do Until Decimal.Parse((Trim(ds.Tables("Run").Rows(j)("ATTRIBUTE_VALUE"))).ToString.Length) > 5

                attQty = ds.Tables("Run").Rows(j)("ATTRIBUTE_QTY")
                attVal = Trim((ds.Tables("run").Rows(j)("ATTRIBUTE_VALUE")))
                MvxSock.SetField(sid, "CONO", "100")       '
                MvxSock.SetField(sid, "ATNR", strATNR) 'Attribute Number
                MvxSock.SetField(sid, "ATID", attVal) 'Attribute ID
                MvxSock.SetField(sid, "ATVA", attQty) 'Value for Attribute

                If attVal = "SUPPPACK" Then
                    strSuppPackID = attQty
                End If

                ' set the transaction name and call it
                MvxSock.SetTrimFields(sid, 0) ' Do not trim trailing spaces
                rc = MvxSock.Access(sid, APIOpr)
                If rc <> 0 Then
                    MvxSock.ShowLastError(sid, "ATS101MI - Error Occurred: ")
                    frmProgress.Close()
                End If
                j = j + 1
                If j > ds.Tables("Run").Rows.Count - 1 Then
                    Exit Do
                End If
            Loop

        Catch ex As Exception
            'MessageBox.Show(ex.ToString)
        End Try

        If rc = 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function MMS235MI()
        Server = "M3BE"
        Port = M3Svr.Port(frmMain.grid)
        UserID = "DTAMIGR"
        PWD = "Q190E87AG"
        APIName = "MMS235MI"
        APIOpr = "GetLotItm"

        ' Connect API here
        rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "MMS235MI - Error Occurred: ")
        End If
        MvxSock.SetField(sid, "BANO", lotNO)  'Lot Number
        MvxSock.SetField(sid, "ITNO", itemNumber) 'Item Number

        ' set the transaction name and call it
        MvxSock.SetTrimFields(sid, 0) ' Do not trim trailing spaces
        rc = MvxSock.Access(sid, APIOpr)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "MMS235MI - Error Occurred: ")
        End If

        strATNR = MvxSock.GetField(sid, "ATNR")

        If rc = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function MWS070MI()

        Server = "M3BE"
        Port = M3Svr.Port(frmMain.grid)
        UserID = "DTAMIGR"
        PWD = "Q190E87AG"
        APIName = "MWS070MI"
        APIOpr = "LstTransByOrder"

        ' Connect API here
        rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "MWS070MI - Error Occurred: ")
        End If

        MvxSock.SetField(sid, "TTYP", "20")

        Select Case igType
            Case "PO"
                MvxSock.SetField(sid, "RIDN", poNumber) 'Purchase Order Number
                MvxSock.SetField(sid, "RIDL", purOrderLine) 'Purchase Order Line
            Case "DO"

            Case "CO"
                MvxSock.SetField(sid, "ITNO", itemNumber) 'itemnumber
        End Select


        MvxSock.SetTrimFields(sid, 0) ' Do not trim trailing spaces
        rc = MvxSock.Access(sid, APIOpr)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "MWS070MI - Error Occurred: ")
        End If

        While MvxSock.More(sid)
            putAway = MvxSock.GetField(sid, "REPN")
            If lotControl Then
                lotNO = MvxSock.GetField(sid, "BANO")
            End If
            MvxSock.Access(sid, Nothing)

        End While

        If rc = 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Sub dgvRun_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvRun.CellClick

        If e.RowIndex >= 0 Then 'open the form to edit the attributes of a run if the corresponding button is clicked
            If e.ColumnIndex = 0 Then
                frmAttEnt = New frmAttributeEntry(Me.attributeTable, supPackID, True, ds2, poNumber.ToString & dgvRun.Rows(e.RowIndex).Cells("RUN_NO").Value.ToString, igType)
                frmAttEnt.Show()
                frmAttEnt.MdiParent = frmMain
            ElseIf e.ColumnIndex = 1 Then 'post the run to m3
                Dim post = MessageBox.Show("You are about to post this run to M3, this cannot be undone, do you wish to proceed?", "Post Run?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                If post = 6 Then
                    PostToM3(dgvRun.Rows(e.RowIndex).Cells("RUN_NO").Value.ToString)
                End If
            ElseIf e.ColumnIndex = 2 Then 'delete the row, but prompt the user if they are sure first
                delete = MessageBox.Show("You are about to delete this run, this cannot be undone, do you wish to proceed?", "Delete Run?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                If delete = 6 Then
                    deleteRun(poNumber, dgvRun.Rows(e.RowIndex).Cells("RUN_NO").Value.ToString)
                    'if run is deleted, then update the grids on the form
                    FillRunDataGrid()
                    FillItemGrid()
                    FillSavedColumn()
                End If
            End If
        End If
    End Sub

    Private Sub dgvRun_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvRun.CellPainting
        ''put some pictures on  the buttons in the run grid to make it more user friendly
        Dim appDir As String = Application.StartupPath()
        If e.ColumnIndex = 0 AndAlso e.RowIndex >= 0 Then
            e.Paint(e.CellBounds, DataGridViewPaintParts.All)
            img = Image.FromFile(appDir & "\edit.png")
            e.Graphics.DrawImage(img, e.CellBounds.Left + 10, e.CellBounds.Top + 3, 15, 15)
            e.Handled = True
        End If

        If e.ColumnIndex = 1 AndAlso e.RowIndex >= 0 Then
            e.Paint(e.CellBounds, DataGridViewPaintParts.All)
            img = Image.FromFile(appDir & "\forward.png")
            e.Graphics.DrawImage(img, e.CellBounds.Left + 10, e.CellBounds.Top + 3, 15, 15)
            e.Handled = True
        End If

        If e.ColumnIndex = 2 AndAlso e.RowIndex >= 0 Then
            e.Paint(e.CellBounds, DataGridViewPaintParts.All)
            img = Image.FromFile(appDir & "\delete.png")
            e.Graphics.DrawImage(img, e.CellBounds.Left + 10, e.CellBounds.Top + 3, 15, 15)
            e.Handled = True
        End If
    End Sub

    Private Sub deleteRun(ByVal PurchNo As String, ByVal runNo As String)
        'delete the relevant run
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
        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
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

        'fill the datagrid with the data from the dataset table Runs
        dgvRun.DataSource = ds2.Tables("Runs")

        'create the buttons in the run datagrid
        If Not dgvRun.Columns.Contains("btn") Then
            Dim btn As New DataGridViewButtonColumn()
            dgvRun.Columns.Add(btn)
            btn.HeaderText = "Edit"
            btn.Text = "..."
            btn.Name = "btn"
            dgvRun.Columns("btn").DisplayIndex = 0
            dgvRun.Columns("btn").Width = 40

            Dim post As New DataGridViewButtonColumn()
            dgvRun.Columns.Add(post)
            post.HeaderText = "Post"
            post.Text = "M3"
            post.Name = "post"
            dgvRun.Columns("post").DisplayIndex = 1
            dgvRun.Columns("post").Width = 40

            Dim del As New DataGridViewButtonColumn()
            dgvRun.Columns.Add(del)
            del.HeaderText = "Delete"
            del.Text = "Delete"
            del.Name = "Delete"
            dgvRun.Columns("Delete").DisplayIndex = 2
            dgvRun.Columns("Delete").Width = 40

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
        ' fill the item grid with relevant data for either PO, DO or Container
        connectionString = M3Svr.ConnString(frmMain.grid)
        If igType = "PO" Then ' PO
            sql = "SELECT MPLINE.IBPUNO, MPLINE.IBPNLI, MPLINE.IBPNLS, MPLINE.IBITNO, MITMAS.MMITDS, MITMAS.MMFUDS, MPLINE.IBWHLO, MPLINE.IBFACI, MPLINE.IBSITE, MPLINE.IBORQA, MPLINE.IBPUPR, MPLINE.IBPPUN, MPLINE.IBPUUN, MPLINE.IBRVQA, MITMAS.MMUNMS, MITMAS.MMALUC, MITMAS.MMSTUN, MPLINE.IBPUSL, MITMAS.MMEVGR " & _
            "FROM MPLINE LEFT JOIN MITMAS ON MPLINE.IBITNO = MITMAS.MMITNO " & _
            "WHERE MPLINE.IBPUNO='" & Me.poNumber & "';"
        ElseIf igType = "DO" Then 'DO
            sql = "SELECT MGLINE.MRPONR, MGLINE.MRPOSX, MGLINE.MRTRNR, MGLINE.MRITNO, MITMAS.MMFUDS, MGLINE.MRTRQT, MGLINE.MRRPQT, MGLINE.MRTRNR AS IBPUNO, MGLINE.MRTRSH, MHDISH.OQDLIX, MHDISH.OQCONA AS IBWHLO, MGLINE.MRPOSX AS IBPNLS, MGLINE.MRBANO, MGLINE.MRTRPR, MITMAS.MMEVGR, MITMAS.MMITDS  " & _
                    "FROM MGLINE INNER JOIN MITMAS ON MGLINE.MRITNO = MITMAS.MMITNO INNER JOIN MHDISH ON MGLINE.MRTRNR = MHDISH.OQRIDN and MGLINE.MRWHLO = MHDISH.OQWHLO " & _
                    "WHERE MGLINE.MRTRNR ='" & Me.poNumber & "' " & _
                    "ORDER BY MGLINE.MRPONR;"
        ElseIf igType = "CO" Then 'Container
            sql = "SELECT MITMAS.MMEVGR, MPLINE.IBPNLI, MPLINE.IBPITD AS MMITDS, MPLINE.IBPNLI, MPLINE.IBPNLS , MPLIND.ICSUDO, MPLIND.ICWHLO, MPLINE.IBRVQA, MPLINE.IBITNO, MPLINE.IBPITT AS MMFUDS, MPLIND.ICRPQA, MPLIND.ICPACN, MPLIND.ICPACN AS IBPUNO, MPLIND.ICPUNO, MPLIND.ICPNLI, MPLIND.ICPNLX, MPLIND.ICWHLO AS IBWHLO, MPLINE.IBPUSL  " & _
                  "FROM MPLIND, MPLINE, MITMAS " & _
                  "WHERE MITMAS.MMITNO = MPLINE.IBITNO AND MPLINE.IBPUNO = MPLIND.ICPUNO AND MPLINE.IBPNLI = MPLIND.ICPNLI AND MPLINE.IBPNLS = MPLIND.ICPNLS AND ICPACN = '" & Me.poNumber & "' AND MPLIND.ICPUOS = 40  AND ICSUDO IN (SELECT TOP 1 ICSUDO " & _
                                                                                                                                                                                                    "FROM MPLIND " & _
                                                                                                                                                                                                    "WHERE ICPACN = '" & Me.poNumber & "' " & _
                                                                                                                                                                                                    "ORDER BY ICSUDO DESC);"
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
                If ds2.Tables(0).Columns.Contains("IBITNO") Then
                    ds2.Tables(0).Columns.Remove("IBITNO")
                    ds2.Tables(0).Columns.Remove("IBPNLI")
                End If
                ds2.Tables(0).Columns("MRITNO").ColumnName = "IBITNO"
                ds2.Tables(0).Columns("MRPONR").ColumnName = "IBPNLI"

                If Not ds2.Tables(0).Columns.Contains("LabelQty") Then
                    ds2.Tables(0).Columns.Add("LabelQty")
                    ds2.Tables(0).Columns.Add("AppCharge")
                End If
            End If

            dgvItems.DataSource = ds2.Tables(0)

            'Pretty up the itemgrid, most of the data the user doesn't need to see and give more human readable names to the fields that are visible.

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
                    .Columns("MMITDS").Visible = False
                    .Columns("MMEVGR").Visible = False
                    .Columns("IBRVQA").DefaultCellStyle.Format = "N0"
                    .Columns("IBORQA").DefaultCellStyle.Format = "N0"
                    .Columns("IBPNLS").HeaderCell.Value = "Sub Line"
                    .Columns("IBITNO").HeaderCell.Value = "Product ID"
                    .Columns("MMFUDS").HeaderCell.Value = "Descripton"
                    .Columns("IBORQA").HeaderCell.Value = "Order Qty"
                    .Columns("IBPNLI").HeaderCell.Value = "Line"
                    .Columns("IBRVQA").HeaderCell.Value = "Qty Receipted"
                    .Columns("IBPUSL").Visible = False
                    .Columns("IBPNLS").Width = 80
                    .Columns("IBITNO").Width = 135
                    .Columns("MMFUDS").Width = 210
                    .Columns("IBORQA").Width = 80
                    .Columns("IBPNLI").Width = 40
                    .Columns("IBPUUN").Width = 50
                End With
            ElseIf igType = "CO" Then
                With dgvItems
                    .Columns("Qty Received").Visible = False
                    .Columns("ICPNLI").Visible = False
                    .Columns("IBPNLI").Visible = False
                    .Columns("IBRVQA").HeaderCell.Value = "Qty Receipted"
                    .Columns("IBPNLS").Visible = False
                    .Columns("ICPACN").Visible = False
                    .Columns("IBPUNO").Visible = False
                    .Columns("MMEVGR").Visible = False
                    .Columns("MMITDS").Visible = False
                    .Columns("IBPNLI1").Visible = False
                    .Columns("ICSUDO").Visible = False
                    .Columns("IBITNO").Visible = False
                    .Columns("ICWHLO").Visible = False
                    .Columns("IBWHLO").Visible = False
                    .Columns("ICPUNO").Visible = False
                    .Columns("ICPNLX").Visible = False
                    .Columns("ICPNLI").Visible = False
                    .Columns("MMFUDS").HeaderCell.Value = "Descripton"
                    .Columns("ICRPQA").HeaderCell.Value = "Order Qty"
                    .Columns("MMFUDS").Width = 210
                    .Columns("IBPUSL").Visible = False
                End With

                If dgvItems.Columns.Contains("INITNO") Then
                    dgvItems.Columns("INITNO").Visible = False
                End If
            ElseIf igType = "DO" Then
                With dgvItems
                    .Columns("MMFUDS").HeaderCell.Value = "Descripton"
                    .Columns("MMFUDS").Width = 210
                    .Columns("IBPNLI").Visible = False
                    .Columns("IBPNLS").Visible = False
                    .Columns("MRTRNR").Visible = False
                    .Columns("IBITNO").Visible = False
                    .Columns("OQDLIX").Visible = False
                    .Columns("IBWHLO").Visible = False
                    .Columns("MRTRSH").Visible = False
                    .Columns("IBPUNO").Visible = False
                    .Columns("LabelQty").Visible = False
                    .Columns("AppCharge").Visible = False
                    .Columns("MRBANO").Visible = False
                    .Columns("MRTRPR").Visible = False
                    .Columns("MRTRQT").HeaderCell.Value = "Order Qty"
                    .Columns("MRRPQT").HeaderCell.Value = "Qty Receipted"
                    .Columns("LabelQty").DisplayIndex = 2

                    If Not dgvItems.Columns.Contains("chk") Then
                        Dim chk As New DataGridViewCheckBoxColumn()
                        .Columns.Add(chk)
                        chk.HeaderText = "Print Labels"
                        chk.Name = "chk"
                        .Columns("chk").DisplayIndex = 2
                        .Columns("chk").Width = 50
                    End If
                End With

                If dgvItems.Columns.Contains("MRPOSX") Then
                    dgvItems.Columns("MRPOSX").Visible = False
                End If

                If dgvItems.Columns.Contains("MRPONR") Then
                    dgvItems.Columns("MRPONR").Visible = False
                End If

                If dgvItems.Columns.Contains("MRITNO") Then
                    dgvItems.Columns("MRITNO").Visible = False
                End If


            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try

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
            connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"

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

        If Len(intday.ToString) = 1 Then
            intday = "0" & intday
        End If
        Dim intMonth As String = currentDate.Month
        If Len(intMonth.ToString) = 1 Then
            intMonth = "0" & intMonth
        End If

        Dim strRecDate = intday & intMonth & intYear
        warehouseID = Trim(ds2.Tables(0).Rows(line)("IBWHLO"))
        branch = warehouseID
        If Strings.Left(warehouseID, 1) = "9" Then
            warehouseID = "900"
        End If
        ProductID = Trim(ds2.Tables("Product Table PO").Rows(line)("MMITNO"))
        Desc = ds2.Tables("Product Table PO").Rows(line)("MMITDS")

        If IsNothing(Trim(ds2.Tables("Product Table PO").Rows(line)("MMITNO"))) = True Then
            typeFlag = ""
        Else
            typeFlag = ds2.Tables("Product Table PO").Rows(line)("MMEVGR")
        End If

        Dim FileNo = FreeFile()
        Dim File2 = FreeFile()
        ChDir(Environment.GetEnvironmentVariable("temp"))

        attributeValue = ds.Tables("Packs").Rows(packsRow)("PACKTYPE")

        If attributeValue.ToString.Contains("FIXED") Or attributeValue.ToString.Contains("MIXED") Then
            lotControl = True
            loopcount = 1
            If Not igType = "DO" Then
                tally = ds.Tables("Packs").Rows(packsRow)("TALLY")
                wrkCAWE = ds.Tables("Packs").Rows(packsRow)("CATCHWEIGHT")
            Else
                tally = GetTally(itemNumber, lotNO)
                wrkCAWE = GetCatchWeight(itemNumber, lotNO)
            End If

            perpack = wrkCAWE

            ds.Tables("Packs").Rows(packsRow)("perPack") = perpack
            ds.Tables("Packs").Rows(packsRow)("shortPack") = shortpack
            ds.Tables("Packs").Rows(packsRow)("lotNo") = lotNO
            ds.Tables("Packs").Rows(packsRow)("putAway") = putAway

        Else
            lotControl = False

            If attributeValue = "BATCH" Then
                loopcount = 1
                perpack = ds.Tables("Packs").Rows(packsRow)("ITEMQTY") / ds.Tables("Packs").Rows(packsRow)("PACKQTY")
            Else
                loopcount = ds.Tables("Packs").Rows(packsRow)("PACKQTY")
                itemQty = ds.Tables("Packs").Rows(packsRow)("ITEMQTY")

                perpack = itemQty / loopcount
                If Not itemQty Mod loopcount = 0 Then
                    perpack = Math.Ceiling(perpack)
                    shortpack = itemQty - ((loopcount - 1) * perpack)
                Else
                    shortpack = perpack
                End If
            End If



            ds.Tables("Packs").Rows(packsRow)("perPack") = perpack
            ds.Tables("Packs").Rows(packsRow)("shortPack") = shortpack
            ds.Tables("Packs").Rows(packsRow)("lotNo") = lotNO
            ds.Tables("Packs").Rows(packsRow)("putAway") = putAway
        End If

        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect " & _
                                "Timeout=15;Encrypt=False;TrustServerCertificate=False"
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

        If ds2.Tables("LblPrint").Rows.Count > 0 Then
            printerport = Trim((ds2.Tables("LblPrint").Rows(0)("Printer_Port")))
            printerid = Trim(ds2.Tables("LblPrint").Rows(0)("Printer_IP"))
            login = Trim(ds2.Tables("LblPrint").Rows(0)("Login"))
            pw = Trim(ds2.Tables("LblPrint").Rows(0)("Password"))

            ds2.Tables.Remove("LBLPrint")

            barcode = GetBarcode(ProductID)

            connectionString = M3Svr.ConnString(frmMain.grid)
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
            Thread.Sleep(5000)
            FileOpen(FileNo, "ftp-cmd.txt", OpenMode.Output)
            Print(FileNo, "open " & printerid & vbCrLf)
            Print(FileNo, login & vbCrLf)
            Print(FileNo, pw & vbCrLf)
            Print(FileNo, "cd dest/" & printerport & vbCrLf)
            Print(FileNo, "send " & Environment.GetEnvironmentVariable("temp") & "\labelprint.txt" & vbCrLf)
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
                Print(File2, Chr(27) & "%3" & vbCrLf)
                Print(File2, Chr(27) & "V120" & Chr(27) & "H731" & Chr(27) & "CC2" & Chr(27) & "PY050" & vbCrLf)
                Print(File2, Chr(27) & "V100" & Chr(27) & "H426" & Chr(27) & "L0202" & Chr(27) & "XMGUNNERSEN" & vbCrLf)

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

                'Print(File2, Chr(27) & "V1250" & Chr(27) & "H600" & Chr(27) & "L0102" & Chr(27) & "XMLocations - Prime:" & vbCrLf)
                'Print(File2, Chr(27) & "V1050" & Chr(27) & "H700" & Chr(27) & "L0102" & Chr(27) & "XMPick:" & vbCrLf)

                'Print(File2, Chr(27) & "V500" & Chr(27) & "H130" & Chr(27) & "BT101030103" & Chr(27) & "BW03100*" & perpack & "*" & vbCrLf)
                Print(File2, Chr(27) & "V1900" & Chr(27) & "H540" & Chr(27) & "L0102" & Chr(27) & "XMQty Per Pack:" & vbCrLf)
                Print(File2, Chr(27) & "&" & vbCrLf)
                Print(File2, Chr(27) & "Z" & vbCrLf)
                Print(File2, Chr(27) & "A" & vbCrLf)
                Print(File2, Chr(27) & "%3" & vbCrLf)
                Print(File2, Chr(27) & "/" & vbCrLf)
                Print(File2, Chr(27) & "V2000" & Chr(27) & "H726" & Chr(27) & "L0102" & Chr(27) & "XM" & warehouseID & "-" & strRecDate & vbCrLf)

                If lotControl Then
                    Print(File2, Chr(27) & "V500" & Chr(27) & "H566" & Chr(27) & "L0102" & Chr(27) & "XM" & ProductID & vbCrLf)
                    Print(File2, Chr(27) & "V500" & Chr(27) & "H726" & Chr(27) & "L0104" & Chr(27) & "XM" & Desc & vbCrLf)
                    Print(File2, Chr(27) & "V500" & Chr(27) & "H626" & Chr(27) & "L0202" & Chr(27) & "XS" & tally & vbCrLf) ' fixed value here
                    If loopval <= 1 Then
                        Print(File2, Chr(27) & "V500" & Chr(27) & "H226" & Chr(27) & "L0202" & Chr(27) & "XSSupplier Pack ID: " & strSuppPackID & vbCrLf)
                    End If
                Else
                    Print(File2, Chr(27) & "V500" & Chr(27) & "H476" & Chr(27) & "L0204" & Chr(27) & "XM" & ProductID & vbCrLf)
                    Print(File2, Chr(27) & "V500" & Chr(27) & "H726" & Chr(27) & "L0205" & Chr(27) & "XM" & Desc & vbCrLf)
                End If

                'Print(File2, Chr(27) & "V750" & Chr(27) & "H580" & Chr(27) & "L0203" & Chr(27) & "XM" & bin1 & vbCrLf)
                'Print(File2, Chr(27) & "V750" & Chr(27) & "H680" & Chr(27) & "L0203" & Chr(27) & "XM" & bin2 & vbCrLf)
                ' Print(File2, Chr(27) & "V1050" & Chr(27) & "H400" & Chr(27) & "L0203" & Chr(27) & "XM" & bin3 & vbcrlf) ' bin 3 is facings
                Print(File2, Chr(27) & "V2160" & Chr(27) & "H540" & Chr(27) & "L0203" & Chr(27) & "XM" & perpack & vbCrLf)
                Print(File2, Chr(27) & "V950" & Chr(27) & "H146" & Chr(27) & "L0202" & Chr(27) & "XSPutAway Number: " & putAway & vbCrLf)
                Print(File2, Chr(27) & "V950" & Chr(27) & "H116" & Chr(27) & "BT101030103" & Chr(27) & "BW03100*" & putAway & "*" & vbCrLf)
                Print(File2, Chr(27) & "V2000" & Chr(27) & "H406" & Chr(27) & "L0202" & Chr(27) & "XSProduct ID" & vbCrLf)
                Print(File2, Chr(27) & "V2000" & Chr(27) & "H376" & Chr(27) & "BQ3015,1" & Trim(barcode) & vbCrLf)
                Print(File2, Chr(27) & "V2000" & Chr(27) & "H36" & Chr(27) & "XU" & Trim(barcode) & vbCrLf)
                If lotControl Then
                    Print(File2, Chr(27) & "V1400" & Chr(27) & "H456" & Chr(27) & "L0202" & Chr(27) & "XSPack ID" & vbCrLf)
                    Print(File2, Chr(27) & "V1400" & Chr(27) & "H426" & Chr(27) & "BQ3009,1" & Trim(lotNO) & vbCrLf)
                    Print(File2, Chr(27) & "V450" & Chr(27) & "H446" & Chr(27) & "L0404" & Chr(27) & "XB1" & Trim(lotNO) & vbCrLf)
                End If
                Print(File2, Chr(27) & "Q1" & vbCrLf)
                Print(File2, Chr(27) & "Z" & vbCrLf)
                loopcount = loopcount - 1
            Loop
            packsRow = packsRow + 1
            FileClose(File2)

            Dim strTemp As String = Environment.GetEnvironmentVariable("temp")
            'Dim strTemp As String = Application.StartupPath
            Dim strFtpFile As String = strTemp & "\ftp-cmd.txt"
            Dim WINDIR As String = Environment.GetEnvironmentVariable("windir")
            Dim exec As String = WINDIR & "\system32\ftp.exe"
            Dim strArgs As String = " -s:" & strFtpFile

            Dim startFTP = New ProcessStartInfo
            startFTP.WindowStyle = ProcessWindowStyle.Hidden
            startFTP.FileName = exec
            startFTP.Arguments = strArgs
            Process.Start(startFTP)
        Else
            MessageBox.Show("Label Printing unavailable at this terminal, please contact help desk.", "Cannot Print Label", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

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

        connectionString = M3Svr.ConnString(frmMain.grid)
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
            MessageBox.Show("No barcode available for product" & Environment.NewLine & Desc)
        End If

        ds2.Tables.Remove("MITPOP")
        Return barcode


    End Function

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        'Handle Ctrl+N to create a new delivery run
        If e.KeyData = Keys.Control + Keys.N Then
            tsbNewRun_Click(sender, e)
        End If
    End Sub

    Private Function SaveRunPackRows()
        ' save some data into a seperate table in the gunnersen database for later retrieval for labelprinting
        Dim dbConnString As String = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
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

    Private Sub deleteRunLine(ByVal PurchNo As String, ByVal runNo As String, Optional ByVal AttItemNo As String = "")
        'Delete a run, first delete the lien attributes, then delete the header (dumb table names...do more planning next time)
        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"
        Try
            If AttItemNo = "" Then
                sql = "DELETE " & _
                  " FROM Attributes" & _
                  " WHERE RUN_NUMBER = '" & PurchNo & runNo & "' AND ROWNUMBER = '" & rowNumber & "';"
            Else
                sql = "DELETE " & _
                  " FROM Attributes" & _
                  " WHERE ATTRIBUTE_ITEM='" & AttItemNo & "';"
            End If

            connection = New SqlConnection(connectionString)

            command = New SqlCommand(sql, connection)
            connection.Open()
            command.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub btnCreateNewDD_Click(sender As Object, e As EventArgs) Handles btnCreateNewDD.Click
        tsbNewRun_Click(sender, e)
    End Sub

    Private Function PPS001MI()
        Server = "M3BE"
        Port = M3Svr.Port(frmMain.grid)
        UserID = "DTAMIGR"
        PWD = "Q190E87AG"
        APIName = "PPS001MI"
        APIOpr = "Receipt"

        rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "Error Occured: ")
        End If

        todayDate = Date.Today().ToString("yyyyMMdd")
        respUser = UCase(modFunctions.GetActiveDirUserDetails(Environment.UserName))
        rowNumber = ds.Tables("Packs").Rows(i)("ROWNUMBER")
        attributeValue = ds.Tables("Packs").Rows(i)("PACKTYPE")

        If attributeValue = "MIXEDPACK" Or attributeValue.ToString.Contains("FIXED") Then
            catchWeight = ds.Tables("Packs").Rows(i)("CATCHWEIGHT")
            wrkCAWE = catchWeight
            tally = ds.Tables("Packs").Rows(i)("Tally")
        ElseIf Not attributeValue = "STANDARD" Then
            lotNO = ds.Tables("Packs").Rows(i)("Tally")
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

        If attributeValue = "BATCH" Then
            itemQty = ds.Tables("Packs").Rows(i)("ITEMQTY") / ds.Tables("Packs").Rows(i)("PACKQTY")
        Else
            itemQty = ds.Tables("Packs").Rows(i)("ITEMQTY")
        End If



        MvxSock.SetField(sid, "CONO", "100")
        MvxSock.SetField(sid, "TRDT", todayDate)
        MvxSock.SetField(sid, "RESP", respUser)
        MvxSock.SetField(sid, "PUNO", poNumber)
        MvxSock.SetField(sid, "PNLI", purOrderLine)
        MvxSock.SetField(sid, "PNLS", PNLS)



        If attributeValue.Contains("IX") Then
            MvxSock.SetField(sid, "CAWE", wrkCAWE)
            MvxSock.SetField(sid, "RVQA", 1)
            lotControl = True
        Else
            Select Case attributeValue
                Case "STANDARD"
                    MvxSock.SetField(sid, "CAWE", wrkCAWE)
                    MvxSock.SetField(sid, "RVQA", itemQty)
                Case "BATCH"
                    MvxSock.SetField(sid, "CAWE", wrkCAWE)
                    MvxSock.SetField(sid, "RVQA", itemQty)
                    MvxSock.SetField(sid, "BANO", lotNO)
                Case "EXPIRY"
                    MvxSock.SetField(sid, "CAWE", wrkCAWE)
                    MvxSock.SetField(sid, "RVQA", itemQty)
                    MvxSock.SetField(sid, "BANO", lotNO)
                    MvxSock.SetField(sid, "PRDT", lotNO)
            End Select
            lotControl = False
        End If


        MvxSock.SetTrimFields(sid, 0)  ' Do not trim trailing spaces
        rc = MvxSock.Access(sid, APIOpr)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, " PPS001MI - Error Occurred: ")
        End If

        If rc = 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function ZPP363MI()
        Server = "M3BE"
        Port = M3Svr.Port(frmMain.grid)
        UserID = "DTAMIGR"
        PWD = "Q190E87AG"
        APIName = "ZPP363MI"
        APIOpr = "ReceiveDetail"

        rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "Error Occured: ")
        End If

        MvxSock.SetField(sid, "CONO", "100")
        MvxSock.SetField(sid, "ITNO", itemNumber)
        MvxSock.SetField(sid, "WHLO", lblWarehouseID.Text)
        MvxSock.SetField(sid, "PACN", poNumber)
        MvxSock.SetField(sid, "CAMU", poNumber)
        MvxSock.SetField(sid, "SUNO", SUNO)
        MvxSock.SetField(sid, "SUTY", "0")
        MvxSock.SetField(sid, "SUDO", SUDO)
        MvxSock.SetField(sid, "DNDT", DNDT)

        If attributeValue.Contains("IX") Then
            MvxSock.SetField(sid, "CAWE", wrkCAWE)
            lotNO = ds.Tables("Packs").Rows(i)("Tally")
            MvxSock.SetField(sid, "BANO", lotNO)
            lotControl = True
        Else
            Select Case attributeValue
                Case "STANDARD"
                    MvxSock.SetField(sid, "CAWE", wrkCAWE)
                Case "BATCH"
                    lotNO = ds.Tables("Packs").Rows(i)("Tally")
                    MvxSock.SetField(sid, "BANO", lotNO)
                Case "EXPIRY"
                    lotNO = ds.Tables("Packs").Rows(i)("Tally")
                    Dim exp = Convert.ToInt32(lotNO.Substring(0, 4).Substring(3, 1))
                    exp = exp + 2
                    exp = lotNO.Remove(3, 1).Insert(3, exp)

                    MvxSock.SetField(sid, "BANO", lotNO)
                    MvxSock.SetField(sid, "EXPI", exp)
            End Select
        End If


        MvxSock.SetTrimFields(sid, 0)  ' Do not trim trailing spaces
        rc = MvxSock.Access(sid, APIOpr)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "ZPP363MI - Error Occurred: ")
        End If

        If rc = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function MHS850()
        Server = "M3BE"
        Port = M3Svr.Port(frmMain.grid)
        UserID = "DTAMIGR"
        PWD = "Q190E87AG"
        APIName = "MHS850MI"
        APIOpr = "AddDOReceipt"

        rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "Error Occured: ")
        End If

        Dim WHLO = Trim(ds2.Tables(0).Rows(rowNumber)("IBPNLI"))
        LineNo = Trim(ds2.Tables(0).Rows(rowNumber)("IBPNLI"))
        Dim intLineNo As Integer = Convert.ToInt32(LineNo)

        If attributeValue = "BATCH" Then
            itemQty = ds.Tables("Packs").Rows(i)("ITEMQTY") / ds.Tables("Packs").Rows(i)("PACKQTY")
        Else
            itemQty = ds.Tables("Packs").Rows(i)("ITEMQTY")
        End If

        Dim RIDI = ds2.Tables(0).Rows(i)("OQDLIX")
        lotNO = ds2.Tables(0).Rows(i)("MRBANO")
        catchWeight = GetCatchWeight(itemNumber, lotNO)
        Dim doubCatchWeight = Math.Round(Convert.ToDouble(catchWeight), 2)

        MvxSock.SetField(sid, "PRFL", "*EXE")
        MvxSock.SetField(sid, "CONO", "100")
        MvxSock.SetField(sid, "WHLO", lblCreditorID.Text)
        MvxSock.SetField(sid, "E0PA", "GUN")
        MvxSock.SetField(sid, "E065", "GUN")
        MvxSock.SetField(sid, "TWHL", lblWarehouseID.Text)
        MvxSock.SetField(sid, "ITNO", itemNumber)
        MvxSock.SetField(sid, "WHSL", "RECEIVING")
        MvxSock.SetField(sid, "QTY", itemQty)
        MvxSock.SetField(sid, "RIDN", poNumber)
        MvxSock.SetField(sid, "RIDL", intLineNo)
        MvxSock.SetField(sid, "RIDI", RIDI)
        MvxSock.SetField(sid, "BANO", lotNO)
        MvxSock.SetField(sid, "CAWE", doubCatchWeight)


        MvxSock.SetTrimFields(sid, 0)  ' Do not trim trailing spaces
        rc = MvxSock.Access(sid, APIOpr)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "MHS850 - Error Occurred: ")
        End If

        If rc = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetCatchWeight(ByVal ITNO, ByVal BANO)
        connectionString = M3Svr.ConnString(frmMain.grid)
        sql = "SELECT MLCAWE " & _
              "FROM MITLOC " & _
                "WHERE MLITNO = '" & ITNO & "' AND MLBANO = '" & BANO & "';"
        connection = New SqlConnection(connectionString)

        If Not ds2.Tables.Contains("CatchWeight") Then
            ds2.Tables.Add("CatchWeight")
        Else
            ds2.Tables("CatchWeight").Clear()
        End If

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds2.Tables("CatchWeight"))
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MsgBox(ex)
        End Try

        Dim cw As Decimal = 0

        If ds2.Tables("CatchWeight").Rows.Count > 0 Then
            cw = Convert.ToDecimal(ds2.Tables("CatchWeight").Rows(0)("MLCAWE"))
        End If

        Return Math.Round(cw, 2)

    End Function

    Private Function GetTally(ByVal ITNO, ByVal BANO)
        connectionString = M3Svr.ConnString(frmMain.grid)
        sql = "select F1A121 " & _
              "from CUGEX1 " & _
              "where F1FILE = 'AAAA' and F1PK01 = (select LMATNR from MVXJDTA.MILOMA " & _
                                                  "where LMCONO = F1CONO and LMITNO = '" & ITNO & "' and LMBANO = '" & BANO & "')"

        connection = New SqlConnection(connectionString)

        If Not ds2.Tables.Contains("Tally") Then
            ds2.Tables.Add("Tally")
        Else
            ds2.Tables("Tally").Clear()
        End If

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds2.Tables("Tally"))
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MsgBox(ex)
        End Try

        Dim tall = ""

        If ds2.Tables("Tally").Rows.Count > 0 Then
            tall = ds2.Tables("Tally").Rows(0)("F1A121")
        End If

        Return tall

    End Function

    Private Function PPS320()
        Server = "M3BE"
        Port = M3Svr.Port(frmMain.grid)
        UserID = "DTAMIGR"
        PWD = "Q190E87AG"
        APIName = "PPS320MI"
        APIOpr = "PutawayPO"

        respUser = UCase(modFunctions.GetActiveDirUserDetails(Environment.UserName))
        catchWeight = GetCatchWeight(itemNumber, lotNO)
        itemQty = ds.Tables("PACKS").Rows(i)("ITEMQTY")

        rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "Error Occured: ")
        End If

        MvxSock.SetField(sid, "REPN", putAway)
        MvxSock.SetField(sid, "RESP", respUser)
        MvxSock.SetField(sid, "RPQA", itemQty)
        MvxSock.SetField(sid, "WHSL", "PUTAWAY")
        MvxSock.SetField(sid, "BANO", lotNO)
        MvxSock.SetField(sid, "CAWE", catchWeight)

        MvxSock.SetTrimFields(sid, 0)  ' Do not trim trailing spaces
        rc = MvxSock.Access(sid, APIOpr)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "PPS320 - Error Occurred: ")
        End If

        If rc = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function WaitForFile(ByVal fullPath)
        Dim numTries As Integer
        While (True)
            numTries = numTries + 1
            Try
                Dim fs = New FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 100)
                fs.ReadByte()

            Catch ex As Exception

                If numTries > 10 Then
                    Return False
                End If

                System.Threading.Thread.Sleep(500)
            End Try
            Return True
        End While

    End Function

    Private Sub AppDelCharge()

        Dim totCost As Decimal = 0
        Dim Perc As New List(Of Decimal)
        Dim appCost As New List(Of Decimal)
        Dim appCostInd As New List(Of Decimal)
        Dim lineCost As New List(Of Decimal)

        For c = 0 To dgvItems.RowCount - 1
            lineCost.Add(dgvItems.Rows(c).Cells("MRTRPR").Value * dgvItems.Rows(c).Cells("MRTRQT").Value)
        Next

        For c = 0 To dgvItems.RowCount - 1
            totCost = totCost + lineCost(c)
        Next

        For c = 0 To dgvItems.RowCount - 1
            Perc.Add(lineCost(c) / totCost)
        Next

        For c = 0 To dgvItems.RowCount - 1
            appCost.Add(Perc(c) * txtFreightCharge.Text)
        Next

        For c = 0 To dgvItems.RowCount - 1
            appCostInd.Add(appCost(c) / dgvItems.Rows(c).Cells("MRTRQT").Value)
            dgvItems.Rows(c).Cells("AppCharge").Value = appCostInd(c) + dgvItems.Rows(c).Cells("MRTRPR").Value
        Next

    End Sub

    Private Function WSAverageCostAdjust(ByVal avgCost As Decimal)

        Dim extQty = arrExtStock(1)
        Dim extAvgCost = arrExtStock(0)
        Dim newQty = arrNewStock(1)

        Dim newCost = Math.Round(((extQty * extAvgCost) + (avgCost * itemQty)) / newQty, 5)

        ' create the webservices clinet and set values.
        Dim client As New GLUtilitiesClient
        Dim binding As New System.ServiceModel.BasicHttpBinding
        binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic
        binding.MaxReceivedMessageSize = 25 * 1024 * 1024
        client.Endpoint.Binding = binding
        client.ClientCredentials.UserName.UserName = "DTAMIGR"
        client.ClientCredentials.UserName.Password = "Q190E87AG"

        'create lws header
        Dim header = New InwardGoodsVB.wsGLUtlities.lws
        header.user = "DTAMIGR"
        header.password = "Q190E87AG"
        Try
            'Create a requests item
            Dim avgCostAdjustItem = New wsGLUtlities.ct_0

            avgCostAdjustItem.ItemNumber = itemNumber
            avgCostAdjustItem.Facility = lblCreditorID.Text
            avgCostAdjustItem.AverageCost = newCost


            'create collection of items(even though there is only one item)
            Dim collection = New Average_Cost_AdjustType
            collection.CAS370 = avgCostAdjustItem

            'put data to M3 webservice
            client.Average_Cost_Adjust(header, collection)

        Catch ex As Exception
            MessageBox.Show("Average cost adjustment has failed, contact help desk immediatley", "Cost Adjustment Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            frmProgress.Close()
        End Try

        If rc = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetAvgCostandStock(ByVal ITNO, ByVal WHLO)
        connectionString = M3Svr.ConnString(frmMain.grid)
        sql = "SELECT M9ITNO, M9APPR, MBSTQT " & _
              "FROM MITFAC, MITBAL " & _
              "WHERE MITFAC.M9ITNO = MITBAL.MBITNO AND MITBAL.MBWHLO = MITFAC.M9FACI AND M9ITNO = '" & ITNO & "' AND M9FACI = '" & WHLO & "';"
        connection = New SqlConnection(connectionString)

        If Not ds2.Tables.Contains("AvgCost") Then
            ds2.Tables.Add("AvgCost")
        Else
            ds2.Tables("AvgCost").Clear()
        End If

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds2.Tables("AvgCost"))
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MsgBox(ex)
        End Try

        Dim avg As New ArrayList

        If ds2.Tables("AvgCost").Rows.Count > 0 Then
            avg.Add(ds2.Tables("AvgCost").Rows(0)("M9APPR"))
            avg.Add(ds2.Tables("AvgCost").Rows(0)("MBSTQT"))
        End If

        Return avg

    End Function

    Private Function PPS200MI(PONo As String) As Boolean
        Server = "M3BE"
        Port = M3Svr.Port(frmMain.grid)
        UserID = "DTAMIGR"
        PWD = "Q190E87AG"
        APIName = "PPS200MI"
        APIOpr = "UpdHead"

        Dim AGRA = GetFXRate(PONo)

        rc = MvxSock.Connect(sid, Server, Port, UserID, PWD, APIName, Nothing)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "Error Occured: ")
        End If

        MvxSock.SetField(sid, "PUNO", PONo)
        MvxSock.SetField(sid, "AGRA", AGRA)
        MvxSock.SetField(sid, "CUTE", "1")

        MvxSock.SetTrimFields(sid, 0)  ' Do not trim trailing spaces
        rc = MvxSock.Access(sid, APIOpr)
        If rc <> 0 Then
            MvxSock.ShowLastError(sid, "PPS200 - Error Occurred: ")
        End If

        If rc = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetFXRate(IAAGRA As String)
        connectionString = M3Svr.ConnString(frmMain.grid)
        sql = "SELECT IAPUNO, IAAGRA " +
              "FROM MPHEAD " +
              "WHERE IAPUNO = '" + IAAGRA + "';"
        connection = New SqlConnection(connectionString)

        If Not ds2.Tables.Contains("FXRate") Then
            ds2.Tables.Add("FXRate")
        Else
            ds2.Tables("FXRate").Clear()
        End If

        Try
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(ds2.Tables("FXRate"))
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch ex As Exception
            MsgBox(ex)
        End Try

        Return ds2.Tables("FXRate").Rows(0)("IAAGRA").ToString
    End Function

End Class