Imports MathNet.Numerics.LinearAlgebra

Namespace Regression
    ''' <summary>
    ''' Linear Regression Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsLinearRegression
        ''' <summary></summary>
        Public TrainDataMatrix As Double()() = Nothing

        ''' <summary></summary>
        Public TrainDataFields As String() = Nothing

        ''' <summary></summary>
        Public CorrectDataVector As Double() = Nothing

        ''' <summary></summary>
        Public CorrectDataField As String = Nothing

        ''' <summary></summary>
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

            ''最小二乗法 (X^T*X)-1*X^T*Y
            'Dim y = CreateMatrix.DenseOfColumnArrays(Me.correctVector)
            'Dim x = CreateMatrix.Dense(Of Double)(TrainDataMatrix.Count, Me.trainFieldNames.Count + 1)
            'For i = 0 To Me.TrainDataMatrix.Count - 1
            '    Dim tempRow(Me.trainFieldNames.Count) As Double
            '    tempRow(0) = 1.0
            '    For j As Integer = 0 To Me.trainFieldNames.Count - 1
            '        tempRow(j + 1) = Me.TrainDataMatrix(i)(j)
            '    Next
            '    x.SetRow(i, tempRow)
            'Next
            'Dim xx = (x.Transpose() * x).Inverse() * x.Transpose() * y
            'Console.WriteLine(xx)
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

            'calc R^2, AIC
            'calc coefficient of determination R^2
            Dim valueR2 As Double = 0
            Dim correctAvg As Double = CorrectDataVector.Sum / CorrectDataVector.Count
            For i As Integer = 0 To Me.CorrectDataVector.Count - 1
                Dim temp = Me.CorrectDataVector(i) - correctAvg
                valueR2 += temp * temp
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

            Dim squareR = 1 - (rss / valueR2)

            'AIC = n*ln(RSS/n)+2*K
            Dim n = CorrectDataVector.Count
            Dim k = TrainDataMatrix(0).Count + 1
            Dim aic = n * Math.Log(rss / n) + 2 * k
            Dim bic = n * Math.Log(rss / n) + Math.Log(n) * k

            Console.WriteLine("Validate:")
            Console.WriteLine("R^2 = {0}", squareR)
            Console.WriteLine("RSS = {0}", rss)
            Console.WriteLine("AIC = {0}", aic)
            Console.WriteLine("BIC = {0}", bic)
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
