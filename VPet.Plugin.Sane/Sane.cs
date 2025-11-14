using LinePutScript.Localization.WPF;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using VPet_Simulator.Windows.Interface;
using static VPet_Simulator.Core.GraphHelper;
using static VPet_Simulator.Core.WorkTimer;

namespace VPet.Plugin.Sane
{
#if DEBUG
	public struct ErrorHelper {
		/// <summary>
		/// 输出异常至消息框
		/// </summary>
		/// <param name="ex">异常</param>
		public static void OutputError(Exception ex) {
			string GetAllErrorMsg(Exception e) {
				string message =
@$"错误信息：{ex.Message}
堆栈跟踪：{ex.StackTrace}
异常类型：{ex.GetType().Name}
源：{ex.Source}

";
				if (ex.InnerException != null) {//递归获取内部异常信息
					message += "内部异常信息：\n";
					message += GetAllErrorMsg(ex.InnerException);
				}
				return message;
			}
			MessageBox.Show(GetAllErrorMsg(ex), "error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
#endif
	public class Sane:MainPlugin{
		public Sane(IMainWindow mainwin) : base(mainwin) {
		}
		public override string PluginName => "Sane";

		GdPanelAction gpa;
		readonly Data dat = new();
		Langs lang;

		public override void LoadPlugin() {
			lang = new(MW);
			dat.SaneValue = DataSave.SaneValue_Exist(MW)
					? (double)DataSave.SaneValue_Get(MW)
					: MW.Core.Save.Health;
			gpa = new(
				MW,
				new GdPanelAction.GdPanelItem() {
					text = new() {
						Text= lang.UI.progBarTitle,
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
			MW.Event_TakeItem += TakeItem;
			dat.OnSaneValueChanged += (v) => MW.Dispatcher.Invoke(/*调用UI线程*/() => gpa.ProgressBar_Change(MW, v));
			dat.OnSaneStatusChanged += SaneStatus_Change;
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
			{
				Random ran = new();
				switch (dat.SaneStatus) {
					case Data.SaneType.hight:
					case Data.SaneType.normal:
						if (ran.Next(10) == 0) {//十分之一的概率说话
							switch (dat.SaneStatus) {
								case Data.SaneType.hight:
									MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.hightSaneSay));
									break;
								case Data.SaneType.normal:
									MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.normalSaneSay));
									break;
							}
						}
						break;
					case Data.SaneType.low:
						if (ran.Next(2) == 0) {//二分之一的概率说话
							MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.lowSaneSay));
						}
						break;
					case Data.SaneType.danger:
						MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.dangerSaneSay));
						break;
				}
			}
		}
		void WorkEnd(FinishWorkInfo finishWorkInfo) {
			nowWork= null;
		}
		void TakeItem(Food food) {
			{
				double changv = food.Health * 0.5;
				while (Math.Abs(changv) > 10) {
					changv *= 0.1;//减小变化计算
				}
				dat.SaneValue += changv;
			}
		}
		void SaneStatus_Change(Data.SaneType s) {
			switch (s) {
				case Data.SaneType.hight:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.hightSaneSay));
					break;
				case Data.SaneType.normal:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.normalSaneSay));
					break;
				case Data.SaneType.low:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.lowSaneSay));
					break;
				case Data.SaneType.danger:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.dangerSaneSay));
					break;
			}
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
			else {
				dat.SaneValue += 0.01;//静止时缓慢恢复理智
			}
			if (dat.SaneStatus is Data.SaneType.low or Data.SaneType.danger) {
				MW.Core.Save.Health--;
				if (dat.SaneStatus == Data.SaneType.danger && MW.Core.Save.Health>20)
					MW.Core.Save.Health = 20;//理智值过低时强制降低健康值
				Random ran = new();
				switch (dat.SaneStatus) {
					case Data.SaneType.low:
						if(ran.Next(Math.Abs((int)(60 - 2 * MW.Set.LogicInterval))) == 0) {//函数y=a-2x, a=60
							MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.lowSaneSay));
						}
						break;
					case Data.SaneType.danger:
						if (ran.Next(Math.Abs((int)(50 - 4 * MW.Set.LogicInterval))) == 0) {//函数y=a-4x，a=50
							MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.dangerSaneSay));
						}
						break;
				}
			}
		}
	}
}
