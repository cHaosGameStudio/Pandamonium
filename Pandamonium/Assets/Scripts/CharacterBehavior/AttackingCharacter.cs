﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public abstract class AttackingCharacter : MonoBehaviour, IAttackable {

    public Weapon[] weapons;
    public int equippedWeaponIndex;                                               // opremljeno oruzje igraca

    public float maxHealth = 25;

    public enum CharacterType { PLAYER, ENEMY, BOSS }
    public CharacterType type;

    public float normalSpeed = 6;

    public ChaosHealthBar healthBar;
    public Image nextAttackBar;

    public CharacterVision vision;

    protected GameObject nextAttackBG;

    public enum PlayerState { IDLE, CHASING_ENEMY, ATTACKING, WALKING, DASHING, IMMOBILE }                     
    
    [HideInInspector]
    public PlayerState playerState = PlayerState.IDLE;                       // trenutno stanje igraca

    protected float health;

    protected CharacterMovement CM;

    [HideInInspector]
    public int ignoreMask;
    protected ContactFilter2D colFilter;

    protected AIPath path;

    protected static float maxRaycastDistance = 50;

    protected Vector2 approxPosition;
    protected Bounds currBounds;

    private ArrayList dotSources = new ArrayList();

    protected Rigidbody2D rb;

    [HideInInspector]
    public SpriteRenderer sprite;

    [HideInInspector]
    public bool isDead = false;

    protected bool isKnockedBack = false;

    protected bool attackable = true;

    public bool IsAttackable()
    {
        return attackable;
    }

    public virtual void Awake()
    {

        CM = GetComponent<CharacterMovement>();
        path = GetComponent<AIPath>();
    }

    public virtual void Start()
    {

        //ignoreMask = (1 << LayerMask.NameToLayer("Obstacles")) | (1 << LayerMask.NameToLayer("Characters"));
        ignoreMask = 1 << LayerMask.NameToLayer("Characters");

        colFilter.useLayerMask = true;
        colFilter.SetLayerMask(ignoreMask);

        rb = GetComponent<Rigidbody2D>();

        if(vision == null)
            vision = transform.Find("Vision").GetComponent<CharacterVision>();

        health = maxHealth;
        healthBar.BuildHealtBar(maxHealth, !(type.Equals(CharacterType.PLAYER) || type.Equals(CharacterType.BOSS)));

        nextAttackBG = nextAttackBar.transform.parent.gameObject;

        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Attack() { }

    public void ChangeWeapon()
    {
        weapons[equippedWeaponIndex].gameObject.SetActive(false);
        equippedWeaponIndex = (equippedWeaponIndex + 1) % weapons.Length;
        weapons[equippedWeaponIndex].gameObject.SetActive(true);
    }

    public Weapon GetWeapon()
    {
        return weapons[equippedWeaponIndex];
    }

    public virtual void OnWeaponAttack()
    {

    }

    public Room GetRoom()
    {

        Room ret = LevelGeneration.I.GetRoomAtPos(transform.position);

        if(ret == null)
        {
            ret = LevelGeneration.I.bossRoom;           // ???
        }

        return ret;
    }

    public virtual Vector2 GetFacingDirection()
    {
        return rb.velocity;
    }

    // metoda proverava da li karakter vidi target na zadatom range-u (od drugih karaktera i obstacle-a)

    public bool CanSee(Transform target, float range = Mathf.Infinity)
    {

        if (range == Mathf.Infinity && !weapons[equippedWeaponIndex].IsInRange(target))
            return false;

        range = Mathf.Clamp(range, 0, maxRaycastDistance);

        Vector3 startCast = transform.position;
        Vector3 endCast = target.position;

        Debug.DrawRay(startCast, endCast - startCast);

        RaycastHit2D[] results;

        results = Physics2D.CircleCastAll(startCast, 0.2f, (endCast - startCast).normalized, range, colFilter.layerMask);

        for (int i = 0; i < results.Length; i++) // ako mu je protivnik vidljiv (od zidova/prepreka)
        {

            AttackingCharacter attChar = results[i].transform.GetComponent<AttackingCharacter>();
            if (attChar && attChar.type == type)
                continue;

            if(results[i].transform.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                return false;
            }

            if (results[i].transform == target)
            {
                return true;
            }
        }

        return false;

    }

    public virtual void Heal(float amount = 200, bool percent = false)
    {

        float newHealth = (percent ? health + maxHealth * (amount / 100f) : health + amount);

        health = Mathf.Clamp(newHealth, 0, maxHealth);
        healthBar.Heal(health / maxHealth);

        UIManager.I.ShowHeal(GetComponentInChildren<Canvas>(), 1);
    }

    protected void UpdateGraph()
    {

        /*Vector2 newApproxPosition = new Vector2(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y)) + new Vector2(0.5f, 0.5f);

        if (!newApproxPosition.Equals(approxPosition))
        {

            GraphUpdateObject guo = new GraphUpdateObject(currBounds)
            {
                updatePhysics = true,
                modifyTag = true,
                setTag = 0
            };
            AstarPath.active.UpdateGraphs(guo);

            currBounds = new Bounds(newApproxPosition, Vector3.one);
            guo = new GraphUpdateObject(currBounds)
            {
                updatePhysics = true,
                modifyTag = true,
                setTag = (int)type + 1
            };
            AstarPath.active.UpdateGraphs(guo);

            approxPosition = newApproxPosition;

        }*/
    }

    protected virtual void Update()
    {

        sprite.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);

        if (isDead)
            return;

        nextAttackBar.fillAmount = 1 - weapons[equippedWeaponIndex].timeToAttack;

        if (weapons[equippedWeaponIndex].timeToAttack <= 0 || weapons[equippedWeaponIndex].timeToAttack == 1)
        {
            nextAttackBG.SetActive(false);
        }
        else
        {
            nextAttackBG.SetActive(true);
        }

    }

    protected IEnumerator ColorTransition(Color color)
    {
        sprite.color = Color.red;

        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * 2;
            sprite.color = Color.Lerp(color, Color.white, i);
            yield return null;
        }
    }

    public virtual bool TakeDamage(float damage)
    {

        if(GameManager.I.playerInstance == this)
        {
            UIManager.I.ShowHitDamage(GetComponentInChildren<Canvas>(), 1, damage, true);
        }
        else
        {
            UIManager.I.ShowHitDamage(GetComponentInChildren<Canvas>(), 1, damage);
        }
        

        if ((health -= damage) <= 0)    // * armorReduction
        {
            Die();
        }
        else
        {
            StartCoroutine(ColorTransition(Color.red));
        }

        return true;
    }

    public virtual void TakePoisonDamage(float damage)
    {

        if (GameManager.I.playerInstance == this)
        {
            UIManager.I.ShowPoisonDamage(GetComponentInChildren<Canvas>(), 1, damage);
        }
        else
        {
            UIManager.I.ShowHitDamage(GetComponentInChildren<Canvas>(), 1, damage);
        }


        if ((health -= damage) <= 0)    // * armorReduction
        {
            Die();
        }
        else
        {
            StartCoroutine(ColorTransition(Color.red));
        }
    }

    protected IEnumerator Knockback(Vector2 dir, float force)
    {
        isKnockedBack = true;
        PlayerState lastState = playerState;
        RigidbodyType2D lastType = rb.bodyType;

        playerState = PlayerState.IMMOBILE;

        bool prevPath = false;
        if (path)
        {
            prevPath = path.enabled;
            path.enabled = false;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(dir * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.3f);

        if (path)
        {
            path.enabled = prevPath;
        }

        rb.bodyType = lastType;
        playerState = lastState;

        isKnockedBack = false;
    }

    public virtual bool TakeDamageWithKnockback(float damage, Vector2 dir, float force)
    {
        bool takenDamage = TakeDamage(damage);

        if (playerState != PlayerState.DASHING && !isKnockedBack)
        {
            StartCoroutine(Knockback(dir, force));
        }

        return takenDamage;
    }

    protected virtual IEnumerator DoT(Transform source, float damage, float interval, int times)
    {

       
        float poisonDamage = (damage * times) / maxHealth;
        healthBar.PoisonOn(poisonDamage, health / maxHealth);    
        while (times-- > 0)
        {
            yield return new WaitForSeconds(interval);
            //TakeDamage(damage);
            TakePoisonDamage(damage);
        }
        healthBar.PoisonOff(poisonDamage, health / maxHealth);

        if (dotSources.Contains(source))
        {
            dotSources.Remove(source);
        }
    }

    // interval - interval na koji ce igrac primati damage (u sekundama); timeInIntervals - trajanje DoT-a u intervalima
    public virtual void TakeDamageOverTime(float damage, float interval, int times, Transform source = null)
    {
        if (!dotSources.Contains(source))
        {
            dotSources.Add(source);
            StartCoroutine(DoT(source, damage, interval, times));
        }

        
    }

    protected abstract IEnumerator Death();

    protected virtual void Die()
    {
        if (!isDead)
        {
            isDead = true;

            StartCoroutine(Death());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        /*AttackingCharacter attChar = collision.gameObject.GetComponent<AttackingCharacter>();

        if(playerState == PlayerState.DASHING && attChar && attChar.type != type)
        {
            attChar.TakeDamage(weapons[equippedWeaponIndex].damage, Vector3.zero);
        }*/
    }

    protected bool IsMoving()
    {

        return !(Mathf.Approximately(path.velocity.x, 0) && Mathf.Approximately(path.velocity.y, 0));

    }

}
