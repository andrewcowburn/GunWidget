Imports System.DirectoryServices
Imports System.Data.SqlClient


Module modFunctions
    Dim connection As New SqlConnection
    Dim command As New SqlCommand
    Dim adapter As New SqlDataAdapter
    Dim dsLine As New DataSet



    'routine to get "pager" from active dirctory, pager acts as M3 username

    Public Function GetActiveDirUserDetails(ByVal userid As String) As String
        Dim dirEntry As System.DirectoryServices.DirectoryEntry
        Dim dirSearcher As System.DirectoryServices.DirectorySearcher
        Dim domainName As String = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName
        Try
            dirEntry = New System.DirectoryServices.DirectoryEntry("LDAP://" & domainName)
            dirSearcher = New System.DirectoryServices.DirectorySearcher(dirEntry)
            dirSearcher.Filter = "(samAccountName=" & userid & ")"

            'get pager as M3 userID
            dirSearcher.PropertiesToLoad.Add("pager")
            Dim sr As SearchResult = dirSearcher.FindOne()
            If sr Is Nothing Then 'return false if user isn't found 
                Return False
            End If
            Dim de As System.DirectoryServices.DirectoryEntry = sr.GetDirectoryEntry()
            Dim userPager = UCase(de.Properties("pager").Value.ToString())
            Return userPager
        Catch ex As Exception ' return false if exception occurs 
            Return ex.Message
        End Try
    End Function
End Module
