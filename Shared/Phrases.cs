﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Shared.Models;
using Shared.Models.Helpers;
using System.Collections.Generic;

namespace Shared
{
    public static class Phrases
    {
        public const string ProjectName = "CORA";
        public const string ProjectWebsite = "Bing.com/CORA";
        public static List<string> ValidChannels = new List<string>() { Channels.Emulator, Channels.Sms, Channels.Webchat };

        public static string EnterNumber = "(enter a number)";
        public static string None = "None";

        public static class Exceptions
        {
            public static string Error = $"Sorry, it looks like something went wrong";
        }

        public static class Greeting
        {
            public static Activity Welcome = MessageFactory.Text("Welcome back!");
            public static Activity WelcomeNew = MessageFactory.Text($"Hi, I'm {ProjectName}, a bot helping to locate critical " +
                $"supplies in case of an emergency. Message and data rates apply. Would you like to continue? {EnterNumber}");

            public static Activity Consent = MessageFactory.Text($"Great! As detailed on {ProjectWebsite}, my job is to locate critical " +
                $"supplies in case of an emergency and notify you if they are needed by local healthcare facilities. Throughout " +
                $"this process I will protect your privacy and not share your phone number");
            public static Activity NoConsent = MessageFactory.Text("No problem! You can message me any time if you change your mind");
            public static Activity Registered = MessageFactory.Text("You are now registered to help provide critical resources in case of an emergency!");

            public static Activity InvalidChannel(ITurnContext turnContext)
            {
                return MessageFactory.Text($"Channel \"{turnContext.Activity.ChannelId}\" is not yet supported");
            }

            public static Activity InvalidSchema(string error)
            {
                return MessageFactory.Text($"The schema for this bot is invalid: \"{error}\"");
            }
        }

        public static class Options
        {
            public static string Request = "Request resources";
            public static string Provide = "Register new resources";
            public static string Update = "Update existing resources";
            public static string MoreOptions = "More options";

            public static Activity GetOptions = MessageFactory.Text($"Here's what you can do {EnterNumber}");

            public static List<string> GetOptionsList(bool isVerifiedOrganization, bool hasResources)
            {
                var list = new List<string>();

                if (isVerifiedOrganization)
                {
                    list.Add(Request);
                }

                list.Add(Provide);

                if (hasResources)
                {
                    list.Add(Update);
                }

                list.Add(MoreOptions);
                return list;
            }

            public static class Extended
            {
                public static string ViewResources = "View your resource availability";
                public static string UpdateLocation = "Update your location";
                public static string ChangeDays = $"Change the days that {ProjectName} will contact you for updates";
                public static string ChangeTime = $"Change the time that {ProjectName} will contact you for updates";
                public static string Enable = $"Enable {ProjectName} to contact you";
                public static string Disable = $"Stop {ProjectName} from contacting you";
                public static string Language = "Change your language";
                public static string Feedback = "Provide feedback";
                public static string GoBack = "Go back to the main menu";

                public static Activity GetOptions = MessageFactory.Text($"Here are some more options {EnterNumber}");
                public static Activity NoResources = MessageFactory.Text($"it looks like you haven't added any resources yet. You can add them from the main menu");

                public static List<string> GetOptionsList(User user)
                {
                    var list = new List<string> { ViewResources, UpdateLocation, ChangeDays, ChangeTime };
                    list.Add(user.ContactEnabled ? Disable : Enable);
                    list.AddRange(new string[] { Language, Feedback, GoBack });
                    return list;
                }

                public static Activity GetResourceAvailability(List<Resource> resources)
                {
                    if (resources.Count == 0)
                    {
                        return NoResources;
                    }

                    var text = "Here are your available resources:" + Helpers.NewLine;

                    foreach (var resource in resources)
                    {
                        // Add a newline if there is already some text.
                        text += string.IsNullOrEmpty(text) ? string.Empty : Helpers.NewLine;
                        text += $"{resource.Name}: {resource.Quantity}";
                    }

                    return MessageFactory.Text(text);
                }
            }
        }

        public static class Preferences
        {
            private const string GetCurrentTimeFormat = "\"h:mm am/pm\"";
            private const string GetUpdateTimeFormat = "\"h am/pm\"";
            private const string GetUpdateDaysFormat = "\"M,T,W,Th,F,Sa,Su\"";

            private const string GetCurrentTimeExample = "You can say things like \"8:30 am\" or \"12:15 pm\"";
            private const string GetUpdateTimeExample = "You can say things like \"8 am\" or \"12 pm\"";
            private const string GetUpdateDaysExample = "You can say things like \"M,W,F\", \"weekdays\", \"weekends\", or \"everyday\"";

            private const string PreferenceUpdated = "Your contact preference has been updated";

