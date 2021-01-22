using System;
using System.Collections.Generic;


public class AvatarLogModel 
{
    public int Id_Avatar { get; set; }
    public string url { get; set; }
    public int ID_ORGANIZATION { get; set; }
    public string IsActive { get; set; }
    public DateTime Updated_Date_Time { get; set; }
    public List<object> tbl_users { get; set; }

}
