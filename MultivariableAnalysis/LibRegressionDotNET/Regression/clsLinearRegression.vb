Imports MathNet.Numerics
Imports MathNet.Numerics.LinearAlgebra

Namespace Regression
    ''' <summary>
    ''' Linear Regression Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsLinearRegression
#Region "Member"
        ''' <summary>データ行列</summary>
        Public TrainDataMatrix As Double()() = Nothing

        ''' <summary>データ行列 フィールド名</summary>
        Public TrainDataFields As String() = Nothing

        ''' <summary>目的変数ベクトル</summary>
        Public CorrectDataVector As Double() = Nothing

        ''' <summary>目的変数ベクトル フィールド名</summary>
        Public CorrectDataField As String = Nothing

        ''' <summary>係数</summary>
        Private weightVector() As Double = Nothing

        ''' <summary>係数</summary>
        Public ReadOnly Property Weight() As Double()
            Get
                Return Me.weightVector
            End Get
        End Property

        ''' <summary>変数選択の手法</summary>
        Public Property ValiableSelection As EnumValiableSelection = EnumValiableSelection.NotUseVariableSelection

        ''' <summary>変数選択の手法</summary>
        Public Enum EnumValiableSelection
            ''' <summary>変数選択を行わない</summary>
            NotUseVariableSelection
            ''' <summary>全組み合わせ</summary>
            AllCombination
            ''' <summary>変数増加法</summary>
            ForwardSelection
            ''' <summary>変数減少法</summary>
            BackwardElimination
            ''' <summary>変数減少法（変数0からスタート）</summary>
            Stepwise
        End Enum
#End Region

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

            If Me.ValiableSelection = EnumValiableSelection.NotUseVariableSelection Then
                'not use variable selection
                Try
                    'using QR method Ref:http://numerics.mathdotnet.com/Regression.html
                    Me.weightVector = Fit.MultiDim(TrainDataMatrix, CorrectDataVector, intercept:=True, method:=MathNet.Numerics.LinearRegression.DirectRegressionMethod.QR)
                Catch ex As Exception
                    Me.weightVector = Nothing
                    Console.WriteLine(MathNet.Numerics.LinearAlgebra.CreateMatrix.DenseOfColumnArrays(weightVector))
                End Try
                Return
            ElseIf Me.ValiableSelection = EnumValiableSelection.ForwardSelection Then
                'use variable selection(experiment)
                Dim fldCount As Integer = Me.TrainDataMatrix(0).Count
                Dim recCount As Integer = Me.TrainDataMatrix.Count
                Dim useIndexArray As New List(Of Integer)
                Dim indexArray As New List(Of Integer)
                For index As Integer = 0 To fldCount - 1
                    indexArray.Add(index)
                Next
                While (True)
                    Dim smallAIC As Double = 0
                    Dim smallAICIndex As Integer = 0
                    Dim isDetectSmallAic As Boolean = False
                    For Each index In indexArray
                        Dim tempUseIndexArray = useIndexArray.ToList()
                        tempUseIndexArray.Add(index)
                        Dim tempDataMat()() As Double = RestructDataMatrix(tempUseIndexArray.ToArray())
                        Dim tempWeight() As Double = Fit.MultiDim(tempDataMat, CorrectDataVector, intercept:=True, method:=LinearRegression.DirectRegressionMethod.QR)

                        'Validate
                        Dim squareR As Double
                        Dim aic As Double
                        Dim bic As Double
                        Me.EvaluateRegression(tempWeight, Me.CorrectDataVector, tempDataMat, squareR, aic, bic)

                        'AICが低いものを採用する
                        If index = 0 Then
                            smallAIC = aic
                            smallAICIndex = index
                        ElseIf smallAIC > aic Then
                            smallAIC = aic
                            smallAICIndex = index
                            isDetectSmallAic = True
                        End If
                    Next

                    '終了条件
                    If isDetectSmallAic = False OrElse useIndexArray.Count = fldCount Then
                        Exit While
                    Else
                        Dim remove As Integer = indexArray.IndexOf(smallAICIndex)
                        indexArray.Remove(remove)
                        useIndexArray.Add(smallAICIndex)
                    End If
                End While
                With Nothing
                    Dim tempDataMat()() As Double = RestructDataMatrix(useIndexArray.ToArray())
                    Dim tempWeight() As Double = Fit.MultiDim(RestructDataMatrix(useIndexArray.ToArray()), CorrectDataVector, intercept:=True, method:=LinearRegression.DirectRegressionMethod.QR)
                    Dim squareR As Double
                    Dim aic As Double
                    Dim bic As Double
                    Me.EvaluateRegression(tempWeight, Me.CorrectDataVector, tempDataMat, squareR, aic, bic)
                End With
            Else
                'use variable selection
            End If

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
        ''' regression result
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub OutputRegressionResult()
            If weightVector Is Nothing Then
                Return
            End If

            'Result
            Console.WriteLine("Result:")
            Console.WriteLine(" w0:{0}", weightVector(0))
            For i As Integer = 1 To weightVector.Count - 1
                Console.WriteLine(" w{0}:{1}", i, weightVector(i))
            Next
            Console.WriteLine("")

            'Validate
            Dim squareR As Double
            Dim aic As Double
            Dim bic As Double
            Me.EvaluateRegression(Me.weightVector, Me.CorrectDataVector, Me.TrainDataMatrix, squareR, aic, bic)
            Console.WriteLine("Validate:")
            Console.WriteLine(" R^2 = {0}", squareR)
            Console.WriteLine(" AIC = {0}", aic)
            Console.WriteLine(" BIC = {0}", bic)
        End Sub
