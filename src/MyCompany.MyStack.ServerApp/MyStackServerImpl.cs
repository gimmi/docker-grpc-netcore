﻿using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Mycompany.Mystack;

namespace MyCompany.MyStack.ServerApp
{
    public class MyStackServerImpl : MyStackServer.MyStackServerBase
    {
        private readonly ILogger<MyStackServerImpl> _logger;

        public MyStackServerImpl(ILogger<MyStackServerImpl> logger)
        {
            _logger = logger;
        }

        public override Task<EchoResponse> Echo(EchoRequest request, ServerCallContext context)
        {
            _logger.LogInformation("=> Echo({})", request.Message);
            return Task.FromResult(new EchoResponse { Message = request.Message });
        }

        public override Task<FailResponse> Fail(FailRequest request, ServerCallContext context)
        {
            _logger.LogInformation("=> Fail({})", request.Message);
            throw new ApplicationException(request.Message);
        }
    }
}