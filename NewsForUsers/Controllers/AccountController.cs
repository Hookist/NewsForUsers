using log4net;
using Microsoft.AspNet.Identity;
using NewsForUsers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace NewsForUsers.Controllers
{
    /// <summary>
    /// Account operations like registration
    /// </summary>
    public class AccountController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private AuthRepository _repo = null;

        public AccountController()
        {
            _repo = new AuthRepository();
        }

        // POST api/Account/Register
        /// <summary>
        /// Registration new user
        /// </summary>
        /// <param name="userModel">UserModel</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/account/register")]
        public async Task<IHttpActionResult> Register(UserModel userModel)
        {
            Log.Debug("Registration new user");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _repo.RegisterUser(userModel);

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        /// <summary>
        /// Get Token for log in user
        /// Data pass via application/x-www-form-urlencoded
        /// </summary>
        /// <param name="userModel">LoginUserModel</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/Account/token")]
        public IHttpActionResult GetToken(LoginUserModel userModel)
        {
            Log.Debug("Get Token for log in user");
            return Ok();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
