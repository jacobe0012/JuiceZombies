using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipDrawingTest : MonoBehaviour
{
    public Image imageBack;
    public TextMeshProUGUI textMeshPro;

    // Start is called before the first frame update

    public void initDrawing()
    {
        //对名称进行更改
        textMeshPro.text = "图纸";
    }

    public void initMaterials()
    {
        //对名称进行更改
        textMeshPro.text = "通用合成材料";
    }

    public void initPanel(string name)
    {
        textMeshPro.text = name;
    }
}