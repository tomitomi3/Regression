Imports LibOptimization
Imports CsvHelper

''' <summary>
''' lasso
''' </summary>
''' <remarks>
''' Refference
''' [1]Tibshirani, R. "Regression shrinkage and selection via the lasso.", Journal of the Royal Statistical Society, Series B, Vol 58, No. 1, pp. 267–288, 1996.
''' 
''' Traindataset
''' Datasets from "The Elements of Statistical Learning"
''' http://statweb.stanford.edu/~tibs/ElemStatLearn/
''' ->Data->Prostate
''' </remarks>
Public Class clsLasso : Inherits LibOptimization.Optimization.absObjectiveFunction
    ''' <summary>Train dataset</summary>
    Private trainDatas As New List(Of List(Of Double))

    ''' <summary>Correct data</summary>
    Private correctDatas As New List(Of Double)

    ''' <summary>Field name</summary>
    Private fieldNames As New List(Of String)

    ''' <summary>Shrinkage parameter</summary>
    Public Property ShrinkageParameter As Double = 1.0

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'nop
    End Sub

    ''' <summary>
    ''' init
    ''' </summary>
    ''' <param name="ai_path"></param>
    ''' <remarks></remarks>
    Public Function Init(ByVal ai_path As String) As Boolean
        If System.IO.File.Exists(ai_path) = False Then
            Return False
        End If

        Me.ReadCsv(ai_path, Me.trainDatas, Me.correctDatas, Me.fieldNames)

        Return True
    End Function

    ''' <summary>
    ''' Read csv
    ''' </summary>
    ''' <param name="ai_path"></param>
    ''' <param name="ao_datas"></param>
    ''' <param name="ao_correct"></param>
    ''' <param name="ao_fields"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ReadCsv(ByVal ai_path As String, ByVal ao_datas As List(Of List(Of Double)),
                             ByVal ao_correct As List(Of Double), ByVal ao_fields As List(Of String)) As Boolean
        Try
            Using r = New IO.StreamReader(ai_path, Text.Encoding.GetEncoding("SHIFT_JIS"))
                Using csv = New CsvHelper.CsvReader(r)
                    csv.Configuration.HasHeaderRecord = True

                    Dim isRead As Boolean = False
                    Dim indexTrain As Integer = 0
                    While csv.Read()
                        If isRead = False Then
                            ao_fields.AddRange(csv.FieldHeaders())
                            indexTrain = csv.CurrentRecord.Length - 1
                            isRead = True
                        End If

                        Dim tempArray As New List(Of Double)
                        For i As Integer = 0 To csv.CurrentRecord.Length - 1
                            'skip ID
                            If i = 0 Then
                                Continue For
                            End If

                            'correct
                            If i = indexTrain Then
                                Dim value As Double = 0.0
                                If Double.TryParse(csv.CurrentRecord(i), value) Then
                                    ao_correct.Add(value)
                                Else
                                    If csv.CurrentRecord(i) = "T" Then
                                        ao_correct.Add(10)
                                    Else
                                        ao_correct.Add(0)
                                    End If
                                End If
                                Continue For
                            End If

                            'train
                            tempArray.Add(Double.Parse(csv.CurrentRecord(i)))
                        Next
                        ao_datas.Add(tempArray)
                    End While
                End Using
            End Using

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Regression function
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function Regression(ByVal evalTrainDatas As List(Of Double), ByVal ai_coeff As Double()) As Double
        Dim predict As Double = 0
        For i As Integer = 0 To evalTrainDatas.Count - 1
            predict += evalTrainDatas(i) * ai_coeff(i)
        Next
        predict += ai_coeff(ai_coeff.Count - 1)
        Return predict
    End Function

    ''' <summary>
    ''' Evaluate
    ''' </summary>
    ''' <param name="ai_path"></param>
    ''' <param name="ai_coeff"></param>
    ''' <param name="ai_allOutputEval"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Evaluate(ByVal ai_path As String, ByVal ai_coeff As Double(), ByVal ai_allOutputEval As Boolean) As Boolean
        Dim evalTrainDatas As New List(Of List(Of Double))
        Dim evalCorrectDatas As New List(Of Double)
        Dim fields As New List(Of String)
        Me.ReadCsv(ai_path, evalTrainDatas, evalCorrectDatas, fields)

        Dim sumSquareErr As Double = 0.0
        For i As Integer = 0 To evalTrainDatas.Count - 1
            Dim predict As Double = Me.Regression(evalTrainDatas(i), ai_coeff)
            sumSquareErr += (evalCorrectDatas(i) - predict) ^ 2
            If ai_allOutputEval = True Then
                Console.WriteLine("Correct:{0} Eval:{1} ", evalCorrectDatas(i), predict)
            End If
        Next
        Console.WriteLine("Sum Error:{0}", sumSquareErr)
        Console.WriteLine()

        Return True
    End Function

#Region "for LibOptimization"
    ''' <summary>
    ''' Model
    ''' </summary>
    ''' <param name="ai_coeff"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function F(ByVal ai_coeff As List(Of Double)) As Double
        Dim sumDiffSquare As Double = 0

        For i As Integer = 0 To Me.trainDatas.Count - 1
            Dim predict As Double = Me.Regression(Me.trainDatas(i), ai_coeff.ToArray())
            sumDiffSquare += (Me.correctDatas(i) - predict) ^ 2 '二乗誤差
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
            Return fieldNames.Count - 1 'field + 1(const value)
        End Get
    End Property
#End Region
End Class
