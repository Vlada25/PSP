namespace Shared.Models;

public class LinearSystemResult
{
    public List<List<double>> Coefficients { get; set; }
    public List<double> Constants { get; set; }
    public string GaussResult { get; set; }
    public string CholeskyResult { get; set; }
}
