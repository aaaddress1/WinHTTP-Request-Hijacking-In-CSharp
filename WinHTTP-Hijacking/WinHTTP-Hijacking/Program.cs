using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace WinHTTP_Hijacking
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            try
            {
                //NEED TO SET PROXY OF IE = 127.0.0.1:5278
                //P.S. IT DOESN'T WORK ON HTTP WITH SSL.
                listener.Prefixes.Add("http://*:5278/");
                listener.Start();
                Console.WriteLine("ADR's WinHTTP-Hijacking, By.aaaddress1.");
                while (true)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem((CurrentContent) =>
                    {
                        HttpListenerRequest request = ((HttpListenerContext)CurrentContent).Request;
                        HttpListenerResponse response = ((HttpListenerContext)CurrentContent).Response;
                        HttpWebRequest Nrequest = (HttpWebRequest)WebRequest.Create(request.RawUrl);
                        Nrequest.Proxy = null;
                        MemoryStream memoryStream = new MemoryStream();
                        Console.WriteLine("catch request url = " +request.RawUrl);
                        Console.WriteLine(request.Headers["Accept"]);
                        Console.WriteLine();
                     
                        
                        Stream responseStream = Nrequest.GetResponse().GetResponseStream();
                        byte[] buffer = new byte[4096];
                        for (int count = -1; count != 0; )
                        {
                            count = responseStream.Read(buffer, 0, buffer.Length);
                            memoryStream.Write(buffer, 0, count);
                        }
                        byte[] result = memoryStream.ToArray();

                        if (request.Headers["Accept"].Contains("text"))
                        {
                            string CurrentHtmlSource = request.ContentEncoding.GetString(result);
                            if (CurrentHtmlSource.ToLower().Contains("google"))
                            {
                                result = request.ContentEncoding.GetBytes("I don't like google :(");
                            }
                        }
                       
                        response.ContentLength64 = result.Length;
                        System.IO.Stream output = response.OutputStream;
                        output.Write(result, 0, result.Length);
                        output.Close();
                    }
                    , listener.GetContext());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                listener.Stop();
            }
        } 
      
    }
}
