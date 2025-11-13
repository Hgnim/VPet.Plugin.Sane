using System;
using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.Sane {
	internal class Data {
		private double saneValue = 100;
		/// <summary>
		/// 理智值
		/// </summary>
		internal double SaneValue {
			get => saneValue;
			set { 
				saneValue = value > 0 ? value : 0; 
				OnSaneValueChanged?.Invoke(saneValue);
			}
		}
		internal event Action<double> OnSaneValueChanged;
	}
	internal struct DataSave {
		const string mainKey = "Sane";

		const string svKey = "SaneValue";
		internal static double? SaneValue_Get(IMainWindow MW) =>
			MW.GameSavesData[mainKey][(LinePutScript.gdbe)svKey];
		internal static void SaneValue_Set(IMainWindow MW, double value) =>
			MW.GameSavesData[mainKey][(LinePutScript.gdbe)svKey] = value;
		internal static bool SaneValue_Exist(IMainWindow MW) =>
			!string.IsNullOrEmpty(MW.GameSavesData[mainKey].GetString(svKey));
	}
}
