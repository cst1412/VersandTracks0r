#![feature(proc_macro_hygiene, decl_macro)]
use self::versandtracks0r_api_rs::*;
#[macro_use]
extern crate rocket;
extern crate diesel;
extern crate versandtracks0r_api_rs;

mod shipments_service;

#[get("/")]
fn index() -> &'static str {
    "Hello, world!"
}

fn main() {
    let connection = establish_connection();

    rocket::ignite().mount("/", routes![index]).launch();
}
