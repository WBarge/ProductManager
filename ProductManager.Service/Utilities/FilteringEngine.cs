// ***********************************************************************
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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProductManager.Service.Models.Request;
using System.Linq.Expressions;
using System.Reflection;
using CrossCutting.Extensions;
using ProductManager.Service.Models.Result;

namespace ProductManager.Service.Utilities;

/// <summary>
/// Class FilteringEngine.
/// </summary>
public static class FilteringEngine
{
    /// <summary>
    /// Filters the specified filters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">The data.</param>
    /// <param name="filters">The filters.</param>
    /// <returns>List&lt;T&gt;.</returns>
    public static List<T> Filter<T>(this List<T> data, Dictionary<string, FilterMetaData[]> filters)
        where T : class
    {
        IQueryable<T> finalQuery = null!;
        Type dataType = typeof(T);

        //this is so we get the right Iqueryable setup incase there are no related objects
        //it does not harm anything if there are related objects
        Expression queryExpression = ((IQueryable<T>)data.AsQueryable()).Expression;
        IQueryable<T> query = ((IQueryable<T>)data.AsQueryable()).Provider.CreateQuery<T>(queryExpression);

        Type queryableType = typeof(Queryable);

        //cast the results to the right type so the where clause will work
        MethodInfo castMethodRef = queryableType.GetMethods()
            .First(m => m.Name == "Cast" && m.IsGenericMethodDefinition);

        MethodInfo castMethod = castMethodRef.MakeGenericMethod(dataType);

        MethodCallExpression castCallExpression = Expression.Call(
            castMethod,
            query.Expression);

        if (filters != null)
        {
            MethodCallExpression whereCallExpression =
                BuildWhereCallExpression(filters, dataType, queryableType, castCallExpression);

            finalQuery = query.Provider.CreateQuery<T>(whereCallExpression);
        }
        else
        {
            finalQuery = query.Provider.CreateQuery<T>(castCallExpression);
        }
#pragma warning disable IDE0305
        return finalQuery.ToList();
    }

    /// <summary>
    /// Builds the where call expression.
    /// </summary>
    /// <param name="filters">The filters.</param>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="queryableType">Type of the queryable.</param>
    /// <param name="castCallExpression">The cast call expression.</param>
    /// <returns>MethodCallExpression.</returns>
    private static MethodCallExpression BuildWhereCallExpression(Dictionary<string, FilterMetaData[]> filters,
        Type dataType, Type queryableType, MethodCallExpression castCallExpression)
    {
        //setup the conditions
        ParameterExpression pe = Expression.Parameter(dataType, dataType.Name[..1]);
        Expression e1 = BuildExpressions(pe, filters);

        Type func = typeof(Func<,>);
        Type genericFunc = func.MakeGenericType(dataType, typeof(bool));

        LambdaExpression predicate = Expression.Lambda(genericFunc, e1, pe);

        //create the where clause
        MethodInfo whereMethodRef = queryableType.GetMethods()
            .First(m =>
            {
                List<ParameterInfo> parameters = m.GetParameters().ToList();
                return m.Name == "Where" && m.IsGenericMethodDefinition && parameters.Count == 2;
            });

        MethodInfo whereMethod = whereMethodRef.MakeGenericMethod(dataType);

        MethodCallExpression whereCallExpression = Expression.Call(
            whereMethod,
            castCallExpression,
            predicate);

        return whereCallExpression;
    }

    /// <summary>
    /// Builds the expressions.
    /// </summary>
    /// <param name="pe">The pe.</param>
    /// <param name="filters">The filters.</param>
    /// <returns>Expression.</returns>
    private static Expression BuildExpressions(ParameterExpression pe, Dictionary<string, FilterMetaData[]> filters)
    {
        //if (string.IsNullOrEmpty(filters.Keys) || string.IsNullOrWhiteSpace(filters.Property))
        //{
        //    throw new Exception("Malformed filter");
        //}
        //if (filters.Comparison == CompareOperation.None)
        //{
        //    throw new Exception("Malformed filter");
        //}
        //if (filters.Operator != LogicalOperator.None && filters.SubFilter == null)
        //{
        //    throw new Exception("Malformed filter");
        //}


        Expression returnValue = null!;
        Expression propertyExpression = null!;
        int keyCount = filters.Count;
        int keySub = 0;
        foreach (string propertyName in filters.Keys)
        {
            FilterMetaData[] propertyFilters = filters[propertyName];
            for (int filterSub =0;filterSub<propertyFilters.Length;filterSub++ )
            {
                FilterMetaData propertyFilter = propertyFilters[filterSub];
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
                            propertyExpression = Expression.And(nodeExpression, propertyExpression!);
                            break;
                        case "or":
                            propertyExpression = Expression.Or(nodeExpression, propertyExpression!);
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
                returnValue = Expression.And(propertyExpression, returnValue!);
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


    //starts with
    //contains
    //not contains
    //ends with
    //equals
    //not equals
}