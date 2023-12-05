using Shared.Models;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Server;

public static class XmlHelper
{
    public static async Task<LinearSystem> ParseToLinearSystem(IFormFile file)
    {
        string[]? coeffs = null;
        string[]? constants = null;
        LinearSystem linearSystem = new();
        List<List<string>> initialCoeffs = new();
        List<string> initialConstants = new();

        try
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;

                //using StreamReader sr = new(stream);

                //var line = sr.ReadLine();
                //bool afterCoeffs = false;
                //bool afterConstants = false;
                //int coefsIndex = 0;

                //while (line is not null)
                //{
                //    //sb.Append(line);
                //    if (line.Contains("</constants>"))
                //    {
                //        break;
                //    }

                //    if (afterCoeffs && !line.Contains("</coefficients>") && !line.Contains("<constants>"))
                //    {
                //        initialCoeffs.Add(new List<string>());
                //        initialCoeffs[coefsIndex].AddRange(line.TrimStart().TrimEnd().Split(" "));
                //        coefsIndex++;
                //    }

                //    if (afterConstants)
                //    {
                //        initialConstants.AddRange(line.TrimStart().TrimEnd().Split(" "));
                //    }

                //    if (line.Contains("<coefficients>"))
                //    {
                //        afterCoeffs = true;
                //    }

                //    if (line.Contains("<constants>"))
                //    {
                //        afterCoeffs = false;
                //        afterConstants = true;
                //    }

                //    line = sr.ReadLine();
                //}

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

            //linearSystem.Coefficients = new List<List<double>>();
            //linearSystem.Constants = initialConstants.Select(c => double.Parse(c)).ToList();

            //foreach (var item in initialCoeffs)
            //{
            //    linearSystem.Coefficients.Add(new List<double>(item.Select(c => double.Parse(c)).ToList()));
            //}
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
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root = xmlDoc.CreateElement("system");
        xmlDoc.AppendChild(root);

        XmlElement coeffsElement = xmlDoc.CreateElement("coefficients");
        StringBuilder sb = new();

        for (int i = 0; i < linearSystem.Constants.Count; i++)
        {
            linearSystem.Coefficients.Add(new List<double>());
            for (int j = 0; j < linearSystem.Constants.Count; j++)
            {
                sb.Append($"{linearSystem.Coefficients[i][j]} ");
            }
        }

        coeffsElement.InnerText = sb.ToString();
        root.AppendChild(coeffsElement);

        XmlElement constElement = xmlDoc.CreateElement("constants");
        constElement.InnerText = string.Join(' ', linearSystem.Constants);
        root.AppendChild(constElement);

        StringWriter stringWriter = new();
        XmlTextWriter xmlTextWriter = new(stringWriter);
        xmlDoc.WriteTo(xmlTextWriter);
        string xmlString = stringWriter.ToString();

        return xmlString;
    }
}
