-- Your SQL goes here

CREATE TABLE Shipments (
  id SERIAL PRIMARY KEY,
  tracking_id VARCHAR NOT NULL,
  title VARCHAR NOT NULL,
  carrier INT NOT NULL,
  manual_entry BOOLEAN NOT NULL DEFAULT 'f',
  shipment_status INT NOT NULL,
  deleted BOOLEAN NOT NULL DEFAULT 'f',
  created_on TIMESTAMP NOT NULL,
  updated_on TIMESTAMP NULL
);

CREATE TABLE ShipmentProgress (
  id SERIAL PRIMARY KEY,
  shipment_id INT NOT NULL,
  location VARCHAR NOT NULL,
  long double precision NOT NULL,
  lat double precision NOT NULL,
  message VARCHAR NOT NULL,
  shipment_status INT NOT NULL,
  created_on TIMESTAMP NOT NULL,
  updated_on TIMESTAMP NULL,
  CONSTRAINT FK_Shipments FOREIGN KEY (shipment_id) REFERENCES Shipments(id)
);