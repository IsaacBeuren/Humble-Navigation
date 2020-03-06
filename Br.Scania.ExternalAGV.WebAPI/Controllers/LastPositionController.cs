using System.Collections.Generic;
using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Br.Scania.ExternalAGV.WebAPI.Controllers
{
    public class LastPositionController : ControllerBase
    {
        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/LastPosition/GetAll")]
        public ActionResult GetAll()
        {
            LastPositionBusiness context = new LastPositionBusiness();
            List<LastPositionModel> ret = context.GetAll();
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/LastPosition/GetAGVPosition")]
        public ActionResult GetAGVPosition(int ID)
        {
            LastPositionBusiness context = new LastPositionBusiness();
            LastPositionModel ret = context.GetById(ID);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/LastPosition/Update")]
        public ActionResult Update(LastPositionModel lastPositionModel)
        {
            LastPositionBusiness context = new LastPositionBusiness();
            LastPositionModel ret = context.Update(lastPositionModel);
            if (ret == null)
            {
                return Unauthorized();
            }
            return Ok(ret);
        }

    }
}