extern crate diesel;
extern crate versandtracks0r_api_rs;

use self::diesel::prelude::*;
use self::models::*;
use self::versandtracks0r_api_rs::*;

pub fn get_shipments(conn: &PgConnection) -> Vec<ShipmentViewModel> {
    use versandtracks0r_api_rs::schema::shipmentprogress::dsl::*;
    use versandtracks0r_api_rs::schema::shipments::dsl::*;

    let _results = shipments
        .filter(deleted.eq(false))
        .load::<Shipment>(conn)
        .expect("Error loading Shipments");

    let ids: Vec<i32> = (&_results).into_iter().map(|i| i.id).collect::<Vec<i32>>();
    let _shipmentprogresses = shipmentprogress
        .filter(shipment_id.eq_any(ids))
        .load::<ShipmentProgress>(conn)
        .expect("Error loading Progresses");

    return _results
        .into_iter()
        .map(|item| ShipmentViewModel {
            id: item.id,
            carrier: item.carrier,
            deleted: item.deleted,
            tracking_id: item.tracking_id,
            title: item.title,
            created_on: item.created_on,
            updated_on: item.updated_on,
            manual_entry: item.manual_entry,
            shipment_status: item.shipment_status,
            history: Default::default(),
        })
        .collect::<Vec<ShipmentViewModel>>();
}

fn get_shipment_progress_by_shipment_id(
    items: &Vec<ShipmentProgress>,
    id: &i32,
) -> Vec<ShipmentProgress> {
    return items
        .into_iter()
        .filter(|sp| sp.shipment_id.eq(id))
        .collect();
}
