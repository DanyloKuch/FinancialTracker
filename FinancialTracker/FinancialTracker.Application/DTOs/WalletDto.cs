using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialTracker.Application.DTOs
{
    public record WalletRequest(
          string Name,
          string Type,
          decimal Balance,
          string CurrencyCode
      );


    public record WalletResponse(
        Guid Id,
        string Name,
        string Type,
        decimal Balance,
        string CurrencyCode,
        DateTime UpdatedAt
    );
}
