#region usings
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.Streams;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	[PluginInfo(Name = "IdGenerator", Category = "PatternTouch", Help = "Basic template with a dynamic amount of in- and outputs", Tags = "")]
	public class PatternTouchIdGeneratorNode : IPluginEvaluate, IPartImportsSatisfiedNotification
	{
		public Spread<IIOContainer<ISpread<Matrix4x4>>> FInputs = new Spread<IIOContainer<ISpread<Matrix4x4>>>();
		public Spread<IIOContainer<ISpread<int>>> FOutputs = new Spread<IIOContainer<ISpread<int>>>();

		[Config("Input Count", DefaultValue = 1, MinValue = 0)]
		public IDiffSpread<int> FInputCountIn;

		[Import()]
		public IIOFactory FIOFactory;
		
		[Import()]
        public ILogger FLogger;

		public void OnImportsSatisfied()
		{
			FInputCountIn.Changed += HandleInputCountChanged;
		}

		private void HandlePinCountChanged<T>(ISpread<int> countSpread, Spread<IIOContainer<T>> pinSpread, Func<int, IOAttribute> ioAttributeFactory) where T : class
		{
			pinSpread.ResizeAndDispose(countSpread[0], i =>
			{
				var ioAttribute = ioAttributeFactory(i + 1);
				return FIOFactory.CreateIOContainer<T>(ioAttribute);
			});
		}

		private void HandleInputCountChanged(IDiffSpread<int> sender)
		{
			HandlePinCountChanged(sender, FInputs, i => new InputAttribute(string.Format("Input {0}", i)));
			HandlePinCountChanged(sender, FOutputs, i => new OutputAttribute(string.Format("Output {0}", i)));
		}

		// Called when data for any output pin is requested.
		public void Evaluate(int SpreadMax)
		{
			var pValue = 0;
			for (int i = 0; i < FInputCountIn[0]; i++) {
				var inputSpread = FInputs[i].IOObject;
				var outputSpread = FOutputs[i].IOObject;
				
				var indexes = new List<int>();
				for(var j = 0; j < inputSpread.SliceCount; j++) {
					indexes.Add(j + pValue);
				}
				
				outputSpread.AssignFrom(indexes);
				
				pValue = indexes.Last() + 1;
			}
		}
	}
}
