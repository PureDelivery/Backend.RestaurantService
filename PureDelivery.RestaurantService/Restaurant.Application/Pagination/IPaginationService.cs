using PureDelivery.Shared.Contracts.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Pagination
{
    public interface IPaginationService
    {
        PagedResult<T> CreatePagedResult<T>(IEnumerable<T> items, int page, int pageSize);
    }
}
