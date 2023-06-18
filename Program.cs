using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

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

string[] logo = {
    @"  _____                        _____                                           _____ _               _             ",
    @" |  __ \                      / ____|                                   _     / ____| |             | |            ",
    @" | |__) | __ _____  ___   _  | (___   ___ _ __ __ _ _ __   ___ _ __   _| |_  | |    | |__   ___  ___| | _____ _ __ ",
    @" |  ___/ '__/ _ \ \/ / | | |  \___ \ / __| '__/ _` | '_ \ / _ \ '__| |_   _| | |    | '_ \ / _ \/ __| |/ / _ \ '__|",
    @" | |   | | | (_) >  <| |_| |  ____) | (__| | | (_| | |_) |  __/ |      |_|   | |____| | | |  __/ (__|   <  __/ |   ",
    @" |_|   |_|  \___/_/\_\\__, | |_____/ \___|_|  \__,_| .__/ \___|_|             \_____|_| |_|\___|\___|_|\_\___|_|   ",
    @"                       __/ |                       | |                                                             ",
    @"                      |___/                        |_|                                                             ",
    @"  ___           _  _          ",
    @" | _ )_  _     | \| |_  ___ __",
    @" | _ \ || |    | .` | || \ \ /",
    @" |___/\_, |    |_|\_|\_, /_\_\",
    @"      |__/           |__/     "
};

void DrawLogo()
{
    Console.ForegroundColor = ConsoleColor.Red;
    foreach (string line in logo)
    {
        Console.WriteLine(line);
    }
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;
}

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

// funstion to perfom the exit sequence
void Exit()
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
    Environment.Exit(0);
}

// ask the user what type of proxy they want to scrape
async Task main()
{
    DrawLogo();
    string action = GetInput("Do you want to scrape proxies or check proxies? (scrape, check): ");

    if (action == "scrape")
    {
        await Scrape();
    }
    else if (action == "check")
    {
        await Check();
    }
    else
    {
        Console.WriteLine("Invalid action");
    }
}

async Task Scrape()
{
    // ask the user what type of proxy they want to scrape
    DrawLogo();
    string proxyType = GetInput("What type of proxy do you want to scrape? (http, socks4, socks5): ");


    string[] urls = { };
    if (proxyType == "http") {  urls = httpUrls; }
    else if (proxyType == "socks4") { urls = socks4Urls; }
    else if (proxyType == "socks5") { urls = socks5Urls; }
    else { Console.WriteLine("Invalid proxy type"); }


    foreach (string url in urls)
    {
        // make a web request to the url and store the response in a string
        string response = await GetResponse(url);

        // split the response string into an array of strings
        string[] proxies = response.Split("\n");

        // print the number of proxies scraped from the url in green
        Console.ForegroundColor = ConsoleColor.Green;
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

    // remove all duplicate proxies from unchecked.txt and write them back to the file
    string[] lines = File.ReadAllLines("unchecked.txt");
    string[] uniqueLines = lines.Distinct().ToArray();
    File.WriteAllLines("unchecked.txt", uniqueLines);

    // print the number of unique proxies scraped in green
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Scraped " + uniqueLines.Length + " unique proxies");
}

async Task Check()
{
    DrawLogo();
    // check if the user wishes to check the proxies
    string checkProxies = GetInput("Do you want to check the proxies? (y/n): ");

    if (checkProxies != "y")
    {
        Exit();
    }

    if (File.Exists("checked.txt")) { File.Delete("checked.txt"); }

    // function to check a single proxy
    async Task CheckProxy(string proxy)
    {
        string[] proxyParts = proxy.Split(":");
        try
        {
            // create a new HttpClientHandler
            HttpClientHandler handler = new HttpClientHandler();

            // set the proxy of the HttpClientHandler to the proxy that was passed to the function
            handler.Proxy = new WebProxy(proxyParts[0], int.Parse(proxyParts[1]));

            // create a new HttpClient with the HttpClientHandler
            HttpClient client = new HttpClient(handler);

            // set the timeout of the HttpClient to 10 seconds
            client.Timeout = TimeSpan.FromSeconds(10);

            // try to make a web request to https://api.ipify.org/ to get the ip address of the proxy
            string response = await client.GetStringAsync("https://api.ipify.org/");

            // write the proxy to checked.txt if the request was successful
            File.AppendAllText("checked.txt", proxy + "\n");

            // print the proxy and the ip address of the proxy
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Valid proxy: " + proxy);
        }
        catch
        {
            // if the request failed, print an error message
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid proxy: " + proxy);
        }
    }

    // ask the user how many threads they want to use
    int threads = int.Parse(GetInput("How many threads do you want to use? (recommended: 1000+): "));

    // ask the user how many file reccuisons they want to use
    int recursions = int.Parse(GetInput("How many file recursions do you want to use? (recommended: 3 - 10): "));

    // read all the proxies from unchecked.txt into an array of strings
    string[] uncheckedfile = File.ReadAllLines("unchecked.txt");

    // add the list to itself recursions amount of times
    string[] proxiesToCheck = uncheckedfile;
    for (int i = 0; i < recursions; i++)
    {
        proxiesToCheck = proxiesToCheck.Concat(uncheckedfile).ToArray();
    }


    List<Task> tasks = new List<Task>();
    // loop through all the proxies in the proxies array
    foreach (string proxy in proxiesToCheck)
    {
        // add a new task to the tasks list that runs the CheckProxy function with the current proxy\
        tasks.Add(CheckProxy(proxy));
        // if the amount of tasks in the tasks list is equal to the amount of threads the user wants to use, wait for all the tasks to finish and then clear the tasks list
        if (tasks.Count == threads)
        {
            await Task.WhenAll(tasks);
            tasks.Clear();
        }
    }
    await Task.WhenAll(tasks);
    tasks.Clear();
    // remove all duplicate proxies from checked.txt and write them to checked.txt
    string[] checkedLines = File.ReadAllLines("checked.txt");
    string[] uniqueCheckedLines = checkedLines.Distinct().ToArray();
    File.WriteAllLines("checked.txt", uniqueCheckedLines);

    // print the number of valid proxies in checked.txt in green
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Found " + uniqueCheckedLines.Length + " valid proxies");
}

while (true)
{
    await main();
}