
USE master;
GO
IF DB_ID('MovieDatabase') IS NULL
    CREATE DATABASE MovieDatabase;
GO
USE MovieDatabase;
GO
-- Slet tabellerne hvis de findes
DROP TABLE IF EXISTS
    TicketSale, Screening, ProductionRecord, filmedAt, MovieCast,
    Movie, Screens, Actor, Cinema, Studio, ProductionCompany,
    Location, Director;
GO


  -- TABELLER
  

CREATE TABLE Director (
    directorID  INT IDENTITY(1,1) PRIMARY KEY,
    Name        VARCHAR(100) NOT NULL,
    BirthDate   DATE NOT NULL,
    Nationality VARCHAR(50) NOT NULL
);

CREATE TABLE Location (
    locationID INT IDENTITY(1,1) PRIMARY KEY,
    Name       VARCHAR(100) NOT NULL,
    City       VARCHAR(50) NOT NULL,
    Country    VARCHAR(50) NOT NULL,
    LocationType VARCHAR(50) NOT NULL CHECK (LocationType IN ('street', 'building', 'landscape', 'set'))
);


CREATE TABLE ProductionCompany (
    companyID   INT IDENTITY(1,1) PRIMARY KEY,
    Name        VARCHAR(100) NOT NULL,
    FoundedYear INT NOT NULL,
    Country     VARCHAR(50) NOT NULL
);


CREATE TABLE Studio (
    studioID            INT IDENTITY(1,1) PRIMARY KEY,
    Name                VARCHAR(100) NOT NULL,
    City                VARCHAR(100) NOT NULL,
    Country             VARCHAR(50) NOT NULL,
    NumberOfSoundStages INT NOT NULL,
    Capacity            INT NOT NULL,
    companyID           INT NOT NULL,
    FOREIGN KEY (companyID) REFERENCES ProductionCompany(companyID)
);

CREATE TABLE Cinema (
    cinemaID    INT PRIMARY KEY,
    name        VARCHAR(100) NOT NULL,
    address     VARCHAR(200) NOT NULL,
    city        VARCHAR(50) NOT NULL,
    openingYear INT NOT NULL
);

CREATE TABLE Actor (
    actorID     INT IDENTITY(1,1) PRIMARY KEY,
    Name        VARCHAR(100) NOT NULL,
    BirthDate   DATE NOT NULL,
    Nationality VARCHAR(50) NOT NULL,
    Gender      VARCHAR(10) NOT NULL
);

CREATE TABLE Screens (
    screenID        INT PRIMARY KEY,
    screenNumber    INT NOT NULL,
    seatingCapacity INT NOT NULL,
    screenType      VARCHAR(20) CHECK (screenType IN ('2D', '3D', 'IMAX', '4DX')),
    cinemaID        INT NOT NULL,
    FOREIGN KEY (cinemaID) REFERENCES Cinema(cinemaID)
);

CREATE TABLE Movie (
    movieID           INT IDENTITY(1,1) PRIMARY KEY,
    Title             VARCHAR(100) NOT NULL,
    Runtime           INT NOT NULL,
    ReleaseDate       DATE NOT NULL,
    AgeRating         VARCHAR(10) NOT NULL,
    MovieStatus       VARCHAR(20) CHECK (MovieStatus IN ('in production', 'released', 'archived', 'planned')),
    Director          INT NOT NULL,
    ProductionCompany INT NOT NULL,
    FOREIGN KEY (Director) REFERENCES Director(directorID),
    FOREIGN KEY (ProductionCompany) REFERENCES ProductionCompany(companyID)
);


CREATE TABLE MovieCast (
    movieID       INT NOT NULL,
    actorID       INT NOT NULL,
    characterName VARCHAR(100) NOT NULL,
    roleType      VARCHAR(50) CHECK (roleType IN ('lead', 'supporting', 'cameo')),
    PRIMARY KEY (movieID, actorID),
    FOREIGN KEY (movieID) REFERENCES Movie(movieID),
    FOREIGN KEY (actorID) REFERENCES Actor(actorID)
);

CREATE TABLE filmedAt (
    movieID              INT NOT NULL,
    locationID           INT NOT NULL,
    NumberOfShootingDays INT NOT NULL,
    PRIMARY KEY (movieID, locationID),
    FOREIGN KEY (movieID) REFERENCES Movie(movieID),
    FOREIGN KEY (locationID) REFERENCES Location(locationID)
);


