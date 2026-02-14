using BibliotecaApi2.Dtos;
using BibliotecaApi2.Models;
using BibliotecaApi2.Responses;
using BibliotecaApi2.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecaApi2.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly BibliotecaApi2DbContext _context;
        private readonly IConfiguration _config;

        public AuthorizationController(BibliotecaApi2DbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        
    }
}