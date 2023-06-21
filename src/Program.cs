using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

// list of http proxy urls  + # + random number to prevent caching
string httpRaw = await GetResponse("https://raw.githubusercontent.com/Necrownyx/Proxy-Scraper-and-Checker/main/http.sites#" + new Random().Next(0, 999999999));
string[] httpUrls = httpRaw.Split("\n");

// list of socks4 proxy urls
string socks4Raw = await GetResponse("https://raw.githubusercontent.com/Necrownyx/Proxy-Scraper-and-Checker/main/socks4.sites#" + new Random().Next(0, 999999999));
string[] socks4Urls = socks4Raw.Split("\n");

// list of socks5 proxy urls
string socks5Raw= await GetResponse("https://raw.githubusercontent.com/Necrownyx/Proxy-Scraper-and-Checker/main/socks5.sites#" + new Random().Next(0, 999999999));
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
    // Set a failure timeout of 10 seconds
    TimeSpan timeoutDuration = TimeSpan.FromSeconds(10);

    using (var cancellationTokenSource = new CancellationTokenSource(timeoutDuration))
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync(url, cancellationTokenSource.Token);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch (OperationCanceledException)
        {
            // Request timed out
            throw new TimeoutException("The request timed out.");
        }
    }
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

    Console.WriteLine("\x1b[32m1.\x1b[0m Scrape Proxies");
    Console.WriteLine("\x1b[32m2.\x1b[0m Check Proxies");
    Console.WriteLine("\x1b[32m3.\x1b[0m Upload Proxies");
    Console.WriteLine("\x1b[33m4.\x1b[0m Find Proxies \x1b[31m* BETA *\x1b[0m");
    Console.WriteLine("\x1b[31m5.\x1b[0m Exit");
    string action = GetInput("Select an option: ");

    if (action == "1")
    {
        await Scrape();
    }
    else if (action == "2")
    {
        await Check();
    }
    else if (action == "3")
    {
        await Upload();
    }
    else if (action == "4")
    {
        await Find();
    }
    else if (action == "5")
    {
        Exit();
    }
    else
    {
        Console.WriteLine("Invalid action");
    }
}

object fileLock = new object();

