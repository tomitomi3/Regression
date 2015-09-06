Imports LibOptimization
Imports LibOptimization.Util
Imports LibOptimization.Optimization

Module Module1

    ''' <summary>
    ''' Estimate Lasso parameters using Heurisic optimization algorithm
    ''' </summary>
    ''' <remarks></remarks>
    Sub Main()
        'init
        Dim objectiveFunction = New clsLasso()
        If objectiveFunction.Init("..\..\..\_sampledata\prostate_modify_traindataset.csv") = False Then
            Debug.Assert(False)
            Return
        End If

        'Minimize objective function
        Dim opt As New LibOptimization.Optimization.clsOptDE(objectiveFunction)
        opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_best_2_bin
        Dim shrinkageParameters() As Double = {0, 0.01, 0.05, 0.1, 0.5, 1, 2, 4, 8, 16, 32, 64, 100, 150, 200, 400}
        For Each param In shrinkageParameters
            'Shrinkage Parameter
            objectiveFunction.ShrinkageParameter = param

            'do optimization
            opt.Init()
            opt.DoIteration()
            Console.WriteLine("Shrinkage Parameter:{0}", param)
            LibOptimization.Util.clsUtil.DebugValue(opt)

            'Evaluate
            If objectiveFunction.Evaluate("..\..\..\_sampledata\prostate_modify_testdataset.csv", opt.Result.ToArray, True) = False Then
                Return
            End If
        Next

        Console.ReadLine()
    End Sub
End Module
