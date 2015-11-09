Module Module1
    Sub Main()
        'Read data set and create data matrix
        Dim mat = New LibRegressionDotNET.Regression.clsDataMatrixCtrl()
        mat.FilePath = "..\..\..\..\_dataset\housing_modify_org.csv"
        If mat.Read() = False Then
            Return
        End If
        mat.OutputDatasetProperty()
        mat.CreateDataMatrix(13)

        'Do regression
        Dim r = New LibRegressionDotNET.Regression.clsLinearRegression()
        r.TrainDataMatrix = mat.GetTrainDataMatrix()
        r.TrainDataFields = Nothing
        r.CorrectDataVector = mat.GetCorrectDataVector()
        r.CorrectDataField = String.Empty
        r.DoRegression()

    End Sub
End Module
