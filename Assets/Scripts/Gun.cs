using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float TimeToReload = 2f;
    public int TotalBulletCount = 90;
    public int currentBulletCount = 30;
    public int maxBulletMagazine = 30;

    public float fireRate = 0.5F;
    float nextFire = 0.0F;

    public float recoilKickback = 0.2f;
    public float recoilAngleStep = 5f;
    public float recoilMaxAngle = 30f;
    public float recoilReturnStep = 0.1f;

    float recoilAngle;
    float recoilRotSmoothDampVelocity;

    Vector3 recoilSmoothDampVelocity;
    Vector3 originalLocalPosition;
    Vector3 originalLocalEuler;

    public Camera fpsCamera;
    public ParticleSystem muzzleFlash;

    public Text bulletText;

    bool automaticFire = true;
    bool isReloading = false;

    private void Start()
    {
        originalLocalPosition = transform.localPosition;
        originalLocalEuler = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        bulletText.text = currentBulletCount.ToString() + " / " + TotalBulletCount.ToString();

        if (!isReloading)
        {
            
            if (Input.GetButtonDown("Reload") && currentBulletCount < maxBulletMagazine)
            {
                StartCoroutine(Reload());
            }

            if (Input.GetButtonDown("FireType"))
            {
                automaticFire = !automaticFire;
                if (!automaticFire)
                {
                    damage *= 2;
                }   else
                {
                    damage /= 2;
                }
            }

            if (automaticFire)
            {
                if (Input.GetButton("Fire1") && Time.time > nextFire)
                {
                    nextFire = Time.time + fireRate;
                    Shoot();
                }
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
    }

    private void LateUpdate()
    {
        StopRecoil();
        transform.localEulerAngles = originalLocalEuler + Vector3.left * recoilAngle;
        CheckHitElement();
    }

    void CheckHitElement()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hitInfo, range))
        {
            Target target = hitInfo.transform.GetComponent<Target>();

            if (target != null)
            {
                if (target is EnemyTarget target1)
                {
                    target1.ShowInfo();
                }
            }
        }
    }

    void Shoot()
    {
        if (currentBulletCount == 0)
        {
            StartCoroutine(Reload());
            return;
        }

        currentBulletCount--;
        muzzleFlash.Play();
        AddRecoil();
   
        RaycastHit hitInfo;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hitInfo, range))
        {

            Target target = hitInfo.transform.GetComponent<Target>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(TimeToReload);

        int bulletsToReload = maxBulletMagazine - currentBulletCount;

        if (bulletsToReload > TotalBulletCount)
        {
            currentBulletCount += TotalBulletCount;
            TotalBulletCount = 0;

        } else
        {
            currentBulletCount += bulletsToReload;
            TotalBulletCount -= bulletsToReload;
        }

        isReloading = false;
    }

    void AddRecoil()
    {
        transform.localPosition -= Vector3.forward * recoilKickback;
        recoilAngle += recoilAngleStep;
        recoilAngle = Mathf.Clamp(recoilAngle, 0, recoilMaxAngle);
    }

    void StopRecoil()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, 
            originalLocalPosition, ref recoilSmoothDampVelocity, recoilReturnStep);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilReturnStep);
    }
}
