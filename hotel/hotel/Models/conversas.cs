using System.ComponentModel.DataAnnotations.Schema;

namespace hotel.Models
{
    [Table("conversas")]
    public class conversas
    {
        public int id { get; set; }
        public int id_propriedade { get; set; }
        public int id_enviante { get; set; }
        public int id_receptor { get; set; }
    }
}
