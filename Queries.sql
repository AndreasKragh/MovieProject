USE MovieDatabase;

SELECT 
title, 
ReleaseDate, 
Director.Name, 
MovieStatus, 
ProductionRecord.budget, 
ProductionCompany.Name 
FROM Movie
INNER JOIN ProductionCompany ON Movie.ProductionCompany=ProductionCompany.companyID
INNER JOIN Director ON Movie.Director=Director.directorID
INNER JOIN ProductionRecord ON ProductionRecord.movieID=Movie.movieID;


SELECT 
Movie.Title, 
Actor.Name, 
MovieCast.characterName, 
MovieCast.roleType 
FROM MovieCast
INNER JOIN Movie ON MovieCast.movieID=Movie.movieID
INNER JOIN Actor ON MovieCast.actorID=Actor.actorID;


SELECT
Movie.Title,
Cinema.name, 
Screens.screenNumber,
Screening.screeningDate,
Screening.startTime,
Screening.price
FROM Screening
INNER JOIN Movie ON Screening.movieID=Movie.movieID
INNER JOIN Screens ON Screening.screenID=Screens.screenID
INNER JOIN Cinema ON Screens.cinemaID=Cinema.cinemaID;


SELECT
Movie.title,
SUM(ticketSale.quantity) AS TotalTicketsSold,
SUM(ticketsale.totalprice) AS TotalRevenue
FROM Movie
INNER JOIN Screening ON Screening.MovieID=Movie.movieID
INNER JOIN TicketSale ON TicketSale.screeningID=Screening.screeningID
GROUP BY Movie.MovieID, Movie.Title;



SELECT
Movie.Title,
Location.Name,
Location.city,
Location.Country,
filmedAt.NumberOfShootingDays
FROM filmedAt
INNER JOIN Movie ON filmedAt.movieID=Movie.movieID
INNER JOIN Location ON filmedAt.locationID=Location.locationID;


SELECT
Actor.Name,
COUNT(DISTINCT MovieCast.movieID) AS NumberOfMovies,
SUM(ticketSale.quantity) AS TicketsSold
FROM MovieCast  
INNER JOIN Actor ON MovieCast.actorID=Actor.actorID
INNER JOIN Screening ON MovieCast.movieID=Screening.movieID
INNER JOIN ticketSale ON ticketSale.screeningID=Screening.screeningID
GROUP BY Actor.actorID, Actor.Name;

