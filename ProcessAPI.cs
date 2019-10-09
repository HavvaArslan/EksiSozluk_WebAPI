using Eski_WebAPI.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Eski_WebAPI.Manager
{
    public class ProcessAPI
    {
        private  int data_id, p_size = 1,sayac=0, ekle_entry=0,donus=0,else_kisim=0;
        private  string target,sabit_name;
        private IEnumerable<Model> results;
        public  List<Model> List = new List<Model>();
      
        public  HttpResponseMessage getAllCodes(string name, int? incoming_date)
        {
            if (sayac == 0)
            {
                sabit_name = name;
                if (sabit_name.Contains("-"))
                {
                    sabit_name = sabit_name.Replace(@"-", "%20");
                }
                if (sabit_name.Contains(" "))
                {
                    sabit_name=sabit_name.Replace(@" ", "%20");
                }
                if (sabit_name.Contains("ı"))
                {
                    sabit_name = sabit_name.Replace(@"ı", "i");
                }
                if (sabit_name.Contains("ü"))
                {
                    sabit_name = sabit_name.Replace(@"ü", "u");
                }
                if (sabit_name.Contains("ç"))
                {
                    sabit_name = sabit_name.Replace(@"ç", "c");
                }
                if (sabit_name.Contains("ş"))
                {
                    sabit_name = sabit_name.Replace(@"ş", "s");
                }
                sayac +=1;
            }
         
            Uri url = new Uri("https://eksisozluk.com/"+ name);
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            string response = client.DownloadString(url);
            // Adresten istek yapıp html kodlarını indiriyoruz.   
            HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument();
            dokuman.LoadHtml(response);
            donus += 1;
            // İndirdiğimiz html kodlarını bir HtmlDocment nesnesine yüklüyoruz. 

           

            HtmlNodeCollection entries = dokuman.DocumentNode.SelectNodes("//div[@class='content']");

            var pageSize = dokuman.DocumentNode.SelectSingleNode("//div[@class='pager']");
            HtmlNodeCollection tarihler = dokuman.DocumentNode.SelectNodes("//div[@class='info']");
           

            if (pageSize != null && sayac==1)
            {
                target = pageSize.Attributes["data-pagecount"].Value;
                sayac += 1;

            }
            var id = dokuman.DocumentNode.SelectSingleNode("//h1");
            data_id= Convert.ToInt32(id.Attributes["data-id"].Value);
            if (ekle_entry == 0)
            {
                addEntry(entries, tarihler);
                ekle_entry += 1;
            }
            else if (p_size <= Convert.ToInt16(target) && donus<= Convert.ToInt16(target))
            {
                addEntry(entries, tarihler);
            }

            if (target != null && p_size < Convert.ToInt64(target))
            {
                    string new_url = "";
                    p_size++;
                    if (sabit_name.Contains("%20"))
                       {
                             sabit_name = sabit_name.Replace(@"%20", "-");
                       }
                    new_url =(sabit_name + "--" + data_id + "?p=" + p_size);
                    
                    getAllCodes(new_url,incoming_date);
            }
          
                string json = JsonConvert.SerializeObject(List.ToArray());
                json = json.Replace(@"\r\n", "");

                var resp = new HttpResponseMessage()
                {
                    Content = new StringContent(json)
                };
                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                if (incoming_date != 0 && else_kisim==0)
                {
                    results = List.Where(s => s.entry_date.Year == incoming_date).ToList();
                        if (results.Count() != 0)
                        {
                                string json2 = JsonConvert.SerializeObject(results.ToArray());
                                resp = new HttpResponseMessage()
                                {
                                    Content = new StringContent(json2.ToString())
                                };
                                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        }else
                        {
                                else_kisim++;
                                List.RemoveAll(x => x.success == true);
                                List.Add(new Model()
                                        {
                                            success = false
                                        });
                                string json3 = JsonConvert.SerializeObject(List.ToArray());
                                resp = new HttpResponseMessage()
                                        {
                                            Content = new StringContent(json3)
                                        };
                                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                   
                        }
              }
                

            return resp;
          
            
        }

  

        public  void addEntry(HtmlNodeCollection entries, HtmlNodeCollection tarihler)
        {
            foreach (var baslik in entries.Zip(tarihler, (n, p) => new { n, p }))
            {
                string[] date = baslik.p.InnerText.Split(' ');
                string iDate = date[6];
                DateTime oDate = Convert.ToDateTime(iDate);
                List.Add(new Model()
                {
                    entry = baslik.n.InnerText,
                    dates = date[6],
                    entry_date = oDate,
                    success=true
                });
                  
            }
        
        }

    }
}