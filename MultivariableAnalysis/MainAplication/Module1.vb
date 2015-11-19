Imports LibRegressionDotNET.Regression

Module Module1
    Sub Main()
        '回帰式、回帰式（R>0.9除外）、リッジ回帰
        With Nothing
            'Read data set and create data matrix
            Dim mat = New LibRegressionDotNET.Regression.clsDataMatrixCtrl()
            mat.FilePath = "..\..\..\..\_dataset\school_test.csv"
            If mat.Read() = False Then
                Return
            End If
            mat.OutputDatasetProperty()
            mat.TargetIndex = 3 '13
            mat.CreateDataMatrix()

            Dim results = New List(Of LibRegressionDotNET.Regression.clsLinearRegression.clsEvaluate)

            'regression
            Dim r = New LibRegressionDotNET.Regression.clsLinearRegression()

            'normal
            Console.WriteLine("相関排除無し")
            r.TrainDataMatrix = mat.GetTrainDataMatrix()
            r.TrainDataFields = mat.GetTrainFieldNames
            r.CorrectDataVector = mat.GetCorrectDataVector()
            r.CorrectDataField = mat.GetCorrectFieldName
            r.VariableSelection = clsLinearRegression.EnumVariableSelection.NotUseVariableSelection
            r.DoRegression()
            results.Add(r.Result)
            r.OutputRegressionResult()

            'normal remove
            Console.WriteLine("相関排除有り")
            mat.CheckCorrelation(False)
            mat.WithoutCorreationCriteria = 0.9
            mat.CheckRemoveIndexByCorrelation(True)
            mat.CreateDataMatrix()
            r.TrainDataMatrix = mat.GetTrainDataMatrix()
            r.TrainDataFields = mat.GetTrainFieldNames
            r.CorrectDataVector = mat.GetCorrectDataVector()
            r.CorrectDataField = mat.GetCorrectFieldName
            r.VariableSelection = clsLinearRegression.EnumVariableSelection.NotUseVariableSelection
            r.DoRegression()
            results.Add(r.Result)
            r.OutputRegressionResult()

            mat.CheckCorrelation(False)
            mat.WithoutCorreationCriteria = 1.0
            mat.CheckRemoveIndexByCorrelation(True)
            mat.CreateDataMatrix()

            'ridge regression
            For i As Integer = 0 To 16
                r.TrainDataMatrix = mat.GetTrainDataMatrix()
                r.TrainDataFields = mat.GetTrainFieldNames
                r.CorrectDataVector = mat.GetCorrectDataVector()
                r.CorrectDataField = mat.GetCorrectFieldName
                r.RegressionMethod = clsLinearRegression.EnumRegressionMethod.RidgeRegression
                r.RidgeParameter = 0.1 * 2 ^ i
                r.DoRegression()
                results.Add(r.Result)
                r.OutputRegressionResult()
            Next

            'output
            Console.WriteLine(results(0).ToCsvHeader())
            For Each info In results
                Console.WriteLine(info.ToCsv())
            Next
        End With

        With Nothing
            'Read data set and create data matrix
            Dim mat = New LibRegressionDotNET.Regression.clsDataMatrixCtrl()
            mat.FilePath = "..\..\..\..\_dataset\school_test10.csv"
            If mat.Read() = False Then
                Return
            End If
            mat.OutputDatasetProperty()
            mat.TargetIndex = 10
            mat.CreateDataMatrix()

            Dim results = New List(Of LibRegressionDotNET.Regression.clsLinearRegression.clsEvaluate)

            'regression
            Dim r = New LibRegressionDotNET.Regression.clsLinearRegression()

            'normal
            Console.WriteLine("全変数使う")
            r.TrainDataMatrix = mat.GetTrainDataMatrix()
            r.TrainDataFields = mat.GetTrainFieldNames
            r.CorrectDataVector = mat.GetCorrectDataVector()
            r.CorrectDataField = mat.GetCorrectFieldName
            r.VariableSelection = clsLinearRegression.EnumVariableSelection.NotUseVariableSelection
            r.DoRegression()
            results.Add(r.Result)
            r.OutputRegressionResult()

            'normal remove
            Console.WriteLine("全変数使う")
            r.TrainDataMatrix = mat.GetTrainDataMatrix()
            r.TrainDataFields = mat.GetTrainFieldNames
            r.CorrectDataVector = mat.GetCorrectDataVector()
            r.CorrectDataField = mat.GetCorrectFieldName
            r.VariableSelection = clsLinearRegression.EnumVariableSelection.StepwiseSelection
            r.DoRegression()
            results.Add(r.Result)
            r.OutputRegressionResult()

            'ridge regression
            Console.WriteLine("LASSO回帰")
            For i As Integer = 0 To 16
                r.TrainDataMatrix = mat.GetTrainDataMatrix()
                r.TrainDataFields = mat.GetTrainFieldNames
                r.CorrectDataVector = mat.GetCorrectDataVector()
                r.CorrectDataField = mat.GetCorrectFieldName
                r.RegressionMethod = clsLinearRegression.EnumRegressionMethod.LassoRegression
                r.ShrinkageParameter = 0.5 * 0.1 * 2 ^ i
                r.DoRegression()
                results.Add(r.Result)
                r.OutputRegressionResult()
            Next

            'output
            Console.WriteLine(results(0).ToCsvHeader())
            For Each info In results
                Console.WriteLine(info.ToCsv())
            Next
        End With

        'Console.ReadLine()
        'issue
        'correlation
        'hogehoge.AIC, hogehoge.BIC
    End Sub
End Module
