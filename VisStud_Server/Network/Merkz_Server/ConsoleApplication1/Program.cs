using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merkz_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Main Server";

            Server_TCP s_TCP = new Server_TCP();
            s_TCP.Start_Listening();

            Console.WriteLine("Press Any Key to Terminate");
            Console.ReadKey();
        }
    }
}
