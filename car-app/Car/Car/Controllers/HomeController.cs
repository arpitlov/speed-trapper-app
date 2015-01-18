using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Car.Helpers;

namespace Car.Controllers
{
    public class HomeController : Controller
    {
        public static CarNames carNames;
        public HomeController() { }

        /// <summary>
        /// Static Constructor to load the car names
        /// </summary>
        static HomeController()
        {
            //The car names are stored in the XML, so that it can be easily deserialized into Cars class.
            string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "CarNames.xml");
            carNames = new CarNames();
            
            using (StreamReader reader = new StreamReader(xmlFilePath))
            {
                string xml = reader.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    carNames = SerializeDeserializeHelper.DeserializeFromXml<CarNames>(xml);
                }
            }
        }

        public ActionResult Index()
        {
            //select the random car name.
            Random random = new Random();
            int index = random.Next(carNames.CarName.Count);
            string carName = carNames.CarName[index];
            ViewBag.CarName = carName;
            return View();
        }


    }
}