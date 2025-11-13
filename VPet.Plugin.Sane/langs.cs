using LinePutScript.Localization.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPet.Plugin.Sane {
	internal readonly struct Langs {
		internal readonly struct UI {
			internal readonly static string progBarTitle = "理智".Translate();
		}
		internal readonly struct Speak {
			internal readonly static string[] hightSaneSay = [
					"我感觉精神饱满！".Translate(),
					"我现在感觉很清醒！".Translate(),
					"头脑清晰，思路敏捷！".Translate(),
					"我感觉自己现在很有干劲！".Translate(),
				];
			internal readonly static string[] normalSaneSay = [
					"我感觉还不错。".Translate(),
					"一切都很正常。".Translate(),
					"我觉得现在挺好的。".Translate(),
				];
			internal readonly static string[] lowSaneSay = [
					"我有点迷糊了。".Translate(),
					"感觉有些不对劲。".Translate(),
					"我可能需要休息一下。".Translate(),
					"让我休息一下吧。".Translate(),
					"让我玩耍一下，这样可能会让我好些。".Translate(),
				];
			internal readonly static string[] dangerSaneSay = [
					"我感觉非常糟糕！".Translate(),
					"头脑一片混乱！".Translate(),
					"VJ$M(#*9a8o|Be/;kljma23brsbIvw>os<$V}".Translate(),
					"我是谁？！我现在在哪？？".Translate(),
					"我觉得我快不行了。。".Translate(),
					"我觉得我需要药品干预。".Translate(),
				];
			internal static string GetSpeakRan(string[] say) => say[new Random().Next(say.Length)];
		}
	}
}
