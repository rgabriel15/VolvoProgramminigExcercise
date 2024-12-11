# README #  
  
  ### What is this repository for? ###  
  
- Quick summary  
Rodrigo Gabriel  
rgabriel15@gmail.com  
2024/12/10  
VolvoProgrammingExercise (see Volvo_Programming_Exercise.pdf in root folder).  
  
- Version  
1.0.0  
  
- Features  
	- Backend:  
API Versioning.  
Clean Architecture.  
Distributed Memory Cache.  
Docker.  
DotNet 9.0.  
Entity Framework ORM  
Entity Framework InMemory Database.  
Logs (Serilog).  
Microservices.  
Mock.  
Rate Limiter.  
RESTful API.  
Solid.  
SonarAnalyzer (global configuration).  
Swagger.  
Tests (full coverage): Unit Tests (xUnit), Integrated Tests (xUnit), Architecture Tests (NetArchTest).  
  
	- Frontend:  
DotNet 9.0.  
MudBlazor (server mode).  
  
	- Some ideas for improvements:  
Authentication/Authorization.  
CORS.  
Cloud (e.g. Azure).  
Kubernetes  
Horizontal scaling (in this case, the PKs need to change from int to GUID/UUID/ULID).  
Message Broker (e.g. RabbitMQ).  
Terraform.  
  
  	Obs:  
I didn't use DDD as a design decision.  
Business rules are in Application layers/*Service classes.  
If needed, it's easy to transfer business rules from Application layers to the Domain layers to implement DDD.  
  
  ### How do I get set up? ###  
- Summary of set up  
Run both backend (Web.API) and frontend (Web.UI) projects.  
  
- Configuration  
  	1) Visual Studio 2022 -> "Solution Explorer" window -> right-click on Solution name (VolvoProgrammingExercise) -> left-click on "Configure Startup Projects";  
  		1.1) "Solution Property Page" window -> Left menu -> Common Properties -> Configure Startup Projects;  
  		1.2) Select "Multiple startup projects";  
  		1.3) In the grid:  
  			1.3.1) In "Project" column, look for the projects "Web.API" and "Web.UI";  
  			1.3.2) In "Action" column, set "Start" for "Web.API" and "Web.UI" projects;  
  	2) Visual Studio 2022 -> Toolbar -> on the "Run" button (green triangle), set to "http" or "https" mode;  
  		2.1) The Web.API project can also run in "Container (Dockerfile)" mode.  
  	3) Check if backend app (Web.API) api url (https://localhost:<port>/api/v1/) is the same configured in frontend app (Web.UI) appsettings.json file  
  		(Web.UI project -> appsettings.json -> VolvoProgrammingExerciseClientV1.ApiV1Url).  
  	  
- Dependencies  
  	All packages references are configured in *.csproj files.  
  
- Database configuration  
This solution uses Entity Framework InMemory Database by default for quick and minimal steps to execute.  
Although database constraints are configured in the *Entity.cs classes in the *.Domain layers with Data Annotations,  
EF InMemory Database will allow you to save data that would violate referential integrity constraints in a relational database.  
Therefore, constraints are not checked by tests in an EF InMemory Database  
(see https://entityframework-extensions.net/efcore-inmemory-provider#:~:text=InMemory%20will%20allow%20you%20to,constraints%20in%20a%20relational%20database)  
To test database constraints, you need to use a real relational database (e.g. SQLServer).  
  
OPTIONAL: how to use a real SQLServer database (SQLServer EF packages references already configured in *.csproj files):  
1) SQLServer Management Studio -> Create a database;  
2) Visual Studio 2022 -> "Solution Explorer" window -> Web.API project -> open "Program.cs" class;  
	2.1) In the "AddDependencyInjection" function, set "useInMemoryDatabase" input variable to "false":  
		builder  
		.Services  
		.AddDependencyInjection(logger: Log.Logger, useInMemoryDatabase: false)  
3) Visual Studio 2022 -> "Solution Explorer" window -> Web.API project -> open appsettings.json file;  
	3.1) set the database connection string (ConnectionStrings.LocalDbConnection);  
4) Visual Studio 2022 -> Toolbar -> View -> Other Windows -> Package Manager Console;  
	4.1) In "Default Project" dropdown, set "Base.Infrasctructure" project as default project;  
	4.2) Run EF commands to add migration and to update the database.  
  
  * How to run tests  
Visual Studio 2022 -> Toolbar -> View -> "Test Explorer" window -> left-click on the "Run" button.  
  
  ### Who do I talk to? ###  
rgabriel15@gmail.com