using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EventCreator.Models;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace EventCreator.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    static string[] Scopes = { CalendarService.Scope.Calendar };

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> CreateEvent()
    {
        return View();
    }

    [HttpPost]
    public async Task<Event> CreateEvents(int create)
    {
        Console.WriteLine($"Text");


        UserCredential credential;
        using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
        {
            string credentialsPath = "tokens.json";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credentialsPath, true)).Result;
        }

        Console.WriteLine($"Text");
        // Create the Calendar service
        var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Event Creator App",
        });

        Console.WriteLine($"Text");

        Event newEvent = new Event()
        {
            Summary = "Google I/O 2015",
            Location = "800 Howard St., San Francisco, CA 94103",
            Description = "A chance to hear more about Google's developer products.",
            Start = new EventDateTime()
            {
                DateTime = DateTime.Parse("2023-05-28T09:00:00-07:00"),
                TimeZone = "America/Los_Angeles",
            },
            End = new EventDateTime()
            {
                DateTime = DateTime.Parse("2023-05-28T17:00:00-07:00"),
                TimeZone = "America/Los_Angeles",
            },
            Recurrence = new String[] { "RRULE:FREQ=DAILY;COUNT=2" },
            Attendees = new EventAttendee[] {
                            new EventAttendee() { Email = "lpage@example.com" },
                            new EventAttendee() { Email = "sbrin@example.com" },
                        },
            Reminders = new Event.RemindersData()
            {
                UseDefault = false,
                Overrides = new EventReminder[] {
                                new EventReminder() { Method = "email", Minutes = 24 * 60 },
                                new EventReminder() { Method = "sms", Minutes = 10 },
                            }
            }
        };

        Console.WriteLine($"Text");

        // Load the credentials from a client_secret.json file


        String calendarId = "primary";
        EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
        Console.WriteLine($"Text");
        Event createdEvent = request.Execute();
        Console.WriteLine($"Text");
        Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);
        Console.WriteLine($"Text");

        return createdEvent;
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
