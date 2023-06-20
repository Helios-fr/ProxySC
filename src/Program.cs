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

// function to get the remote instructions from pastebin  split them by line then split them by | to create a list of lists
async Task<List<List<string>>> GetInstructions()
{
    string raw = await GetResponse("https://raw.githubusercontent.com/Nyxqxx/ProxySC/main/site.patterns");
    string[] lines = raw.Split("\n");
    List<List<string>> instructions = new List<List<string>>();
    foreach (string line in lines)
    {
        instructions.Add(line.Split("|").ToList());
    }
    return instructions;
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
    // Get the instructions from the remote
    List<List<string>> instructions = await GetInstructions();

    foreach (List<string> i in instructions)
    {
        // Get the page
        WebClient client = new WebClient();
        string page = await client.DownloadStringTaskAsync(i[0]);

        // Split the page by the first instruction
        string[] splitPage = page.Split(new string[] { i[1] }, StringSplitOptions.None);

        // Remove the first element
        List<string> pageList = new List<string>(splitPage);
        pageList.RemoveAt(0);

        // Split the page by the second instruction
        pageList = pageList.ConvertAll(x => x.Split(new string[] { i[2] }, StringSplitOptions.None)[0]);

        // Split every element by the third instruction then turn the list into a list of lists
        List<string[]> pageLists = new List<string[]>();
        foreach (string element in pageList)
        {
            string[] splitElement = element.Split(new string[] { i[3] }, StringSplitOptions.None);
            pageLists.Add(splitElement);
        }

        // If the i[0] element has an '&' in it, remove the i element from the list
        pageLists.RemoveAll(item => item[0].Contains("&"));

        // print all the tit

        // Sort the list to only contain pastes that have the word "prox" in them convert the titles to lowercase first
        pageLists.RemoveAll(item => !item[1].ToLower().Contains("prox"));
        // append the 6th instruction to the beginning the first element of every list
        pageLists = pageLists.ConvertAll(x => new string[] { i[4] + x[0] });


        // Print the list
        foreach (string[] item in pageLists)
        {
            Console.WriteLine(string.Join(", ", item));
        }
    }
    Console.WriteLine("Done");
    Console.ReadLine();
}

while (true)
{
    await main();
}