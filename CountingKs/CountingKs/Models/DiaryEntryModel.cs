using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CountingKs.Data.Entities;

namespace CountingKs.Models
{
    public class DiaryEntryModel
    {
        public DiaryModel Diary { get; set; }
        public FoodModel FoodItem { get; set; }
        public MeasureModel Measure { get; set; }
        public double Quantity { get; set; }
        public string Url { get; set; }
        public object FoodDescription { get; set; }
        public string MeasureDescription { get; set; }
        public string MeasureUrl { get; set; }
    }
}