Imports System

Namespace GameFramework.Utilities
    Public Class Randomizer
        Private Shared random As New Random()

        Public Shared Function NextDouble() As Double
            Return random.NextDouble()
        End Function

        Public Shared Function Next(minValue As Integer, maxValue As Integer) As Integer
            Return random.Next(minValue, maxValue)
        End Function

        Public Shared Function Next(maxValue As Integer) As Integer
            Return random.Next(maxValue)
        End Function

        Public Shared Function NextSingle() As Single
            Return CSng(random.NextDouble())
        End Function
    End Class
End Namespace