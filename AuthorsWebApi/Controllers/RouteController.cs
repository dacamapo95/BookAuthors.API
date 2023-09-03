using AuthorsWebApi.DataTransferObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RouteController : Controller
    {
        private readonly IAuthorizationService authorizationService;

        public RouteController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DataHATEOAS>>> Get()
        {
            var data = new List<DataHATEOAS>();

            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");
            data
                .AddRange(new List<DataHATEOAS>()
                          {
                            new DataHATEOAS(link: Url.Link( "GetRoot", new { }), "self", "GET"),
                            new DataHATEOAS(link: Url.Link( "GetAuthors", new { }), "Get Authors", "GET")
      

                          });

            if (isAdmin.Succeeded)
                data.AddRange(new List<DataHATEOAS>()
                    {
                        new DataHATEOAS(link: Url.Link( "GetAuthorById", new { }), "Get Author By Id", "GET"),
                        new DataHATEOAS(link: Url.Link( "UpdateAuthor", new { }), "Update Author", "Put")
                    });
                    

            return data;
        }
    }
}
