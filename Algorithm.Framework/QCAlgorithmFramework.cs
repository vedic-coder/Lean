﻿/*
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
using QuantConnect.Algorithm.Framework.Execution;
using QuantConnect.Algorithm.Framework.Portfolio;
using QuantConnect.Algorithm.Framework.Risk;
using QuantConnect.Algorithm.Framework.Selection;
using QuantConnect.Algorithm.Framework.Signals;
using QuantConnect.Data;
using QuantConnect.Data.UniverseSelection;

namespace QuantConnect.Algorithm.Framework
{
    /// <summary>
    /// Algorithm framework base class that enforces a modular approach to algorithm development
    /// </summary>
    public class QCAlgorithmFramework : QCAlgorithm
    {
        /// <summary>
        /// Gets or sets the portfolio selection model.
        /// </summary>
        public IPortfolioSelectionModel PortfolioSelection { get; set; }

        /// <summary>
        /// Gets or sets the signal model
        /// </summary>
        public ISignalModel Signal { get; set; }

        /// <summary>
        /// Gets or sets the portoflio construction model
        /// </summary>
        public IPortfolioConstructionModel PortfolioConstruction { get; set; }

        /// <summary>
        /// Gets or sets the execution model
        /// </summary>
        public IExecutionModel Execution { get; set; }

        /// <summary>
        /// Gets or sets the risk management model
        /// </summary>
        public IRiskManagementModel RiskManagement { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QCAlgorithmFramework"/> class
        /// </summary>
        public QCAlgorithmFramework()
        {
            var type = GetType();
            var onDataSlice = type.GetMethod("OnData", new[] { typeof(Slice) });
            if (onDataSlice.DeclaringType != typeof(QCAlgorithmFramework))
            {
                throw new Exception("Framework algorithms can not override OnData(Slice)");
            }
            var onSecuritiesChanged = type.GetMethod("OnSecuritiesChanged", new[] { typeof(SecurityChanges) });
            if (onSecuritiesChanged.DeclaringType != typeof(QCAlgorithmFramework))
            {
                throw new Exception("Framework algorithms can not override OnSecuritiesChanged(SecurityChanges)");
            }
        }

        /// <summary>
        /// Called by setup handlers after Initialize and allows the algorithm a chance to organize
        /// the data gather in the Initialize method
        /// </summary>
        public override void PostInitialize()
        {
            CheckModels();

            foreach (var universe in PortfolioSelection.CreateUniverses(this))
            {
                AddUniverse(universe);
            }

            base.PostInitialize();
        }

        /// <summary>
        /// Event - v3.0 DATA EVENT HANDLER: (Pattern) Basic template for user to override for receiving all subscription data in a single event
        /// </summary>
        /// <code>
        /// TradeBars bars = slice.Bars;
        /// Ticks ticks = slice.Ticks;
        /// TradeBar spy = slice["SPY"];
        /// List{Tick} aaplTicks = slice["AAPL"]
        /// Quandl oil = slice["OIL"]
        /// dynamic anySymbol = slice[symbol];
        /// DataDictionary{Quandl} allQuandlData = slice.Get{Quand}
        /// Quandl oil = slice.Get{Quandl}("OIL")
        /// </code>
        /// <param name="slice">The current slice of data keyed by symbol string</param>
        public override void OnData(Slice slice)
        {
            var signals = Signal.Update(this, slice);
            var targets = PortfolioConstruction.CreateTargets(this, signals);
            Execution.Execute(this, targets);
            RiskManagement.ManageRisk(this);
        }

        /// <summary>
        /// Event fired each time the we add/remove securities from the data feed
        /// </summary>
        /// <param name="changes">Securities added and removed from the algorithm</param>
        public override void OnSecuritiesChanged(SecurityChanges changes)
        {
            Signal.OnSecuritiesChanged(this, changes);
            PortfolioConstruction.OnSecuritiesChanged(this, changes);
            Execution.OnSecuritiesChanged(this, changes);
            RiskManagement.OnSecuritiesChanged(this, changes);
        }

        /// <summary>
        /// Sets the portfolio selection model
        /// </summary>
        /// <param name="portfolioSelection">Model defining universes for the algorithm</param>
        public void SetPortfolioSelection(IPortfolioSelectionModel portfolioSelection)
        {
            PortfolioSelection = portfolioSelection;
        }

        /// <summary>
        /// Sets the signal model
        /// </summary>
        /// <param name="signal">Model defining trading signals</param>
        public void SetSignal(ISignalModel signal)
        {
            Signal = signal;
        }

        /// <summary>
        /// Sets the portfolio construction model
        /// </summary>
        /// <param name="portfolioConstruction">Model defining how to build a portoflio from signals</param>
        public void SetPortfolioConstruction(IPortfolioConstructionModel portfolioConstruction)
        {
            PortfolioConstruction = portfolioConstruction;
        }

        /// <summary>
        /// Sets the execution model
        /// </summary>
        /// <param name="execution">Model defining how to execute trades to reach a portfolio target</param>
        public void SetExecution(IExecutionModel execution)
        {
            Execution = execution;
        }

        /// <summary>
        /// Sets the risk management model
        /// </summary>
        /// <param name="riskManagement">Model defining </param>
        public void SetRiskManagement(IRiskManagementModel riskManagement)
        {
            RiskManagement = riskManagement;
        }

        private void CheckModels()
        {
            if (PortfolioSelection == null)
            {
                throw new Exception("Framework algorithms must specify a portfolio selection model using the 'PortfolioSelection' property.");
            }
            if (Signal == null)
            {
                throw new Exception("Framework algorithms must specify a signal model using the 'Signal' property.");
            }
            if (PortfolioConstruction == null)
            {
                throw new Exception("Framework algorithms must specify a portfolio construction model using the 'PortfolioConstruction' property");
            }
            if (Execution == null)
            {
                throw new Exception("Framework algorithms must specify an execution model using the 'Execution' property.");
            }
            if (RiskManagement == null)
            {
                throw new Exception("Framework algorithms must specify an risk management model using the 'RiskManagement' property.");
            }
        }
    }
}