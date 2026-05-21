using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hotel.Data;
using hotel.Models;
using BCrypt.Net;
using System;
using Microsoft.AspNetCore.Identity;
using hotel.Jwt;
using Microsoft.AspNetCore.Authentication;
using hotel.Services;
using hotel.DTOs.AuthDTOs.Requests;
using System.Reflection.Metadata.Ecma335;

namespace hotel.Controllers
{
    [Route("/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AutenthicationServices _authenticationServices;

        private readonly AppDbContext _context;

        private readonly TokenProvider _tokenProvider;

        public AuthenticationController(AppDbContext context, TokenProvider tokenProvider, AutenthicationServices authenticationServices)
        {
            _context = context;
            _tokenProvider = tokenProvider;
            _authenticationServices = authenticationServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDTO userDTO)
        {
            var user = await _authenticationServices.RegisterUser(userDTO);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return BadRequest("Username or email already exists.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDTO loginDTO)
        {
            var user = await _authenticationServices.LoginUser(loginDTO);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                var errormsg = new LoginUserDTO
                {
                    message = "Invalid username or password."
                };
                return Unauthorized(errormsg);
            }
        }
    }
}
