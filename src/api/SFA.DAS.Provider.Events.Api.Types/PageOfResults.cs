namespace SFA.DAS.Provider.Events.Api.Types
{
    public class PageOfResults<T>
    {
        public int PageNumber { get; set; }
        public int TotalNumberOfPages { get; set; }
        public T[] Items { get; set; }
    }
}