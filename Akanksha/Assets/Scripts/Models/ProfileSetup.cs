using SimpleSQL;

public class ProfileSetup 
{
    [AutoIncrement, PrimaryKey]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int Oid { get; set; }
    public string Username { get; set; }
    public string EmailId { get; set; }
    public string Mobileno { get; set; }
    public string Orgname { get; set; }
    public int LoginType { get; set; }
    public int AvatarId { get; set; }
}
