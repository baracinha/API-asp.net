using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel.Models
{
    [Table("propriedades_caracteristicas")]
    public class propriedades_caracteristicas
    {
        public int id { get; set; }
        public int id_propriedade { get; set; }
        public int id_caracteristica { get; set; }
        public propriedades propriedade { get; set; }
        public caracteristicas caracteristica { get; set; }
    }
}
