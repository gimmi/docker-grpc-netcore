﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace MyCompany.MyStack.MyRestApp
{
    public class ValuesController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFooService _fooService;

        public ValuesController(IHostingEnvironment hostingEnvironment, IFooService fooService)
        {
            _hostingEnvironment = hostingEnvironment;
            _fooService = fooService;
        }

        [HttpGet("api/values")]
        public object Get()
        {
            _fooService.LogSomething();
            return _hostingEnvironment;
        }

        [HttpGet("api/jsonserialization")]
        public object GetJsonSerialization()
        {
            return new {
                PascalCaseProperty = "value",
                camelCaseProperty = "value",
                dictionary = new Dictionary<string, object> {
                    ["PascalCaseKey"] = "value",
                    ["camelCaseKey"] = "value"
                },
                array = new[]{ "value1", "value2" },
                utcDateTime = new DateTime(2018, 5, 11, 8, 20, 31, 123, DateTimeKind.Utc),
                localDateTime = new DateTime(2018, 5, 11, 8, 20, 31, 123, DateTimeKind.Local),
            };
        }
    }
}
