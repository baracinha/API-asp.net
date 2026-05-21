using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel.Models
{
    [Table("imagens_propriedades")]
    public class imagens_propriedades
    {
        public int id { get; set; }
        public int id_propriedade { get; set; }
        public string image_url { get; set; }
        public propriedades propriedade { get; set; }
    }
}
