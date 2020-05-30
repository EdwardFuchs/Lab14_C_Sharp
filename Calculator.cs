using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab14
{
    class Calculator
    {
        static StringBuilder sb = new StringBuilder();
        private static string ToDNF(string x, bool trace = false) {
            //получаем список лексем
            var lexems = BPN.ToLexems(x);
            //преобразуем в обратную польскую запись
            var bpn = BPN.ToBPN(lexems);
            //преобразуем в синтаксическое дерево
            var tree = SyntaxTree.ToTree(bpn);
            if (trace) Console.WriteLine(SyntaxTree.ToString(tree));
            //устраняем импликацию по формуле A->B=A'VB
            var lastTree = tree;
            tree = SyntaxTree.EliminateImplication(tree);
            if (trace)
            {
                sb.Append("Устраняем импликацию"+ Environment.NewLine);
                sb.Append(SyntaxTree.ToString(tree)+ Environment.NewLine);
                Console.WriteLine("Устраняем импликацию");
                Console.WriteLine(SyntaxTree.ToString(tree));
            }
            //проносим отрицание на нижний уровень по формулам де Моргана, убираем двойное отрицание
            lastTree = tree;
            tree = SyntaxTree.PushInversion(tree);
            if (trace)
            {
                sb.Append("Проносим отрицание на нижний уровень по формулам де Моргана, убираем двойное отрицание" + Environment.NewLine);
                sb.Append(SyntaxTree.ToString(tree) + Environment.NewLine);
                Console.WriteLine("Проносим отрицание на нижний уровень по формулам де Моргана, убираем двойное отрицание");
                Console.WriteLine(SyntaxTree.ToString(tree));
            }
            //применяем диcтрибутивный закон для выражений типа A^(BvC)=A^BvA^C
            lastTree = tree;
            tree = SyntaxTree.DistributiveRule(tree);
            if (trace)
            {
                sb.Append("Применяем диcтрибутивный закон для выражений типа A^(BvC)=A^BvA^C" + Environment.NewLine);
                sb.Append(SyntaxTree.ToString(tree) + Environment.NewLine);
                Console.WriteLine("Применяем диcтрибутивный закон для выражений типа A^(BvC)=A^BvA^C");
                Console.WriteLine(SyntaxTree.ToString(tree));
            }
            if (trace)
            {
                sb.Append("ДНФ:" + Environment.NewLine);
                sb.Append(SyntaxTree.ToStringNoBrackets(tree) + Environment.NewLine);
                Console.WriteLine("ДНФ:");
                Console.WriteLine(SyntaxTree.ToStringNoBrackets(tree));
            }
            return SyntaxTree.ToStringNoBrackets(tree);
        }
        private static List<string> GetVars(string x) {
            var allVar = new List<string>();
            foreach (var mutliplic in x.Split('v'))
            {
                foreach (var variable in mutliplic.Split('^'))
                    if (!allVar.Contains(variable.Replace("'", "")))
                        allVar.Add(variable.Replace("'", ""));
            }
            return allVar;
        }
        private static Dictionary<long, int> Table(string x, List<string> allVar, bool trace = false) {
            var tableResults = new Dictionary<long, int>();
            if (trace)
            {
                sb.Append("Составим таблицу истинности" + Environment.NewLine);
                sb.Append(string.Join(" ", allVar) + " F" + Environment.NewLine);
                Console.WriteLine("Составим таблицу истинности");
                Console.WriteLine(string.Join(" ", allVar) + " F");
            }
            for (long i = 0; i < Math.Pow(2, allVar.Count); i++)

            {
                if (trace)
                    for (var j = 0; j< allVar.Count; j++)
                    {
                        if ((i & (1 << j)) != 0)
                        {
                            Console.Write("1 ");
                            sb.Append("1 ");
                        }
                        else
                        {
                            Console.Write("0 ");
                            sb.Append("0 ");
                        }
                    }
                //расчет результата
                var resultSum = 0;
                foreach (var mutliplic in x.Split('v'))
                {
                    var resultMult = 1;
                    foreach (var variable in mutliplic.Split('^'))
                    {
                        var getVar = (i & (1 << allVar.FindIndex(s => s == variable.Replace("'", "")))) != 0 ? 1:0;
                        if (variable.Contains('\'')){
                            getVar = getVar == 1 ? 0 : 1;
                        }
                        resultMult *= getVar;
                    }
                    resultSum += resultMult;
                }
                if (resultSum > 1)
                    resultSum = 1;
                if (trace)
                {
                    sb.Append(resultSum + Environment.NewLine);
                    Console.WriteLine(resultSum);
                }
                tableResults.Add(i, resultSum);
            }
            return tableResults;
        }
        private static string GetSDNF(Dictionary<long, int> table, List<string> allVar, bool trace = false) {
            var result = String.Empty;
            if (trace)
            {
                sb.Append("Для нахождения СДНФ находим строки, где у нас функция принимает значение 1" + Environment.NewLine);
                Console.WriteLine("Для нахождения СДНФ находим строки, где у нас функция принимает значение 1");
            }
            var i = 0;
            foreach (var str in table) {
                if (str.Value == 1) {
                    if (i != 0)
                        result += "v";
                    var j = 0;
                    result += "(";
                    for (var k = 0; k< allVar.Count; k++) {
                        if (j != 0)
                            result += "^";
                        result += (str.Key & (1 << k)) != 0 ? allVar[j] : allVar[j] + "'";
                        j++;
                    }
                    result += ")";
                    i++;
                }
            }
            if (trace)
            {
                sb.Append("Исходя из таблицы СДНФ имеет вид:" + Environment.NewLine);
                sb.Append(result + Environment.NewLine);
                Console.WriteLine("Исходя из таблицы СДНФ имеет вид:");
                Console.WriteLine(result);
            }
            return result;
        }
        private static string GetSKNF(Dictionary<long, int> table, List<string> allVar, bool trace = false)
        {
            var result = String.Empty;
            if (trace)
            {
                sb.Append("Для нахождения СКНФ находим строки, где у нас функция принимает значение 0" + Environment.NewLine);
                Console.WriteLine("Для нахождения СКНФ находим строки, где у нас функция принимает значение 0");
            }
            var i = 0;
            foreach (var str in table)
            {
                if (str.Value == 0)
                {
                    if (i != 0)
                        result += "^";
                    var j = 0;
                    result += "(";
                    for (var k = 0; k < allVar.Count; k++)
                    {
                        if (j != 0)
                            result += "v";
                        result += (str.Key & (1 << k)) != 0 ? allVar[j] + "'" : allVar[j];
                        j++;
                    }
                    result += ")";
                    i++;
                }
            }
            if (trace)
            {
                sb.Append("Исходя из таблицы СКНФ имеет вид:" + Environment.NewLine);
                sb.Append(result + Environment.NewLine);
                Console.WriteLine("Исходя из таблицы СКНФ имеет вид:");
                Console.WriteLine(result);
            }
            return result;
        }
        public static string[] GetSDNFandSKNF(string function, bool trace = false) {
            sb = new StringBuilder();
            sb.Append("==============================================" + Environment.NewLine);
            sb.Append($"Функция: {function}" + Environment.NewLine);
            Console.WriteLine("==============================================");
            Console.WriteLine($"Функция: {function}");
            var result = new string[2];
            var dnf = ToDNF(function, trace);
            if (trace)
            {
                Console.WriteLine();
                sb.Append(Environment.NewLine);
            }
            var allVar = GetVars(dnf);
            var table = Table(dnf, allVar, trace);
            if (trace)
            {
                Console.WriteLine();
                sb.Append(Environment.NewLine);
            }
            var sdnf = GetSDNF(table, allVar, trace);
            if (trace) { 
                Console.WriteLine();
                sb.Append(Environment.NewLine);
            }
            var sknf = GetSKNF(table, allVar, trace);
            if (trace) 
            { 
                Console.WriteLine();
                sb.Append(Environment.NewLine);
                sb.Append("==============================================" + Environment.NewLine);
                Console.WriteLine("==============================================");
                try
                {
                    using (StreamWriter sw = new StreamWriter(new FileStream(@"C:\Users\Edward\source\repos\Lab14\output.txt", FileMode.Append)))
                    {
                        sw.WriteLine(sb.ToString());
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (UnauthorizedAccessException e) {
                    Console.WriteLine(e.Message);
                }
            }
            result[0] = sdnf;
            result[1] = sknf;
            return result;
        }
    }
}
