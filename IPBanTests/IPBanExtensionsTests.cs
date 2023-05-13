﻿/*
MIT License

Copyright (c) 2012-present Digital Ruby, LLC - https://www.digitalruby.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;

using DigitalRuby.IPBanCore;

using NUnit.Framework;

namespace DigitalRuby.IPBanTests
{
    [TestFixture]
    public class IPBanExtensionTests
    {
        [Test]
        public void TestNetworkGetAllIPAddresses()
        {
            var ips = NetworkUtility.GetSortedIPAddresses();
            Assert.IsTrue(ips.Any());
        }

        [Test]
        public void TestNetworkGetPriorityIPAddresses()
        {
            var ips = NetworkUtility.GetIPAddressesByPriority();
            Assert.IsTrue(ips.Any());

            var sortedIps = NetworkUtility.GetSortedIPAddresses(new KeyValuePair<string, int>[]
            {
                new("1999:0db8:85a3:0000:0000:8a2e:0370:7334", 0),
                new("127.0.0.1", 0),
                new("10.0.0.1", 0),
                new("44.44.44.44", 0),
                new("2003:0db8:85a3:0000:0000:8a2e:0370:7334", 0)
            });
            Assert.IsTrue(sortedIps.Any());

            Assert.AreEqual("44.44.44.44", sortedIps.First().ToString());

            sortedIps = NetworkUtility.GetSortedIPAddresses(new KeyValuePair<string, int>[]
            {
                new("1999:0db8:85a3:0000:0000:8a2e:0370:7334", 1),
                new("127.0.0.1", 2),
                new("10.0.0.1", 3),
                new("44.44.44.44", 4),
                new("2003:0db8:85a3:0000:0000:8a2e:0370:7334", 5),
                new("215.4.5.6", 10),
            });
            Assert.IsTrue(sortedIps.Any());

            Assert.AreEqual("215.4.5.6", sortedIps.First().ToString());
        }

        [Test]
        public void TestNetworkGetPriorityIPAddressesPreferInternal()
        {
            var ips = new KeyValuePair<string, int>[]
            {
                new("2001:0:2851:782c:5c:2dd:fa69:7000", 0),
                new("10.0.0.1", 0),
                new("44.44.44.44", 0),
                new("2003:0db8:85a3:0000:0000:8a2e:0370:7334", 0),
            };
            var sortedIps = NetworkUtility.GetSortedIPAddresses(ips, true);
            Assert.That(sortedIps.First().ToString(), Is.EqualTo("10.0.0.1"));
        }

        [TestCase("0.0.0.0-0.255.255.255,1.0.0.0-1.0.0.255",
        "1.0.1.0-9.255.255.255,11.0.0.0-100.63.255.255,100.128.0.0-126.255.255.255,128.0.0.0-169.253.255.255,169.255.0.0-172.15.255.255,172.32.0.0-191.255.255.255,192.0.1.0-192.0.1.255,192.0.3.0-192.88.98.255,192.88.100.0-192.167.255.255,192.169.0.0-198.17.255.255,198.20.0.0-198.51.99.255,198.51.101.0-203.0.112.255,203.0.114.0-223.255.255.255")]
            [TestCase("1.0.0.0/24,1.0.4.0/22,1.1.1.0/24,1.2.3.0/24",
        "1.0.1.0-1.0.3.255,1.0.8.0-1.1.0.255,1.1.2.0-1.2.2.255,1.2.4.0-9.255.255.255,11.0.0.0-100.63.255.255,100.128.0.0-126.255.255.255,128.0.0.0-169.253.255.255,169.255.0.0-172.15.255.255,172.32.0.0-191.255.255.255,192.0.1.0-192.0.1.255,192.0.3.0-192.88.98.255,192.88.100.0-192.167.255.255,192.169.0.0-198.17.255.255,198.20.0.0-198.51.99.255,198.51.101.0-203.0.112.255,203.0.114.0-223.255.255.255")]
            [TestCase("50.50.50.50-60.60.60.60, 90.90.90.90-100.100.100.101",
        "1.0.0.0-9.255.255.255,11.0.0.0-50.50.50.49,60.60.60.61-90.90.90.89,100.128.0.0-126.255.255.255,128.0.0.0-169.253.255.255,169.255.0.0-172.15.255.255,172.32.0.0-191.255.255.255,192.0.1.0-192.0.1.255,192.0.3.0-192.88.98.255,192.88.100.0-192.167.255.255,192.169.0.0-198.17.255.255,198.20.0.0-198.51.99.255,198.51.101.0-203.0.112.255,203.0.114.0-223.255.255.255")]
            [TestCase("125.253.93.224-125.253.93.255,128.0.0.1",
        "1.0.0.0-9.255.255.255,11.0.0.0-100.63.255.255,100.128.0.0-125.253.93.223,125.253.94.0-126.255.255.255,128.0.0.0,128.0.0.2-169.253.255.255,169.255.0.0-172.15.255.255,172.32.0.0-191.255.255.255,192.0.1.0-192.0.1.255,192.0.3.0-192.88.98.255,192.88.100.0-192.167.255.255,192.169.0.0-198.17.255.255,198.20.0.0-198.51.99.255,198.51.101.0-203.0.112.255,203.0.114.0-223.255.255.255")]
            [TestCase("2.16.14.0/24,2.16.33.76,2.16.37.0/24,2.16.48.0/24,2.16.56.0/22,2.16.74.0/23,2.16.109.0/24,2.16.114.0/23,2.16.137.0/24,2.16.162.0/24,2.16.167.0/24,2.16.170.0/24,2.17.20.0/22,2.17.38.0/24,2.17.44.0/24,2.17.113.0/24,2.17.115.0/24,2.17.148.0/22,2.17.208.76/30,2.17.208.84/30,2.17.208.92/30,2.17.208.100/30,2.17.208.108/30,2.17.208.116/30,2.17.208.124/30,2.17.208.132/31,2.17.208.140/31,2.17.208.143,2.17.208.148/30,2.17.208.157,2.17.208.164/31,2.17.208.167,2.17.208.172/31,2.17.208.180/30,2.17.208.188/30,2.17.208.196/30,2.17.208.204/30,2.17.208.212/30,2.17.208.220,2.17.210.4/30,2.17.210.12/30,2.17.210.20/30,2.17.210.28/30,2.17.210.36/30,2.17.210.44/30,2.17.210.52/30,2.17.210.60/30,2.17.210.68/30,2.17.210.76,2.17.210.84/30,2.17.210.92/30,2.17.210.100/30,2.17.210.108/30,2.17.210.116/30,2.17.210.124/30,2.17.210.132/31,2.18.16.0/23,2.18.42.0/24,2.18.66.0/24,2.18.80.0/21,2.18.165.0/24,2.18.167.0/24,2.18.248.4/30,2.18.248.12/30,2.18.248.20/30,2.18.248.28/30,2.18.248.36/30,2.18.248.44/30,2.18.248.52/30,2.18.248.60/31,2.18.248.78/31,2.18.248.86/31,2.18.248.94/31,2.18.248.102/31,2.18.248.110/31,2.18.248.118/31,2.18.248.126/31,2.18.248.134/31,2.18.248.142/31,2.19.52.0/24,2.19.56.0/21,2.19.128.0/20,2.19.144.0/20,2.19.186.0/24,2.19.188.0/22,2.19.242.0/24,2.20.32.0/22,2.20.36.0/22,2.20.60.0/22,2.20.70.0/24,2.20.92.0/22,2.21.9.0/24,2.21.67.0/24,2.21.75.0/24,2.21.134.0/23,2.21.188.0/22,2.22.4.0/22,2.22.12.0/22,2.22.30.0/24,2.22.44.0/22,2.22.51.0/24,2.22.64.0/22,2.22.72.0/22,2.22.96.0/20,2.22.121.4/30,2.22.121.70/31,2.22.121.78/31,2.22.121.86/31,2.22.121.94/31,2.22.121.102/31,2.22.121.110/31,2.22.121.118/31,2.22.121.126/31,2.22.121.132/30,2.22.121.140/30,2.22.121.148/30,2.22.121.156/30,2.22.121.164/30,2.22.121.172/30,2.22.121.180/30,2.22.121.188/30,2.22.121.196/30,2.22.121.204/30,2.22.121.212/30,2.22.121.220/30,2.22.121.228/30,2.22.121.236,2.22.128.0/20,2.22.146.0/24,2.22.225.0/24,2.22.227.0/24,2.22.246.0/23,2.23.5.0/24,2.23.85.0/24,2.23.88.0/24,2.23.90.0/23,2.23.177.0/24,2.23.192.0/19,2.24.0.0/13",
        "1.0.0.0-2.16.13.255,2.16.15.0-2.16.33.75,2.16.33.77-2.16.36.255,2.16.38.0-2.16.47.255,2.16.49.0-2.16.55.255,2.16.60.0-2.16.73.255,2.16.76.0-2.16.108.255,2.16.110.0-2.16.113.255,2.16.116.0-2.16.136.255,2.16.138.0-2.16.161.255,2.16.163.0-2.16.166.255,2.16.168.0-2.16.169.255,2.16.171.0-2.17.19.255,2.17.24.0-2.17.37.255,2.17.39.0-2.17.43.255,2.17.45.0-2.17.112.255,2.17.114.0-2.17.114.255,2.17.116.0-2.17.147.255,2.17.152.0-2.17.208.75,2.17.208.80-2.17.208.83,2.17.208.88-2.17.208.91,2.17.208.96-2.17.208.99,2.17.208.104-2.17.208.107,2.17.208.112-2.17.208.115,2.17.208.120-2.17.208.123,2.17.208.128-2.17.208.131,2.17.208.134-2.17.208.139,2.17.208.142,2.17.208.144-2.17.208.147,2.17.208.152-2.17.208.156,2.17.208.158-2.17.208.163,2.17.208.166,2.17.208.168-2.17.208.171,2.17.208.174-2.17.208.179,2.17.208.184-2.17.208.187,2.17.208.192-2.17.208.195,2.17.208.200-2.17.208.203,2.17.208.208-2.17.208.211,2.17.208.216-2.17.208.219,2.17.208.221-2.17.210.3,2.17.210.8-2.17.210.11,2.17.210.16-2.17.210.19,2.17.210.24-2.17.210.27,2.17.210.32-2.17.210.35,2.17.210.40-2.17.210.43,2.17.210.48-2.17.210.51,2.17.210.56-2.17.210.59,2.17.210.64-2.17.210.67,2.17.210.72-2.17.210.75,2.17.210.77-2.17.210.83,2.17.210.88-2.17.210.91,2.17.210.96-2.17.210.99,2.17.210.104-2.17.210.107,2.17.210.112-2.17.210.115,2.17.210.120-2.17.210.123,2.17.210.128-2.17.210.131,2.17.210.134-2.18.15.255,2.18.18.0-2.18.41.255,2.18.43.0-2.18.65.255,2.18.67.0-2.18.79.255,2.18.88.0-2.18.164.255,2.18.166.0-2.18.166.255,2.18.168.0-2.18.248.3,2.18.248.8-2.18.248.11,2.18.248.16-2.18.248.19,2.18.248.24-2.18.248.27,2.18.248.32-2.18.248.35,2.18.248.40-2.18.248.43,2.18.248.48-2.18.248.51,2.18.248.56-2.18.248.59,2.18.248.62-2.18.248.77,2.18.248.80-2.18.248.85,2.18.248.88-2.18.248.93,2.18.248.96-2.18.248.101,2.18.248.104-2.18.248.109,2.18.248.112-2.18.248.117,2.18.248.120-2.18.248.125,2.18.248.128-2.18.248.133,2.18.248.136-2.18.248.141,2.18.248.144-2.19.51.255,2.19.53.0-2.19.55.255,2.19.64.0-2.19.127.255,2.19.160.0-2.19.185.255,2.19.187.0-2.19.187.255,2.19.192.0-2.19.241.255,2.19.243.0-2.20.31.255,2.20.40.0-2.20.59.255,2.20.64.0-2.20.69.255,2.20.71.0-2.20.91.255,2.20.96.0-2.21.8.255,2.21.10.0-2.21.66.255,2.21.68.0-2.21.74.255,2.21.76.0-2.21.133.255,2.21.136.0-2.21.187.255,2.21.192.0-2.22.3.255,2.22.8.0-2.22.11.255,2.22.16.0-2.22.29.255,2.22.31.0-2.22.43.255,2.22.48.0-2.22.50.255,2.22.52.0-2.22.63.255,2.22.68.0-2.22.71.255,2.22.76.0-2.22.95.255,2.22.112.0-2.22.121.3,2.22.121.8-2.22.121.69,2.22.121.72-2.22.121.77,2.22.121.80-2.22.121.85,2.22.121.88-2.22.121.93,2.22.121.96-2.22.121.101,2.22.121.104-2.22.121.109,2.22.121.112-2.22.121.117,2.22.121.120-2.22.121.125,2.22.121.128-2.22.121.131,2.22.121.136-2.22.121.139,2.22.121.144-2.22.121.147,2.22.121.152-2.22.121.155,2.22.121.160-2.22.121.163,2.22.121.168-2.22.121.171,2.22.121.176-2.22.121.179,2.22.121.184-2.22.121.187,2.22.121.192-2.22.121.195,2.22.121.200-2.22.121.203,2.22.121.208-2.22.121.211,2.22.121.216-2.22.121.219,2.22.121.224-2.22.121.227,2.22.121.232-2.22.121.235,2.22.121.237-2.22.127.255,2.22.144.0-2.22.145.255,2.22.147.0-2.22.224.255,2.22.226.0-2.22.226.255,2.22.228.0-2.22.245.255,2.22.248.0-2.23.4.255,2.23.6.0-2.23.84.255,2.23.86.0-2.23.87.255,2.23.89.0-2.23.89.255,2.23.92.0-2.23.176.255,2.23.178.0-2.23.191.255,2.23.224.0-2.23.255.255,2.32.0.0-9.255.255.255,11.0.0.0-100.63.255.255,100.128.0.0-126.255.255.255,128.0.0.0-169.253.255.255,169.255.0.0-172.15.255.255,172.32.0.0-191.255.255.255,192.0.1.0-192.0.1.255,192.0.3.0-192.88.98.255,192.88.100.0-192.167.255.255,192.169.0.0-198.17.255.255,198.20.0.0-198.51.99.255,198.51.101.0-203.0.112.255,203.0.114.0-223.255.255.255")]
            [TestCase("40.82.124.0/22,40.82.248.0/21,40.83.128.0/17,40.85.144.0/20,40.86.0.0/17",
        "1.0.0.0-9.255.255.255,11.0.0.0-40.82.123.255,40.82.128.0-40.82.247.255,40.83.0.0-40.83.127.255,40.84.0.0-40.85.143.255,40.85.160.0-40.85.255.255,40.86.128.0-100.63.255.255,100.128.0.0-126.255.255.255,128.0.0.0-169.253.255.255,169.255.0.0-172.15.255.255,172.32.0.0-191.255.255.255,192.0.1.0-192.0.1.255,192.0.3.0-192.88.98.255,192.88.100.0-192.167.255.255,192.169.0.0-198.17.255.255,198.20.0.0-198.51.99.255,198.51.101.0-203.0.112.255,203.0.114.0-223.255.255.255")]
            [TestCase("2a12:fa80::/29,2a12:fb00::/29,2a12:fb80::/29,2a12:fc80::/29,2a12:fd80::/29,2a12:fe80::/29",
        "2000::-2000:ffff:ffff:ffff:ffff:ffff:ffff:ffff,2001:1::-2001:db7:ffff:ffff:ffff:ffff:ffff:ffff,2001:db9::-2001:ffff:ffff:ffff:ffff:ffff:ffff:ffff,2003::-2a12:fa7f:ffff:ffff:ffff:ffff:ffff:ffff,2a12:fa88::-2a12:faff:ffff:ffff:ffff:ffff:ffff:ffff,2a12:fb08::-2a12:fb7f:ffff:ffff:ffff:ffff:ffff:ffff,2a12:fb88::-2a12:fc7f:ffff:ffff:ffff:ffff:ffff:ffff,2a12:fc88::-2a12:fd7f:ffff:ffff:ffff:ffff:ffff:ffff,2a12:fd88::-2a12:fe7f:ffff:ffff:ffff:ffff:ffff:ffff,2a12:fe88::-3fff:ffff:ffff:ffff:ffff:ffff:ffff:ffff")]
        public void TestInvertRanges(string ranges, string invertedRanges)
        {
            IEnumerable<IPAddressRange> rangesObj = ranges.Split(',').Select(r => IPAddressRange.Parse(r)).ToArray();
            IEnumerable<IPAddressRange> invertedRangesObj = rangesObj.Invert().ToArray();
            string invertedRangesObjString = string.Join(',', invertedRangesObj.Select(r => r.ToString('-')));
            Assert.AreEqual(invertedRanges, invertedRangesObjString);
        }
    }
}
