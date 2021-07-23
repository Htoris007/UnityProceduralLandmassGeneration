using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomGradient
{

    public enum BlendMode{Linear, Discrete};
    public BlendMode blendMode;
    public bool randomizedColor;

    [SerializeField]
    List<ColorKey> keys = new List<ColorKey>();

    public CustomGradient()
    {
        AddKey(Color.white, 0);
        AddKey(Color.black, 1);
    }

    public int AddKey(Color color, float height)
    {
        ColorKey newKey = new ColorKey(color, height);
        for (int i = 0; i < keys.Count; i++)
        {
            if (newKey.Height < keys[i].Height)
            {
                keys.Insert(i, newKey);
                return i;
            }
        }
        keys.Add(newKey);
        return keys.Count - 1;
    }

    public void RemoveKey(int index)
    {
        if (keys.Count >= 2)
        {
            keys.RemoveAt(index);
        }
    }

    public int UpdateKeyHeight(int index, float height)
    {
        Color oldKeyColor =keys[index].Color;
        RemoveKey(index);
        return AddKey(oldKeyColor, height);
    }

    public void UpdateKeyColor(int index, Color newColor)
    {
        keys[index] = new ColorKey(newColor, keys[index].Height);
    }

    public List<ColorKey> Keys
    {
        get
        {
            return keys;
        }
    } 

    public int NumKeys
    {
        get
        {
            return keys.Count;
        }
    }

    public ColorKey GetKey(int i)
    {
        return keys[i];
    }

    public Color Evaluate(float height)
    {
        ColorKey keyLeft = keys[0];
        ColorKey keyRight = keys[keys.Count - 1];
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].Height <= height)
            {
                keyLeft = keys[i];
            }
            if ( keys[i].Height >= height)
            {
                keyRight = keys[i];
                break;           
            }
        }

        if (blendMode == BlendMode.Linear)
        {
            float blendHeight = Mathf.InverseLerp(keyLeft.Height, keyRight.Height, height);
            return Color.Lerp(keyLeft.Color, keyRight.Color, blendHeight);
        }
        return keyRight.Color;
    }

    public Texture2D GetTexture(int width)
    {
        Texture2D texture = new Texture2D(width, 1);
        Color[] colors = new Color[width];
        for (int i = 0; i < width; i++)
        {
            colors[i] = Evaluate((float)i/ (width - 1));
        }
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }

    [System.Serializable]
    public struct ColorKey
    {
        [SerializeField]
        Color color;
        [SerializeField]
        float height;

        public ColorKey(Color color, float height)
        {
            this.color = color;
            this.height = height;
        }

        public Color Color
        {
            get
            {
                return color;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }
        }
    }
}
