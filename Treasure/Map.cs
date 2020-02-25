using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Treasure
{
    
    public class Map
    {
        private int widthMap;
        private int heightMap;
        private List<string> lines;

        private int[,] map;

        private List<Point> Base = new List<Point>();
        private List<Point> Bridge = new List<Point>();
        private List<Point> Treasure = new List<Point>();
        private List<List<Point>> Water = new List<List<Point>>();

        private const int WALL = 1;
        private const int BASE = 2;
        private const int BRIDGE = 3;
        private const int TREASURE = 4;
        private const int WATER = 5;

        private const int BORDER_ANGLE = 6;
        private const int BORDER_LR = 7;
        private const int BORDER_UD = 8;

        private const int WAY = 9;

        public Map(string nameMap)
        {
            try
            {
                lines = File.ReadAllLines(nameMap).ToList();

                List<string> _lines = new List<string>();

                foreach (string l in lines)
                    if (!String.IsNullOrEmpty(l))
                        _lines.Add(l);

                lines = _lines;

                LinesParsing();
                CreateMap();
                AddWay();

                Console.SetWindowSize(widthMap+1, heightMap+1);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

        private void LinesParsing()
        {
            foreach (string TypeObject in lines)
            {
                string[] SplitLine = TypeObject.Split('(');
                SplitLine[0] = SplitLine[0].ToLower().Replace(" ", "");
                SplitLine[1] = SplitLine[1].TrimEnd(')').Replace(" ", "");

  
                if ("base, bridge, treasure, water".IndexOf(SplitLine[0]) > -1)
                {
                    string[] Coordinates;

                    if (SplitLine[0] == "water")
                    {
                        SplitLine[1] = SplitLine[1].Replace("->","~");
                        Coordinates = SplitLine[1].Split('~');
                    }
                    else
                        Coordinates = SplitLine[1].Split(':');

                    string[] Coordinate;
                    List<Point> values = new List<Point>();

                    foreach (string el in Coordinates)
                    {
                        Coordinate = el.Split(',');
                        int x = 0;
                        int y = 0;
                        for (int i = 0; i < Coordinate.Count(); i++)
                        {
                            int val;
                            if (!Int32.TryParse(Coordinate[i], out val) || String.IsNullOrEmpty(Coordinate[i]))
                            {
                                Regex my_reg = new Regex(@"\D");
                                Coordinate[i] = my_reg.Replace(Coordinate[i], "");
                                if (!Int32.TryParse(Coordinate[i], out val) || String.IsNullOrEmpty(Coordinate[i]))
                                    throw new Exception($"Value '{Coordinate[i]}' in the file is not a number");

                            }
                            if (i == 0 && val > widthMap)
                                widthMap = val;
                            else if (i == 1 && val > heightMap)
                                heightMap = val;

                            if (i == 0) x = val;
                            else y = val;
                        }
                        values.Add(new Point(x, y));
                    }

                    if (SplitLine[0] == "base")
                        foreach (var el in values)
                            Base.Add(el);
                    else if (SplitLine[0] == "bridge")
                        Bridge.AddRange(values);
                    else if (SplitLine[0] == "treasure")
                        Treasure.Add(values[0]);
                    else
                        Water.Add(values);
                }
            }

            widthMap++;
            heightMap++;

            CheckBorderWater();

            map = new int[widthMap, heightMap];
            for (int x = 0; x < widthMap; x++)
                for (int y = 0; y < heightMap; y++)
                    map[x,y] = WALL;
        }

        private void CreateMap()
        {
            AddBorder();

            foreach (List<Point> wat in Water)
                for (int i = 1; i < wat.Count(); i++)
                    CreateWater(wat[i - 1], wat[i]);

            for (int i = 0; i < Base.Count(); i+=2)
                for (int x = Base[i].X; x <= Base[i+1].X; x++)
                    for (int y = Base[i].Y; y <= Base[i+1].Y; y++)
                        map[x, y] = BASE;

            foreach (Point brid in Bridge)
                map[brid.X, brid.Y] = BRIDGE;

            foreach (Point treas in Treasure)
                map[treas.X, treas.Y] = TREASURE;

        }

        private void AddBorder()
        {
            widthMap += 2;
            heightMap += 2;
            int[,] newMap = new int[widthMap, heightMap];

            for (int x = 0; x < widthMap; x++)
                for (int y = 0; y < heightMap; y++)
                {
                    if ((x == 0 && y == 0) || (x == 0 && y == heightMap - 1) || (x == widthMap - 1 && y == 0) || x == widthMap - 1 && y == heightMap - 1)
                        newMap[x, y] = BORDER_ANGLE;
                    else if (y == 0 || y == heightMap - 1)
                        newMap[x, y] = BORDER_UD;
                    else if (x == 0 || x == widthMap - 1)
                        newMap[x, y] = BORDER_LR;
                    else
                        newMap[x, y] = map[x - 1, y - 1];
                }
            map = newMap;

            for (int i = 0; i < Base.Count(); i++)
                Base[i] = new Point(Base[i].X + 1, Base[i].Y + 1);

            for (int i = 0; i < Bridge.Count(); i++)
                Bridge[i] = new Point(Bridge[i].X + 1, Bridge[i].Y + 1);

            for (int i = 0; i < Treasure.Count(); i++)
                Treasure[i] = new Point(Treasure[i].X + 1, Treasure[i].Y + 1);

            for (int i = 0; i < Water.Count(); i++)
                for (int j = 0; j < Water[i].Count(); j++)
                    Water[i][j] = new Point(Water[i][j].X + 1, Water[i][j].Y + 1);
        }

        private void CreateWater(Point start, Point end)
        {
            int x0 = start.X;
            int x1 = end.X;
            int y0 = start.Y;
            int y1 = end.Y;

            int dx = (x1 > x0) ? (x1 - x0) : (x0 - x1);
            int dy = (y1 > y0) ? (y1 - y0) : (y0 - y1);

            int sx = (x1 >= x0) ? (1) : (-1);
            int sy = (y1 >= y0) ? (1) : (-1);

            if (dy < dx)
            {
                int d = (dy << 1) - dx;
                int d1 = dy << 1;
                int d2 = (dy - dx) << 1;

                map[x0, y0] = WATER;

                int x = x0 + sx;
                int y = y0;
                for (int i = 1; i <= dx; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        y += sy;
                    }
                    else
                        d += d1;

                    map[x, y] = WATER;

                    x += sx;
                }
            }
            else
            {
                int d = (dx << 1) - dy;
                int d1 = dx << 1;
                int d2 = (dx - dy) << 1;

                map[x0, y0] = WATER;

                int x = x0;
                int y = y0 + sy;
                for (int i = 1; i <= dy; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        x += sx;
                    }
                    else
                        d += d1;

                    map[x, y] = WATER;

                    y += sy;
                }
            }
        }

        public void PrintMap()
        {
            try
            {
                for (int y = 0; y < heightMap; y++)
                {
                    for (int x = 0; x < widthMap; x++)
                    {
                        string str = "";
                        switch (map[x, y])
                        {

                            case WALL:
                                str = " ";
                                break;
                            case BASE:
                                str = "@";
                                break;
                            case BRIDGE:
                            case BORDER_ANGLE:
                                str = "#";
                                break;
                            case TREASURE:
                                str = "+";
                                break;
                            case WATER:
                                str = "~";
                                break;
                            case BORDER_LR:
                                str = "|";
                                break;
                            case BORDER_UD:
                                str = "—";
                                break;
                            case WAY:
                                str = "%";
                                break;
                            default: break;

                        }
                        Console.Write(str);
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void AddWay()
        {
            List<List<Point>> all_way = new List<List<Point>>();
            List<Point> way = new List<Point>();
            List<Point> start = new List<Point>();


            if ((Base[0].Y - Base[1].Y) % 2 == 0)
            {
                start.Add(new Point(Base[0].X, Base[0].Y + (Base[1].Y - Base[0].Y) / 2));
                start.Add(new Point(Base[1].X, Base[0].Y + (Base[1].Y - Base[0].Y) / 2));
            }
            else
            {
                start.Add(new Point(Base[0].X, Base[0].Y + (Base[1].Y - Base[0].Y) / 2));
                start.Add(new Point(Base[0].X, Base[0].Y + (Base[1].Y - Base[0].Y) / 2 + 1));
                start.Add(new Point(Base[1].X, Base[0].Y + (Base[1].Y - Base[0].Y) / 2));
                start.Add(new Point(Base[1].X , Base[0].Y + (Base[1].Y - Base[0].Y) / 2 + 1));
            }

            if ((Base[0].X - Base[1].X) % 2 == 0)
            {
                start.Add(new Point(Base[0].X + (Base[1].X - Base[0].X) / 2 , Base[0].Y));
                start.Add(new Point(Base[0].X + (Base[1].X - Base[0].X) / 2 , Base[1].Y));
            }
            else
            {
                start.Add(new Point(Base[0].X + (Base[1].X - Base[0].X) / 2, Base[0].Y));
                start.Add(new Point(Base[0].X + (Base[1].X - Base[0].X) / 2 + 1, Base[0].Y));
                start.Add(new Point(Base[0].X + (Base[1].X - Base[0].X) / 2, Base[1].Y));
                start.Add(new Point(Base[0].X + (Base[1].X - Base[0].X) / 2 + 1, Base[1].Y));
            }

            /*            foreach (Point finish in Treasure)
                        {*/
            var rand = new Random();
            Point finish = Treasure[rand.Next(0, Treasure.Count())];

            foreach (Point el in start)
            {
                if (way.Count() == 0)
                    Way.SearchWay(map, ref way, el, finish);
                else
                {
                    List<Point> _way = new List<Point>();
                    Way.SearchWay(map, ref _way, el, finish);
                    if (_way.Count() < way.Count())
                        way = _way;
                    else if (_way.Count() == way.Count())
                    {
                        //var rand = new Random();
                        if (rand.Next(0, 2) == 1)
                            way = _way;
                    }
                }
            }
            all_way.Add(way);
//            }
            if (way.Count() == 0)
                throw new Exception("Failed to build route");
            foreach (List<Point> w in all_way)
                foreach (Point p in w)
                    if (map[p.X, p.Y] == WALL || map[p.X, p.Y] == BASE)
                        map[p.X, p.Y] = WAY;
        }

        private void CheckBorderWater()
        {
            int x1, x2, x3;
            int y1, y2, y3;

            for (int i = 0; i < Water.Count(); i++)
                for (int j = 1; j < Water[i].Count() - 1; j++)
                {
                    x1 = Water[i][j - 1].X;
                    y1 = Water[i][j - 1].Y;
                    x2 = Water[i][j].X;
                    y2 = Water[i][j].Y;
                    x3 = Water[i][j + 1].X;
                    y3 = Water[i][j + 1].Y;

                    if ((x1 < x2) && (x2 > x1) && (x2 == widthMap - 1)
                        && (Water[i].First().X != widthMap - 1) 
                        && (Water[i].Last().X != widthMap -1))
                    {
                        widthMap++;
                    }
                    if ((x1 > x2) && (x2 < x1) && (x2 == 0)
                        && (Water[i].First().X != 0)
                        && (Water[i].Last().X != 0))
                    {
                        for (int k = 0; k < Base.Count(); k++)
                            Base[k] = new Point(Base[k].X + 1, Base[k].Y);

                        for (int k = 0; k < Bridge.Count(); k++)
                            Bridge[k] = new Point(Bridge[k].X + 1, Bridge[k].Y);

                        for (int k = 0; k < Treasure.Count(); k++)
                            Treasure[k] = new Point(Treasure[k].X + 1, Treasure[k].Y);

                        for (int k = 0; k < Water.Count(); k++)
                            for (int l = 0; l < Water[k].Count(); l++)
                                Water[k][l] = new Point(Water[k][l].X + 1, Water[k][l].Y);
                        widthMap++;
                    }

                    if ((y1 < y2) && (y2 > y1) && (y2 == heightMap - 1)
                        && (Water[i].First().Y != heightMap - 1)
                        && (Water[i].Last().Y != heightMap - 1))
                        heightMap++;

                    if ((y1 > y2) && (y2 < y1) && (y2 == 0)
                        && (Water[i].First().Y != 0)
                        && (Water[i].Last().Y != 0))
                    {
                        for (int k = 0; k < Base.Count(); k++)
                            Base[k] = new Point(Base[k].X, Base[k].Y + 1);

                        for (int k = 0; k < Bridge.Count(); k++)
                            Bridge[k] = new Point(Bridge[k].X, Bridge[k].Y + 1);

                        for (int k = 0; k < Treasure.Count(); k++)
                            Treasure[k] = new Point(Treasure[k].X, Treasure[k].Y + 1);

                        for (int k = 0; k < Water.Count(); k++)
                            for (int l = 0; l < Water[k].Count(); l++)
                                Water[k][l] = new Point(Water[k][l].X, Water[k][l].Y + 1);
                        heightMap++;
                    }
                }
        }
    }
}
