﻿// ***********************************************************************
// Author           : Bill Barge
// Created          : 08-02-2024
//
// Last Modified By : Bill Barge
// Last Modified On : 08-02-2024
// ***********************************************************************
// <copyright file="FilteringEngine.cs" company="N/A">
//     Copyright (c) N/A. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Linq.Expressions;
using System.Reflection;
using CrossCutting.Extensions;
using ProductManager.Glue.Interfaces.Models;

namespace ProductManager.Service.Utilities;

/// <summary>
/// Class FilteringEngine.
/// </summary>
internal static class FilteringEngine
{
    /// <summary>
    /// Builds the lambda expression used in a where clause.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filters">The filters.</param>
    /// <returns>Func&lt;T, System.Boolean&gt;.</returns>
    internal static Func<T, bool> BuildLambdaExpression<T>(Dictionary<string, IFilterMetaData[]> filters)
    {
        Type dataType = typeof(T);
        ParameterExpression pe = Expression.Parameter(dataType, dataType.Name[..1]);
        Expression e1 = BuildExpressions(pe, filters);

        Type func = typeof(Func<T,bool>);
        if (e1.IsEmpty())
        {
            e1 = Expression.Constant(true);
        }
        LambdaExpression predicate = Expression.Lambda(func, e1, pe);
        return (Func<T, bool>)predicate.Compile();
    }

    /// <summary>
    /// Builds the expressions.
    /// </summary>
    /// <param name="pe">The pe.</param>
    /// <param name="filters">The filters.</param>
    /// <returns>Expression.</returns>
    private static Expression BuildExpressions(ParameterExpression pe, Dictionary<string, IFilterMetaData[]> filters)
    {
        Expression returnValue = null!;
        Expression propertyExpression = null!;
        int keyCount = filters.Count;
        int keySub = 0;
        foreach (string propertyName in filters.Keys)
        {
            IFilterMetaData[] propertyFilters = filters[propertyName];
            for (int filterSub = 0; filterSub < propertyFilters.Length; filterSub++)
            {
                IFilterMetaData propertyFilter = propertyFilters[filterSub];
                if ((propertyFilter.LogicalOperator ?? string.Empty).IsEmpty() || (propertyFilter.MatchMode ?? string.Empty).IsEmpty() ||
                    (propertyFilter.SearchValue ?? string.Empty).IsEmpty())
                {
                    continue;
                }
                Expression nodeExpression = BuildPropertyExpression(pe, propertyName, propertyFilter.MatchMode!, propertyFilter.SearchValue!);
                if (filterSub > 0 && propertyFilters.Length > 1)
                {
                    // ReSharper disable once ConvertSwitchStatementToSwitchExpression
                    switch (propertyFilter.LogicalOperator)
                    {
                        case "and":
                            propertyExpression = Expression.And(nodeExpression, propertyExpression);
                            break;
                        case "or":
                            propertyExpression = Expression.Or(nodeExpression, propertyExpression);
                            break;
                    }
                }
                else
                {
                    propertyExpression = nodeExpression;
                }
            }

            if (keySub > 0 && keyCount > 1)
            {
                returnValue = Expression.And(propertyExpression, returnValue);
            }
            else
            {
                returnValue = propertyExpression;
            }

            keySub++;
        }

        return returnValue;
    }

    /// <summary>
    /// Builds the property expression.
    /// </summary>
    /// <param name="pe">The pe.</param>
    /// <param name="incomingProperty">The incoming property.</param>
    /// <param name="comparisonOperation">The comparison operation.</param>
    /// <param name="compareValue">The compare value.</param>
    /// <returns>Expression.</returns>
    private static Expression BuildPropertyExpression(Expression pe, string incomingProperty, string comparisonOperation, object compareValue)
    {
#pragma warning disable IDE0059
        Expression returnValue = null!;

        returnValue = BuildValueExpression(pe, incomingProperty, comparisonOperation, compareValue);

        return returnValue;
    }

