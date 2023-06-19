using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

// list of http proxy urls
string httpRaw = await GetResponse("https://raw.githubusercontent.com/Necrownyx/Proxy-Scraper-and-Checker/main/http.sites");
string[] httpUrls = httpRaw.Split("\n");

// list of socks4 proxy urls
string socks4Raw = await GetResponse("https://raw.githubusercontent.com/Necrownyx/Proxy-Scraper-and-Checker/main/socks4.sites");
string[] socks4Urls = socks4Raw.Split("\n");

// list of socks5 proxy urls
string socks5Raw= await GetResponse("https://raw.githubusercontent.com/Necrownyx/Proxy-Scraper-and-Checker/main/socks5.sites");
string[] socks5Urls = socks5Raw.Split("\n");

// set the console title
Console.Title = "Proxy Scraper and Checker by Nyx";

string[] logo = {
    @"  _____                      _____  _____ ",
    @" |  __ \  By Nyx            / ____|/ ____|",
    @" | |__) | __ _____  ___   _| (___ | |     ",
    @" |  ___/ '__/ _ \ \/ / | | |\___ \| |     ",
    @" | |   | | | (_) >  <| |_| |____) | |____ ",
    @" |_|   |_|  \___/_/\_\\__, |_____/ \_____|",
    @"                       __/ |              ",
    @"      pastebin.com/   |___/   sJAzRdyK    "
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

async Task Upload()
{
    string data = "";
    foreach (string line in File.ReadAllLines("checked.txt"))
    {
        data += line + "\n";
    }

    HttpClient client = new HttpClient();
    var content = new StringContent(data);
    var response = await client.PostAsync("https://api.pastes.dev/post", content);
    var responseString = await response.Content.ReadAsStringAsync();

    // Parse the JSON response to extract the key
    var keyObject = JsonDocument.Parse(responseString).RootElement;
    var key = keyObject.GetProperty("key").GetString();

    Console.WriteLine("RAW URL: https://api.pastes.dev/" + key);
    Console.WriteLine("Press any key to continue");
    Console.ReadKey();
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

// function to clear the console
void Clear()
{
    Console.Clear();
}

// ask the user what type of proxy they want to scrape
async Task main()
{
    Clear();
    DrawLogo();
    string action = GetInput("What do you want to do? (scrape, check, upload): ");

    if (action == "scrape")
    {
        await Scrape();
    }
    else if (action == "check")
    {
        await Check();
    }
    else if (action == "upload")
    {
        await Upload();
    }
    else if (action == "find")
    {
        await Find();
    }
    else
    {
        Console.WriteLine("Invalid action");
    }
}

async Task Scrape()
{
    // ask the user what type of proxy they want to scrape
    Clear();
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
    // ask user if they want to check the proxies
    Console.ResetColor();
    string checkProxies = GetInput("Do you want to check the proxies? (y/n): ");

    if (checkProxies == "y")
    {
        await Check();
    }
}

async Task Check()
{
    Clear();
    DrawLogo();

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

    // ask the user if they want to upload the proxies.
    Console.ResetColor();
    string uploadProxies = GetInput("Do you want to upload the proxies? (y/n): ");

    if (uploadProxies == "y")
    {
        await Upload();
    }
}

async Task Find()
{

    // for every line in data.txt split it at the "|" and put it in a list of lists
    List<List<string>> information = new List<List<string>>();
    using (StreamReader file = new StreamReader("data.txt"))
    {
        string line;
        while ((line = file.ReadLine()) != null)
        {
            List<string> siteInstructions = new List<string>(line.Split("|").Select(item => item.Trim()));
            information.Add(siteInstructions);
        }
    }

    Console.WriteLine(string.Join(Environment.NewLine, information.Select(line => string.Join(" | ", line))));

    foreach (List<string> data in information)
    {
        string site = GetSiteContent(data[0]);
        List<string> siteLines = site.Split('\n').ToList();

        siteLines = FilterLines(data[1], siteLines);
        siteLines = RemoveFromLines(data[2], siteLines);
        siteLines = RemoveFromLines(data[3], siteLines);
        List<List<string>> siteData = SplitLines(data[4], siteLines);
        siteData = FilterList(siteData);

        foreach (List<string> item in siteData)
        {
            Console.WriteLine(string.Join(Environment.NewLine, item));
            // get the site content of data[5] + item[0]
            string itemSite = GetSiteContent(data[5] + item[0]);
            // append the lines of the content to unchecked.txt but only if they start with a number
            for (int i = 0; i < itemSite.Split('\n').Length; i++)
            {
                if (itemSite.Split('\n')[i].StartsWith("1") || itemSite.Split('\n')[i].StartsWith("2") || itemSite.Split('\n')[i].StartsWith("3") || itemSite.Split('\n')[i].StartsWith("4") || itemSite.Split('\n')[i].StartsWith("5") || itemSite.Split('\n')[i].StartsWith("6") || itemSite.Split('\n')[i].StartsWith("7") || itemSite.Split('\n')[i].StartsWith("8") || itemSite.Split('\n')[i].StartsWith("9") || itemSite.Split('\n')[i].StartsWith("0"))
                {
                    File.AppendAllText("unchecked.txt", itemSite.Split('\n')[i] + "\n");
                }
            }

        }
    }
    Console.ReadLine();
}

static string GetSiteContent(string site)
{
    using (WebClient client = new WebClient())
    {
        return client.DownloadString(site);
    }
}

static List<string> FilterLines(string startsWith, List<string> lines)
{
    List<string> output = new List<string>();
    foreach (string line in lines)
    {
        if (line.StartsWith(startsWith))
        {
            output.Add(line);
        }
    }
    return output;
}

static List<string> RemoveFromLines(string remove, List<string> lines)
{
    List<string> output = new List<string>();
    foreach (string line in lines)
    {
        output.Add(line.Replace(remove, ""));
    }
    return output;
}

static List<List<string>> SplitLines(string split, List<string> lines)
{
    List<List<string>> output = new List<List<string>>();
    foreach (string key in lines)
    {
        output.Add(key.Split(new string[] { split }, StringSplitOptions.None).ToList());
    }
    return output;
}

static List<List<string>> FilterList(List<List<string>> list)
{
    List<List<string>> output = new List<List<string>>();
    foreach (List<string> item in list)
    {
        // if the phrases "http", "socks", "prox" are not in the item[1].ToLower() then append it to the output list
        if (item[1].ToLower().Contains("http") || item[1].ToLower().Contains("socks") || item[1].ToLower().Contains("prox"))
        {
            output.Add(item);
        }
    }
    return output;
}


while (true)
{
    await main();
}