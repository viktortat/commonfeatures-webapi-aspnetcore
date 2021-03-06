﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApiDemo.Models;

namespace WebApiDemo.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionMiddleware> _logger;

        public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            var message = "Unhandled error";
            var code = "00009";
            var errors = new List<string>();

            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // log generic exception message (original error)
            _logger.LogError(exception, exception.Message);
            
            // Response
            await response.WriteAsync(JsonConvert.SerializeObject(new Error
            {
                Code = code,
                Message = message,
                Errors = errors
            }));
        }
    }
}
