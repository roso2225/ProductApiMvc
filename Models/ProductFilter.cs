﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductApiMvc.Models
{
    public class ProductFilter
    {
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}