using UnityEngine;
using Fungus.DentedPixel;

namespace Fungus
{
    [CommandInfo("GameObject", 
                "Spawn Hero With Flip", 
                "Spawn Object/Prefab to Scene with Position and Flip SpriteRenderer option.")]
    public class SpawnHeroWithFlip : Command
    {
        [Tooltip("Prefab or Object to Spawn")]
        [SerializeField] private GameObject sourceObject;

        [Tooltip("if filled, Object will copy position of this object")]
        [SerializeField] private Transform spawnAtTransform;

        [Header("Manual Coordinates (If Transform Empty)")]
        [Tooltip("Posisi manual (X, Y, Z)")]
        [SerializeField] private Vector3 customPosition;

        [Header("Sprite Flip Settings")]
        [Tooltip("Flip X (Kiri/Kanan)")]
        [SerializeField] private bool flipX = false;

        [Tooltip("Flip Y (Atas/Bawah)")]
        [SerializeField] private bool flipY = false;

        [Header("Hierarchy")]
        [Tooltip("Optional: Make New object as Child of this Transform")]
        [SerializeField] private Transform parentTransform;

        [Header("hero Data")]
        [SerializeField] private ScriptableHero heroData;



        public override void OnEnter()
        {
            if (sourceObject == null)
            {
                Continue();
                return;
            }

            GameObject newObject = Instantiate(sourceObject);

            if (parentTransform != null) newObject.transform.SetParent(parentTransform);

            if (spawnAtTransform != null)
                newObject.transform.position = spawnAtTransform.position;
            else
                newObject.transform.position = customPosition;

            SpriteRenderer spriteRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
            
            if (spriteRenderer != null)
            {
                if (heroData != null && heroData.DefaultSprite != null)
                {
                    spriteRenderer.sprite = heroData.DefaultSprite;
                }

                spriteRenderer.flipX = flipX;
                spriteRenderer.flipY = flipY;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (sourceObject == null) return "Error: No object selected";
            
            string extraInfo = "";
            if (heroData != null) extraInfo += $" [{heroData.name}]";
            if (flipX) extraInfo += " [Flip X]";
            if (flipY) extraInfo += " [Flip Y]";

            string targetText = spawnAtTransform != null ? spawnAtTransform.name : "Custom Pos";
            return $"{sourceObject.name} at {targetText}{extraInfo}";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255); 
        }
    }
}