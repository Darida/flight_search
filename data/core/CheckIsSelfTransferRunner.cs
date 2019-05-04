using System;
using System.Collections.ObjectModel;
using local;
using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace core {
        class CheckIsSelfTransferRunner {

            public async Task<bool> isSelfTransferAsync(Itinerary it) {
                string page = await getUrl(it);
 
                return page.Contains("Self-transfer");
            }

            public async Task<string> getUrl(Itinerary it) {        
                Console.WriteLine(it.Deeplink);  

                string firstPage = await it.Deeplink.GetStringAsync();   
                string callbackUrl = firstPage.Substring(firstPage.IndexOf("callback_url: \"") + "callback_url: \"".Length);
                callbackUrl = "https://www.skyscanner.net" + callbackUrl.Substring(0, callbackUrl.IndexOf("\""));
                Console.WriteLine(callbackUrl);

                string finalUrl = (await callbackUrl
                        .WithHeader("User-Agent", "Mozilla/5.0")
                        .GetJsonAsync()).url;
                Console.WriteLine(finalUrl);

                return finalUrl;
            }      
        }
}