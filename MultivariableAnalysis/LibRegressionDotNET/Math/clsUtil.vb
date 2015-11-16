Namespace Util
    ''' <summary>
    ''' common use
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsUtil
        ''' <summary>
        ''' Generate Random permutation
        ''' </summary>
        ''' <param name="ai_max"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RandomPermutaion(ByVal ai_max As Integer) As List(Of Integer)
            Dim ary As New List(Of Integer)(ai_max)
            For i As Integer = 0 To ai_max - 1
                ary.Add(i)
            Next

            Dim r As New System.Random() '

            Dim n As Integer = ary.Count
            While n > 1
                n -= 1
                Dim k As Integer = CInt(r.Next(0, n + 1))
                Dim tmp As Integer = ary(k)
                ary(k) = ary(n)
                ary(n) = tmp
            End While
            Return ary
        End Function

        ''' <summary>
        ''' Generate Random permutation with Remove
        ''' </summary>
        ''' <param name="ai_max"></param>
        ''' <param name="ai_removeIndex">RemoveIndex</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RandomPermutaion(ByVal ai_max As Integer, ByVal ai_removeIndex As Integer) As List(Of Integer)
            Dim ary As New List(Of Integer)(ai_max)
            For i As Integer = 0 To ai_max - 1
                If ai_removeIndex <> i Then
                    ary.Add(i)
                End If
            Next

            Dim r As New System.Random() '

            Dim n As Integer = ary.Count
            While n > 1
                n -= 1
                Dim k As Integer = CInt(r.Next(0, n + 1))
                Dim tmp As Integer = ary(k)
                ary(k) = ary(n)
                ary(n) = tmp
            End While
            Return ary
        End Function
    End Class
End Namespace