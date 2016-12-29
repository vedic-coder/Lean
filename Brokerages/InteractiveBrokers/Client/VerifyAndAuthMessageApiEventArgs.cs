/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;

namespace QuantConnect.Brokerages.InteractiveBrokers.Client
{
    /// <summary>
    /// Event arguments class for the <see cref="InteractiveBrokersClient.VerifyAndAuthMessageApi"/> event
    /// </summary>
    public class VerifyAndAuthMessageApiEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public string ApiData { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string XyzChallenge { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyAndAuthMessageApiEventArgs"/> class
        /// </summary>
        public VerifyAndAuthMessageApiEventArgs(string apiData, string xyzChallenge)
        {
            ApiData = apiData;
            XyzChallenge = xyzChallenge;
        }
    }
}