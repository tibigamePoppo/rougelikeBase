using UnityEngine;
using UniRx;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Scenes.MainScene.Player;

namespace Scenes.Title
{
    public enum SheetType
    {
        UnitData,
        EnemyGroupData,
        EventTable,
        EventLimitTable,
        EventEffetcArg
    }
    public class MasterDataLoadModel
    {
        private IntReactiveProperty _progress = new IntReactiveProperty();
        public IObservable<int> OnProgress => _progress;
        private const string SPREAD_SHEET_ID = "1q0lFCISt-xxfq8HAwQ1RDjZZSHe0qqI1A9WNYHlB_oU";

        public void Init()
        {

        }

        public async void LoadMasterDataProcess()
        {
            _progress.Value = 0;
            await LoadWebRequest(SheetType.UnitData) ;
            _progress.Value = 10;
            await LoadWebRequest(SheetType.EnemyGroupData);
            _progress.Value = 20;
        }
        private async UniTask LoadWebRequest(SheetType sheetType)
        {
            Debug.Log("MasterDataApiClient.LoadStart!");
            string url = $"https://docs.google.com/spreadsheets/d/{SPREAD_SHEET_ID}/gviz/tq?tqx=out:csv&sheet={sheetType.ToString()}";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var masterData = ConvertToArrayListFrom(request.downloadHandler.text);
                    Debug.Log($"masterData.Length = {masterData.Count}");
                    Debug.Log($"masterData[0].Length = {masterData[0].Count}");
                    Debug.Log($"masterData[0][0].Value = {masterData[0][0]}");
                    GenerateDataList(sheetType, masterData);
                }
                else
                {
                    Debug.LogError("Request failed: " + request.error);
                }
            }
        }

        private List<List<string>> ConvertToArrayListFrom(string _text)
        {
            List<List<string>> characterDataArrayList = new List<List<string>>();
            StringReader reader = new StringReader(_text);
            reader.ReadLine();
            reader.ReadLine();
            reader.ReadLine();
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                string[] elements = line.Split(',');
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i] == "\"\"")
                    {
                        continue;
                    }
                    elements[i] = elements[i].TrimStart('"').TrimEnd('"');
                }
                characterDataArrayList.Add(elements.ToList());
            }
            return characterDataArrayList;
        }

        private void GenerateDataList(SheetType type,List<List<string>> dataList)
        {
            switch (type)
            {
                case SheetType.UnitData:
                    var _cardDataList = Resources.Load<CardPool>("Value/MasterDataPool/AllUnitPool").CardList();
                    foreach (var data in dataList)
                    {
                        int id = int.Parse(data[0]);
                        var targetUnit = _cardDataList.FirstOrDefault(c => c.status.id == id);
                        if (targetUnit == default || targetUnit == null) continue;

                        string name = data[1];
                        string text = data[2];
                        var weaponType = StringToUnitWeaponType(data[3]);
                        var group = StringToUnitGroup(data[4]);
                        float hp = float.Parse(data[5]);
                        float shiled = float.Parse(data[6]);
                        float attack = float.Parse(data[7]);
                        float attackRange = float.Parse(data[8]);
                        float attackspeed = float.Parse(data[9]);
                        float speed = float.Parse(data[10]);
                        UnitStatus newUnitData = new UnitStatus(id,name, targetUnit.status.sprite, text, weaponType, group, hp, shiled, attack, attackRange, attackspeed, speed);

                        // update data
                        targetUnit.status = newUnitData;
                    }
                    break;
                case SheetType.EnemyGroupData:
                    var _enemyGroups = Resources.Load<EnemyDataPool>("Value/MasterDataPool/AllEnemyGroupData").normalPool;
                    var _cardData = Resources.Load<CardPool>("Value/MasterDataPool/AllUnitPool").CardList();
                    foreach (var data in dataList)
                    {
                        int id = int.Parse(data[0]);
                        var targetGroup = _enemyGroups.FirstOrDefault(c => c.id == id);
                        if (targetGroup == default || targetGroup == null) continue;

                        string name = data[1];
                        int minStageDepth = int.Parse(data[2]);
                        int maxStageDepth = int.Parse(data[3]);
                        UnitEnemyGroup unitGroupPosition1 = IntArrayToUnitEnemyGroup(SplitIntoThreeDigits(data[4]), _cardData);
                        UnitEnemyGroup unitGroupPosition2 = IntArrayToUnitEnemyGroup(SplitIntoThreeDigits(data[5]), _cardData);
                        UnitEnemyGroup unitGroupPosition3 = IntArrayToUnitEnemyGroup(SplitIntoThreeDigits(data[6]), _cardData);
                        UnitEnemyGroup unitGroupPosition4 = IntArrayToUnitEnemyGroup(SplitIntoThreeDigits(data[7]), _cardData);
                        UnitEnemyGroup unitGroupPosition5 = IntArrayToUnitEnemyGroup(SplitIntoThreeDigits(data[8]), _cardData);
                        UnitEnemyGroup unitGroupPosition6 = IntArrayToUnitEnemyGroup(SplitIntoThreeDigits(data[9]), _cardData);
                        UnitEnemyGroup[] unitEnemyGroups = new UnitEnemyGroup[] { unitGroupPosition1, unitGroupPosition2, unitGroupPosition3, unitGroupPosition4, unitGroupPosition5, unitGroupPosition6 };

                        // update data
                        targetGroup.name = name;
                        targetGroup.minStageDepth = minStageDepth;
                        targetGroup.maxStageDepth = maxStageDepth;
                        targetGroup.unitGroupData = unitEnemyGroups;
                    }
                    break;
                default:
                    break;
            }

        }

        private int[] SplitIntoThreeDigits(string input)
        {
            return input.Length < 3 ? null : input.Split('/').Select(int.Parse).ToArray();
        }

        private UnitEnemyGroup IntArrayToUnitEnemyGroup(int[] inputs,List<UnitData> baseData)
        {
            UnitEnemyGroup unitEnemyGroup = new UnitEnemyGroup();
            List<UnitData> unitDatas = new List<UnitData>();
            if (inputs == null)
            {
                unitEnemyGroup.unitData = unitDatas.ToArray();
                return unitEnemyGroup;
            }
            else
            {
                foreach (var input in inputs)
                {
                    var unitData = baseData.Where(b => b.status.id == input).FirstOrDefault();
                    if (unitData == default)
                    {
                        Debug.LogWarning($"invalid enemy id {input}");
                    }
                    else
                    {
                        unitDatas.Add(unitData);
                    }
                }
                unitEnemyGroup.unitData = unitDatas.ToArray();
                return unitEnemyGroup;
            }
        }

        public UnitWeaponType StringToUnitWeaponType(string stateStr)
        {
            if (Enum.TryParse(stateStr, true, out UnitWeaponType result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("error: " + stateStr);
            }
        }

        public UnitGroup StringToUnitGroup(string stateStr)
        {
            if (Enum.TryParse(stateStr, true, out UnitGroup result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("error: " + stateStr);
            }
        }
    }
}
