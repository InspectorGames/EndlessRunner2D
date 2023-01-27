using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplainTutorialPart : MonoBehaviour
{
    public TutorialParts explain;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (explain)
        {
            case TutorialParts.Jump:
                TutorialManager.OnWaitJump();
                break;
            case TutorialParts.FastFall:
                TutorialManager.OnWaitFastFall();
                break;
            case TutorialParts.Obstacle:
                TutorialManager.OnWaitObstacle();
                break;
            case TutorialParts.ExcavatorFuel:
                TutorialManager.OnWaitExcavatorFuel();
                break;
            case TutorialParts.UseExcavatorFuel:
                TutorialManager.OnWaitUseExcavatorFuel();
                break;
            case TutorialParts.MinecartFuel:
                TutorialManager.OnWaitMinecartFuel();
                break;
            case TutorialParts.Bars:
                TutorialManager.OnBars();
                break;
            case TutorialParts.Wall:
                TutorialManager.OnWaitWall();
                break;
            case TutorialParts.EndTutorial:
                TutorialManager.OnEndTutorial();
                break;
        }
        Destroy(this.gameObject);
    }
}
