Imports System.IO
Imports System.Threading
Imports System.Xml

Public Class RunBatSvc : Inherits myService

    Private processing As New List(Of String)
    Dim doc As New XmlDocument

#Region "Override Service Methods"

    Overrides Sub svcStart(ByVal args As Dictionary(Of String, String))

        Log("Starting...")
        Try

            doc.Load(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "paths.xml"))

            For Each p As XmlNode In doc.SelectNodes("paths/loc")
                If Directory.Exists(p.Attributes("name").Value) Then
                    Dim fsw As New FileSystemWatcher
                    With fsw
                        AddHandler .Created, AddressOf fsw_Created
                        .Path = p.Attributes("name").Value
                        .IncludeSubdirectories = False
                        .EnableRaisingEvents = True


                    End With
                End If
            Next


        Catch ex As Exception
            Log(ex.Message)
            End

        End Try

    End Sub

#End Region

#Region "FileSystemWatcher Handler"

    Private Sub fsw_Created(ByVal sender As Object, ByVal e As FileSystemEventArgs)
        'TryCast(sender, FileSystemWatcher).Path
        If Not processing.Contains(e.FullPath) Then
            processing.Add(e.FullPath)
            With New FileInfo(e.FullPath)
                Select Case .Extension.ToLower
                    Case Else
                        With New Thread(AddressOf hLoad)
                            .Name = e.FullPath
                            .Start(e)
                        End With

                End Select
            End With
        End If

    End Sub

#End Region

#Region "Threads"

    Private Sub hLoad(ByVal e As FileSystemEventArgs)

        Try

            Threading.Thread.Sleep(500)
            While IsFileInUse(e.FullPath)
                Threading.Thread.Sleep(500)
            End While

            Dim bat As String = ""
            Dim dir As New DirectoryInfo(New FileInfo(e.FullPath).DirectoryName)
            For Each p As XmlNode In doc.SelectNodes("paths/loc")
                Dim testdir As New DirectoryInfo(p.Attributes("name").Value)
                If String.Compare(testdir.FullName, dir.FullName, True) = 0 Then
                    bat = p.Attributes("bat").Value
                    Exit For
                End If
            Next

            With New Process
                With .StartInfo
                    .UseShellExecute = False
                    .WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory
                    .FileName = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        String.Format("{0}.bat", bat)
                    )
                    .Arguments = e.FullPath & " " & System.Guid.NewGuid.ToString.Split("-")(0)
                    .RedirectStandardOutput = True
                    .RedirectStandardError = True

                End With

                AddHandler .OutputDataReceived, AddressOf hLogShell
                AddHandler .ErrorDataReceived, AddressOf hLogShell

                .Start()
                .BeginErrorReadLine()
                .BeginOutputReadLine()

                While Not .HasExited And .Responding
                    Thread.Sleep(10)

                End While

            End With

        Catch ex As Exception
            Log(ex.Message)

        Finally
            processing.Remove(e.FullPath)

        End Try

    End Sub

    Private Sub hLogShell(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        If Not e.Data Is Nothing Then Log(e.Data)

    End Sub

    Private Function IsFileInUse(sFile As String) As Boolean
        Try
            If Not New FileInfo(sFile).Exists Then Return False
            Using f As New IO.FileStream(sFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
            End Using
        Catch Ex As Exception
            Return True
        End Try
        Return False
    End Function

#End Region

End Class
