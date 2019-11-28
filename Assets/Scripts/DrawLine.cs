using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;
using System.Threading;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using UnityEngine.Networking;

public class myStopWatch
{
    public Stopwatch clock = new Stopwatch();

};
public class DrawLine : MonoBehaviour
{
    public myStopWatch time = new myStopWatch();
    public GameObject linePrefab;
    public GameObject currentLine;
    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;

    public Vector2 currentPosition;
    public List<Vector2> points;
    public int idx;
    public bool isTipDown;
    public int tempx;
    public int tempy;
    void loadvideo(string textfilepath)
    {

        string json;
        using (StreamReader r = new StreamReader("C:\\Users\\DC\\Desktop\\sample data\\video.json"))
        {
            json = r.ReadToEnd();
            mydatalist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<mydata>>(json);
            //mydatalist = JsonUtility.FromJson<List<mydata>>(json);
        }
    }
    public class mydata
    {
        public float x;
        public float y;
        public double time;
    }
    public List<mydata> mydatalist = new List<mydata>();
    //For Audio file
    public WWW www;
    List<string> songs = new List<string>();
    public AudioSource source;


    public List<Vector2> fingerPositions;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        UnityEngine.Debug.Log("Start Called");
       
        isTipDown = true;
        loadvideo("abc");
        StartSong("C:\\Users\\DC\\Desktop\\sample data\\audio.wav");
        tempx = 0;
        tempy = 0;
        UnityEngine.Debug.Log(tempx + "+" + mydatalist.Count);

        points.Add(new Vector2(10, 10));
        points.Add(new Vector2(10, 20));
        points.Add(new Vector2(10, 30));
        points.Add(new Vector2(10, 40));
        points.Add(new Vector2(10, 50));
        //currentPosition = new Vector2(mydatalist[tempx].x, mydatalist[tempx].y);
        time.clock.Start();

        //UpdateLine(currentPosition);
    }
    public IEnumerator StartSong(string path)
    {
        www = new WWW(path);
        if (www.error != null)
        {
            UnityEngine.Debug.Log(www.error);
        }
        else
        {
            source.clip = www.GetAudioClip();
            while (source.clip.loadState != AudioDataLoadState.Loaded)
                yield return new WaitForSeconds(0.1f);
            source.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Debug.Log(tempx + "+" + mydatalist.Count);

        if (tempx < mydatalist.Count - 1)
        {
            UnityEngine.Debug.Log(mydatalist[tempx].time + "+"+time.clock.ElapsedMilliseconds);
            if (((long)mydatalist[tempx].time - time.clock.ElapsedMilliseconds) <= 1 || ((long)mydatalist[tempx].time - time.clock.ElapsedMilliseconds) <= -1)
            {
                currentPosition = new Vector2(mydatalist[tempx].x, mydatalist[tempx].y);
                Console.WriteLine("Line Drawn");
                if (isTipDown) { CreateLine(new Vector2(1280 / 2 + currentPosition.x, 550 / 2 + currentPosition.y));
                    //Debug.Log(currentPosition); 
                }
                isTipDown = false;
                lineDraw(currentPosition);
                tempx++;
            }
            //Debug.Log(mydatalist[tempx].x);
            //Debug.Log(mydatalist[tempx].y);
            
        }
        
    }
    void lineDraw(Vector2 currentPosition)
    {
        //Vector2 tempFingerPos = Camera.main.ScreenToWorldPoint(currentPosition);
        Vector2 tempFingerPos = (currentPosition);
        //Debug.Log(tempFingerPos);

        if (Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]) > .001f)
        {
            UpdateLine(tempFingerPos);
        }
    }
    void CreateLine(Vector2 position)
    {
        Vector3 temp = new Vector3(position.x, position.y, 0);
        //currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        currentLine = Instantiate(linePrefab, temp, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        fingerPositions.Clear();
        //fingerPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //fingerPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        fingerPositions.Add(Camera.main.ScreenToWorldPoint(position));
        fingerPositions.Add(Camera.main.ScreenToWorldPoint(position));
        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);
        edgeCollider.points = fingerPositions.ToArray();
    }

    void UpdateLine(Vector2 newFingerPos)
    {
        fingerPositions.Add(newFingerPos);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);
        edgeCollider.points = fingerPositions.ToArray();
    }
}
