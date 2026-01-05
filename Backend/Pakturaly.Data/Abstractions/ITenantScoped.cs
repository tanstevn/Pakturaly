namespace Pakturaly.Data.Abstractions {
    public interface ITenantScoped {
        public Guid TenantId { get; set; }
    }
}
