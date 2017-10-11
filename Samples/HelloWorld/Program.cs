using System;

namespace HelloWorld
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            int n = (int)Int64.Parse(args[0]);
           
            SayHelloWorld hello = new SayHelloWorld(n);
            hello.Loop();
        }
    }
}
