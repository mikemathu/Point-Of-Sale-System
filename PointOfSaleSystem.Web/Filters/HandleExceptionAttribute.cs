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
            if (ex is NpgsqlException)//DB exceptions
            {
                if (ex.Message.Contains("value violates unique constraint"))
                {
                    return new ObjectResult("Sorry, the Name already exists. Use a different name.")
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
                    if (ex.Message.Contains("violates foreign key constraint \"cashflowcategoryfk\""))
                    {
                        return new ObjectResult("CashFlow Category is required.")
                        {
                            StatusCode = StatusCodes.Status422UnprocessableEntity
                        };
                    }
                    return new ObjectResult("Sorry, the action couldn't be completed due to the database constrain.")
                    {
                        StatusCode = StatusCodes.Status422UnprocessableEntity
                    };
                    //Possible constraints
                    //Accounts.Ledger.Accounts: Cannot delete Account as there are Sub accounts assiciated with it.
                    //Security.Roles : Cannot delete role as there are privileges associated with it.
                    //Accounts.Fiscal.Periods: This period has associated Journl voucher(s)
                    //Accounts.JV.JournalVouchers: This Journal Voucher has an associted entries
                    //Inventory.Inventory.Items : You are attempting to delete an ordered item. Consider deactivating the item instead
                    //Inventory.Inventory.CustomerOrders->customerOrderIDFk : You are attempting to delete order with item. Consider removing items first and try again.
                    //Accounts.Ledger.SubAccounts : You are attempting to delete sub account that is associated with a product.
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
            if (ex is EmptyDataResultException)
            {
                return new ObjectResult(new { data = new List<object>() });
            }
            if (ex is ItemNotFoundException)//validateId Id not found
            {
                return new ObjectResult(ex.Message)
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            if (ex is ActionFailedException)
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
            return new ObjectResult("Internal Server Error. Try again Later.")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
