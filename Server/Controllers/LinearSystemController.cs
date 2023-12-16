using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Server.Helpers;
using Shared.Models;

namespace Server.Controllers;

[Route("api/linear-system")]
[ApiController]
[EnableCors("CorsPolicy")]
public class LinearSystemController : ControllerBase
{
    [HttpPost("solve")]
    public IActionResult SolveLinearSystem(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest();
        }

        var linearSystem = XmlHelper.ParseToLinearSystem(file);

        if (linearSystem is null)
        {
            return BadRequest();
        }

        var res1 = LinearEquationSolver.SolveUsingGauss(linearSystem);
        string resGauss = string.Join("; ", res1.Select(x => x.ToString()));

        string resCholesky = string.Empty;
        try
        {
            var res2 = LinearEquationSolver.SolveUsingCholesky(linearSystem);
            resCholesky = string.Join("; ", res2.Select(x => x.ToString()));
        }
        catch (Exception ex)
        {
            resCholesky = ex.Message;
        }
        
        return Ok(new LinearSystemResult() 
        { 
            Coefficients = linearSystem.Coefficients,
            Constants = linearSystem.Constants,
            GaussResult = resGauss, 
            CholeskyResult = resCholesky 
        });
    }

    [HttpPost("solve/gauss")]
    [RequestSizeLimit(long.MaxValue)]
    public IActionResult SolveUsingGauss(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest();
        }

        var linearSystem = XmlHelper.ParseToLinearSystem(file);

        if (linearSystem is null)
        {
            return BadRequest();
        }

        var res = LinearEquationSolver.SolveUsingGauss(linearSystem);
        string resGauss = string.Join("; ", res.Select(x => x.ToString()));

        return Ok(resGauss);
    }

    [HttpPost("solve/cholesky")]
    [RequestSizeLimit(long.MaxValue)]
    public IActionResult SolveUsingCholesky(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest();
        }

        var linearSystem = XmlHelper.ParseToLinearSystem(file);

        if (linearSystem is null)
        {
            return BadRequest();
        }

        string resCholesky = string.Empty;
        try
        {
            var res = LinearEquationSolver.SolveUsingCholesky(linearSystem);
            resCholesky = string.Join("; ", res.Select(x => x.ToString()));
        }
        catch (Exception ex)
        {
            resCholesky = ex.Message;
        }

        return Ok(resCholesky);
    }

    [HttpPost("solve/cholesky/from-path")]
    [RequestSizeLimit(long.MaxValue)]
    public IActionResult SolveUsingCholeskyFromPath([FromQuery] string filename)
    {
        var filepath = $"./data/{filename}.xml";

        using FileStream fileStream = new(filepath, FileMode.Open);
        MemoryStream memoryStream = new();

        fileStream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        var linearSystem = XmlHelper.ParseToLinearSystem(memoryStream);

        if (linearSystem is null)
        {
            return BadRequest();
        }

        string resCholesky = string.Empty;
        try
        {
            var res = LinearEquationSolver.SolveUsingCholesky(linearSystem);
            resCholesky = string.Join("; ", res.Select(x => x.ToString()));
        }
        catch (Exception ex)
        {
            resCholesky = ex.Message;
        }

        return Ok(resCholesky);
    }

    [HttpPost("file")]
    [RequestSizeLimit(long.MaxValue)]
    public IActionResult CreateFile([FromQuery] string filename, IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest();
        }

        var filepath = $"./data/{filename}.xml";
        string data = string.Empty;

        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            stream.Position = 0;

            using StreamReader reader = new(stream);
            data = reader.ReadToEnd();
        }

        if (!System.IO.File.Exists(filepath))
        {
            using StreamWriter sw = System.IO.File.CreateText(filepath);
            sw.WriteLine(data);
        }
        else
        {
            using StreamWriter sw = System.IO.File.AppendText(filepath);
            sw.WriteLine(data);
        }

        return Ok();
    }

    [HttpGet("random")]
    public IActionResult GetRandomMatrix([FromQuery] int size)
    {
        List<List<double>> coefficients = MatrixGenerator.GenerateMatrix(size);
        List<double> constants = MatrixGenerator.GenerateVector(size);

        return Ok(XmlHelper.GenerateXmlString(new LinearSystem() 
        { 
            Coefficients = coefficients, 
            Constants = constants
        }));
    }

    [HttpGet("random/positive")]
    public IActionResult GetRandomPositiveMatrix([FromQuery] int size)
    {
        List<List<double>> coefficients = MatrixGenerator.GeneratePositiveDefiniteMatrix(size);
        List<double> constants = MatrixGenerator.GenerateVector(size);

        return Ok(XmlHelper.GenerateXmlString(new LinearSystem()
        {
            Coefficients = coefficients,
            Constants = constants
        }));
    }
}
