using Eski_WebAPI.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eski_WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EksiController : ApiController
    {

        // GET api/eksi?name=havva&incoming_date=0 
        [HttpGet]
        public HttpResponseMessage Get(string name, int incoming_date)
        {
            ProcessAPI api = new ProcessAPI();
            return api.getAllCodes(name, incoming_date);
        }

    }
}
