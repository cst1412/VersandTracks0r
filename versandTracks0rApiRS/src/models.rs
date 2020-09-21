use chrono::NaiveDateTime;
#[derive(Queryable)]
pub struct Shipment {
    pub id: i32,
    pub tracking_id: String,
    pub title: String,
    pub carrier: i32,
    pub manual_entry: bool,
    pub shipment_status: i32,
    pub deleted: bool,
    pub created_on: NaiveDateTime,
    pub updated_on: Option<NaiveDateTime>,
}

#[derive(Queryable, Clone)]
pub struct ShipmentProgress {
    pub id: i32,
    pub shipment_id: i32,
    pub location: String,
    pub long: f64,
    pub lat: f64,
    pub message: String,
    pub shipment_status: i32,
    pub created_on: NaiveDateTime,
    pub updated_on: Option<NaiveDateTime>,
}

pub struct ShipmentViewModel {
    pub id: i32,
    pub tracking_id: String,
    pub title: String,
    pub carrier: i32,
    pub manual_entry: bool,
    pub shipment_status: i32,
    pub deleted: bool,
    pub created_on: NaiveDateTime,
    pub updated_on: Option<NaiveDateTime>,
    pub history: Vec<ShipmentProgress>,
}
