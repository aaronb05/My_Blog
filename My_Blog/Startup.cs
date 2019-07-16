using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(My_Blog.Startup))]
namespace My_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
