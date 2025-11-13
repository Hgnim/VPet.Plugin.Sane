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
				saneValue = 
					value > 0 
					? value <= 100
						? value 
						: 100
					: 0;
				if (saneValue > 90) {
					if (saneStatus != SaneType.hight) {
						saneStatus = SaneType.hight;
						OnSaneStatusChanged?.Invoke(SaneStatus);
					}
				}
				else if (saneValue > 50) {
					if (saneStatus != SaneType.normal) {
						saneStatus = SaneType.normal;
						OnSaneStatusChanged?.Invoke(SaneStatus);
					}
				}
				else if (saneValue > 20) {
					if (saneStatus != SaneType.low) {
						saneStatus = SaneType.low;
						OnSaneStatusChanged?.Invoke(SaneStatus);
					}
				}
				else {
					if (saneStatus != SaneType.danger) {
						saneStatus = SaneType.danger;
						OnSaneStatusChanged?.Invoke(SaneStatus);
					}
				}
				OnSaneValueChanged?.Invoke(saneValue);
			}
		}
		/// <summary>
		/// 理智值更改
		/// </summary>
		internal event Action<double> OnSaneValueChanged;
		internal enum SaneType {
			hight,normal,low,danger
		}
		internal event Action<SaneType> OnSaneStatusChanged;
		private SaneType saneStatus = SaneType.hight;
		/// <summary>
		/// 当前理智状态
		/// </summary>
		internal SaneType SaneStatus => saneStatus;
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
