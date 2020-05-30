using System;
using System.Collections.Generic;
using System.IO;

namespace Lab14
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Чтение файла");
            List<string> AllLinesTXT = new List<string>();
            try
            {
                var mas = File.ReadAllLines(@"C:\Users\Edward\source\repos\Lab14\input.txt");
                foreach (var el in mas)
                    AllLinesTXT.Add(el);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            foreach (var line in AllLinesTXT) {
                var sdnfAndSknf = Calculator.GetSDNFandSKNF(line, true);
                Console.WriteLine($"Функция: {line}");
                Console.WriteLine($"СДНФ = {sdnfAndSknf[0]}");
                Console.WriteLine($"СКНФ = {sdnfAndSknf[1]}");
                Console.WriteLine();
            }


        }

    }
}
