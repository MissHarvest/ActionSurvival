using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobTestor : MonoBehaviour
{
    //public IEnumerator Start()
    //{
    //    Stopwatch watch = new Stopwatch();
    //    watch.Start();
    //    /*
    //    int count = 10000;
    //    NativeArray<float> a = new NativeArray<float>(10000, Allocator.TempJob);
    //    NativeArray<float> b = new NativeArray<float>(10000, Allocator.TempJob);
    //    NativeArray<float> result = new NativeArray<float>(10000, Allocator.TempJob);

    //    for(int i = 0; i < a.Length; ++i)
    //    {
    //        a[i] = i * 0.1f;
    //        b[i] = i * 1.1f;
    //    }

    //    MyParrelJob job = new MyParrelJob();
    //    job.a = a;
    //    job.b = b;
    //    job.result = result;

    //    var handle = job.Schedule(count, 10);

    //    while (!handle.IsCompleted)
    //        yield return null;

    //    handle.Complete();
    //    */

    //    //for (int i = 0; i < 10000; ++i)
    //    //{
    //    //    TestJob job = new TestJob(i * 0.1f, i * 1.1f);
    //    //    var handle = job.Schedule();
    //    //    while (!handle.IsCompleted)
    //    //        yield return null;

    //    //    handle.Complete();
    //    //    UnityEngine.Debug.Log($"[Job] {i} Clear");
    //    //}

    //    watch.Stop();
    //    UnityEngine.Debug.Log($"[Complete] {watch.ElapsedMilliseconds} ms");
    //}

    //private void TestNormalJob()
    //{
    //    
    //}
}
