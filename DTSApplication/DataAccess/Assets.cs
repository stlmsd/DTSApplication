using DTSApplication.Models;
using ESRI.ArcGIS;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.SessionState;
using ILog = log4net.ILog;

namespace DTSApplication.DataAccess
{
    public class Assets
    {
        private readonly static ILog logger;

        public DateTime dtStartTime;

        public DateTime dtEndTime;

        public DateTime localdtStartTime;

        public double dlTimeDiff;

        public TimeSpan tsDiff;

        static Assets()
        {
            Assets.logger = LogManager.GetLogger("DTSApp");
        }

        public Assets()
        {
        }

        public List<Asset> GetAssetDetails(string PJOBID, string DateStatus, int state)
        {
            string Query;
            this.dtStartTime = DateTime.Now;
            Assets.logger.Debug(string.Concat("GetAssetDetails start at ", this.dtStartTime.ToString()));
            string featureNames = ConfigurationManager.AppSettings["AssetsList"].ToString();
            string[] featureName = featureNames.Split(new char[] { ',' });
            List<Asset> lstAssets = new List<Asset>();
            try
            {
                if ((int)featureName.Length > 1)
                {
                    Query = (state != 1 ? string.Concat("PJOBID = '", PJOBID, "' AND OWNERSHIP = '6'") : string.Concat("PJOBID = '", PJOBID, "' AND OWNERSHIP = '1'"));
                    this.dtStartTime = DateTime.Now;
                    Assets.logger.Debug(string.Concat("GetAssetDetails using Query ", Query, " at ", this.dtStartTime.ToString()));
                    for (int i = 0; i < (int)featureName.Length; i++)
                    {
                        List<Asset> assets = this.GetAssetsDatafromFeaturesClass(Query, featureName[i], DateStatus);
                        lstAssets.AddRange(assets);
                    }
                }
            }
            catch (Exception exception)
            {
                Exception ee = exception;
                Assets.logger.Debug(string.Concat("Error from GetAssetDetails :", ee.Message, " ", ee.StackTrace));
            }
            this.dtEndTime = DateTime.Now;
            this.tsDiff = this.dtEndTime.Subtract(this.dtStartTime);
            this.dlTimeDiff = this.tsDiff.TotalMilliseconds;
            Assets.logger.Debug(string.Concat("GetAssetDetails end at ", this.dtStartTime.ToString(), " Total diff : ", this.dlTimeDiff.ToString()));
            return lstAssets;
        }

