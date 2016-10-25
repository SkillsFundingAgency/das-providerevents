namespace SFA.DAS.Payments.Events.Domain.Data.Entities
{
    public class PageOfEntities<T>
    {
        public int PageNumber { get; set; }
        public int NumberOfPages { get; set; }
        public T[] Items { get; set; }
    }
}
