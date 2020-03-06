using Br.Scania.ExternalAGV.Data;
using Br.Scania.ExternalAGV.Model.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Br.Scania.ExternalAGV.Business
{
    public class ConfigPointsBusiness : IDisposable
    {
        private readonly dataContext context;
        private readonly EventLogBusiness log;
        private utilBusiness util;

        public ConfigPointsBusiness()
        {
            context = new dataContext();
            log = new EventLogBusiness();
            util = new utilBusiness();
        }

        public ConfigPointsBusiness(dataContext _context)
        {
            context = _context;
        }
        
        public ConfigPointsModel Insert(ConfigPointsModel obj)
        {
            try
            {
                obj.ID = 0;
                obj.Lat = Convert.ToDouble(obj.Lat.ToString("R").Substring(0, 5) + "," + obj.Lat.ToString("R").Substring(5, obj.Lat.ToString("R").Length - 5));
                obj.Lng = Convert.ToDouble(obj.Lng.ToString("R").Substring(0, 5) + "," + obj.Lng.ToString("R").Substring(5, obj.Lng.ToString("R").Length - 5));
                context.ConfigPoints.Add(obj);
                context.SaveChanges();
                return obj;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public ConfigPointsModel DirectInsert(ConfigPointsModel obj)
        {
            try
            {
                obj.ID = 0;
                context.ConfigPoints.Add(obj);
                context.SaveChanges();

                return obj;
            }
            catch (Exception ex)
            {

                Console.Write("Error db: -- " + ex.ToString());
                log.Write(ex.ToString());
                return null;
            }
        }

        public ConfigPointsModel Update(ConfigPointsModel obj)
        {
            try
            {
                ConfigPointsModel configPoints = context.ConfigPoints.Where(o => o.ID == obj.ID).FirstOrDefault();
                if (configPoints != null)
                {
                    configPoints.Lat = Convert.ToDouble(obj.Lat.ToString("R").Substring(0, 5) + "," + obj.Lat.ToString("R").Substring(5, obj.Lat.ToString("R").Length - 5));
                    configPoints.Lng = Convert.ToDouble(obj.Lng.ToString("R").Substring(0, 5) + "," + obj.Lng.ToString("R").Substring(5, obj.Lng.ToString("R").Length - 5));
                    configPoints.Description = obj.Description;
                    configPoints.icon = obj.icon;
                    configPoints.Velocity = obj.Velocity;  
                    configPoints.LeftLight = obj.LeftLight;
                    configPoints.RightLight = obj.RightLight;
                    configPoints.OnStraight = obj.OnStraight;
                    context.SaveChanges();
                }
                return configPoints;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public ConfigPointsModel DirectUpdate(ConfigPointsModel obj)
        {
            try
            {
                ConfigPointsModel configPoints = context.ConfigPoints.Where(o => o.ID == obj.ID).FirstOrDefault();
                if (configPoints != null)
                {
                    configPoints.Lat = obj.Lat;
                    configPoints.Lng = obj.Lng;
                    configPoints.Description = obj.Description;
                    configPoints.icon = obj.icon;
                    configPoints.Velocity = obj.Velocity;
                    configPoints.LeftLight = obj.LeftLight;
                    configPoints.RightLight = obj.RightLight;
                    configPoints.OnStraight = obj.OnStraight;
                    context.SaveChanges();
                }
                return configPoints;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public ConfigPointsModel UpdatePoint(ConfigPointsModel obj)
        {
            try
            {
                ConfigPointsModel configPoints = context.ConfigPoints.Where(o => o.ID == obj.ID).FirstOrDefault();
                if (configPoints != null)
                {
                    configPoints.Lat = Convert.ToDouble(obj.Lat.ToString("R").Substring(0, 5) + "," + obj.Lat.ToString("R").Substring(5, obj.Lat.ToString("R").Length - 5));
                    configPoints.Lng = Convert.ToDouble(obj.Lng.ToString("R").Substring(0, 5) + "," + obj.Lng.ToString("R").Substring(5, obj.Lng.ToString("R").Length - 5));
                    context.SaveChanges();
                }
                return configPoints;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public List<ConfigPointsModel> TransferRouteToAGV(int ID)
        {
            RouteBusiness routeBusiness = new RouteBusiness();
            ConfigPointsBusiness configBusiness = new ConfigPointsBusiness();
            try
            {
                RouteModel route = routeBusiness.GetById(ID);
                string[] splitedRoutes = route.Routes.Split(";");
                List<ConfigPointsModel> configs = new List<ConfigPointsModel>();
                PointsBusiness points = new PointsBusiness();
                points.RemoveAll();
                int Sequence = 1;
                foreach (var item in splitedRoutes)
                {
                    int numero;
                    bool resultado = Int32.TryParse(item, out numero);
                    if (resultado)
                    {
                        ConfigPointsModel configPointsModel = configBusiness.GetById(numero);
                        PointsModel pointsModel = new PointsModel();

                        pointsModel.Lat = configPointsModel.Lat;
                        pointsModel.Lng = configPointsModel.Lng;
                        pointsModel.Velocity = configPointsModel.Velocity;
                        pointsModel.LeftLight = configPointsModel.LeftLight;
                        pointsModel.RightLight = configPointsModel.RightLight;
                        pointsModel.Description = configPointsModel.Description;
                        pointsModel.OnStraight = configPointsModel.OnStraight;
                        pointsModel.Done = false;
                        pointsModel.IDRoute = ID;
                        pointsModel.Sequence = Sequence;
                        points.Insert(pointsModel);
                        Sequence++;
                    }
                }
                return configs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool RemoveById(int ID)
        {
            try
            {
                ConfigPointsModel ConfigPoints = context.ConfigPoints.Where(o => o.ID == ID).FirstOrDefault();
                context.ConfigPoints.Remove(ConfigPoints);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public List<ConfigPointsModel> GetAll()
        {
            try
            {
                List<ConfigPointsModel> listAll = context.ConfigPoints.ToList();
                return listAll;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public List<ConfigPointsModel> GetCompleteRoutesByID(int ID)
        {
            RouteBusiness routeBusiness = new RouteBusiness();
            ConfigPointsBusiness configBusiness = new ConfigPointsBusiness();
            try
            {
                RouteModel route = routeBusiness.GetById(ID);
                string[] splitedRoutes = route.Routes.Split(";");
                List<ConfigPointsModel> configs = new List<ConfigPointsModel>();
                foreach (var item in splitedRoutes)
                {
                    int numero;
                    bool resultado = Int32.TryParse(item, out numero);
                    if (resultado)
                    {
                        configs.Add(configBusiness.GetById(numero));

                    }
                }
                return configs;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public ConfigPointsModel GetById(int ID)
        {
            try
            {
                ConfigPointsModel configPoints = context.ConfigPoints.Where(o => o.ID == ID).FirstOrDefault();
                return configPoints;
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