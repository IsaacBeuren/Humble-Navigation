using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model;
using Br.Scania.ExternalAGV.Model.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Br.Scania.ExternalAGV.WebAPI.Controllers
{
    public class ConfigPointsController : ControllerBase
    {
        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/ConfigPoints/GetAll")]
        public ActionResult GetAll()
        {
            ConfigPointsBusiness context = new ConfigPointsBusiness();
            List<ConfigPointsModel> ret = context.GetAll();
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/ConfigPoints/GetCompleteRoutesByID")]
        public ActionResult GetCompleteRoutesByID(int ID)
        {
            ConfigPointsBusiness context = new ConfigPointsBusiness();
            List<ConfigPointsModel> ret = context.GetCompleteRoutesByID(ID);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/ConfigPoints/TransferRouteToAGV")]
        public ActionResult TransferRouteToAGV(int ID)
        {
            ConfigPointsBusiness context = new ConfigPointsBusiness();
            List<ConfigPointsModel> ret = context.TransferRouteToAGV(ID);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/ConfigPoints/GetByID")]
        public ActionResult GetByID(int ID)
        {
            ConfigPointsBusiness context = new ConfigPointsBusiness();
            ConfigPointsModel ret = context.GetById(ID);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/ConfigPoints/Insert")]
        public ActionResult Insert(ConfigPointsModel ConfigPointsModel)
        {
            ConfigPointsBusiness context = new ConfigPointsBusiness();
            ConfigPointsModel ret = context.Insert(ConfigPointsModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/ConfigPoints/Update")]
        public ActionResult Update(ConfigPointsModel ConfigPointsModel)
        {
            ConfigPointsBusiness context = new ConfigPointsBusiness();
            ConfigPointsModel ret = context.Update(ConfigPointsModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/ConfigPoints/UpdatePoint")]
        public ActionResult UpdatePoint(ConfigPointsModel ConfigPointsModel)
        {
            ConfigPointsBusiness context = new ConfigPointsBusiness();
            ConfigPointsModel ret = context.UpdatePoint(ConfigPointsModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/ConfigPoints/DirectUpdate")]
        public ActionResult DirectUpdate(ConfigPointsModel ConfigPointsModel)
        {
            ConfigPointsBusiness context = new ConfigPointsBusiness();
            ConfigPointsModel ret = context.DirectUpdate(ConfigPointsModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Get")]
        [Route("api/ConfigPoints/Remove")]
        public ActionResult Remove(int ID)
        {
            ConfigPointsBusiness context = new ConfigPointsBusiness();
            bool ret = context.RemoveById(ID);
            if (!ret)
            {
                return NotFound();
            }
            return Ok(ret);
        }
    }
}
