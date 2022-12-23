using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    //Game Flow
    public static event UnityAction MainMenuLoaded;
    public static event UnityAction GameStarted;
    public static PriorityActionQueue EnterWall = new PriorityActionQueue();
    public static PriorityActionQueue Wall = new PriorityActionQueue();
    public static PriorityActionQueue EnterTrack = new PriorityActionQueue();
    public static event UnityAction GameOver;
    public static event UnityAction GameRestarted;

    public static void OnMainMenuLoaded() => MainMenuLoaded?.Invoke();
    public static void OnGameStarted() => GameStarted?.Invoke();
    public static void OnEnterWall() => EnterWall?.Invoke();
    public static void OnWall() => Wall?.Invoke();
    public static void OnEnterTrack() => EnterTrack?.Invoke();
    public static void OnGameOver() => GameOver?.Invoke();
    public static void OnGameRestarted() => GameRestarted.Invoke();

    //Minecart Fuel
    public static event UnityAction MinecartFuelCollected;
    public static void OnMinecartFuelCollected() => MinecartFuelCollected?.Invoke();

    //Excavator Fuel
    //public static event UnityAction ExcavatorFuelCollected;
    public static PriorityActionQueue ExcavatorFuelCollected = new PriorityActionQueue();
    //public static event UnityAction ExcavatorFuelConsumed;
    public static PriorityActionQueue ExcavatorFuelConsumed = new PriorityActionQueue();
    
    public static void OnExcavatorFuelCollected() => ExcavatorFuelCollected.Invoke();
    public static void OnExcavatorFuelConsumed() => ExcavatorFuelConsumed?.Invoke();

    //Score
    public static event UnityAction WallScored;
    public static void OnWallScored() => WallScored?.Invoke();
    
    //Collisions
    public static event UnityAction ObstacleHitted;
    public static void OnObstacleHitted() => ObstacleHitted?.Invoke();
}

public class PriorityActionQueue
{
    private SortedList<int, Action> _queue = new SortedList<int, Action>();

    public void Enqueue(int priority, Action method)
    {
        if (!_queue.ContainsKey(priority))
        {
            _queue.Add(priority, method);
        }
    }

    public void Invoke()
    {
        foreach(Action action in _queue.Values)
        {
            action();
        }
    }

    public void Dequeue(int index)
    {
        _queue.Remove(index);
    }

    public void Clear()
    {
        _queue.Clear();
    }
}
