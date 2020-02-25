using System;
using System.Collections.Generic;
using System.Drawing;


namespace Treasure
{
    class Way
    {
        struct Node
        {
            public int x, y, 
                       parentX, parentY, 
                       gCost, hCost, fCost;
            public Node(int X, int Y, int ParentX, int ParentY, int GCost, int HCost)
            {
                this.x = X;
                this.y = Y;
                this.parentX = ParentX;
                this.parentY = ParentY;
                this.gCost = GCost;
                this.hCost = HCost;
                this.fCost = GCost + HCost;
            }
        }
        public static void SearchWay(int[,] Map, ref List<Point> Way, Point start, Point Treasure)
        {
            int W = Map.GetLength(0);
            int H = Map.GetLength(1);

            int[,] WhereWeCan = new int[W, H];

            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    if (Map[x, y] == 1 || Map[x, y] == 3)
                        WhereWeCan[x, y] = 0;
                    else
                        WhereWeCan[x, y] = 1;

            List<Node> ActivNode = new List<Node>();
            List<Node> DisActivNode = new List<Node>();
            Node Start = new Node(start.X, start.Y, -1, -1, 0, Math.Abs(Treasure.X - start.X) + Math.Abs(Treasure.Y - start.Y));
            ActivNode.Add(Start);
            Node MinNode;

            while (ActivNode.Count != 0)
            {
                MinNode = ActivNode[0];
                foreach (Node node in ActivNode)
                {
                    if (MinNode.fCost > node.fCost)
                    {
                        MinNode = node;
                    }
                }
                if ((MinNode.x + 1 == Treasure.X && MinNode.y == Treasure.Y) ||
                    (MinNode.x - 1 == Treasure.X && MinNode.y == Treasure.Y) ||
                    (MinNode.x == Treasure.X && MinNode.y + 1 == Treasure.Y) ||
                    (MinNode.x == Treasure.X && MinNode.y - 1 == Treasure.Y))
                {
                    ActivNode.Remove(MinNode);
                    DisActivNode.Add(MinNode);
                    Way = BuildWay(DisActivNode, MinNode);
                    return;
                }
                

                if (MinNode.x + 1 < W && WhereWeCan[MinNode.x + 1, MinNode.y] == 0)
                {
                    Node NewNode = new Node(MinNode.x + 1, MinNode.y, MinNode.x, MinNode.y, MinNode.gCost + 1, Math.Abs(Treasure.X - MinNode.x + 1) + Math.Abs(Treasure.Y - MinNode.y));
                    bool Search = GetSearch(ActivNode, DisActivNode, NewNode);

                    if (Search == false)
                        ActivNode.Add(NewNode);
                }
                if (MinNode.x - 1 > 0 && WhereWeCan[MinNode.x - 1, MinNode.y] == 0)
                {
                    Node NewNode = new Node(MinNode.x - 1, MinNode.y, MinNode.x, MinNode.y, MinNode.gCost + 1, Math.Abs(Treasure.X - MinNode.x - 1) + Math.Abs(Treasure.Y - MinNode.y));
                    bool Search = GetSearch(ActivNode, DisActivNode, NewNode);

                    if (Search == false)
                        ActivNode.Add(NewNode);
                }
                if (MinNode.y + 1 < H && WhereWeCan[MinNode.x, MinNode.y + 1] == 0)
                {
                    Node NewNode = new Node(MinNode.x, MinNode.y + 1, MinNode.x, MinNode.y, MinNode.gCost + 1, Math.Abs(Treasure.X - MinNode.x) + Math.Abs(Treasure.Y - MinNode.y + 1));
                    bool Search = GetSearch(ActivNode, DisActivNode, NewNode);

                    if (Search == false)
                        ActivNode.Add(NewNode);
                }
                if (MinNode.y - 1 > 0 && WhereWeCan[MinNode.x, MinNode.y - 1] == 0)
                {
                    Node NewNode = new Node(MinNode.x, MinNode.y - 1, MinNode.x, MinNode.y, MinNode.gCost + 1, Math.Abs(Treasure.X - MinNode.x) + Math.Abs(Treasure.Y - MinNode.y - 1));
                    bool Search = GetSearch(ActivNode, DisActivNode, NewNode);

                    if (Search == false)
                        ActivNode.Add(NewNode);
                }

                ActivNode.Remove(MinNode);
                DisActivNode.Add(MinNode);
            }
        }
        private static bool GetSearch(List<Node> ActivNode, List<Node> DisActivNode, Node NewNode)
        {
            bool Search = false;
            foreach (Node node in ActivNode)
            {
                if (node.x == NewNode.x && node.y == NewNode.y)
                {
                    Search = true;
                    break;
                }
            }
            foreach (Node node in DisActivNode)
            {
                if (node.x == NewNode.x && node.y == NewNode.y)
                {
                    Search = true;
                    break;
                }
            }
            return Search;
        }
        private static List<Point> BuildWay(List<Node> DisActivNode, Node MinNode)
        {
            List<Point> Way = new List<Point>();
            while (MinNode.parentX != -1 && MinNode.parentY != -1)
            {
                Point way = new Point(MinNode.x, MinNode.y);
                foreach (Node node in DisActivNode)
                {
                    if (node.x == MinNode.parentX && node.y == MinNode.parentY)
                    {
                        MinNode = node;
                        Way.Add(way);
                        break;
                    }
                }
            }
            return Way;
        }
    }
}
