﻿Imports CsvHelper
Imports MathNet.Numerics.LinearAlgebra

Namespace Regression
    ''' <summary>
    ''' データ行列操作クラス
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsDataMatrixCtrl
#Region "Member"
        ''' <summary>read filen ame</summary>
        Public Property FileName As String = String.Empty

        ''' <summary>read file path</summary>
        Public Property FilePath As String = String.Empty

        ''' <summary>Target index</summary>
        Public Property TargetIndex As Integer = -1

        ''' <summary>オリジナル フィールド名</summary>
        Private orgFieldNames() As String

        ''' <summary>オリジナル データ行列</summary>
        Private orgDataMatrix()() As Double

        ''' <summary>説明変数（データ行列）</summary>
        Private trainDataMatrix()() As Double

        ''' <summary>説明変数（フィールド名）</summary>
        Private trainFieldNames() As String

        ''' <summary>目的変数ベクトル</summary>
        Private correctVector() As Double

        ''' <summary>番号対フィールド名</summary>
        Private dicTrainIndexVsFieldName As New Dictionary(Of Integer, String)

        ''' <summary>目的変数フィールド名</summary>
        Private correctFieldName = String.Empty

        ''' <summary>分散共分散行列</summary>
        Private varCovarMatrix()() As Double

        ''' <summary>相関行列</summary>
        Private correMatrix()() As Double
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
        ''' Read header
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReadHeader() As Boolean
            'Exist check
            If System.IO.File.Exists(Me.FilePath) = False Then
                Return False
            End If

            'save file info
            Me.FileName = System.IO.Path.GetFileName(Me.FilePath)

            'read csv data
            Try
                Dim tempMat = New List(Of Double())
                Using r = New IO.StreamReader(Me.FilePath, Text.Encoding.GetEncoding("SHIFT_JIS"))
                    Using csv = New CsvHelper.CsvReader(r)
                        csv.Configuration.HasHeaderRecord = True
                        csv.Read()
                        If csv.FieldHeaders IsNot Nothing Then
                            Me.orgFieldNames = csv.FieldHeaders.ToArray()
                        End If
                    End Using
                End Using

            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function

        ''' <summary>
        ''' read dataset with header
        ''' </summary>
        ''' <remarks></remarks>
        Public Function Read() As Boolean
            'Exist check
            If System.IO.File.Exists(Me.FilePath) = False Then
                Return False
            End If

            'save file info
            Me.FileName = System.IO.Path.GetFileName(Me.FilePath)

            'read csv data
            Try
                Dim tempMat = New List(Of Double())
                Using r = New IO.StreamReader(Me.FilePath, Text.Encoding.GetEncoding("SHIFT_JIS"))
                    Using csv = New CsvHelper.CsvReader(r)
                        csv.Configuration.HasHeaderRecord = True
                        While (csv.Read())
                            Dim tempRowArray = csv.CurrentRecord.Select(Function(a As String)
                                                                            Return CDbl(a)
                                                                        End Function).ToArray
                            tempMat.Add(tempRowArray)
                        End While
                        Me.orgFieldNames = csv.FieldHeaders.ToArray()
                    End Using
                End Using

                'restructure org dataset
                Me.orgDataMatrix = New Double(tempMat.Count - 1)() {}
                For i As Integer = 0 To tempMat.Count - 1
                    Me.orgDataMatrix(i) = tempMat(i)
                Next

                'delete
                tempMat = Nothing
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function

        ''' <summary>
        ''' create data matrix
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CreateDataMatrix()
            If Me.orgDataMatrix Is Nothing Then
                Return
            End If
            If TargetIndex = -1 OrElse Me.orgFieldNames.Count < TargetIndex Then
                Return
            End If

            Dim fieldCount As Integer = Me.orgFieldNames.Count
            Dim recCount As Integer = Me.orgDataMatrix.Count
            Dim index As Integer = 0

            'restruc data matrix
            trainDataMatrix = New Double(recCount - 1)() {}
            correctVector = New Double(recCount - 1) {}
            For i As Integer = 0 To orgDataMatrix.Count - 1
                Dim tempArray(fieldCount - 2) As Double
                index = 0
                For j As Integer = 0 To fieldCount - 1
                    If j = TargetIndex Then
                        correctVector(i) = orgDataMatrix(i)(j)
                    Else
                        tempArray(index) = orgDataMatrix(i)(j)
                        index += 1
                    End If
                Next
                trainDataMatrix(i) = tempArray
            Next

            'restruct field name
            dicTrainIndexVsFieldName.Clear()
            trainFieldNames = New String(fieldCount - 2) {}
            correctFieldName = Me.orgFieldNames(TargetIndex)
            index = 0
            For i As Integer = 0 To fieldCount - 1
                If i = TargetIndex Then
                    Me.correctFieldName = Me.orgFieldNames(i)
                Else
                    Me.trainFieldNames(index) = Me.orgFieldNames(i)
                    dicTrainIndexVsFieldName.Add(index, Me.trainFieldNames(index)) 'resturct table
                    index += 1
                End If
            Next
        End Sub

        ''' <summary>
        ''' get train data matrix(with out TargetIndex from orgDataMatrix)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTrainDataMatrix() As Double()()
            Return Me.trainDataMatrix
        End Function

        ''' <summary>
        ''' get train field names(with out TargetIndex from orgDataMatrix)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTrainFieldNames() As String()
            Return Me.trainFieldNames
        End Function

        ''' <summary>
        ''' Get correct dataset
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCorrectDataVector() As Double()
            Return Me.correctVector
        End Function

        ''' <summary>
        ''' Get correct field name
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCorrectFieldName() As String
            Return Me.correctFieldName
        End Function

        ''' <summary>
        ''' get collumn array from train data matrix
        ''' </summary>
        ''' <param name="colIndex"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToColArray(ByVal colIndex As Integer) As Double()
            Dim recCount = Me.trainDataMatrix.Count()
            Dim retArray(recCount - 1) As Double
            For i As Integer = 0 To recCount - 1
                retArray(i) = Me.trainDataMatrix(i)(colIndex)
            Next
            Return retArray
        End Function

        ''' <summary>
        ''' get row array from train data matrix
        ''' </summary>
        ''' <param name="rowIndex"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToRowArray(ByVal rowIndex As Integer) As Double()
            Return Me.trainDataMatrix(rowIndex).ToArray()
        End Function

        ''' <summary>
        ''' dataset property
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub OutputDatasetProperty()
            If orgDataMatrix Is Nothing Then
                Console.WriteLine("Dataset FileName:No Data")
                Return
            End If
            Console.WriteLine("Dataset FileName:{0}", Me.FileName)
            Console.WriteLine("Date            :{0}", System.DateTime.Now.ToString)
            If Me.orgDataMatrix IsNot Nothing Then
                Console.WriteLine("Dataset Records :{0}", Me.orgDataMatrix.Count)
            Else
                Console.WriteLine("Dataset Records :not read")
            End If
            Console.WriteLine("Number of Field :{0}", Me.orgFieldNames.Count)
            Console.WriteLine("Fields")
            For i As Integer = 0 To Me.orgFieldNames.Count - 1
                Console.WriteLine(" {0}:{1}", i, Me.orgFieldNames(i))
            Next
            Console.WriteLine("")
        End Sub

        ''' <summary>
        ''' Check Correlation
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CheckCorrelation()
            'check
            If Me.orgDataMatrix Is Nothing Then
                Me.CreateDataMatrix()
                If Me.orgDataMatrix Is Nothing Then
                    Return
                End If
            End If

            'cal Var-CoVarMatrix and CorrelationMatrix
            Me.varCovarMatrix = Me.CalcVarianceCovarianceMatrix()
            Me.correMatrix = Me.CalcCorrelationMatrix()

            'access lower triangle
            Dim cor = New List(Of clsCorrelationSort)
            For i = 0 To correMatrix.Count - 1
                For j = 0 To correMatrix.Count - 1
                    If i = j Then
                        Exit For
                    End If
                    cor.Add(New clsCorrelationSort(i, j, Me.correMatrix(i)(j)))
                Next
            Next
            cor.Sort()

            'High correlation Top10
            Dim topNCount As Integer = 10
            If cor.Count < 10 Then
                topNCount = cor.Count
            End If
            Console.WriteLine("High Correlation Top{0}", topNCount)
            For i As Integer = 0 To topNCount - 1
                Dim temp = cor(i)
                Console.WriteLine(" {0} {1} {2}", Me.dicTrainIndexVsFieldName(temp.RowIndex), Me.dicTrainIndexVsFieldName(temp.ColIndex), temp.Correlation)
            Next
        End Sub
#End Region
#Region "Private"
        ''' <summary>
        ''' 相関行列のソートクラス
        ''' </summary>
        ''' <remarks></remarks>
        Private Class clsCorrelationSort
            Implements IComparable
            ''' <summary>行</summary>
            Public Property RowIndex As Integer = 0

            ''' <summary>列</summary>
            Public Property ColIndex As Integer = 0

            ''' <summary>相関</summary>
            Public Property Correlation As Double = 0.0

            ''' <summary>相関の絶対値</summary>
            Private AbsCorrelation As Double = 0.0

            ''' <summary>相関のソート条件（True:相関の絶対値を用いてソート）</summary>
            Public Property IsUseAbsoluteSort As Boolean = True

            Public Sub New(ByVal row As Integer, ByVal col As Integer, ByVal cor As Double, Optional ByVal isUseAbsSort As Boolean = True)
                Me.RowIndex = row
                Me.ColIndex = col
                Me.Correlation = cor
                Me.AbsCorrelation = Math.Abs(cor)
                Me.IsUseAbsoluteSort = isUseAbsSort
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
                Dim mineValue As Double = Me.AbsCorrelation
                Dim compareValue As Double = DirectCast(ai_obj, clsCorrelationSort).AbsCorrelation
                If IsUseAbsoluteSort = False Then
                    mineValue = Me.Correlation
                    compareValue = DirectCast(ai_obj, clsCorrelationSort).Correlation
                End If
                If mineValue = compareValue Then
                    Return 0
                ElseIf mineValue < compareValue Then
                    Return 1
                Else
                    Return -1
                End If
            End Function
        End Class

        ''' <summary>
        ''' 分散共分散行列
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CalcVarianceCovarianceMatrix() As Double()()
            Dim fieldCount = Me.trainDataMatrix(0).Count
            Dim recCount = Me.trainDataMatrix.Count()

            'calc variance covariance matrix
            Dim x = CreateMatrix.DenseOfRowArrays(trainDataMatrix)
            For i As Integer = 0 To fieldCount - 1
                Dim avg = x.Column(i).Average()
                For j As Integer = 0 To recCount - 1
                    x(j, i) -= avg
                Next
            Next

            'VarianceCovarianceMatrix = 1/n( x^T * X )
            Dim varCovarMat = (x.TransposeThisAndMultiply(x)) / recCount
            Return varCovarMat.ToRowArrays()
        End Function

        ''' <summary>
        ''' 相関行列
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CalcCorrelationMatrix() As Double()()
            Dim fieldCount = Me.trainDataMatrix(0).Count
            Dim recCount = Me.trainDataMatrix.Count()

            'correlation matrix もう少しシンプルに出来る。
            Dim corMat = CreateMatrix.Dense(Of Double)(fieldCount, fieldCount)
            For i As Integer = 0 To fieldCount - 1
                For j As Integer = 0 To fieldCount - 1
                    If i = j Then
                        'diagonal is all r=1.0
                        corMat(i, j) = 1.0
                    Else
                        'r = sxy/(sxx^0.5 * syy^0.5)
                        corMat(i, j) = varCovarMatrix(i)(j) / (Math.Sqrt(varCovarMatrix(i)(i)) * Math.Sqrt(varCovarMatrix(j)(j)))
                    End If
                Next
            Next

            Return corMat.ToRowArrays()
        End Function

        Private Sub DebugView(ByVal ai_mat()() As Double)
            'ControlChars.Tab

        End Sub
#End Region
    End Class
End Namespace