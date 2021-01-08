using log4net;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.Mvc;

namespace DTSApplication.DataAccess
{
    public class JobsData
    {
        private readonly static ILog logger;

        public DateTime dtStartTime;

        public DateTime dtEndTime;

        public DateTime localdtStartTime;

        public double dlTimeDiff;

        public TimeSpan tsDiff;

        static JobsData()
        {
            JobsData.logger = LogManager.GetLogger("DTSApp");
        }

        public JobsData()
        {
        }

        public static string[] GetJobListfromExcelOleDb()
        {
            string[] strArrays;
            string path = ConfigurationManager.AppSettings["filepath"];
            string ConnectionString = string.Concat("Provider= Microsoft.ACE.OLEDB.12.0;Data Source=", path, ";Mode=Read;Extended Properties='Excel 12.0;Persist Security Info=False;HDR=Yes'");
            string sheetName = ConfigurationManager.AppSettings["SheetName"];
            string[] lstJobid = new string[0];
            using (OleDbConnection con = new OleDbConnection(ConnectionString))
            {
                try
                {
                    con.Open();
                    OleDbCommand comm = new OleDbCommand(string.Concat("SELECT * FROM [", sheetName, "$]"), con);
                    OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter()
                    {
                        SelectCommand = comm
                    };
                    DataSet ds = new DataSet();
                    oleDbDataAdapter.Fill(ds);
                   System.Data.DataTable dt = ds.Tables[0];
                    string[] lstPjobid = new string[dt.Rows.Count];
                    int i = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string id = dr[0].ToString();
                        string date = dr[3].ToString();
                        lstPjobid[i] = string.Concat(id, ",", date);
                        i++;
                    }
                    strArrays = lstPjobid;
                    return strArrays;
                }
                catch (Exception exception)
                {
                    Exception ee = exception;
                    JobsData.logger.Debug(string.Concat("Error from GetJobListfromExcelOleDb : ", ee.Message, " ", ee.StackTrace));
                }
                strArrays = lstJobid;
            }
            return strArrays;
        }

        public static string[] GetJobsListFromExcel()
        {
            Application excelApplication = (Application)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
            List<string> jobsList = new List<string>();
            string filePath = ConfigurationManager.AppSettings["filepath"];
            Workbook workbooks = excelApplication.Workbooks.Open(filePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Worksheet worksheet = (Worksheet)((dynamic)workbooks.Worksheets[1]);
            Range range = worksheet.UsedRange;
            int row = range.Rows.Count;
            int col = range.Columns.Count;
            Range firstColumn = (Range)((dynamic)worksheet.UsedRange.Columns[1, Type.Missing]);
            Range lastColumn = (Range)((dynamic)worksheet.UsedRange.Columns[4, Type.Missing]);
            Array array1 = (Array)((dynamic)lastColumn.Cells[Type.Missing]);
            Array array = (Array)((dynamic)firstColumn.Cells[Type.Missing]);
            string[] strArray = (
                from a in array.OfType<object>()
                select a.ToString()).ToArray<string>();
            string[] strArray1 = (
                from a in array1.OfType<object>()
                select a.ToString()).ToArray<string>();
            string[] finalArray = new string[(int)strArray.Length];
            for (int i = 0; i < (int)strArray.Length; i++)
            {
                if (i != 0)
                {
                    finalArray[i] = string.Concat(strArray[i], ",", strArray1[i - 1]);
                }
                else
                {
                    finalArray[i] = strArray[i];
                }
            }
            workbooks.Close(Type.Missing, Type.Missing, Type.Missing);
            return finalArray;
        }

        public static IEnumerable<SelectListItem> LoadJobs()
        {
            List<SelectListItem> selectJobList = new List<SelectListItem>();
            string[] jobsList = JobsData.GetJobListfromExcelOleDb();
            int i = 0;
            selectJobList.Add(new SelectListItem()
            {
                Value = jobsList[0],
                Text = "PJOBID"
            });
            try
            {
                string[] strArrays = jobsList;
                for (int num = 0; num < (int)strArrays.Length; num++)
                {
                    string job = strArrays[num];
                    i++;
                    if (i >= 2)
                    {
                        string[] pid = job.Split(new char[] { ',' });
                        selectJobList.Add(new SelectListItem()
                        {
                            Value = job,
                            Text = pid[0]
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                Exception ee = exception;
                JobsData.logger.Debug(string.Concat("Error from LoadJobs : ", ee.Message, " ", ee.StackTrace));
            }
            return selectJobList;
        }
    }
}