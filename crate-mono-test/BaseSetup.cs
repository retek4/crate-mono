using System;
using cratemonotest.Utils;
using Crate;
using NUnit.Framework;

namespace cratemonotest
{
    internal class BaseSetup
    {
        public static CrateConnection TestCrateConnection()
        {
            return new CrateConnection("192.168.1.200:80");
        }
    }
}
