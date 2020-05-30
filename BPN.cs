using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab14
{
    public static class BPN
    {
        static string[] lexemes = new string[] { ")", "(", "→", "v", "^", "'" };

        static int GetPrior(string lexem)
        {
            var res = Array.IndexOf(lexemes, lexem);
            if (res < 0) return 100;
            return res;
        }

        //получение списка лексем
        public static List<string> ToLexems(string str)
        {
            str = str.Replace("->", "→");
            return str.Select(c => c.ToString()).ToList();
        }

        //получение обратной польской записи
        public static List<string> ToBPN(List<string> lexList)
        {
            var bpnList = new List<string>();
            var stack = new Stack<string>();

            //перебираем входную строку
            for (int i = 0; i < lexList.Count; i++)
            {
                var lex = lexList[i];
                //если это операция, то получаем ее индекс
                var prior = GetPrior(lex);
                //операнды просто переписываются  в выходную строку
                if (prior == 100)
                {
                    bpnList.Add(lex);
                    continue;
                }

                //
                if (stack.Count == 0)
                    stack.Push(lex);//если стек пуст, то кладем операцию в стек
                else
                if (lex == "(")
                    stack.Push(lex);//открывающая скобка проталкивается в стек
                else
                {
                    //выталкиваем все более или равно-приоритетные операции
                    while (stack.Count > 0 && GetPrior(stack.Peek()) >= prior)
                    {
                        var l = stack.Pop();
                        if (lex == ")" && l == "(")
                            break;
                        bpnList.Add(l);
                    }
                    //помещаем в стек
                    if (lex != ")")
                        stack.Push(lex);
                }
            }
            //выталкиваем оставшиеся операции
            while (stack.Count > 0)
                bpnList.Add(stack.Pop());

            return bpnList;
        }
    }
}
