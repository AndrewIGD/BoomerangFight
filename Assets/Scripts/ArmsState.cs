using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsState : MonoBehaviour
{
    [Header("Player Object")]
    [SerializeField] Player parent;

    //Methods

        /// <summary>
        /// Idles arms
        /// </summary>
    public void Idle()
    {
        parent.ArmsIdle();
    }

    /// <summary>
    /// Throws boomerang
    /// </summary>
    public void Throw()
    {
        parent.ThrowBoomerang();
    }
}
