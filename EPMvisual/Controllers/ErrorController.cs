using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProyectoVisual.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult Error404()
        {
            return View();
        }

        public ActionResult ErrorGeneral()
        {
            return View();
        }

    }
}