namespace CustomMiddleware.Attributes;

public class NeedRoleAttribute : Attribute
{
    public string Role { get; set; }
    
    public NeedRoleAttribute(string role)
    {
        Role = role;
    }
}