# Hotels Management System (HMS)

## Overview

Hotels Management System (HMS) is a RESTful API built with ASP.NET Core for managing hotels, rooms, managers, guests, and reservations. The system follows a hotel-centric structure where most operations are performed through hotel endpoints. Authentication and role-based access control are implemented using JWT.

## Technologies

* ASP.NET Core Web API
* Entity Framework Core (Code First)
* SQL Server
* JWT Authentication
* Swagger / OpenAPI

## Architecture

The project follows a layered structure:

* **Controllers** – expose API endpoints
* **Services** – contain business logic
* **Repositories** – handle database operations
* **Infrastructure** – database context and configuration

## Main Entities

* **Hotel** – basic hotel information (name, rating, location, address)
* **Room** – belongs to a hotel and has a price
* **Manager** – associated with a hotel
* **Guest** – system user who can create reservations
* **Reservation** – booking made by a guest for one or more rooms

## Roles

* **Admin** – full system access
* **Manager** – manages hotel resources
* **Guest** – can create and manage reservations

## Main Endpoints

Authentication

* POST `/api/auth/register`
* POST `/api/auth/login`

Hotels

* GET `/api/hotels`
* POST `/api/hotels`
* GET `/api/hotels/{hotelId}`
* PUT `/api/hotels/{hotelId}`
* DELETE `/api/hotels/{hotelId}`

Rooms

* POST `/api/hotels/{hotelId}/rooms`
* GET `/api/hotels/{hotelId}/rooms/{roomId}`
* PUT `/api/hotels/{hotelId}/rooms/{roomId}`
* DELETE `/api/hotels/{hotelId}/rooms/{roomId}`

Reservations

* POST `/api/hotels/{hotelId}/reservations`
* GET `/api/hotels/{hotelId}/reservations/{reservationId}`
* PUT `/api/hotels/{hotelId}/reservations/{reservationId}`
* DELETE `/api/hotels/{hotelId}/reservations/{reservationId}`

