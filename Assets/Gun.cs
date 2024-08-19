using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class Gun : MonoBehaviour
{
    //object refernce
    [SerializeField] Camera _camera;
    [SerializeField] Text _ammoCount;
    

    //GameObjects
    [SerializeField] GameObject gun;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform _bulletFireFromLocation;
    
    
    

    //booleans
    [SerializeField] bool _gunIsSemiAuto = false;
    [SerializeField] bool _gunIsAutomatic = true;
    [SerializeField] bool _gunIsShotgun = false;
    [SerializeField] bool _gunIsGrenadeLauncher = false;
    [SerializeField] bool _fireWasPressedLastFrame = false;
    [SerializeField] bool _swapPressedLastFrame = false;
    [SerializeField] bool reloadISPushedlastFrame = false;



    [Header("Shooting")]
    [SerializeField] float _shotForce;
    [SerializeField] float upwardForce;
    [SerializeField] int _reloadSpeed;
    [SerializeField] int _numberOfShotgunBullets;
    public int _ammo;
    [SerializeField] int _clipSize;
    [SerializeField] int TotalShots;
    [SerializeField] float spread;

    [Header("Timers")]
    [SerializeField] float _fireSpeedTimeCounter = 0.0f;
    [SerializeField] float fireSpeedTime = 1.0f;
    [SerializeField] float _semiAutoFireSpeedCounter;
    [SerializeField] float _semiAutoFireSpeedTime;

    // private script references
    [SerializeField] ControllerInput _input;
    
    

    [SerializeField] int _selectedWeapon;
   
    
    private void rotatingGun()
    {
        gun.transform.localRotation = Quaternion.Euler(new Vector3(-_camera.transform.rotation.eulerAngles.x, 180, 0));
    }

    private void Fire(float x, float y)
    {
        

        // find where to shoot bullet
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        //if raycast hits something shoot bullet towards location of rayHit else shoot at random point
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); // just some random faraway point
        // if aiming down
        Vector3 directionWithoutSpread = targetPoint - _bulletFireFromLocation.position; //Direction from point A -> B = B.pos - A.pos

        //calculate spread
       

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);
         
        if(_input.fireIsPushed > 0.0f && _ammo > 0)
        {
            if(!_fireWasPressedLastFrame)
            {
                _fireSpeedTimeCounter = fireSpeedTime;
            }
           
            if(_gunIsSemiAuto && (!(_fireWasPressedLastFrame)) && _semiAutoFireSpeedCounter <= 0)
            {
                GameObject Bullet = Instantiate(bullet, _bulletFireFromLocation.position, Quaternion.identity);
                Bullet.transform.forward = directionWithoutSpread.normalized;
                if(_input.aimIsPushed)
                    Bullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * _shotForce, ForceMode.Impulse);
                else
                    Bullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * _shotForce, ForceMode.Impulse);

                _ammo--;
                Destroy(Bullet, 1);
                _semiAutoFireSpeedCounter = _semiAutoFireSpeedTime;


            }
            else if (_gunIsAutomatic)
            {
                if(!_fireWasPressedLastFrame && _semiAutoFireSpeedCounter <= 0)
                {
                    GameObject FirstBullet = Instantiate(bullet, _bulletFireFromLocation.position, Quaternion.identity);
                    FirstBullet.transform.forward = directionWithoutSpread.normalized;
                    if(_input.aimIsPushed)
                        FirstBullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * _shotForce, ForceMode.Impulse);
                    else
                        FirstBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * _shotForce, ForceMode.Impulse);

                    // FirstBullet.GetComponent<Rigidbody>().AddForce(_camera.transform.up * upwardForce, ForceMode.Impulse); // for upward curved shots like Grenades launchers
                    _ammo--;
                    Destroy(FirstBullet, 1);
                    _semiAutoFireSpeedCounter = fireSpeedTime;
                }
                if (_fireSpeedTimeCounter <= 0)
                {
                    GameObject Bullet = Instantiate(bullet, _bulletFireFromLocation.position, Quaternion.identity);
                    Bullet.transform.forward = directionWithoutSpread.normalized;
                    if(_input.aimIsPushed)
                        Bullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * _shotForce, ForceMode.Impulse);
                    else
                        Bullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * _shotForce, ForceMode.Impulse);
                    _ammo--;
                    Destroy(Bullet, 1);
                    _fireSpeedTimeCounter = fireSpeedTime;
                }
               
                
            }
            else if(_gunIsShotgun && !_fireWasPressedLastFrame && _semiAutoFireSpeedCounter <= 0)
            {
                
                for (int i = 0; i != _numberOfShotgunBullets; i++)
                {
                    float a = Random.Range(-spread, spread);
                    float b = Random.Range(-spread, spread);

                    Vector3 newDirectionWithSpread = directionWithoutSpread + new Vector3(a, b, 0);

                    GameObject Bullet = Instantiate(bullet, _bulletFireFromLocation.position, Quaternion.identity);
                    Bullet.transform.forward = directionWithoutSpread.normalized;
                    if (_input.aimIsPushed)
                        Bullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * _shotForce, ForceMode.Impulse);
                    else
                        Bullet.GetComponent<Rigidbody>().AddForce(newDirectionWithSpread.normalized * _shotForce, ForceMode.Impulse);

                    Destroy(Bullet, 1);

                    _semiAutoFireSpeedCounter = _semiAutoFireSpeedTime; // for delayed shots intervals
                }
                _ammo--;
                
            }
            else if (_gunIsGrenadeLauncher && !_fireWasPressedLastFrame && _semiAutoFireSpeedCounter <= 0)
            {
                GameObject Bullet = Instantiate(bullet, _bulletFireFromLocation.position, Quaternion.identity);
                Bullet.transform.forward = directionWithoutSpread.normalized;
                Bullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * _shotForce, ForceMode.Impulse);
                Bullet.GetComponent<Rigidbody>().AddForce(_camera.transform.up * upwardForce, ForceMode.Impulse);
                _ammo--;
                Destroy(Bullet, 1);
                _semiAutoFireSpeedCounter = _semiAutoFireSpeedTime;
            }
           
        }
        _fireWasPressedLastFrame = _input.fireIsPushed > 0.0f;


    }

    private void WeaponToggle()
    {
        int i = 0;
        

        if (_input.WeaponIsSwapped && !_swapPressedLastFrame)
        {
            if (_selectedWeapon > transform.childCount - 2)
            {
                _selectedWeapon = 0;
            }
            else
            {
                _selectedWeapon++;
            }
        }

        foreach (Transform weapon in transform)
        {
            if (i == _selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }

        _swapPressedLastFrame = _input.WeaponIsSwapped;

        
    }

    


    //timer is counting for semi auto gun cannot shoot until timer has passed theshold.

    private void AutomaticFireRate()
    {
        if(_input.fireIsPushed > 0.0f)
        {
            if (_fireSpeedTimeCounter > 0)
            {
                _fireSpeedTimeCounter -= Time.fixedDeltaTime;
            }
        }
        if(_fireSpeedTimeCounter <= 0.0f)
        {
            _fireSpeedTimeCounter = 0.0f;
        }
    }
    private void SemiAutoFireRate()
    {

        if(_semiAutoFireSpeedCounter >= 0)
        {
                _semiAutoFireSpeedCounter -= Time.fixedDeltaTime;
        }
        //limit countdown amount
    }

   



    private async void Reload()
    {

        if (_input.reloadISPushed && (!(reloadISPushedlastFrame)))
        {
            await Task.Delay(_reloadSpeed);
            TotalShots -= Mathf.Abs(_clipSize - _ammo);
            reloadISPushedlastFrame = true;
           

        }
        
        
        await Task.Delay(_reloadSpeed);

        if (_input.reloadISPushed && _ammo != _clipSize)
        {
            await Task.Delay(_reloadSpeed); 
            _ammo = _clipSize;
        }
        reloadISPushedlastFrame = false;
    }

    private void Toggle()
    {
        if(_input.toggleIsPushed)
        {
            _gunIsSemiAuto = !_gunIsSemiAuto;
        }
    }

    private void TurnOffScript()
    {
        if (_selectedWeapon == 1)
        {
            this.enabled = false;
        }
    }

    

    private void Update()
    {
       // TurnOffScript();
        WeaponToggle();
        rotatingGun();
        Fire(Random.Range(-spread, spread), Random.Range(-spread, spread));
        Reload();
        Toggle();
        _ammoCount.text = _ammo + "              " + TotalShots;
        //Debug.Log(reloadISPushedlastFrame);
    }

    private void FixedUpdate()
    {
        AutomaticFireRate();
        SemiAutoFireRate();
    }

}
