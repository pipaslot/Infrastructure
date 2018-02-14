using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Pipaslot.Infrastructure.EntityFrameworkCoreTests")]
namespace Pipaslot.Infrastructure.EntityFrameworkCore
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Convert table names by this patterns (without ignored namespaces):
        ///     MyEntity                    => MyEntity (without change)
        ///     Entities.MyEntity           => Entities.MyEntity
        ///     Root.Entities.MyEntity      => Entities_MyEntity
        ///     Extra.Root.Entities.MyEntity=> Entities_MyEntity
        /// 
        /// Convert table names by this patterns (with ignored namespace "Entities"):
        ///     MyEntity => MyEntity (without change)
        ///     Entities.MyEntity           => MyEntity
        ///     Root.Entities.MyEntity      => Root_MyEntity
        ///     Extra.Root.Entities.MyEntity=> Root_MyEntity
        /// </summary>
        ///  public void ConvertClassName11() => AssertTableName("MyEntity", "MyEntity");
        /// <param name="modelBuilder"></param>
        /// <param name="ignoredNamespaces">Namespace names whic hwill be ignored into name conversion.</param>
        /// <returns></returns>
        public static ModelBuilder PrefixAllTablesByEntityNamespace(this ModelBuilder modelBuilder, string[] ignoredNamespaces = null)
        {
            ignoredNamespaces = ignoredNamespaces ?? new string[] { };
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var name = GetTableNameWithNamespace(entityType.Name, ignoredNamespaces);
                entityType.Relational().TableName = name;
            }
            return modelBuilder;
        }

        public static string GetTableNameWithNamespace(string entityName, string[] ignoredNamespaces)
        {
            var parts = entityName.Split('.').Reverse().ToArray();
            //Do not rename classes without namespace
            if (parts.Count() < 2) return entityName;

            var className = parts.First();
            var genericDefinitionStartIndex = className.IndexOf('<');
            if (genericDefinitionStartIndex >= 0)
            {
                className = className.Substring(0, genericDefinitionStartIndex);
            }
            var namespaceName = GetLastNamespace(parts.Skip(1).ToArray(), ignoredNamespaces);

            //Namespace and class name must be set, otherwise default name will be used
            if (string.IsNullOrWhiteSpace(namespaceName)) return className;
            return namespaceName + "_" + className;
        }

        private static string GetLastNamespace(string[] namespaces, string[] ignoredNamespaces)
        {
            foreach (var name in namespaces)
            {
                if (!string.IsNullOrWhiteSpace(name) && !ignoredNamespaces.Contains(name))
                {
                    return name;
                }
            }
            return null;
        }
    }
}
