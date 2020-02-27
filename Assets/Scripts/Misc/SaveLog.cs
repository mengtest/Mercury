using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mercury
{
    public class SaveLog : MonoBehaviour
    {
        private float _length;
        private ConcurrentQueue<string> _queue;
        private string _fileName;

        private void Awake()
        {
            DontDestroyOnLoad(this);
#if UNITY_EDITOR
            _fileName = $"{Application.dataPath}/../Logs/Unitylog.txt";
#else
            _fileName = Application.persistentDataPath + "/Unitylog.txt";
#endif
            LogToFile("\n-------Version of the runtime: " + Application.unityVersion + "-------");
            Application.logMessageReceivedThreaded += OnReceiveLogMsg;
            _queue = new ConcurrentQueue<string>();
        }

        private void OnReceiveLogMsg(string condition, string stackTrace, LogType logType)
        {
            var builder = new StringBuilder();
            builder.Append('[').Append(DateTime.UtcNow).Append("][").Append(logType).Append("]:").Append(condition);
            var list = stackTrace.Split('\n');
            foreach (var s in list)
            {
                if (s == string.Empty)
                {
                    continue;
                }

                builder.Append("\n\t").Append(s);
            }

            _queue.Enqueue(builder.ToString());
        }

        private Task _task;

        private void Update()
        {
            if (_queue.Count != 0 && ReferenceEquals(_task, null))
            {
                _task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (_queue.TryDequeue(out var msg))
                        {
                            LogToFile(msg);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    finally
                    {
                        _task = null;
                    }
                });
            }
        }

        private void LogToFile(string str, Action callback = null)
        {
            if (str == null)
            {
                return;
            }

            var writer = new StreamWriter(_fileName, true, System.Text.Encoding.UTF8);
            writer.WriteLine(str);
            writer.Close();
            callback?.Invoke();
        }
    }
}