using System.Text.RegularExpressions;

namespace Server;

public static class FileHelper
{
    public static void GenerateSmallFile(string inputPath, string outputPath)
    {
        StreamReader sr = new(inputPath);
        StreamWriter sw = new(outputPath);

        var line = sr.ReadLine();
        var regex = new Regex(@"\s+");
        var count = 0;

        while (line != null)
        {
            var res = regex.Replace(line, @" ");
            res = res.Replace("0,0000000", "0");
            sw.WriteLine(res);

            line = sr.ReadLine();
            count++;
        }

        Console.WriteLine(count);

        sr.Close();
        sw.Close();
    }

    public static void GeneratePartialFile(string inpPath)
    {
        StreamReader sr = new(inpPath);
        StreamWriter sw1 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part1.xml");
        StreamWriter sw2 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part2.xml");
        StreamWriter sw3 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part3.xml");
        StreamWriter sw4 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part4.xml");
        StreamWriter sw5 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part5.xml");
        StreamWriter sw6 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part6.xml");
        StreamWriter sw7 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part7.xml");
        StreamWriter sw8 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part8.xml");
        StreamWriter sw9 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part9.xml");
        //StreamWriter sw10 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part10.xml");
        //StreamWriter sw11 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part11.xml");
        //StreamWriter sw12 = new("C:\\D\\univ\\labs4\\PSP\\kursach\\myTests\\part12.xml");

        var line = sr.ReadLine();
        var count = 0;

        while (line != null)
        {
            if (count < 3400)
            {
                sw1.WriteLine(line);
            }
            else if (count >= 3400 && count < 6800)
            {
                sw2.WriteLine(line);
            }
            else if (count >= 6800 && count < 10200)
            {
                sw3.WriteLine(line);
            }
            else if (count >= 10200 && count < 13600)
            {
                sw4.WriteLine(line);
            }
            else if (count >= 13600 && count < 17000)
            {
                sw5.WriteLine(line);
            }
            else if (count >= 17000 && count < 20400)
            {
                sw6.WriteLine(line);
            }
            else if (count >= 20400 && count < 23800)
            {
                sw7.WriteLine(line);
            }
            else if (count >= 23800 && count < 27200)
            {
                sw8.WriteLine(line);
            }
            else
            {
                sw9.WriteLine(line);
            }

            line = sr.ReadLine();
            count++;
        }

        Console.WriteLine(count);

        sr.Close();
        sw1.Close();
        sw2.Close();
        sw3.Close();
        sw4.Close();
        sw5.Close();
        sw6.Close();
    }
}
