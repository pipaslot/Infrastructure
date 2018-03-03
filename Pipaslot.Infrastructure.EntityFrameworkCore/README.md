# Entity Framework Core Tools

## Database Entity Name Prefixing

By default entity framework converts Entity names like **MyProject.Accounts.Entities.User** to table name **User**. 
This tool allows you to prefix table name by class namespace name and ignore generic namespace.
With following configuration we cant reach table name **Accounts_User**
```
public class MyDatabase : DbContext
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.PrefixAllTablesByEntityNamespace(new string[]{ "Entities" });
	}
}
```