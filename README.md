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

````C#
// Example code snippet for API Gateway setup
public void ConfigureServices(IServiceCollection services)
{
    services.AddOcelot();
}
````

### Inter-Service Communication

RabbitMQ: Facilitates asynchronous messaging between the microservices "Async Messages", ensuring smooth and efficient communication.

````C#
// Example code snippet for RabbitMQ messaging
public void SendMessage(string message)
{
    var factory = new ConnectionFactory() { HostName = "localhost" };
    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {
        channel.QueueDeclare(queue: "messages", durable: false, exclusive: false, autoDelete: false, arguments: null);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "", routingKey: "messages", basicProperties: null, body: body);
    }
}
````
### Resilience
Implement patterns like Circuit Breaker, Retry, and Timeout to ensure the system remains operational even when some services fail.

````C#
// Example code snippet for resilience using Policy
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10));

services.AddHttpClient("ResilientClient")
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);
    
````

### Logging
Docker Compose is used to setup Elasticsearch, Logstash, and Kibana (ELK) services. All microservices "Send Logs" to the ELK Stack for centralized logging, making it easy to monitor and troubleshoot the system

````C#
// Example for the docker compose file
version: '3'
services:
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:7.10.1
    environment:
      - discovery.type=single-node
    ports:
      - 9200:9200
  logstash:
    image: docker.elastic.co/logstash/logstash:7.10.1
    ports:
      - 5000:5000
    volumes:
      - ./logstash.conf:/usr/share/logstash/pipeline/logstash.conf
    depends_on:
      - elasticsearch
  kibana:
    image: docker.elastic.co/kibana/kibana:7.10.1
    ports:
      - 5601:5601
    depends_on:
      - elasticsearch
````


````C#
// Example code snippet for logging setup
var logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
    })
    .CreateLogger();
````


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
