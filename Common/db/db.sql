CREATE TABLE MarketUsers(
   id INTEGER PRIMARY KEY AUTOINCREMENT,
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