using System;

namespace MyCompany.MyStack.ClientApp
{
    public class AppSettings
    {
        public string ServerAppTarget => Environment.GetEnvironmentVariable("MYCOMPANY_MYSTACK_SERVERAPP_TARGET") ?? "localhost:50052";
    }
}
