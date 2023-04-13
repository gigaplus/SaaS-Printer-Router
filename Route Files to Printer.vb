Imports System.Environment
Imports System.IO
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.System.Profile

Namespace MyNamespace

    Class MyClassVB
        '\\DESKTOP-NAME\ZD220 Text Only
        Shared PrinterShare As String

        Shared Sub Main(args As String())

            If args.Count = 0 Then
                Console.WriteLine("Label Printer Share Name must be provided as command line argument.\n Exiting program.")
            Else
                PrinterShare = args(0)
                Console.WriteLine("Label Printer: {0}", PrinterShare)
                Dim userProfilePath As String = GetFolderPath(SpecialFolder.UserProfile)
                Dim downloadsFolderPath As String = Path.Combine(userProfilePath, "Downloads")
                Console.WriteLine("Watching folder " & downloadsFolderPath)

                Using watcher = New FileSystemWatcher(downloadsFolderPath)
                    watcher.NotifyFilter = NotifyFilters.Attributes Or
                                           NotifyFilters.CreationTime Or
                                           NotifyFilters.DirectoryName Or
                                           NotifyFilters.FileName Or
                                           NotifyFilters.LastAccess Or
                                           NotifyFilters.LastWrite Or
                                           NotifyFilters.Security Or
                                           NotifyFilters.Size

                    AddHandler watcher.Renamed, AddressOf OnRenamed
                    AddHandler watcher.Error, AddressOf OnError

                    watcher.Filter = "*(ZPL)*.txt"
                    watcher.IncludeSubdirectories = False
                    watcher.EnableRaisingEvents = True

                    Console.WriteLine("Press enter to exit.")
                    Console.ReadLine()
                End Using

            End If
        End Sub

        Private Shared Sub OnRenamed(sender As Object, e As RenamedEventArgs)
            Console.WriteLine($"Printing label: {e.FullPath}")
            Console.Write("to " & PrinterShare)
            Dim filePath As String = e.FullPath
            Dim StartInfo As New ProcessStartInfo("cmd.exe")
            StartInfo.Arguments = "/c copy """ & e.FullPath & """ """ & PrinterShare & """ & DEL """ & e.FullPath & """"
            Process.Start(StartInfo)
            

        End Sub

        Private Shared Sub OnError(sender As Object, e As ErrorEventArgs)
            PrintException(e.GetException())
        End Sub

        Private Shared Sub PrintException(ex As Exception)
            If ex IsNot Nothing Then
                Console.WriteLine($"Message: {ex.Message}")
                Console.WriteLine("Stacktrace:")
                Console.WriteLine(ex.StackTrace)
                Console.WriteLine()
                PrintException(ex.InnerException)
            End If
        End Sub


        Public Function GetDownloadsFolderPath() As String
            Dim userProfilePath As String = GetFolderPath(SpecialFolder.UserProfile)
            Dim downloadsFolderPath As String = Path.Combine(userProfilePath, "Downloads")
            Return downloadsFolderPath
        End Function




    End Class
End Namespace
