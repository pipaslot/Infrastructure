# Infrastructure
.Net Core 2 project Infrastructure Tools

## 1 Unit of Work and Repository pattern
Common generic library providing structure and workflow is described into project [Pipaslot.Infrastructure.Data](Pipaslot.Infrastructure.Data/README.md).
Concrete implementation for Entity Framework Core is described into project [Pipaslot.Infrastructure.Data.EntityFrameworkCore](Pipaslot.Infrastructure.Data.EntityFrameworkCore/README.md).

### 1.1 Controller as a Repository
For rapid deveopment of for only Dial record we do not need to create business logic layer, but we can directly access store/database from controller with CRUD operations. For more details see [Pipaslot.Infrastructure.Data.Mvc](Pipaslot.Infrastructure.Data.Mvc/README.md)


## 2 Security Framework
TODO
[Pipaslot.Infrastructure.Security](Pipaslot.Infrastructure.Security/README.md)

### 2.1 Entity Framework Integration
TODO
[Pipaslot.Infrastructure.Security.EntityFrameworkCore](Pipaslot.Infrastructure.Security.EntityFrameworkCore/README.md)

### 2.2 MVC API Integration
TODO
[Pipaslot.Infrastructure.Security.Mvc](Pipaslot.Infrastructure.Security.Mvc/README.md)

## 3 Another Tools
[Entity Framework Core Tools](Pipaslot.Infrastructure.EntityFrameworkCore/README.md) - Prefixing database table names by class namespace

[ASP.NET Core MVC Tools](Pipaslot.Infrastructure.Mvc/README.md) - Re-structuring controllers