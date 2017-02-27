using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;
using CountingKs.Data;
using CountingKs.Data.Entities;

namespace CountingKs.Models
{
    public class ModelFactory
    {
        private UrlHelper _urlHelper;
        private ICountingKsRepository _repo;

        public ModelFactory(HttpRequestMessage request, ICountingKsRepository repo)
        {
            _urlHelper = new UrlHelper(request);
            _repo = repo;
        }

        public FoodModel Create(Food food)
        {
            return new FoodModel()
            {
                Id =  food.Id,
                url = _urlHelper.Link("Food", new { foodId = food.Id }),
                Description = food.Description,
                Measures = food.Measures.Select(Create)
            };
        }

        public MeasureModel Create(Measure measure)
        {
            return new MeasureModel()
            {
                Id = measure.Id,
                url = _urlHelper.Link("Measures", new { foodId = measure.Food.Id, id = measure.Id }),
                Description = measure.Description,
                Calories = measure.Calories,
                Carbohydrates = measure.Carbohydrates,
                Cholestrol = measure.Cholestrol
            };
        }

        internal DiaryEntryModel Create(DiaryEntry d)
        {
            return new DiaryEntryModel()
            {
                Url = _urlHelper.Link("DiaryEntries", new { diaryid = d.Diary.CurrentDate.ToString("yyyy-MM-dd"), id = d.Id }),
                Quantity = d.Quantity,
                FoodDescription = d.FoodItem.Description,
                MeasureDescription = d.Measure.Description,
                MeasureUrl = _urlHelper.Link("Measures", new { foodid = d.FoodItem.Id, id = d.Measure.Id }),

                Diary = Create(d.Diary),
                FoodItem = Create(d.FoodItem),
                Measure = Create(d.Measure),
            };
        }

        internal DiaryModel Create(Diary d)
        {
            return new DiaryModel()
            {
                Url = _urlHelper.Link("Diaries",  new {diaryId = d.CurrentDate.ToString("yyyy-MM-dd")}),
                CurrentDate = d.CurrentDate
            };
        }

        public DiaryEntry Parse(DiaryEntryModel model)
        {
            try
            {
                var entry = new DiaryEntry();
                if (Math.Abs(model.Quantity - default(double)) > .01)
                {
                    entry.Quantity = model.Quantity;
                }

                if (!string.IsNullOrWhiteSpace(model.MeasureUrl))
                {
                    var uri = new Uri(model.MeasureUrl);
                    var measureId = int.Parse(uri.Segments.Last());
                    var measure = _repo.GetMeasure(measureId);

                    entry.Measure = measure;
                    entry.FoodItem = measure.Food;
                }

                return entry;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object CreateSummary(Diary diary)
        {
            return new DiarySummaryModel()
            {
                DiaryDate = diary.CurrentDate.Date,
                TotalCalories = Math.Round(diary.Entries.Sum(e => e.Measure.Calories * e.Quantity))
            };
        }
    }
}