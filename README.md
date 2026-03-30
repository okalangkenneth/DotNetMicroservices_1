# DotNet Microservices — E-Commerce Platform

> A production-grade microservices backend built with **.NET 5**, demonstrating  
> real-world distributed systems patterns used by engineering teams at scale.

[![.NET](https://img.shields.io/badge/.NET-5.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)](https://docker.com)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Event%20Bus-FF6600?logo=rabbitmq)](https://rabbitmq.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## Overview

This project showcases a complete e-commerce backend built as independently deployable
microservices. Each service owns its own database, communicates asynchronously via
RabbitMQ, and is accessible through a single Ocelot API Gateway. A zero-dependency
demo UI lets you walk through a full purchase flow in the browser.

**One command starts the entire platform:**
```bash
docker-compose up --build
```

---

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Browser / Client                      │
└───────────────────────┬─────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│           API Gateway — Ocelot  :5000                   │
│     Routing · Rate limiting · Request aggregation       │
└───┬──────────────┬──────────────┬────────────────┬──────┘
    │              │              │                │
    ▼              ▼              ▼                ▼
┌────────┐   ┌─────────┐   ┌──────────┐   ┌───────────┐
│Catalog │   │ Basket  │   │ Discount │   │ Ordering  │
│  API   │   │   API   │   │   API    │   │    API    │
│ :8000  │   │  :8001  │   │  :8002   │   │   :8003   │
│MongoDB │   │  Redis  │   │PostgreSQL│   │SQL Server │
└────────┘   └────┬────┘   └──────────┘   └─────┬─────┘
                  │                              │
                  │      ┌───────────┐           │
                  └─────►│ RabbitMQ  ├───────────┘
                         │  :5672    │
                         └───────────┘
                    BasketCheckout event
```

When a user checks out, Basket publishes a `BasketCheckoutEvent` to RabbitMQ.
Ordering consumes it asynchronously and creates the order via a CQRS command pipeline —
with no direct HTTP coupling between the two services.

---

## Services

| Service | Port | Database | Responsibility |
|---------|------|----------|----------------|
| **Catalog API** | 8000 | MongoDB | Product catalogue — CRUD with seeded demo data |
| **Basket API** | 8001 | Redis | Shopping cart — persists to cache, publishes checkout event |
| **Discount API** | 8002 | PostgreSQL | Coupon management — queried by basket at checkout |
| **Ordering API** | 8003 | SQL Server | Order lifecycle — CQRS / MediatR, EF Core, email notification |
| **API Gateway** | 5000 | — | Ocelot routing, rate limiting, response aggregation |
| **Elasticsearch** | 9200 | — | Log storage |
| **Kibana Dashboard** | 5601 | — | Log visualisation |


---

## Patterns & Technical Highlights

| Pattern | Where | Detail |
|---------|-------|--------|
| **CQRS + MediatR** | Ordering API | Commands and queries fully separated; each handler is independently testable |
| **Event-driven architecture** | Basket → Ordering | Checkout triggers a RabbitMQ event; ordering consumes asynchronously |
| **Repository pattern** | All services | Abstracts data access; swap databases without touching business logic |
| **API Gateway** | Ocelot | Single entry point; rate limiting on checkout, response aggregation endpoint |
| **Database-per-service** | All services | MongoDB, Redis, PostgreSQL, SQL Server — each service owns its data |
| **Auto migrations** | Discount API | DbUp runs embedded SQL scripts on startup; zero manual setup |
| **Seed data** | Catalog + Ordering | Services start with realistic demo data out of the box |
| **Swagger on every service** | All APIs | Fully documented endpoints; explore at `/swagger` on each port |

---

## Technology Stack

**Backend**
- ASP.NET Core 5 Web API
- MediatR 11 : CQRS command/query dispatching
- AutoMapper: object mapping across layers
- FluentValidation: declarative request validation
- Entity Framework Core 5: ORM for SQL Server
- Dapper: lightweight ORM for PostgreSQL

**Messaging**
- RabbitMQ 3.12 : async event bus
- RabbitMQ.Client : producer and consumer implementation

**Databases**
- MongoDB 6 : document store for product catalogue
- Redis 7 : distributed cache for shopping baskets
- PostgreSQL 15 : relational store for discount coupons
- SQL Server 2022 : relational store for orders

**Logging**
- Serilog : structured logging across all services
- Elasticsearch : log storage and indexing
- Kibana : log visualisation and dashboards

**Infrastructure**
- Docker + Docker Compose : full local orchestration
- Ocelot 15 : API gateway with routing, rate limiting, aggregation

---

## Quick Start

**Prerequisites:** [Docker Desktop](https://www.docker.com/products/docker-desktop/) (running)

```bash
# Clone
git clone https://github.com/okalangkenneth/DotNetMicroservices_1.git
cd DotNetMicroservices_1

# Start everything — first run takes ~5 min to pull images
docker-compose up --build
```

All containers start automatically. Services, databases, and RabbitMQ are wired
together via Docker networking, no manual configuration needed.

---

## Demo

Open `demo-ui.html` directly in your browser (no server needed) for a visual walkthrough:

1. **Browse** — product catalogue loads from Catalog API (MongoDB-backed, seeded data)
2. **Discount** — each product shows its live discount from Discount API (PostgreSQL)
3. **Add to basket** — cart state persisted to Redis via Basket API
4. **Checkout** — triggers a RabbitMQ `BasketCheckoutEvent`; basket is cleared
5. **Order confirmed** — Ordering API consumed the event and created the order via CQRS


### API Endpoints (via Gateway on port 5000)

```
# Catalog
GET    /Catalog                              — all products
GET    /Catalog/{id}                         — single product
GET    /Catalog/GetProductByCategory/{cat}   — filter by category
POST   /Catalog                              — create product
PUT    /Catalog                              — update product
DELETE /Catalog/{id}                         — delete product

# Basket
GET    /Basket/{userName}                    — get cart
POST   /Basket                               — update cart
POST   /Basket/Checkout                      — checkout (publishes event)
DELETE /Basket/{userName}                    — clear cart

# Discount
GET    /Discount/{productName}               — get coupon
POST   /Discount                             — create coupon
PUT    /Discount                             — update coupon
DELETE /Discount/{productName}               — delete coupon

# Ordering
GET    /Order/{userName}                     — get orders for user
POST   /Order                                — create order directly
PUT    /Order                                — update order
DELETE /Order/{id}                           — delete order
```

### Swagger UIs (direct service access)

| Service | URL |
|---------|-----|
| Catalog API | http://localhost:8000/swagger |
| Basket API | http://localhost:8001/swagger |
| Discount API | http://localhost:8002/swagger |
| Ordering API | http://localhost:8003/swagger |
| RabbitMQ Dashboard | http://localhost:15672 (guest / guest) |
| Kibana Dashboard | http://localhost:5601 |
| Elasticsearch | http://localhost:9200 |

---

## Project Structure

```
DotNetMicroservices_1/
├── ApiGateways/                  # Ocelot gateway + request aggregator
│   ├── ocelot.json               # Route configuration
│   └── Aggregators/              # Custom response aggregation
├── Basket.API/
│   ├── Controllers/              # BasketController (GET, POST, Checkout)
│   ├── Entities/                 # ShoppingCart, ShoppingCartItem, BasketCheckout
│   ├── Events/                   # BasketCheckoutEvent (RabbitMQ payload)
│   ├── Repositories/             # Redis-backed basket repository
│   └── Services/                 # RabbitMQ publisher + background consumer
├── Catalog.API/
│   ├── Controllers/              # CatalogController — full CRUD
│   ├── Data/                     # MongoDB context + seed data
│   └── Repositories/             # MongoDB product repository
├── Dicount.API/                  # Note: original typo preserved
│   ├── Controllers/              # DiscountController — full CRUD
│   ├── Repositories/             # Dapper + PostgreSQL repository
│   └── Scripts/                  # DbUp SQL migration scripts
├── Ordering_API/
│   ├── Controllers/              # OrderController — CQRS dispatch
│   ├── EventConsumer/            # BasketCheckoutConsumer (RabbitMQ)
│   ├── Handlers/                 # MediatR commands + queries
│   │   ├── Commands/             # CheckoutOrder, UpdateOrder, DeleteOrder
│   │   └── Queries/              # GetOrdersList
│   ├── Repositories/             # EF Core repository base + order repo
│   └── Services/                 # SendGrid email service
├── demo-ui.html                  # Zero-dependency browser demo
├── docker-compose.yml            # Full stack orchestration
└── README.md
```

---

## What I Learned / Built

This project covers the full spectrum of distributed systems concerns:

- Designing **service boundaries**: what belongs in each service and why
- **Async messaging**: decoupling checkout from order creation with RabbitMQ
- **CQRS** : separating read models from write commands; every operation has a dedicated handler
- **Gateway patterns**: routing, rate limiting, and aggregating responses at the edge
- **Database-per-service**: choosing the right storage for each problem (cache vs document vs relational)
- **Docker orchestration**: networking, environment-based config, startup dependencies
- **Auto-migration**: zero-touch schema setup via DbUp embedded scripts

---

## License

MIT — see [LICENSE](LICENSE) for details.

---

*Built by [Kenneth Okalang](https://github.com/okalangkenneth)*
