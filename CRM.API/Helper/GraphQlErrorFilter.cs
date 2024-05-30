using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace CRM.API.Helper
{
  public class GraphQlErrorFilter : IErrorFilter
  {
    public IError OnError(IError error)
    {
      if (error.Exception is DbUpdateException dbUpdateException)
      {
        var pgException = dbUpdateException.InnerException as PostgresException;
        if (pgException != null && pgException.SqlState == "23505")
        {
          return ErrorBuilder.FromError(error)
            .SetMessage("Violation of uniqueness of values.")
            .SetCode("UNIQUE_CONSTRAINT_VIOLATION")
            .RemoveException()
            .ClearExtensions()
            .ClearLocations()
            //.SetExtension("detail", pgException.Detail)
            .Build();
        }

        return ErrorBuilder.FromError(error)
          .SetMessage("A database error occurred.")
          .SetCode("DATABASE_ERROR")
          .RemoveException()
          .ClearExtensions()
          .ClearLocations()
          .Build();
      }

      if (error.Code == "AUTH_NOT_AUTHORIZED")
      {
        return ErrorBuilder.FromError(error)
          .SetMessage(error.Message)
          .SetCode("NOT_AUTHORIZED")
          .RemoveException()
          .ClearExtensions()
          .ClearLocations()
          .Build();
      }

      return error;
    }
  }
}