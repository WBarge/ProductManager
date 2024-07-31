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

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Service.Models.Result;

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
        /// </summary>
        /// <param name="page" optional="true">The page number is one based. Will default to 1 if null or less than 1</param>
        /// <param name="pageSize" optional="true">The number of items on a page. Will cause all data to be returned if null or less than 1</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        public IActionResult Get(int? page,int? pageSize)
        {
            if (page is null or < 1 )
            {
                page = 1;
            }

            if (pageSize is null or < 1)
            {
                pageSize = DATA_SIZE;
            }

            var returnValue = new { data = GenerateData(page.Value, pageSize.Value), totalRecordSize = DATA_SIZE };
            return new OkObjectResult(returnValue);
        }

        /// <summary>
        /// Generates the data.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>IEnumerable&lt;TestData&gt;.</returns>
        private static IEnumerable<TestData> GenerateData(int page,int pageSize)
        {
            List<TestData> data = [];
            for (int i = 0; i < DATA_SIZE; i++)
            {
                data.Add(new TestData (  i, (i+1).ToString()));
            }

            return data.Skip((page-1)*pageSize).Take(pageSize);
        }

    }
}
