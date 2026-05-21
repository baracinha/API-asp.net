namespace hotel.DTOs.MessageDTOs.Requests
{
    public class ListMessagesDTO
    {
        public int id { get; set; }
        public int id_enviado_por { get; set; }
        public int id_recebido_por { get; set; }
        public string? texto_mensagem { get; set; }
    }
}
