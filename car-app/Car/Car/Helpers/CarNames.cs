using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    [System.Xml.Serialization.XmlRoot(Namespace = "")]
    public class CarNames
    {
        public CarNames()
        {
            CarName = new List<string>();
        }

        [System.Xml.Serialization.XmlElementAttribute("CarName")]
        public List<string> CarName { get; set; }
    }