CREATE TABLE ProductionRecord (
    recordID            INT IDENTITY(1,1) PRIMARY KEY,
    budget              DECIMAL(15, 2) NOT NULL,
    productionStartDate DATE NOT NULL,
    productionEndDate   DATE NOT NULL,
    movieID             UNIQUE INT NOT NULL    ,
    FOREIGN KEY (movieID) REFERENCES Movie(movieID)
);


CREATE TABLE Screening (
    screeningID   INT PRIMARY KEY,
    movieID       INT NOT NULL,
    screeningDate DATE NOT NULL,
    startTime     TIME NOT NULL,
    price         DECIMAL(10, 2) NOT NULL,
    screenID      INT NOT NULL,
    FOREIGN KEY (movieID) REFERENCES Movie(movieID),
    FOREIGN KEY (screenID) REFERENCES Screens(screenID)
);


CREATE TABLE TicketSale (
    ticketSaleID  INT IDENTITY(1,1) PRIMARY KEY,
    screeningID   INT NOT NULL,
    saleDateTime  DATETIME NOT NULL,
    quantity      INT NOT NULL CHECK (quantity > 0),
    totalPrice    DECIMAL(10, 2) NOT NULL
    FOREIGN KEY (screeningID) REFERENCES Screening(screeningID)
);
GO



/* ---------------------------------------------------------------------
   TESTDATA
   (IDENTITY-tabeller får ID 1, 2, 3 ... i den rækkefølge rækkerne indsættes)
   --------------------------------------------------------------------- */

-- Directors (3)  -> ID 1-3
INSERT INTO Director (Name, BirthDate, Nationality) VALUES
('Anna Holm',       '1972-03-14', 'Danish'),
('Marcus Reed',     '1965-11-02', 'American'),
('Sofia Lindqvist', '1980-07-21', 'Swedish');

-- Filming locations (5)  -> ID 1-5
INSERT INTO Location (Name, City, Country, LocationType) VALUES
('Råbjerg Mile',  'Skagen',      'Denmark', 'landscape'),
('Nyhavn',        'Copenhagen',  'Denmark', 'street'),
('Griffith Park', 'Los Angeles', 'USA', 'landscape'),
('Gamla Stan',    'Stockholm',   'Sweden', 'street'),
('Aarhus Ø',      'Aarhus',      'Denmark', 'street');

-- Production companies (3)  -> ID 1-3
INSERT INTO ProductionCompany (Name, FoundedYear, Country) VALUES
('Nordlys Film',         1998, 'Denmark'),
('Silver Peak Pictures', 1985, 'USA'),
('Aurora Media Group',   2005, 'Sweden');

-- Studios (4): selskab 1 ejer 2 studios
INSERT INTO Studio (Name, City, Country, NumberOfSoundStages, Capacity, companyID) VALUES
('Nordlys Studio Aarhus',     'Aarhus',      'Denmark', 3, 400,  1),
('Nordlys Studio København',  'Copenhagen',  'Denmark', 5, 650,  1),
('Silver Peak Lot A',         'Los Angeles', 'USA',     12, 2500, 2),
('Aurora Studio Stockholm',   'Stockholm',   'Sweden',  4, 500,  3);

-- Cinemas (3) - ingen IDENTITY, så ID skrives selv
INSERT INTO Cinema (cinemaID, name, address, city, openingYear) VALUES
(1, 'Bio Aarhus C',       'Store Torv 4, 8000 Aarhus C',      'Aarhus',     1985),
(2, 'Nordlys Bio',        'Vesterbrogade 20, 1620 København', 'Copenhagen', 2002),
(3, 'Kino Odense',        'Kongensgade 30, 5000 Odense C',    'Odense',     1994);

-- Actors (8)  -> ID 1-8
INSERT INTO Actor (Name, BirthDate, Nationality, Gender) VALUES
('Mikkel Brandt',  '1978-01-09', 'Danish',   'Male'),
('Clara Juhl',     '1990-06-30', 'Danish',   'Female'),
('Henrik Dahl',    '1962-12-12', 'Danish',   'Male'),
('Ida Kjær',       '1999-04-18', 'Danish',   'Female'),
('James Porter',   '1975-08-25', 'American', 'Male'),
('Rachel Stone',   '1983-02-11', 'American', 'Female'),
('Laura Winther',  '1987-10-05', 'Danish',   'Female'),
('Elsa Nyberg',    '1992-05-27', 'Swedish',  'Female');

-- Screens (6): biograf 1 har 3 sale, biograf 2 har 2, biograf 3 har 1
INSERT INTO Screens (screenID, screenNumber, seatingCapacity, screenType, cinemaID) VALUES
(1, 1, 220, 'IMAX', 1),
(2, 2, 120, '2D',   1),
(3, 3,  80, '3D',   1),
(4, 1, 180, '2D',   2),
(5, 2,  90, '4DX',  2),
(6, 1, 150, '2D',   3);

