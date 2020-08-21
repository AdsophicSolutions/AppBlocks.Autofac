# Documentation for AppBlocks.Autofac
## Introduction
AppBlocks.Autofac extends functionality of Autofac, a fantastic IOC container, to help speed up the task of creating enterprise applications. The framework is especially intended to simplify testing and supportability enterprise applications. 
## Features 
### Automated Logging
AppBlocks adds support for automatically logging service call requests and responses. That means that service writers do not have to explicitly write logging code. 
### Aspect Oriented Validation
Many issues in production systems are caused by underlying data. Often, it is difficult to replicate the issue in other environments. AppBlocks lets users attach validation entities dynamically to Autofac services. Since the validation entities support dynamic attachment they can be safely updated without updates to the services code. It lets validation code that targets support to evolve on its own timeline. 
### Aspect Oriented Workflow Writers
AppBlocks.Autofac support creations of workflow writers. Like validation entities, workflow writers can also be attached to Autofac services and can also be safely updated with having to update service code. Workflow writers are especially useful in a micro-services like architecture where it can be used to monitor data transformation and locate data issues efficiently. 

## Examples
Here are few examples to get you started on AppBlocks.Autofac [Examples](https://github.com/AdsophicSolutions/AppBlocks.Autofac.Examples)


