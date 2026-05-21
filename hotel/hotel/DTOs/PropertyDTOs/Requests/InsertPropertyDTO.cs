using System;
using System.Collections.Generic;
using System.Linq;

namespace hotel.DTOs.PropertyDTOs.Requests
{
    public class InsertPropertyDTO
    {
        public int id_utilizador { get; set; }
        public string titulo { get; set; }
        public string descricao { get; set; }
        public decimal preco { get; set; }
        public string tipo_propriedade { get; set; }
        public string tipo_negocio { get; set; }
        public string endereco { get; set; }
        public string cidade { get; set; }
        public int quartos { get; set; }
        public int casa_banho { get; set; }
        public decimal area_m2 { get; set; }
        public List<string> imagens { get; set; } = new();
        public List<int> caracteristicas { get; set; } = new();
    }
}
