# AppBlocks.Autofac
AppBlocks intends to simplify the task of integrating Autofac into your application with emphasis on building supportable and testable modular applications. 

## Why AppBlocks? 
A good enterprise application should be maintenable, supportable, testable and performant. Many application architectures focus on maintenability and performance but do not focus as much on testability and even less so on supportability. A testable application architecture makes it easy to swap out live data services - services that interface with other applications or systems - with mock test implementations. A supportable application makes it easy to log information in the production environment to help speed up the process of locating and solving application issues. 

AppBlocks addresses both testability and supportability architectural issues. It utilizes aspect oriented programming principles to introduce support code without having to modify the business logic code. This means that support code can be managed by the dev ops team, not the application team. To enhance testing capabilities, it lets the developer declaratively exclude "live" services and introduce mock services to take their place in test environment.

### Building Modular Applications
AppBlocks simplifies building modular applications. It simplifies the task of updating modules, since only the changed modules need to be updated. It helps in application testing, modules would rarely need to be tested in the context of the full application.    

### Support Modules 
AppBlocks frameworks adopts the support first mentality. It overcomes some of the challenges around adding support related functionality into an application. Code responsible for custom logging, data validation and workflow monitoring can exist in its own modules and can be updated independently. This separation of concerns is a powerful mechanism to provide a full separation between application business logic and code that is dedicated for application support. [Support First Philosophy](supportFirst.html)

### Test Support 
Application services can be excluded from test environment making it easy to replace them with mock replacements. Services that are declaratively marked as live services are excluded in the services container repository in test application environment. These services can then be replaced by mock services that can provide test data required for application testing. 

### Configurable implementations
JSON configuration defines directory sources that make up an AppBlocks application. An application can be constructed as a combination of application modules managed by different teams within an organization.   




