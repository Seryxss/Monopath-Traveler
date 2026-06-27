using UnityEngine;
using UnityEngine.UI;

public class TurnIconVisual : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private Image frameImage;
    [SerializeField] private Sprite heroFrame;
    [SerializeField] private Sprite enemyFrame;

    public void Setup(CharacterBase character)
    {
        if (character == null || character.CharacterData == null) return;

        if (portraitImage != null)
            portraitImage.sprite = character.CharacterData.MenuSprite;

        if (frameImage != null)
        {
            frameImage.sprite = character is HeroCharBase ? heroFrame : enemyFrame;
        }
    }
} 