using System.Collections.Generic;
using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Br.Scania.ExternalAGV.WebAPI.Controllers
{
    public class ConfigController : ControllerBase
    {
        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Config/GetAGVList")]
        public ActionResult GetAGVList()
        {
            ConfigBusiness context = new ConfigBusiness();
            List<ConfigModel> ret = context.GetAGVList();
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET")]
        [Route("api/Config/GetAGVByID")]
        public ActionResult GetAGVByID(int ID)
        {
            ConfigBusiness context = new ConfigBusiness();
            ConfigModel ret = context.GetById(ID);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/Config/Insert")]
        public ActionResult Insert(ConfigModel configModel)
        {
            ConfigBusiness context = new ConfigBusiness();
            ConfigModel ret = context.Insert(configModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/Config/Update")]
        public ActionResult Update(ConfigModel configModel)
        {
            ConfigBusiness context = new ConfigBusiness();
            ConfigModel ret = context.Update(configModel);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Get")]
        [Route("api/Config/Remove")]
        public ActionResult Remove(int ID)
        {
            ConfigBusiness context = new ConfigBusiness();
            bool ret = context.RemoveById(ID);
            if (!ret)
            {
                return NotFound();
            }
            return Ok(ret);
        }


        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/Config/StartById")]
        public ActionResult StartById(int ID, bool Start)
        {
            ConfigBusiness context = new ConfigBusiness();
            ConfigModel ret = context.UpdateById(ID, Start);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

        [AllowAnonymous]
        [AcceptVerbs("Post")]
        [Route("api/Config/UpdateStartById")]
        public ActionResult UpdateStartById(int ID, bool Start)
        {
            ConfigBusiness context = new ConfigBusiness();
            ConfigModel ret = context.UpdateStartById(ID, Start);
            if (ret == null)
            {
                return NotFound();
            }
            return Ok(ret);
        }

    }
}