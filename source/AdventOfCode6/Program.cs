using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode6
{
    class Program
    {
        public class GraphNode
        {
            public GraphNode Parent { get; private set; }
            public string Name { get; }
            public GraphNode(string name, GraphNode parent)
            {
                Name = name;
                if (parent != null) SetParent(parent);
            }

            public void SetParent(GraphNode parent)
            {
                if (Parent != null) throw new InvalidOperationException();
                Parent = parent;
            }

            public int CountAncestors()
            {
                return GetAncestors().Count();
            }

            public IEnumerable<GraphNode> GetAncestors()
            {
                var current = this;
                while (current.Parent != null)
                {
                    current = current.Parent;
                    yield return current;
                }
            }
        }

        public static string PrintPath(IEnumerable<GraphNode> path)
        {
            var reversed = path.Reverse().Select(gn => gn.Name);
            return string.Join(")", reversed);
        }

        static void Main(string[] args)
        {
            Dictionary<string, GraphNode> bodies = new Dictionary<string, GraphNode>();
            var lines = File.ReadAllLines("./input.txt");
            foreach(var line in lines)
            {
                var elements = line.Split(")");
                var parent = elements[0];
                var child = elements[1];
                if (!bodies.ContainsKey(parent))
                {
                    bodies.Add(parent, new GraphNode(parent, null));
                }

                if (bodies.ContainsKey(child))
                {
                    bodies[child].SetParent(bodies[parent]);
                }
                else
                {
                    bodies.Add(child, new GraphNode(child, bodies[parent]));
                }
            }

            Console.WriteLine($"Total orbits: {bodies.Values.Sum(b => b.CountAncestors())}");
            if (bodies.ContainsKey("YOU"))
            {
                var youPath = bodies["YOU"].GetAncestors().ToList();
                var santaPath = bodies["SAN"].GetAncestors().ToList();
                var firstCommonAncestor = santaPath.First(n => youPath.Contains(n));
                var numJumps = youPath.IndexOf(firstCommonAncestor) + santaPath.IndexOf(firstCommonAncestor);

                Console.WriteLine(PrintPath(youPath));
                Console.WriteLine(PrintPath(santaPath));
                Console.WriteLine($"Numer of jumps required: {numJumps}");
            }
            Console.ReadKey();
        }
    }
}
