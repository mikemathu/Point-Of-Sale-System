using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Npgsql;
using PointOfSaleSystem.Service.Services.Exceptions;
using System.Security.Authentication;

namespace PointOfSaleSystem.Web.Filters
{
    public class HandleExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception != null)
            {
                context.Result = HandleException(context.Exception);
                context.ExceptionHandled = true;
            }
        }

        private IActionResult HandleException(Exception ex)
        {
            if (ex is NpgsqlException)
            {
                if (ex.Message.Contains("duplicate key value violates unique constraint"))
                {
                    if (ex.Message.Contains("itemName"))
                    {
                        return new ObjectResult("Item with a similar Name already exists.")
                        {
                            StatusCode = StatusCodes.Status422UnprocessableEntity
                        };
                    }
                    return new ObjectResult("Name already exists.")
                    {
                        StatusCode = StatusCodes.Status422UnprocessableEntity
                    };
                }
                else if (ex.Message.Contains("value too long for type"))
                {
                    return new ObjectResult("Please ensure that your input values are not too long.")
                    {
                        StatusCode = StatusCodes.Status422UnprocessableEntity
                    };
                }
                else if (ex.Message.Contains("violates foreign key constraint"))
                {
                    if (ex.Message.Contains("Accounts.Ledger.Accounts"))
                    {
                        return new ObjectResult("Cannot delete Account as there are Sub accounts assiciated with it.")
                        {
                            StatusCode = StatusCodes.Status422UnprocessableEntity
                        };
                    }
                    if (ex.Message.Contains("Security.Roles"))
                    {
                        return new ObjectResult("Cannot delete role as there are privileges associated with it.")
                        {
                            StatusCode = StatusCodes.Status422UnprocessableEntity
                        };
                    }
                    if (ex.Message.Contains("Accounts.Fiscal.Periods"))
                    {
                        return new ObjectResult("This period has associated Journl voucher(s)")
                        {
                            StatusCode = StatusCodes.Status422UnprocessableEntity
                        };
                    }
                    if (ex.Message.Contains("Accounts.JV.JournalVouchers"))
                    {
                        return new ObjectResult("This Journal Voucher has an associted entries")
                        {
                            StatusCode = StatusCodes.Status422UnprocessableEntity
                        };
                    }
                    if (ex.Message.Contains("Inventory.Inventory.Items"))//itemIDFk
                    {
                        return new ObjectResult("You are attempting to delete an ordered item. Consider deactivating the item instead")
                        {
                            StatusCode = StatusCodes.Status422UnprocessableEntity
                        };
                    }
                    if (ex.Message.Contains("Inventory.Inventory.CustomerOrders"))//customerOrderIDFk
                    {
                        return new ObjectResult("You are attempting to delete order with item. Consider removing items first and try again.")
                        {
                            StatusCode = StatusCodes.Status422UnprocessableEntity
                        };
                    }
                    if (ex.Message.Contains("userName"))//customerOrderIDFk
                    {
                        return new ObjectResult("Name already exists.")
                        {
                            StatusCode = StatusCodes.Status422UnprocessableEntity
                        };
                    }
                }
            }
            if (ex is AuthenticationException)
            {
                return new ObjectResult(ex.Message)
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
            }
            if (ex is ArgumentException)//validation
            {
                return new ObjectResult(ex.Message)
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
            }
            if (ex is NullException)
            {
                return new ObjectResult(new { data = new List<object>() });
            }
            if (ex is ValidationRowNotFoudException)//validateId Id not found
            {
                return new ObjectResult(ex.Message)
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            if (ex is FalseException)
            {
                return new ObjectResult(ex.Message)
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity //StatusCodes.Status200OK: TODO
                };
            }
            if (ex is ArgumentOutOfRangeException)//flag
            {
                return new ObjectResult(ex.Message)
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
            }
            else
            {
                return new ObjectResult(ex.Message);
            }
            return new ObjectResult("Internal Server Error.")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
