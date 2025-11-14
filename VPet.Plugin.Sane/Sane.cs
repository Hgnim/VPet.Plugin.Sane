using LinePutScript.Localization.WPF;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
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
			dat.OnSaneValueChanged += SaneValue_Change;
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
			nowWork = work;
			{
				Random ran = new();
				switch (dat.SaneStatus) {
					case Data.SaneType.hight:
					case Data.SaneType.normal:
						if (ran.Next(10) == 0)//十分之一的概率说话
							SaneStatus_Change(dat.SaneStatus);
						break;
					case Data.SaneType.low:
						if (ran.Next(2) == 0)//二分之一的概率说话
							SaneStatus_Change(dat.SaneStatus);
						break;
					case Data.SaneType.danger:
						SaneStatus_Change(dat.SaneStatus);
						break;
				}
			}
		}
		void WorkEnd(FinishWorkInfo finishWorkInfo) {
			nowWork= null;
		}
		void TakeItem(Food food) {
			if(food.Health>0)
				dat.SaneTempValue += food.Health * 0.6;//通过食物恢复些许理智
		}
		void SaneValue_Change(double value) =>
			//调用UI线程
			MW.Dispatcher.Invoke(() => 
				gpa.ProgressBar_Change(MW, value+dat.SaneTempValue, changeForeground: () => {
					if (dat.HaveSaneTemp)
						gpa.ChangeForeground(MW, GdPanelAction.GetForeground(MW, dat.SaneValue / Data.saneMax));
					else
						gpa.ChangeForeground(MW);
				})
			);
		void SaneStatus_Change(Data.SaneType s) {
			switch (s) {
				case Data.SaneType.hight:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.hightSaneSay));
					break;
				case Data.SaneType.hight_temp:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.hightTempSaneSay));
					break;
				case Data.SaneType.normal:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.normalSaneSay));
					break;
				case Data.SaneType.normal_temp:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.normalTempSaneSay));
					break;
				case Data.SaneType.low:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.lowSaneSay));
					break;
				case Data.SaneType.low_temp:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.lowTempSaneSay));
					break;
				case Data.SaneType.danger:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.dangerSaneSay));
					break;
				case Data.SaneType.danger_temp:
					MW.Main.Say(Langs.SpeakC.GetSpeakRan(lang.Speak.dangerTempSaneSay));
					break;
			}
		}
		/// <summary>
		/// 游戏每个tick的调用
		/// </summary>
		void TickElapsed(object sender,ElapsedEventArgs e) {
			if(nowWork != null) {
				switch (nowWork.Type) {
					case Work.WorkType.Work:
					case Work.WorkType.Study: {
							double changv = -nowWork.Feeling * 0.8;
							while (Math.Abs(changv) > 0.2) {
								changv *= 0.3;//减小变化计算
							}
							dat.SaneValue += changv;
						}
						break;
					case Work.WorkType.Play: {
							double changv = -nowWork.Feeling;
							while (Math.Abs(changv) > 1) {
								changv *= 0.5;
							}
							dat.SaneValue += changv;
						}
						break;
				}
			}
			else if (MW.Main.State == VPet_Simulator.Core.Main.WorkingState.Sleep) {
				dat.SaneValue += 1.2;//睡觉时恢复理智
			}
			else {
				dat.SaneValue += 0.01;//静止时缓慢恢复理智
			}
			if (dat.SaneStatus is Data.SaneType.low or Data.SaneType.low_temp or Data.SaneType.danger or Data.SaneType.danger_temp) {
				MW.Core.Save.Health -= (50 - dat.SaneValue)*0.06;//理智值越低，健康值下降越快
				if (
					(dat.SaneStatus is Data.SaneType.danger or Data.SaneType.danger_temp) && 
					MW.Core.Save.Health>40
					)
					MW.Core.Save.Health = 40;//理智值过低时强制降低健康值
				Random ran = new();
				switch (dat.SaneStatus) {
					case Data.SaneType.low:
					case Data.SaneType.low_temp:
						if(ran.Next(Math.Abs((int)(60 - 2 * MW.Set.LogicInterval))) == 0)//函数y=a-2x, a=60
							SaneStatus_Change(dat.SaneStatus);
						break;
					case Data.SaneType.danger:
					case Data.SaneType.danger_temp:
						if (ran.Next(Math.Abs((int)(50 - 4 * MW.Set.LogicInterval))) == 0)//函数y=a-4x，a=50
							SaneStatus_Change(dat.SaneStatus);
						break;
				}
			}
			//通过药物恢复的理智将根据时间逐渐减少
			if (dat.HaveSaneTemp)
				dat.SaneTempValue_SelfReduce();
		}
	}
}
