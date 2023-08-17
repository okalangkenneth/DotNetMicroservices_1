# DotNet Microservices: A Comprehensive Microservices Solution

## Overview
This repository showcases a comprehensive microservices solution built using .NET Core. It demonstrates the creation, communication, and deployment of multiple microservices, each with its dedicated database, all orchestrated through an API Gateway.

## System Architecture Diagram.

![image](https://github.com/okalangkenneth/DotNetMicroservices_1/blob/master/Microservices_bg.png)


## Features

### Microservices Creation
- Basket Service: Uses Redis as its database.
- Catalog Service: Utilizes MongoDB for data storage.
- Discount Service: Employs PostgreSQL for its database needs.
- Ordering Service: Leverages SQL Server for data persistence.

### API Gateway
Ocelot: Acts as the entry point for all client requests, routing  "API Requests" to the appropriate microservices.

### Inter-Service Communication

RabbitMQ: Facilitates asynchronous messaging between the microservices "Async Messages", ensuring smooth and efficient communication.

### Resilience
Implement patterns like Circuit Breaker, Retry, and Timeout to ensure the system remains operational even when some services fail.

´´´´// Example code snippet for resilience using Policy
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10));

services.AddHttpClient("ResilientClient")
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);
    ´´´´


### Logging
Elastic Stack (ELK): Provides centralized distributed logging, making it easy to monitor and troubleshoot the system. All microservices "Send Logs" to the ELK Stack for centralized logging.

### Containerization
Docker Compose: Enables containerization of each microservice and their respective databases, simplifying deployment and scaling.

### Testing
Comprehensive testing of each microservice and the entire system to ensure seamless integration and communication.

### Documentation
Detailed documentation of each step in the README, aiding others in understanding and replicating the setup.

## Getting Started

### Project Setup
1. Create a new repository on GitHub named "DotNetMicroservicesPortfolio".
2. Clone the repository to your local machine:
3. 
Create File


````git clone https://github.com/[YourUsername]/DotNetMicroservicesPortfolio.git````

[... Follow the steps from the project outline for each section ...]

## Contributing
Contributions are welcome! Please ensure you follow the outlined steps and maintain the integrity of the microservices architecture.

## License
This project is licensed under the MIT License. For more details, refer to the LICENSE.md file.
