using hotel.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using hotel.Models;
using hotel.Services;
using hotel.Controllers;
using hotel.Jwt;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Nest;
using hotel.DTOs.AuthDTOs.Requests;

namespace hotel.Services
{
    public class AutenthicationServices
    {
        private readonly TokenProvider _tokenProvider;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;
        public AutenthicationServices(AppDbContext context, IDistributedCache cache, TokenProvider tokenProvider)
        {
            _context = context;
            _tokenProvider = tokenProvider;
            _cache = cache;
        }

        public async Task<LoginUserDTO> LoginUser(LoginUserDTO loginDTO)
        {
            var user = await _context.utilizadores.FirstOrDefaultAsync(u => u.nome == loginDTO.username);
            if (user != null && BCrypt.Net.BCrypt.Verify(loginDTO.password_hash, user.password_hash))
            {
                var token = _tokenProvider.GenerateToken(user);
                return new LoginUserDTO
                {
                    token = token,
                    id = user.id,
                    username = user.nome,
                    email = user.email,
                    telefone = user.telefone,
                    tempo_inscrito = user.created_at
                };
            }
            else if (user == null)
            {
                var userByEmail = await _context.utilizadores.FirstOrDefaultAsync(u => u.email == loginDTO.username);
                if (userByEmail != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(loginDTO.password_hash, userByEmail.password_hash))
                    {
                        var token = _tokenProvider.GenerateToken(userByEmail);
                        return new LoginUserDTO
                        {
                            token = token,
                            id = userByEmail.id,
                            username = userByEmail.nome,
                            email = userByEmail.email,
                            telefone = userByEmail.telefone,
                            tempo_inscrito = userByEmail.created_at
                        };
                    }
                }
            }
                return null;
        }

        public async Task<RegisterUserDTO> RegisterUser(RegisterUserDTO registerDTO)
        {
            if (await _context.utilizadores.AnyAsync(u => u.nome == registerDTO.nome || u.email == registerDTO.email))
            {
                var errorMsg = new RegisterUserDTO
                {
                    message = "Username or email already exists."
                };
                return errorMsg;

            }
            var user = new utilizadores
            {
                id = registerDTO.id,
                nome = registerDTO.nome,
                email = registerDTO.email,
                telefone = registerDTO.telefone,
                password_hash = BCrypt.Net.BCrypt.HashPassword(registerDTO.password_hash),
                cargo = registerDTO.cargo,
                bio = registerDTO.bio,
                imagem_perfil = registerDTO.imagem_perfil,
                cidade = registerDTO.cidade
            };
            _context.utilizadores.Add(user);
            await _context.SaveChangesAsync();
            return registerDTO;
        }
    }
}
