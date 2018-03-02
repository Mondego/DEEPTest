﻿using System;

namespace DeepTest
{
    public class AssertionResult
    {
        public string Value;
        public int Key;

        public AssertionResult(int k, object v)
        {
            Key = k;
            Value = v.ToString();
        }
    }
}
