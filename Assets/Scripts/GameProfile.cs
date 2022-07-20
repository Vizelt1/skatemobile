/**
 *  GameProfile
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
using UnityEngine;
using UnityEngine.UI;
using EncryptStringSample;
using System.IO;
using System.Collections.Generic;
using System;

public class GameProfile : MonoBehaviour
{
    private string custom_path = Application.persistentDataPath + "/Customization/";

    [Header("Item List")]
    public List<Item> ItemsAvaible;

    [Header("Profile")]
    public int Balance = 0;
    public string Username;

    [Header("XP")]
    public int CurrentLevel;
    public int CurrentXP;
    public int NextLevelXP;
    public int LastLevelXP;

    [Header("Customization")]
    public Material DeckWearMaterial;
    public Material SkateMaterial;
    public Material GripMaterial;
    public Material TopMaterial;
    public Material WheelMaterial;
    public Material BottomMaterial;
    public Material ShoesMaterial;
    public Material SkinMaterial;

    [Header("Customization Loader")]
    private string DeckWearName;
    private string SkateName;
    private string GripName;
    private string WheelName;
    private string TopName;
    private string BottomName;
    private string ShoesName;
    private string SkinName;

    [Header("Customization Menu")]
    public Dropdown SkateList;
    public Dropdown DeckWearList;
    public Dropdown GripList;
    public Dropdown WheelsList;
    public Dropdown TopList;
    public Dropdown BottomList;
    public Dropdown ShoesList;
    public Dropdown SkinList;

    [Header("Customization Defaults")]
    public Texture DefaultDeck;
    public Texture DefaultDeckWear;
    public Texture DefaultGrip;
    public Texture DefaultWheels;
    public Texture DefaultTop;
    public Texture DefaultBottom;
    public Texture DefaultShoes;
    public Texture DefaultSkin;


    [Header("UI")]
    public Text UsernameText;
    public Text LevelText;
    public Slider LevelSlider;
    public Text CurLevelNbr;
    public Text NextLevelNbr;
    public Text BalanceText;
    public InputField UsernameField;

    [Header("Menus")]
    public GameObject MainMenu;
    public GameObject UsernameMenu;

    private static GameProfile instance;

    void Start()
    {
        instance = this;

        SkateMaterial.mainTexture = DefaultDeck;
        DeckWearMaterial.mainTexture = DefaultDeckWear;
        GripMaterial.mainTexture = DefaultGrip;
        TopMaterial.mainTexture = DefaultTop;
        BottomMaterial.mainTexture = DefaultBottom;
        ShoesMaterial.mainTexture = DefaultShoes;
        SkinMaterial.mainTexture = DefaultSkin;
        WheelMaterial.mainTexture = DefaultWheels;

        AddDefaultCustomization();
        AddCustomizationFolder();
        ReadSettings();
        UpdateUI();

        if (SkateName != "") SwitchItem(GetItemByName(SkateName));

        if (DeckWearName != "") SwitchItem(GetItemByName(DeckWearName));

        if (GripName != "") SwitchItem(GetItemByName(GripName));

        if (WheelName != "") SwitchItem(GetItemByName(WheelName));

        if (TopName != "") SwitchItem(GetItemByName(TopName));

        if (BottomName != "") SwitchItem(GetItemByName(BottomName));

        if (ShoesName != "") SwitchItem(GetItemByName(ShoesName));

        if (SkinName != "") SwitchItem(GetItemByName(SkinName));
    }

    public void AddDefaultCustomization()
    {
        ItemsAvaible = new List<Item>();
        List<Dropdown.OptionData> opt = new List<Dropdown.OptionData>();
        ItemsAvaible.Add(new Item("Default Deck", DefaultDeck, ItemType.Deck));
        opt.Clear();
        opt.Add(new Dropdown.OptionData("Default Deck"));
        SkateList.AddOptions(opt);

        ItemsAvaible.Add(new Item("Default DeckWear", DefaultDeckWear, ItemType.DeckWear));
        opt.Clear();
        opt.Add(new Dropdown.OptionData("Default DeckWear"));
        DeckWearList.AddOptions(opt);

        ItemsAvaible.Add(new Item("Default Grip", DefaultGrip, ItemType.Grip));
        opt.Clear();
        opt.Add(new Dropdown.OptionData("Default Grip"));
        GripList.AddOptions(opt);

        ItemsAvaible.Add(new Item("Default Top", DefaultTop, ItemType.Top));
        opt.Clear();
        opt.Add(new Dropdown.OptionData("Default Top"));
        TopList.AddOptions(opt);

        ItemsAvaible.Add(new Item("Default Bottom", DefaultBottom, ItemType.Bottom));
        opt.Clear();
        opt.Add(new Dropdown.OptionData("Default Bottom"));
        BottomList.AddOptions(opt);

        ItemsAvaible.Add(new Item("Default Shoes", DefaultShoes, ItemType.Shoes));
        opt.Clear();
        opt.Add(new Dropdown.OptionData("Default Shoes"));
        ShoesList.AddOptions(opt);

        ItemsAvaible.Add(new Item("Default Skin", DefaultSkin, ItemType.Skin));
        opt.Clear();
        opt.Add(new Dropdown.OptionData("Default Skin"));
        SkinList.AddOptions(opt);

        ItemsAvaible.Add(new Item("Default Wheels", DefaultWheels, ItemType.Wheels));
        opt.Clear();
        opt.Add(new Dropdown.OptionData("Default Wheels"));
        WheelsList.AddOptions(opt);
    }

    public void RefreshCustomization()
    {
        ItemsAvaible.Clear();
        SkateList.options.Clear();
        DeckWearList.options.Clear();
        GripList.options.Clear();
        TopList.options.Clear();
        BottomList.options.Clear();
        ShoesList.options.Clear();
        SkinList.options.Clear();
        WheelsList.options.Clear();
        AddDefaultCustomization();
        AddCustomizationFolder();
    }

    public static GameProfile GetInstance()
    {
        return instance;
    }

    public void SwitchItem(Item i)
    {
        Debug.Log(i.Name);
        switch (i.Type)
        {
            case ItemType.Deck:
                SkateMaterial.mainTexture = i.Texture;
                SkateName = i.Name;
                break;
            case ItemType.DeckWear:
                DeckWearMaterial.mainTexture = i.Texture;
                DeckWearName = i.Name;
                break;
            case ItemType.Grip:
                GripMaterial.mainTexture = i.Texture;
                GripName = i.Name;
                break;
            case ItemType.Top:
                TopMaterial.mainTexture = i.Texture;
                TopName = i.Name;
                break;
            case ItemType.Bottom:
                BottomMaterial.mainTexture = i.Texture;
                BottomName = i.Name;
                break;
            case ItemType.Shoes:
                ShoesMaterial.mainTexture = i.Texture;
                ShoesName = i.Name;
                break;
            case ItemType.Skin:
                SkinMaterial.mainTexture = i.Texture;
                SkinName = i.Name;
                break;
            case ItemType.Wheels:
                WheelMaterial.mainTexture = i.Texture;
                WheelName = i.Name;
                break;
            default: break;
        }

        WriteSettings();
    }



    public void SaveProfile()
    {
        if (UsernameField.text.Length == 0) UsernameField.text = "Player";

        UsernameMenu.SetActive(false);
        MainMenu.SetActive(true);

        Username = UsernameField.text;
        WriteSettings();
        UpdateUI();


    }

    public int GetItemId(Item it)
    {
        int i = 0;
        foreach (Item item in ItemsAvaible)
        {
            if (item.Name == it.Name) return i;
            i++;
        }
        return 0;
    }

    public void UpdateUI()
    {

        UsernameText.text = Username;
        LevelText.text = "Level : " + CurrentLevel;
        BalanceText.text = "Balance : " + Balance + "$";
        CurLevelNbr.text = "" + CurrentLevel;
        LevelSlider.minValue = LastLevelXP;
        LevelSlider.maxValue = NextLevelXP;
        LevelSlider.value = CurrentXP;
        if (CurrentLevel < 999) NextLevelNbr.text = "" + (CurrentLevel + 1);

    }

    public void AddXP(int xp)
    {

        CurrentXP += xp;

        if (CurrentXP > NextLevelXP)
        {


            LastLevelXP = NextLevelXP;
            NextLevelXP = Mathf.RoundToInt(NextLevelXP * 2f);
            CurrentLevel++;

            LevelSlider.minValue = LastLevelXP;
            LevelSlider.maxValue = NextLevelXP;
        }

        WriteSettings();
        UpdateUI();
    }

    public void SelectDeck()
    {
        int i = SkateList.value;
        SwitchItem(GetItemByName(SkateList.options[i].text));
        
    }

    public void SelectWheel()
    {
        int i = WheelsList.value;
        SwitchItem(GetItemByName(WheelsList.options[i].text));

    }

    public void SelectDeckWear()
    {
        int i = DeckWearList.value;
        SwitchItem(GetItemByName(DeckWearList.options[i].text));
    }

    public void SelectGrip()
    {
        int i = GripList.value;
        SwitchItem(GetItemByName(GripList.options[i].text));
    }

    public void SelectTop()
    {
        int i = TopList.value;
        Debug.Log(i);
        SwitchItem(GetItemByName(TopList.options[i].text));
    }

    public void SelectBottom()
    {
        int i = BottomList.value;
        SwitchItem(GetItemByName(BottomList.options[i].text));
    }

    public void SelectShoes()
    {
        int i = ShoesList.value;
        SwitchItem(GetItemByName(ShoesList.options[i].text));
    }

    public void SelectSkin()
    {
        int i = SkinList.value;
        SwitchItem(GetItemByName(SkinList.options[i].text));
    }

    public Item GetItemByName(string name)
    {
        foreach (Item i in ItemsAvaible)
        {
            if (name.ToLower() == i.Name.ToLower()) return i;
        }

        return null;
    }

   

    public void AddCustomizationFolder()
    {
        if (!Directory.Exists(custom_path))
        {
            Directory.CreateDirectory(custom_path);
         
        } else
        {
            string[] paths = Directory.GetFiles(custom_path);
            foreach (string str in paths)
            {
                string name = Path.GetFileNameWithoutExtension(str);
                Texture tex = LoadPNG(str);
                string itemname = name.Split('_')[1];
                string n = name.Split('_')[0].ToLower();
                ItemType itemtype = 0;

                List<Dropdown.OptionData> opt = new List<Dropdown.OptionData>();
                opt.Add(new Dropdown.OptionData(itemname));

                if (n == "deck")
                {
                    itemtype = ItemType.Deck;
                    SkateList.AddOptions(opt);
                }
                if (n == "deckwear")
                {
                    itemtype = ItemType.DeckWear;
                    DeckWearList.AddOptions(opt);
                }
                if (n == "grip")
                {
                    itemtype = ItemType.Grip;
                    GripList.AddOptions(opt);
                }
                if (n == "top")
                {
                    itemtype = ItemType.Top;
                    TopList.AddOptions(opt);
                }
                if (n == "bottom")
                {
                    itemtype = ItemType.Bottom;
                    BottomList.AddOptions(opt);
                }
                if (n == "shoes")
                {
                    itemtype = ItemType.Shoes;
                    ShoesList.AddOptions(opt);
                }
                if (n == "skin")
                {
                    itemtype = ItemType.Skin;
                    SkinList.AddOptions(opt);
                }
                if (n == "wheel")
                {
                    itemtype = ItemType.Wheels;
                    WheelsList.AddOptions(opt);
                }

                ItemsAvaible.Add(new Item(itemname, tex, itemtype));
            }

        }
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    public void ReadSettings()
    {
        string path = Application.persistentDataPath + "/player.data";

        if (!File.Exists(path))
        {
            UsernameMenu.SetActive(true);
            MainMenu.SetActive(false);
            return;
        }
        else
        {
            UsernameMenu.SetActive(false);
            MainMenu.SetActive(true);
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
                case "username":
                    Username = value;
                    break;
                case "currentlevel":
                    CurrentLevel = int.Parse(value);
                    break;
                case "currentxp":
                    CurrentXP = int.Parse(value);
                    break;
                case "lastlevelxp":
                    LastLevelXP = int.Parse(value);
                    break;
                case "nextlevelxp":
                    NextLevelXP = int.Parse(value);
                    break;
                case "balance":
                    Balance = int.Parse(value);
                    break;
                case "skatename":
                    SkateName = value;
                    break;
                case "deckwearname":
                    DeckWearName = value;
                    break;
                case "gripname":
                    GripName = value;
                    break;
                case "topname":
                    TopName = value;
                    break;
                case "wheelname":
                    WheelName = value;
                    break;
                case "bottomname":
                    BottomName = value;
                    break;
                case "shoesname":
                    ShoesName = value;
                    break;
                case "skinname":
                    SkinName = value;
                    break;
                default:
                    break;
            }
        }

        reader.Close();
    }

    public void WriteSettings()
    {
        string path = Application.persistentDataPath + "/player.data";

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.Create(path).Dispose();

        string content = "#PlayerData#";

        content += "\nUsername:" + Username;
        content += "\nCurrentLevel:" + CurrentLevel;
        content += "\nCurrentXP:" + CurrentXP;
        content += "\nLastLevelXP:" + LastLevelXP;
        content += "\nNextLevelXP:" + NextLevelXP;
        content += "\nBalance:" + Balance;
        content += "\nSkateName:" + SkateName;
        content += "\nDeckWearName:" + DeckWearName;
        content += "\nGripName:" + GripName;
        content += "\nTopName:" + TopName;
        content += "\nBottomName:" + BottomName;
        content += "\nShoesName:" + ShoesName;
        content += "\nSkinName:" + SkinName;
        content += "\nWheelName:" + WheelName;

        string encrypt = StringCipher.Encrypt(content, "rappa123");

        StreamWriter writer = new StreamWriter(path, true);
        writer.Write(encrypt);
        writer.Close();
    }

    void OnApplicationQuit()
    {
        WriteSettings();
    }
}
