using Br.Scania.ExternalAGV.Data;
using Br.Scania.ExternalAGV.Model.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Br.Scania.ExternalAGV.Business
{
    public class ConfigBusiness : IDisposable
    {
        private readonly dataContext context;
        private readonly EventLogBusiness log;

        public ConfigBusiness()
        {
            context = new dataContext();
            log = new EventLogBusiness();
        }

        public ConfigBusiness(dataContext _context)
        {
            context = _context;
        }

        public List<ConfigModel> GetAGVList()
        {
            try
            {
                List<ConfigModel> listAll = context.Config.OrderBy(x=>x.IDAGV).ToList();
                return listAll;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public ConfigModel Insert(ConfigModel obj)
        {
            try
            {
                context.Config.Add(obj);
                context.SaveChanges();
                return obj;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public ConfigModel Update(ConfigModel obj)
        {
            try
            {
                ConfigModel Config = context.Config.Where(o => o.ID == obj.ID).FirstOrDefault();
                Config.Prefix = obj.Prefix;
                Config.IDAGV = obj.IDAGV;
                context.SaveChanges();
                return Config;
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
                ConfigModel Config = context.Config.Where(o => o.ID == ID).FirstOrDefault();
                context.Config.Remove(Config);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public List<ConfigModel> GetAll()
        {
            try
            {
                List<ConfigModel> listAll = context.Config.ToList();
                return listAll;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public ConfigModel GetById(int ID)
        {
            try
            {
                ConfigModel transporteOrder = context.Config.Where(o => o.ID == ID).FirstOrDefault();

                return transporteOrder;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public ConfigModel GetByIdAGV(int IDAGV)
        {
            try
            {
                ConfigModel transporteOrder = context.Config.Where(o => o.IDAGV == IDAGV).FirstOrDefault();

                return transporteOrder;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public ConfigModel UpdateById(int IDAGV, bool start)
        {
            try
            {
                ConfigModel Config = context.Config.Where(o => o.IDAGV == IDAGV).FirstOrDefault();
                if (start != Config.Start){Config.Start = start;}
                else { Config.Start = !start; }
                context.SaveChanges();
                return Config;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return null;
            }
        }

        public ConfigModel UpdateStartById(int IDAGV, bool start)
        {
            try
            {
                ConfigModel Config = context.Config.Where(o => o.IDAGV == IDAGV).FirstOrDefault();
                Config.Start = start;
                context.SaveChanges();
                return Config;
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