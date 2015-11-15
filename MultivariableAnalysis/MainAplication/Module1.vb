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
        mat.TargetIndex = 13 '13
        mat.CreateDataMatrix()
        mat.CheckCorrelation()

        'create data matrix without higher correlation variable

        'Do regression
        Dim r = New LibRegressionDotNET.Regression.clsLinearRegression()
        r.TrainDataMatrix = mat.GetTrainDataMatrix()
        r.TrainDataFields = Nothing
        r.CorrectDataVector = mat.GetCorrectDataVector()
        r.CorrectDataField = String.Empty
        r.DoRegression()
        r.OutputRegressionResult()
    End Sub
End Module
