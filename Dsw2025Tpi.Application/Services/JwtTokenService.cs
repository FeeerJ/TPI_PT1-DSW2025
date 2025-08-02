using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace Dsw2025Tpi.Application.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public JwtTokenService(IConfiguration config, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public string GenerateToken(string username, string role)
        {
            var jwtConfig = _config.GetSection("Jwt");
            var keyText = jwtConfig["Key"] ?? throw new ArgumentNullException("Jwt Key");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Sub,username),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), /*Se podrian agregar mas claims*/
               new Claim(ClaimTypes.Role, role) // Ejemplo de claim personalizado
              
            };

         
            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtConfig["ExpireMinutes"] ?? "60")),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException("Los campos no pueden estar vacios.");
            }
            if (!ValidateEmail(request.Email))
            {
                throw new ArgumentException("El mail ingresado no es valido.");
            }
            if (await _userManager.FindByNameAsync(request.Username) != null)
            {
                throw new ArgumentException("El nombre de usuario ya esta en uso.");
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                throw new ArgumentException("El correo electronico ya esta en uso.");
            }
            var user = new IdentityUser { UserName = request.Username, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password); 
            if (!result.Succeeded)
            {
                throw new ArgumentException("Argumentos invalidos o incompletos");
            }
            await _userManager.AddToRoleAsync(user, "User"); // Asignar rol por defecto
            return new RegisterResponse(user.UserName, user.Email);
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (result.IsLockedOut) throw new UnauthorizedAccessException("Usuario bloqueado temporalmente por intentos fallidos.");
            
            if (!result.Succeeded) throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
            
            var userRoles = await _userManager.GetRolesAsync(user);
            var role = userRoles.FirstOrDefault() ?? "User"; 
            var token = GenerateToken(request.Username, role);
            return new LoginResponse(user.UserName, token);
        }

        public bool ValidateEmail(string email)
        {
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);

        }
     }
    }
