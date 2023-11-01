using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPiece : MonoBehaviour
{
    public enum ColorType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        ORANGE,
        ANY,
        COUNT
    };

    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    };

    private ColorType color;
    public ColorType Color
    {
        get { return color; }
        set { SetColor(value); }
    }

    public int NumColors
    {
        get { return colorSprites.Length; }
    }

    private SpriteRenderer sprite;

    public ColorSprite[] colorSprites;
    private Dictionary<ColorType, Sprite> colorSpriteDict;

    private void Awake()
    {
        sprite = transform.Find("Piece").GetComponent<SpriteRenderer>();

        //converting the struct to a Dcitionary
        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for (int i = 0; i < colorSprites.Length; i++)
        {
            if (!colorSpriteDict.ContainsKey(colorSprites[i].color))
            {
                colorSpriteDict.Add(colorSprites[i].color, colorSprites[i].sprite);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(ColorType newColor)
    {
        color = newColor;

        if(colorSpriteDict.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDict[newColor];
        }
    }
}
