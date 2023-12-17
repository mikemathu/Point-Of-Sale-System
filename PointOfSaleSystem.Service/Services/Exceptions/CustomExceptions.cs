namespace PointOfSaleSystem.Service.Services.Exceptions
{
    public class EmptyDataResultException : ApplicationException //NoDataFoundException
    {
        public EmptyDataResultException() { }
    }
    public class ItemNotFoundException : ApplicationException
    {
        public ItemNotFoundException(string? message) : base(message) { }
    }
    public class ActionFailedException : ApplicationException
    {
        public ActionFailedException(string? message) : base(message) { }
    }
}