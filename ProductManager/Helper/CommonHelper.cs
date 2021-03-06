﻿using ProductManager.Model.ItemModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ProductManager.Logic;
using ProductManager.Model.Dictionary;

namespace ProductManager.Helper {

    public class CommonHelper {
        public static bool IsNumberOrNull(dynamic value) {
            if (value == null) {
                return true;
            }
            var str = value.ToString();
            return string.IsNullOrEmpty(str) || Regex.IsMatch(str, @"^[-]?\d+[.]?\d*$");
        }

        public static UnderDateTime GetUnderDateTime(dynamic value) {
            var str = value.ToString();
            DateTime dt;
            if (!DateTime.TryParse(str, out dt)) {
                return null;
            }

            var result = new UnderDateTime {
                Year = dt.Year
            };

            if (str.Contains("月")) {
                result.Month = dt.Month;
            }
            return result;
        }

        public static string GetCompanyName(dynamic value) {
            var str = value.ToString();
            return str.Replace("编制单位：", "");
        }

        public static bool ExistCompany(string name) {
            return new CompanyLogic().GetCompanyId(name) > 0;
        }
        /// <summary>
        /// 将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IList<T> list) {
            DataTable result = new DataTable();
            if (list.Count > 0) {
                PropertyInfo[] propertys = SortPropertyInfosByReportDictionary(list[0].GetType().GetProperties());
                foreach (PropertyInfo pi in propertys) {
                    var property = (pi.GetValue(list[0]) ?? "0").GetType();
                    result.Columns.Add(pi.Name, property);
                }

                for (int i = 0; i < list.Count; i++) {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys) {
                        object obj = pi.GetValue(list[i], null) ?? "0";
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
        public static DataTable StringListToDataTable<T>(string propertyName, IList<T> list) {
            DataTable result = new DataTable();
            if (list.Count > 0) {

                for (int i = 0; i < list.Count; i++) {
                    ArrayList tempList = new ArrayList();
                    tempList.Add(list[i]);
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        public static bool CopyDbFileToPublic() {
            var dbPath = ConfigurationSettings.AppSettings["DbPath"];
            var destFile = Path.Combine(dbPath, "ProductManagerDB.db");
            if (File.Exists(destFile)) {
                return false;
            }
            Directory.CreateDirectory(dbPath);
            var sourceFile = Path.Combine("Db", "ProductManagerDB.db");
            File.Copy(sourceFile, destFile);
            return true;
        }
        public static void SetFileAccess() {
            var dbPath = ConfigurationSettings.AppSettings["DbPath"];
            File.SetAttributes(Path.Combine(dbPath, "ProductManagerDB.db"), FileAttributes.Normal);
            if (File.Exists(Path.Combine(dbPath, "data.bin")))
                File.SetAttributes(Path.Combine(dbPath, "data.bin"), FileAttributes.Normal);
        }

        private static PropertyInfo[] SortPropertyInfosByReportDictionary(PropertyInfo[] propertyInfos) {
            int index = 0;
            var dics = NormDictionary.GetReportDictionary();
            PropertyInfo[] newPropertyInfos = new PropertyInfo[dics.Keys.Count];
            foreach (var item in dics) {
                var property = propertyInfos.FirstOrDefault(n => n.Name == item.Value);
                newPropertyInfos[index] = property;
                index++;
                if (index == dics.Keys.Count)
                    break;
            }
            return newPropertyInfos;
        }
    }
}