using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowTimer : MonoBehaviour
{
    private BoxCollider2D fox;
    private BoxCollider2D timerCollider;

    private BoxCollider2D upperBackground;
    private BoxCollider2D lowerBackground;
    private BoxCollider2D threshold;
    private BoxCollider2D ground1;
    private BoxCollider2D ground2;
    // Start is called before the first frame update
    void Start()
    {
        fox = GameObject.Find("Foxy").GetComponent<BoxCollider2D>();
        timerCollider = GetComponent<BoxCollider2D>();
        upperBackground = GameObject.Find("back").GetComponent<BoxCollider2D>();
        lowerBackground = GameObject.Find("back 2").GetComponent<BoxCollider2D>();
        threshold = GameObject.Find("threshold").GetComponent<BoxCollider2D>();
        ground1 = GameObject.Find("foreground").GetComponent<BoxCollider2D>();
        ground2 = GameObject.Find("foreground 2").GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(timerCollider, upperBackground);
        Physics2D.IgnoreCollision(timerCollider, lowerBackground);
        Physics2D.IgnoreCollision(timerCollider, threshold);
        Physics2D.IgnoreCollision(timerCollider, ground1);
        Physics2D.IgnoreCollision(timerCollider, ground2);
        Physics2D.IgnoreCollision(timerCollider, fox);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "StartLine")
        {
            DataEntry entry = new DataEntry(
            GameControl.instance.stopwatch.ElapsedMilliseconds,
            GameControl.instance.currTarget,
            GameControl.instance.sensorValue,
            LogType.JUMP_WINDOW_START
        );
            GameControl.instance.logger.addEntry(entry);
        }
        if (collision.gameObject.tag == "FinishLine")
        {
            DataEntry entry = new DataEntry(
            GameControl.instance.stopwatch.ElapsedMilliseconds,
            GameControl.instance.currTarget,
            GameControl.instance.sensorValue,
            LogType.JUMP_WINDOW_END
        );
            GameControl.instance.logger.addEntry(entry);
        }
    }
}
