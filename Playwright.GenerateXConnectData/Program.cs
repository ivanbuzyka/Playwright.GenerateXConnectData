// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Microsoft.Playwright;

#region constants
// the page to call
const string hostName = "https://hostname/";

// the page for ending session that helps to speed up flushing sessions from session state to the xConnect
// the example for Sitecore 9 can be found here: https://gist.github.com/ivanbuzyka/94780aca7f5f90d83e4795c0d6670617
const string endSessionPage = $"{hostName}flush.aspx";

// how many interactions to create for a contact
const int interactionsCount = 5;

// how many contacts (unique visitors) will be created
const int contactsCount = 300;

// Sitecore Global Analytics cookie name
const string gACookieName = "SC_ANALYTICS_GLOBAL_COOKIE";
#endregion

using var playwright = await Playwright.CreateAsync();

// create headless Chromium browser
await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });

// new browser context to be able to work with cookies
var options = new BrowserNewContextOptions()
{
  ExtraHTTPHeaders = new Dictionary<string, string>(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("X-Forwarded-For", "103.4.96.156") })
};
var context = await browser.NewContextAsync(options);

//await RunContacts(browser);
await RunParallel(browser);
//await context.SetExtraHTTPHeadersAsync(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("X-Forwarded-For", "103.4.96.156") });

// var page = await context.NewPageAsync();

//do the very first request in order to get Global Analytics Cookie;
// var resp = await page.GotoAsync(hostName, new PageGotoOptions() { WaitUntil = WaitUntilState.NetworkIdle });
// var respstring = await resp.TextAsync();

// trick Sitecore Visitor Identitfication by running mouse move event (to make contact go through Sitecore robot detection)
// await page.Mouse.MoveAsync(20, 20);

// save SC_ANALYTICS_GLOBAL_COOKIE cookie
// var gAcookie = (await context.CookiesAsync()).Where(c => c.Name.Equals(gACookieName)).First();

// Console.WriteLine($"Current Cookie: {gAcookie.Name}: {gAcookie.Value}");

// var resp2 = await page.GotoAsync(endSessionPage, new PageGotoOptions() { WaitUntil = WaitUntilState.NetworkIdle });
// var resp2string = await resp2.TextAsync();

//var cookie = new Cookie()
//{
//    Name = gAcookie.Name,
//    Domain = gAcookie.Domain,
//    Path = gAcookie.Path,
//    Expires = gAcookie.Expires,
//    HttpOnly = gAcookie.HttpOnly,
//    SameSite = gAcookie.SameSite,
//    Secure = gAcookie.Secure,
//    Value = gAcookie.Value
//};

//close browser context to remove cookies and clean it up
// await context.CloseAsync();

// ToDo: here to add nested 'for' for generating certain amount of contacts with certain amount of interactions

//for (var i = 0; i < interactionsCount; i++)
//{
//    context = await browser.NewContextAsync();
//    await context.SetExtraHTTPHeadersAsync(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("X-Forwarded-For", "103.4.96.156") });

//    // adding saved Sitecore Global Analytics cookie to the browser context
//    await context.AddCookiesAsync(new List<Cookie>() {
//        cookie
//    });

//    // creating new page
//    page = await context.NewPageAsync();

//    // call page, wait for network response is received and the document started loading
//    await page.GotoAsync(hostName, new PageGotoOptions() { WaitUntil = WaitUntilState.Commit });

//    Console.WriteLine($"Interaction #{i}, calling {hostName}");

//    // call end session page (to speed up flushing data to xConnect DB)
//    await page.GotoAsync(endSessionPage, new PageGotoOptions() { WaitUntil = WaitUntilState.Commit });

//    Console.WriteLine($"Ending session, calling {endSessionPage}");

//    await context.CloseAsync();
//}

//async version for parallel running

//List<Task> tasks = new();
//for (var i = 0; i < interactionsCount; i++)
//{
//    tasks.Add(CallPageWithKnownGACookie(i, hostName, endSessionPage, cookie, browser));
//}

//await Task.WhenAll(tasks);

//async Task CallPageWithKnownGACookie(int taskId, string pageUrl, string endSessionPageUrl, Cookie gACookie, IBrowser browser)
//{
//    var context = await browser.NewContextAsync();
//    await context.AddCookiesAsync(new List<Cookie>()
//    {
//        gACookie
//    });

//    // creating new page
//    var page = await context.NewPageAsync();

//    // call page, wait for network response is received and the document started loading
//    var resp = await page.GotoAsync(pageUrl, new PageGotoOptions() { WaitUntil = WaitUntilState.Load });

//    Console.WriteLine($"Interaction for task #{taskId}, calling {pageUrl}");
//    Console.WriteLine($"Status: {resp.Status}");

//    //await Task.Delay(500);

//    // call end session page (to speed up flushing data to xConnect DB)
//    var resp1 = await page.GotoAsync(endSessionPageUrl, new PageGotoOptions() { WaitUntil = WaitUntilState.Load });

//    Console.WriteLine($"Ending session for task #{taskId}, calling {endSessionPageUrl}");
//    Console.WriteLine($"Status: {resp1.Status}");

//    await context.CloseAsync();
//}

static async Task RunContacts(IBrowser browser)
{
  for (var i = 0; i < contactsCount; i++)
  {
    var newContext = await browser.NewContextAsync();
    await newContext.SetExtraHTTPHeadersAsync(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("X-Forwarded-For", "103.4.96.156") });

    // creating new page
    var newPage = await newContext.NewPageAsync();

    // call page, wait for network response is received and the document started loading
    await newPage.GotoAsync(hostName, new PageGotoOptions() { WaitUntil = WaitUntilState.NetworkIdle });

    // trick Sitecore Visitor Identitfication by running mouse move event (to make contact go through Sitecore robot detection)
    await newPage.Mouse.MoveAsync(20, 20);
    await newPage.Mouse.MoveAsync(-20, 20);
    await newPage.Mouse.MoveAsync(-20, -20);

    Console.WriteLine($"Contact #{i}, calling {hostName}");

    // call end session page (to speed up flushing data to xConnect DB)
    await newPage.GotoAsync(endSessionPage, new PageGotoOptions() { WaitUntil = WaitUntilState.NetworkIdle });

    Console.WriteLine($"Ending session, calling {endSessionPage}");

    await newContext.CloseAsync();
  }
}

static async Task RunParallel(IBrowser browser)
{
  for (var z = 0; z < 30; z++)
  {
    var result = Parallel.For(1, 5, async (i, state) =>
    {
      var newContext = await browser.NewContextAsync();
      await newContext.SetExtraHTTPHeadersAsync(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("X-Forwarded-For", "103.4.96.156") });

      // creating new page
      var newPage = await newContext.NewPageAsync();

      // call page, wait for network response is received and the document started loading
      await newPage.GotoAsync(hostName, new PageGotoOptions() { WaitUntil = WaitUntilState.NetworkIdle });

      // trick Sitecore Visitor Identitfication by running mouse move event (to make contact go through Sitecore robot detection)
      await newPage.Mouse.MoveAsync(20, 20);
      await newPage.Mouse.MoveAsync(-20, 20);
      await newPage.Mouse.MoveAsync(-20, -20);

      Console.WriteLine($"Contact #{z}:{i}, calling {hostName}");

      // call end session page (to speed up flushing data to xConnect DB)
      await newPage.GotoAsync(endSessionPage, new PageGotoOptions() { WaitUntil = WaitUntilState.NetworkIdle });

      Console.WriteLine($"Ending session, calling {endSessionPage}");

      await newContext.CloseAsync();      
    });

    Thread.Sleep(8000);
  }
}
