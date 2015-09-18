Imports System.Data.SqlClient
Imports System.Threading

Public Class frmReprint
    Dim M3Svr As M3Point.M3Point = New M3Point.M3Point
    'variables used to retriving data from Purchase orders out of M3
    Dim connectionString As String
    Dim connection As SqlConnection
    Dim command As SqlCommand
    Dim adapter As New SqlDataAdapter
    Dim dtReprint As New DataTable
    Dim dtLines As New DataTable
    Dim sql As String
    Dim attributeValue As String
    Dim lotControl As Boolean
    Dim Desc As Object
    Dim loopcount As Integer
    Dim igType As String
    Dim tally As Object
    Dim wrkCAWE As Object
    Dim perpack As Object
    Dim itemQty As Object
    Dim shortpack As Object
    Dim putAway As String
    Dim lotNO As String
    Dim ds2 As New DataSet

    Private Sub frmReprint_Load(sender As Object, e As EventArgs) Handles Me.Load


        'sql to retrieve purchase orders from M3 


        connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect " & _
                            "Timeout=15;Encrypt=False;TrustServerCertificate=False"
        sql = "SELECT LEFT(RUN_NUMBER,LEN(RUN_NUMBER)-1) AS PO " & _
              "FROM LabelPrinting " & _
              "GROUP BY LEFT(RUN_NUMBER,LEN(RUN_NUMBER)-1)" & _
              "ORDER BY LEFT(RUN_NUMBER,LEN(RUN_NUMBER)-1) DESC"

        connection = New SqlConnection(connectionString)

        Try
            'Open the database, fill recordset and close
            connection.Open()
            command = New SqlCommand(sql, connection)
            adapter.SelectCommand = command
            adapter.Fill(dtReprint)
            adapter.Dispose()
            command.Dispose()
            connection.Close()
        Catch

        End Try
        lstReprint.DisplayMember = "PO"
        lstReprint.ValueMember = "PO"
        lstReprint.DataSource = dtReprint
    End Sub

    Private Sub lstReprint_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstReprint.SelectedIndexChanged
        
    End Sub

    Public Function PrintLabels(ByVal lineNumber As Integer)
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
        Dim currentDate As DateTime = DateTime.Now
        Dim intday As String = currentDate.Day
        Dim intYear As String = currentDate.Year
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
        warehouseID = Trim(dtLines.Rows(lineNumber).Item("Warehouse"))
        If Not IsDBNull(dtLines.Rows(lineNumber).Item("putAway")) Then
            putAway = Trim(dtLines.Rows(lineNumber).Item("putAway"))
        End If


        branch = warehouseID
        If Strings.Left(warehouseID, 1) = "9" Then
            warehouseID = "900"
        End If
        ProductID = Trim(dtLines.Rows(lineNumber).Item("ItemNO"))
        Desc = Trim(dtLines.Rows(lineNumber).Item("Descrip"))

        typeFlag = Trim(dtLines.Rows(lineNumber).Item("typeFlag"))

        Dim FileNo = FreeFile()
        Dim File2 = FreeFile()
        ChDir(Environment.GetEnvironmentVariable("temp"))

        attributeValue = Trim(dtLines.Rows(lineNumber).Item("PACKTYPE"))

        If attributeValue.ToString.Contains("FIXED") Or attributeValue.ToString.Contains("MIXED") Then
            lotControl = True
            loopcount = 1
            tally = Trim(dtLines.Rows(lineNumber).Item("TALLY"))
            wrkCAWE = Trim(dtLines.Rows(lineNumber).Item("CATCHWEIGHT"))
            lotNO = Trim(dtLines.Rows(lineNumber).Item("lotNo"))
            perpack = wrkCAWE
        Else
            lotControl = False
            loopcount = Trim(dtLines.Rows(lineNumber).Item("PACKQTY"))
            itemQty = Trim(dtLines.Rows(lineNumber).Item("ITEMQTY"))

            perpack = itemQty / loopcount
            If Not itemQty Mod loopcount = 0 Then
                perpack = Math.Ceiling(perpack)
                shortpack = itemQty - ((loopcount - 1) * perpack)
            Else
                shortpack = perpack
            End If
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

            'barcode = GetBarcode(ProductID)

            If ds2.Tables.Contains("MITBAL") Then
                ds2.Tables("MITBAL").Clear()
            Else
                ds2.Tables.Add("MITBAL")
            End If


            connectionString = M3Svr.ConnString(frmMain.grid)
            sql = "SELECT *" & _
                  " FROM MITBAL" & _
                  " WHERE MBITNO = '" & ProductID & "' AND MBWHLO = '" & warehouseID & "';"

            connection = New SqlConnection(connectionString)

            Try
                connection.Open()
                command = New SqlCommand(sql, connection)
                adapter.SelectCommand = command
                adapter.Fill(ds2.Tables("MITBAL"))
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
                        Print(File2, Chr(27) & "V500" & Chr(27) & "H226" & Chr(27) & "L0202" & Chr(27) & "XSSupplier Pack ID: " & "SuppPackID" & vbCrLf)
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

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        If Not lstReprint.Text = "" Then
            Me.Cursor = Cursors.WaitCursor
            Dim reprint = MessageBox.Show("Do you want to reprint ALL of the labels for this receipt?", "Gunnersen Inward Goods", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If reprint = 6 Then
                connectionString = "Data Source=m3db;Initial Catalog=Gunnersen;Integrated Security=False;User ID=GunUpdate;Password=Sabr2th12;Connect " & _
                                "Timeout=15;Encrypt=False;TrustServerCertificate=False"
                sql = "SELECT Left(RUN_NUMBER,len(RUN_NUMBER)-1), ID, ATTRIBUTE_VALUE, ATTRIBUTE_ITEM, ATTRIBUTE_QTY, LINE_ID, RUN_NUMBER, ROWNUMBER, CATCHWEIGHT, TALLY, PACKQTY, ITEMQTY, PACKTYPE, perPack, ShortPack, putaway, lotNo, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag " & _
                      "FROM LabelPrinting " & _
                      "GROUP BY Left(RUN_NUMBER,len(RUN_NUMBER)-1), ID, ATTRIBUTE_VALUE, ATTRIBUTE_ITEM, ATTRIBUTE_QTY, LINE_ID, RUN_NUMBER, ROWNUMBER, CATCHWEIGHT, TALLY, PACKQTY, ITEMQTY, PACKTYPE, perPack, ShortPack, putaway, lotNo, LABELQTY, AppCharge, Warehouse, ItemNo, Descrip, typeFlag " & _
                      "HAVING Left(RUN_NUMBER,len(RUN_NUMBER)-1) = '" & lstReprint.Text & "'"

                connection = New SqlConnection(connectionString)

                Try
                    'Open the database, fill recordset and close
                    connection.Open()
                    command = New SqlCommand(sql, connection)
                    adapter.SelectCommand = command
                    adapter.Fill(dtLines)
                    adapter.Dispose()
                    command.Dispose()
                    connection.Close()
                Catch

                End Try

                For i = 0 To dtLines.Rows.Count - 1
                    PrintLabels(i)
                Next
                Me.Cursor = Cursors.Arrow
                MessageBox.Show("Label Printing Complete", "Gunnsersen Inward Goods", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub
End Class