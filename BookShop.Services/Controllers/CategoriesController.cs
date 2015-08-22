namespace BookShop.Services.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using System.Web.OData;

    using BookShop.Data;
    using BookShop.Models;
    using BookShop.Services.Models;

    [RoutePrefix("api/categories")]
    public class CategoriesController : BaseApiController
    {
        public CategoriesController()
            : base(new BookShopData())
        {
        }

        [EnableQuery]
        [HttpGet]
        public IHttpActionResult GetAllCategories()
        {
            var categories = this.Data.Categories.All().Select(c => new CategoryModel { Id = c.Id, Name = c.Name });

            return this.Ok(categories);
        }

        [HttpGet]
        public IHttpActionResult CategoryById(int id)
        {
            var category =
                this.Data.Categories.All()
                    .Where(c => c.Id == id)
                    .Select(c => new CategoryModel { Id = c.Id, Name = c.Name })
                    .FirstOrDefault();
            if (category == null)
            {
                return this.BadRequest("Invalid category id");
            }

            return this.Ok(category);
        }

        [Authorize(Roles = AdministratorRole)]
        [HttpPut]
        public IHttpActionResult EditCategoryById(int id, [FromBody]CategoryModel categoryModel)
        {
            var category = this.Data.Categories.All().FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return this.BadRequest("Invalid category id");
            }

            category.Name = categoryModel.Name;
            this.Data.Categories.Update(category);
            this.Data.SaveChanges();
            categoryModel.Id = category.Id;
            return this.Ok(categoryModel);
        }

        [Authorize(Roles = AdministratorRole)]
        [HttpDelete]
        public IHttpActionResult DeleteCategoryById(int id)
        {
            var category = this.Data.Categories.All().FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return this.BadRequest("Invalid category id");
            }

            this.Data.Categories.Delete(category);
            this.Data.SaveChanges();
            return this.Ok();
        }

        [Authorize(Roles = AdministratorRole)]
        [HttpPost]
        public IHttpActionResult CreateCategory([FromBody] CategoryModel categoryModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var category = new Category() { Name = categoryModel.Name };
            this.Data.Categories.Add(category);
            this.Data.SaveChanges();

            categoryModel.Id = category.Id;
            return this.Ok(categoryModel);
        }
    }
}
