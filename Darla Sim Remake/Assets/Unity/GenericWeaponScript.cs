using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float distanceFromWeapon;
    [SerializeField] private Transform Hand;
     
    void Start()
    {
        //make the weapon and yanchan ignore collision. when rigidbodies touch eachother they "fly" away, we dont want that.
        Physics.IgnoreCollision(weaponCollider, Player.GetComponent<CapsuleCollider>());
        Weapon = gameObject.name;
        thisTransform = gameObject.transform;
        switch (Weapon)
        {
            case "Knife":
                WeaponPosition = new Vector3(4.983105f, 0.0009453297f, 15.39676f);
                WeaponRotation = new Vector3(0,0,90);
                break;
        }
    }

    private void LateUpdate()
    {
        distanceFromWeapon = Vector3.Distance(gameObject.transform.position, Player.transform.position);
        //detect buttonpress if player is 3 meters away.
        if (Input.GetKeyDown(KeyCode.R) && distanceFromWeapon <= 5 && !weaponEquipped)
        {
            PickUp();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !weaponEquipped)
        {
            Drop();
        }
        //make her grab whatever weapon she's holding.
    }

    public void PickUp()
    {
        weaponCollider.isTrigger = true;
        weaponRigidbody.isKinematic = true;
        weaponRigidbody.useGravity = false;
        this.gameObject.transform.parent = Hand;
        this.transform.localPosition = WeaponPosition;
        this.transform.localEulerAngles = WeaponRotation;
    }

    public void Drop()
    {
        weaponCollider.isTrigger = false;
        weaponRigidbody.isKinematic = false;
        weaponRigidbody.useGravity = true;
        this.gameObject.transform.parent = null;
    }
}
