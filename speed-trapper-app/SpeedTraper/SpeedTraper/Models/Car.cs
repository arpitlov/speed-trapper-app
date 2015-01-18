using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpeedTraper.Models
{
    public class Car
    {
        public string Name { get; set; }
        public int Speed { get; set; }
        public string SessionId { get; set; }
        public bool IsBusted { get; set; }
        public string ConnectionId { get; set; }
    }
}