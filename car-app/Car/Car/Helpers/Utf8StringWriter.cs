using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Car.Helpers
{
    public class Utf8StringWriter : StringWriter
    {
        public Utf8StringWriter(StringBuilder sb)
            : base(sb)
        {
        }
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}