/**
 *  Settings
    Copyright (C) 2021 Vizelt

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 * 
 * 
*/

using EncryptStringSample;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static bool PostProcessing = false;
    public static bool FisheyeCam = false;
    public static bool Terrain = false;
    public static bool Shadows = true;
    public static bool Replay = true;
    public static int ReplayFreq = 25;
    public static int QualityLevel = 0;
    public static bool Arcade = true;
    public static float DeckLength = 1;
    public static float DeckWidth = 1;
    public static float WheelSize = 1;
    public static float PopForce = 220;
    public static float MaxSpeed = 6.5f;
    public static int Haircut = 0;

    private void Awake()
    {
        ReadSettings();
        QualitySettings.SetQualityLevel(QualityLevel);
    }

    public static void ReadSettings()
    {
        string path = Application.persistentDataPath + "/settings.data";
        if (!File.Exists(path))
        {
            WriteSettings();
        }

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string c = reader.ReadToEnd();
        string content = StringCipher.Decrypt(c, "rappa123");

        string[] lines = content.Split('\n');

        foreach (string line in lines)
        {
            if (line.StartsWith("#")) continue;

            Debug.Log(line);

            string l = line.Trim();
            string data = l.Split(':')[0].ToLower();
            string value = l.Split(':')[1];

            switch (data)
            {
                case "postprocessing":
                    PostProcessing = bool.Parse(value);
                    break;
                case "fisheyecam":
                    FisheyeCam = bool.Parse(value);
                    break;
                case "replays":
                    Replay = bool.Parse(value);
                    break;
                case "terrains":
                    Terrain = bool.Parse(value);
                    break;
                case "shadows":
                    Shadows = bool.Parse(value);
                    break;
                case "replayfreq":
                    ReplayFreq = int.Parse(value);
                    break;
                case "qualitylevel":
                    QualityLevel = int.Parse(value);
                    break;
                case "haircut":
                    Haircut = int.Parse(value);
                    break;
                case "deckwidth":
                    DeckWidth = float.Parse(value);
                    break;
                case "decklength":
                    DeckLength = float.Parse(value);
                    break;
                case "popforce":
                    PopForce = float.Parse(value);
                    break;
                case "maxspeed":
                    MaxSpeed = float.Parse(value);
                    break;
                case "wheelsize":
                    WheelSize = float.Parse(value);
                    break;
                default:
                    break;
            }
        }

        reader.Close();
    }

    public static void WriteSettings()
    {
        string path = Application.persistentDataPath + "/settings.data";

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.Create(path).Dispose();

        string content = "#Settings#";

        content += "\nPostProcessing:" + PostProcessing;
        content += "\nFisheyeCam:" + FisheyeCam;
        content += "\nTerrains:" + Terrain;
        content += "\nShadows:" + Shadows;
        content += "\nReplays:" + Replay;
        content += "\nReplayFreq:" + ReplayFreq;
        content += "\nQualityLevel:" + QualityLevel;
        content += "\nDeckWidth:" + DeckWidth;
        content += "\nDeckLength:" + DeckLength;
        content += "\nPopForce:" + PopForce;
        content += "\nMaxSpeed:" + MaxSpeed;
        content += "\nWheelSize:" + WheelSize;
        content += "\nHaircut:" + Haircut;
        string encrypt = StringCipher.Encrypt(content, "rappa123");

        StreamWriter writer = new StreamWriter(path, true);
        writer.Write(encrypt);
        writer.Close();
    }

    public void OnApplicationQuit()
    {
        WriteSettings();
    }
}
