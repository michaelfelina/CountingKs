using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Filters;
using CountingKs.Models;

namespace CountingKs.Controllers
{

    public class FoodsController : BaseApiController
    {
        private const int pageSize = 50;
        public FoodsController(ICountingKsRepository repo) : base(repo)
        {

        }

        public object Get(bool includeMeasures = true, int page = 0)
        {
            IQueryable<Food> query;
            if (includeMeasures)
            {
                query = repo.GetAllFoodsWithMeasures();
            }
            else
            {
                query = repo.GetAllFoods();
            }

            IOrderedQueryable<Food> baseQuery = query.OrderBy(f => f.Description);

            var totalCount = baseQuery.Count();
            var totalPages = Math.Ceiling((double)totalCount/pageSize);

            var helper = new UrlHelper(Request);
            var prevUrl = page > 0 ?  helper.Link("Food", new {page = page - 1}) : "";
            var nextUrl = page < totalPages - 1 ? helper.Link("Food", new { page = page + 1 }) : "";

            var results = baseQuery.Skip(pageSize*page)
                .Take(pageSize)
                .ToList()
                .Select(f => modelFactory.Create(f));

            return new
            {
                PreviousPage = prevUrl,
                NextPage = nextUrl,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Results = results
            };
        }

        public FoodModel Get(int foodId)
        {
            return modelFactory.Create(repo.GetFood(foodId));
        }

    }
}
