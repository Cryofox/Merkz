using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merkz_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client_TCP c_TCP = new Client_TCP();
            c_TCP.StartClient();

            Console.WriteLine("Program has Ended \n Press Any Key to Terminate");
            Console.ReadKey();
        }


    }
}
