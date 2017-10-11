using System;

namespace HelloWorld
{
    public class SayHelloWorld
    {
        private int nTimes;
        private string message;

        public SayHelloWorld(int n)
        {
            nTimes = n;
            message = "Hello World!";
        }

        public void Loop()
        {
            for (int i = 0; i < nTimes; i++) {
                Hello();
            }
        }

        public void Hello()
        {
            Console.WriteLine(message);
        }
    }
}

