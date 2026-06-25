using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System;
using Fungus.DentedPixel;

namespace Fungus
{
    [CommandInfo("Movement", 
                "Move At Speed", 
                "Bergerak ke target dengan kecepatan konstan (m/s) menggunakan LeanTween.")]
    public class MoveAtSpeed : Command
    {
        [Tooltip("Objek yang ingin dipindahkan")]
        [SerializeField] private GameObject objectToMove;

        [Tooltip("Transform tujuan")]
        [SerializeField] private Transform targetTransform;

        [Tooltip("Kecepatan gerakan (meter per detik)")]
        [SerializeField] private float speed = 5.0f;

        public override void OnEnter()
        {
            if (objectToMove == null || targetTransform == null)
            {
                Continue();
                return;
            }

            // 1. Hitung jarak antara posisi saat ini dan target
            float distance = Vector3.Distance(objectToMove.transform.position, targetTransform.position);

            // 2. Hitung durasi: Jarak / Kecepatan
            // Tambahkan pengecekan agar tidak dibagi nol
            float duration = (speed > 0) ? (distance / speed) : 0f;

            // 3. Eksekusi gerakan dengan LeanTween
            LeanTween.move(objectToMove, targetTransform.position, duration)
                .setEase(LeanTweenType.linear) // Linear agar kecepatannya stabil
                .setOnComplete(Continue);      // Fungus lanjut setelah sampai
        }

        public override string GetSummary()
        {
            if (objectToMove == null) return "Error: No object selected";
            return $"{objectToMove.name} -> {targetTransform.name} (Speed: {speed} m/s)";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}