using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{   
    [SerializeField] private EnemyInfoPopup info_Panl;
    [SerializeField] private List<SettlementNode> settlementNodes;


    /// <summary>
    /// 씬 내의 모든 거점 노드를 탐색하여 수집합니다.
    /// </summary>
    private void FindAllNodes()
    {
        SettlementNode[] nodes = FindObjectsByType<SettlementNode>(FindObjectsSortMode.None);
        settlementNodes = new List<SettlementNode>(nodes);
    }
    public void Init(List<int> clearId)
    {   
        info_Panl.Close();
        FindAllNodes();
        foreach (var data in settlementNodes)
        {       

            data.action = Open;
            data.Init(clearId);
        }
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            info_Panl.Close();  
        }
    }

    private void Open(EnemyInfoSO infoSO) 
    {
        info_Panl.Open(infoSO);
    }
}
