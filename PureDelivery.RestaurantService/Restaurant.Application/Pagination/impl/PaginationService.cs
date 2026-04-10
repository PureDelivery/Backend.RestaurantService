using PureDelivery.Shared.Contracts.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Pagination.impl
{
    public class PaginationService : IPaginationService
    {
        public PagedResult<T> CreatePagedResult<T>(IEnumerable<T> items, int page, int pageSize)
        {
            var itemsList = items.ToList();
            var totalCount = itemsList.Count;

            var pagedItems = itemsList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
