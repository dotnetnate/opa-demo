public class PolicyWrapper{
    public Guid id { get;set;}
    public Guid tenantId { get;set;}
    public object resource { get;set;}
    public object rules { get;set;}
}