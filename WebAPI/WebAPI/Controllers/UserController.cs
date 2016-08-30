using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI.Models;
using WebAPI.Util;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    public class UserController : ApiController
    {
        static private List<User> _userList = new List<User>();
        private AccountSecurityService _ASS = new AccountSecurityService();

        public UserController ()
        {
            _userList.Add(new User { Id = 1, Username = "jerry", Password = "EC54164E6BA593C3EEFAEE53FCB196C4D9B06A6C", PasswordSalt = "I+OQh6nLY51Ze6yQjnwejQ==", JWTSecret = "zBJHgfpfkDl63EkuKCQ4Fw==" });
        }

        [HttpPost]
        [ResponseType(typeof(User))]
        [Route("api/User/Register")]
        public IHttpActionResult Register([FromBody]UserRegisterViewModel model)
        {
            if (_userList.Any(x => x.Username == model.Username.ToLower()))
            {
                ModelState.AddModelError("Username", "帳號已存在!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = new User();
            user.Id = _userList.Count + 1;
            user.Username = model.Username.ToLower();
            user.PasswordSalt = _ASS.GenerateSalt();
            user.Password = _ASS.CryptographyPassword(model.Password, user.PasswordSalt);
            user.JWTSecret = _ASS.GenerateSalt();

            _userList.Add(user);

            return Ok(user);
        }

        [HttpPost]
        [ResponseType(typeof(User))]
        [Route("api/User/Login")]
        public IHttpActionResult Login([FromBody]UserLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = _userList.FirstOrDefault(x => x.Username == model.Username.ToLower());

                if (user != null)
                {
                    var password = _ASS.CryptographyPassword(model.Password, user.PasswordSalt);

                    if (user.Password == password)
                    {
                        return Ok(user);
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "密碼錯誤!");
                    }
                }
                else
                {
                    ModelState.AddModelError("Username", "帳號不存在!");
                }
            }

            return BadRequest(ModelState);
        }

        [ResponseType(typeof(User))]
        //[Route("api/User/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            User user = _userList.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
