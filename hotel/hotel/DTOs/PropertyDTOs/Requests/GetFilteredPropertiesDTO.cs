namespace hotel.DTOs.PropertyDTOs.Requests
{
    public class GetFilteredPropertiesDTO
    {
        public string? cidade { get; set; }
        public string? tipo_propriedade { get; set; }
        public string? tipo_negocio { get; set; }
        public decimal? preco_min { get; set; }
        public decimal? preco_max { get; set; }
        public int? quartos_min { get; set; }
        public int? quartos_max { get; set; }
        public int? casa_banho_min { get; set; }
        public int? casa_banho_max { get; set; }
        public decimal? area_m2_min { get; set; }
        public decimal? area_m2_max { get; set; }
        public List<int>? caracteristicas { get; set; } = new();
    }
}
