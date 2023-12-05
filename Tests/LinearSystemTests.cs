using Server;
using Shared.Models;

namespace Tests;

public class LinearSystemTests
{
    LinearSystem _correctResults = new LinearSystem()
    {
        Coefficients = new()
            {
                new List<double>() { 2, -1, 0 },
                new List<double>() { -1, 2, -1 },
                new List<double>() { 0, -1, 2 }
            },
        Constants = new() { 1, 2, 3 },
    };

    LinearSystem _incorrectCholeskyResults = new LinearSystem()
    {
        Coefficients = new()
            {
                new List<double>() { 1, 2, 3 },
                new List<double>() { 3, 5, 7 },
                new List<double>() { 1, 3, 4 }
            },
        Constants = new() { 3, 0, 1 },
    };

    [Fact]
    public void SolveUsingGauss_CorrectData_ReturnSolutionVector()
    {
        var result = LinearEquationSolver.SolveUsingGauss(_correctResults);

        Assert.NotNull(result);
        Assert.Equal(result.Length, _correctResults.Constants.Count);
    }

    [Fact]
    public void SolveUsingCholesky_CorrectData_ReturnSolutionVector()
    {
        var result = LinearEquationSolver.SolveUsingCholesky(_correctResults);

        Assert.NotNull(result);
        Assert.Equal(result.Length, _correctResults.Constants.Count);
    }

    [Fact]
    public void CompareSolutions_CorrectData_ReturnSolutionVectors()
    {
        var resultGauss = LinearEquationSolver.SolveUsingGauss(_correctResults);
        var resultCholesky = LinearEquationSolver.SolveUsingCholesky(_correctResults);

        Assert.NotNull(resultGauss);
        Assert.NotNull(resultCholesky);
        Assert.Equal(resultGauss.Length, _correctResults.Constants.Count);
        Assert.Equal(resultCholesky.Length, _correctResults.Constants.Count);

        for (int i = 0; i < resultGauss.Length; i++)
        {
            Assert.True(resultGauss[i] - resultCholesky[i] < 0.5);
        }
    }

    [Fact]
    public void SolveUsingCholesky_IncorrectData_ThrowException()
    {
        Action act = () => LinearEquationSolver.SolveUsingCholesky(_incorrectCholeskyResults);

        Exception exception = Assert.Throws<Exception>(act);
        Assert.Equal("The matrix is not positive definite.", exception.Message);
    }

    [Fact]
    public void SolveUsingGauss_NoData_ThrowException()
    {
        Action act = () => LinearEquationSolver.SolveUsingGauss(new LinearSystem());

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("Value cannot be null. (Parameter 'source')", exception.Message);
    }

    [Fact]
    public void SolveUsingCholesky_NoData_ThrowException()
    {
        Action act = () => LinearEquationSolver.SolveUsingCholesky(new LinearSystem());

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("Value cannot be null. (Parameter 'source')", exception.Message);
    }
}