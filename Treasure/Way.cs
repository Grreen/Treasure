using System;
using System.Collections.Generic;
using System.Drawing;


namespace Treasure
{
    class Way
    {
        public static bool SearchWay(Point start, Point finish, int[,] map, ref List<Point> way)
        {
            if (start == finish) return false;

            int ax = start.X, ay = start.Y,
                bx = finish.X, by = finish.Y;
            //    int ax = 1, ay = 1 , bx = 1, by = 5;
            int W = map.GetLength(0);//ширина поля
            int H = map.GetLength(1);//высота поля

            const int WALL = -1;
            const int BLANK = -2;

            //координаты ячеек, входящих в путь
            int[] px = new int[W * H];
            int[] py = new int[W * H];
            for (int j = 0; j < W * H; j++)
            {
                px[j] = py[j] = 0;
            }
            int len;//длина пути
            int[,] grid = new int[W, H];

            for (int i = 0; i < W; i++)
                for (int j = 0; j < H; j++)
                    if (map[i, j] == 1 || map[i, j] == 3 || map[i, j] == 4 || map[i, j] == 2)
                        grid[i, j] = BLANK;
                    else grid[i, j] = WALL;


            int[] dy = { 1, 0, -1, 0 };   // смещения, соответствующие соседям ячейки
            int[] dx = { 0, 1, 0, -1 };   // справа, снизу, слева и сверху
            int d, x, y, k;
            bool stop;

            if (grid[ax,ay] == WALL || grid[bx,by] == WALL) return false;  // ячейка (ax, ay) или (bx, by) - стена

            // распространение волны
            d = 0;
            grid[ax,ay] = 0;            // стартовая ячейка помечена 0
            do 
            {
                stop = true;               // предполагаем, что все свободные клетки уже помечены
                for (x = 0; x<W; x++ )
                  for (y = 0; y<H; y++ )
                    if (grid[x,y] == d )                         // ячейка (x, y) помечена числом d
                    {
                        for (k = 0; k< 4; ++k )                    // проходим по всем непомеченным соседям
                        {
                            int iy = y + dy[k], ix = x + dx[k];
                            if (iy >= 0 && iy<H && ix >= 0 && ix<W && grid[ix,iy] == BLANK)
                            {
                                stop = false;              // найдены непомеченные клетки
                                grid[ix,iy] = d + 1;      // распространяем волну
                            }
                      }
                    }
                d++;
            } while ( !stop && grid[bx,by] == BLANK);

            if (grid[bx,by] == BLANK) return false;  // путь не найден

    // восстановление пути
            len = grid[bx,by]+1;            // длина кратчайшего пути из (ax, ay) в (bx, by)
            x = bx;
            y = by;
            d = len;
            while (d > 0 )
            {
                px[d] = x;
                py[d] = y;                   // записываем ячейку (x, y) в путь
                d--;
                for (k = 0; k< 4; ++k)
                {
                    int iy = y + dy[k], ix = x + dx[k];
                    if (iy >= 0 && iy<H && ix >= 0 && ix<W && grid[ix,iy] == d)
                    {
                        x = x + dx[k];
                        y = y + dy[k];           // переходим в ячейку, которая на 1 ближе к старту
                        break;
                    }
                }
            }
            px[0] = ax;
            py[0] = ay;                    // теперь px[0..len] и py[0..len] - координаты ячеек пути


            for (int i=0;i<len;i++)
                way.Add(new Point(px[i], py[i]));

            return true;
        }
    }
}
