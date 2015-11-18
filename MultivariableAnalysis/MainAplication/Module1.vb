Imports LibRegressionDotNET.Regression

Module Module1
    Sub Main()
        'Read data set and create data matrix
        Dim mat = New LibRegressionDotNET.Regression.clsDataMatrixCtrl()
        mat.FilePath = "..\..\..\..\_dataset\housing_modify_org.csv"
        'mat.FilePath = "..\..\..\..\_dataset\school_test.csv"
        'mat.FilePath = "..\..\..\..\_dataset\sample_baddata_cor1.csv"
        If mat.Read() = False Then
            Return
        End If
        mat.OutputDatasetProperty()

        'set target
        mat.TargetIndex = 13 '13
        mat.CreateDataMatrix()
        'create data matrix without higher correlation variable
        mat.CheckCorrelation(False)
        mat.WithoutCorreationCriteria = 0.99
        mat.CheckRemoveIndexByCorrelation(True)
        mat.CreateDataMatrix()

        'for CV
        'for Testdata, Traindata
        mat.CreateSplitDataMatrix(10)

        'Do regression
        Dim r = New LibRegressionDotNET.Regression.clsLinearRegression()

        Console.WriteLine("===============================")
        Console.WriteLine("Variable select")
        Console.WriteLine("===============================")
        For Each vSelection In [Enum].GetValues(GetType(clsLinearRegression.EnumVariableSelection))
            r.TrainDataMatrix = mat.GetTrainDataMatrix()
            r.TrainDataFields = mat.GetTrainFieldNames
            r.CorrectDataVector = mat.GetCorrectDataVector()
            r.CorrectDataField = mat.GetCorrectFieldName
            r.VariableSelection = vSelection
            r.RegressionMethod = clsLinearRegression.EnumRegressionMethod.LinearRegression
            r.DoRegression()
            r.OutputRegressionResult()
        Next
        Console.WriteLine("===============================")
        Console.WriteLine("Ridge")
        Console.WriteLine("===============================")
        For i As Integer = 0 To 10
            r.TrainDataMatrix = mat.GetTrainDataMatrix()
            r.TrainDataFields = mat.GetTrainFieldNames
            r.CorrectDataVector = mat.GetCorrectDataVector()
            r.CorrectDataField = mat.GetCorrectFieldName
            r.RegressionMethod = clsLinearRegression.EnumRegressionMethod.RidgeRegression
            r.RidgeParameter = 100000 * 10 ^ -i
            r.DoRegression()
            r.OutputRegressionResult()
        Next

        Console.WriteLine("===============================")
        Console.WriteLine("LASSO")
        Console.WriteLine("===============================")
        For i As Integer = 0 To 10
            r.TrainDataMatrix = mat.GetTrainDataMatrix()
            r.TrainDataFields = mat.GetTrainFieldNames
            r.CorrectDataVector = mat.GetCorrectDataVector()
            r.CorrectDataField = mat.GetCorrectFieldName
            r.RegressionMethod = clsLinearRegression.EnumRegressionMethod.LassoRegression
            r.RidgeParameter = 100000 * 10 ^ -i
            r.DoRegression()
            r.OutputRegressionResult()
        Next

        Console.ReadLine()
        'issue
        'correlation
        'hogehoge.AIC, hogehoge.BIC
    End Sub
End Module
