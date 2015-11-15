# Regression
回帰分析の実験

## MultivariableAnalysis

線形回帰（単回帰、重回帰）を行うためのライブラリ。作成中。

ToDo
* 変数選択（変数増加法、変数減少法、変数増減法）
* 交差検証によるモデル評価
* 正則化項を用いた回帰（リッジ回帰、LASSO回帰）

Done
* 相関行列
* 重相関係数、残差、AIC、BICの算出

## LASSOUsingHeuristic

I tried using the Heurisic optimization algorithm to estimate the LASSO parameters.

回帰分析の一つであるLassoをヒューリスティックな最適化アルゴリズムを用いて求めてみた。
縮小パラメータを大きくすると、回帰係数がどんどん小さくなっていく（変数選択）のを確認した。
係数の結果は要検証。

# Refference
1. Tibshirani, R. "Regression shrinkage and selection via the lasso.", Journal of the Royal Statistical Society, Series B, Vol 58, No. 1, pp. 267–288, 1996.

# Dataset
Datasets from "The Elements of Statistical Learning"
http://statweb.stanford.edu/~tibs/ElemStatLearn/
