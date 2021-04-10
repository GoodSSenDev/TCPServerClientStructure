﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TCPNetworkModule
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class RouteAttribute : Attribute
    {
        public string Path { get; }

        public RouteAttribute(string path) => Path = path;
    }
}
