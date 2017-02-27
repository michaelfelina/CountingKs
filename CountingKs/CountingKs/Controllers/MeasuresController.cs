using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    public class MeasuresController : BaseApiController
    {
        public MeasuresController(ICountingKsRepository repo) : base(repo)
        {
        }

        public IEnumerable<MeasureModel> Get(int foodId)
        {
            var results = repo.GetMeasuresForFood(foodId)
                                .ToList()
                                .Select(m => modelFactory.Create(m));

            return results;
        }

        public MeasureModel Get(int foodId, int id)
        {
            var results = repo.GetMeasure(id);
            if (results != null)
            {
                return results.Food.Id == foodId ? modelFactory.Create(results) : null;
            }
            return null;
        }
    }
}