        private List<Asset> GetAssetsDatafromFeaturesClass(string featureQuery, string FeatureName, string DateStatus)
        {
            this.dtStartTime = DateTime.Now;
            Assets.logger.Debug(string.Concat("GetAssetsDatafromFeaturesClass start at ", this.dtStartTime.ToString()));
            List<Asset> assets = new List<Asset>();
            IPropertySet propertySet = Assets.GetPropertySet();
            IWorkspace workspaceName = (new SdeWorkspaceFactoryClass()).Open(propertySet, 0);
            IFeatureClass featureClass = ((IFeatureWorkspace)workspaceName).OpenFeatureClass(FeatureName);
            IQueryFilter queryFilter = new QueryFilterClass()
            {
                WhereClause = featureQuery
            };
            IFeatureCursor featureCursor = featureClass.Search(queryFilter, true);
            for (IFeature featureSelect = featureCursor.NextFeature(); featureSelect != null; featureSelect = featureCursor.NextFeature())
            {
                int facilityidindex = featureSelect.Fields.FindField("FACILITYID");
                int mxassetnumindex = featureSelect.Fields.FindField("MXASSETNUM");
                int systemindex = featureSelect.Fields.FindField("SYSTEM");
                int ownershipindex = featureSelect.Fields.FindField("OWNERSHIP");
                int statusindex = featureSelect.Fields.FindField("STATUS");
                int pjobidindex = featureSelect.Fields.FindField("PJOBID");
                int lastjobidindex = featureSelect.Fields.FindField("LAST_JOBID");
                int lastmodifieddtindex = featureSelect.Fields.FindField("LAST_MODIFIED_DATE");
                int lastmodifiedbyindex = featureSelect.Fields.FindField("LAST_MODIFIED_BY");
                //  Asset asset = new Asset();
                //  asset.FacilityID = featureSelect[1].ToString();
                //Asset asset = new Asset()
                //{
                //    FacilityID = featureSelect[facilityidindex].ToString(),
                //    IsUpdate = true,
                //    MxAssetNum = featureSelect[mxassetnumindex].ToString(),
                //    System = featureSelect[systemindex].ToString(),
                //    Ownership = featureSelect[ownershipindex].ToString(),
                //    Status = featureSelect[statusindex].ToString(),
                //    PJobID = featureSelect[pjobidindex].ToString(),
                //    LastJobID = featureSelect[lastjobidindex].ToString(),
                //    LastModifiedDate = featureSelect[lastmodifieddtindex].ToString(),
                //    LastModifiedBy = featureSelect[lastmodifiedbyindex].ToString(),
                //    FeatureClassName = FeatureName,
                //    DateStatus = DateStatus,
                //    StatusName = this.getStatus(Convert.ToInt32(asset.Status)),
                //    OwnershipName = this.getOwnership(Convert.ToInt32(asset.Ownership))
                //};
                Asset asset = new Asset();

                asset.FacilityID = featureSelect.Value[facilityidindex].ToString();
                asset.IsUpdate = true;
                asset.MxAssetNum = featureSelect.Value[mxassetnumindex].ToString();
                asset.System = featureSelect.Value[systemindex].ToString();
                asset.Ownership = featureSelect.Value[ownershipindex].ToString();
                asset.Status = featureSelect.Value[statusindex].ToString();
                asset.PJobID = featureSelect.Value[pjobidindex].ToString();
                asset.LastJobID = featureSelect.Value[lastjobidindex].ToString();
                asset.LastModifiedDate = featureSelect.Value[lastmodifieddtindex].ToString();
                asset.LastModifiedBy = featureSelect.Value[lastmodifiedbyindex].ToString();
                asset.FeatureClassName = FeatureName;
                asset.DateStatus = DateStatus;
                asset.StatusName = this.getStatus(Convert.ToInt32(asset.Status));
                asset.OwnershipName = this.getOwnership(Convert.ToInt32(asset.Ownership));
                           assets.Add(asset);
            }
            this.dtEndTime = DateTime.Now;
            this.tsDiff = this.dtEndTime.Subtract(this.dtStartTime);
            this.dlTimeDiff = this.tsDiff.TotalMilliseconds;
            ILog log = Assets.logger;
            string[] featureName = new string[] { "GetAssetsDatafromFeaturesClass : Feature Name  ", FeatureName, " : Total Assets found  ", null, null, null, null, null };
            featureName[3] = assets.Count.ToString();
            featureName[4] = " end at ";
            featureName[5] = this.dtStartTime.ToString();
            featureName[6] = " Total diff : ";
            featureName[7] = this.dlTimeDiff.ToString();
            log.Debug(string.Concat(featureName));
            return assets;
        }

        public string getOwnership(int value)
        {
            string ownershipValue;
            int num = value;
            if (num == 1)
            {
                ownershipValue = "MSD";
            }
            else
            {
                ownershipValue = (num == 6 ? "Dedication-Transitional" : "None");
            }
            return ownershipValue;
        }

