using System.Web.Mvc;
using MongoDB.Component;

namespace MongoDB.WebIDE.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CacheExpire()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetServerDetail()
        {
            var nodes = MongoContext.GetTreeNodes();
            return Json(nodes);
        }
    }
}
