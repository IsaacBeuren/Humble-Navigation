using Br.Scania.ExternalAGV.Data;
using Br.Scania.ExternalAGV.Model.DataBase;
using CoordinateSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Br.Scania.ExternalAGV.Business
{
    public class LastPositionBusiness : IDisposable
    {
        private readonly dataContext context;
        private readonly EventLogBusiness log;

        public LastPositionBusiness()
        {
            context = new dataContext();
            log = new EventLogBusiness();
        }

        public LastPositionBusiness(dataContext _context)
        {
            context = _context;
        }

        public LastPositionModel Insert(LastPositionModel obj)
        {
            try
            {
                context.LastPosition.Add(obj);
                context.SaveChanges();
                return obj;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public LastPositionModel Update(LastPositionModel obj)
        {
            try
            {
                LastPositionModel lastPosition = context.LastPosition.Where(o => o.IDAGV == obj.IDAGV).FirstOrDefault();
                if (lastPosition != null)
                {
                    lastPosition.Latitude = obj.Latitude;
                    lastPosition.Longitude = obj.Longitude;
                    lastPosition.UpdateTime = obj.UpdateTime;
                    lastPosition.GPSQuality = obj.GPSQuality;
                    lastPosition.IDAGV = obj.IDAGV;
                    context.SaveChanges();
                    return lastPosition;
                }
                return Insert(obj);
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public bool RemoveById(LastPositionModel obj)
        {
            try
            {
                LastPositionModel lastPosition = context.LastPosition.Where(o => o.Latitude == obj.Latitude && o.Longitude == obj.Longitude).FirstOrDefault();
                context.LastPosition.Remove(lastPosition);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public List<LastPositionModel> GetAll()
        {
            try
            {
                List<LastPositionModel> listAll = context.LastPosition.ToList();
                return listAll;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }
        

        public LastPositionModel GetById(int ID)
        {
            try
            {
                CoordinatesBusiness coordinate = new CoordinatesBusiness();
                LastPositionModel lastPosition = context.LastPosition.Where(o => o.ID == ID).FirstOrDefault();

                LastPositionModel lastPositionModel = new LastPositionModel();
                lastPositionModel.Latitude = lastPosition.Latitude;
                lastPositionModel.Longitude = lastPosition.Longitude;
                lastPositionModel.ID = lastPosition.ID;
                lastPositionModel.UpdateTime = lastPosition.UpdateTime;
                lastPositionModel.GPSQuality = lastPosition.GPSQuality;
                lastPositionModel.IDAGV = lastPosition.IDAGV;

                return lastPositionModel;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public LastPositionModel GetByAGVID(int ID)
        {
            try
            {
                CoordinatesBusiness coordinate = new CoordinatesBusiness();
                LastPositionModel lastPosition = context.LastPosition.Where(o => o.IDAGV == ID).FirstOrDefault();

                LastPositionModel lastPositionModel = new LastPositionModel();
                lastPositionModel.Latitude = coordinate.ConvertDM2DMS(lastPosition.Latitude);
                lastPositionModel.Longitude = coordinate.ConvertDM2DMS(lastPosition.Longitude);
                lastPositionModel.ID = lastPosition.ID;
                lastPositionModel.UpdateTime = lastPosition.UpdateTime;
                lastPositionModel.GPSQuality = lastPosition.GPSQuality;
                lastPositionModel.IDAGV = lastPosition.IDAGV;

                return lastPositionModel;
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