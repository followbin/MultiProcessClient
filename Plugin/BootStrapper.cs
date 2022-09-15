using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin
{
    public class BootStrapper : MarshalByRefObject
    {
        public void Run(string name)
        {
            new RemotePlugin().RegisterChannel(name);
        }
    }
}
