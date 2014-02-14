using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ElasticDemo.Startup))]
namespace ElasticDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
