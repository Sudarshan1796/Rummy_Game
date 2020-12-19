using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Rummy.Network
{
    public enum InternetStatus : short
    {
        Excellent,
        Good,
        average,
        Bad,
        NotConnected,
        None, // Internet samples are still in processing state
    }

    public class InternetConnectionScanner : MonoBehaviour
    {
        [SerializeField] private string ipAddress = "8.8.8.8";
        [SerializeField] private float minPingIntervalInSeconds = 0.20f;
        [SerializeField] private float averageExpectedPingTimeInMiliSeconds = 300;
        [SerializeField] private float pingTimeoutInSeconds = 1;

        public InternetStatus internetStatus = InternetStatus.None;
        public float averagePing;
        public bool isInternetFluctuating = false;

        private float timer = 0;
        private float fluctuationSampleTrueCounter;
        private float fluctuationSampleFalseCounter;
        private readonly int maxPingSampleCount = 5;
        private readonly int maxFluctuationSampleCount = 10;
        private readonly Queue<int> pingSamples = new Queue<int>();
        private readonly Queue<bool> networkFluctuationSamples = new Queue<bool>();
        private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(0.1f);

        private static InternetConnectionScanner instance;
        internal static InternetConnectionScanner GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<InternetConnectionScanner>();
                }
                return instance;
            }
        }

        internal void SetIp(string ip)
        {
            ipAddress = ip;
        }

        internal string GetIp()
        {
            return ipAddress;
        }

        private void Start()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                internetStatus = InternetStatus.NotConnected;
            }
            StartCoroutine(Ping());
            StartCoroutine(PingSampleAnalyzer());
        }

        /// <summary>
        /// At most this ping request can consume approximately 1.15MB/Hour (if minPingIntervalInSeconds == 0.20ms)
        /// 64 or 32 bytes/Ping request (we will consider worst case scenario i.e 64bytes/ping), we are calling each ping request approximately in an interval of "minPingIntervalInSeconds" its default value is 0.2ms => 200ms.
        /// So in a second atmost 5 times we will ping server or atleast once becuase for any ping we will wait for atmost 1sec (pingTimeoutInSeconds = 1sec (default)).
        /// 64 bytes * 5 times pers second = 320 bytes/Second.
        /// 320 bytes * 60 seconds = 19200 bytes/Minutes.
        /// 19200 bytes * 60 minutes = 1152000bytes/Hour.
        /// 1152000 bytes/Hour => 1.152MB/Hour.
        ///
        /// High ping value can be because of 2 reasons
        /// Either your internet is very slow or you are trying to communicate to a server which is too far from your location or both.
        /// But in both of the above cases outcome will be same that is packet loss if internet is slow / or very high RTT if you have very good internet but server is too far.
        /// </summary>
        private IEnumerator Ping()
        {
            while (true)
            {
                if (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    Ping ping = new Ping(ipAddress);
                    timer = pingTimeoutInSeconds;
                    while (!ping.isDone && timer > 0)
                    {
                        yield return null;
                        timer -= Time.unscaledDeltaTime;
                    }

                    if (ping.isDone && ping.time != -1)
                    {
                        if (pingSamples.Count == maxPingSampleCount)
                        {
                            pingSamples.Dequeue();
                            pingSamples.Enqueue(ping.time);
                        }
                        else
                        {
                            pingSamples.Enqueue(ping.time);
                        }
                        CheckForInternetfluctuation(ping.time);
                    }
                    else
                    {
                        if (pingSamples.Count == maxPingSampleCount)
                        {
                            pingSamples.Dequeue();
                            pingSamples.Enqueue(1000);
                        }
                        else
                        {
                            pingSamples.Enqueue(1000);
                        }
                        CheckForInternetfluctuation(1000);
                    }

                    while (timer > (pingTimeoutInSeconds - minPingIntervalInSeconds))
                    {
                        yield return null;
                        timer -= Time.unscaledDeltaTime;
                    }

                    ping.DestroyPing();
                }
                else
                {
                    if (pingSamples.Count == maxPingSampleCount)
                    {
                        pingSamples.Dequeue();
                        pingSamples.Enqueue(1000);
                    }
                    else
                    {
                        pingSamples.Enqueue(1000);
                    }
                    CheckForInternetfluctuation(1000);

                    timer = pingTimeoutInSeconds;
                    while (timer > (pingTimeoutInSeconds - minPingIntervalInSeconds))
                    {
                        yield return null;
                        timer -= Time.unscaledDeltaTime;
                    }
                }
            }
        }

        private IEnumerator PingSampleAnalyzer()
        {
            while (true)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    internetStatus = InternetStatus.NotConnected;
                }
                else if (pingSamples.Count == maxPingSampleCount)
                {
                    averagePing = 0;
                    foreach (var val in pingSamples)
                    {
                        averagePing += val;
                    }
                    averagePing /= maxPingSampleCount;

                    if (averagePing == 1000)
                    {
                        internetStatus = InternetStatus.NotConnected;
                    }
                    else if (averagePing <= 100)
                    {
                        internetStatus = InternetStatus.Excellent;
                    }
                    else if (averagePing <= 200)
                    {
                        internetStatus = InternetStatus.Good;
                    }
                    else if (averagePing <= 400)
                    {
                        internetStatus = InternetStatus.average;
                    }
                    else
                    {
                        internetStatus = InternetStatus.Bad;
                    }
                }
                yield return waitForSecondsRealtime;
            }
        }

        private void CheckForInternetfluctuation(int pingTime)
        {
            if(pingTime <= averageExpectedPingTimeInMiliSeconds)
            {
                CheckIfFluctuating(true);
            }
            else
            {
                CheckIfFluctuating(false);
            }

            void CheckIfFluctuating(bool isPingTimeLooksGood)
            {
                if (networkFluctuationSamples.Count == maxFluctuationSampleCount)
                {
                    networkFluctuationSamples.Dequeue();
                    networkFluctuationSamples.Enqueue(isPingTimeLooksGood);
                }
                else
                {
                    networkFluctuationSamples.Enqueue(isPingTimeLooksGood);
                }

                fluctuationSampleTrueCounter = fluctuationSampleFalseCounter = 0;
                foreach(var sample in networkFluctuationSamples)
                {
                    if(sample)
                    {
                        fluctuationSampleTrueCounter++;
                    }
                    else
                    {
                        fluctuationSampleFalseCounter++;
                    }
                }

                if(fluctuationSampleTrueCounter > 0 && fluctuationSampleFalseCounter == 0)
                {
                    isInternetFluctuating = false;
                }
                else if (fluctuationSampleFalseCounter > 0 && fluctuationSampleTrueCounter == 0)
                {
                    isInternetFluctuating = true;
                }
                else if(fluctuationSampleTrueCounter / fluctuationSampleFalseCounter < 2.3f)
                {
                    isInternetFluctuating = true;
                }
                else
                {
                    isInternetFluctuating = false;
                }
            }
        }
    }
}
