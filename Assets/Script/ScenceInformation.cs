using Cysharp.Threading.Tasks;
using MapFrame;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenceInformation : MonoBehaviour
{
    [SerializeField] TextAsset mapInformationJson;
    [SerializeField] PathFindingMapType pathFindingMap;
    
    public NormalMap normalMap { get; private set; }

    public async UniTask LoadScenceInformation()
    {
        this.normalMap = new NormalMap();
        this.normalMap.SetMapType(this.pathFindingMap);
        this.normalMap.mapData = JsonConvert.DeserializeObject<MapData>(this.mapInformationJson.text);

        await this.normalMap.SetGridData(this.normalMap.mapData);
    }

}
