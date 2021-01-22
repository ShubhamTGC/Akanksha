using System;

public class EquipmentModel 
{
    public int Id_Equipment { get; set; }
    public string EquipmentName { get; set; }
    public string Purpose { get; set; }
    public string Related_Clue { get; set; }
    public string Location { get; set; }
    public string Extra_Clues { get; set; }
    public int ID_ORGANIZATION { get; set; }
    public string IsActive { get; set; }
    public DateTime Updated_Date_Time { get; set; }
    public object Equipment_Ranking { get; set; }
}
