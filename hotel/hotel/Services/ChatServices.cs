using hotel.Data;
using hotel.DTOs.MessageDTOs.Requests;
using hotel.DTOs.MessageDTOs.Responses;
using hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Nest;
using Newtonsoft.Json;

namespace hotel.Services
{
    public class ChatServices
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public ChatServices(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public async Task<List<ListContactsResponseDTO>> GetContacts(ListContactsDTO listContactsDTO)
        {
            string cacheKey = $"ListUsers_{listContactsDTO.id}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonConvert.DeserializeObject<List<ListContactsResponseDTO>>(cachedData);
            }

            // 1. Buscamos todas as conversas onde o utilizador participa (enviante OU receptor)
            // Usamos o .Include ou subqueries otimizadas para evitar consultas repetitivas
            var query = await _context.conversas
                .Where(c => c.id_enviante == listContactsDTO.id || c.id_receptor == listContactsDTO.id)
                .Select(c => new
                {
                    // Descobrimos quem é o "outro" na conversa
                    OutroId = c.id_enviante == listContactsDTO.id ? c.id_receptor : c.id_enviante,
                    c.id_propriedade
                })
                .Distinct() // 2. Remove duplicados (mesma pessoa + mesma propriedade)
                .ToListAsync();

            if (!query.Any()) return new List<ListContactsResponseDTO>();

            // 3. Mapeamos os detalhes (Nomes e Títulos) de forma eficiente
            var result = new List<ListContactsResponseDTO>();

            foreach (var item in query)
            {
                result.Add(new ListContactsResponseDTO
                {
                    id = item.OutroId,
                    id_propriedade = item.id_propriedade,
                    nome = _context.utilizadores
                        .Where(u => u.id == item.OutroId)
                        .Select(u => u.nome)
                        .FirstOrDefault() ?? "Utilizador Desconhecido",
                    nome_propriedade = _context.propriedades
                        .Where(p => p.id == item.id_propriedade)
                        .Select(p => p.titulo)
                        .FirstOrDefault() ?? "Propriedade Geral"
                });
            }

            // 4. Cache dos resultados
            await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(result), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return result;
        }
        public async Task<BasicListResponseDTO> GetBasicList(BasicListDTO basicListDTO)
        {
            var user = await _context.utilizadores.FirstOrDefaultAsync(u => u.nome == basicListDTO.nome);
            if (user != null)
            {
                return new BasicListResponseDTO
                {
                    id = user.id,
                    nome = user.nome
                };
            }
            return null;
        }

        public async Task<List<ListMessagesDTO>> GetMessages(ListMessagesDTO listMessagesDTO)
        {
            var messages = await _context.mensagens
                .Where(m => (m.id_enviado_por == listMessagesDTO.id_enviado_por && m.id_recebido_por == listMessagesDTO.id_recebido_por) ||
                            (m.id_enviado_por == listMessagesDTO.id_recebido_por && m.id_recebido_por == listMessagesDTO.id_enviado_por))
                .Select(m => new ListMessagesDTO
                {
                    id = m.id,
                    id_enviado_por = m.id_enviado_por,
                    texto_mensagem = m.texto_mensagem,
                    id_recebido_por = m.id_recebido_por
                })
                .ToListAsync();
            return messages;
        }

        public async Task SendMessage(InsertMessagesDTO insertMessagesDTO)
        {

            var message = new mensagens
            {
                id_enviado_por = insertMessagesDTO.id_enviado_por,
                texto_mensagem = insertMessagesDTO.texto_mensagem,
                id_recebido_por = insertMessagesDTO.id_recebido_por
            };
            _context.mensagens.Add(message);
            await _context.SaveChangesAsync();
            await ClearCache($"ListMessages_{insertMessagesDTO.id_enviado_por}_{insertMessagesDTO.id_recebido_por}");
        }

        public async Task<GenerateChatDTO> GenerateChat(GenerateChatDTO generateChatDTO)
        {
            var existingChat = await _context.conversas.FirstOrDefaultAsync(c =>
                (c.id_enviante == generateChatDTO.id_enviante && c.id_receptor == generateChatDTO.id_receptor && c.id_propriedade == generateChatDTO.id_propriedade) ||
                (c.id_enviante == generateChatDTO.id_receptor && c.id_receptor == generateChatDTO.id_enviante && c.id_propriedade == generateChatDTO.id_propriedade));
            if (existingChat != null)
            {
                return new GenerateChatDTO
                {
                    id_enviante = existingChat.id_enviante,
                    id_receptor = existingChat.id_receptor,
                    id_propriedade = existingChat.id_propriedade
                };
            }
            var newChat = new conversas
            {
                id_enviante = generateChatDTO.id_enviante,
                id_receptor = generateChatDTO.id_receptor,
                id_propriedade = generateChatDTO.id_propriedade
            };
            _context.conversas.Add(newChat);
            await _context.SaveChangesAsync();
            await ClearCache($"ListUsers_{generateChatDTO.id_enviante}");
            await ClearCache($"ListUsers_{generateChatDTO.id_receptor}");
            return new GenerateChatDTO
            {
                id_enviante = newChat.id_enviante,
                id_receptor = newChat.id_receptor,
                id_propriedade = newChat.id_propriedade
            };
        }

        public async Task ClearCache(string cacheKey)
        {
            await _cache.RemoveAsync(cacheKey);
        }
        public async Task ClearAllCache()
        {
            var cacheKeys = new List<string> { /* List of all cache keys */ };
            foreach (var key in cacheKeys)
            {
                await _cache.RemoveAsync(key);
            }
        }
    }
}
