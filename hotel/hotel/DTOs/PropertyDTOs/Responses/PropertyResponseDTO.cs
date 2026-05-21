namespace hotel.DTOs.PropertyDTOs.Responses
{
    public class PropertyResponseDTO
    {
        public int id { get; set; }
        public int id_utilizador { get; set; }
        public string titulo { get; set; }
        public string descricao { get; set; }
        public decimal preco { get; set; }
        public string tipo_propriedade { get; set; }
        public decimal area_m2 { get; set; } = 0;
        public string cidade { get; set; }
        public int quartos { get; set; }
        public int casa_banho { get; set; }
        public List<string> imagens { get; set; }
        public string imagem_principal { get; set; } = string.Empty;
        public List<string> caracteristicas { get; set; }
    }
}
