Imports System
Imports System.Threading
Imports System.Windows.Controls
Imports Microsoft.Phone.Controls
Imports Microsoft.Phone.Shell
Imports System.Net.Http
Imports System.Net.Sockets
Imports Windows.Storage.Streams
Imports System.Text
Imports Windows.Phone.Speech.VoiceCommands


Partial Public Class MainPage
    Inherits PhoneApplicationPage
    Dim i As Integer
    Dim ip As String = Nothing
    ' Constructor
    Public Sub New()
        InitializeComponent()
        i = 1

        SupportedOrientations = SupportedPageOrientation.Portrait Or SupportedPageOrientation.Landscape
        ' Código de ejemplo para traducir ApplicationBar
        'BuildLocalizedApplicationBar()

    End Sub

    ' Código de ejemplo para compilar una ApplicationBar traducida
    'Private Sub BuildLocalizedApplicationBar()
    '    ' Establecer ApplicationBar de la página en una nueva instancia de ApplicationBar.
    '    ApplicationBar = New ApplicationBar()

    '    ' Crear un nuevo botón y establecer el valor de texto en la cadena traducida de AppResources.
    '    Dim appBarButton As New ApplicationBarIconButton(New Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative))
    '    appBarButton.Text = AppResources.AppBarButtonText
    '    ApplicationBar.Buttons.Add(appBarButton)

    '    ' Crear un nuevo elemento de menú con la cadena traducida de AppResources.
    '    Dim appBarMenuItem As New ApplicationBarMenuItem(AppResources.AppBarMenuItemText)
    '    ApplicationBar.MenuItems.Add(appBarMenuItem)
    'End Sub



    Async Sub registrar(sender As Object, e As RoutedEventArgs)
        Await VoiceCommandService.InstallCommandSetsFromFileAsync(New Uri("ms-appx:///VoiceCommands.xml"))
        MessageBox.Show("Parece que se registró!")
    End Sub



    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)
        If NavigationContext.QueryString.ContainsKey("voiceCommandName") Then
            If NavigationContext.QueryString("voiceCommandName").ToString = "TurnLightOn" Then
                turnlighton()
            ElseIf NavigationContext.QueryString("voiceCommandName").ToString = "TurnLightOff" Then
                turnlightoff()
            End If
        End If
        
    End Sub


    Async Sub turnlighton()
        If ip Is Nothing Then
            Await meh()
        End If
        Button_Click(New Object, New RoutedEventArgs)
    End Sub

    Async Sub turnlightoff()
        If ip Is Nothing Then
            Await meh()
        End If
        Button_Click_1(New Object, New RoutedEventArgs)
    End Sub

    Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
        If ip IsNot Nothing Then
            Dim http As HttpClient = New HttpClient
            Dim res As HttpResponseMessage = Nothing
            Try
                res = Await http.GetAsync("http://" + ip + ":88/led1=On&guid=" + Guid.NewGuid.ToString)
            Catch ex As Exception
                MessageBox.Show("Algo anduvo mal, caez")
                Exit Sub
            End Try
            Dim content As String = Await res.Content.ReadAsStringAsync
            i += 1
            MessageBox.Show(content)
        Else
            MessageBox.Show("Todavía no se encontró ningún Arduino!")
        End If

    End Sub

    Private Async Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        If ip IsNot Nothing Then
            Dim http As HttpClient = New HttpClient
            Dim res As HttpResponseMessage = Nothing
            Try
                res = Await http.GetAsync("http://" + ip + ":88/led1=Off&guid" + Guid.NewGuid.ToString)
            Catch ex As Exception
                MessageBox.Show("Algo anduvo mal, caez")
                Exit Sub
            End Try
            Dim content As String = Await res.Content.ReadAsStringAsync
            MessageBox.Show(content)
        Else
            MessageBox.Show("Todavía no se encontró ningún arduino!")
        End If

    End Sub

    Async Sub descubrir(sender As Object, e As RoutedEventArgs)
        meh()
    End Sub

    Async Function meh() As Tasks.Task
        Dim client As SocketClient = New SocketClient
        client.Send("192.168.2.255", 8888, "blabla")
        Dim response As String = client.Receive(8888)
        ip = response.Split("@")(1).Split(":")(0)
        tbip.Text = "La ip del Arduino es: " + ip
    End Function


End Class