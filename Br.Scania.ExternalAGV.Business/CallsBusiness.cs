using Br.Scania.ExternalAGV.Data;
using Br.Scania.ExternalAGV.Model.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Br.Scania.ExternalAGV.Business
{
    public class CallsBusiness : IDisposable
    {
        private readonly dataContext context;
        private readonly EventLogBusiness log;
        private readonly NlogBusiness nlog;

        public CallsBusiness()
        {
            context = new dataContext();
            log = new EventLogBusiness();

            nlog = new NlogBusiness();
        }

        public CallsBusiness(dataContext _context)
        {
            context = _context;
        }

        public List<CallsModel> GetCallsList()
        {
            try
            {
                List<CallsModel> listAll = context.Calls.OrderBy(x=>x.ID).ToList();
                return listAll;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public CallsModel Insert(CallsModel obj)
        {
            try
            {
                if (context.Calls == null) { nlog.Write(" ====  null ===== "); }
                context.Calls.Add(obj);
                context.SaveChanges();
                nlog.Write(" ====  deu certo ===== ");
                return obj;
            }
            catch (Exception ex)
            {
                nlog.Write("DB error: " + ex.ToString());
                nlog.Write(" ====  deu ruim ===== ");
                return null;
            }
        }

        public CallsModel Update(CallsModel obj)
        {
            try
            {
                CallsModel Calls = context.Calls.Where(o => o.ID == obj.ID).FirstOrDefault();
                Calls.IDAGV = obj.IDAGV;
                Calls.IDRoute = obj.IDRoute;
                Calls.initTime = obj.initTime;
                Calls.endTime = obj.endTime;
                Calls.CarriedItem = obj.CarriedItem;
                Calls.CUCode = obj.CUCode;
                context.SaveChanges();
                return Calls;
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
                CallsModel Calls = context.Calls.Where(o => o.ID == ID).FirstOrDefault();
                context.Calls.Remove(Calls);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }
       

        public CallsModel GetById(int ID)
        {
            try
            {
                CallsModel Call = context.Calls.Where(o => o.ID == ID).FirstOrDefault();
                return Call;
            }
            catch (Exception ex)
            {
                
                nlog.Write(ex.ToString());
                return null;
            }
        }

        //public CallsModel GetByIdAGV(int IDAGV)
        //{
        //    try
        //    {
        //        ConfigModel transporteOrder = context.Config.Where(o => o.IDAGV == IDAGV).FirstOrDefault();

        //        return transporteOrder;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Write(ex.ToString());
        //        return null;
        //    }
        //}

        public void Dispose()
        {
            if (context != null)
                context.Dispose();
        }
    }
}