using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RenderScript : MonoBehaviour
{
    //im not a professional so this might not be best practice. however - it works and im living for some of the results i've got so.. have fun!
    [Header("PLACE THIS SCRIPT ON THE CAMERA. Script made by Tibu. Please credit if used, this wasn't the easiest to figure out..")]
    [Tooltip("This will be assigned when the scene starts. Leave empty.")]public Camera renderCamera;
    [Header("Render Properties")]
    public int renderTextureHeight;
    public int renderTextureWidth;
    [Header("Choose the render key. When this key is pressed, the camera will render what's in view.")]
    public KeyCode renderKey;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Renders"))
        {
            PlayerPrefs.SetInt("Renders", 0);
        }

        if(!Directory.Exists(Application.streamingAssetsPath + "/TibuRenders"))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/TibuRenders");
        }

        renderCamera = gameObject.GetComponent<Camera>();
    }
    public void render()
    {
        RenderTexture renderBackground = new RenderTexture(renderTextureHeight, renderTextureWidth, 24);
        renderCamera.targetTexture = renderBackground;
        renderCamera.Render();
        renderCamera.targetTexture = null;
        RenderTexture.active = renderBackground;
        Texture2D rendered = new Texture2D(renderTextureHeight, renderTextureWidth, TextureFormat.RGB24, false);
        rendered.ReadPixels(new Rect(0, 0, renderTextureHeight, renderTextureWidth), 0, 0);
        rendered.Apply();
        RenderTexture.active = null;
        byte[] bytearray = rendered.EncodeToPNG();
        int am = PlayerPrefs.GetInt("Renders") + 1;
        string filename = "Tib_render" + am.ToString() + ".png";
        if(File.Exists(Application.streamingAssetsPath + "/TibuRenders/" + filename))
        {
            Debug.LogWarning("[RenderScript] File already exists. It's been overwritten.");
        }
        File.WriteAllBytes(Application.streamingAssetsPath + "/TibuRenders/" + filename, bytearray);
        Debug.Log("[RenderScript] Rendered");
        PlayerPrefs.SetInt("Renders", am);

    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(renderKey))
        {
            render();
        }
    }
}
