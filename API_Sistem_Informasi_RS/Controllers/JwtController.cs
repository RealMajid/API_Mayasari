using API_Sistem_Informasi_RS.Models;
using API_Sistem_Informasi_RS.Security;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace API_Sistem_Informasi_RS.Controllers
{
    [RoutePrefix("jwt")]
    public class JwtController : ApiController
    {
        private mayasariEntities db = new mayasariEntities();

        [HttpGet]
        [Route("notok")]
        public IHttpActionResult NotAuthenticated() => Unauthorized();

        [HttpGet]
        [Authorize]
        [Route("ok")]
        public IHttpActionResult Authenticated() => Ok("Authenticated");

        [HttpPost]
        [Route("jwt")]
        public async Task<IHttpActionResult> Authenticate(Auth user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password)) return BadRequest("Username/password tidak valid");

            var loginrequest = new Auth
            {
                Username = user.Username.ToLower(),
                Password = user.Password
            };

            var appUsers = db.USERS.Where(u => u.USERNAME == loginrequest.Username).FirstOrDefault();
            if (appUsers == null) return BadRequest("username tidak ditemukan");
            if (appUsers.ACTIVE_YN == "N") return BadRequest("Username terkunci");

            var encrptyPass = Encryptor.Base64Encode(Encryptor.MD5Hash(loginrequest.Password));
            loginrequest.Username = Regex.Replace(loginrequest.Username.ToLower(), @"\s", "");

            var appUserLogin = db.USERS.Where(u => u.USERNAME == loginrequest.Username && u.PASSWORD == encrptyPass).FirstOrDefault();
            //var query = "SELECT * FROM USERS WHERE REPLACE(LOWER(USERNAME),' ','') = '@Username' AND PASSWORD = '@Password'";
            //var appUserLogin = db.USERS.SqlQuery(query, new SqlParameter("Username", loginrequest.Username),
            //                                                     new SqlParameter("Password", encrptyPass)).FirstOrDefault();

            // updateLastLogin
            appUsers.LAST_LOGIN = DateTime.Now;
            await db.SaveChangesAsync();

            if (appUserLogin == null) 
            {
                return Unauthorized();
            } else
            {
                var token = CreateToken(loginrequest.Username);
                //return the token
                return Ok(token);
            }
        }

        private string CreateToken(string email)
        {
            //Set issued at date
            DateTime issuedAt = DateTime.UtcNow;
            //set the time when it expires
            DateTime expires = DateTime.UtcNow.AddDays(7);

            var tokenHandler = new JwtSecurityTokenHandler();

            var appUser = db.USERS.Where(u => u.USERNAME == email).FirstOrDefault();
            //create a identity and add claims to the user which we want to log in
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, appUser.USERNAME),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToUniversalTime().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, appUser.USERNAME),
                new Claim("fullname", appUser.FULL_NAME),
                new Claim(ClaimTypes.Role, appUser.ROLES.FirstOrDefault().NAMA_ROLE),
            });

            const string secrectKey = "manajemenlayanankesehatanterintegrasi";
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secrectKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //Create the jwt (JSON Web Token)
            //Replace the issuer and audience with your URL (ex. http:localhost:12345)
            var token =
                (JwtSecurityToken)
                tokenHandler.CreateJwtSecurityToken(
                    issuer: "realmajid7",
                    audience: "realmajid7",
                    subject: claimsIdentity,
                    notBefore: issuedAt,
                    expires: expires,
                    signingCredentials: signingCredentials);

            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
