using System.IO;//此处调用IO仅仅是为了读取语言文件
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace VPet.Plugin.Sane {
	public class Lang {
		/// <summary>
		/// 读取数据文件并将数据引入实例中
		/// </summary>
		/// <param name="configFile">配置文件路径</param>
		internal static Lang ReadData(string configFile) => 
			new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build()
				.Deserialize<Lang>(File.ReadAllText(configFile));

		public class LanguageC {
			public class SpeakC {
				private string[] hightSaneSay = [];
				public string[] HightSaneSay {
					get => hightSaneSay;
					set => hightSaneSay = value;
				}

				private string[] normalSaneSay = [];
				public string[] NormalSaneSay {
					get => normalSaneSay;
					set => normalSaneSay = value;
				}

				private string[] lowSaneSay = [];
				public string[] LowSaneSay {
					get => lowSaneSay;
					set => lowSaneSay = value;
				}

				private string[] dangerSaneSay = [];
				public string[] DangerSaneSay {
					get => dangerSaneSay;
					set => dangerSaneSay = value;

				}
			}
			private SpeakC speak = new();
			public SpeakC Speak {
				get => speak;
				set => speak = value;
			}
			public class UserInterfaceC {
				private string progBarTitle;
				public string ProgBarTitle {
					get => progBarTitle;
					set => progBarTitle = value;
				}
			}
			private UserInterfaceC userInterface = new();
			public UserInterfaceC UserInterface {
				get => userInterface;
				set => userInterface = value;
			}
		}
		private LanguageC language = new();
		public LanguageC Language {
			get => language;
			set => language = value;
		}
		private string useLanguage;
		public string UseLanguage {
			get => useLanguage;
			set => useLanguage = value;
		}
	}
}
