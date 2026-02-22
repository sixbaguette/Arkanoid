using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BricCfg", menuName = "SpaceInvaders/BricConfiguration")]
public class BricData : ScriptableObject
{
    [System.Serializable]
    public class BricType
    {
        public string name;
        public int points;
        public Color color;
        public GameObject prefab;

    }

    public List<BricType> bricTypes;
}
