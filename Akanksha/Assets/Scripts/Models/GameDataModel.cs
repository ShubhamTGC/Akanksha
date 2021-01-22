using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataModel 
{
    public List<QuestionList> QuestionList { get; set; }
    public List<GameList> gameList { get; set; }
    public List<ChaptersList> chaptersList { get; set; }
    public List<ChapterDivisionList> chapter_divisionList { get; set; }
}

public class QuestionList
{
    public int Id_Game { get; set; }
    public int Id_Chapter { get; set; }
    public string ChapterName { get; set; }
    public int Id_Chapter_Division { get; set; }
    public string Chapter_DivisionName { get; set; }
    public int Id_Question { get; set; }
    public string Question { get; set; }
    public int Id_Option { get; set; }
    public string Behavioural_Abbreviation { get; set; }
    public string Option { get; set; }
}

public class GameList
{
    public int Id { get; set; }
    public int Id_Game { get; set; }
    public string GameName { get; set; }
    public int ID_ORGANIZATION { get; set; }
    public string IsActive { get; set; }
    public DateTime Updated_Date_Time { get; set; }
    public string GameType { get; set; }
    public int UpdatedFlag { get; set; }
}

public class ChaptersList
{
    public int Id_Chapter { get; set; }
    public string ChapterName { get; set; }
    public int ID_ORGANIZATION { get; set; }
    public string IsActive { get; set; }
    public DateTime Updated_Date_Time { get; set; }
    public int Id_Game { get; set; }
}

public class ChapterDivisionList
{
    public int Id_Chapter_Division { get; set; }
    public int Id_Chapter { get; set; }
    public string Chapter_DivisionName { get; set; }
    public int ID_ORGANIZATION { get; set; }
    public string IsActive { get; set; }
    public DateTime Updated_Date_Time { get; set; }
}
