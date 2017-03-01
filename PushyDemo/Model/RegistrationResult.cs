using System;
using ME.Pushy.Sdk.Util.Exceptions;

namespace PushyDemo
{
	public class RegistrationResult
	{
		public String DeviceToken { get; set; }
		public PushyException Error { get; set; }
	}
}
