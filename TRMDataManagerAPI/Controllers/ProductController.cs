﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDLL.DataAccess;
using TRMDLL.Models;

namespace TRMDataManagerAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : ApiController
    {
        public List<ProductModel> Get()
        {
            ProductData data = new ProductData();

            return data.GetProducts();
        }
    }
}
