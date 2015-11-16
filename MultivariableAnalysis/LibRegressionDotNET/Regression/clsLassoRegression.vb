Imports MathNet.Numerics
Imports MathNet.Numerics.LinearAlgebra

Namespace Regression
    ''' <summary>
    ''' Lasso Regression Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsLassoRegression
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
        Public Sub DoRegression(Optional ByVal ai_isPrint As Boolean = False)
            'check
            If TrainDataMatrix Is Nothing OrElse CorrectDataVector Is Nothing Then
                Return
            End If

            Dim fldCount As Integer = Me.TrainDataMatrix(0).Count
            Dim recCount As Integer = Me.TrainDataMatrix.Count

            Dim useIndexArray As New List(Of Integer)
            Dim iterateIndex As New List(Of Integer)
            For index As Integer = 0 To fldCount - 1
                iterateIndex.Add(index)
            Next

            If ai_isPrint = True Then
                Console.WriteLine("Ridge Regression:")
                Console.WriteLine("")
            End If

            ''最小二乗法 (X^T*X)-1*X^T*Y
            'With Nothing
            '    Dim y = CreateMatrix.DenseOfColumnArrays(CorrectDataVector)
            '    Dim x = CreateMatrix.Dense(Of Double)(TrainDataMatrix.Count, fldCount + 1)
            '    For i = 0 To Me.TrainDataMatrix.Count - 1
            '        Dim tempRow(fldCount) As Double
            '        tempRow(0) = 1.0
            '        For j As Integer = 0 To fldCount - 1
            '            tempRow(j + 1) = Me.TrainDataMatrix(i)(j)
            '        Next
            '        x.SetRow(i, tempRow)
            '    Next
            '    Dim xx = (x.Transpose() * x).Inverse() * x.Transpose() * y
            '    Console.WriteLine(xx)
            '    '※w0だけの場合 = 目的変数の平均値になる
            '    Dim temp = CorrectDataVector.Sum / CorrectDataVector.Count
            'End With
        End Sub

        ''' <summary>
        ''' regression result
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub OutputRegressionResult()
            If weightVector Is Nothing Then
                Return
            End If

            Dim dataField() As String = Me.TrainDataFields
            If Me.TrainDataFields Is Nothing Then
                Dim fldCount As Integer = Me.TrainDataMatrix(0).Count
                Dim tempFieldNames As New List(Of String)
                For i As Integer = 0 To fldCount - 1
                    tempFieldNames.Add(String.Format("{0}", i))
                Next
                dataField = tempFieldNames.ToArray()
            End If

            'Result
            Console.WriteLine("Result:")
            Console.WriteLine(" w0,{0}", weightVector(0))
            For i As Integer = 1 To weightVector.Count - 1
                Console.WriteLine(" w{0},{1},{2}", i, dataField(i - 1), weightVector(i))
            Next
            Console.WriteLine("")

            'Validate
            Console.WriteLine(Me.EvaluateRegression(Me.weightVector, Me.CorrectDataVector, Me.TrainDataMatrix))
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
        ''' 評価値格納クラス
        ''' </summary>
        ''' <remarks></remarks>
        Private Class clsEvaluate
            Implements IComparable
            Public Property AIC As Double = 0
            Public Property BIC As Double = 0
            Public Property SquareR As Double = 0
            Public Property RSS As Double = 0
            Public UseIndexArray() As Integer = Nothing

            Public Sub New()

            End Sub

            Public Sub New(ByVal ai_aic As Double, ByVal ai_bic As Double, ByVal ai_r As Double, ByVal ai_ar() As Integer)
                Me.AIC = ai_aic
                Me.BIC = ai_bic
                Me.SquareR = ai_r
                Me.UseIndexArray = ai_ar
            End Sub

            Public Function CompareTo(ai_obj As Object) As Integer Implements IComparable.CompareTo
                'Nothing check
                If ai_obj Is Nothing Then
                    Return 1
                End If

                'Type check
                If Not Me.GetType() Is ai_obj.GetType() Then
                    Throw New ArgumentException("Different type", "obj")
                End If

                'Compare
                Dim mineValue As Double = Me.AIC
                Dim compareValue As Double = DirectCast(ai_obj, clsEvaluate).AIC
                If mineValue = compareValue Then
                    Return 0
                ElseIf mineValue < compareValue Then
                    Return 1
                Else
                    Return -1
                End If
            End Function

            Public Overrides Function ToString() As String
                Dim tempStr As String = String.Empty
                tempStr = tempStr & String.Format("Evaluate:") & ControlChars.NewLine
                tempStr = tempStr & String.Format(" RSS = {0}", RSS) & ControlChars.NewLine
                tempStr = tempStr & String.Format(" R^2 = {0}", SquareR) & ControlChars.NewLine
                tempStr = tempStr & String.Format(" AIC = {0}", AIC) & ControlChars.NewLine
                tempStr = tempStr & String.Format(" BIC = {0}", BIC) & ControlChars.NewLine
                Return tempStr
                'Return MyBase.ToString()
            End Function
        End Class

        ''' <summary>
        ''' evaluate
        ''' </summary>
        ''' <remarks></remarks>
        Private Function EvaluateRegression(ByVal w() As Double, ByVal CorrectDataVector() As Double, ByVal TrainDataMatrix()() As Double) As clsEvaluate
            Dim retEval = New clsEvaluate()

            'Calc RSS(residual sum of square)
            Dim rss As Double = 0
            If TrainDataMatrix Is Nothing OrElse TrainDataMatrix.Count = 0 Then
                Dim predictValue = w(0)
                For i As Integer = 0 To Me.CorrectDataVector.Count - 1
                    Dim temp = CorrectDataVector(i) - predictValue
                    rss += temp * temp
                Next
            Else
                For i As Integer = 0 To Me.CorrectDataVector.Count - 1
                    Dim predictValue = clsLinearRegression.Predict(w, TrainDataMatrix(i))
                    Dim temp = CorrectDataVector(i) - predictValue
                    rss += temp * temp
                Next
            End If
            retEval.RSS = rss

            'Calc R^2 決定係数
            Dim tempValue As Double = 0
            Dim correctAvg As Double = CorrectDataVector.Sum / CorrectDataVector.Count
            For i As Integer = 0 To CorrectDataVector.Count - 1
                Dim temp = CorrectDataVector(i) - correctAvg
                tempValue += temp * temp
            Next
            retEval.SquareR = 1 - (rss / tempValue)

            'AIC = n*ln(RSS/n)+2*K
            Dim n = CorrectDataVector.Count
            Dim k = w.Count + 1
            retEval.AIC = n * Math.Log(rss / n) + 2 * k
            retEval.BIC = n * Math.Log(rss / n) + Math.Log(n) * k

            Return retEval
        End Function

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

            'Array.Sort(ai_useIndex)
            Dim fldCount As Integer = Me.TrainDataMatrix(0).Count
            Dim recCount As Integer = Me.TrainDataMatrix.Count
            Dim retDataMat = New Double(recCount - 1)() {}

            For i As Integer = 0 To recCount - 1
                Dim tempArray(ai_useIndex.Count - 1) As Double
                Dim j As Integer = 0
                For Each useIndex In ai_useIndex
                    tempArray(j) = Me.TrainDataMatrix(i)(useIndex)
                    j += 1
                Next
                retDataMat(i) = tempArray
            Next

            Return retDataMat
        End Function

        ''' <summary>
        ''' restruct field by useindex
        ''' </summary>
        ''' <param name="ai_useIndex"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RestructFieldNames(ByVal ai_useIndex() As Integer) As String()
            'check
            If TrainDataMatrix Is Nothing OrElse CorrectDataVector Is Nothing Then
                Return Nothing
            End If

            'Array.Sort(ai_useIndex)
            Dim fldCount As Integer = Me.TrainDataMatrix(0).Count
            Dim retFiledlNames(ai_useIndex.Count - 1) As String
            Dim i As Integer = 0
            For Each useIndex In ai_useIndex
                retFiledlNames(i) = Me.TrainDataFields(useIndex)
                i += 1
            Next

            Return retFiledlNames
        End Function
#End Region
    End Class
End Namespace
