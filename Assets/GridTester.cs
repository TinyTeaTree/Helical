using UnityEngine;

public class GridTester : MonoBehaviour
{
    public GameObject[] Hexs;
    
    [ContextMenu("Do")]
    public void Setup()
    {
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                var hex = Hexs[UnityEngine.Random.Range(0, Hexs.Length)];
                var newHex = Instantiate(hex);
                var poxXZ = new Vector2Int(i, j).ToWorldXZ();
                newHex.transform.position = new Vector3(poxXZ.x, 0f,  poxXZ.y);
                newHex.name = "Hex" + i + "-" + j;
                newHex.transform.localScale = Vector3.one * GridUtils.HexScaleModifier;
            }
        }
    }

}
