namespace OrderManagement.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,        // Aguardando pagamento
    Confirmed = 2,      // Pagamento confirmado
    Processing = 3,     // Em preparação
    Shipped = 4,        // Enviado
    Delivered = 5,      // Entregue
    Cancelled = 6       // Cancelado
}
