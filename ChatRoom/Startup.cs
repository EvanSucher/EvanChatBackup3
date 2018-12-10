// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.SignalR.Samples.ChatRoom
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR()
                    //REPLACE THE STRING HERE WITH THE AZURE SIGNALR CONNECTION STRING (In Settings/Keys for the Azure SignalR Resource)
                    //NOTE: In Production Code you'd save this value securely: see https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-quickstart-dotnet-core#add-secret-manager-to-the-project
                    .AddAzureSignalR("Endpoint=https://snaker.service.signalr.net;AccessKey=UzAOxJglIWkzcnt+Tltnkj5QianVm9fop4hLvU2yWrk=;Version=1.0;");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
            app.UseFileServer();
            app.UseAzureSignalR(routes =>
            {
                //Note: "Maps" the received URL request to hub that handles the request.
                routes.MapHub<Chat>("/chat");
            });
        }
    }
}
