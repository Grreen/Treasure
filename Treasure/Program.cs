using System;
using System.IO;


namespace Treasure
{
    class Program
    {
        static void Main(string[] args)
        { 
            const string NameMap = "C:/Users/kiril/Desktop/dotnet.course/Task1/TestData/Map6.txt";
            Map mapa = new Map(NameMap);
            mapa.PrintMap();
            Console.ReadKey();
        }
    }
}
