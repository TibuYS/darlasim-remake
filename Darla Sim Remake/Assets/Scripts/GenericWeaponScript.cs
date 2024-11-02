using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    Knife
}
public class GenericWeaponScript : MonoBehaviour
{
    [Header("Edit in inspector")]
    [Tooltip("Choose this weapon from the list. If you can't see it, add it to the Weapon enum in GenericWeaponScript.")] public Weapon weaponType = new Weapon();
    [Tooltip("Position when equipped in player's right hand")] public Vector3 WeaponPosition;
    [Tooltip("Rotation when equipped in player's right hand")] public Vector3 WeaponRotation;
    [Tooltip("Is this item a weapon?")] public bool isWeapon = true;
    [Tooltip("Is this a heavy weapon? If it is, it can't be hidden.")] public bool Heavy = false;
    [Tooltip("The sound this item will play when the player picks it up.")]public AudioClip equipSound;
    [Header("Components")]
    public Rigidbody weaponRigidbody;
    public Collider weaponCollider;
    public PromptScript weaponPrompt;
    [Header("Runtime values")]
    public bool weaponEquipped;
    
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q) && weaponEquipped)
        {
            Drop();
        }
    }

    public void PickUp()
    {
        SoundManager.instance.PlaySound(equipSound, 1, false, SoundManager.AudioGroup.SFX);
        weaponPrompt.enabled = false;
        weaponCollider.isTrigger = true;
        weaponRigidbody.isKinematic = true;
        weaponRigidbody.useGravity = false;
        this.gameObject.transform.parent = GameGlobals.instance.Player.Hand;
        if(isWeapon) GameGlobals.instance.Player.holdingWeapon = true;
        GameGlobals.instance.Player.currentItem = this;
        this.transform.localPosition = WeaponPosition;
        this.transform.localEulerAngles = WeaponRotation;
        weaponEquipped = true;
    }

    public void Drop()
    {
        if (isWeapon) GameGlobals.instance.Player.holdingWeapon = false;
        GameGlobals.instance.Player.currentItem = null;
        weaponPrompt.enabled = true;
        weaponEquipped = false;
        weaponCollider.isTrigger = false;
        weaponRigidbody.isKinematic = false;
        weaponRigidbody.useGravity = true;
        this.gameObject.transform.parent = null;
    }
}
