using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXConst : MonoBehaviour
{
    public static VFXConst instance;
    [SerializeField] public GameObject hitEffect;
    [SerializeField] public GameObject slashEffect;
    [SerializeField] public GameObject sparkEffect;
    [SerializeField] public GameObject healEffect;
    [SerializeField] public GameObject slash2Effect;
    [SerializeField] public GameObject powerAuraEffect;

    private void Awake() {
        instance = this;
    }

}
