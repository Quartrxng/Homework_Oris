using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer
{
    public class Link
    {
        public string PublicDirectoryPath { get; set; }
        public string Domain { get; set; }
        public string Port { get; set; }

        public Link(string publicdirectorypath, string domain, string port)
            {
                PublicDirectoryPath = publicdirectorypath;
                Domain = domain;
                Port = port;
            }

    }
}
