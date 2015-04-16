CREATE TABLE MarketUsers(
   id INTEGER PRIMARY KEY,
   nickname TEXT,
   username TEXT,
   password TEXT
);

CREATE TABLE MarketDiginotes(
   id INTEGER PRIMARY KEY
);

CREATE TABLE Market(
   id INTEGER PRIMARY KEY,
   diginoteId INTEGER REFERENCES MarketDiginotes(id),
   userId INTEGER REFERENCES MarketUsers(id)
);

CREATE TABLE MarketLog(
   id INTEGER PRIMARY KEY AUTOINCREMENT,
   time TEXT,
   description TEXT
);

CREATE TABLE SaleOrders(
   id INTEGER PRIMARY KEY,
   quantity INTEGER,
   value REAL,
   processed BOOLEAN,
   userId INTEGER
);

CREATE TABLE BuyOrders(
   id INTEGER PRIMARY KEY,
   quantity INTEGER,
   value REAL,
   processed BOOLEAN,
   userId INTEGER
);