using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DynamicFilters.Models;
using System.Linq.Dynamic.Core;
using DynamicFilters.Filters;

namespace DynamicFilters.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersContext _ordersContext;
        private readonly IMapper _mapper;
        public OrdersController(OrdersContext ordersContext, IMapper mapper) 
        {
            _ordersContext = ordersContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<OrderViewModel>> Get()
        {
            return await _ordersContext.Orders.ProjectTo<OrderViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpPost]
        [Route("filter")]
        public async Task<IList<OrderViewModel>> GetOrdersView(FilterDTO filter) 
        {
            //define the base query
            var ordersQuery = _ordersContext
                .Orders
                .ProjectTo<OrderViewModel>(_mapper.ConfigurationProvider);
            // apply the sort and filter
            ordersQuery = ordersQuery.ToFilterView(filter);
            // return the result
            return await ordersQuery.ToListAsync();
        }

    }
}
