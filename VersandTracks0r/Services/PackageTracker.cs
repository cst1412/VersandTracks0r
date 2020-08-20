using System;
using System.Collections.Generic;
using System.Linq;
using VersandTracks0r.Models;

namespace VersandTracks0r.Services
{

    public static class PackageTracker
    {
        public static void track(Shipment shipment, IEnumerable<IShipmentTracker> trackers,AppSettings appSettings,GeoCoder geoCoder)
        {
            if (shipment.Status == ShipmentStatus.Done || shipment.Status == ShipmentStatus.Invalid)
            {
                return;
            }

            //wurde innerhalb der letzten 5min geupdatet also skippen
            if (shipment.UpdatedAt.AddMinutes(5) > DateTime.Now && shipment.HasData)
            {
                Console.WriteLine("ALREADY UPDATED");
                Console.WriteLine(shipment.Id);
                Console.WriteLine(shipment.TrackingId);
                Console.WriteLine(shipment.UpdatedAt);
                Console.WriteLine(shipment.Status);
                return;
            }

            try
            {
                var tracker = trackers.FirstOrDefault(t => t.SupportsCarrier(shipment.Carrier));

                if (tracker == null)
                {
                    return;
                }

                Console.WriteLine(shipment.TrackingId);
                var history = tracker.Track(shipment.TrackingId);

                if (history.FirstOrDefault(h => h.Status == ShipmentStatus.Done) is ShipmentProgress p)
                {
                    if (p.Location == string.Empty)
                    {
                        p.Location = appSettings.DefaultLocation;
                    }
                }

                // geocode location (ort zu längen und breitengrad)
                foreach (var entry in history)
                {
                    if (geoCoder.TryLookupCoordinates(entry.Location, out var lon, out var lat))
                    {
                        entry.Lat = lat;
                        entry.Long = lon;
                    }
                }

                var hasInvalid = history.Any(h => h.Status == ShipmentStatus.Invalid);

                // invalid nur reinschreiben wenn leer
                if (hasInvalid && shipment.History.Count == 0)
                {
                    shipment.History.AddRange(history);
                }
                // nicht überschreiben mit was kaputtenem wenn  was drin ist
                // wenn invalid und vorher schon was drin war dann nix machen
                else if (!hasInvalid)
                {
                    // gucken ob das ding schon drin is dann nicht löschen
                    shipment.History.Clear();
                    shipment.History.AddRange(history);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            shipment.UpdatedAt = DateTime.Now;

            if (!shipment.Manual && shipment.Status == ShipmentStatus.Invalid)
            {
                shipment.IsDeleted = true;
            }
        }
    }
}