using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
