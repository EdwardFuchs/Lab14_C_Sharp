using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab14
{
    public static class SyntaxTree
    {
        static string[] unaryOperators = new string[] { "'" };
        static string[] binaryOperators = new string[] { "→", "v", "^" };

        //преобразование обратной польской записи в синтаксическое дерево
        public static Node ToTree(List<string> bpn)
        {
            var stack = new Stack<Node>();

            foreach (var lexem in bpn)
            {
                if (binaryOperators.Contains(lexem))
                {
                    //бинарный оператор
                    var arg2 = stack.Pop();
                    var arg1 = stack.Pop();
                    var node = new Node(lexem, arg1, arg2);
                    stack.Push(node);
                }
                else if (unaryOperators.Contains(lexem))
                {
                    //унарный оператор
                    var arg = stack.Pop();
                    var node = new Node(lexem, arg);
                    stack.Push(node);
                }
                else
                {
                    //аругмент
                    stack.Push(new Node(lexem));
                }
            }

            return stack.Pop();
        }

        //преобразования импликации
        public static Node EliminateImplication(Node node)
        {
            for (int i = 0; i < node.Count; i++)
                node[i] = EliminateImplication(node[i]);

            if (node.Operator == "→")
                return new Node("v", new Node("'", node[0]), node[1]);
            else
                return node;
        }

        //проброс инверсии на нижние уровни
        public static Node PushInversion(Node node)
        {
            if (node.Operator == "'" && node[0].Operator == "v")
                return new Node("^", PushInversion(new Node("'", node[0][0])), PushInversion(new Node("'", node[0][1])));

            if (node.Operator == "'" && node[0].Operator == "^")
                return new Node("v", PushInversion(new Node("'", node[0][0])), PushInversion(new Node("'", node[0][1])));

            if (node.Operator == "'" && node[0].Operator == "'")
                return PushInversion(node[0][0]);

            if (node.Operator == "'" && node[0].Count == 0)
                return new Node(node[0].Operator + "'");//(X)' => X'

            for (int i = 0; i < node.Count; i++)
                node[i] = PushInversion(node[i]);

            return node;
        }

        //проброс конъюнкции на нижние уровни (дистирибутивный закон A^(BvC)=A^BvA^C)
        public static Node DistributiveRule(Node node)
        {
            if (node.Operator == "^")
            {
                var arg1 = DistributiveRule(node[0]);
                var arg2 = DistributiveRule(node[1]);

                var res = new Node("v");
                foreach (var n in GetNodeOrChild(arg1))
                    foreach (var m in GetNodeOrChild(arg2))
                        res.Add(new Node("^", n, m));

                if (res.Count == 1)
                    res = res[0];

                return res;
            }

            for (int i = 0; i < node.Count; i++)
                node[i] = DistributiveRule(node[i]);

            return node;
        }

        static IEnumerable<Node> GetNodeOrChild(Node node)
        {
            if (node.Operator == "v")
                foreach (var n in node)
                    yield return n;
            else
                yield return node;
        }

        //преобразование дерева в строковое выражение (без скобок)
        public static string ToStringNoBrackets(Node node)
        {
            if (node.Count == 0)
                return node.Operator;//аргумент

            return string.Join(node.Operator, node.Select(n => ToStringNoBrackets(n)).ToArray());
        }

        //преобразование дерева в строковое выражение
        public static string ToString(Node node)
        {
            if (node.Count == 0)
                return node.Operator;//аргумент
            if (node.Operator == "'")
                return ToStringWithBrackets(node[0]) + "'";

            return string.Join(node.Operator, node.Select(n => ToStringWithBrackets(n)).ToArray()).Replace("→", "->");
        }

        static string ToStringWithBrackets(Node node)
        {
            if (node.Count < 2)
                return ToString(node);
            return "(" + ToString(node) + ")";
        }
    }
}
