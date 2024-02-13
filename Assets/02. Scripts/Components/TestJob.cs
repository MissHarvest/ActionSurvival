using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct TestJob : IJob
{
    public string _path;
    public string _name;
    public string _json;


    public void Execute()
    {
        FileStream fs = new FileStream($"{_path}/{_name}.json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(_json);
        fs.Write(data, 0, data.Length);
        fs.Close();
    }
}
