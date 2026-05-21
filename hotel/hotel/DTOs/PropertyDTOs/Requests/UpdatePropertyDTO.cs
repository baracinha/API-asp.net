namespace hotel.DTOs.PropertyDTOs.Requests
{
    public class UpdatePropertyDTO
    {
        public int id { get; set; }
        public string titulo { get; set; }
        public string tipo_propriedade { get; set; }
        public string descricao { get; set; }
        public decimal preco { get; set; }
        public int quartos { get; set; }
        public int casa_banho { get; set; }
        public decimal area_m2 { get; set; }
        public string cidade { get; set; }
    }
}
