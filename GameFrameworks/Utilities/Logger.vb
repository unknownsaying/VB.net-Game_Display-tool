Imports System
Imports System.IO

Namespace GameFramework.Utilities
    Public Class Logger
        Private Shared logFile As String = "game.log"

        Public Shared Sub Log(message As String)
            Dim logMessage As String = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}"
            Console.WriteLine(logMessage)
            File.AppendAllText(logFile, logMessage & Environment.NewLine)
        End Sub

        Public Shared Sub LogError(message As String)
            Log($"ERROR: {message}")
        End Sub

        Public Shared Sub LogWarning(message As String)
            Log($"WARNING: {message}")
        End Sub
    End Class
End Namespace

Imports System
Imports System.IO
Imports System.Text
Imports System.Threading

Namespace GameFramework.Utilities
    Public Enum LogLevel
        Debug = 0
        Info = 1
        Warning = 2
        [Error] = 3
        Critical = 4
    End Enum
    
    Public Class Logger
        Implements IDisposable
        
        Private logWriter As StreamWriter
        Private logFile As String
        Private minLogLevel As LogLevel
        Private writeLock As Object
        Private isDisposed As Boolean
        
        Public Event LogMessage As EventHandler(Of LogEventArgs)
        
        Public Sub New(Optional logFileName As String = "game.log", 
                      Optional minimumLevel As LogLevel = LogLevel.Info)
            logFile = logFileName
            minLogLevel = minimumLevel
            writeLock = New Object()
            
            Try
                logWriter = New StreamWriter(logFile, True, Encoding.UTF8)
                LogInternal(LogLevel.Info, $"Logger initialized at {DateTime.Now}")
            Catch ex As Exception
                ' Fallback to console if file logging fails
                Console.WriteLine($"Failed to initialize file logger: {ex.Message}")
            End Try
        End Sub
        
        Public Sub Debug(message As String, Optional ParamArray args As Object())
            Log(LogLevel.Debug, message, args)
        End Sub
        
        Public Sub Info(message As String, Optional ParamArray args As Object())
            Log(LogLevel.Info, message, args)
        End Sub
        
        Public Sub Warning(message As String, Optional ParamArray args As Object())
            Log(LogLevel.Warning, message, args)
        End Sub
        
        Public Sub [Error](message As String, Optional ParamArray args As Object())
            Log(LogLevel.Error, message, args)
        End Sub
        
        Public Sub Critical(message As String, Optional ParamArray args As Object())
            Log(LogLevel.Critical, message, args)
        End Sub
        
        Public Sub Log(level As LogLevel, message As String, Optional ParamArray args As Object())
            If level < minLogLevel Then Return
            
            Dim formattedMessage As String = If(args IsNot Nothing AndAlso args.Length > 0,
                                              String.Format(message, args),
                                              message)
            
            LogInternal(level, formattedMessage)
        End Sub
        
        Private Sub LogInternal(level As LogLevel, message As String)
            Dim logEntry As String = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}"
            
            ' Write to console
            Dim originalColor As ConsoleColor = Console.ForegroundColor
            Try
                Select Case level
                    Case LogLevel.Debug
                        Console.ForegroundColor = ConsoleColor.Gray
                    Case LogLevel.Info
                        Console.ForegroundColor = ConsoleColor.White
                    Case LogLevel.Warning
                        Console.ForegroundColor = ConsoleColor.Yellow
                    Case LogLevel.Error
                        Console.ForegroundColor = ConsoleColor.Red
                    Case LogLevel.Critical
                        Console.ForegroundColor = ConsoleColor.Magenta
                End Select
                Console.WriteLine(logEntry)
                Console.ForegroundColor = originalColor
            Catch
                ' Ignore console errors
            End Try
            
            ' Write to file
            SyncLock writeLock
                Try
                    If logWriter IsNot Nothing AndAlso Not isDisposed Then
                        logWriter.WriteLine(logEntry)
                        logWriter.Flush()
                    End If
                Catch ex As Exception
                    Console.WriteLine($"Log write failed: {ex.Message}")
                End Try
            End SyncLock
            
            ' Raise event
            RaiseEvent LogMessage(Me, New LogEventArgs(level, message, DateTime.Now))
        End Sub
        
        Public Sub Flush()
            SyncLock writeLock
                logWriter?.Flush()
            End SyncLock
        End Sub
        
#Region "IDisposable Implementation"
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not isDisposed Then
                If disposing Then
                    SyncLock writeLock
                        If logWriter IsNot Nothing Then
                            LogInternal(LogLevel.Info, "Logger shutting down")
                            logWriter.Flush()
                            logWriter.Dispose()
                            logWriter = Nothing
                        End If
                    End SyncLock
                End If
                isDisposed = True
            End If
        End Sub
        
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
    
    Public Class LogEventArgs
        Inherits EventArgs
        
        Public ReadOnly Property Level As LogLevel
        Public ReadOnly Property Message As String
        Public ReadOnly Property Timestamp As DateTime
        
        Public Sub New(level As LogLevel, message As String, timestamp As DateTime)
            Me.Level = level
            Me.Message = message
            Me.Timestamp = timestamp
        End Sub
    End Class
End Namespace