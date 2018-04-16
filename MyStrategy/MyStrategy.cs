using System;

using SmartQuant;
using SmartQuant.Indicators;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;

namespace OpenQuant
{
	public class MyStrategy : InstrumentStrategy
	{
		private BarSeries bars1min; 
		private BarSeries bars5min;
		
		private Group bars1MinGroup;
		private Group bars5MinGroup;
		private Group volumeGroup;
		private Group HLGroup;
		
		private Group fillGroup;
		private Group equityGroup;
		
		private bool firstCal = true;
		private int HLbarcount = 0;
		public TimeSeries ReversePoint;
		private DateTime LLDateTime;
		private DateTime HHDateTime;
		private uint HHLastPeriodCnt = 0;
		private uint LLLastPeriodCnt = 0;
		private double barOpen = 0;
		private double barClose = 0;
		private double barLow = 0;
		private double barHigh = 0;
		private double HH = 0;
		private double HL = 0;
		private double LL = 0;
		private double LH = 0;
		
		
		private Group sma120Group;
		
		public MyStrategy(Framework framework, string name)
			: base(framework, name)
		{
		}

		protected override void OnStrategyStart()
		{
			bars1min = new BarSeries(); 
			bars5min = new BarSeries();
			AddGroups();
		}

		protected override void OnBar(Instrument instrument, Bar bar)
		{
			//Console.WriteLine("OnBar enter121---trade.DateTime={0:yyyyMMdd HH:mm:ss.fff}, bar.Open = {1}",bar.OpenDateTime,bar.Open);
			Bars.Add(bar);
			if (bar.Size == 60) 
			{
				Log(bar, bars1MinGroup);//draw K line
				//Console.WriteLine("OnBar First Done ");
				//Log(bar.Volume, bars1MinGroup);//draw K line
				//Log(new TimeSeriesItem(bar.DateTime, bar.Volume), volumeGroup);
				if (   (bar.Close > 0) 
					&& (bar.High > 0) 
					&& (bar.Low > 0) 
					&& (bar.Open > 0)
					)
				{
					//Console.WriteLine("OnBar First Done ");
					HLbarcount++;
					bars1min.Add(bar);
					
					barOpen = bar.Open;
					barClose = bar.Close;
					barHigh = bar.High;
					barLow = bar.Low;
					if((HLbarcount == 1)&&(firstCal == true))
					{
						//Console.WriteLine("OnBar enter2");

						LL = barLow;
						LH = barHigh;

						HH = barHigh;
						HL = barLow;
						
						LLDateTime = bar.DateTime;
						HHDateTime = bar.DateTime;
						HHLastPeriodCnt = 0;
						LLLastPeriodCnt = 0;
						
						//Direction = 0;	
						//Console.WriteLine("OnBar enter12");
						//ReversePoint.Add(bar.DateTime, bar.Open);
						//Log(bar.Open, HLGroup, bar.DateTime);
						//Console.WriteLine("OnBar ReversePoint DateTime={0:yyyyMMdd HH:mm:ss.fff}, LL = {1}",bar.DateTime,bar.Open);
						//Log(bar.DateTime,bar.Open, HLGroup);
						//Log("start", HLGroup,bar.DateTime);
						//Console.WriteLine("OnBar First Done ");
						//Console.WriteLine("OnBar enter121");
					} 
					else if(firstCal == true)
					{
						//Console.WriteLine("OnBar First112 start ");
						if(barClose >barOpen )
						{
							if(barHigh>LH)
							{
								HH = barHigh;
								HL = barLow;
								HHDateTime = bar.DateTime;
								HHLastPeriodCnt++;
								if(HHLastPeriodCnt>=3)
								{
									//Console.WriteLine("OnBar First111 Done ");
									firstCal = false;
									//ReversePoint.Add(LLDateTime, LL);
									Log(LL, HLGroup,LLDateTime);
									Console.WriteLine("OnBar LL DateTime={0:yyyyMMdd HH:mm:ss.fff}, LL = {1}",LLDateTime,LL);
									LLLastPeriodCnt = 0;
									//Console.WriteLine("OnBar First112 Done ");
								
								
								}
							}
						}
						else if(barClose <barOpen)
						{
							if(barLow<HL)
							{
								LL = barLow;
								LH = barHigh;
								LLDateTime = bar.DateTime;
								LLLastPeriodCnt++;	
								if(LLLastPeriodCnt>=3)
								{
									firstCal = false;
									//ReversePoint.Add(HHDateTime, HH);
									Log(HH, HLGroup,HHDateTime);
									Console.WriteLine("OnBar ReversePoint DateTime={0:yyyyMMdd HH:mm:ss.fff}, HH = {1}",HHDateTime,HH);
									HHLastPeriodCnt = 0;
									Console.WriteLine("OnBar First Done ");
								}
							}
						}
					}				
					else if(barClose>barOpen)
					{
						if(LLLastPeriodCnt>=3)
						{
							if(barLow < LL)
							{
								LL = barLow;
								LLDateTime = bar.DateTime;
							}
							if(barHigh>LH)
							{
								LH = barHigh;
								HHLastPeriodCnt++;
								//Console.WriteLine("OnBar > LH  DateTime={0:yyyyMMdd HH:mm:ss.fff}, HHLastPeriodCnt = {1}, LH = {2}",bar.DateTime,HHLastPeriodCnt,LH);
								if(HHLastPeriodCnt>=3)
								{
									HHDateTime = bar.DateTime;
									HH = barHigh;
									HL = barLow;
									//Console.WriteLine("OnBar test raise up HHLastPeriodCnt >3 {0} LLLastPeriodCnt{1}", HHLastPeriodCnt, LLLastPeriodCnt);
									
									//Console.WriteLine("OnBar test raise up mark ll point");
									//ReversePoint.Add(LLDateTime, LL); 
									Log(LL, HLGroup,LLDateTime);
									//Console.WriteLine("OnBar LL DateTime={0:yyyyMMdd HH:mm:ss.fff}, LL = {1}",LLDateTime,LL);
									LLLastPeriodCnt = 0;
									
								}
							}
						}
						else if(barHigh>HH)	
						{
							HH= barHigh;
							HL = barLow;
							HHDateTime = bar.DateTime;
							HHLastPeriodCnt++;
							//Console.WriteLine("OnBar test raise up HHLastPeriodCnt {0}", HHLastPeriodCnt);
							if(HHLastPeriodCnt>=3)
							{
								//LLLastPeriodCnt = 0;
								//Console.WriteLine("OnBar test raise up add ll point");
								//Console.WriteLine("OnBar test raise up HHLastPeriodCnt >3 {0} LLLastPeriodCnt{1}", HHLastPeriodCnt, LLLastPeriodCnt);
								if(LLLastPeriodCnt < 3)
								{
									LLLastPeriodCnt = 0;	
								}
								else
								{
									Console.WriteLine("OnBar test raise up not goto here");
									//ReversePoint.Add(LLDateTime, LL); 
									Log(LL, HLGroup,LLDateTime);
									
									
									LLLastPeriodCnt = 0;
								}
							}
						}
						//Console.WriteLine("OnBar 123112 start end ");
					}
					else if(barClose < barOpen)
					{
						
						//Console.WriteLine("OnBar 111123112 start ");
						if(HHLastPeriodCnt>=3)
						{
							if(barHigh > HH)
							{
								HH = barHigh;
								HHDateTime = bar.DateTime;
							}
							if(barLow<HL)
							{
								HL = barLow;
								LLLastPeriodCnt++;
								
								
								if(LLLastPeriodCnt >=3)
								{
									LL = barLow;
									LH = barHigh;
									LLDateTime = bar.DateTime;
									//Console.WriteLine("OnBar test fall down LLLastPeriodCnt >3 {0} HHLastPeriodCnt{1}", LLLastPeriodCnt, HHLastPeriodCnt);
									if(HHLastPeriodCnt < 3)
									{
										Console.WriteLine("OnBar test Fall down  not goto here last time {0}", HHLastPeriodCnt);
										HHLastPeriodCnt = 0;
									}
									else
									{
										//Console.WriteLine("OnBar test Fall down  mark HH point last time {0}", HHLastPeriodCnt);
										//ReversePoint.Add(HHDateTime, HH);
										Log( HH, HLGroup,HHDateTime);
										//Console.WriteLine("OnBar HH DateTime={0:yyyyMMdd HH:mm:ss.fff}, HH = {1}",HHDateTime,HH);
										
										HHLastPeriodCnt = 0;
									}
									
								}
							}
						}
						else if(barLow<LL)
						{
							//Console.WriteLine("OnBar test fall down <LL");
							LL = barLow;
							LH = barHigh;
							LLDateTime = bar.DateTime;
							LLLastPeriodCnt++;
							//Console.WriteLine("OnBar test fall down LLLastPeriodCnt {0}", LLLastPeriodCnt);
							if(LLLastPeriodCnt >=3)
							{
								if(HHLastPeriodCnt < 3)
								{
									HHLastPeriodCnt = 0;
								}
								if(HHLastPeriodCnt >=3)
								{
									Console.WriteLine("OnBar test fall not goto here astPeriodCnt >3 {0} HHLastPeriodCnt{1}", LLLastPeriodCnt, HHLastPeriodCnt);
									//ReversePoint.Add(HHDateTime, HH);
									Log(HH, HLGroup,HHDateTime);
									
									HHLastPeriodCnt = 0;		
								}
							}
						}
							
						//Console.WriteLine("OnBar end");
					}
					else  if(barClose == barOpen)
					{
						if(HHLastPeriodCnt>=3)
						{
							if(barHigh > HH)
							{
								HH = barHigh;
								HHDateTime = bar.DateTime;
							}	
						}
						if(LLLastPeriodCnt>=3)
						{
							if(barLow < LL)
							{
								LL = barLow;
								LLDateTime = bar.DateTime;
							}	
						}
					}
					
					
				}//end (bar.Close > 0) 
			}//end if (bar.Size == 60)
			

		}//end onBar
		
