using System.Collections.Generic;
using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Br.Scania.ExternalAGV.WebAPI.Controllers
{

    public class PointsController : ControllerBase
    {
        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Points/GetAll")]
        public ActionResult GetAll()
        {
            PointsBusiness context = new PointsBusiness();
            List<PointsModel> ret = context.GetAll();
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Points/RemoveAll")]
        public ActionResult RemoveAll()
        {
            PointsBusiness context = new PointsBusiness();
            bool ret = context.RemoveAll();
            if (ret == true)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Points/GetAllByAGV")]
        public ActionResult GetAllByAGV(double Lat, double Lng)
        {
            PointsBusiness context = new PointsBusiness();
            List<PointsModel> ret = context.GetAllByAGV(Lat, Lng);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("POST")]
        [Route("api/Points/TogglePoint")]
        public ActionResult TogglePoint(PointsModel pointsModel)
        {
            PointsBusiness context = new PointsBusiness();
            bool ret = context.TogglePoint(pointsModel);
            if (ret == false)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Points/GetRoutesByAGV")]
        public ActionResult GetRoutesByAGV(double Lat, double Lng)
        {
            PointsBusiness context = new PointsBusiness();
            List<PointsModel> ret = context.GetAllByAGV(Lat, Lng);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("POST")]
        [Route("api/Points/Insert")]
        public ActionResult Insert(PointsModel PointsModel)
        {
            PointsBusiness context = new PointsBusiness();
            PointsModel ret = context.Insert(PointsModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("POST")]
        [Route("api/Points/Update")]
        public ActionResult Update(PointsModel PointsModel)
        {
            PointsBusiness context = new PointsBusiness();
            PointsModel ret = context.Update(PointsModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("POST")]
        [Route("api/Points/UpdateDone")]
        public ActionResult UpdateDone(List<PointsModel> PointsModel)
        {
            PointsBusiness context = new PointsBusiness();
            string test = context.UpdateDone(PointsModel);
            //if (test != "true")
            //{
            //    return Unauthorized();
            //}
            return Ok(test);
        }

    }
}
