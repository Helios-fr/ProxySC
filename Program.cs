using System;
using System.Net.Http;
using System.Threading.Tasks;

// list of http proxy urls
string[] httpUrls = {
    "https://raw.githubusercontent.com/monosans/proxy-list/main/proxies/http.txt",
    "https://api.proxyscrape.com/v2/?request=getproxies&protocol=http&timeout=10000&country=all&ssl=all&anonymity=all",
    "https://raw.githubusercontent.com/proxy4parsing/proxy-list/main/http.txt",
    "https://www.proxy-list.download/api/v1/get?type=https",
    "https://api.openproxylist.xyz/http.txt",
    "https://raw.githubusercontent.com/monosans/proxy-list/main/proxies_anonymous/http.txt",
    "https://raw.githubusercontent.com/ShiftyTR/Proxy-List/master/https.txt",
    "https://raw.githubusercontent.com/HyperBeats/proxy-list/main/http.txt",
    "https://raw.githubusercontent.com/roosterkid/openproxylist/main/HTTPS_RAW.txt",
    "https://raw.githubusercontent.com/ShiftyTR/Proxy-List/master/http.txt",
    "https://raw.githubusercontent.com/jetkai/proxy-list/main/online-proxies/txt/proxies-https.txt",
    "https://www.proxyscan.io/download?type=http",
    "https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/http.txt",
    "https://www.proxy-list.download/api/v1/get?type=http",
    "https://raw.githubusercontent.com/jetkai/proxy-list/main/online-proxies/txt/proxies-http.txt",
    "http://pubproxy.com/api/proxy",
    "https://raw.githubusercontent.com/mmpx12/proxy-list/master/https.txt",
    "https://raw.githubusercontent.com/mmpx12/proxy-list/master/http.txt",
    "https://www.proxyscan.io/download?type=https"
};

// list of socks4 proxy urls
string[] socks4Urls = {
    "https://api.openproxylist.xyz/socks4.txt",
    "https://www.proxy-list.download/api/v1/get?type=socks4",
    "https://raw.githubusercontent.com/jetkai/proxy-list/main/online-proxies/txt/proxies-socks4.txt",
    "https://raw.githubusercontent.com/ShiftyTR/Proxy-List/master/socks4.txt",
    "https://raw.githubusercontent.com/saschazesiger/Free-Proxies/master/proxies/socks4.txt",
    "https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/socks4.txt",
    "https://raw.githubusercontent.com/HyperBeats/proxy-list/main/socks4.txt",
    "https://www.proxyscan.io/download?type=socks4",
    "https://www.socks-proxy.net/",
    "https://raw.githubusercontent.com/mmpx12/proxy-list/master/socks4.txt",
    "https://raw.githubusercontent.com/monosans/proxy-list/main/proxies/socks4.txt",
    "https://openproxy.space/list/socks4",
    "https://raw.githubusercontent.com/monosans/proxy-list/main/proxies_anonymous/socks4.txt",
    "https://raw.githubusercontent.com/roosterkid/openproxylist/main/SOCKS4_RAW.txt",
    "https://api.proxyscrape.com/v2/?request=getproxies&protocol=socks4&timeout=10000&country=all"
};

// list of socks5 proxy urls
string[] socks5Urls = {
    "https://raw.githubusercontent.com/thespeedx/proxy-list/master/socks5.txt",
    "https://api.openproxylist.xyz/socks5.txt",
    "https://www.proxyscan.io/download?type=socks5",
    "https://raw.githubusercontent.com/roosterkid/openproxylist/main/SOCKS5_RAW.txt",
    "https://raw.githubusercontent.com/hookzof/socks5_list/master/proxy.txt",
    "https://www.proxy-list.download/api/v1/get?type=socks5",
    "https://raw.githubusercontent.com/monosans/proxy-list/main/proxies/socks5.txt",
    "https://raw.githubusercontent.com/saschazesiger/Free-Proxies/master/proxies/socks5.txt",
    "https://raw.githubusercontent.com/hyperbeats/proxy-list/main/socks5.txt",
    "https://openproxy.space/list/socks5",
    "https://raw.githubusercontent.com/mmpx12/proxy-list/master/socks5.txt",
    "https://api.proxyscrape.com/v2/?request=getproxies&protocol=socks5&timeout=10000&country=all",
    "https://raw.githubusercontent.com/jetkai/proxy-list/main/online-proxies/txt/proxies-socks5.txt",
    "https://raw.githubusercontent.com/monosans/proxy-list/main/proxies_anonymous/socks5.txt",
    "https://raw.githubusercontent.com/shiftytr/proxy-list/master/socks5.txt"
};



