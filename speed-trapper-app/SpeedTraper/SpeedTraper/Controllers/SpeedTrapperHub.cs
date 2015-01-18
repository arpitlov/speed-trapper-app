using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using SpeedTraper.Models;
[assembly: OwinStartup(typeof(SpeedTraper.Controllers.Startup))]
namespace SpeedTraper.Controllers
{
    public class SpeedTrapperHub : Hub
    {
        private static int SpeedLimit = 200;
        private static List<Car> RegisteredCars;
        private static List<string> RegisteredTrapperApps;

        static SpeedTrapperHub()
        {
            RegisteredCars = new List<Car>();
            RegisteredTrapperApps = new List<string>();
        }

        public SpeedTrapperHub() { }

        /// <summary>
        /// This method is called by the SpeedTrapper app to set the speed limit.
        /// </summary>
        /// <param name="speedLimit"></param>
        public void SetSpeedLimit(int speedLimit)
        {
            SpeedLimit = speedLimit;

            //Update the speed to all the registered SpeedTrapper instances.
            foreach (var connectionId in RegisteredTrapperApps)
            {
                Clients.Client(connectionId).UpdateSpeedLimit(SpeedLimit);
            }
        }

       /// <summary>
       /// This method is called by the Car app to update the Car speed.
       /// </summary>
       /// <param name="carName"></param>
       /// <param name="sessionId"></param>
       /// <param name="carSpeed"></param>
        public void SpeedTrap(string carName, string sessionId, int carSpeed)
        {
            Car car = new Car();
            car.Name = carName;
            car.Speed = carSpeed;
            car.SessionId = sessionId;
            car.ConnectionId = Context.ConnectionId;
            car.IsBusted = IsBusted(car.Speed);
            var matchingCars = RegisteredCars.FindAll(x => x.SessionId.Equals(car.SessionId));
            foreach (var matchingCar in matchingCars)
            {
                matchingCar.Speed = car.Speed;
                matchingCar.IsBusted = car.IsBusted;
            }

            //Update the Car status on all the SpeedTrapper instances.
            foreach (var registeredApp in RegisteredTrapperApps)
            {
                Clients.Client(registeredApp).UpdateCar(car);
            }
            //Updates the car app if it is busted.
            if (car.IsBusted)
            {
                Clients.Client(Context.ConnectionId).Busted(SpeedLimit);
            }
        }

        /// <summary>
        /// This method is called by the Car app on start.
        /// </summary>
        /// <param name="carName"></param>
        /// <param name="sessionId"></param>
        /// <param name="carSpeed"></param>
        public void RegisterCar(string carName, string sessionId, int carSpeed)
        {
            Car car = new Car();

            var matchingCar = RegisteredCars.FindAll(x => x.SessionId.Equals(sessionId));
            if (matchingCar == null || matchingCar.Count() <= 0)
            {
                car.Name = carName;
                car.Speed = carSpeed;
                car.SessionId = sessionId;
                car.ConnectionId = Context.ConnectionId;
                car.IsBusted = IsBusted(car.Speed);
                RegisteredCars.Add(car);

                //Update all the instances of Speed Trapper to add the car in the list
                foreach (var connectionId in RegisteredTrapperApps)
                {
                    Clients.Client(connectionId).CarAdded(car);
                }

            }
            //Updates car if it is busted
            if (car.IsBusted)
            {
                Clients.Client(Context.ConnectionId).Busted(SpeedLimit);
            }

        }

        /// <summary>
        /// Register the SpeedTrapper apps.
        /// There can be multiple SpeedTrapper instances.
        /// </summary>
        public void RegisterTrapper()
        {

            var matchingApp = RegisteredTrapperApps.FindAll(x => x.Equals(Context.ConnectionId));
            if (matchingApp == null || matchingApp.Count() <= 0)
            {
                RegisteredTrapperApps.Add(Context.ConnectionId);
                Clients.Client(Context.ConnectionId).UpdateSpeedLimit(SpeedLimit);
            }
        }

        /// <summary>
        /// This method is called by the SpeedTrapper app when starts to get the list of Cars that are already registered. 
        /// </summary>
        public void GetRegisteredCars()
        {
                Clients.Client(Context.ConnectionId).ShowCars(RegisteredCars);
        }

        
        /// <summary>
        /// Remove the Car and Speed Trapper session from the list.
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            if (RegisteredCars != null && RegisteredCars.Any())
            {
                RegisteredCars.RemoveAll(x => x.ConnectionId.Equals(Context.ConnectionId));
                foreach (var registeredApp in RegisteredTrapperApps)
                {
                    Clients.Client(registeredApp).RemoveCar(Context.ConnectionId);
                }

            }
            if (RegisteredTrapperApps != null && RegisteredTrapperApps.Any())
            {
                RegisteredTrapperApps.RemoveAll(x => x.Equals(Context.ConnectionId));
            }
            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Check if the speed is exceeding the Speed Limit.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        private bool IsBusted(int speed)
        {
            return speed > SpeedLimit ? true : false;
        }
    }
}