using HtmlAgilityPack;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VersandTracks0r.Models;

namespace VersandTracks0r.Services
{
    public class IMAPMailScanner
    {
        private readonly Regex dhl = new Regex(@"(\d{20}|JD\d{18}|JJD\d{18})");
        private readonly Regex dhlDetailRegex = new Regex(@"(\d{12}|\d{20}|JD\d{18}|JJD\d{18})");
        private readonly Regex ups = new Regex(@"(1Z\w{16})");
        private readonly Regex amazon = new Regex(@"(A\w{2}\d{9})", RegexOptions.IgnoreCase);
        private readonly Regex dpd = new Regex(@"(0\d{13})");
        private readonly Regex hermes = new Regex(@"(\d{14})");
        private readonly AppSettings appSettings;

        private DateTime lastScan = DateTime.Today.Subtract(TimeSpan.FromDays(30));

        public event Action<Shipment> Found;

        public IMAPMailScanner()
        {
            this.appSettings = new AppSettings();
        }

        public void Scan()
        {
            using var client = new ImapClient();

            client.Connect(this.appSettings.Server, 993, true);

            var config = new AppSettings();

            var username = config.Account;
            var pw = config.Password;

            client.Authenticate(username, pw);

            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadOnly);

            var result = inbox.Search(SearchQuery.DeliveredAfter(this.lastScan));
            this.lastScan = DateTime.Now.Subtract(TimeSpan.FromMinutes(5));

            var messages = result.Select(id => inbox.GetMessage(id)).OrderByDescending(msg => msg.Date);

            foreach (var message in messages)
            {
                Console.WriteLine(message.Subject);
                this.Parse(message);
            }
        }

        public void Parse(MimeMessage message)
        {
            if (message.TextBody is string || message.HtmlBody is string)
            {
                string text = message.TextBody;
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = message.HtmlBody;
                }
                text = text.ToLower();
                var address = message.From.Mailboxes.First().Address;
                var domain = address.Split('@').Last();

                if (message.Subject.Contains("MediaMarkt"))
                {
                    var d = "";
                }

                if (this.checkMatchesForCarriers(text, domain))
                {
                    return;
                }

                Regex regex = new Regex("<a\\s+(?:[^>]*?\\s+)?href=([\"'])(.*?)\\1", RegexOptions.IgnoreCase);
                Match match;
                for (match = regex.Match(text); match.Success; match = match.NextMatch())
                {
                    Console.WriteLine("Found a href. Groups: ");
                    foreach (Group group in match.Groups)
                    {
                        Console.WriteLine(group.Value);
                        var doc = new HtmlDocument();
                        doc.LoadHtml(group.Value);
                        var anchor = doc.DocumentNode.FirstChild;
                        string href ="";
                        if (anchor != null && anchor.Attributes.Count > 0 && anchor.Attributes["href"] != null)
                        {
                            href = anchor.Attributes["href"].Value;
                        }
                        if (!string.IsNullOrWhiteSpace(href))
                        {
                            Uri url;
                            bool parseResult = Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out url);
                            if (parseResult)
                            {
                                Console.WriteLine(href);
                                if (this.checkMatchesForCarriers(url.ToString(), domain, true))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }


            }
        }

        private bool checkMatchesForCarriers(string text, string domain, bool useDetailRegex = false)
        {
            var shipment = new Shipment
            {
                CreatedAt = DateTime.Now,
                Comment = domain,
            };

            var test = this.dhl.Matches(text);
            if (this.dhl.Match(text) is Match dhlMatch && dhlMatch.Success && text.Contains("dhl"))
            {
                shipment.Carrier = Carrier.DHL;
                shipment.TrackingId = dhlMatch.Groups[0].Value;
                this.Found?.Invoke(shipment);
                return true;
            }
            else if (this.ups.Match(text) is Match upsMatch && upsMatch.Success && text.Contains("ups"))
            {
                shipment.Carrier = Carrier.UPS;
                shipment.TrackingId = upsMatch.Groups[0].Value;
                this.Found?.Invoke(shipment);
                return true;

            }
            else if (this.dpd.Match(text) is Match dpdMatch && dpdMatch.Success && text.Contains("dpd"))
            {
                shipment.Carrier = Carrier.DPD;
                shipment.TrackingId = dpdMatch.Groups[0].Value;
                this.Found?.Invoke(shipment);
                return true;

            }
            else if (this.hermes.Match(text) is Match hermesMatch && hermesMatch.Success && text.Contains("hermes"))
            {
                shipment.Carrier = Carrier.Hermes;
                shipment.TrackingId = hermesMatch.Groups[0].Value;
                this.Found?.Invoke(shipment);
                return true;

            }
            else if (this.amazon.Match(text) is Match amazonMatch && amazonMatch.Success && text.Contains("amazon"))
            {
                shipment.Carrier = Carrier.Amazon;
                shipment.TrackingId = amazonMatch.Groups[0].Value;
                this.Found?.Invoke(shipment);
                return true;

            }
            else if (useDetailRegex && this.dhlDetailRegex.Match(text) is Match dhlDetailRegex && dhlDetailRegex.Success
                    && (text.Contains("sendung") || text.Contains("verfolgen") || text.Contains("track")))
            {
                shipment.Carrier = Carrier.DHL;
                shipment.TrackingId = dhlDetailRegex.Groups[0].Value;
                this.Found?.Invoke(shipment);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
