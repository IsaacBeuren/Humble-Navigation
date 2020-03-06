using Br.Scania.ExternalAGV.Data;
using Br.Scania.ExternalAGV.Model.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Br.Scania.ExternalAGV.Business
{
    public class RouteBusiness : IDisposable
    {
        private readonly dataContext context;
        private readonly EventLogBusiness log;

        public RouteBusiness()
        {
            context = new dataContext();
            log = new EventLogBusiness();
        }

        public RouteBusiness(dataContext _context)
        {
            context = _context;
        }

        public RouteModel Insert(RouteModel obj)
        {
            try
            {
                context.Route.Add(obj);
                context.SaveChanges();
                return obj;
            }
            catch (Exception ex)
            {

                Console.WriteLine(" Error db: 77: "+ ex.ToString());
                log.Write(ex.ToString());
                return null;
            }
        }

        public RouteModel Update(RouteModel obj)
        {
            try
            {
                RouteModel Route = context.Route.Where(o => o.ID == obj.ID).FirstOrDefault();
                if (Route != null)
                {
                    Route.Description = obj.Description;
                    context.SaveChanges();
                    return Route;
                }
                return Insert(obj);
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public bool RemoveById(int ID)
        {
            try
            {
                RouteModel Route = context.Route.Where(o => o.ID == ID).FirstOrDefault();
                context.Route.Remove(Route);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public List<RouteModel> GetAll()
        {
            try
            {
                List<RouteModel> listAll = context.Route.OrderBy(o=>o.Description).ToList();
                return listAll;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public RouteModel UpdateRoute(RouteModel routeModel)
        {
            try
            {
                CoordinatesBusiness coordinate = new CoordinatesBusiness();
                RouteModel Route = context.Route.Where(o => o.ID == routeModel.ID).FirstOrDefault();
                Route.Routes = routeModel.Routes;
                Route.PickUpPoint = routeModel.PickUpPoint;
                Route.DropPoint = routeModel.DropPoint;
                context.SaveChanges();
                return Route;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public RouteModel GetById(int ID)
        {
            try
            {
                CoordinatesBusiness coordinate = new CoordinatesBusiness();
                RouteModel Route = context.Route.Where(o => o.ID == ID).FirstOrDefault();
                return Route;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public void Dispose()
        {
            if (context != null)
                context.Dispose();
        }
    }
}