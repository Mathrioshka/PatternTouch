using System.Linq;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	[PluginInfo(Name = "Touch", Category = "PatternTouch", Version = "Gesture", Help = "Detects when you touching object", Tags = "multitouch")]
	public class TouchGestureNode : TransformProcessor
	{
		[Input("Number Of Fingers", DefaultValue = 1, Visibility = PinVisibility.OnlyInspector)] 
		public ISpread<int> MinFingersIn;
		
		[Output("Touched")] 
		public ISpread<bool> TouchedOutput;

		protected override void Reinit(int index)
		{
			TransformStates.Add(new TransformState(IdIn[index], new Vector2D()));
		}

		protected override void Reset(int index)
		{
			TransformStates[index].Reset(IdIn[index], 0);
		}

		protected override void OutputData(int spreadMax)
		{
			TouchedOutput.SliceCount = spreadMax;

			for (var i = 0; i < spreadMax; i++)
			{
                TouchedOutput[i] = (TransformStates[i].Phase == TransformPhase.Transforming) && TransformStates[i].Blobs.SliceCount >= MinFingersIn[i];    
			}
		}
	}

    [PluginInfo(Name = "Hover", Category = "PatternTouch", Version = "Gesture", Help = "Detects when you hover the object", Tags = "multitouch")]
    public class HoverGestureNode : IPluginEvaluate
    {
        [Input("ID")]
        protected IDiffSpread<int> IdIn;

        [Input("Blobs")]
        protected ISpread<Blob> BlobIn;

        [Input("Number Of Fingers", DefaultValue = 1, Visibility = PinVisibility.OnlyInspector)]
        public ISpread<int> MinFingersIn;

        [Input("Single Touch", DefaultValue = 1, Visibility = PinVisibility.OnlyInspector)] 
        public ISpread<bool> SingleTouchIn;
        
        [Output("Hovered")]
        public ISpread<bool> HoveredOutput;

        public void Evaluate(int spreadMax)
        {
            var maxIter = BlobIn.Any() ? spreadMax : IdIn.SliceCount;
            HoveredOutput.SliceCount = IdIn.SliceCount;

            for (var i = 0; i < maxIter; i++)
            {
                var id = IdIn[i];

                bool hovered;
                if (SingleTouchIn[0] && BlobIn.Any())
                {
                    hovered = id == BlobIn[0].HitId;
                }
                else
                {
                    hovered = BlobIn.Any(blob => blob.HitId == id);
                }

                HoveredOutput[i] = hovered;
            }
        }
    }
}