        private static IPropertySet GetPropertySet()
        {
            RuntimeManager.Bind(ProductCode.EngineOrDesktop);
            (new AoInitializeClass()).Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard);
            IPropertySet propertySet = new PropertySetClass();
            try
            {
                string ServerName = ConfigurationManager.AppSettings["ServerName"].ToString();
                string InstanceName = ConfigurationManager.AppSettings["InstanceName"].ToString();
                string DBName = ConfigurationManager.AppSettings["DatabaseName"].ToString();
                string UserName = ConfigurationManager.AppSettings["UserName"].ToString();
                string UserPassword = ConfigurationManager.AppSettings["UserPassword"].ToString();
                propertySet.SetProperty("Server", ServerName);
                propertySet.SetProperty("Instance", InstanceName);
                propertySet.SetProperty("Database", DBName);
                propertySet.SetProperty("user", UserName);
                propertySet.SetProperty("password", UserPassword);
                propertySet.SetProperty("version", "MSD.QC");
            }
            catch (Exception exception)
            {
                Exception ee = exception;
                Assets.logger.Debug(string.Concat("Error from GetPropertySet :", ee.Message, " ", ee.StackTrace));
            }
            return propertySet;
        }

        public string getStatus(int value)
        {
            string statusValue;
            switch (value)
            {
                case 1:
                    {
                        statusValue = "Active";
                        break;
                    }
                case 2:
                    {
                        statusValue = "Abandoned";
                        break;
                    }
                case 3:
                    {
                        statusValue = "Removed";
                        break;
                    }
                case 4:
                    {
                        statusValue = "Consolidated";
                        break;
                    }
                case 5:
                    {
                        statusValue = "Design";
                        break;
                    }
                case 6:
                    {
                        statusValue = "Operating";
                        break;
                    }
                case 7:
                    {
                        statusValue = "TOBE_REMOVED";
                        break;
                    }
                case 8:
                    {
                        statusValue = "INACTIVE";
                        break;
                    }
                default:
                    {
                        statusValue = "None";
                        break;
                    }
            }
            return statusValue;
        }

        private static int GetUpdateAssets(List<AssetModified> assetModifieds, int counter, IWorkspaceEdit workspaceEdit)
        {
            try
            {
                foreach (AssetModified assetModified in assetModifieds)
                {
                    IFeatureWorkspace featureWorkspaceEdit = (IFeatureWorkspace)workspaceEdit;
                    IFeatureClass featureClassEdit = featureWorkspaceEdit.OpenFeatureClass(assetModified.FeatureName.Trim());
                    IQueryFilter query = new QueryFilterClass();
                    if (string.IsNullOrEmpty(assetModified.FacilityID))
                    {
                        query.WhereClause = string.Concat("FACILITYID IS NULL AND MXASSETNUM = '", assetModified.MxAssetNum, "'");
                    }
                    else if (!string.IsNullOrEmpty(assetModified.MxAssetNum))
                    {
                        query.WhereClause = string.Concat(new string[] { "FACILITYID = '", assetModified.FacilityID, "' AND MXASSETNUM = '", assetModified.MxAssetNum, "'" });
                    }
                    else
                    {
                        query.WhereClause = string.Concat("FACILITYID = '", assetModified.FacilityID, "' AND MXASSETNUM IS NULL");
                    }
                    IFeatureCursor featureCursor = featureClassEdit.Search(query, false);
                    for (IFeature featureEdit = featureCursor.NextFeature(); featureEdit != null; featureEdit = featureCursor.NextFeature())
                    {
                      //  featureEdit[6].ToString();
                        int ownershipindex = featureEdit.Fields.FindField("OWNERSHIP");
                        int lastmodifieddtindex = featureEdit.Fields.FindField("LAST_MODIFIED_DATE");
                        int lastmodifiedbyindex = featureEdit.Fields.FindField("LAST_MODIFIED_BY");
                        int inservicedtindex = featureEdit.Fields.FindField("INSERVICEDATE");
                        int lastjobidindex = featureEdit.Fields.FindField("LAST_JOBID");
                        int mxcreationstateindex = featureEdit.Fields.FindField("MXCREATIONSTATE");
                        DateTime lastmodifieddt = DateTime.Now.Date;
                        featureEdit.Value[ownershipindex] = "1";
                        featureEdit.Value[mxcreationstateindex] = 1;
                        featureEdit.Value[inservicedtindex] = Convert.ToDateTime(assetModified.DateStatus);
                        featureEdit.Value[lastjobidindex] = "DTSEDIT";
                        featureEdit.Value[lastmodifiedbyindex] = HttpContext.Current.Session["uname"].ToString();
                        featureEdit.Value[lastmodifieddtindex] = lastmodifieddt.Date;
                        //featureEdit[ownershipindex] = "1";
                        //featureEdit[mxcreationstateindex] = 1;
                        //featureEdit[inservicedtindex] = Convert.ToDateTime(assetModified.DateStatus);
                        //featureEdit[lastjobidindex] = "DTSEDIT";
                        //featureEdit[lastmodifiedbyindex] = HttpContext.Current.Session["uname"].ToString();
                        //featureEdit[lastmodifieddtindex] = lastmodifieddt.Date;
                        featureEdit.Store();
                        counter++;
                    }
                }
            }
            catch (Exception exception)
            {
                Exception ee = exception;
                Assets.logger.Debug(string.Concat("Error from GetUpdateAssets : ", ee.Message, " ", ee.StackTrace));
            }
            return counter;
        }

