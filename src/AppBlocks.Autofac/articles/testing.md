# Test Support 

As applications have become more complex automated testing has become one of the key elements to help build stable applications. It is one of the elements SDLC that we cannot do without. 

One of the challenges faced during automated testing is that many services require other systems, databases, network connections etc. to retrieve data. This is because we generally do not want automated tests to rely on these external systems, systems that may not be available in test conditions. Using mock implementation is the architectural pattern most commonly used to overcome this issue. Ideally, mock implementations should replace service implementations that rely on external systems. 

AppBlocks provides a mechanism to do exactly that. In AppBlocks, services that are attributed as live services do not get registered in the IOC/DI container when the application is run in test mode. The test assembly can then generate mock implmentations for those services. Effectively, AppBlocks provides a mechanism declaratively replace service implementations with mock implementations.

