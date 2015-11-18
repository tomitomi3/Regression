Namespace Regression
    ''' <summary>
    ''' Lasso回帰　最適化クラス
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Class clsLassoEstimate : Inherits LibOptimization.Optimization.absObjectiveFunction
        ''' <summary>Shrinkage parameter</summary>
        Public Property ShrinkageParameter As Double = 1.0

        ''' <summary>correct data</summary>
        Public CorretDataVector() As Double = Nothing

        ''' <summary>train data matring</summary>
        Public TrainDataMatrix()() As Double = Nothing

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            'nop
        End Sub

#Region "for LibOptimization"
        ''' <summary>
        ''' Evaluate function
        ''' </summary>
        ''' <param name="ai_coeff">weight</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function F(ByVal ai_coeff As List(Of Double)) As Double
            Dim sumDiffSquare As Double = 0

            'Predict
            Dim weight = ai_coeff.ToArray()
            For i As Integer = 0 To Me.TrainDataMatrix.Count - 1
                Dim predict As Double = clsLinearRegression.Predict(weight, Me.TrainDataMatrix(i))
                sumDiffSquare += (Me.CorretDataVector(i) - predict) ^ 2 '二乗誤差
            Next

            'L1 Norm
            Dim l1norm As Double = ai_coeff.Sum(Function(x As Double)
                                                    Return Math.Abs(x)
                                                End Function)

            'lasso
            sumDiffSquare = sumDiffSquare + Me.ShrinkageParameter * l1norm

            Return sumDiffSquare
        End Function

        Public Overrides Function Gradient(x As List(Of Double)) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(x As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides ReadOnly Property NumberOfVariable As Integer
            Get
                Return TrainDataMatrix(0).Count + 1
            End Get
        End Property
#End Region
    End Class
End Namespace
