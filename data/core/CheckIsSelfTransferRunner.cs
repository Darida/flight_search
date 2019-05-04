using System;
using System.Collections.ObjectModel;
using local;
using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace core {
        class CheckIsSelfTransferRunner {

            public async Task<bool> isSelfTransferAsync(Itinerary it) {
                Console.WriteLine(await getUrl(it));
 
                return false;
            }

            public async Task<string> getUrl(Itinerary it) {         
                string firstPage = await it.Deeplink.GetStringAsync();     
                string redirectToken = firstPage.Substring(firstPage.IndexOf("var redirect_id = ") + "var redirect_id = ".Length + 1, 36);

                string deeplink = Uri.UnescapeDataString(it.Deeplink);  
                string redirectUrl = "https://www.skyscanner.net/skippy_api"
                     + deeplink.Substring(deeplink.IndexOf("www.skyscanner.net") + "www.skyscanner.net".Length);
                redirectUrl = redirectUrl.Substring(0, redirectUrl.IndexOf("flights") + "flights".Length) + "?redirect_id=" + redirectToken;
                Console.WriteLine(redirectUrl);
                string finalUrl = (await redirectUrl
                        .WithHeader("User-Agent", "Mozilla/5.0")
                        .GetJsonAsync()).url;

                return finalUrl;
            }      
        }
}