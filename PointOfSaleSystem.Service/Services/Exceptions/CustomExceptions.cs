namespace PointOfSaleSystem.Service.Services.Exceptions
{
    public class NullException : ApplicationException //NoDataFoundException
    {
        public NullException() { }
    }
    public class ValidationRowNotFoudException : ApplicationException
    {
        public ValidationRowNotFoudException(string? message) : base(message) { }
    }
    public class FalseException : ApplicationException
    {
        public FalseException(string? message) : base(message) { }
    }
}