using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExcavatorV2 : MonoBehaviour
{
    [SerializeField] private GameObject effectPf;
    private GameObject instatiatedEffect;

    private void OnEnable()
    {
        EventManager.EnterWall.Enqueue(10, SpawnEffect);
        EventManager.Track += DestroyEffect;
        EventManager.MainMenuLoaded += DestroyEffect;
        EventManager.GameRestarted += DestroyEffect;
    }

    private void OnDisable()
    {
        EventManager.EnterWall.Dequeue(10);
        EventManager.Track -= DestroyEffect;
        EventManager.MainMenuLoaded -= DestroyEffect;
        EventManager.GameRestarted -= DestroyEffect;
    }

    private void SpawnEffect()
    {
        instatiatedEffect = Instantiate(effectPf, new Vector3(transform.position.x, 1), Quaternion.identity);
        instatiatedEffect.GetComponent<MineEffect>().SetPlayer(transform);
    }

    private void DestroyEffect()
    {
        Destroy(instatiatedEffect);
    }

    public void SpawnEffectTutorial()
    {
        SpawnEffect();
    }

    public void DestroyEffectTutorial()
    {
        DestroyEffect();
    }
}
