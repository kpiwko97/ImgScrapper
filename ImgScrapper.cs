using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;
using System.Linq;


namespace Process.ImgScrapper
{
    public static class ImgScrapper
    {
        [FunctionName("ImgScrapper")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
       {

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic jData = JsonConvert.DeserializeObject(requestBody);
            var uriString = jData?.url.ToString();

            Uri uri;
            if (Uri.TryCreate(uriString, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                try
                {
                    using var client = new HttpClient();

                    var byteArrayResult = await client.GetByteArrayAsync(uri);
                    var websiteContent = System.Text.Encoding.UTF8.GetString(byteArrayResult);
                    
                    var matches = Regex.Matches(websiteContent, "(?<prefix><img\\s+(?:[^>]*?\\s+)?src\\s*=\\s*(?<hrefseparator>[\"\"']))(?<hrefvalue>.*?)(?<sufix>\\k<hrefseparator>(?:[^>]*?)>)", RegexOptions.IgnoreCase | RegexOptions.Compiled); 
                    var imgHrefs = new HashSet<string>(matches
                    .Select(m => HttpUtility.HtmlDecode(m.Groups["hrefvalue"].Value))
                    .Skip(1));

                    var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
                    var blobClient = storageAccount.CreateCloudBlobClient();
                    var container = blobClient.GetContainerReference("photos");
                    container.CreateIfNotExists();
                    
                    var imgErrorsCount = 0;

                    foreach (string imgName in imgHrefs)
                    {
                        try
                        {
                            var imgUri =  (imgName.StartsWith("http://") || imgName.StartsWith("https://")) ? new Uri(imgName): 
                            new UriBuilder
                            { 
                                Scheme = uri.Scheme, 
                                Host = uri.Host, 
                                Path = imgName 
                            }.Uri;
                        
                            var img = await client.GetStreamAsync(imgUri);
                            var blob = container.GetBlockBlobReference(uri.Host + "/" + imgUri.Segments.LastOrDefault());
                            await blob.UploadFromStreamAsync(img);
                            log.LogInformation($"HTTP POST {imgName} successfully uploaded");
                        }
                        catch(Exception e)
                        {
                            log.LogError($"Error while processing image: {imgName}. {e.Message}");
                            imgErrorsCount++;
                        }
  
                    }
                    
                    return new OkObjectResult($"{imgHrefs.Count - imgErrorsCount} image/s have been successfully added");

                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    return new BadRequestObjectResult(e.Message);
                }
            }    

            else
            {
                return new BadRequestObjectResult("Invalid Json or url parameter");
            }
        }
    }
}
