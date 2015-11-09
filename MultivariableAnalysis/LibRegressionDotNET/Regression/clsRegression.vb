Imports CsvHelper

Namespace Regression
    ''' <summary>
    ''' Regression
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsRegression
#Region "Member Dataset"
        ''' <summary>dataset(org)</summary>
        Private orgDataMatrix()() As Double

        ''' <summary>dataset field name(org)</summary>
        Private orgFieldNames() As String

        ''' <summary>train dataset</summary>
        Private trainDataMatrix()() As Double

        ''' <summary>train dataset field name</summary>
        Private trainFieldNames() As String

        ''' <summary>correct Array</summary>
        Private correctVector() As Double

        ''' <summary>cprrect field name</summary>
        Private correctFieldName As String

        Private datasetFileName As String = String.Empty
        Private datasetFilePath As String = String.Empty
#End Region

#Region "Member Regression paramters"
        ''' <summary>using CV</summary>
        Public Property IsCrossValidation As Boolean = True

        ''' <summary>K-Fold by CV</summary>
        Public Property KFoldCrossValidation As Integer = True

        ''' <summary>using Variable selection</summary>
        Public Property IsVariableSelection As Boolean = True

        ''' <summary>Variable selection method</summary>
        Property VariableSelectionMethod As SelectionMethod

        Public Enum SelectionMethod
            ForwardSelection
            BackwardSelection
            StepwiseSelection
            AllSubsetSelection
        End Enum

        Public Property TargetVariableIndex As Integer = -1
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
        ''' read dataset
        ''' </summary>
        ''' <param name="ai_path"></param>
        ''' <remarks></remarks>
        Public Function ReadDatasetByCsv(ByVal ai_path As String) As Boolean
            'Exist check
            If System.IO.File.Exists(ai_path) = False Then
                Return False
            End If

            'save file info
            Me.datasetFileName = System.IO.Path.GetFileName(ai_path)
            Me.datasetFilePath = ai_path

            'read csv data
            Try
                Dim tempMat = New List(Of Double())
                Using r = New IO.StreamReader(ai_path, Text.Encoding.GetEncoding("SHIFT_JIS"))
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
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function

        ''' <summary>
        ''' DataCheck
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CheckData()
            Console.WriteLine("DoDataCleansing()")

            'without Target
            Dim index As New List(Of Integer)
            For i As Integer = 0 To Me.orgFieldNames.Count - 1
                If i = Me.TargetVariableIndex Then
                    Continue For
                End If
                index.Add(i)
            Next

            'generate combination
            Dim combination As New List(Of Integer())
            While (index.Count <> 1)
                Dim firstIndex = index(0)
                index.RemoveAt(0)
                For Each value In index
                    combination.Add({firstIndex, value})
                Next
            End While

            Console.WriteLine("End DoDataCleansing()")
        End Sub

        ''' <summary>
        ''' Do Regression Analysis
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DoRegression() As Boolean
            'check
            If Me.TargetVariableIndex = -1 Then
                Return False
            End If

            'create train datamatrix
            Me.CreateTrainDataMatrix(Me.orgDataMatrix, Me.orgFieldNames, Me.TargetVariableIndex, Me.trainDataMatrix, Me.trainFieldNames, Me.correctVector, Me.correctFieldName)

            'using QR method Ref:http://numerics.mathdotnet.com/Regression.html
            Dim weightVector = MathNet.Numerics.Fit.MultiDim(trainDataMatrix, correctVector, intercept:=True, method:=MathNet.Numerics.LinearRegression.DirectRegressionMethod.QR)
            Console.WriteLine(MathNet.Numerics.LinearAlgebra.CreateMatrix.DenseOfColumnArrays(weightVector))

            'calc residual sum of square
            Dim residualArray(trainDataMatrix.Count - 1) As Double
            Dim rssArray(trainDataMatrix.Count - 1) As Double
            For i As Integer = 0 To residualArray.Count - 1
                Dim tempSum As Double = weightVector(0)
                For j As Integer = 1 To weightVector.Count - 1
                    tempSum += weightVector(j) * trainDataMatrix(i)(j - 1)
                Next
                residualArray(i) = correctVector(i) - tempSum
                rssArray(i) = residualArray(i) * residualArray(i)
            Next

            Dim rss = rssArray.Sum

        End Function

        '分散共分散行列
        'With Nothing
        '    Dim datamat = MathNet.Numerics.LinearAlgebra.CreateMatrix.DenseOfRowArrays(dataMatrix)
        '    Dim datamatt = datamat.Transpose()
        '    Dim avgArray = datamat.ColumnSums() / orgDataMatrix.Count
        '    For i As Integer = 0 To datamat.RowCount - 1
        '        For j As Integer = 0 To datamat.ColumnCount - 1
        '            datamat(i, j) -= avgArray(j)
        '        Next
        '    Next
        '    Dim crossMat = datamat * datamatt
        'End With

        '平均ベクトルを求める
        'Dim avgArray(Me.fieldNames.Count - 2) As Double
        'For i As Integer = 0 To dataMatrix.Length - 1
        '    For j As Integer = 0 To Me.fieldNames.Count - 2
        '        avgArray(j) += dataMatrix(i)(j)
        '    Next
        'Next
        'For i As Integer = 0 To avgArray.Length - 1
        '    avgArray(i) /= datas.Count
        'Next

        ''' <summary>
        ''' AIC
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CalcAIC() As Double

        End Function

        ''' <summary>
        ''' Datamatrix
        ''' </summary>
        ''' <param name="ai_orgDataMatrix"></param>
        ''' <param name="ai_FieldNames"></param>
        ''' <param name="ai_colIndex"></param>
        ''' <param name="ao_trainDataMatrix"></param>
        ''' <param name="ao_trainFieldNames"></param>
        ''' <param name="ao_correctArray"></param>
        ''' <param name="ao_correctFieldName"></param>
        ''' <remarks></remarks>
        Private Sub CreateTrainDataMatrix(ByVal ai_orgDataMatrix As Double()(), ByVal ai_FieldNames As String(), ByVal ai_colIndex As Integer, _
                                          ByRef ao_trainDataMatrix As Double()(), ByRef ao_trainFieldNames As String(), _
                                          ByRef ao_correctArray As Double(), ByRef ao_correctFieldName As String)
            'check
            If ai_colIndex < 0 OrElse ai_orgDataMatrix Is Nothing OrElse ai_orgDataMatrix.Count = 0 Then
                Return
            End If

            ao_trainDataMatrix = New Double(ai_orgDataMatrix.Count - 1)() {}
            ao_trainFieldNames = New String(ai_FieldNames.Count - 2) {}
            ao_correctArray = New Double(ai_orgDataMatrix.Count - 1) {}
            ao_correctFieldName = String.Empty

            'restructure data matrix
            For i As Integer = 0 To ai_orgDataMatrix.Count - 1
                Dim tempArray(ai_FieldNames.Count - 2) As Double
                For j As Integer = 0 To ai_FieldNames.Count - 1
                    If j = Me.TargetVariableIndex Then
                        ao_correctArray(i) = ai_orgDataMatrix(i)(j)
                    Else
                        tempArray(j) = ai_orgDataMatrix(i)(j)
                    End If
                Next
                ao_trainDataMatrix(i) = tempArray
            Next
        End Sub

        ''' <summary>
        ''' dataset property
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ConsoleDatasetProperty()
            Console.WriteLine("Date            :{0}", System.DateTime.Now.ToString)
            Console.WriteLine("Dataset FileName:{0}", Me.datasetFileName)
            Console.WriteLine("Dataset Records :{0}", Me.orgDataMatrix.Count)
            Console.WriteLine("Number of Field :{0}", Me.orgFieldNames.Count)
            Console.WriteLine("Fields")
            For i As Integer = 0 To Me.orgFieldNames.Count - 1
                Console.WriteLine(" {0}:{1}", i, Me.orgFieldNames(i))
            Next
            Console.WriteLine("")
        End Sub

        ''' <summary>
        ''' Regression result
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ConsoleRegressionResult()
            Throw New NotImplementedException
        End Sub
#End Region
    End Class
End Namespace