#End Region

#Region "Public(shared)"
        ''' <summary>
        ''' Predict by dataset
        ''' </summary>
        ''' <param name="ai_dataVector"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Predict(ByVal ai_weight() As Double, ByVal ai_dataVector() As Double) As Double
            'If TrainDataMatrix Is Nothing OrElse TrainDataMatrix(0).Count <> ai_dataVector.Count Then
            '    Return 0
            'End If
            Dim tempSum As Double = ai_weight(0) 'constant value
            For j As Integer = 1 To ai_weight.Count - 1
                tempSum += ai_weight(j) * ai_dataVector(j - 1)
            Next
            Return tempSum
        End Function
#End Region

#Region "Private"
        ''' <summary>
        ''' evaluate
        ''' </summary>
        ''' <param name="w"></param>
        ''' <param name="ao_squareR"></param>
        ''' <param name="ao_aic"></param>
        ''' <param name="ao_bic"></param>
        ''' <remarks></remarks>
        Private Sub EvaluateRegression(ByVal w() As Double, ByVal CorrectDataVector() As Double, ByVal TrainDataMatrix()() As Double, _
                                       ByRef ao_squareR As Double, ByRef ao_aic As Double, ByRef ao_bic As Double)
            'Calc R^2
            Dim tempValue As Double = 0
            Dim correctAvg As Double = CorrectDataVector.Sum / CorrectDataVector.Count
            For i As Integer = 0 To CorrectDataVector.Count - 1
                Dim temp = CorrectDataVector(i) - correctAvg
                tempValue += temp * temp
            Next

            'Calc RSS(residual sum of square)
            Dim rss As Double = 0
            Dim rsArray(TrainDataMatrix.Count - 1) As Double
            For i As Integer = 0 To Me.CorrectDataVector.Count - 1
                Dim predictValue = clsLinearRegression.Predict(w, TrainDataMatrix(i))
                Dim temp = CorrectDataVector(i) - predictValue
                rsArray(i) = temp * temp
                rss += rsArray(i)
            Next

            'R^2 重相関係数
            ao_squareR = 1 - (rss / tempValue)

            'AIC = n*ln(RSS/n)+2*K
            Dim n = CorrectDataVector.Count
            Dim k = TrainDataMatrix(0).Count + 1
            ao_aic = n * Math.Log(rss / n) + 2 * k
            ao_bic = n * Math.Log(rss / n) + Math.Log(n) * k
        End Sub

        ''' <summary>
        ''' restruct data matrix
        ''' </summary>
        ''' <param name="ai_useIndex"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RestructDataMatrix(ByVal ai_useIndex() As Integer) As Double()()
            'check
            If TrainDataMatrix Is Nothing OrElse CorrectDataVector Is Nothing Then
                Return Nothing
            End If
            If ai_useIndex Is Nothing OrElse ai_useIndex.Count = 0 Then
                Return Nothing
            End If

            Dim fldCount As Integer = Me.TrainDataMatrix(0).Count
            Dim recCount As Integer = Me.TrainDataMatrix.Count
            Dim retDataMat = New Double(recCount - 1)() {}

            With Nothing
                Dim useIndexArray As New List(Of Integer)
                For i As Integer = 0 To ai_useIndex.Count - 1
                    useIndexArray.Add(ai_useIndex(i))
                Next
                For i As Integer = 0 To recCount - 1
                    Dim tempArray(ai_useIndex.Count - 1) As Double
                    Dim j As Integer = 0
                    For Each useIndex In useIndexArray
                        tempArray(j) = Me.TrainDataMatrix(i)(useIndex)
                        j += 1
                    Next
                    retDataMat(i) = tempArray
                Next
            End With

            Return retDataMat
        End Function
#End Region
    End Class
End Namespace