async Task Scrape()
{
    // ask the user what type of proxy they want to scrape
    Clear();
    DrawLogo();

    if (File.Exists("unchecked.txt"))
    {
        string overwrite = GetInput("unchecked.txt already exists, do you want to overwrite it? (y/n): ");
        if (overwrite == "y")
        {
            File.Delete("unchecked.txt");
            // write the addvertisement to the top of the file
            File.AppendAllText("unchecked.txt", "📀   PROXYS SCRAPED USING PROXYSC   📀\n");
            File.AppendAllText("unchecked.txt", "📀 http://github.com/Nyxqxx/ProxySC 📀\n\n");
        }
    }

    string proxyType = GetInput("What type of proxy do you want to scrape? (http, socks4, socks5, all): ");

    string[] urls = { };
    if (proxyType == "http") { urls = httpUrls; }
    else if (proxyType == "socks4") { urls = socks4Urls; }
    else if (proxyType == "socks5") { urls = socks5Urls; }
    else if (proxyType == "all")
    {
        urls = httpUrls.Concat(socks4Urls).Concat(socks5Urls).ToArray();
    }
    else { Console.WriteLine("Invalid proxy type"); return; }

    // if yes, add the proxies from sites.txt to the urls array
    string useFound = GetInput("Do you want to use the found pastes from sites.txt? (y/n): ");
    if (useFound == "y")
    {
        foreach (string line in File.ReadAllLines("sites.txt"))
        {
            urls = urls.Append(line).ToArray();
        }
    }

    // function to scrape proxies from a url
    async Task ScrapeUrl(string url, bool filewrite)
    {
        // make a web request to the url and store the response in a string
        string response = "";
        try
        {
            response = await GetResponse(url);
        }
        catch
        {
            Console.WriteLine("\x1b[31m[-]\x1b[0m" + "Failed to scrape proxies from url: " + url);
            return;
        }

        // split the response string into an array of strings
        string[] proxies = response.Split("\n");

        // print the number of proxies scraped from the url in green
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\x1b[32m[+]\x1b[0m" + "Scraped " + proxies.Length + " proxies from " + url);

        // loop through all the proxies in the proxies array and write them to unchecked.txt if they start with a number
        string finalProxies = "";
        foreach (string proxy in proxies)
        {
            if (proxy.StartsWith("1") || proxy.StartsWith("2") || proxy.StartsWith("3") || proxy.StartsWith("4") || proxy.StartsWith("5") || proxy.StartsWith("6") || proxy.StartsWith("7") || proxy.StartsWith("8") || proxy.StartsWith("9") || proxy.StartsWith("0"))
            {
                finalProxies += proxy + "\n";
            }
        }
        // wait for the file lock to be released
        lock (fileLock)
        {
            // write the proxies to unchecked.txt
            File.AppendAllText("unchecked.txt", finalProxies);
        }
    }

    // ask the user if they want to use threads
    string useThreads = GetInput("Do you want to use threads? (y/n): ");
    if (useThreads == "y")
    {
        // set the thread count to the number of urls if there are fewer than 100 urls
        int threadCount = urls.Length;
        bool filewrite = true;
        if (urls.Length > 100)
        {
            // ask the user how many threads they want to use
            threadCount = Convert.ToInt32(GetInput("How many threads do you want to use? (100+): "));
        }

        // create a list of tasks
        List<Task> tasks = new List<Task>();

        // create a lock object to synchronize file access
        object fileLock = new object();

        // loop through the urls array and start a new task for each url
        foreach (string url in urls)
        {
            tasks.Add(ScrapeUrl(url, filewrite));
        }

        // wait for all the tasks to finish
        await Task.WhenAll(tasks);
    }
    else
    {
        // loop through the urls array and scrape each url
        foreach (string url in urls)
        {
            await ScrapeUrl(url, true);
        }
    }

    // remove all duplicate proxies from unchecked.txt and write them back to the file
    string[] lines = File.ReadAllLines("unchecked.txt");
    string[] uniqueLines = lines.Distinct().ToArray();
    File.WriteAllLines("unchecked.txt", uniqueLines);

    // print the number of unique proxies scraped in green
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Scraped " + uniqueLines.Length + " unique proxies");

    // ask the user if they want to check the proxies
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

    if (File.Exists("checked.txt")) {
        string delete = GetInput("Do you want to delete the checked.txt file? (y/n): ");
        if (delete == "y")
        {
            File.Delete("checked.txt");
            // create the checked.txt file and append the advertising text to it
            File.AppendAllText("checked.txt", "📀 PROXYS SCRAPED AND CHECKED USING PROXYSC 📀\n");
            File.AppendAllText("checked.txt", "📀     http://github.com/Nyxqxx/ProxySC     📀\n\n");
        }
    }

    // function to check a single proxy
    async Task CheckProxy(string proxy)
    {
        string[] proxyParts = proxy.Split(":");
        // try to connect to
        try
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.Proxy = new WebProxy(proxyParts[0], int.Parse(proxyParts[1]));
            HttpClient client = new HttpClient(handler);
            string response = await client.GetStringAsync("https://api.ipify.org/");

            // write the proxy to checked.txt if the request was successful
            File.AppendAllText("checked.txt", proxy + "\n");

            Console.WriteLine("\x1b[32m[+]\x1b[0m " + proxy + " - Working");
        }
        catch
        {
            Console.WriteLine("\x1b[31m[-]\x1b[0m " + proxy + " - Not Working");
        }
    }

    // ask the user how many threads they want to use
    int threads = 10000000;
    try { threads = int.Parse(GetInput("How many threads do you want to use? (recommended: 1000-inf): ")); }
    catch {}

    // ask the user how many file reccuisons they want to use
    int recursions = int.Parse(GetInput("How many file recursions do you want? (recommended: 3 - 10): "));
    string[] proxiesToCheck =  File.ReadAllLines("unchecked.txt");
    for (int i = 0; i < recursions; i++)
    {
        proxiesToCheck = proxiesToCheck.Concat(proxiesToCheck).ToArray();
    }


    List<Task> tasks = new List<Task>();
    foreach (string proxy in proxiesToCheck)
    {
        tasks.Add(CheckProxy(proxy));
        // if the amount of tasks in the tasks list is equal to the amount of threads the user wants to use, wait for a task to finish and remove it from the list
        if (tasks.Count == threads)
        {
            await Task.WhenAny(tasks);
            tasks.RemoveAt(0);
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

        // append the urls found to the sites.txt file
        foreach (string[] item in pageLists) {
            File.AppendAllText("sites.txt", string.Join(",", item) + Environment.NewLine);
        }
    }
    Console.ReadLine();
}

while (true)
{
    await main();
}