-- Movies (5)  -> ID 1-5
-- Instruktør 1 har film 1 og 3; selskab 1 har film 1, 3 og 5
INSERT INTO Movie (Title, Runtime, ReleaseDate, AgeRating, MovieStatus, Director, ProductionCompany) VALUES
('Stormens Øje',    118, '2023-03-10', '15', 'released', 1, 1),
('The Last Harbor', 132, '2022-09-15', '15', 'released', 2, 2),
('Midnatssol',      105, '2024-06-21', '11', 'released', 1, 1),
('Glass City',      124, '2025-11-07', '15', 'released', 3, 3),
('Ekko',             98, '2019-02-01', '7',  'archived', 2, 1);

-- Cast: film 1 har 4 skuespillere; skuespiller 1, 2, 3, 6 og 7 er med i flere film
INSERT INTO MovieCast (movieID, actorID, characterName, roleType) VALUES
(1, 1, 'Erik Storm',     'lead'),
(1, 2, 'Maja Lund',      'lead'),
(1, 3, 'Kaptajn Holm',   'supporting'),
(1, 4, 'Fiskerpige',     'cameo'),
(2, 5, 'Jack Mercer',    'lead'),
(2, 6, 'Ellen Mercer',   'supporting'),
(2, 2, 'Nora',           'supporting'),
(3, 1, 'Jonas',          'lead'),
(3, 7, 'Liv',            'lead'),
(4, 8, 'Astrid Berg',    'lead'),
(4, 6, 'Detective Hale', 'supporting'),
(5, 3, 'Faderen',        'lead'),
(5, 7, 'Datteren',       'supporting');

-- filmedAt: film 1 bruger 2 locations; location 5 og 2 bruges af flere film
INSERT INTO filmedAt (movieID, locationID, NumberOfShootingDays) VALUES
(1, 1, 12),
(1, 5,  8),
(2, 3, 20),
(2, 2,  6),
(3, 5, 10),
(4, 4, 18),
(5, 2,  9);

-- Production records: én pr. film
INSERT INTO ProductionRecord (budget, productionStartDate, productionEndDate, movieID) VALUES
(45000000.00, '2021-04-01', '2021-10-15', 1),
(80000000.00, '2020-08-01', '2021-06-30', 2),
(30000000.00, '2022-09-01', '2023-03-20', 3),
(55000000.00, '2023-10-01', '2024-08-31', 4),
(12000000.00, '2017-05-01', '2017-11-30', 5);

-- Screenings (10): sal 1 har flere visninger; film 1 vises i biograf 1 og 2
INSERT INTO Screening (screeningID, movieID, screeningDate, startTime, price, screenID) VALUES
(1,  1, '2025-11-10', '19:00', 110.00, 1),
(2,  1, '2025-11-10', '21:30', 110.00, 1),
(3,  1, '2025-11-11', '18:00',  95.00, 4),
(4,  2, '2025-11-12', '20:00', 100.00, 2),
(5,  2, '2025-11-13', '17:30',  90.00, 6),
(6,  3, '2025-11-14', '16:00',  85.00, 3),
(7,  3, '2025-11-14', '19:15',  85.00, 5),
(8,  4, '2025-11-15', '20:30', 130.00, 1),
(9,  4, '2025-11-16', '21:00', 120.00, 4),
(10, 4, '2025-11-17', '19:00', 100.00, 6);

-- Ticket sales (15): visning 1 har 3 salg; totalPrice = quantity * price
INSERT INTO TicketSale (screeningID, saleDateTime, quantity, totalPrice) VALUES
(1,  '2025-11-08 14:22', 2, 220.00),
(1,  '2025-11-09 10:05', 4, 440.00),
(1,  '2025-11-10 18:40', 1, 110.00),
(2,  '2025-11-10 12:00', 2, 220.00),
(2,  '2025-11-10 21:05', 2, 220.00),
(3,  '2025-11-11 17:10', 3, 285.00),
(4,  '2025-11-12 19:30', 2, 200.00),
(5,  '2025-11-13 16:55', 1,  90.00),
(6,  '2025-11-14 15:20', 5, 425.00),
(7,  '2025-11-14 18:45', 2, 170.00),
(8,  '2025-11-15 09:12', 2, 260.00),
(8,  '2025-11-15 20:01', 3, 390.00),
(9,  '2025-11-16 20:15', 2, 240.00),
(10, '2025-11-17 18:30', 1, 100.00),
(10, '2025-11-17 18:35', 4, 400.00);
GO





