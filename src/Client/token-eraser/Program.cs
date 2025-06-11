using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace token_eraser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            File.Delete("auth.token");
        }
    }
}
