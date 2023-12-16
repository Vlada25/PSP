using Shared.Models;
using System.Text;
using System.Xml;

namespace Server;

public static class XmlHelper
{
    public static LinearSystem ParseToLinearSystem(IFormFile file)
    {
        LinearSystem linearSystem = new()
        {
            Coefficients = new List<List<double>>(),
            Constants = new List<double>()
        };

        try
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;

                using StreamReader sr = new(stream);

                var line = sr.ReadLine();
                bool afterCoeffs = false;
                bool afterConstants = false;
                int coefsIndex = 0;

                while (line is not null)
                {
                    if (line.Contains("</constants>"))
                    {
                        break;
                    }

                    if (afterCoeffs && !line.Contains("</coefficients>") && !line.Contains("<constants>"))
                    {
                        linearSystem.Coefficients.Add(new List<double>());
                        linearSystem.Coefficients[coefsIndex].AddRange(line.TrimStart().TrimEnd().Split(" ").Select(c => double.Parse(c)));
                        coefsIndex++;
                    }

                    if (afterConstants)
                    {
                        linearSystem.Coefficients[coefsIndex].Add(double.Parse(line.TrimStart().TrimEnd()));
                        coefsIndex++;
                    }

                    if (line.Contains("<coefficients>"))
                    {
                        afterCoeffs = true;
                    }

                    if (line.Contains("<constants>"))
                    {
                        afterCoeffs = false;
                        afterConstants = true;
                        coefsIndex = 0;
                    }

                    line = sr.ReadLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }

        return linearSystem;
    }

    public static LinearSystem? ParseToLinearSystem(MemoryStream stream)
    {
        string[]? coeffs = null;
        string[]? constants = null;
        LinearSystem linearSystem = new();

        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);

            XmlElement? xRoot = xmlDoc.DocumentElement;

            if (xRoot is not null)
            {
                foreach (XmlElement xnode in xRoot)
                {
                    if (xnode.Name == "coefficients")
                    {
                        coeffs = xnode.InnerText.Replace("\r\n", "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    }

                    if (xnode.Name == "constants")
                    {
                        constants = xnode.InnerText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    }
                }
            }

            linearSystem.Coefficients = new List<List<double>>();
            linearSystem.Constants = constants.Select(c => double.Parse(c)).ToList();

            int index = 0;
            for (int i = 0; i < constants.Length; i++)
            {
                linearSystem.Coefficients.Add(new List<double>());
                for (int j = 0; j < constants.Length; j++)
                {
                    linearSystem.Coefficients[i].Add(double.Parse(coeffs[index]));
                    index++;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }

        return linearSystem;
    }

    public static string GenerateXmlString(LinearSystem linearSystem)
    {
        XmlDocument xmlDoc = new();
        XmlElement root = xmlDoc.CreateElement("system");
        xmlDoc.AppendChild(root);

        XmlElement coeffsElement = xmlDoc.CreateElement("coefficients");
        StringBuilder sb = new();
        sb.Append(" \n");

        for (int i = 0; i < linearSystem.Constants.Count; i++)
        {
            linearSystem.Coefficients.Add(new List<double>());
            for (int j = 0; j < linearSystem.Constants.Count; j++)
            {
                sb.Append($"{linearSystem.Coefficients[i][j]} ");
            }
            sb.Append(" \n");
        }

        coeffsElement.InnerText = sb.ToString();
        root.AppendChild(coeffsElement);

        XmlElement constElement = xmlDoc.CreateElement("constants");
        constElement.InnerText = "\n" + string.Join(" \n", linearSystem.Constants) + "\n";
        root.AppendChild(constElement);

        StringWriter stringWriter = new();
        XmlTextWriter xmlTextWriter = new(stringWriter);
        xmlDoc.WriteTo(xmlTextWriter);

        string xmlString = stringWriter.ToString();

        return xmlString;
    }
}
