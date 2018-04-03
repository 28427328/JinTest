using System;

using SmartQuant;

namespace OpenQuant
{
    public partial class MyScenario : Scenario
    {
        public MyScenario(Framework framework)
            : base(framework)
        {
        }

        public override void Run()
        {
			Instrument instrument1 = InstrumentManager.Instruments["rb88"];
            strategy = new MyStrategy(framework, "Backtest");
			strategy.AddInstrument(instrument1);
			
			Console.WriteLine("DataSimulator {0} {1} {2} {3}", DataSimulator.SubscribeBid, 
				DataSimulator.SubscribeAsk, 
				DataSimulator.SubscribeTrade,
				DataSimulator.SubscribeBar
				);
			
			
			
			DataSimulator.SubscribeBid = true;
			DataSimulator.SubscribeAsk = true;
			DataSimulator.SubscribeTrade = true;
			DataSimulator.SubscribeBar = false;
			Console.WriteLine("ExecutionSimulator {0} {1} {2} {3}", ExecutionSimulator.FillOnBar,
				ExecutionSimulator.FillOnTrade, ExecutionSimulator.FillOnQuote,
				ExecutionSimulator.FillLimitOnNext );
			
			ExecutionSimulator.FillOnBar = false;
			ExecutionSimulator.FillOnTrade = false;
			ExecutionSimulator.FillOnQuote = true;
			ExecutionSimulator.FillAtLimitPrice = false;
			ExecutionSimulator.FillAtLimitPrice = true;
			ExecutionSimulator.Queued = true;
		
			ExecutionSimulator.CommissionProvider.Type = CommissionType.PerShare;
			ExecutionSimulator.CommissionProvider.Commission = 1;
			
			DataSimulator.DateTime1 = new DateTime(2017, 01, 10,09,00,00);
			DataSimulator.DateTime2 = new DateTime(2017, 01, 11,09,27,00);
			
			
			BarFactory.Clear();
			BarFactory.Add(instrument1, BarType.Time, 60);//1 min
			//BarFactory.Add(instrument1, BarType.Time, 300);//3 min
			//Initialize();
			
            StartStrategy();
        }
    }
}



