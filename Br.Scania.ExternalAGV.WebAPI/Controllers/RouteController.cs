using System.Collections.Generic;
using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Br.Scania.ExternalAGV.WebAPI.Controllers
{
    public class RouteController : ControllerBase
    {
        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Route/GetAll")]
        public ActionResult GetAll()
        {
            RouteBusiness context = new RouteBusiness();
            List<RouteModel> ret = context.GetAll();
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Route/GetAGVByID")]
        public ActionResult GetAGVByID(int ID)
        {
            RouteBusiness context = new RouteBusiness();
            RouteModel ret = context.GetById(ID);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/Route/Insert")]
        public ActionResult Insert(RouteModel RouteModel)
        {
            RouteBusiness context = new RouteBusiness();
            RouteModel ret = context.Insert(RouteModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/Route/Update")]
        public ActionResult Update(RouteModel RouteModel)
        {
            RouteBusiness context = new RouteBusiness();
            RouteModel ret = context.Update(RouteModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/Route/UpdateRoute")]
        public ActionResult UpdateRoute(RouteModel RouteModel)
        {
            RouteBusiness context = new RouteBusiness();
            RouteModel ret = context.UpdateRoute(RouteModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Get")]
        [Route("api/Route/Remove")]
        public ActionResult Remove(int ID)
        {
            RouteBusiness context = new RouteBusiness();
            bool ret = context.RemoveById(ID);
            if (!ret)
            {
                return NotFound();
            }
            return Ok(ret);
        }
    }
}