    /// <summary>
    /// Builds the value expression.
    /// </summary>
    /// <param name="workingExpression">The working expression.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="comparisonOperation">The comparison operation.</param>
    /// <param name="compareValue">The compare value.</param>
    /// <returns>Expression.</returns>
    private static Expression BuildValueExpression(Expression workingExpression, string propertyName, string comparisonOperation, object compareValue)
    {
        workingExpression = Expression.Property(workingExpression, propertyName);
        Expression valueExpression = BuildValueExpression(compareValue, ref workingExpression);
        Expression returnValue = BuildComparisonExpression(comparisonOperation, workingExpression, valueExpression);
        return returnValue;
    }

    /// <summary>
    /// Builds the value expression.
    /// </summary>
    /// <param name="compareValue">The compare value.</param>
    /// <param name="workingExpression">The working expression.</param>
    /// <returns>Expression.</returns>
    private static Expression BuildValueExpression(object compareValue, ref Expression workingExpression)
    {
        Expression valueExpression;
        if (workingExpression.Type == typeof(decimal))
        {
            valueExpression = Expression.Constant(decimal.Parse(compareValue.ToString() ?? throw new InvalidOperationException())); // this will be the value
        }
        else if (workingExpression.Type == typeof(DateTime))
        {
            valueExpression = Expression.Constant(DateTime.Parse(compareValue.ToString() ?? throw new InvalidOperationException())); // this will be the value
        }
        else if (workingExpression.Type == typeof(int))
        {
            valueExpression = Expression.Constant(int.Parse(compareValue.ToString() ?? throw new InvalidOperationException())); // this will be the value
        }
        else if (workingExpression.Type == typeof(string))
        {
            valueExpression = Expression.Constant(compareValue.ToString(), typeof(string));//Expression.Constant(0); // this will be the value
        }
        else
        {
            valueExpression = Expression.Constant(compareValue); // this will be the value
        }

        return valueExpression;
    }

    /// <summary>
    /// Builds the comparison expression.
    /// </summary>
    /// <param name="comparisonOperation">The comparison operation.</param>
    /// <param name="workingExpression">The working expression.</param>
    /// <param name="valueExpression">The value expression.</param>
    /// <returns>Expression.</returns>
    /// <exception cref="Exception">Malformed comparison</exception>
    /// <exception cref="ArgumentOutOfRangeException">comparisonOperation - null</exception>
    private static Expression BuildComparisonExpression(string comparisonOperation, Expression workingExpression, Expression valueExpression)
    {
        Expression returnValue;
        Expression left;
        MethodInfo method;
        switch (comparisonOperation.Trim())
        {
            case "":
                throw new Exception("Malformed comparison");
            case "equals":
                returnValue = Expression.Equal(workingExpression, valueExpression);
                break;
            case "notEquals":
                returnValue = Expression.NotEqual(workingExpression, valueExpression);
                break;
            case "startsWith":
                if (valueExpression.Type == typeof(string))
                {
                    left = workingExpression;
                    method = typeof(string).GetMethods().First(m => m.Name == "StartsWith");
                    workingExpression = Expression.Call(left,
                        method,
                        valueExpression);
                    returnValue = workingExpression;
                }
                else
                {
                    returnValue = Expression.Constant(true);
                }
                break;
            case "contains":
                if (valueExpression.Type == typeof(string))
                {
                    left = workingExpression;
                    method = typeof(string).GetMethods().First(m => m.Name == "Contains");
                    workingExpression = Expression.Call(left,
                        method,
                        valueExpression);
                    returnValue = workingExpression;
                }
                else
                {
                    returnValue = Expression.Constant(true);
                }
                break;
            case "notContains":
                if (valueExpression.Type == typeof(string))
                {
                    left = workingExpression;
                    method = typeof(string).GetMethods().First(m => m.Name == "Contains");
                    workingExpression = Expression.Call(left,
                        method,
                        valueExpression);
                    returnValue = Expression.Not(workingExpression);
                }
                else
                {
                    returnValue = Expression.Constant(true);
                }
                break;
            case "endsWith":
                if (valueExpression.Type == typeof(string))
                {
                    left = workingExpression;
                    method = typeof(string).GetMethods().First(m => m.Name == "EndsWith");
                    workingExpression = Expression.Call(left,
                        method,
                        valueExpression);
                    returnValue = workingExpression;
                }
                else
                {
                    returnValue = Expression.Constant(true);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(comparisonOperation), comparisonOperation, null);
        }

        return returnValue;
    }
}