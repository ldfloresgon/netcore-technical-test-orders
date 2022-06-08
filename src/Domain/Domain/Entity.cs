namespace Core.Domain
{
    public class Entity<T>
    {
        public T Id { get; set; }

        public Entity(T id)
        {
            Id = id;
        }
    }
}