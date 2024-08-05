using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Controllers
{
    public class CheckController : Controller
    {
        public JsonResult CheckDateEndMoreThenStartForEmployee (DateTime dateAdmissionTransfer, DateTime dateDismissalTransfer)
        {
            double different = (dateDismissalTransfer.Date - dateAdmissionTransfer.Date).TotalDays;
            if (different < 0)
            {
                return Json(false);
            }
            return Json(true);
        }

        public JsonResult CheckDateEndMoreThenStartForDivision(DateTime dateCreation, DateTime dateLiquidation)
        {
            double different = (dateLiquidation.Date - dateCreation.Date).TotalDays;
            if (different < 0)
            {
                return Json(false);
            }
            return Json(true);
        }
    }
}
