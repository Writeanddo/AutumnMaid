using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBroom : MonoBehaviour
{
    [Header("Parameters")]
    public float m_SweepSpeed;
    public float m_ArcDegrees;
    public float m_Range;
    //private float m_PosMod = 2.0f;
    public LayerMask m_TargetLayers;
    [SerializeField] private Transform m_HolderObj;
    [SerializeField] private Transform m_Target;
    [SerializeField] private Transform m_AttackPoint;
    [SerializeField] private bool m_isAttacking;
    public float m_SweepDelay;
    private float m_SweepTimer;

    [Header("After Image Components")]
    public float m_ImageDistance;
    public float m_LastImageRot;
    
    [Header("Components")]
    [SerializeField] private AudioSource m_as;
    private GameObject m_Sprite;
    [SerializeField] private ParticleSystem m_ps;
    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private SpriteRenderer m_sr;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;

    void Awake()
    {
        m_Sprite = m_sr.gameObject;

        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Sweep.performed += Sweep;
    }

    void Update()
    {
        /*if((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.L)))
        {
            m_SweepTimer = Time.time + m_SweepDelay;
        }*/
        
        if(m_isAttacking)
        {
            m_sr.enabled = true;
        }
        else
        {
            m_sr.enabled = false;
        }
        transform.position = m_HolderObj.position;
    }

    void FixedUpdate()
    {
        if(m_SweepTimer > Time.time && !m_isAttacking && !GameManager.instance.m_IsDelivering)
        {
            AfterImagePool.instance.GetFromPool();
            StartCoroutine("Attack");
        }
    }

    public void Sweep(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            m_SweepTimer = Time.time + m_SweepDelay;
        }
    }

    void CreateAfterImage()
    {
        if(Mathf.Abs(transform.eulerAngles.z - m_LastImageRot) >= m_ImageDistance)
        {
            AfterImagePool.instance.GetFromPool();
            m_LastImageRot = transform.eulerAngles.z;
        }
    }

    void HitTargets()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(m_AttackPoint.position, m_Range, m_TargetLayers);
        foreach(Collider2D hitTarget in hits)
        {
            if(hitTarget.gameObject.CompareTag("NPC"))
            {
                Entity e = hitTarget.GetComponent(typeof(Entity)) as Entity;
                if(e == null) return;
                e.SpinSprite();
            }

            if(hitTarget.gameObject.CompareTag("Destructable"))
            {
                Destructable d = hitTarget.GetComponent(typeof(Destructable)) as Destructable;
                d.GetHit();
            }

            if(hitTarget.gameObject.CompareTag("Ball"))
            {
                BeachBall b = hitTarget.transform.parent.GetComponent<BeachBall>();
                b.BounceBall(GameObject.FindGameObjectWithTag("Player").transform, true);
            }
        }
    }

    IEnumerator Attack()
    {
        m_ps.Play();
        m_as.pitch = Random.Range(0.9f, 1.0f);
        m_as.Play();

        Vector3 difference = m_Target.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotZ + m_ArcDegrees * -1f);

        m_isAttacking = true;
        bool attackAgain = false;

        HitTargets();

        float t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime * m_SweepSpeed;
            float rotMod = m_ArcDegrees * (-1f + t * 2f);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotZ + rotMod);
            CreateAfterImage();

            if(playerInputActions.Player.Sweep.WasPerformedThisFrame())
            {
                if(t > 0.25f)
                {
                    attackAgain = true;
                }
            }

            yield return null;
        }

        if(attackAgain)
        {
            HitTargets();
            
            m_as.pitch = Random.Range(1.1f, 1.2f);
            m_as.Play();
            t = 0.0f;
            while(t < 1.0f)
            {
                t += Time.deltaTime * (m_SweepSpeed * 1.25f);
                float rotMod = m_ArcDegrees * (-1f + t * 2f);
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotZ - rotMod);
                CreateAfterImage();

                yield return null;
            }
        }

        yield return new WaitForSeconds(0.1f);
        StopAttack();

        yield return null;
    }

    public void StopAttack()
    {
        m_isAttacking = false;
        m_ps.Stop();
    }

    void OnDisable()
    {
        m_sr.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        if(m_Target == null) return;
        Gizmos.DrawWireSphere(m_AttackPoint.position, m_Range);
    }
}
