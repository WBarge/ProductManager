// ***********************************************************************
// Author           : Bill Barge
// Created          : 07-29-2024
//
// Last Modified By : Bill Barge
// Last Modified On : 07-29-2024
// ***********************************************************************
// <copyright file="ProductsController.cs" company="N/A">
//     Copyright (c) N/A. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using CrossCutting.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Service.Models.Request;
using ProductManager.Service.Models.Result;
using ProductManager.Service.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductManager.Service.Controllers
{
    /// <summary>
    /// Class ProductsController.
    /// Implements the <see cref="ControllerBase" />
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        /// <summary>
        /// The data size
        /// </summary>
        const int DATA_SIZE = 1000;

        /// <summary>
        /// Gets the products using paging
        /// The request is a Post method - swagger does not currently support sending data in the body of a Get methods
        /// </summary>
        /// <param name="request"></param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public IActionResult GetProducts([FromBody] ProductListRequest request)
        {
            request ??= new ProductListRequest();

            if (request.Page is null or < 1 )
            {
                request.Page = 1;
            }

            if (request.PageSize is null or < 1)
            {
                request.PageSize = DATA_SIZE;
            }

            var returnValue = new { data = GenerateData(request.Page.Value, request.PageSize.Value,request.Filters), totalRecordSize = DATA_SIZE };
            return new OkObjectResult(returnValue);
        }

        /// <summary>
        /// Generates the data.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="filters">Any filters that need to be applied to the data</param>
        /// <returns>IEnumerable&lt;TestData&gt;.</returns>
        private static IEnumerable<TestData> GenerateData(int page,int pageSize,Dictionary<string,FilterMetaData[]>? filters)
        {
            List<TestData> data = [];
            for (int i = 0; i < DATA_SIZE; i++)
            {
                data.Add(new TestData (  i, (i+1).ToString()));
            }

            //starts with
            //contains
            //not contains
            //ends with
            //equals
            //not equals

            if (filters!.IsNotEmpty())
            {
                data = data.Filter(filters!);
            }




            return data.Skip((page-1)*pageSize).Take(pageSize);
        }

    }
}
