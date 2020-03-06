using System.Collections.Generic;
using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Br.Scania.ExternalAGV.WebAPI.Controllers
{
    public class CallsController : ControllerBase
    {
        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Calls/GetCallsList")]
        public ActionResult GetCallsList()
        {
            CallsBusiness context = new CallsBusiness();
            List<CallsModel> ret = context.GetCallsList();
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Calls/GetCallByID")]
        public ActionResult GetCallByID(int ID)
        {
            CallsBusiness context = new CallsBusiness();
            CallsModel ret = context.GetById(ID);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/Calls/Insert")]
        public ActionResult Insert(CallsModel callsModel)
        {
            CallsBusiness context = new CallsBusiness();
            CallsModel ret = context.Insert(callsModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/Calls/Update")]
        public ActionResult Update(CallsModel callsModel)
        {
            CallsBusiness context = new CallsBusiness();
            CallsModel ret = context.Update(callsModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Get")]
        [Route("api/Calls/Remove")]
        public ActionResult Remove(int ID)
        {
            CallsBusiness context = new CallsBusiness();
            bool ret = context.RemoveById(ID);
            if (!ret)
            {
                return NotFound();
            }
            return Ok(ret);
        }

    }
}