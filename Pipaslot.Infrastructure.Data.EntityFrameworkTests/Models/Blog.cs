namespace Pipaslot.Infrastructure.Data.EntityFrameworkTests.Models
{

    public class Blog : IEntity<int>, IBlog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        object IBlog.Id { get => Id; set => Id = (int)value; }

        public Blog()
        {
        }

        public Blog(string name)
        {
            Name = name;
        }
    }

    public interface IBlog
    {
        object Id { get; set; }
        string Name { get; set; }
    }
}
