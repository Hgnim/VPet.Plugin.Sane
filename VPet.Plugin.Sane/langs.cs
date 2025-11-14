using LinePutScript.Localization.WPF;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.Sane {
	internal class Langs {
		/// <summary>
		/// 委托UiC的构造函数
		/// </summary>
		private static Func<UiC> UiC_func;
		internal class UiC {
			internal readonly string progBarTitle;
			static UiC() => UiC_func = () => new UiC();
			private UiC() {
				progBarTitle = "ui_progBarTitle".Translate();
			}
		}
		private readonly UiC ui;
		internal UiC UI => ui;
		/// <summary>
		/// 委托SpeakC的构造函数，使得SpeakC的构造函数可以为私有
		/// </summary>
		private static Func<IMainWindow,SpeakC> speakC_func;
		internal class SpeakC {
			internal readonly ClickText[] hightSaneSay;
			internal readonly ClickText[] normalSaneSay;
			internal readonly ClickText[] lowSaneSay;
			internal readonly ClickText[] dangerSaneSay;
			static SpeakC() => speakC_func = (MW) => new SpeakC(MW);
			private SpeakC(IMainWindow MW) {
				hightSaneSay = [.. MW.ClickTexts.FindAll(x => x.Working == "sepak_hightSaneSay")];//List<ClickText>.ToArray();
				normalSaneSay = [.. MW.ClickTexts.FindAll(x => x.Working == "sepak_normalSaneSay")];
				lowSaneSay = [.. MW.ClickTexts.FindAll(x => x.Working == "sepak_lowSaneSay")];
				dangerSaneSay = [.. MW.ClickTexts.FindAll(x => x.Working == "sepak_dangerSaneSay")];
			}
			internal static string GetSpeakRan(ClickText[] say) => say[new Random().Next(say.Length)].TranslateText;
		}
		private readonly SpeakC speak;
		internal SpeakC Speak => speak;

		/// <summary>
		/// 初始化语言类
		/// </summary>
	 	internal Langs(IMainWindow MW) {
			//挂起并等待UiC类的静态构造函数执行完成
			RuntimeHelpers.RunClassConstructor(typeof(UiC).TypeHandle);
			ui = UiC_func();
			RuntimeHelpers.RunClassConstructor(typeof(SpeakC).TypeHandle);
			speak = speakC_func(MW);
		}
	}
}
