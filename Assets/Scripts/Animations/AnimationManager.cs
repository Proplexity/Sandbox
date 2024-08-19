using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] Animator _easterEgg;
    [SerializeField] Gun gun;

    private void EasterEgg()
    {
        if (gun._ammo == 0)
            _easterEgg.SetBool("Play", true);
        else if (gun._ammo != 0)
            _easterEgg.SetBool("Play", false);
            

    }

    private void Update()
    {
        EasterEgg();
    }
}
