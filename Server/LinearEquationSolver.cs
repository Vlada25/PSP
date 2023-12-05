using Shared.Models;

namespace Server;

public static class LinearEquationSolver
{
    public static double[] SolveUsingGauss(LinearSystem linearSystem)
    {
        int size = linearSystem.Constants.Count();
        var coefficients = GetCoeffs(linearSystem, size);

        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                double ratio = coefficients[j][i] / coefficients[i][i];

                for (int k = 0; k <= size; k++)
                {
                    coefficients[j][k] -= ratio * coefficients[i][k];
                }
            }
        }

        double[] result = new double[size];
        for (int i = size - 1; i >= 0; i--)
        {
            result[i] = coefficients[i][size] / coefficients[i][i];

            for (int j = i - 1; j >= 0; j--)
            {
                coefficients[j][size] -= coefficients[j][i] * result[i];
            }
        }

        return result;
    }

    public static double[] SolveUsingCholesky(LinearSystem linearSystem)
    {
        int size = linearSystem.Constants.Count();
        var coefficients = GetCoeffs(linearSystem, size);
        double[,] lowerTriangular = CholeskyDecomposition(linearSystem.Coefficients, size);

        double[] y = new double[size];
        for (int i = 0; i < size; i++)
        {
            double sum = 0;
            for (int j = 0; j < i; j++)
            {
                sum += lowerTriangular[i, j] * y[j];
            }
            y[i] = (coefficients[i][size] - sum) / lowerTriangular[i, i];
        }

        double[] result = new double[size];
        for (int i = size - 1; i >= 0; i--)
        {
            double sum = 0;
            for (int j = i + 1; j < size; j++)
            {
                sum += lowerTriangular[j, i] * result[j];
            }
            result[i] = (y[i] - sum) / lowerTriangular[i, i];
        }

        return result;
    }

    private static double[,] CholeskyDecomposition(List<List<double>> matrix, int size)
    {
        double[,] lowerTriangular = new double[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                double sum = 0;

                if (j == i)
                {
                    for (int k = 0; k < j; k++)
                    {
                        sum += lowerTriangular[j, k] * lowerTriangular[j, k];
                    }

                    double temp = matrix[j][j] - sum;
                    if (temp < 0)
                    {
                        throw new Exception("The matrix is not positive definite.");
                    }

                    lowerTriangular[j, j] = Math.Sqrt(matrix[j][j] - sum);
                }
                else
                {
                    for (int k = 0; k < j; k++)
                    {
                        sum += lowerTriangular[i, k] * lowerTriangular[j, k];
                    }

                    lowerTriangular[i, j] = (matrix[i][j] - sum) / lowerTriangular[j, j];
                }
            }
        }

        return lowerTriangular;
    }

    private static List<List<double>> GetCoeffs(LinearSystem linearSystem, int size)
    {
        List<List<double>> coefficients = new();

        for (int i = 0; i < size; i++)
        {
            coefficients.Add(new List<double>());

            for (int j = 0; j < size; j++)
            {
                coefficients[i].Add(linearSystem.Coefficients[i][j]);
            }

            coefficients[i].Add(linearSystem.Constants[i]);
        }

        return coefficients;
    }
}
