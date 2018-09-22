using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseLibrary
{
    public static class NVHelper
    {
        /// <summary>
        /// 插件路径
        /// </summary>
        //public static string dllPlusPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"dllplus\netcoreapp2.0");
        public static string dllPlusPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"dllplus");
        /// <summary>
        /// 插件容器
        /// </summary>
        private static List<DllPlusInfo> _dllPlusList = new List<DllPlusInfo>();

        public static INVBase NVBaseObject(string url)
        {
            DllPlusInfo pe = null/* TODO Change to default(_) if this is not a reference type */;
            INVBase obj = null/* TODO Change to default(_) if this is not a reference type */;
            if (string.IsNullOrEmpty(url))
                return obj;
            string reg = @"(\w+\.){2}\w+";
            Match m = Regex.Match(url, reg);

            if (m.Success)
            {
                var pl = (from p in NVHelper.DllPlusList
                          where p.WebName == m.Value
                          select p);
                if (pl.Count() > 0)
                {
                    if (pl.Count() == 1)
                        pe = pl.FirstOrDefault();
                    else
                    {
                    }
                }
                if (pe != null)
                    obj = (INVBase)GetInstance(pe.Type);
            }
            return obj;
        }
        public static object GetInstance(System.Type Type)
        {
            return Activator.CreateInstance(Type);
        }

        /// <summary>
        /// 获取插件集合
        /// </summary>
        public static List<DllPlusInfo> DllPlusList
        {
            get
            {
                if (_dllPlusList.Count() == 0)
                    LoadDllPlusList();
                return _dllPlusList;
            }
            set
            {
                _dllPlusList = value;
            }
        }
        /// <summary>
        /// 加载插件集合
        /// </summary>
        /// <returns></returns>
        public static List<DllPlusInfo> LoadDllPlusList()
        {
            if (_dllPlusList.Count > 0)
                return _dllPlusList;

            if (System.IO.Directory.Exists(dllPlusPath))
            {
                foreach (var f in Directory.GetFiles(dllPlusPath, "*.dll"))
                    LoadDllPlusInfo(f);
            }
            return _dllPlusList;
        }

        /// <summary>
        /// 获取插件信息
        /// </summary>
        /// <param name="filePath"></param>
        public static void LoadDllPlusInfo(string filePath)
        {
            if (File.Exists(filePath))
            {
                Assembly ably = Assembly.LoadFile(filePath);
                Type[] types = ably.GetTypes();
                foreach (var t in types)
                {
                    if (t.IsClass && typeof(BaseLibrary.INVBase).IsAssignableFrom(t))
                    {
                        BaseLibrary.INVBase obj = (BaseLibrary.INVBase)Activator.CreateInstance(t);
                        DllPlusInfo pb = new DllPlusInfo() { WebName = obj.GetWebName(), Type = t };
                        _dllPlusList.Add(pb);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 插件信息表
    /// </summary>
    public class DllPlusInfo
    {
        public string WebName;
        public System.Type Type;
    }
}
