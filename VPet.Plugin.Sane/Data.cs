using System;
using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.Sane {
	internal class Data {
		internal const double saneMax = 100;
		internal const double saneMin = 0;
		private double saneValue = saneMax;
		/// <summary>
		/// 理智值
		/// </summary>
		internal double SaneValue {
			get => saneValue;
			set { 
				saneValue = 
					value > saneMin
					? value <= saneMax
						? value 
						: saneMax
					: saneMin;
				SaneStatus_Judg();
				OnSaneValueChanged?.Invoke(saneValue);
				SaneTempChange_ForSaneValue();
			}
		}
		/// <summary>
		/// 理智值更改
		/// </summary>
		internal event Action<double> OnSaneValueChanged;
		internal enum SaneType {
			hight,normal,low,danger,
			//通过SaneTempValue达到目标状态后的状态
			hight_temp, normal_temp,low_temp, danger_temp,
		}
		internal event Action<SaneType> OnSaneStatusChanged;
		private SaneType saneStatus = SaneType.hight;
		private void SaneStatus_Set(SaneType value) {
			saneStatus = value;
			OnSaneStatusChanged?.Invoke(SaneStatus);
		}
		/// <summary>
		/// 当前理智状态
		/// </summary>
		internal SaneType SaneStatus => saneStatus;
		private void SaneStatus_Judg() {
			if (SaneValue + SaneTempValue > 90) {
				if (SaneValue > 90) {//无需药物即可达到该状态
					if (SaneStatus != SaneType.hight) {
						SaneStatus_Set(SaneType.hight);
					}
				}
				else {//依靠药物达到该状态
					if (SaneStatus!=SaneType.hight_temp) {
						SaneStatus_Set(SaneType.hight_temp);
					}
				}
			}
			else if (SaneValue + SaneTempValue > 50) {
				if (SaneValue > 50) {
					if (SaneStatus != SaneType.normal) {
						SaneStatus_Set(SaneType.normal);
					}
				}
				else {
					if (SaneStatus != SaneType.normal_temp) {
						SaneStatus_Set(SaneType.normal_temp);
					}
				}
			}
			else if (SaneValue + SaneTempValue > 20) {
				if (SaneValue > 20) {
					if (SaneStatus != SaneType.low) {
						SaneStatus_Set(SaneType.low);
					}
				}
				else {
					if(SaneStatus!=SaneType.low_temp) {
						SaneStatus_Set(SaneType.low_temp);
					}
				}
			}
			else {
				if (HaveSaneTemp) {
					if (SaneStatus != SaneType.danger_temp) {
						SaneStatus_Set(SaneType.danger_temp);
					}
				}
				else {
					if (SaneStatus != SaneType.danger) {
						SaneStatus_Set(SaneType.danger);
					}
				}
			}
		}

		private double saneTempValue = 0;
		/// <summary>
		/// 临时理智值，通过药物获得，将持续减小至0
		/// </summary>
		internal double SaneTempValue {
			get => saneTempValue;
			set {
				saneTempValue = 
					value > 0 
					? value + SaneValue <= saneMax 
						? value 
						: saneMax - SaneValue 
					: 0;
				SaneStatus_Judg();
			}
		}
		internal bool HaveSaneTemp => SaneTempValue > 0.1;//使用0.1作为判断依据，因为自减少函数每次减1%导致难以达到0
		/// <summary>
		/// 跟随SaneValue进行自我修正
		/// </summary>
		private void SaneTempChange_ForSaneValue() {
			if (SaneTempValue + SaneValue > saneMax) 
				saneTempValue = saneMax - SaneValue;
		}
		/// <summary>
		/// 自我减少函数，可根据实际使用调用<br/>
		/// 每调用一次根据算法减少一定的值
		/// </summary>
		internal void SaneTempValue_SelfReduce() => SaneTempValue -= 0.1 * SaneTempValue;
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
