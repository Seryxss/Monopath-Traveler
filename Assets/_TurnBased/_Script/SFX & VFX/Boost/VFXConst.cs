using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXConst : MonoBehaviour
{
    public static VFXConst instance;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject slashEffect;
    [SerializeField] private GameObject sparkEffect;
    [SerializeField] private GameObject healEffect;
    [SerializeField] private GameObject slash2Effect;
    [SerializeField] private GameObject powerAuraEffect;

    private void Awake() {
        instance = this;
    }

}
