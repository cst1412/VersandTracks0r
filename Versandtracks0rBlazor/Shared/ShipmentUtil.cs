namespace Versandtracks0rBlazor.Shared
{
    public static class ShipmentUtil{
        public static string GetShipmentStatusIcon(Models.ShipmentStatus status){
         switch (status) {
            case Models.ShipmentStatus.Done:
              return "far fa-check-circle";
            case Models.ShipmentStatus.Transit:
              return "fas fa-plane-departure";
            case Models.ShipmentStatus.Delivery:
              return "fas fa-shuttle-van";
            case Models.ShipmentStatus.Invalid:
              return "far fa-times-circle";
            case Models.ShipmentStatus.Pickup:
              return "fas fa-exclamation-circle";
            default:
              return "fas fa-sync-alt";
          }
    }
    }
}