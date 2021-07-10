using System;
using System.Collections.Generic;

namespace ConsoleUI
{
    // The top of the tree is a node itself.
    // It has no parent, so you can walk up the tree to find the root.
    public class Tree : Node
    {
        public Tree(Rectangle rectangle)
        {
            // Value = rectangle;
        }
    }

    
    public class Node
    {
        public Node       Parent    { get; private set; }
        public List<Node> Children  { get; init;        } 

        public Layout     Layout    { get; set;         }
        public Rectangle  Rectangle { get; set;         }


        public int Depth
        {
            get
            {
                // Walk the tree upwards, the root node has no parent and is the end
                int depth = 0;
                var node  = this;
                while (node.Parent != null)
                {
                    node = node.Parent;
                    depth++;
                }

                return depth;
            }
        }


        public Node()
        {
            this.Children = new List<Node>();
        }

        public void Add(Rectangle value)
        {
            var node       = new Node();
            node.Parent    = this;
            node.Rectangle = value;
            Children.Add(node);
        }

        public void Remove()
        {
            Children.Clear();
        }

        public void Traverse()
        {
            Traverse(this);
        }

        public void Traverse(Node node)
        {
            // Depth first
            foreach (var child in node.Children)
            {
                Console.WriteLine(child.ToString());
                Traverse(child);
            }
        }

        public bool Split(float percentage = 50.0f, bool vertical = false)
        {
            // If this is more than 0, you're trying to split a Node that's 
            // already been split. This would create duplicate children.
            if (Children.Count == 0)
            {
                var split = this.Rectangle.Split(percentage, vertical);
                Add(split.Item1);
                Add(split.Item2);

                return true;
            }

            return false;
        }


        public override string ToString()
        {
            string spaces = string.Empty.PadRight((Depth - 1) * 4);
            return $"Depth: {Depth} -> {spaces}X: {Rectangle.X}, Y: {Rectangle.Y}, Width: {Rectangle.Width}, Height: {Rectangle.Height}";
        }  
    }
}

