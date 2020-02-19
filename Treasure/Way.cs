using System;
using System.Collections.Generic;
using System.Drawing;


namespace Treasure
{
    class Way
    {
        public static bool SearchWay(Point start, Point finish, int[,] map, ref List<Point> way)
        {
            if (start == finish) return true;

            int ax = start.X, ay = start.Y,
                bx = finish.X, by = finish.Y;

            int W = map.GetLength(0);
            int H = map.GetLength(1);

            const int WALL = -1;
            const int BLANK = -2;

            int[] px = new int[W * H];
            int[] py = new int[W * H];
            for (int j = 0; j < W * H; j++)
            {
                px[j] = py[j] = 0;
            }
            int len;
            int[,] grid = new int[W, H];

            for (int i = 0; i < W; i++)
                for (int j = 0; j < H; j++)
                    if (map[i, j] == 1 || map[i, j] == 3 || map[i, j] == 4 || map[i, j] == 2)
                        grid[i, j] = BLANK;
                    else grid[i, j] = WALL;


            int[] dy = { 1, 0, -1, 0 }; 
            int[] dx = { 0, 1, 0, -1 };   
            int d, x, y, k;
            bool stop;

            if (grid[ax,ay] == WALL || grid[bx,by] == WALL) return false;  

            d = 0;
            grid[ax,ay] = 0;          
            do 
            {
                stop = true;           
                for (x = 0; x<W; x++ )
                  for (y = 0; y<H; y++ )
                    if (grid[x,y] == d )                
                    {
                        for (k = 0; k< 4; ++k )                 
                        {
                            int iy = y + dy[k], ix = x + dx[k];
                            if (iy >= 0 && iy<H && ix >= 0 && ix<W && grid[ix,iy] == BLANK)
                            {
                                stop = false;            
                                grid[ix,iy] = d + 1;     
                            }
                      }
                    }
                d++;
            } while ( !stop && grid[bx,by] == BLANK);

            if (grid[bx,by] == BLANK) return false; 

            len = grid[bx,by]+1;            
            x = bx;
            y = by;
            d = len;
            while (d > 0 )
            {
                px[d] = x;
                py[d] = y;                  
                d--;
                for (k = 0; k< 4; ++k)
                {
                    int iy = y + dy[k], ix = x + dx[k];
                    if (iy >= 0 && iy<H && ix >= 0 && ix<W && grid[ix,iy] == d)
                    {
                        x = x + dx[k];
                        y = y + dy[k];           
                        break;
                    }
                }
            }
            px[0] = ax;
            py[0] = ay;                    

            for (int i=0;i<len;i++)
                way.Add(new Point(px[i], py[i]));

            return true;
        }
    }
}
