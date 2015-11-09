Module Module1
    Sub Main()
        Dim r = New LibRegressionDotNET.Regression.clsRegression()

        'read dataset
        If r.ReadDatasetByCsv("..\..\..\..\_dataset\housing_modify_org.csv") = False Then
            Console.WriteLine("Error")
            Return
        End If

        'output dataset property
        r.ConsoleDatasetProperty()

        'Set target variable index 目的変数
        r.TargetVariableIndex = 13

        'data check
        r.CheckData()

        'DoRegression
        r.IsCrossValidation = True
        r.KFoldCrossValidation = 10
        r.IsVariableSelection = False
        If r.DoRegression() = False Then
            Console.WriteLine("Error")
            Return
        End If

        'output 
        r.ConsoleRegressionResult()

    End Sub
End Module
