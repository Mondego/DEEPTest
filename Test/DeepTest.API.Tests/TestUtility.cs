using System;
using System.IO;
using System.Linq;

using ExampleClientServerEchoApp;


namespace DeepTest.API.Tests
{
    public static class TestUtility
    {
        public static string getRelativeSolutionPath(string testDirectory)
        {
            return Directory.GetParent(testDirectory).Parent.Parent.Parent.FullName;
        }
    }
}

