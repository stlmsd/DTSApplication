using System;
using System.Runtime.CompilerServices;

namespace DTSApplication.Models
{
    public class Asset
    {
        public string DateStatus
        {
            get;
            set;
        }

        public string FacilityID
        {
            get;
            set;
        }

        public string FeatureClassName
        {
            get;
            set;
        }

        public bool IsUpdate
        {
            get;
            set;
        }

        public string LastJobID
        {
            get;
            set;
        }

        public string LastModifiedBy
        {
            get;
            set;
        }

        public string LastModifiedDate
        {
            get;
            set;
        }

        public string MxAssetNum
        {
            get;
            set;
        }

        public string Ownership
        {
            get;
            set;
        }

        public string OwnershipName
        {
            get;
            set;
        }

        public string PJobID
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }

        public string StatusName
        {
            get;
            set;
        }

        public string System
        {
            get;
            set;
        }

        public Asset()
        {
        }
    }
    public class AssetModified
    {
        public string DateStatus
        {
            get;
            set;
        }

        public string FacilityID
        {
            get;
            set;
        }

        public string FeatureName
        {
            get;
            set;
        }

        public string IsUpdateID
        {
            get;
            set;
        }

        public string MxAssetNum
        {
            get;
            set;
        }

        public AssetModified()
        {
        }
    }
    public class PJob
    {
        public string DateStatus
        {
            get;
            set;
        }

        public string JOBID
        {
            get;
            set;
        }

        public PJob()
        {
        }
    }
}