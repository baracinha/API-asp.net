using Google.Protobuf.WellKnownTypes;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel.Models
{
    [Table("utilizadores")]
    public class utilizadores
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
        public string password_hash { get; set; }
        public string cargo { get; set; }
        public string bio { get; set; }
        public string imagem_perfil { get; set; }
        public string cidade { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
