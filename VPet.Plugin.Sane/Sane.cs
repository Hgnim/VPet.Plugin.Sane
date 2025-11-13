using System;
using System.Timers;
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
		Lang lan = new();

		public override void LoadPlugin() {
			lan = Lang.ReadData(Lang.ReadData("lang.yml").UseLanguage);
			MW.Main.Say(MW.Set.Language);
			dat.SaneValue = DataSave.SaneValue_Exist(MW)
					? (double)DataSave.SaneValue_Get(MW)
					: MW.Core.Save.Health;
			gpa = new(
				MW,
				new GdPanelAction.GdPanelItem() {
					text = new() {
						Text= lan.Language.UserInterface.ProgBarTitle,
					},
					progressBar = new() {
						Value=dat.SaneValue,
						Maximum=100,
					},
				}, 
				MW.Main.ToolBar.gdPanel
				);//初始化

			MW.Main.Event_WorkStart += WorkStart;
			MW.Main.Event_WorkEnd += WorkEnd;
			MW.Main.EventTimer.Elapsed += TickElapsed;
			dat.OnSaneValueChanged += (v) => MW.Dispatcher.Invoke(/*调用UI线程*/() => gpa.ProgressBar_Change(MW, v));
		}
		void SaveData() {
			DataSave.SaneValue_Set(MW, dat.SaneValue);
		}
		void SaveConfig() { }
		public override void Save() {
			SaveData();
			base.Save();
		}
		public override void EndGame() {
			SaveData();
			base.EndGame();
		}

		Work nowWork = null;
		void WorkStart(Work work) {
			nowWork= work;
		}
		void WorkEnd(FinishWorkInfo finishWorkInfo) {
			nowWork= null;
		}
		/// <summary>
		/// 游戏每个tick的调用
		/// </summary>
		void TickElapsed(object sender,ElapsedEventArgs e) {
			if(nowWork != null) {
				/*switch (nowWork) {
					case Work.WorkType.Work:
					case Work.WorkType.Study:
						break;
					case Work.WorkType.Play:
						break;
				}*/
				double changv=-nowWork.Feeling * 0.5;
				while (Math.Abs(changv) > 0.7) {
					changv *= 0.1;//减小变化计算
				}
				dat.SaneValue += changv;
			}
		}
	}
}
