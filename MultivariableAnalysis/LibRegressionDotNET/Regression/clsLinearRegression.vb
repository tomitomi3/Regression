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

        ''' <summary>result</summary>
        Public ReadOnly Property Weight() As Double()
            Get
                Return Me.weightVector
            End Get
        End Property
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
        ''' Predict by dataset
        ''' </summary>
        ''' <param name="ai_dataVector"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Predict(ByVal ai_dataVector() As Double) As Double
            If TrainDataMatrix Is Nothing OrElse TrainDataMatrix(0).Count <> ai_dataVector.Count Then
                Return 0
            End If

            Dim tempSum As Double = weightVector(0)
            For j As Integer = 1 To weightVector.Count - 1
                tempSum += weightVector(j) * ai_dataVector(j - 1)
            Next

            Return tempSum
        End Function

        ''' <summary>
        ''' validate of weight
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Validate()
            'check
            If Me.weightVector Is Nothing Then
                Return
            End If

            'calc coefficient of determination R^2
            Dim value As Double = 0
            Dim correctAvg As Double = CorrectDataVector.Sum / CorrectDataVector.Count
            For i As Integer = 0 To Me.CorrectDataVector.Count - 1
                Dim temp = Me.CorrectDataVector(i) - correctAvg
                value += temp * temp
            Next

            'calc RSS(residual sum of square)
            Dim rss As Double = 0
            Dim rsArray(TrainDataMatrix.Count - 1) As Double
            For i As Integer = 0 To Me.CorrectDataVector.Count - 1
                Dim predictValue = Me.Predict(Me.TrainDataMatrix(i))
                Dim temp = Me.CorrectDataVector(i) - predictValue
                rsArray(i) = temp * temp
                rss += rsArray(i)
            Next

            Dim r = 1 - (rss / value)

            Console.WriteLine("Validate:")
            Console.WriteLine("R^2 = {0}", r)
            Console.WriteLine("RSS = {0}", rss)
        End Sub

        ''' <summary>
        ''' regression result
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub OutputRegressionResult()
            If weightVector Is Nothing Then
                Return
            End If
            Console.WriteLine("Result:")
            Console.WriteLine("w0:{0}", weightVector(0))
            For i As Integer = 1 To weightVector.Count - 1
                Console.WriteLine("w{0}:{1}", i, weightVector(i))
            Next
            Me.Validate()
        End Sub
#End Region
    End Class
End Namespace
