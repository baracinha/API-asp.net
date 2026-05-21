using hotel.Data;
using hotel.DTOs.PropertyDTOs.Requests;
using hotel.DTOs.PropertyDTOs.Responses;
using hotel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace hotel.Services
{
    public class SalesServices
    {
        private string _baseUrl = "http://localhost:90";
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public SalesServices(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<InsertPropertyDTO> InsertProperty(InsertPropertyDTO propertyDTO)
        {
            var property = new Models.propriedades
            {
                id_utilizador = propertyDTO.id_utilizador,
                titulo = propertyDTO.titulo,
                descricao = propertyDTO.descricao,
                preco = propertyDTO.preco,
                tipo_propriedade = propertyDTO.tipo_propriedade,
                endereco = propertyDTO.endereco,
                cidade = propertyDTO.cidade,
                quartos = propertyDTO.quartos,
                casa_banho = propertyDTO.casa_banho,
                area_m2 = propertyDTO.area_m2,
                tipo_negocio = propertyDTO.tipo_negocio,
            };
            _context.propriedades.Add(property);
            await _context.SaveChangesAsync();

            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "properties", property.id.ToString());
            if (!Directory.Exists(pasta))
            {
                Directory.CreateDirectory(pasta);
            }

            int ordem = 0;

            foreach (var imagem in propertyDTO.imagens)
            {
                var base64 = imagem.Contains(",") ? imagem.Split(',')[1] : imagem;
                var imagemBytes = Convert.FromBase64String(base64);
                var nomeImagem = $"imagem_{ordem}.jpg";
                var caminhoImagem = Path.Combine(pasta, nomeImagem);
                await File.WriteAllBytesAsync(caminhoImagem, imagemBytes);
                var imagemPropriedade = new imagens_propriedades
                {
                    id_propriedade = property.id,
                    image_url = $"/images/properties/{property.id}/{nomeImagem}"
                };
                _context.imagens_propriedades.Add(imagemPropriedade);
                ordem++;
            }

            var caracteristicas = new List<propriedades_caracteristicas>();
            foreach (var idCaracteristica in propertyDTO.caracteristicas)
            {
                var caracteristicaPropriedade = new propriedades_caracteristicas
                {
                    id_propriedade = property.id,
                    id_caracteristica = idCaracteristica
                };
                caracteristicas.Add(caracteristicaPropriedade);
            }
            _context.propriedades_caracteristicas.AddRange(caracteristicas);
            await _context.SaveChangesAsync();
            await ClearCache($"properties_{propertyDTO.id_utilizador}");
            return propertyDTO;
        }

        public async Task<List<PropertyResponseDTO>> GetProperties(GetPropertiesDTO getPropertiesDTO)
        {
            var cacheKey = $"properties_{getPropertiesDTO.id_utilizador}";
            var cachedProperties = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedProperties))
            {
                return JsonConvert.DeserializeObject<List<PropertyResponseDTO>>(cachedProperties);
            }
            var properties = await _context.propriedades
            .Include(p => p.imagens)
            .Include(p => p.caracteristicas)
                .ThenInclude(pc => pc.caracteristica)
            .Where(p => p.id_utilizador != getPropertiesDTO.id_utilizador)
            .ToListAsync();
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            var query = properties
            .OrderBy(p => Guid.NewGuid());

            var result = query.Select(p => new PropertyResponseDTO
            {
                id = p.id,
                id_utilizador = p.id_utilizador,
                titulo = p.titulo,
                descricao = p.descricao,
                preco = p.preco,
                tipo_propriedade = p.tipo_propriedade,
                area_m2 = p.area_m2,
                cidade = p.cidade,
                quartos = p.quartos,
                casa_banho = p.casa_banho,
                imagens = p.imagens
                .Select(i => $"{_baseUrl}{i.image_url}")
                .ToList(),
                imagem_principal = p.imagens.Any() ? $"{_baseUrl}{p.imagens.First().image_url}" : null,
                caracteristicas = p.caracteristicas.Select(c => c.caracteristica.nome).ToList()
            }).ToList();
            await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(result), cacheOptions);
            return result;
        }

        public async Task<List<PropertyResponseDTO>> GetMyProperties(GetPropertiesDTO getPropertiesDTO)
        {
            var properties = await _context.propriedades
            .Include(p => p.imagens)
            .Include(p => p.caracteristicas)
                .ThenInclude(pc => pc.caracteristica)
            .Where(p => p.id_utilizador == getPropertiesDTO.id_utilizador)
            .ToListAsync();
            return properties.Select(p => new PropertyResponseDTO
            {
                id = p.id,
                id_utilizador = p.id_utilizador,
                titulo = p.titulo,
                descricao = p.descricao,
                preco = p.preco,
                quartos = p.quartos,
                tipo_propriedade = p.tipo_propriedade,
                area_m2 = p.area_m2,
                cidade = p.cidade,
                imagens = p.imagens.Select(i => $"{_baseUrl}{i.image_url}").ToList(),
                imagem_principal = p.imagens.Any() ? $"{_baseUrl}{p.imagens.First().image_url}" : null,
                caracteristicas = p.caracteristicas.Select(c => c.caracteristica.nome).ToList()
            }).ToList();
        }

        public async Task<PropertyByIdResponseDTO> GetPropertyById(GetPropertyByIdDTO getPropertyByIdDTO)
        {
            var property = await _context.propriedades
            .Include(p => p.imagens)
            .Include(p => p.caracteristicas)
                .ThenInclude(pc => pc.caracteristica)
            .FirstOrDefaultAsync(p => p.id == getPropertyByIdDTO.id);
            if (property == null) return null;
            return new PropertyByIdResponseDTO
            {
                id = property.id,
                id_utilizador = property.id_utilizador,
                titulo = property.titulo,
                descricao = property.descricao,
                tipo_propriedade = property.tipo_propriedade,
                tipo_negocio = property.tipo_negocio,
                preco = property.preco,
                cidade = property.cidade,
                quartos = property.quartos,
                area_m2 = property.area_m2,
                casa_banho = property.casa_banho,
                imagens = property.imagens.Select(i => $"{_baseUrl}{i.image_url}").ToList(),
                imagem_principal = property.imagens.Any() ? $"{_baseUrl}{property.imagens.First().image_url}" : null,
                caracteristicas = property.caracteristicas.Select(c => c.caracteristica.nome).ToList()
            };
        }

        public async Task<DeletePopertieDTO> DeleteProperty(DeletePopertieDTO deletePopertieDTO)
        {
            var property = await _context.propriedades.FindAsync(deletePopertieDTO.id);
            if (property != null)
            {
                _context.propriedades.Remove(property);
                await _context.SaveChangesAsync();
            }
            return deletePopertieDTO;
        }

        public async Task<PropertyResponseDTO> UpdateProperty(UpdatePropertyDTO updatePropertyDTO)
        {
            var property = await _context.propriedades.FindAsync(updatePropertyDTO.id);
            if (property != null)
            {
                property.titulo = updatePropertyDTO.titulo;
                property.descricao = updatePropertyDTO.descricao;
                property.preco = updatePropertyDTO.preco;
                property.cidade = updatePropertyDTO.cidade;
                property.tipo_propriedade = updatePropertyDTO.tipo_propriedade;
                property.quartos = updatePropertyDTO.quartos;
                property.casa_banho = updatePropertyDTO.casa_banho;
                property.area_m2 = updatePropertyDTO.area_m2;

                await _context.SaveChangesAsync();
            }
            return new PropertyResponseDTO
            {
                id = property.id,
                id_utilizador = property.id_utilizador,
                titulo = property.titulo,
                descricao = property.descricao,
                preco = property.preco,
                cidade = property.cidade,
                tipo_propriedade = property.tipo_propriedade,
                quartos = property.quartos,
                casa_banho = property.casa_banho,
                area_m2 = property.area_m2
            };
        }

        public async Task<List<PropertyResponseDTO>> GetFilteredProperties(GetFilteredPropertiesDTO getFilteredPropertiesDTO)
        {
            var properties = await _context.propriedades
            .Include(p => p.imagens)
            .Include(p => p.caracteristicas)
                .ThenInclude(pc => pc.caracteristica)
            .Where(p =>
                (string.IsNullOrEmpty(getFilteredPropertiesDTO.cidade) || p.cidade == getFilteredPropertiesDTO.cidade) &&
                (string.IsNullOrEmpty(getFilteredPropertiesDTO.tipo_propriedade) || p.tipo_propriedade == getFilteredPropertiesDTO.tipo_propriedade) &&
                (string.IsNullOrEmpty(getFilteredPropertiesDTO.tipo_negocio) || p.tipo_negocio == getFilteredPropertiesDTO.tipo_negocio) &&
                (!getFilteredPropertiesDTO.preco_min.HasValue || p.preco >= getFilteredPropertiesDTO.preco_min.Value) &&
                (!getFilteredPropertiesDTO.preco_max.HasValue || p.preco <= getFilteredPropertiesDTO.preco_max.Value) &&
                (!getFilteredPropertiesDTO.quartos_min.HasValue || p.quartos >= getFilteredPropertiesDTO.quartos_min.Value) &&
                (!getFilteredPropertiesDTO.quartos_max.HasValue || p.quartos <= getFilteredPropertiesDTO.quartos_max.Value) &&
                (!getFilteredPropertiesDTO.casa_banho_min.HasValue || p.casa_banho >= getFilteredPropertiesDTO.casa_banho_min.Value) &&
                (!getFilteredPropertiesDTO.casa_banho_max.HasValue || p.casa_banho <= getFilteredPropertiesDTO.casa_banho_max.Value) &&
                (!getFilteredPropertiesDTO.area_m2_min.HasValue || p.area_m2 >= getFilteredPropertiesDTO.area_m2_min.Value) &&
                (!getFilteredPropertiesDTO.area_m2_max.HasValue || p.area_m2 <= getFilteredPropertiesDTO.area_m2_max.Value) &&
                (getFilteredPropertiesDTO.caracteristicas == null || !getFilteredPropertiesDTO.caracteristicas.Any() ||
                 p.caracteristicas.Any(c => getFilteredPropertiesDTO.caracteristicas.Contains(c.id_caracteristica)))
            )
            .ToListAsync();
            return properties.Select(p => new PropertyResponseDTO
            {
                id = p.id,
                id_utilizador = p.id_utilizador,
                titulo = p.titulo,
                descricao = p.descricao,
                preco = p.preco,
                cidade = p.cidade,
                imagens = p.imagens.Select(i => i.image_url).ToList(),
                caracteristicas = p.caracteristicas.Select(c => c.caracteristica.nome).ToList()
            }).ToList();
        }

        public async Task ClearCache(string cacheKey)
        {
            await _cache.RemoveAsync(cacheKey);
        }
    }
}