        public int UpdateAssets(List<AssetModified> assetModifieds)
        {
            int num;
            this.dtStartTime = DateTime.Now;
            Assets.logger.Debug(string.Concat("UpdateAssets start at ", this.dtStartTime.ToString()));
            IPropertySet propertySet = Assets.GetPropertySet();
            int counter = 0;
            try
            {
                IWorkspace workspaceName = (new SdeWorkspaceFactoryClass()).Open(propertySet, 0);
                IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)workspaceName;
                IEnumVersionInfo enumVersionInfo = versionedWorkspace.Versions;
                enumVersionInfo.Reset();
                IVersionInfo existversionInfo = enumVersionInfo.Next();
                while (existversionInfo != null)
                {
                    if (!existversionInfo.VersionName.Trim().Equals("MSD.DTS_EDIT"))
                    {
                        existversionInfo = enumVersionInfo.Next();
                    }
                    else
                    {
                        versionedWorkspace.FindVersion("MSD.DTS_EDIT").Delete();
                        break;
                    }
                }
                enumVersionInfo.Reset();
                IVersion version = (IVersion)((IFeatureWorkspace)workspaceName);
                string name = version.VersionInfo.VersionName;
                IVersion dtsVersion = version.CreateVersion("DTS_EDIT");
                dtsVersion.Access = esriVersionAccess.esriVersionAccessPublic;
                dtsVersion.Description = "Update DTS to MSD Ownership via DTS app";
                if (dtsVersion.VersionName.Contains("DTS_EDIT"))
                {
                    IMultiuserWorkspaceEdit multiuserWorkspaceEdit = (IMultiuserWorkspaceEdit)dtsVersion;
                    IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)dtsVersion;
                    IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;
                    if (multiuserWorkspaceEdit.SupportsMultiuserEditSessionMode(esriMultiuserEditSessionMode.esriMESMVersioned))
                    {
                        multiuserWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                        versionEdit.Reconcile4("MSD.QC", true, false, false, false);
                        workspaceEdit.StartEditOperation();
                        ILog log = Assets.logger;
                        DateTime now = DateTime.Now;
                        log.Debug(string.Concat("GetUpdateAssets start at ", now.ToString()));
                        counter = Assets.GetUpdateAssets(assetModifieds, counter, workspaceEdit);
                        ILog log1 = Assets.logger;
                        now = DateTime.Now;
                        log1.Debug(string.Concat("GetUpdateAssets end at ", now.ToString()));
                        if (versionEdit.CanPost())
                        {
                            versionEdit.Post("MSD.QC");
                        }
                        workspaceEdit.StopEditOperation();
                        workspaceEdit.StopEditing(true);
                    }
                }
                this.dtEndTime = DateTime.Now;
                this.tsDiff = this.dtEndTime.Subtract(this.dtStartTime);
                this.dlTimeDiff = this.tsDiff.TotalMilliseconds;
                Assets.logger.Debug(string.Concat("UpdateAssets end at ", this.dtStartTime.ToString(), " Total diff : ", this.dlTimeDiff.ToString()));
                num = counter;
            }
            catch (COMException cOMException)
            {
                COMException ex = cOMException;
                Assets.logger.Debug(string.Concat("Error from UpdateAssets : ", ex.Message, " ", ex.StackTrace));
                throw ex;
            }
            catch (Exception exception)
            {
                Exception ex = exception;
                Assets.logger.Debug(string.Concat("Error from UpdateAssets : ", ex.Message, " ", ex.StackTrace));
                throw ex;
            }
            return num;
        }

        public void UpdateAssetsTest()
        {
            RuntimeManager.Bind(ProductCode.Desktop);
            (new AoInitializeClass()).Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard);
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("Server", "RJMSDDBT02");
            propertySet.SetProperty("Instance", "sde:oracle11g:gisdevl");
            propertySet.SetProperty("Database", "GISDEVL");
            propertySet.SetProperty("user", "MSD");
            propertySet.SetProperty("password", "jc_tc1954");
            propertySet.SetProperty("version", "MSD.QC");
            int flag = 0;
            try
            {
                IWorkspace workspaceName = (new SdeWorkspaceFactoryClass()).Open(propertySet, 0);
                IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)workspaceName;
                IEnumVersionInfo enumVersionInfo = versionedWorkspace.Versions;
                enumVersionInfo.Reset();
                IVersionInfo existversionInfo = enumVersionInfo.Next();
                while (existversionInfo != null)
                {
                    if (!existversionInfo.VersionName.Trim().Equals("MSD.DTS_EDIT"))
                    {
                        existversionInfo = enumVersionInfo.Next();
                    }
                    else
                    {
                        versionedWorkspace.FindVersion("MSD.DTS_EDIT").Delete();
                        flag = 1;
                        break;
                    }
                }
                enumVersionInfo.Reset();
                IVersion version = (IVersion)((IFeatureWorkspace)workspaceName);
                string name = version.VersionInfo.VersionName;
                IVersion dtsVersion = version.CreateVersion("DTS_EDIT");
                dtsVersion.Access = esriVersionAccess.esriVersionAccessPublic;
                dtsVersion.Description = "Update DTS to MSD Ownership via DTS app";
                if (dtsVersion.VersionName.Contains("DTS_EDIT"))
                {
                    IMultiuserWorkspaceEdit multiuserWorkspaceEdit = (IMultiuserWorkspaceEdit)dtsVersion;
                    IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)dtsVersion;
                    IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;
                    if (multiuserWorkspaceEdit.SupportsMultiuserEditSessionMode(esriMultiuserEditSessionMode.esriMESMVersioned))
                    {
                        multiuserWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                        versionEdit.Reconcile4("MSD.QC", true, false, false, false);
                        workspaceEdit.StartEditOperation();
                        IFeature featureEdit = ((IFeatureWorkspace)workspaceEdit).OpenFeatureClass("SWRAINGAUGE").GetFeature(330);
                        // Convert.ToString(featureEdit[6]);
                        Convert.ToString(featureEdit.Value[6]);
                        int Totalfield = featureEdit.Fields.FieldCount;
                        int ownershipindex = featureEdit.Fields.FindField("OWNERSHIP");
                        int lastmodifieddtindex = featureEdit.Fields.FindField("LAST_MODIFIED_DATE");
                        DateTime lastmodifieddt = DateTime.Now.Date;
                        //featureEdit[ownershipindex] = "1";
                        //featureEdit[lastmodifieddtindex] = lastmodifieddt.Date;
                        featureEdit.Value[ownershipindex] = "1";
                        featureEdit.Value[lastmodifieddtindex] = lastmodifieddt.Date;
                        featureEdit.Store();
                        if (versionEdit.CanPost())
                        {
                            versionEdit.Post("MSD.QC");
                        }
                        workspaceEdit.StopEditOperation();
                        workspaceEdit.StopEditing(true);
                    }
                }
            }
            catch (COMException cOMException)
            {
                if (cOMException.ErrorCode == -2147217147)
                {
                }
            }
            catch (Exception exception)
            {
            }
        }
    }
}