		private void AddGroups()
		{
			// Create bars group.
			bars1MinGroup = new Group("1MinBars");
			bars1MinGroup.Add("Pad", DataObjectType.String, 0);
			bars1MinGroup.Add("SelectorKey", Instrument.Symbol);
			bars1MinGroup.Add("ChartStyle", "Candle");
			bars1MinGroup.Add("CandleBlackColor", Color.Green);
			bars1MinGroup.Add("CandleWhiteColor", Color.Red);
			//barsGroup.Add("CandleColor", Color.Red);
			//barsGroup.Add("BarColor", Color.Red);
			//barsGroup.Add("CandleBorderColor", Color.White);
			
			bars5MinGroup = new Group("5MinBars");
			bars5MinGroup.Add("Pad", DataObjectType.String, 1);
			bars5MinGroup.Add("SelectorKey", Instrument.Symbol);
			bars5MinGroup.Add("ChartStyle", "Candle");
			bars5MinGroup.Add("CandleBlackColor", Color.Green);
			bars5MinGroup.Add("CandleWhiteColor", Color.Red);	
			// Create volume group.
			volumeGroup = new Group("Volume");
			volumeGroup.Add("Pad", 2);
			volumeGroup.Add("SelectorKey", Instrument.Symbol);
			volumeGroup.Add("Style", "Bar");
			
			//create reverse group
			HLGroup = new Group("HL");
			HLGroup.Add("Pad", 1);
			HLGroup.Add("SelectorKey", Instrument.Symbol);
			HLGroup.Add("Style", "Line");
			HLGroup.Add("Color", Color.Black);
			HLGroup.Add("Width", 4);

			// Create fills group.
			fillGroup = new Group("Fills");
			fillGroup.Add("Pad", 0);
			fillGroup.Add("SelectorKey", Instrument.Symbol);
			
			// Create sma5 values group.
			//sma5Group = new Group("SMA5");
			//sma5Group.Add("Pad", 0);
			//sma5Group.Add("SelectorKey", Instrument.Symbol);
			//sma5Group.Add("Color", Color.Green);

			// Create sma10 values group.
			//sma10Group = new Group("SMA10");
			//sma10Group.Add("Pad", 0);
			//sma10Group.Add("SelectorKey", Instrument.Symbol);
			//sma10Group.Add("Color", Color.Red);

			sma120Group = new Group("SMA120");
			sma120Group.Add("Pad", 0);
			sma120Group.Add("SelectorKey", Instrument.Symbol);
			sma120Group.Add("Color", Color.Red);
			
			#if TDEBUG
			sma26Group = new Group("SMA26");
			sma26Group.Add("Pad", 0);
			sma26Group.Add("SelectorKey", Instrument.Symbol);
			sma26Group.Add("Color", Color.White);
			
			bbl26Group = new Group("BBL26");
			bbl26Group.Add("Pad", 0);
			bbl26Group.Add("SelectorKey", Instrument.Symbol);
			bbl26Group.Add("Color", Color.Yellow);
			
			bbu26Group = new Group("BBU26");
			bbu26Group.Add("Pad", 0);
			bbu26Group.Add("SelectorKey", Instrument.Symbol);
			bbu26Group.Add("Color", Color.Blue);
			#endif
			


			
			// Create equity group.
			equityGroup = new Group("Equity");
			equityGroup.Add("Pad", 2);
			equityGroup.Add("SelectorKey", Instrument.Symbol);
			
			// Add groups to manager.
			GroupManager.Add(bars1MinGroup);
			GroupManager.Add(bars5MinGroup);
			
			GroupManager.Add(volumeGroup);
			GroupManager.Add(HLGroup);
			
			//GroupManager.Add(sma5Group);
			//GroupManager.Add(sma10Group);
			GroupManager.Add(sma120Group);
			//GroupManager.Add(sma26Group);
			//GroupManager.Add(bbl26Group);
			//GroupManager.Add(bbu26Group);
			GroupManager.Add(fillGroup);
			GroupManager.Add(equityGroup);
			
		}
	}
}