// Function to get input from user and return it as a string
string GetInput(string prompt)
{
    Console.Write(prompt);
    string input = Console.ReadLine();
    return input;
}

// function to make a web request and return the response as a string
async Task<string> GetResponse(string url)
{
    HttpClient client = new HttpClient();
    HttpResponseMessage response = await client.GetAsync(url);
    response.EnsureSuccessStatusCode();
    string responseBody = await response.Content.ReadAsStringAsync();
    return responseBody;
}

// ask the user what type of proxy they want to scrape
string proxyType = GetInput("What type of proxy do you want to scrape? (http, socks4, socks5): ");

// check if the user wants to scrape http proxies
if (proxyType == "http")
{
    // loop through all the urls in the httpUrls array
    foreach (string url in httpUrls)
    {
        // make a web request to the url and store the response in a string
        string response = await GetResponse(url);

        // split the response string into an array of strings
        string[] proxies = response.Split("\n");

        // loop through all the proxies in the proxies array and write them to unchecked.txt if they start with a number
        foreach (string proxy in proxies)
        {
            if (proxy.StartsWith("1") || proxy.StartsWith("2") || proxy.StartsWith("3") || proxy.StartsWith("4") || proxy.StartsWith("5") || proxy.StartsWith("6") || proxy.StartsWith("7") || proxy.StartsWith("8") || proxy.StartsWith("9") || proxy.StartsWith("0"))
            {
                File.AppendAllText("unchecked.txt", proxy + "\n");
            }
        }
    }
}

// check if the user wants to scrape socks4 proxies
else if (proxyType == "socks4")
{
    // loop through all the urls in the socks4Urls array
    foreach (string url in socks4Urls)
    {
        // make a web request to the url and store the response in a string
        string response = await GetResponse(url);

        // split the response string into an array of strings
        string[] proxies = response.Split("\n");

        // loop through all the proxies in the proxies array and write them to unchecked.txt if they start with a number
        foreach (string proxy in proxies)
        {
            if (proxy.StartsWith("1") || proxy.StartsWith("2") || proxy.StartsWith("3") || proxy.StartsWith("4") || proxy.StartsWith("5") || proxy.StartsWith("6") || proxy.StartsWith("7") || proxy.StartsWith("8") || proxy.StartsWith("9") || proxy.StartsWith("0"))
            {
                File.AppendAllText("unchecked.txt", proxy + "\n");
            }
        }
    }
}

// check if the user wants to scrape socks5 proxies
else if (proxyType == "socks5")
{
    // loop through all the urls in the socks5Urls array
    foreach (string url in socks5Urls)
    {
        // make a web request to the url and store the response in a string
        string response = await GetResponse(url);

        // split the response string into an array of strings
        string[] proxies = response.Split("\n");

        // print the amount of proxies scraped and from what url
        Console.WriteLine("Scraped " + proxies.Length + " proxies from " + url);

        // loop through all the proxies in the proxies array and write them to unchecked.txt if they start with a number
        foreach (string proxy in proxies)
        {
            if (proxy.StartsWith("1") || proxy.StartsWith("2") || proxy.StartsWith("3") || proxy.StartsWith("4") || proxy.StartsWith("5") || proxy.StartsWith("6") || proxy.StartsWith("7") || proxy.StartsWith("8") || proxy.StartsWith("9") || proxy.StartsWith("0"))
            {
                File.AppendAllText("unchecked.txt", proxy + "\n");
            }
        }
    }
}

// if the user didn't enter a valid proxy type, print an error message
else
{
    Console.WriteLine("Invalid proxy type!");
}
