## AppBlocks.Autofac

### Introduction
AppBlocks intends to simplify the task of integrating Autofac into your application with emphasis on building supportable and testable applications. AppBlocks supports modular development through dynamic assembly loading. New assemblies can be added to an application using configuration files. It supports the introduction of dynamically loaded support code, thus decoupling support code from application code. Additionally, it actively prevents support code from negatively impacting application code. That means that the support specific code can evolve on its own and be released to production environment without the risk of changing application functionality. 

### Background
#### Application Monitoring
I have often encountered the following situation. Application support is unable to solve a problem and the development team is called to assist. The development team performs its analysis, finds out the exact conditions for the problem and proposes a solution. Frequently, the issue can be tied to specific data conditions. These data conditions are captured and added to the knowledge base. However, no concrete link is established between the knowledge base article and source of the issue. That means that when the issue occurs again, support must perform a knowledge base search and hope they locate the right article quickly. Additionally, support must often perform data analysis to ascertain that this article pertains to that exact problem. 

One way to solve the problem is to add validation and logging i.e. application monitoring code into the application. But that could be risky. These code changes can have a negative impact on the application. What if there are bugs in the monitoring code? If this code is no longer required how could you safely remove this code? It may not be desirable to update an application with just monitoring code - do you want to release a new version of your application that just includes new monitoring code? It also means that your source control is littered with updates that include no business functionality just making it more difficult to track business changes.

AppBlocks solves this problem by providing support for dynamically injecting application monitoring code into service calls. The monitoring code could live in a different assembly, and even be maintained by the support group. It can have its own release cycle. That means that when new issues are encountered and analysis performed, the monitoring code could be modified to perform data checks on the incoming service calls and direct support team to the exact knowledge base article created to resolve the issue. It also simplifies the task of modifying / removing monitoring code without impacting the application code. 

#### DI / IOC Container
I came across Autofac a few years ago. I really liked its comprehensive support for DI / IOC. It supports scanning assemblies for services and interface implementations, however, I wanted more granular control over what implementations were included in the container. Hence the support for attribute based service definitions. 

Additionally, there is built in support for MediatR based request response and notification services. 

#### Usage & Examples 
AppBlocks.Autofac is available on NuGet [AppBlocks.Autofac](https://www.nuget.org/packages/AppBlocks.Autofac/#). 

Samples and Examples are available [here](https://github.com/AdsophicSolutions/AppBlocks.Autofac.Examples)
