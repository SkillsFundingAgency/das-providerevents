namespace SFA.DAS.Payments.Events.Domain
{
    public class PageOfResults<T>
    {
        public int PageNumber { get; set; }
        public int TotalNumberOfPages { get; set; }
        public T[] Items { get; set; }
    }
}