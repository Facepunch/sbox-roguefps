public struct PageCameraSetup
{
	[KeyProperty] public string Url { get; set; }
	[KeyProperty] public GameObject CameraObject { get; set; }

	public float PositionLerpSpeed { get; set; }
	public float RotationLerpSpeed { get; set; }
}
