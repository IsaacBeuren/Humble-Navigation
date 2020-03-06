using Br.Scania.ExternalAGV.Data;
using Br.Scania.ExternalAGV.Model.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Br.Scania.ExternalAGV.Business
{
    public class PointsBusiness : IDisposable
    {
        private readonly dataContext context;
        private readonly EventLogBusiness log;
        CoordinatesBusiness coordinates;

        public PointsBusiness()
        {
            context = new dataContext();
            log = new EventLogBusiness();
            coordinates = new CoordinatesBusiness();
        }

        public PointsBusiness(dataContext _context)
        {
            context = _context;
        }

        public PointsModel Insert(PointsModel obj)
        {
            try
            {
                context.Point.Add(obj);
                context.SaveChanges();
                return obj;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public bool RemoveAll()
        {
            try
            {
                List<PointsModel> listAll = context.Point.ToList();
                foreach (var item in listAll)
                {
                    RemoveById(item);
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public PointsModel Update(PointsModel obj)
        {
            try
            {
                PointsModel Points = context.Point.Where(o => o.Lat == obj.Lat && o.Lng == obj.Lng).FirstOrDefault();
                if (Points != null)
                {
                    Points.Description = obj.Description;
                    Points.Lat = obj.Lat;
                    Points.Lng = obj.Lng;
                    Points.Velocity = obj.Velocity;
                    Points.LeftLight = obj.LeftLight;
                    Points.RightLight = obj.RightLight;
                    Points.Description = obj.Description;
                    Points.Done = obj.Done;
                    Points.IDRoute = obj.IDRoute;
                    Points.Sequence = obj.Sequence;
                    Points.OnStraight = obj.OnStraight;
                    context.SaveChanges();
                    return Points;
                }
                return null;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public bool TogglePoint(PointsModel obj)
        {
            try
            {
                double Lat = Convert.ToDouble(obj.Lat.ToString("R").Substring(0, 5) + "," + obj.Lat.ToString("R").Substring(5, obj.Lat.ToString("R").Length - 5));
                double Lng = Convert.ToDouble(obj.Lng.ToString("R").Substring(0, 5) + "," + obj.Lng.ToString("R").Substring(5, obj.Lng.ToString("R").Length - 5));

                PointsModel points = context.Point.Where(o => o.Description == obj.Description).FirstOrDefault();
                if (points != null)
                {
                    if (points.Done == true)
                    {
                        points.Done = false;
                    }
                    else
                    {
                        points.Done = true;
                    }
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public string UpdateDone (List<PointsModel> objs)
        {
            try
            {
                List<PointsModel> point = new List<PointsModel>();
                int count = 0;
                foreach (PointsModel obj in objs)
                {
                    point.Add(context.Point.Where(o => o.Description == obj.Description).FirstOrDefault());
                    point[count].Done = obj.Done;
                    count++;
                }
                context.SaveChanges();
                return "true";
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return ex.ToString();
            }
        }

        public bool RemoveById(PointsModel obj)
        {
            try
            {
                PointsModel Points = context.Point.Where(o => o.Lat == obj.Lat && o.Lng == obj.Lng).FirstOrDefault();
                context.Point.Remove(Points);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public List<PointsModel> GetAll()
        {
            try
            {
                List<PointsModel> listAll = context.Point.OrderBy(o=>o.Sequence).ToList();
                return listAll;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public List<PointsModel> GetAllByAGV(double Lat, double Lng)
        {
            try
            {
                List<PointsModel> listAll = context.Point.Where(o => o.Lat == Lat && o.Lng == Lng).ToList();
                List<PointsModel> newList = new List<PointsModel>();

                foreach (var item in listAll)
                {
                    item.Lat = coordinates.ConvertDM2DMS(item.Lat);
                    item.Lng = coordinates.ConvertDM2DMS(item.Lng);

                    newList.Add(item);
                }
                return newList;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public PointsModel GetNextToExecute(int ID)
        {
            try
            {
                PointsModel point = context.Point.Where(o => o.Done == false).OrderBy(o => o.Sequence).FirstOrDefault();
                return point;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public PointsModel GetActual()
        {
            try
            {
                PointsModel point = context.Point.Where(o => o.Done == true).OrderBy(o => o.Sequence).LastOrDefault();
                return point;
            }
            catch (Exception ex)
            {

                log.Write(ex.ToString());
                return null;
            }
        }

        public bool PostPointDone(PointsModel obj)
        {
            try
            {
                PointsModel point = context.Point.Where(o => o.Lat == obj.Lat && o.Lng == obj.Lng).FirstOrDefault();
                point.Done = true;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public bool ResetRoute()
        {
            try
            {
                List<PointsModel> Points = context.Point.ToList();

                foreach (var item in Points)
                {
                    item.Done = false;
                }

                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public PointsModel GetById(PointsModel obj)
        {
            try
            {
                PointsModel transporteOrder = context.Point.Where(o => o.Lat == obj.Lat && o.Lng == obj.Lng).FirstOrDefault();

                return transporteOrder;
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