            public static Activity GetLocation = MessageFactory.Text("Where are you located? (enter City, State/Province, Country)");
            public static Activity GetLocationRetry = MessageFactory.Text($"Oops, I couldn't find that location. Please try again...");
            public static Activity LocationUpdated = MessageFactory.Text("Your location has been updated!");

            public static Activity GetCurrentTime = MessageFactory.Text($"What time is it for you currently? This is to determine your timezone. {GetCurrentTimeExample}");
            public static Activity GetCurrentTimeRetry = MessageFactory.Text($"Oops, the format is {GetCurrentTimeFormat}. {GetCurrentTimeExample}");

            public static Activity GetUpdateTime = MessageFactory.Text($"Which hour of the day would you like to be contacted? {GetUpdateTimeExample}");
            public static Activity GetUpdateTimeRetry = MessageFactory.Text($"Oops, the format is {GetUpdateTimeFormat}. {GetUpdateTimeExample}");

            public static Activity GetUpdateDays = MessageFactory.Text($"Which days of the week would you like to be contacted? {GetUpdateDaysExample}");
            public static Activity GetUpdateDaysRetry = MessageFactory.Text($"Oops, the format is {GetUpdateDaysFormat}. {GetUpdateDaysExample}");

            public static Activity GetLanguage = MessageFactory.Text($"Say \"hello\" in the language you prefer and I will switch to that language for you");
            public static Activity LanguageUpdated = MessageFactory.Text($"Your language has been updated!");

            public static Activity GetLocationConfirm(string location)
            {
                return MessageFactory.Text($"I matched your city to \"{location}\". Is this correct? {EnterNumber}");
            }

            public static Activity UpdateTimeUpdated(string time)
            {
                return MessageFactory.Text($"{PreferenceUpdated} to {time}!");
            }

            public static Activity UpdateDaysUpdated(DayFlags days)
            {
                return MessageFactory.Text($"{PreferenceUpdated} to {days.ToString()}!");
            }

            public static Activity ContactEnabledUpdated(bool contactEnabled)
            {
                return MessageFactory.Text($"{PreferenceUpdated}!");
            }
        }

        public static class Provide
        {
            public static Activity GetCategory = MessageFactory.Text($"Which category of resource are you able to provide? {EnterNumber}");
            public static Activity Another = MessageFactory.Text($"Would you like to register another resource? {EnterNumber}");
            public static Activity CompleteUpdate = MessageFactory.Text("Thank you for the update!");

            public static Activity GetResource(string category)
            {
                return MessageFactory.Text($"Which type of {category.ToLower()} are you able to provide? {EnterNumber}");
            }

            public static Activity GetQuantity(string resource)
            {
                return MessageFactory.Text($"What quantity of {resource} do you have available? {EnterNumber}");
            }

            public static Activity CompleteCreate(User user)
            {
                var text = "Thank you for making your resources available to the community! I will reach out if a need arises that matches your resources.";
                
                if (user.ContactEnabled)
                {
                    text += $" I will also contact you *{user.ReminderFrequency.ToString()}* to update your availability. This frequency can be customized from the options menu";
                }
                else
                {
                    text += $" Your contact preference is disabled, so please make sure to update your availability as it changes!";
                }

                return MessageFactory.Text(text);
            }

            public static class Update
            {
                public static Activity GetResource = MessageFactory.Text($"What resource would you like to update? {EnterNumber}");
                public static Activity Another = MessageFactory.Text($"Would you like to update another resource? {EnterNumber}");
            }
        }

        public static class Request
        {
            public static Activity Categories = MessageFactory.Text("Which category of resource are you looking for?");
            public static Activity Instructions = MessageFactory.Text("What are your instructions for contact or delivery? You can include things like a phone number or a location");
            public static Activity Sent = MessageFactory.Text("Your request has been sent! You will be contacted directly by anyone who responds to your request");

            public static Activity Resources(string category)
            {
                return MessageFactory.Text($"Which type of {category.ToLower()} are you looking for? {EnterNumber}");
            }

            public static Activity GetQuantity(string resource)
            {
                return MessageFactory.Text($"What quantity of {resource} do you need? {EnterNumber}");
            }

            public static Activity Distance(SchemaUnits units)
            {
                return MessageFactory.Text($"What distance would you like to broadcast your request? (enter {units.ToString().ToLower()})");
            }

            public static string GetOutgoingMessage(string name, string resource, int quantity, string instructions)
            {
                return $"{name} has an immediate need for {quantity} {resource}. " +
                    $"Here are their instructions: \"{instructions}\"";
            }
        }

        public static class Feedback
        {
            public static Activity GetFeedback = MessageFactory.Text($"What would you like to let the {ProjectName} team know?");
            public static Activity Thanks = MessageFactory.Text("Thanks for the feedback!");
        }
    }
}