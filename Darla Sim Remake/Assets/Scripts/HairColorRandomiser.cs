using UnityEngine;

public class HairColorRandomiser : MonoBehaviour
{
    public SkinnedMeshRenderer hairRenderer;
    public Material originalHairMaterial;
    public Material newHairMaterial;
    public StudentScript student;

    private void Start()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        student.subtitleColor = randomColor;
        Material[] materials = hairRenderer.sharedMaterials;
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] == originalHairMaterial)
            {
                newHairMaterial = Instantiate(originalHairMaterial);
                newHairMaterial.color = randomColor;
                materials[i] = newHairMaterial;
                hairRenderer.sharedMaterials = materials;
                break;
            }
        }

        if(newHairMaterial != null)
        {
            newHairMaterial.color = randomColor;
        }
    }
}
