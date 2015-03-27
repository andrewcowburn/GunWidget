Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Net
Imports System.IO
Imports InwardGoodsVB.wsStockOperations
Imports System.ServiceModel

Public Class frmTestWS

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Create a client with basic http credentials
        Dim client As New StockOperationsClient
        Dim binding As New System.ServiceModel.BasicHttpBinding()
        binding.Security.Mode = BasicHttpSecurityMode.Transport
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic
        binding.MaxReceivedMessageSize = 25 * 1024 * 1024
        client.Endpoint.Binding = binding

        MessageBox.Show(client.Endpoint.Address.ToString)
        MessageBox.Show(client.Endpoint.Name.ToString)

        client.ClientCredentials.UserName.UserName = "DTAMIGR"
        client.ClientCredentials.UserName.Password = "Q190E87AG"

        Dim header = New lws
        header.user = client.ClientCredentials.UserName.UserName
        header.password = client.ClientCredentials.UserName.Password

        'Create a requests item
        Dim item1 = New Catchweight
        'item1. = client.ClientCredentials.UserName.UserName;

        'client.Catchweight()
    End Sub
End Class