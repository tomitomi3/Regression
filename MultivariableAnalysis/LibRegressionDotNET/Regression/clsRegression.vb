Imports CsvHelper

Namespace Regression
    ''' <summary>
    ''' Regression
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsRegression
#Region "Member Dataset"
        ''' <summary>org dataset</summary>
        Private datas As New List(Of List(Of Double))

        ''' <summary>dataset field name</summary>
        Private fieldNames As New List(Of String)

        ''' <summary>Train dataset</summary>
        Private trainDatas As New List(Of List(Of Double))

        ''' <summary>Correct data</summary>
        Private correctDatas As New List(Of Double)

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
            If System.IO.File.Exists(ai_path) = False Then
                Return False
            End If

            Me.datasetFileName = System.IO.Path.GetFileName(ai_path)
            Me.datasetFilePath = ai_path

            Try
                Using r = New IO.StreamReader(ai_path, Text.Encoding.GetEncoding("SHIFT_JIS"))
                    Using csv = New CsvHelper.CsvReader(r)
                        csv.Configuration.HasHeaderRecord = True
                        While csv.Read()
                            Dim rowArray = csv.CurrentRecord.Select(Function(a As String)
                                                                        Return CDbl(a)
                                                                    End Function).ToList()
                            Me.datas.Add(rowArray)
                        End While
                        Me.fieldNames = csv.FieldHeaders.ToList()
                    End Using
                End Using
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
            For i As Integer = 0 To Me.fieldNames.Count - 1
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

            'restructure data matrix
            Dim mat(datas.Count - 1)() As Double
            For i As Integer = 0 To Me.datas.Count - 1
                Dim array(Me.fieldNames.Count - 2) As Double
                For j As Integer = 0 To Me.fieldNames.Count - 1
                    If j = Me.TargetVariableIndex Then
                        Continue For
                    End If
                    array(j) = Me.datas(i)(j)
                Next
                mat(i) = array
            Next

            'ライブラリ↓
            'http://numerics.mathdotnet.com/Regression.html

            '分散共分散行列
            'Dim datamat = MathNet.Numerics.LinearAlgebra.CreateMatrix.DenseOfRowArrays(mat)
            'Dim datamatt = datamat.Transpose()
            'Dim aa = datamatt * datamat
            'Console.WriteLine(aa)
        End Function

        ''' <summary>
        ''' dataset property
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ConsoleDatasetProperty()
            Console.WriteLine("Date            :{0}", System.DateTime.Now.ToString)
            Console.WriteLine("Dataset FileName:{0}", Me.datasetFileName)
            Console.WriteLine("Dataset Records :{0}", Me.datas.Count)
            Console.WriteLine("Number of Field :{0}", Me.fieldNames.Count)
            Console.WriteLine("Fields")
            For i As Integer = 0 To Me.fieldNames.Count - 1
                Console.WriteLine(" {0}:{1}", i, Me.fieldNames(i))
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
