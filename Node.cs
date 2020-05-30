using System;
using System.Collections.Generic;
using System.Text;

namespace Lab14
{
    public class Node : List<Node>
    {
        public string Operator;

        public Node(string op, params Node[] child)
        {
            this.Operator = op;
            AddRange(child);
        }

        public override string ToString()
        {
            return Operator;
        }

        public Node Clone()
        {
            var res = new Node(Operator);
            foreach (var child in this)
                res.Add(child.Clone());

            return res;
        }
    }
}
