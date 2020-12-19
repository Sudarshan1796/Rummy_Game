using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Rummy.Network
{
    public enum InternetStatus : short
    {
        None, // Internet samples are still in processing state
        NotConnected,
        Excellent,
        Good,
        average,
        Bad
    }

    public class InternetConnectionScanner : MonoBehaviour
    {
        [SerializeField] private string ipAddress = "8.8.8.8";
        [SerializeField] private float minPingInterval = 0.20f;
        [SerializeField] private float pingTimeout = 1;

        public InternetStatus internetStatus;
        public float averagePing;
        public bool isInternetFluctuating = false;

        private float timer = 0;
        private int fluctuationSampleTrueCounter;
        private int fluctuationSampleFalseCounter;
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
                CheckForInternetfluctuation(internetStatus);
            }
            StartCoroutine(Ping());
            StartCoroutine(PingSampleAnalyzer());
        }

        /// <summary>
        /// At most this ping request can consume approximately 1.15MB/Hour (if minPingInterval == 0.20ms)
        /// 64 or 32 bytes/Ping request (we will consider worst case scenario i.e 64bytes/ping), we are calling each ping request in an interval of "minPingInterval" its default value is 0.2ms => 200ms.
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
                Ping ping = new Ping(ipAddress);
                timer = pingTimeout;
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
                }

                while (timer > (pingTimeout - minPingInterval))
                {
                    yield return null;
                    timer -= Time.unscaledDeltaTime;
                }

                ping.DestroyPing();
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
                    else if (averagePing <= 150)
                    {
                        internetStatus = InternetStatus.Excellent;
                    }
                    else if (averagePing <= 300)
                    {
                        internetStatus = InternetStatus.Good;
                    }
                    else if (averagePing <= 500)
                    {
                        internetStatus = InternetStatus.average;
                    }
                    else
                    {
                        internetStatus = InternetStatus.Bad;
                    }
                }
                CheckForInternetfluctuation(internetStatus);
                yield return waitForSecondsRealtime;
            }
        }

        private void CheckForInternetfluctuation(InternetStatus status)
        {
            if(status == InternetStatus.Excellent || status == InternetStatus.Good || status == InternetStatus.average)
            {
                CheckIfFluctuating(true);
            }
            else if(status == InternetStatus.Bad || status == InternetStatus.NotConnected)
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
