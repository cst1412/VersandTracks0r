table! {
    shipmentprogress (id) {
        id -> Int4,
        shipment_id -> Int4,
        location -> Varchar,
        long -> Float8,
        lat -> Float8,
        message -> Varchar,
        shipment_status -> Int4,
        created_on -> Timestamp,
        updated_on -> Nullable<Timestamp>,
    }
}

table! {
    shipments (id) {
        id -> Int4,
        tracking_id -> Varchar,
        title -> Varchar,
        carrier -> Int4,
        manual_entry -> Bool,
        shipment_status -> Int4,
        deleted -> Bool,
        created_on -> Timestamp,
        updated_on -> Nullable<Timestamp>,
    }
}

joinable!(shipmentprogress -> shipments (shipment_id));

allow_tables_to_appear_in_same_query!(
    shipmentprogress,
    shipments,
);
