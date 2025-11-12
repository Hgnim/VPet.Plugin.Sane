using LinePutScript.Localization.WPF;
using Panuon.WPF.UI;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VPet_Simulator.Windows.Interface;
using static VPet_Simulator.Core.GraphHelper;
using static VPet_Simulator.Core.WorkTimer;

namespace VPet.Plugin.Sane
{
    public class Sane:MainPlugin{
		public Sane(IMainWindow mainwin) : base(mainwin) {
		}
		public override string PluginName => "Sane";

		GdPanelAction gpa;
		readonly Data dat = new();

		public override void LoadPlugin() {
			dat.SaneValue = MW.Core.Save.Health;//目前的理智值初始值为从进入游戏时的健康值开始，后续将为其添加存档支持
			gpa = new(
				new GdPanelAction.GdPanelItem() {
					text = new() {
						Text= "理智",//.Translate();
					},
					progressBar = new() {
						Value=dat.SaneValue,
						Maximum=100,
					},
				}, 
				MW.Main.ToolBar.gdPanel);//初始化

			MW.Main.Event_WorkStart += WorkStart;
			MW.Main.Event_WorkEnd += WorkEnd;
			MW.Main.EventTimer.Elapsed += TickElapsed;
			dat.OnSaneValueChanged += (v) => MW.Dispatcher.Invoke(/*调用UI线程*/() => gpa.ProgressBar_Change(MW, v));
		}

		Work nowWork = null;
		void WorkStart(Work work) {
			nowWork= work;
			//MW.Main.Say($"开始工作{nowWork}");
		}
		void WorkEnd(FinishWorkInfo finishWorkInfo) {
			nowWork= null;
			//MW.Main.Say($"结束工作{nowWork}");
		}
		/// <summary>
		/// 游戏每个tick的调用
		/// </summary>
		void TickElapsed(object sender,ElapsedEventArgs e) {
			try {
				
				if(nowWork != null) {
					/*switch (nowWork) {
						case Work.WorkType.Work:
						case Work.WorkType.Study:
							break;
						case Work.WorkType.Play:
							break;
					}*/
					dat.SaneValue += -nowWork.Feeling * 0.5; //(nowWork != null) ? -nowWork.Feeling * 0.5 : 0;
				}
			} catch (Exception ex){ MW.Main.Say(ex.Message); }
		}
	}
}
