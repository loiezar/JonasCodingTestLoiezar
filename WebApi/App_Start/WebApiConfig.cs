using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Web.Http;


namespace WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Web API configuration and services
            var jsonFormatter = config.Formatters.JsonFormatter;

            // Set JSON serializer settings for Newtonsoft.Json
            jsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            // Optional: Use camelCase for JSON property names
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Remove the XML formatter to return JSON only
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            //Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
