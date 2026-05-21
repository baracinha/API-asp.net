using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel.Models
{
    [Table("mensagens")]
    public class mensagens
    {
        public int id { get; set; }
        public int id_enviado_por { get; set; }
        public string texto_mensagem { get; set; }
        public int id_recebido_por { get; set; }
    }
}
