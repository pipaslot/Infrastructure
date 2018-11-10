# Unit or Work and Repository pattern
######  Entity Framework Core Integration

## Introduction
Sometimes we need to encapsulate more database operations into transaction scope and ensure that both or none change will be stored.
For this purpose was created this Unit of Work providing basic support for transaction scopes in background for Entity Framework.


## Installation
Create own repositories like this:
```
public class MyCustomUserRepository : EntityFrameworkRepository<AppDatabase,User,int>
{
    public UserRepository(IUnitOfWorkFactory uowFactory, IEntityFrameworkDbContextFactory dbContextFactory) : base(uowFactory, dbContextFactory)
    {
    }

    public User GetByLogin(string login)
    {
        return ContextReadOnly.User
            .Include(u => u.UserRoles)
            .ThenInclude(ur=>ur.Role)
            .FirstOrDefault(u => u.Login.Equals(login));
        }
    }
```
Copy following code in you **Startup.cs**
```
//Database factories
 services.AddSingleton<IEntityFrameworkDbContextFactory>(_ =>
{
    var options = new DbContextOptionsBuilder<AppDatabase>();
    options.UseSqlServer(Configuration.GetSection("ConnectionString").Value);
    return new EntityFrameworkDbContextFactory<AppDatabase>(options.Options);
});

//Unit of work
services.AddSingleton<IUnitOfWorkRegistry, UnitOfWorkRegistry>();
services.AddSingleton<IUnitOfWorkFactory, EntityFrameworkUnitOfWorkFactory<AppDatabase>>();

//Repositories
services.AddSingleton<MyCustomUserRepository>();
```

And have a fun. :)

## 1. Unit of Work

### 1.1 Basic example

```
using (var uow = uowFactory.Create())
{
    uow.Context.Blog.Add(new Blog
    {
        Name = "MyName"
    });
    // Persist Blog Entity. Entity Framework will setup Primary key
    uow.Context.SaveChanges();

    //Create Many to Many relation
    var user = uow.Context.User.First(u=>u.Id == id);
    user.UserBlogs.Add(new UserBlog
    {
        BlogId = blog.Id
    });
    // Here is not neccessary to save changes, because it will happen during commit

    var success = ... //Check status or some else our logic

    // Commit both changes in one transaction 
    if(success){
        uow.Commit();
    }
    // Otherwise both changes will be rejected 
}
```

### 1.2 Synchronizing two unit of work
```
// Problem
public void CallMethodsOneAndTwoInTwoTransactions(){
    MyMethodOne();
    MyMethodTwo();
    // If MyMethodTwo fail, changes from MyMethodOne will be persiste. 
    // That may cause incosistent database state.
}

// Solution
public void CallMethodsOneAndTwoInOneTransaction(){
    using (var uow = uowFactory.Create())
    {
        MyMethodOne();
        MyMethodTwo();
        uow.Commit();
        // If MyMethodTwo fail, changes from MyMethodOne will be rejected. 
    }
}

public void MyMethodOne(){
    using (var uow = uowFactory.Create())
    {
        ...
        uow.Commit();
    }
}

public void MyMethodTwo(){
    using (var uow = uowFactory.Create())
    {
        ...
        uow.Commit();
    }
}
```

## 2 Repository
Repositories are designed from general pattern. These repoistories consuming UnitOfWorkFactory and DatabaseFactory. 
For querying or persisting into database we are using two different context accessors:
### 2.1. Context Property (Read/Write Context)
Is used for Read/Write. If we want write data in database through repository, the call must be surrounded by Unit of Work, otherwise exception will be thrown. 
This prevent us to make inconsistent database state and in code we can fully see where we are using write operations.

**!! Notice !!** - Never create Unit of Work in Repository. That must be created in higher application layer.

### 2.2. ContextReadOnly Property
Provides Read only operations. This context may not be the same as the Read/Write Context. 
Does not need to be surrounded by Unit of Work. 
In Case we sourround operation above this contex by Unit of Work, then the Context and ContextReadOnly are the same.

**!! Notice !!** - Keep in mind that all write operations should be cross Read/Write context, otherwise changes will be lost.

### 2.3. Example
```
// Business Logic
using (var uow = uowFactory.Create())
{
    var category = _blogCategoryRepository.Find(categoryId);

    _blogRepository.MyInsert(new Blog
    {
        Name = "Default name",
        Category = category
    });
    uow.Commit();
}

// BlogRepository
public void MyInsert(Blog blog){
    ...
    Context.Blog.Add(blog);
}

// BlogCategoryRepository
public BlogCategory MyInsert(int id){
    var category = ContextReadOnly.FirstOrDefault(c => c.Id == id);
    ... Process or map object
    return category;
}
```