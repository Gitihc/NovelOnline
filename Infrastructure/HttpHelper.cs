using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace Infrastructure
{
    public static class HttpHelper
    {
        public static string GetString(string url, Encoding _encoding)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ReadWriteTimeout = 30 * 1000;
                var response = (HttpWebResponse)request.GetResponse();
                string html = string.Empty;
                if (response.ContentEncoding != null && response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    using (GZipStream GzipSteam = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        html = new StreamReader(GzipSteam, _encoding).ReadToEnd().ToString();
                    }
                }
                else
                    html = new StreamReader(response.GetResponseStream(), _encoding).ReadToEnd().ToString();
                return html;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string PostString(string url, Encoding _encoding, string PostData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            byte[] bt = Encoding.UTF8.GetBytes(PostData);
            using (Stream sw = request.GetRequestStream())
            {
                sw.Write(bt, 0, bt.Count());
            }
            HttpWebResponse reponse = (HttpWebResponse)request.GetResponse();
            string html = new StreamReader(reponse.GetResponseStream(), _encoding).ReadToEnd().ToString();
            return html;
        }

        public static event FinishedEventHandler Finished;

        public delegate void FinishedEventHandler(object obj);

        public static void GetFile(string url, string filepath)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse reponse = (HttpWebResponse)request.GetResponse();
            int offset;
            byte[] Buffer = new byte[1024];
            using (FileStream fWrite = new FileStream(filepath, FileMode.Create))
            {
                using (Stream _stream = reponse.GetResponseStream())
                {
                    offset = _stream.Read(Buffer, 0, Buffer.Length);
                    while (offset > 0)
                    {
                        fWrite.Write(Buffer, 0, System.Convert.ToInt32(offset));
                        offset = _stream.Read(Buffer, 0, Buffer.Length);
                    }
                }
            }
            Finished(filepath);
        }

    }
}
