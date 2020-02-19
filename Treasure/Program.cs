using System;
using System.IO;


namespace Treasure
{
    class Program
    {
        static void Main(string[] args)
        { 
            const string NameMap = "";
            Map mapa = new Map(NameMap);
            mapa.PrintMap();
            Console.ReadKey();
        }
    }
}
