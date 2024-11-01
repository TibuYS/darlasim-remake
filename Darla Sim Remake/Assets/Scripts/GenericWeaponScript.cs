using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    Knife
}
public class GenericWeaponScript : MonoBehaviour
{
    [SerializeField] private string Weapon;
    [SerializeField] private GameObject Player;
    [SerializeField] private bool Heavy = false;
    [SerializeField] private Transform thisTransform;
    [SerializeField] private Vector3 WeaponRotation;
    [SerializeField] private Vector3 WeaponPosition;
    [SerializeField] private Rigidbody weaponRigidbody;
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private bool weaponEquipped;
    private PromptScript weaponPrompt;
     
    void Start()
    {
        //make the weapon and yanchan ignore collision. when rigidbodies touch eachother they "fly" away, we dont want that.
       // Physics.IgnoreCollision(weaponCollider, Player.GetComponent<CapsuleCollider>());
        Weapon = gameObject.name;
        thisTransform = gameObject.transform;
        weaponPrompt = GetComponent<PromptScript>();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q) && weaponEquipped)
        {
            Drop();
        }
    }

    public void PickUp()
    {
        weaponPrompt.enabled = false;
        weaponCollider.isTrigger = true;
        weaponRigidbody.isKinematic = true;
        weaponRigidbody.useGravity = false;
        this.gameObject.transform.parent = GameGlobals.instance.Player.Hand;
        this.transform.localPosition = WeaponPosition;
        this.transform.localEulerAngles = WeaponRotation;
        weaponEquipped = true;
    }

    public void Drop()
    {
        weaponPrompt.enabled = true;
        weaponEquipped = false;
        weaponCollider.isTrigger = false;
        weaponRigidbody.isKinematic = false;
        weaponRigidbody.useGravity = true;
        this.gameObject.transform.parent = null;
    }
}
