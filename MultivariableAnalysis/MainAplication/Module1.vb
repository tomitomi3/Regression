Imports LibRegressionDotNET.Regression

Module Module1
    Sub Main()
        'Read data set and create data matrix
        Dim mat = New LibRegressionDotNET.Regression.clsDataMatrixCtrl()
        mat.FilePath = "..\..\..\..\_dataset\housing_modify_org.csv"
        'mat.FilePath = "..\..\..\..\_dataset\sample_baddata_cor1.csv"
        If mat.Read() = False Then
            Return
        End If
        mat.OutputDatasetProperty()

        'set target
        mat.TargetIndex = 3 '13
        mat.CreateDataMatrix()
        'create data matrix without higher correlation variable
        mat.CheckCorrelation(False)
        mat.WithoutCorreationCriteria = 0.99
        mat.CheckRemoveIndexByCorrelation(True)
        mat.CreateDataMatrix()

        'for CV
        mat.CreateSplitDataMatrix(10)

        'Do regression
        Dim r = New LibRegressionDotNET.Regression.clsLinearRegression()
        For Each vSelection In [Enum].GetValues(GetType(clsLinearRegression.EnumVariableSelection))
            r.TrainDataMatrix = mat.GetTrainDataMatrix()
            r.TrainDataFields = mat.GetTrainFieldNames
            r.CorrectDataVector = mat.GetCorrectDataVector()
            r.CorrectDataField = mat.GetCorrectFieldName
            r.VariableSelection = vSelection
            r.DoRegression(True)
            r.OutputRegressionResult()
        Next
        Console.ReadLine()
        'issue
        'correlation
        'hogehoge.AIC, hogehoge.BIC
    End Sub
End Module
