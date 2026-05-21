
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel.Models
{
    [Table("caracteristicas")]
    public class caracteristicas
    {
        public int id { get; set; }
        public string nome { get; set; }
        public List<propriedades_caracteristicas> propriedades_caracteristicas { get; set; } = new();
    }
}
