Imports System.Net.Sockets
Imports System.Threading
Imports System.Text

Public Class SocketClient
    Dim socket As Socket = Nothing
    Shared clientDone As ManualResetEvent = New ManualResetEvent(False)
    Dim TIMEOUT_MILLISECONDS As Integer = 5000
    Dim MAX_BUFFER_SIZE As Integer = 2048
    Dim response As String
    Public Sub New()
        socket = New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
    End Sub

    Public Function Send(serverName As String, portNumber As Integer, data As String) As String
        response = "Timeout"
        If socket IsNot Nothing Then
            Dim socketEventArg = New SocketAsyncEventArgs
            socketEventArg.RemoteEndPoint = New DnsEndPoint(serverName, portNumber)
            AddHandler socketEventArg.Completed, AddressOf socketEventCompletedSend
            Dim payload As Byte() = Encoding.UTF8.GetBytes(data)
            socketEventArg.SetBuffer(payload, 0, payload.Length)
            clientDone.Reset()
            socket.SendToAsync(socketEventArg)
            clientDone.WaitOne(TIMEOUT_MILLISECONDS)
        Else
            response = "Socket is not initialized"
        End If
        Return response
    End Function

    Public Function Receive(portNumber As Integer) As String
        response = "Timeout"
        If socket IsNot Nothing Then
            Dim socketEventArg = New SocketAsyncEventArgs
            socketEventArg.RemoteEndPoint = New IPEndPoint(IPAddress.Any, portNumber)
            Dim buffer(MAX_BUFFER_SIZE) As Byte
            socketEventArg.SetBuffer(buffer, 0, MAX_BUFFER_SIZE)
            AddHandler socketEventArg.Completed, AddressOf socketEventCompletedReceive
            clientDone.Reset()
            socket.ReceiveFromAsync(socketEventArg)
            clientDone.WaitOne(TIMEOUT_MILLISECONDS)
        Else
            response = "Socket is not initialized"
        End If
        Return response
    End Function

    Sub socketEventCompletedReceive(s As Object, e As SocketAsyncEventArgs)
        If e.SocketError = SocketError.Success Then
            response = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred)
            response = response.Trim + "@" + e.RemoteEndPoint.ToString()
        Else
            response = e.SocketError.ToString
        End If
        clientDone.Set()
    End Sub


    Sub socketEventCompletedSend(s As Object, e As SocketAsyncEventArgs)
        response = e.SocketError.ToString
        clientDone.Set()
    End Sub

    Public Sub close()
        If socket IsNot Nothing Then
            socket.Close()
        End If
    End Sub

End Class
