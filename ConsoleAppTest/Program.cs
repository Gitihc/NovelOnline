using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleAppTest
{
    class Program
    {
        //测试获取数据类
        static void Main(string[] args)
        {
            try
            {
                //var url = @"http://www.lwxslwxs.com/53/53504/11199231.html";
                //Regex reg = new Regex(@"http://www\.lwxslwxs\.com/\d+/\d+/$");
                //Match m = reg.Match(url);

                //if (m.Success)
                //{

                //}

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var urlList = @"http://www.lwxslwxs.com/22/22438/";
                //var urlContent = @"http://www.lwxslwxs.com/0/22438/3465049.html";


                var library = new LWXSLWXSLibrary.LWXSLWXSLibrary();

                var novel = library.GetNovelInfo(urlList);

                var listChapter = library.GetList(urlList);

                foreach (var item in listChapter)
                {
                    var c = library.GetChapterContent(item);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message.ToString());
                Console.ReadLine();
            }

        }
    }
}
