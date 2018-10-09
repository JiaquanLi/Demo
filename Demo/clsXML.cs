using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace ConvertXML
{
    class clsXML
    {
        public struct DataInfo
        {
            public string X;
            public string Y;
            public string Z;
            public string A;
            public string B;
            public string C;
            public string S;
            public string T;
            public string E1;
            public string E2;
            public string E3;
            public string E4;
            public string E5;
            public string E6;
        }

        public static string dataFile;
        private const int DATALEN = 30;
        private DataInfo[] dataInfo;

        public DataInfo[] Data
        {
            get
            {
                return dataInfo;
            }
        }

        private struct DataInfoKeyStr
        {
            public static string HeadKey = "DECL E6POS XP";
            public static string X = "X ";
            public static string Y = "Y ";
            public static string Z = "Z ";
            public static string A = "A ";
            public static string B = "B ";
            public static string C = "C ";
            public static string S = "S ";
            public static string T = "T ";
            public static string E1 = "E1 ";
            public static string E2 = "E2 ";
            public static string E3 = "E3";
            public static string E4 = "E4";
            public static string E5 = "E5";
            public static string E6 = "E6";
            public static string End = "}";
        }
        public bool WriteMyXML(string input_name, string strXmlFileName, int type_model)
        {
            dataFile = input_name;
            string strErr = "";
            CreateInitData();

            if (GetData(dataFile, ref strErr) == false)
            {
                return false;
            }

            XmlWriter(strXmlFileName, type_model);
            return true;
        }
        private void CreateInitData()
        {
            dataInfo = new DataInfo[DATALEN];

            for (int i = 0; i < DATALEN; i++)
            {
                dataInfo[i].X = "0";
                dataInfo[i].Y = "0";
                dataInfo[i].Z = "0";
                dataInfo[i].A = "0";
                dataInfo[i].B = "0";
                dataInfo[i].C = "0";
                dataInfo[i].E1 = "0";
                dataInfo[i].E2 = "0";
                dataInfo[i].E3 = "0";
                dataInfo[i].E4 = "0";
                dataInfo[i].E5 = "0";
                dataInfo[i].E6 = "0";
                dataInfo[i].S = "0";
                dataInfo[i].T = "0";
            }
        }
        public bool GetData(string fileName, ref string strErrMsg)
        {
            string strTemp;

            if (File.Exists(fileName) == false)
            {
                strErrMsg = "file :" + dataFile + "not exists";
                return false;
            }

            FileStream fs = new FileStream(dataFile, FileMode.Open);
            StreamReader streamReader = new StreamReader(fs);

            int iline = 1;
            int iOffset = 0;
            string strKey = "";
            DataInfo dataInfoTemp = new DataInfo();

            while ((strTemp = streamReader.ReadLine()) != null)
            {
                strKey = string.Format("{0}{1}{2}", DataInfoKeyStr.HeadKey, iline, "={");
                iOffset = strTemp.IndexOf(strKey);
                if (iOffset < 0)
                {
                    strErrMsg = "first line head key error";
                    return false;
                }

                dataInfoTemp.A = "0";
                dataInfoTemp.B = "0";
                dataInfoTemp.C = "0";
                dataInfoTemp.X = "0";
                dataInfoTemp.Y = "0";
                dataInfoTemp.Z = "0";

                dataInfoTemp.E1 = "0";
                dataInfoTemp.E2 = "0";
                dataInfoTemp.E3 = "0";
                dataInfoTemp.E4 = "0";
                dataInfoTemp.E5 = "0";
                dataInfoTemp.E6 = "0";
                dataInfoTemp.S = "0";
                dataInfoTemp.T = "0";

                strTemp = strTemp.Replace(strKey, "");
                if (GetEachLineData(ref dataInfoTemp, strTemp) == false)
                {
                    return false;
                }

                dataInfo[iline - 1].X = dataInfoTemp.X;
                dataInfo[iline - 1].Y = dataInfoTemp.Y;
                dataInfo[iline - 1].Z = dataInfoTemp.Z;

                dataInfo[iline - 1].A = dataInfoTemp.A;
                dataInfo[iline - 1].B = dataInfoTemp.B;
                dataInfo[iline - 1].C = dataInfoTemp.C;

                dataInfo[iline - 1].S = dataInfoTemp.S;
                dataInfo[iline - 1].T = dataInfoTemp.T;

                dataInfo[iline - 1].E1 = dataInfoTemp.E1;
                dataInfo[iline - 1].E2 = dataInfoTemp.E2;
                dataInfo[iline - 1].E3 = dataInfoTemp.E3;
                dataInfo[iline - 1].E4 = dataInfoTemp.E4;
                dataInfo[iline - 1].E5 = dataInfoTemp.E5;
                dataInfo[iline - 1].E6 = dataInfoTemp.E6;

                iline++;
            }
            streamReader.Close();
            fs.Close();
            return true;
        }
        private bool GetEachLineData(ref DataInfo data, string lineString)
        {
            string strTemp;
            string strValue;

            strTemp = lineString;
            strValue = GetKeyValue(DataInfoKeyStr.X, strTemp);
            if (strValue == "") return false;
            data.X = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.Y, strTemp);
            if (strValue == "") return false;
            data.Y = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.Z, strTemp);
            if (strValue == "") return false;
            data.Z = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.A, strTemp);
            if (strValue == "") return false;
            data.A = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.B, strTemp);
            if (strValue == "") return false;
            data.B = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.C, strTemp);
            if (strValue == "") return false;
            data.C = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.S, strTemp);
            if (strValue == "") return false;
            data.S = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.T, strTemp);
            if (strValue == "") return false;
            data.T = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.E1, strTemp);
            if (strValue == "") return false;
            data.E1 = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.E2, strTemp);
            if (strValue == "") return false;
            data.E2 = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.E3, strTemp);
            if (strValue == "") return false;
            data.E3 = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.E4, strTemp);
            if (strValue == "") return false;
            data.E4 = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.E5, strTemp);
            if (strValue == "") return false;
            data.E5 = strValue;

            strValue = GetKeyValue(DataInfoKeyStr.E6, strTemp);
            if (strValue == "") return false;
            data.E6 = strValue;

            return true;
        }
        private string GetKeyValue(string strKey, string strLineString)
        {
            string strValue = "";
            int iPosS = 0;
            int iPosE = 0;
            int iOffset = 0;
            if (strKey == DataInfoKeyStr.E6)
            {
                iOffset = strLineString.IndexOf(strKey, 0);
                if (iOffset < 0) return "";
                iPosS = DataInfoKeyStr.X.Length + iOffset;
                iPosE = strLineString.IndexOf(DataInfoKeyStr.End, iPosS);
                strValue = strLineString.Substring(iPosS, iPosE - iPosS);
            }
            else
            {
                iOffset = strLineString.IndexOf(strKey, 0);
                if (iOffset < 0) return "";
                iPosS = DataInfoKeyStr.X.Length + iOffset;
                iPosE = strLineString.IndexOf(",", iPosS);
                strValue = strLineString.Substring(iPosS, iPosE - iPosS);
            }

            return strValue;
        }
        private void XmlWriter(string strXMLFileName, int type_model)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(dec);

            XmlElement root = doc.CreateElement("Sensor");
            doc.AppendChild(root);

            XmlElement eachValue = doc.CreateElement("Read");
            root.AppendChild(eachValue);

            XmlElement attr_a = doc.CreateElement("xyzabc");
            attr_a.SetAttribute("A", type_model.ToString());
            eachValue.AppendChild(attr_a);

            for (int i = 0; i < DATALEN; i++)
            {
                string elmt = string.Format("xyzabc{0}", i + 1);
                XmlElement xyzabc = doc.CreateElement(elmt);
                xyzabc.SetAttribute("X", dataInfo[i].X);
                xyzabc.SetAttribute("Y", dataInfo[i].Y);
                xyzabc.SetAttribute("Z", dataInfo[i].Z);

                xyzabc.SetAttribute("A", dataInfo[i].A);
                xyzabc.SetAttribute("B", dataInfo[i].B);
                xyzabc.SetAttribute("C", dataInfo[i].C);

                xyzabc.SetAttribute("S", dataInfo[i].S);
                xyzabc.SetAttribute("T", dataInfo[i].T);

                xyzabc.SetAttribute("E1", dataInfo[i].E1);
                xyzabc.SetAttribute("E2", dataInfo[i].E2);
                xyzabc.SetAttribute("E3", dataInfo[i].E3);
                xyzabc.SetAttribute("E4", dataInfo[i].E4);
                xyzabc.SetAttribute("E5", dataInfo[i].E5);
                xyzabc.SetAttribute("E6", dataInfo[i].E6);

                eachValue.AppendChild(xyzabc);
            }
            doc.Save(strXMLFileName);
        }

        public static void WritePoint(List<Demo.frm_Main.SCloutdPoint> lstPT, int type_model, string strXMLFileName)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(dec);

            XmlElement root = doc.CreateElement("Sensor");
            doc.AppendChild(root);

            XmlElement eachValue = doc.CreateElement("Read");
            root.AppendChild(eachValue);
            for (int cout = 0; cout < lstPT.Count; cout++)
            {
                string elmt = string.Format("xyzabc{0}", cout + 1);
                XmlElement xyzabc = doc.CreateElement(elmt);
                xyzabc.SetAttribute("X", lstPT[cout].X.ToString());
                xyzabc.SetAttribute("Y", lstPT[cout].Y.ToString());
                xyzabc.SetAttribute("Z", lstPT[cout].Z.ToString());

                xyzabc.SetAttribute("A", lstPT[cout].A.ToString());
                xyzabc.SetAttribute("B", lstPT[cout].B.ToString());
                xyzabc.SetAttribute("C", lstPT[cout].C.ToString());

                eachValue.AppendChild(xyzabc);
            }

            doc.Save(strXMLFileName);
        }
    }
}
