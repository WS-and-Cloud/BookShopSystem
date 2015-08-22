namespace BookShop.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using System.Web.OData;

    using BookShop.Data;
    using BookShop.Services.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    [RoutePrefix("api/users")]
    public class UsersController : BaseApiController
    {
        public UsersController()
            : base(new BookShopData())
        {
        }

        [EnableQuery]
        [HttpGet]
        [Route("{username}/purchases")]
        public IHttpActionResult GetUserPurchases(string username)
        {
            var user = this.Data.ApplicationUsers.All().FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return this.NotFound();
            }

            var purchases =
                this.Data.Purchases.All()
                    .Where(p => p.ApplicationUserId == new Guid(user.Id))
                    .OrderBy(p => p.DateOfPurchase)
                    .Select(p => new
                                 {
                                     Username = p.ApplicationUser.UserName, 
                                     p.Book.Title,
                                     p.Price,
                                     p.DateOfPurchase,
                                     p.IsRecalled
                                 });
            return this.Ok(purchases);
        }

        [Authorize(Roles = AdministratorRole)]
        [HttpPut]
        [Route("{username}/roles")]
        public IHttpActionResult UpdateUserRole(string username, [FromBody]AddUserRoleBindingModel roleBindingModel)
        {
            var user = this.Data.ApplicationUsers.All().FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return this.NotFound();
            }

            var role = this.Data.Roles.All().FirstOrDefault(r => r.Name == roleBindingModel.RoleName);
            if (role == null)
            {
                return this.NotFound();
            }

            user.Roles.Add(new IdentityUserRole
                           {
                               RoleId = role.Id,
                               UserId = user.Id
                           });
            this.Data.SaveChanges();

            return this.Ok("Successfully added role-" + roleBindingModel.RoleName);
        }

        [Authorize(Roles = AdministratorRole)]
        [HttpDelete]
        [Route("{username}/roles")]
        public IHttpActionResult DeleteUserRole(string username, [FromBody]AddUserRoleBindingModel roleBindingModel)
        {
            var user = this.Data.ApplicationUsers.All().FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return this.NotFound();
            }

            var role = this.Data.Roles.All().FirstOrDefault(r => r.Name == roleBindingModel.RoleName);
            if (role == null)
            {
                return this.NotFound();
            }

            var userRole = user.Roles.FirstOrDefault(ur => ur.RoleId == role.Id);
            if (userRole == null)
            {
                return this.BadRequest("User does not have such role");
            }

            user.Roles.Remove(userRole);

            this.Data.SaveChanges();

            return this.Ok("Successfully removed role-" + roleBindingModel.RoleName);
        }
    }
}