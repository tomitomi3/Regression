Namespace Regression
    ''' <summary>
    ''' Linear Regression Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsLinearRegression
        Public TrainDataMatrix As Double()() = Nothing
        Public TrainDataFields As String() = Nothing
        Public CorrectDataVector As Double() = Nothing
        Public CorrectDataField As String = Nothing
        Private weightVector() As Double = Nothing

#Region "Public"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            'nop
        End Sub

        ''' <summary>
        ''' do regression analysis
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub DoRegression()
            'check
            If TrainDataMatrix Is Nothing OrElse CorrectDataVector Is Nothing Then
                Return
            End If

            'using QR method Ref:http://numerics.mathdotnet.com/Regression.html
            Try
                Me.weightVector = MathNet.Numerics.Fit.MultiDim(TrainDataMatrix, CorrectDataVector, intercept:=True, method:=MathNet.Numerics.LinearRegression.DirectRegressionMethod.QR)
            Catch ex As Exception
                Me.weightVector = Nothing
                Console.WriteLine(MathNet.Numerics.LinearAlgebra.CreateMatrix.DenseOfColumnArrays(weightVector))
                Return
            End Try
        End Sub

        ''' <summary>
        ''' validate of weight
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Validate()
            'check
            If Me.weightVector Is Nothing Then
                Return
            End If

            'calc residual sum of square
            Dim residualArray(TrainDataMatrix.Count - 1) As Double
            Dim rssArray(TrainDataMatrix.Count - 1) As Double
            For i As Integer = 0 To residualArray.Count - 1
                residualArray(i) = CorrectDataVector(i) - Me.Predict(TrainDataMatrix(i))
                rssArray(i) = residualArray(i) * residualArray(i)
            Next

            Dim rss = rssArray.Sum
        End Sub

        ''' <summary>
        ''' Predict by dataset
        ''' </summary>
        ''' <param name="ai_dataVector"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Predict(ByVal ai_dataVector() As Double) As Double
            If Me.CorrectDataField.Count <> ai_dataVector.Count Then
                Return 0
            End If

            Dim tempSum As Double = weightVector(0)
            For j As Integer = 1 To weightVector.Count - 1
                tempSum += weightVector(j) * ai_dataVector(j)
            Next

            Return tempSum
        End Function
#End Region
    End Class
End Namespace
