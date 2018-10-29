using System;
using BCL.Entity;
using BCL.Interfaces;

namespace BCL
{
    public class Logger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
