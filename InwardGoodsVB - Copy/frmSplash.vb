Imports System.Xml
Imports System.Reflection

Public Class frmSplash
    Dim tmr As Timer
    Private Sub frmSplash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim version = GetVersion()
        lblVer.Text = lblVer.Text & " " & version
    End Sub

    Private Sub frmSplash_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        tmr = New Timer()
        tmr.Interval = 4000
        tmr.Start()
        AddHandler tmr.Tick, AddressOf Timer_Tick
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs)
        tmr.Stop()
        Dim mf = New frmMain()
        mf.Show()
        Me.Hide()
    End Sub

    Private Function GetVersion() As Object
        Dim xmlDoc As XmlDocument = New XmlDocument()
        Dim asmCurrent As Assembly = System.Reflection.Assembly.GetExecutingAssembly()
        Dim executePath As String = New Uri(asmCurrent.GetName().CodeBase).LocalPath
        xmlDoc.Load(executePath + ".manifest")

        Dim retval As String = String.Empty
        If xmlDoc.HasChildNodes Then
            retval = xmlDoc.ChildNodes(1).ChildNodes(0).Attributes.GetNamedItem("version").Value.ToString()
        End If

        Return retval
    End Function
End Class