Imports IniParser

Public Class Class1


   Public Sub Test()

      Dim fp As New FileIniDataParser

      For i = 1 To 10

      Next

      Dim g = 0
Iterate:
      g = g + 1
      If g < 10 Then
         GoTo Iterate
      End If


   End Sub

End Class
