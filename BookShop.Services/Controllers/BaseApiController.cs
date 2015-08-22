namespace BookShop.Services.Controllers
{
    using System.Web.Http;

    using BookShop.Data;

    public class BaseApiController : ApiController
    {
        protected const string AdministratorRole = "Admin";
        private readonly IBookShopData data;

        public BaseApiController()
            : this(new BookShopData())
        {
        }

        public BaseApiController(IBookShopData data)
        {
            this.data = data;
        }

        protected IBookShopData Data
        {
            get
            {
                return this.data;
            }
        }
    }
}
