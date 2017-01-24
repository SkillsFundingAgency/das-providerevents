namespace SFA.DAS.Provider.Events.Domain.Data.Entities
{
    public class PageOfEntities<T>
    {
        public int PageNumber { get; set; }
        public int TotalNumberOfPages { get; set; }
        public T[] Items { get; set; }
    }
}
