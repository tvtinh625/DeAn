using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;

namespace DeAn.Models.ViewModel
{
    public class ProductSearchVM
    {
        public string SearchTerm { get; set; }
        public decimal? minPrice { get; set; }
        public decimal? maxPrice { get; set; }
        public string sortOrder { get; set; }
        public int Pagenumber { get; set; }
        public int Pagesize { get; set; } = 10;
        public PagedList.IPagedList<Product> Products { get; set; }
    }
}
