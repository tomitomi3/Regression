Imports CsvHelper

Namespace Regression
    ''' <summary>
    ''' データ行列操作クラス
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsDataMatrixCtrl
#Region "Member Dataset"
        ''' <summary></summary>
        Public Property FileName As String = String.Empty

        ''' <summary></summary>
        Public Property FilePath As String = String.Empty

        ''' <summary></summary>
        Public FieldNames() As String

        ''' <summary>dataset(org)</summary>
        Private orgDataMatrix()() As Double

        ''' <summary></summary>
        Private trainDataMatrix()() As Double

        ''' <summary></summary>
        Private trainFieldNames() As String

        ''' <summary></summary>
        Private correctVector() As Double

        ''' <summary></summary>
        Private correctFieldName = String.Empty
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
                            Me.FieldNames = csv.FieldHeaders.ToArray()
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
                        Me.FieldNames = csv.FieldHeaders.ToArray()
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
        ''' <param name="ai_colIndex"></param>
        ''' <remarks></remarks>
        Public Sub CreateDataMatrix(ByVal ai_colIndex As Integer)
            If Me.orgDataMatrix Is Nothing Then
                Return
            End If
            Dim numberOfField As Integer = Me.FieldNames.Count
            Dim numberOfData As Integer = Me.orgDataMatrix.Count
            Dim index As Integer = 0

            'restructure data matrix
            trainDataMatrix = New Double(numberOfData - 1)() {}
            correctVector = New Double(numberOfData - 1) {}
            For i As Integer = 0 To orgDataMatrix.Count - 1
                Dim tempArray(numberOfField - 2) As Double
                index = 0
                For j As Integer = 0 To numberOfField - 1
                    If j = ai_colIndex Then
                        correctVector(i) = orgDataMatrix(i)(j)
                    Else
                        tempArray(index) = orgDataMatrix(i)(j)
                        index += 1
                    End If
                Next
                trainDataMatrix(i) = tempArray
            Next

            'restructure field name
            trainFieldNames = New String(numberOfField - 2) {}
            correctFieldName = Me.FieldNames(ai_colIndex)
            index = 0
            For i As Integer = 0 To numberOfField - 1
                If i = ai_colIndex Then
                    Me.correctFieldName = Me.FieldNames(i)
                Else
                    Me.trainFieldNames(index) = Me.FieldNames(i)
                    index += 1
                End If
            Next
        End Sub

        Public Function GetTrainDataMatrix() As Double()()
            Return Me.trainDataMatrix
        End Function

        Public Function GetTrainFieldHeaders() As String()
            Return Me.trainFieldNames
        End Function

        Public Function GetCorrectDataVector() As Double()
            Return Me.correctVector
        End Function

        Public Function GetCorrectFieldHeader() As String
            Return Me.correctFieldName
        End Function

        ''' <summary>
        ''' get collumn array
        ''' </summary>
        ''' <param name="colIndex"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToColArray(ByVal colIndex As Integer) As Double()

        End Function

        ''' <summary>
        ''' get row array
        ''' </summary>
        ''' <param name="rowIndex"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToRowArray(ByVal rowIndex As Integer) As Double()

        End Function

        ''' <summary>
        ''' dataset property
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub OutputDatasetProperty()
            Console.WriteLine("Date            :{0}", System.DateTime.Now.ToString)
            Console.WriteLine("Dataset FileName:{0}", Me.FileName)
            If Me.orgDataMatrix IsNot Nothing Then
                Console.WriteLine("Dataset Records :{0}", Me.orgDataMatrix.Count)
            Else
                Console.WriteLine("Dataset Records :not read")
            End If
            Console.WriteLine("Number of Field :{0}", Me.FieldNames.Count)
            Console.WriteLine("Fields")
            For i As Integer = 0 To Me.FieldNames.Count - 1
                Console.WriteLine(" {0}:{1}", i, Me.FieldNames(i))
            Next
            Console.WriteLine("")
        End Sub
#End Region

#Region "Private"
#End Region
    End Class
End Namespace
