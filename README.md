# smERP

smERP is a small Enterprise Resource Planning (ERP) system used to showcase my advancement in software development skills and best practices. This project is currently under development, with the goal of creating a compact ERP solution.

## Project Overview

smERP demonstrates the implementation of several software architecture patterns and principles:

- Clean Architecture
- Command Query Responsibility Segregation (CQRS)
- Domain-Driven Design (DDD)
- Result Pattern

## Project Structure

The project is organized into the following layers:

1. **smERP.SharedKernel**: Contains shared components and utilities used across all layers, including the domain.

2. **smERP.Domain**: Houses the core business logic, including domain entities and value objects.

3. **smERP.Infrastructure**: Focuses on external concerns that can be provided by third parties, currently utilized for identity management.

4. **smERP.Persistence**: Manages data persistence using Entity Framework Core, repositories, and the Unit of Work pattern.

5. **smERP.Application**: Implements application logic, including request handlers, commands, and queries.

6. **smERP.WebApi**: Provides the API endpoints for interacting with the system.

## Key Technologies and Packages

- **MediatR**: Implements the mediator pattern for loosely-coupled communication between components.
- **FluentValidation**: Provides a fluent interface for building strongly-typed validation rules.
- **FluentResults**: Offers a robust result object for handling success, failure, and error cases.

## Features

As this project is being developed as a graduate project (DEPI), the current focus is on the Inventory Management module:

Inventory Management

- Robust and flexible product catalog system
- Stock level tracking and management (underway)
- Inventory valuation (underway)
- Low stock alerts and reorder point management (underway)


Generic Product Implementation

- Versatile product model adaptable to various business needs
- Customizable product attributes and categories
- Scalable design to accommodate future expansion beyond inventory management


Planned Future Enhancements

- Order management
- Supplier management
- Basic reporting and analytics
- Integration capabilities with other business systems

## License

It's a free real estate

## Contact

feel free to contact me any time by email at xomar.metwallyx@gmail.com
---

This project is actively under development. Stay tuned for updates and new features!
