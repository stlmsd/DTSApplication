using DTSApplication.DataAccess;
using DTSApplication.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace DTSApplication.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public ActionResult About()
        {
            ((dynamic)base.ViewBag).Message = "Your application description page.";
            return base.View();
        }

        public ActionResult Contact()
        {
            ((dynamic)base.ViewBag).Message = "Your contact page.";
            return base.View();
        }

        public ActionResult Index(int? page)
        {
            int? nullable;
            ActionResult actionResult;
            bool valueOrDefault;
            if (base.TempData["record"] == null)
            {
                valueOrDefault = true;
            }
            else
            {
                nullable = page;
                int num = 1;
                valueOrDefault = nullable.GetValueOrDefault() < num & nullable.HasValue;
            }
            if (!valueOrDefault)
            {
                int PageSize = 100;
                nullable = page;
                int PageIndex = (nullable.HasValue ? nullable.GetValueOrDefault() : 1);
                List<Asset> lstAssets = base.TempData["record"] as List<Asset>;
                base.TempData.Keep();
                actionResult = base.View(lstAssets.ToPagedList<Asset>(PageIndex, PageSize));
            }
            else
            {
                base.Session["PJOBID"] = null;
                string uname = base.Request.LogonUserIdentity.Name;
                string[] namesplit = uname.Split(new char[] { '\\' });
                base.Session["uname"] = namesplit[1];
                actionResult = base.View();
            }
            return actionResult;
        }

        [HttpPost]
        public ActionResult Index(string PJOBID, int? page)
        {
            ActionResult actionResult;
            if (PJOBID == null)
            {
                actionResult = base.View();
            }
            else
            {
                base.HttpContext.Session.Add("PJOBID", PJOBID);
                int PageSize = 100;
                int PageIndex = 1;
                Assets assets = new Assets();
                List<Asset> lstAssets = new List<Asset>();
                string[] pid = PJOBID.Split(new char[] { ',' });
                lstAssets = assets.GetAssetDetails(pid[0].Trim(), pid[1].Trim(), 0);
                base.TempData["record"] = lstAssets;
                base.TempData["counter"] = lstAssets.Count;
                actionResult = base.View(lstAssets.ToPagedList<Asset>(PageIndex, PageSize));
            }
            return actionResult;
        }

        public ActionResult Update(FormCollection formCollection)
        {
            string[] IsUpdateID = formCollection.GetValues("IsUpdateID");
            string[] FacilityID = formCollection.GetValues("FacilityID");
            string[] MxAssetNum = formCollection.GetValues("MxAssetNum");
            string[] FeatureName = formCollection.GetValues("FeatureName");
            string[] DateStatus = formCollection.GetValues("DateStatus");
            List<AssetModified> assetModifieds = new List<AssetModified>();
            for (int i = 0; i < (int)IsUpdateID.Length; i++)
            {
                if (IsUpdateID[i].Trim() == "1")
                {
                    AssetModified assetModified = new AssetModified()
                    {
                        IsUpdateID = IsUpdateID[i].Trim(),
                        FacilityID = FacilityID[i].Trim(),
                        MxAssetNum = MxAssetNum[i].Trim(),
                        FeatureName = FeatureName[i].Trim(),
                        DateStatus = DateStatus[i].Trim()
                    };
                    assetModifieds.Add(assetModified);
                }
            }
            int countrecord = assetModifieds.Count;
            int counter = (new Assets()).UpdateAssets(assetModifieds);
            base.TempData["record"] = null;
            base.TempData["counter"] = counter;
            return base.RedirectToAction("ViewUpdatedAssets");
        }

        public ActionResult ViewQCAssets(int? page)
        {
            int? nullable;
            ActionResult actionResult;
            bool valueOrDefault;
            if (base.TempData["record"] == null)
            {
                valueOrDefault = true;
            }
            else
            {
                nullable = page;
                int num = 1;
                valueOrDefault = nullable.GetValueOrDefault() < num & nullable.HasValue;
            }
            if (!valueOrDefault)
            {
                int PageSize = 100;
                nullable = page;
                int PageIndex = (nullable.HasValue ? nullable.GetValueOrDefault() : 1);
                List<Asset> lstAssets = base.TempData["record"] as List<Asset>;
                base.TempData.Keep();
                actionResult = base.View(lstAssets.ToPagedList<Asset>(PageIndex, PageSize));
            }
            else
            {
                actionResult = base.View();
            }
            return actionResult;
        }

        [HttpPost]
        public ActionResult ViewQCAssets(string PJOBID, int? page)
        {
            ActionResult actionResult;
            if (PJOBID == null)
            {
                actionResult = base.View();
            }
            else
            {
                int PageSize = 100;
                int PageIndex = 1;
                base.HttpContext.Session.Add("PJOBID", PJOBID);
                Assets assets = new Assets();
                List<Asset> lstAssets = new List<Asset>();
                string[] pid = PJOBID.Split(new char[] { ',' });
                lstAssets = assets.GetAssetDetails(pid[0].Trim(), pid[1].Trim(), 1);
                base.TempData["record"] = lstAssets;
                base.TempData["counter"] = lstAssets.Count;
                actionResult = base.View(lstAssets.ToPagedList<Asset>(PageIndex, PageSize));
            }
            return actionResult;
        }

        public ActionResult ViewUpdatedAssets()
        {
            ActionResult actionResult;
            Assets asset = new Assets();
            if (base.TempData["counter"] == null)
            {
                actionResult = base.View();
            }
            else
            {
                string PjobID = base.HttpContext.Session["PJOBID"].ToString();
                string[] pid = PjobID.Split(new char[] { ',' });
                List<Asset> lstAssets = asset.GetAssetDetails(pid[0].Trim(), pid[1].Trim(), 1);
                int TotalCount = lstAssets.Count;
                base.TempData["message"] = string.Concat("The Total Records have been updated in QC : ", TotalCount.ToString(), " And currently updated : ", base.TempData["counter"].ToString());
                actionResult = base.View(lstAssets);
            }
            return actionResult;
        }
    }
}