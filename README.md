# Multi-Tenant E-Commerce SaaS Platform

A scalable, high-performance E-Commerce platform built on a multi-tenant architecture. This system empowers entrepreneurs to launch their own independent online stores within a unified platform, providing each tenant with a dedicated database schema, custom domain, and isolated administration dashboard.

## Architecture
This project strictly follows **Clean Architecture** principles to ensure separation of concerns, testability, and maintainability.

- **Backend:** .NET 9.0
- **ORM:** Entity Framework Core
- **Database:** Microsoft SQL Server
- **Cloud Services:** AWS (Amazon S3) for secure invoice and media storage.

## Key Features
- **Multi-Tenancy:** Automated provisioning of store databases, domains, and configurations upon registration.
- **Role-Based Access Control (RBAC):**
  - **Super Admin:** Platform-wide oversight and management.
  - **Store Owner:** Independent management of store products, customers, and orders.
  - **Store User:** Dedicated interface for shopping and order history.
- **Isolated Dashboards:** Custom UI/UX experiences tailored to each user role.
- **Cloud Storage:** Integrated AWS S3 for reliable document and invoice management.
- **API-First Design:** Fully documented RESTful APIs.

## API Documentation
The system includes a comprehensive Swagger UI for API exploration and testing. Once the backend is running, you can access it at:
`/swagger`

## Getting Started

### Prerequisites
- .NET 9.0 SDK or higher
- SQL Server
- AWS Account (for S3 storage)

### Configuration
1. **Clone the repository:**
   git clone [https://github.com/mohamed-alkasem/MultiTenantProject_E-Commerce.git](https://github.com/mohamed-alkasem/MultiTenantProject_E-Commerce.git)
Environment Setup:

Locate the appsettings.json file.

Configure your SQL Server ConnectionStrings.

Update the AWS section with your own credentials (AccessKey, SecretKey, and BucketName) to ensure cloud services function correctly.

Database Initialization
Ensure your environment is set up to match the .NET 9.0 dependencies. Run the following command to apply migrations and initialize the database schema:

dotnet ef database update
Running the Application
After configuring your environment and database, run the backend service:

dotnet run
Security & Best Practices
The application uses modern .NET 9 features for performance and security.

Ensure your appsettings.json is added to .gitignore to prevent sensitive AWS credentials and connection strings from being committed to the repository.

Status
Backend API: Well-developed and functional.

Frontend/Dashboard: Currently under active development to provide a seamless UI experience.

Developed by Mohamad Alkassem
