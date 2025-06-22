using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using static DataManager;
using System.Threading;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class Build : MonoBehaviour
{
    DataManager dataManager;
    private SaveLoadService saveLoadService;
    TownBuildings buildings;
    public SetBalls setBalls;

    // add canvas
    public Canvas addCanvas;
    public TextMeshProUGUI buildingTitle, buildingDescription, buildingStatus;
    public LeanButton buildButton;

    // lvup cangas
    public Canvas lvUpCanvas;
    public TextMeshProUGUI lvUpTitle, lvUpDescription, lvUpStatus;
    public LeanButton lvUpButton;

    // get resources
    public LeanButton getFaithButton, getSunPowerButton;
    private decimal currentFaith, currentSunPower;
    public TextMeshProUGUI currentFaithText, currentSunPowerText;

    // buildings
    string[] buildingNameArray = { "Heart of the Town", "Alter of Prayer", "Faith Vault", "Sun Tower" };
    string[] buildingDescriptionArray = {
        "Townの中心。他の建造物を建設するのに必要。Heart of the Townのレベルに応じて他の建造物のレベルを上げられる。",
        "Faithを生産する。回収するとFaithを獲得できる。レベルに応じて祭壇の保有最大値と生産速度が増加する。",
        "Faithを保有する。最大量のときには回収できない。レベルに応じて保有できる最大値が増加する",
        "Sun Powerを生産する。回収するとSun Powerを獲得できる。レベルに応じて塔の保有最大値と生産速度が増加する。"
    };
    string[] buildingStatusArray = {
        "Lv 1\nAlter of Prayer、Faith Vault、Sun Towerを建設できる。",// \n最大保有Faith　1000",
        "Lv 1\n1時間あたりの生産Faith　1\n最大保有Faith　100",
        "Lv 1\n最大保有Faith　2000",
        "Lv 1\n1日当たりの生産Sun Power　1\n最大保有Sun Power　5"
    }; 
    int[] maxLevels = { 1, 1, 1, 1 };

    // alterofprayer
    decimal[] genFaithPerHourByAlterOfPrayer = { 1m, 1.2m, 1.4m, 1.7m, 2m };
    decimal[] maxFaithByAlterOfPrayer = { 100m, 120m, 140m, 170m, 200m };

    // suntower
    decimal[] genSunPowerPerDayBySunTower = { 1 };
    decimal[] maxSunPowerBySunTower = { 5 };

    public GameObject HeartOfTheTown;
    public GameObject[] AlterOfPrayers;
    public GameObject[] FaithValuts;
    public GameObject[] SunTowers;

    CancellationTokenSource _cts;

    public enum Building
    {
        HeartOfTheTown, AlterOfPrayer, FaithValut, SunTower
    }

    async void Start()
    {
        SceneManager.sceneUnloaded += OnUnloaded;
        dataManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>();
        if (setBalls == null)
        {
            Debug.LogWarning("SetBalls is not assigned in the inspector. Trying to find it automatically.");
            setBalls = FindObjectOfType<SetBalls>();
        }
        await UniTask.WaitUntil(() => dataManager.IsInitialized);

        saveLoadService = new SaveLoadService();
        buildings = new TownBuildings(saveLoadService);

        string directory = Path.Combine(Application.persistentDataPath, "Building");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string path = Path.Combine(directory, "TownBuildings.json");
        if (!File.Exists(path)) // Pathの存在を保証
        {
            buildings.Save();
        }
        else
        {
            string json = saveLoadService.Load(path);
            Debug.Log(json);
            buildings = JsonUtility.FromJson<TownBuildings>(json);
            buildings.SetSaveLoadService(saveLoadService); // Deserialization after new instance
        }

        CheckIsBuild();
    }

    void CheckIsBuild()
    {
        if (GetBuildingLevel(Building.HeartOfTheTown) == 0)
        {
            GetAddObj(HeartOfTheTown).SetActive(true);
            GetLvObj(HeartOfTheTown).SetActive(false);
            GetAddButton(HeartOfTheTown).OnClick.AddListener(() => OpenAddCanvas(Building.HeartOfTheTown));
        }
        else
        {
            GetAddObj(HeartOfTheTown).SetActive(false);
            GetLvObj(HeartOfTheTown).SetActive(true);
            GetLvButton(HeartOfTheTown).OnClick.AddListener(() => OpenGetCanvas(Building.HeartOfTheTown));
            GetLvButtonText(HeartOfTheTown).text = $"Lv {GetBuildingLevel(Building.HeartOfTheTown)}";
        }


        for (int i = 0; i < AlterOfPrayers.Length; i++)
        {
            SetBuildButton(AlterOfPrayers[i], Building.AlterOfPrayer, i);
        }
        for (int i = 0; i < FaithValuts.Length; i++)
        {
            SetBuildButton(FaithValuts[i], Building.FaithValut, i);
        }
        for (int i = 0; i < SunTowers.Length; i++)
        {
            SetBuildButton(SunTowers[i], Building.SunTower, i);
        }
        GenerateResources();
    }

    void SetBuildButton(GameObject g, Building building, int index)
    {
        LeanButton l = GetAddButton(g);
        LeanButton m = GetLvButton(g);

        if (GetBuildingLevel(Building.HeartOfTheTown) == 0)
        {
            GetAddObj(g).SetActive(true);
            GetLvObj(g).SetActive(false);
            GetCollectObj(g).SetActive(false);
            l.enabled = false;
            GetAddButtonImage(g).color = Color.gray;
        }
        else if (GetBuildingLevel(building, index) == 0)
        {
            GetAddObj(g).SetActive(true);
            GetLvObj(g).SetActive(false);
            GetCollectObj(g).SetActive(false);
            l.enabled = true;
            GetAddButtonImage(g).color = CONSTANTS.BUTTONCOLOR;
        }
        else
        {
            GetAddObj(g).SetActive(false);
            GetLvObj(g).SetActive(true);
            GetCollectObj(g).SetActive(true);
            GetLvButtonText(g).text = $"Lv {GetBuildingLevel(building, index)}";
        }
        Building b = building;
        int i = index;
        l.OnClick.AddListener(() => OpenAddCanvas(b, i));
        m.OnClick.AddListener(() => OpenGetCanvas(b, i));
    }

    private object _lockObject = new object();

    void GenerateResources()
    {

        DateTime?[] latestUpdateTimeInAlterOfPrayers = new DateTime?[AlterOfPrayers.Length];
        DateTime?[] latestUpdateTimeInSunTowers = new DateTime?[SunTowers.Length];

        decimal?[] currentFaithInAlterOfPrayers = new decimal?[AlterOfPrayers.Length];
        decimal?[] currentSunPowerInSunTowers = new decimal?[SunTowers.Length];

        int[] levelInAlterOfPrayers = new int[AlterOfPrayers.Length];
        int[] levelInSunTowers = new int[SunTowers.Length];

        // まず、最初の資源量と最終更新時間を取得
        for (int i = 0; i < AlterOfPrayers.Length; i++)
        {
            TownBuilding b = buildings.Get(Building.AlterOfPrayer, i);
            Debug.Log(b.latestUpdateTime);
            if (b.level == 0 || b.latestUpdateTime == "")
            {
                currentFaithInAlterOfPrayers[i] = null;
                latestUpdateTimeInAlterOfPrayers[i] = null;
            }
            else
            {
                Debug.Log(b.latestUpdateTime);
                currentFaithInAlterOfPrayers[i] = decimal.Parse(b.generatingValue);
                latestUpdateTimeInAlterOfPrayers[i] = DateTime.ParseExact(b.latestUpdateTime, CONSTANTSDATE.FORMAT, null);
            }
            levelInAlterOfPrayers[i] = b.level;
        }
        for (int i = 0; i < SunTowers.Length; i++)
        {
            TownBuilding b = buildings.Get(Building.SunTower, i);
            if (b.level == 0 || b.latestUpdateTime == "")
            {
                currentSunPowerInSunTowers[i] = null;
                latestUpdateTimeInSunTowers[i] = null;
            }
            else
            {
                currentSunPowerInSunTowers[i] = decimal.Parse(b.generatingValue);
                latestUpdateTimeInSunTowers[i] = DateTime.ParseExact(b.latestUpdateTime, CONSTANTSDATE.FORMAT, null);
            }
            levelInSunTowers[i] = b.level;
        }

        // 次に、最終更新時刻に応じて資源量を加算
        DateTime now = DateTime.Now;
        for (int i = 0; i < AlterOfPrayers.Length; i++)
        {
            TownBuilding b = buildings.Get(Building.AlterOfPrayer, i);
            if (latestUpdateTimeInAlterOfPrayers[i] != null)
            {
                int lv = levelInAlterOfPrayers[i];
                decimal perHour = genFaithPerHourByAlterOfPrayer[lv - 1];
                decimal max = maxFaithByAlterOfPrayer[lv - 1];
                TimeSpan delta = now - (DateTime)latestUpdateTimeInAlterOfPrayers[i];
                decimal gameTick = Math.Round((decimal)delta.TotalSeconds / 0.36m);
                b.SetLatestUpdateTime(now);
                b.AddGeneratingValue(gameTick * perHour / 10000, max);
                buildings.Save();
            }
            else if (b.level != 0)
            {
                b.SetLatestUpdateTime(now);
                buildings.Save();
            }
            GetCollectButtonText(AlterOfPrayers[i]).text = b.GetGeneratingValue().ToString("F3");
        }

        for (int i = 0; i < SunTowers.Length; i++)
        {
            TownBuilding b = buildings.Get(Building.SunTower, i);
            if (latestUpdateTimeInSunTowers[i] != null)
            {
                int lv = levelInSunTowers[i];
                decimal perDay = genSunPowerPerDayBySunTower[lv - 1];
                decimal max = maxSunPowerBySunTower[lv - 1];
                TimeSpan delta = now - (DateTime)latestUpdateTimeInSunTowers[i];
                decimal gameTick = Math.Round((decimal)delta.TotalSeconds / 0.36m);
                b.SetLatestUpdateTime(now);
                b.AddGeneratingValue(gameTick * perDay / 240000, max);
                buildings.Save();
            }
            else if (b.level != 0)
            {
                b.SetLatestUpdateTime(now);
                buildings.Save();
            }
            GetCollectButtonText(SunTowers[i]).text = b.GetGeneratingValue().ToString("F3");
        }

        // 最後に、シーンの表示中は常に資源量を更新し、最終更新時間を更新し続ける
        UpdateResources();

    }

    async void UpdateResources()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        while (true)
        {
            try
            {
                DateTime now = DateTime.Now;
                for (int i = 0; i < AlterOfPrayers.Length; i++)
                {
                    TownBuilding b = buildings.Get(Building.AlterOfPrayer, i);
                    if (b.latestUpdateTime != "")
                    {
                        int lv = b.level;
                        decimal perHour = genFaithPerHourByAlterOfPrayer[lv - 1];
                        decimal max = maxFaithByAlterOfPrayer[lv - 1];
                        b.SetLatestUpdateTime(now);
                        b.AddGeneratingValue(perHour / 10000, max);
                        buildings.Save();
                        GameObject g = AlterOfPrayers[i];
                        GetCollectButton(g).OnClick.RemoveAllListeners();
                        if (b.GetGeneratingValue() >= 1m)
                        {
                            GetCollectImage(g).color = CONSTANTS.BUTTONCOLOR;
                            GetCollectButton(g).OnClick.AddListener(() => {
                                int getFaith = (int)b.GetGeneratingValue();
                                dataManager.res.Add(GameResource.Faith, getFaith);
                                if (setBalls != null)
                                {
                                    setBalls.GenBalls(getFaith, true);
                                    setBalls.UpdateFaith();
                                }
                                else
                                {
                                    Debug.LogError("setBalls is null!");
                                }
                                b.AddGeneratingValue(-(decimal)getFaith, 100);
                                buildings.Save();
                                GetCollectImage(g).color = Color.gray;
                                GetCollectButton(g).enabled = false;
                                GetCollectButtonText(g).text = b.GetGeneratingValue().ToString("F3");
                            });
                            GetCollectButton(g).enabled = true;
                        }
                        else
                        {
                            GetCollectImage(g).color = Color.gray;
                            GetCollectButton(g).enabled = false;
                        }
                    }
                    else if (b.level != 0)
                    {
                        b.SetLatestUpdateTime(now);
                        buildings.Save();
                    }
                    GetCollectButtonText(AlterOfPrayers[i]).text = b.GetGeneratingValue().ToString("F3");
                }

                for (int i = 0; i < SunTowers.Length; i++)
                {
                    TownBuilding b = buildings.Get(Building.SunTower, i);
                    if (b.latestUpdateTime != "")
                    {
                        int lv = b.level;
                        decimal perDay = genSunPowerPerDayBySunTower[lv - 1];
                        decimal max = maxSunPowerBySunTower[lv - 1];
                        b.SetLatestUpdateTime(now);
                        b.AddGeneratingValue(perDay / 240000, max);
                        buildings.Save();
                        GameObject g = SunTowers[i];
                        GetCollectButton(g).OnClick.RemoveAllListeners();
                        if (b.GetGeneratingValue() >= 1m)
                        {
                            GetCollectImage(g).color = CONSTANTS.BUTTONCOLOR;
                            GetCollectButton(g).OnClick.AddListener(() => {
                                int getSunPower = (int)b.GetGeneratingValue();
                                dataManager.res.Add(GameResource.SunPower, getSunPower);
                                b.AddGeneratingValue(-(decimal)getSunPower, 100);
                                buildings.Save();
                                GetCollectImage(g).color = Color.gray;
                                GetCollectButton(g).enabled = false;
                                GetCollectButtonText(g).text = b.GetGeneratingValue().ToString("F3");
                            });
                            GetCollectButton(g).enabled = true;
                        }
                        else
                        {
                            GetCollectImage(g).color = Color.gray;
                            GetCollectButton(g).enabled = false;
                        }
                    }
                    else if (b.level != 0)
                    {
                        b.SetLatestUpdateTime(now);
                        buildings.Save();
                    }
                    GetCollectButtonText(SunTowers[i]).text = b.GetGeneratingValue().ToString("F3");
                }
                DateTime now2 = DateTime.Now;
                TimeSpan delta = now2 - now;
                await UniTask.Delay(Math.Max(360 - delta.Milliseconds, 100), cancellationToken: _cts.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    void OnUnloaded(Scene currentScene)
    {
        Debug.Log("Unload Scene :" + currentScene.name);
        _cts.Cancel();
        SceneManager.sceneUnloaded -= OnUnloaded;
    }

    GameObject GetAddObj(GameObject g)
    {
        return g.transform.GetChild(1).gameObject;
    }

    LeanButton GetAddButton(GameObject g)
    {
        return g.transform.GetChild(1).GetComponent<LeanButton>();
    }

    Image GetAddButtonImage(GameObject g)
    {
        return g.transform.GetChild(1).GetChild(1).GetComponent<Image>();
    }

    GameObject GetLvObj(GameObject g)
    {
        return g.transform.GetChild(2).gameObject;
    }

    LeanButton GetLvButton(GameObject g)
    {
        return g.transform.GetChild(2).GetComponent<LeanButton>();
    }

    TextMeshProUGUI GetLvButtonText(GameObject g)
    {
        return g.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    GameObject GetCollectObj(GameObject g)
    {
        return g.transform.GetChild(3).gameObject;
    }

    LeanButton GetCollectButton(GameObject g)
    {
        return g.transform.GetChild(3).GetComponent<LeanButton>();
    }

    Image GetCollectImage(GameObject g)
    {
        return g.transform.GetChild(3).GetChild(1).GetComponent<Image>();
    }

    TextMeshProUGUI GetCollectButtonText(GameObject g)
    {
        return g.transform.GetChild(3).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void OpenAddCanvas(Building building)
    {
        addCanvas.enabled = true;
        buildingTitle.text = buildingNameArray[(int)building];
        buildingDescription.text = buildingDescriptionArray[(int)building];
        buildingStatus.text = buildingStatusArray[(int)building];

        buildButton.OnClick.RemoveAllListeners();
        buildButton.OnClick.AddListener(() => {
            buildings.SetLevel(1, building);
            addCanvas.enabled = false;
            CheckIsBuild();
        });
        /*
        switch (building) {
            case Building.HeartOfTheTown:
                break;
            case Building.AlterOfPreyer:
                break;
            case Building.FaithValut:
                break;
            case Building.SunTower:
                break;
            default:
                break;
        }
        */
    }

    void OpenAddCanvas(Building building, int index)
    {
        addCanvas.enabled = true;
        buildingTitle.text = buildingNameArray[(int)building];
        buildingDescription.text = buildingDescriptionArray[(int)building];
        buildingStatus.text = buildingStatusArray[(int)building];

        Building b = building;
        int i = index;

        buildButton.OnClick.RemoveAllListeners();
        buildButton.OnClick.AddListener(() => {
            buildings.SetLevel(1, b, i);
            addCanvas.enabled = false;
            CheckIsBuild();
        });
    }

    void OpenGetCanvas(Building building)
    {
        lvUpCanvas.enabled = true;
        lvUpTitle.text = buildingNameArray[(int)building];
        lvUpDescription.text = buildingDescriptionArray[(int)building];
        lvUpStatus.text = buildingStatusArray[(int)building];

        lvUpButton.OnClick.RemoveAllListeners();
        lvUpButton.OnClick.AddListener(() => {
            CheckIsBuild();
        });

        lvUpButton.OnClick.RemoveAllListeners();
        // 暫定的
        lvUpButton.enabled = false;
        lvUpButton.transform.GetChild(1).GetComponent<Image>().color = Color.gray;
    }

    void OpenGetCanvas(Building building, int index)
    {
        lvUpCanvas.enabled = true;
        lvUpTitle.text = buildingNameArray[(int)building];
        lvUpDescription.text = buildingDescriptionArray[(int)building];
        lvUpStatus.text = buildingStatusArray[(int)building];

        lvUpButton.OnClick.RemoveAllListeners();
        lvUpButton.OnClick.AddListener(() => {
            CheckIsBuild();
        });

        lvUpButton.OnClick.RemoveAllListeners();
        // 暫定的
        lvUpButton.enabled = false;
        lvUpButton.transform.GetChild(1).GetComponent<Image>().color = Color.gray;
    }

    int GetBuildingLevel(Building building)
    {
        return buildings.Get(building).level; //PlayerPrefs.GetInt($"{building.ToString()}Lv", 0);
    }
    int GetBuildingLevel(Building building, int index)
    {
        return buildings.Get(building, index).level; //PlayerPrefs.GetInt($"{building.ToString()}{index + 1}Lv", 0);
    }

    [System.Serializable]
    public class TownBuildings
    {
        [System.NonSerialized]
        private SaveLoadService saveLoadService;
        string path;
        public List<TownBuilding> townBuildings;

        public TownBuildings(SaveLoadService sls)
        {
            townBuildings = new List<TownBuilding>();
            saveLoadService = sls;
        }

        public void SetSaveLoadService(SaveLoadService sls)
        {
            saveLoadService = sls;
        }

        /// <summary>
        /// 存在しない場合は設定する
        /// </summary>
        /// <param name="building"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public TownBuilding Get(Building building, int index = 0)
        {
            bool isExist = townBuildings.Exists(n => n.name == building.ToString() && n.index == index);
            if (!isExist)
            {
                townBuildings.Add(new TownBuilding(building, index));
                Save();
            }
            return townBuildings.First(n => n.name == building.ToString() && n.index == index);
        }

        public void SetLevel(int level, Building building, int index = 0)
        {
            bool isExist = townBuildings.Exists(n => n.name == building.ToString() && n.index == index);
            if (!isExist)
            {
                townBuildings.Add(new TownBuilding(building, index));
            }
            townBuildings.First(n => n.name == building.ToString() && n.index == index).level = level;
            Save();
        }

        public void Save()
        {
            string json = JsonUtility.ToJson(this);
            if (path == null)
                path = Path.Combine(Application.persistentDataPath, "Building", "TownBuildings.json");
            if (saveLoadService != null)
            {
                saveLoadService.Save(path, json);
            }
            else
            {
                Debug.LogError("SaveLoadService is not initialized in TownBuildings.");
            }
            //Debug.Log("TownBuildings saved");
        }
    }

    [System.Serializable]
    public class TownBuilding
    {
        public string name;
        public int index;
        public int level = 0;

        // 生産
        public string generatingValue = "0"; // 資源を生産する施設の場合は使用する
        public string latestUpdateTime = ""; // ミリ秒までのフォーマット

        public TownBuilding(Building building, int index)
        {
            this.name = building.ToString();
            this.index = index;
            //Debug.Log($"Generate {name} at {index}");
        }

        public void SetLatestUpdateTime(DateTime time)
        {
            latestUpdateTime = time.ToString(CONSTANTSDATE.FORMAT);
        }

        public DateTime GetLatestUpdateTime()
        {
            return DateTime.ParseExact(latestUpdateTime, CONSTANTSDATE.FORMAT, null);
        }

        public void SetGeneratingValue(decimal value)
        {
            generatingValue = value.ToString();
        }

        public void AddGeneratingValue(decimal value, decimal max)
        {
            SetGeneratingValue(Math.Min(value + GetGeneratingValue(), max));
        }

        public decimal GetGeneratingValue()
        {
            return decimal.Parse(generatingValue);
        }
    }

}
