# Unit of Work and Repository pattern
###### Definition of structure and logic

## Unit of Work
Sometimes we need to encapsulate more mostly database operations into transaction scope and ensure that both or none change will be stored.
For this purpose was created this Unit of Work providing basic support for transaction scopes in background.

Consider this simple example:
```
using (var uow = uowFactory.Create())
{
    ... database operation 1 (Update, Insert, Delete)

    ... database operation 2 (Update, Insert, Delete)

    ... filesystem operation (Update, Insert, Delete)

    //If all previous operations passed, then let persisted data
    uow.Commit();
    //If commit is not invoked, all changes will be reverted
}
```

But not all operations and transactions are quite simple as the example above.

## Repository

TODO

## Query

TODO