using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SaveSystem
{
    // Save a full game snapshot to PlayerPrefs
    public static void SaveGame(int rows, int cols, int score, int turns, int combo,
                                List<int> cardIDs, List<bool> matchedStates)
    {
        if (cardIDs == null) cardIDs = new List<int>();
        if (matchedStates == null) matchedStates = new List<bool>();

        PlayerPrefs.SetInt("Rows", rows);
        PlayerPrefs.SetInt("Columns", cols);
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("Turns", turns);
        PlayerPrefs.SetInt("Combo", combo);

        PlayerPrefs.SetString("CardIDs", string.Join(",", cardIDs));
        PlayerPrefs.SetString("MatchedCards", string.Join(",", matchedStates.Select(b => b ? "1" : "0")));

        PlayerPrefs.SetString("HasSave", "1");
        PlayerPrefs.Save();
    }

    public static bool HasSave()
    {
        return PlayerPrefs.GetString("HasSave", "0") == "1";
    }

    public static void ClearSave()
    {
        PlayerPrefs.SetString("HasSave", "0");
        PlayerPrefs.Save();
    }

    // Loads card IDs as list; returns empty list if none
    public static List<int> LoadCardIDs()
    {
        string csv = PlayerPrefs.GetString("CardIDs", "");
        if (string.IsNullOrEmpty(csv)) return new List<int>();
        return csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
    }

    // Loads matched booleans; returns empty list if none
    public static List<bool> LoadMatchedStates()
    {
        string csv = PlayerPrefs.GetString("MatchedCards", "");
        if (string.IsNullOrEmpty(csv)) return new List<bool>();
        return csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x == "1").ToList();
    }

    public static int LoadScore() => PlayerPrefs.GetInt("Score", 0);
    public static int LoadTurns() => PlayerPrefs.GetInt("Turns", 0);
    public static int LoadCombo() => PlayerPrefs.GetInt("Combo", 0);
    public static int LoadRows() => PlayerPrefs.GetInt("Rows", 0);
    public static int LoadColumns() => PlayerPrefs.GetInt("Columns", 0);
}
