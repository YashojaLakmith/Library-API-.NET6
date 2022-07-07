-----Library API-----

This Web API application is written to be run on .NET 6 using ASP.NET Core while database operations are handled through Entity Framework Core.

This Web API gives following functionalities.
			-	Create and manage user(or member) accounts and assign them with one role each.
			-	Create and track books.
			-	Full authentication using JWT and role based authorization.
			-	Borrow, return and write off unrecovered books.

During development stage, appsettings.json file was configured to include and use following values,
			-	JWT Security key for encryption.
			-	Issuer, Audience and valid time for JWT.
			-	SQL Server connection string.