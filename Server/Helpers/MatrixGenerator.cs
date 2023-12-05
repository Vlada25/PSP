using System.Drawing;

namespace Server.Helpers;

public static class MatrixGenerator
{
    public static List<List<double>> GenerateMatrix(int size)
    {
        var random = new Random();
        var list = new List<List<double>>();

        for (int i = 0; i < size; i++)
        {
            list.Add(new List<double>());
            for (int j = 0; j < size; j++)
            {
                list[i].Add(random.Next(1, 10));
            }
        }

        return list;
    }

    public static List<List<double>> GeneratePositiveDefiniteMatrix(int size)
    {
        var random = new Random();
        var list = new List<List<double>>();

        for (int i = 0; i < size; i++)
        {
            list.Add(new List<double>());
            for (int j = 0; j < size; j++)
            {
                list[i].Add(i == j ? random.Next(1, 10) : 0);
            }
        }

        return list;
    }

    public static List<double> GenerateVector(int n)
    {
        Random random = new Random();
        double[] vector = new double[n];

        for (int i = 0; i < n; i++)
        {
            vector[i] = random.Next(1, 10);
        }

        return vector.ToList();
    }
}
