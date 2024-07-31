// ***********************************************************************
// Author           : Bill Barge
// Created          : 07-29-2024
//
// Last Modified By : Bill Barge
// Last Modified On : 07-29-2024
// ***********************************************************************
// <copyright file="TestData.cs" company="N/A">
//     Copyright (c) N/A. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json;

namespace ProductManager.Service.Models.Result;

/// <summary>
/// Class TestData.
/// </summary>
public class TestData
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    public TestData(int id,string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
}