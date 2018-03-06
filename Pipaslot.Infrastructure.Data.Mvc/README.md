# Unit of Work and Repository pattern
###### Controller as a Repository

## Introduction
Provides directly Create Read Update Delete operations to your api without business logic layer.
These controllers might be helpfull in early development.

## Installation
```
public class MyEntityController : RepositoryController<MyEntity, int>
{
    public MyEntityController(IUnitOfWorkFactory unitOfWorkFactory, IRepository<MyEntity, int> repository) : base(unitOfWorkFactory, repository)
    {
    }
}
```

Register controller repository in **Startup.cs**
```
services.AddSingleton<IRepository<MyEntity, int>, MyEntityRepository<int, AppDatabase, MyEntity>>();
```

## Usage

If controllers and dependencies are prepared, you can call these methods available on your API:
- GET **my-entity/paged/{{pageIndex}}/{{pageSize}}** - Get all records paged
- GET **my-entity/{{id}}** - Get record by ID
- POST **my-entity** - Create new record
- POST **my-entity/{{id}}** - Update record
- DELETE **my-entity/{{id}}** - Remove record